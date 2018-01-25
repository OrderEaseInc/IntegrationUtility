using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LinkGreenODBCUtility
{
    public partial class UtilityDashboard : Form
    {
        public static Mapping Mapping = new Mapping();

        public UtilityDashboard()
        {
            InitializeComponent();
        }

        private void UtilityDashboard_Load(object sender, EventArgs e)
        {
            Settings.Init();

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["ApiKey"].Value = Settings.GetApiKey();
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");

            if (!string.IsNullOrEmpty(config.AppSettings.Settings["ApiKey"].Value))
            {
                Settings.SetupUserConfig(config.AppSettings.Settings["ApiKey"].Value);
            }

            var Tasks = new Tasks();
            Tasks.RestoreTasks();

            Log.PurgeLog();
        }

        private void UtilityDashboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show($"Tasks will not execute while the Integration Utility is closed. Are you sure you want to exit?", "Are you sure?", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                JobManager.Dispose();
            }

            e.Cancel = dialogResult == DialogResult.No;
        }

        private void settingsMappingMenuItem_Click(object sender, EventArgs e)
        {
            UtilityMappings utilitySettings = new UtilityMappings();
            utilitySettings.ShowDialog();
        }

        private void syncCategories_Click(object sender, EventArgs e)
        {
            var categories = new Categories();
            categories.UpdateTemporaryTables();
            categories.Empty();

            string mappedDsnName = Mapping.GetDsnName("Categories");
            var newMapping = new Mapping(mappedDsnName);
            if (newMapping.MigrateData("Categories"))
            {
                if (categories.Publish())
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                    Logger.Instance.Info("Categories synced.");
                }
                else
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                    Logger.Instance.Error("Categories failed to sync.");
                }
            }
            else
            {
                if (!newMapping._validFields)
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                }
                else
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                    Logger.Instance.Error("Categories failed to migrate.");
                }
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UtilitySettings utilitySettings = new UtilitySettings();
            utilitySettings.ShowDialog();
        }

        private void eventLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EventLog eventLog = new EventLog();
            eventLog.ShowDialog();
        }

        private void SetButtonState(bool enabled) {
            syncCategories.Enabled = enabled;
            syncCustomers.Enabled = enabled;
            syncPriceLevels.Enabled = enabled;
            syncPricing.Enabled = enabled;
            syncProducts.Enabled = enabled;
        }

        private void syncCustomers_Click(object sender, EventArgs e)
        {
            var bw = new BackgroundWorker();
            lblStatus.Text = "Processing customer sync (Preparing)\n\rPlease wait";

            SetButtonState(false);
            bw.DoWork += (bwSender, bwEventArgs) =>
            {
                var customers = new Customers();
                customers.UpdateTemporaryTables();
                customers.Empty();

                string mappedDsnName = Mapping.GetDsnName("Customers");
                var newMapping = new Mapping(mappedDsnName);
                if (newMapping.MigrateData("Customers"))
                {
                    ((BackgroundWorker)bwSender).ReportProgress(0, "Processing customer sync (Pushing)\n\rPlease wait");
                    if (customers.Publish((BackgroundWorker)bwSender))
                    {
                        bwEventArgs.Result = new
                        {
                            Message = "Customers Synced",
                            Title = "Success",
                            Error = string.Empty,
                            InfoMessage = string.Empty
                        };
                    }
                    else
                    {
                        bwEventArgs.Result = new
                        {
                            Message = "Customers failed to sync. Do you have your API Key set?",
                            Title = "Sync Failure",
                            Error = "Customers failed to sync.",
                            InfoMessage = string.Empty
                        };
                    }
                }
                else
                {
                    if (!newMapping._validFields)
                    {
                        bwEventArgs.Result = new
                        {
                            Message = "All required fields indicated with a * must be mapped.",
                            Title = "Map Required Fields",
                            Error = string.Empty,
                            InfoMessage = string.Empty
                        };
                    }
                    else
                    {
                        bwEventArgs.Result = new
                        {
                            Message = "Customers failed to migrate.",
                            Title = "Unknown Error",
                            Error = "Customers failed to migrate.",
                            InfoMessage = string.Empty
                        };
                    }
                }
            };

            bw.ProgressChanged += (bwSender, changedEventArgs) =>
            {
                lblStatus.Text = changedEventArgs.UserState.ToString();
            };

            bw.RunWorkerCompleted += (bwSender, completedEventArgs) => {
                SetButtonState(true);
                dynamic info = completedEventArgs.Result;

                lblStatus.Text = info.Message;
                MessageBox.Show(info.Message, info.Title);
                if (!string.IsNullOrWhiteSpace(info.Error))
                    Logger.Instance.Error(info.Error);
                if (!string.IsNullOrWhiteSpace(info.InfoMessage))
                    Logger.Instance.Info(info.InfoMessage);
            };

            bw.WorkerReportsProgress = true;
            bw.RunWorkerAsync();
        }

        private void syncProducts_Click(object sender, EventArgs e)
        {
            var products = new Products();
            products.UpdateTemporaryTables();
            products.Empty();

            string mappedDsnName = Mapping.GetDsnName("Products");
            var newMapping = new Mapping(mappedDsnName);
            if (newMapping.MigrateData("Products"))
            {
                if (products.Publish())
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                    Logger.Instance.Info("Products synced.");
                }
                else
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                    Logger.Instance.Error("Products failed to sync.");
                }
            }
            else
            {
                if (!newMapping._validFields)
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                }
                else
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                    Logger.Instance.Error("Products failed to migrate.");
                }
            }
        }

        private void syncPriceLevels_Click(object sender, EventArgs e)
        {
            var priceLevels = new PriceLevels();
            priceLevels.UpdateTemporaryTables();
            priceLevels.Empty();

            string mappedDsnName = Mapping.GetDsnName("PriceLevels");
            var newMapping = new Mapping(mappedDsnName);
            if (newMapping.MigrateData("PriceLevels"))
            {
                if (priceLevels.Publish())
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                    Logger.Instance.Info("Price Levels synced.");
                }
                else
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                    Logger.Instance.Error("Price Levels failed to sync.");
                }
            }
            else
            {
                if (!newMapping._validFields)
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                }
                else
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                    Logger.Instance.Error("Price Levels failed to migrate.");
                }
            }
        }

        private void syncPricing_Click(object sender, EventArgs e)
        {
            var priceLevelPrices = new PriceLevelPrices();
            priceLevelPrices.UpdateTemporaryTables();
            priceLevelPrices.Empty();

            string mappedDsnName = Mapping.GetDsnName("PriceLevelPrices");
            var newMapping = new Mapping(mappedDsnName);
            if (newMapping.MigrateData("PriceLevelPrices"))
            {
                if (priceLevelPrices.Publish())
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                    Logger.Instance.Info("Pricing synced.");
                }
                else
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                    Logger.Instance.Error("Pricing failed to sync.");
                }
            }
            else
            {
                if (!newMapping._validFields)
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                }
                else
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                    Logger.Instance.Error("Pricing failed to migrate.");
                }
            }
        }

        private void taskManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var TaskManager = new TaskManager();
            TaskManager.ShowDialog();
        }
    }
}
