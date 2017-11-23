using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
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
                    MessageBox.Show("Categories Synced", "Success");
                    Logger.Instance.Info("Categories synced.");
                }
                else
                {
                    MessageBox.Show("Categories failed to sync. Do you have your API Key set?", "Sync Failure");
                    Logger.Instance.Error("Categories failed to sync.");
                }
            }
            else
            {
                if (!newMapping._validFields)
                {
                    MessageBox.Show("All required fields indicated with a * must be mapped.", "Map Required Fields");
                }
                else
                {
                    MessageBox.Show("Categories failed to migrate.", "Unknown Error");
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

        private void syncCustomers_Click(object sender, EventArgs e)
        {
            var customers = new Customers();
            customers.UpdateTemporaryTables();
            customers.Empty();
            
            string mappedDsnName = Mapping.GetDsnName("Customers");
            var newMapping = new Mapping(mappedDsnName);
            if (newMapping.MigrateData("Customers"))
            {
                if (customers.Publish())
                {
                    MessageBox.Show("Customers Synced", "Success");
                    Logger.Instance.Info("Customers synced.");
                }
                else
                {
                    MessageBox.Show("Customers failed to sync. Do you have your API Key set?", "Sync Failure");
                    Logger.Instance.Error("Customers failed to sync.");
                }
            }
            else
            {
                if (!newMapping._validFields)
                {
                    MessageBox.Show("All required fields indicated with a * must be mapped.", "Map Required Fields");
                }
                else
                {
                    MessageBox.Show("Customers failed to migrate.", "Unknown Error");
                    Logger.Instance.Error("Customers failed to migrate.");
                }
            }
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
                    MessageBox.Show("Products Synced", "Success");
                    Logger.Instance.Info("Products synced.");
                }
                else
                {
                    MessageBox.Show("Products failed to sync. Do you have your API Key set?", "Sync Failure");
                    Logger.Instance.Error("Products failed to sync.");
                }
            }
            else
            {
                if (!newMapping._validFields)
                {
                    MessageBox.Show("All required fields indicated with a * must be mapped.", "Map Required Fields");
                }
                else
                {
                    MessageBox.Show("Products failed to migrate.", "Unknown Error");
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
                    MessageBox.Show("Price Levels Synced", "Success");
                    Logger.Instance.Info("Price Levels synced.");
                }
                else
                {
                    MessageBox.Show("Price Levels failed to sync. Do you have your API Key set?", "Sync Failure");
                    Logger.Instance.Error("Price Levels failed to sync.");
                }
            }
            else
            {
                if (!newMapping._validFields)
                {
                    MessageBox.Show("All required fields indicated with a * must be mapped.", "Map Required Fields");
                }
                else
                {
                    MessageBox.Show("Price Levels failed to migrate.", "Unknown Error");
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
                    MessageBox.Show("Pricing Synced", "Success");
                    Logger.Instance.Info("Pricing synced.");
                }
                else
                {
                    MessageBox.Show("Pricing failed to sync. Do you have your API Key set?", "Sync Failure");
                    Logger.Instance.Error("Pricing failed to sync.");
                }
            }
            else
            {
                if (!newMapping._validFields)
                {
                    MessageBox.Show("All required fields indicated with a * must be mapped.", "Map Required Fields");
                }
                else
                {
                    MessageBox.Show("Pricing failed to migrate.", "Unknown Error");
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
