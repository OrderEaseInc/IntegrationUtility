﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows.Forms;
using System.ComponentModel;
using LinkGreen.Email;
using LinkGreenODBCUtility.RunnableActivities;
// ReSharper disable LocalizableElement

// ReSharper disable once CheckNamespace
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

            var tasks = new Tasks();
            tasks.RestoreTasks();

            Log.PurgeLog();
        }

        private void UtilityDashboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            var dialogResult =
                MessageBox.Show(
                    @"Tasks will not execute while the Integration Utility is closed. Are you sure you want to exit?",
                    @"Are you sure?", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                JobManager.Dispose();
            }

            e.Cancel = dialogResult == DialogResult.No;
        }

        private void settingsMappingMenuItem_Click(object sender, EventArgs e)
        {
            var utilitySettings = new UtilityMappings();
            utilitySettings.ShowDialog();
        }

        private void syncCategories_Click(object sender, EventArgs e)
        {
            var notificationEmail = Settings.GetNotificationEmail();
            var categories = new Categories();
            categories.UpdateTemporaryTables();
            categories.Empty();

            var mappedDsnName = Mapping.GetDsnName("Categories");
            var newMapping = new Mapping(mappedDsnName);
            if (newMapping.MigrateData("Categories"))
            {
                if (categories.Publish(out var messageDetails))
                {
                    MessageBox.Show(Resources.Text.Resource_en_US.Categories_have_been_successfully_published, Resources.Text.Resource_en_US.Published_Successfully);
                    Logger.Instance.Info("Categories synced.");

                    if (!string.IsNullOrWhiteSpace(notificationEmail))
                        Mail.SendProcessCompleteEmail(notificationEmail, messageDetails, "Category Publish",
                            response => Logger.Instance.Info(response));
                }
                else
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                    Logger.Instance.Error("Categories failed to sync.");
                    if (!string.IsNullOrWhiteSpace(notificationEmail))
                        Mail.SendProcessCompleteEmail(notificationEmail, messageDetails, "Category Publish",
                            response => Logger.Instance.Info(response));
                }
            }
            else
            {
                if (!newMapping._validFields)
                {
                    MessageBox.Show(@"Mapping contains invalid fields", @"Migration Failed");
                    if (!string.IsNullOrWhiteSpace(notificationEmail))
                        Mail.SendProcessCompleteEmail(notificationEmail, "Mapping contains invalid fields", "Category Publish",
                            response => Logger.Instance.Info(response));
                    Logger.Instance.Error("Categories failed to migrate - Invalid Fields.");
                }
                else
                {
                    MessageBox.Show(@"There was an unexpected error.\n\r\n\rPlease contact support", @"Migration Failed");
                    if (!string.IsNullOrWhiteSpace(notificationEmail))
                        Mail.SendProcessCompleteEmail(notificationEmail, "Unknown Error", "Category Publish",
                            response => Logger.Instance.Info(response));
                    Logger.Instance.Error("Categories failed to migrate.");
                }
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var utilitySettings = new UtilitySettings { Owner = this };
            utilitySettings.ShowDialog();
        }

        private void eventLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var eventLog = new EventLog();
            eventLog.ShowDialog();
        }

        private void SetButtonState(bool enabled)
        {
            syncCategories.Enabled = enabled;
            syncCustomers.Enabled = enabled;
            syncPriceLevels.Enabled = enabled;
            syncPricing.Enabled = enabled;
            syncProducts.Enabled = enabled;
            downloadOrders.Enabled = enabled;
        }

        private void syncCustomers_Click(object sender, EventArgs e)
        {
            var bw = new BackgroundWorker();
            lblStatus.Text = "Processing customer sync (Preparing)\n\rPlease wait";

            SetButtonState(false);
            bw.DoWork += (bwSender, bwEventArgs) =>
            {
                var notificationEmail = Settings.GetNotificationEmail();
                var customers = new Customers();
                customers.UpdateTemporaryTables();
                customers.Empty();

                var mappedDsnName = Mapping.GetDsnName("Customers");
                var newMapping = new Mapping(mappedDsnName);
                if (newMapping.MigrateData("Customers"))
                {
                    ((BackgroundWorker)bwSender).ReportProgress(0, "Processing customer sync (Pushing)\n\rPlease wait");
                    if (customers.Publish(out var publishDetails, (BackgroundWorker)bwSender))
                    {
                        bwEventArgs.Result = new
                        {
                            Message = "Customers Synced",
                            Title = "Success",
                            Error = string.Empty,
                            InfoMessage = string.Empty
                        };

                        if (!string.IsNullOrWhiteSpace(notificationEmail))
                            Mail.SendProcessCompleteEmail(notificationEmail, publishDetails, "Customers Publish",
                                response => Logger.Instance.Info(response));

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

                        if (!string.IsNullOrWhiteSpace(notificationEmail))
                            Mail.SendProcessCompleteEmail(notificationEmail, "Customers Publish failed, please check logs or contact support", $"Customers Publish",
                                response => Logger.Instance.Info(response));

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

            bw.ProgressChanged += Status_BackgroundWorker_ProgressChanged;
            bw.RunWorkerCompleted += Status_BackgroundWorker_Completed;

            bw.WorkerReportsProgress = true;
            bw.RunWorkerAsync();
        }

        private void Status_BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lblStatus.Text = e.UserState.ToString();
        }

        private void Status_BackgroundWorker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            SetButtonState(true);
            dynamic info = e.Result;

            lblStatus.Text = info.Message;
            MessageBox.Show(info.Message, info.Title);
            if (!string.IsNullOrWhiteSpace(info.Error))
                Logger.Instance.Error(info.Error);
            if (!string.IsNullOrWhiteSpace(info.InfoMessage))
                Logger.Instance.Info(info.InfoMessage);
        }

        private void syncProducts_Click(object sender, EventArgs e)
        {
            var bw = new BackgroundWorker();
            lblStatus.Text = "Processing product sync (Preparing)\n\rPlease wait";

            SetButtonState(false);
            bw.DoWork += (bwSender, bwEventArgs) =>
            {
                var notificationEmail = Settings.GetNotificationEmail();
                var products = new Products();
                products.UpdateTemporaryTables();
                products.Empty();

                string mappedDsnName = Mapping.GetDsnName("Products");
                var newMapping = new Mapping(mappedDsnName);
                if (newMapping.MigrateData("Products"))
                {
                    if (products.Publish(out var publishDetails, bw))
                    {
                        bwEventArgs.Result = new
                        {
                            Message = "Your products have been pushed",
                            Title = "Published Successfully",
                            Error = string.Empty,
                            InfoMessage = string.Empty
                        };

                        if (!string.IsNullOrWhiteSpace(notificationEmail))
                            Mail.SendProcessCompleteEmail(notificationEmail, publishDetails, "Product Publish",
                                response => Logger.Instance.Info(response));

                    }
                    else
                    {
                        bwEventArgs.Result = new
                        {
                            Message = "Please select your product table!",
                            Title = "Publish Failed",
                            Error = "Products failed to sync.",
                            InfoMessage = string.Empty
                        };

                        if (!string.IsNullOrWhiteSpace(notificationEmail))
                            Mail.SendProcessCompleteEmail(notificationEmail, "Product Publish failed, please check logs or contact support", "Product Publish",
                                response => Logger.Instance.Info(response));

                    }
                }
                else
                {
                    if (!newMapping._validFields)
                    {
                        bwEventArgs.Result = new
                        {
                            Message = "Invalid mapping fields, please validate your mapping.",
                            Title = "Publish Failed",
                            Error = string.Empty,
                            InfoMessage = string.Empty
                        };

                        if (!string.IsNullOrWhiteSpace(notificationEmail))
                            Mail.SendProcessCompleteEmail(notificationEmail, "Product Publish failed, please validate your field mappings.", "Product Publish",
                                response => Logger.Instance.Info(response));
                    }
                    else
                    {
                        bwEventArgs.Result = new
                        {
                            Message = "Please select your products table",
                            Title = "Publish Failed",
                            Error = "Products failed to migrate.",
                            InfoMessage = string.Empty
                        };

                        if (!string.IsNullOrWhiteSpace(notificationEmail))
                            Mail.SendProcessCompleteEmail(notificationEmail, "Product Publish failed, please check logs or contact support", "Product Publish",
                                response => Logger.Instance.Info(response));
                    }
                }
            };

            bw.ProgressChanged += Status_BackgroundWorker_ProgressChanged;
            bw.RunWorkerCompleted += Status_BackgroundWorker_Completed;

            bw.WorkerReportsProgress = true;
            bw.RunWorkerAsync();
        }

        private void syncPriceLevels_Click(object sender, EventArgs e)
        {
            var notificationEmail = Settings.GetNotificationEmail();
            var priceLevels = new PriceLevels();
            priceLevels.UpdateTemporaryTables();
            priceLevels.Empty();

            string mappedDsnName = Mapping.GetDsnName("PriceLevels");
            var newMapping = new Mapping(mappedDsnName);

            if (newMapping.MigrateData("PriceLevels"))
            {
                if (priceLevels.Publish(out var publishDetails))
                {
                    MessageBox.Show(@"Your price levels have been published", @"Published Successfully");
                    Logger.Instance.Info("Price Levels synced.");

                    if (!string.IsNullOrWhiteSpace(notificationEmail))
                        Mail.SendProcessCompleteEmail(notificationEmail, publishDetails, "PriceLevel Publish",
                            response => Logger.Instance.Info(response));
                }
                else
                {
                    MessageBox.Show(@"Unable to publish your Price Levels", @"Publish Failed");
                    Logger.Instance.Error("Price Levels failed to sync.");

                    if (!string.IsNullOrWhiteSpace(notificationEmail))
                        Mail.SendProcessCompleteEmail(notificationEmail,
                            "PriceLevel Publish failed, please check logs or contact support",
                            "Price Level Publish",
                            response => Logger.Instance.Info(response));
                }
            }
            else
            {
                if (!newMapping._validFields)
                {
                    MessageBox.Show(@"Mapping failed, please validate your table mapping", @"Publish Failed");
                    if (!string.IsNullOrWhiteSpace(notificationEmail))
                        Mail.SendProcessCompleteEmail(notificationEmail,
                            "Price Level Publish failed, please check your table mapping.", "Price Level Publish",
                            response => Logger.Instance.Info(response));
                }
                else
                {
                    MessageBox.Show(@"An unexpected error has occurred, please check your logs or contact support",
                        @"Publish Failed");
                    Logger.Instance.Error("Price Levels failed to migrate.");

                    if (!string.IsNullOrWhiteSpace(notificationEmail))
                        Mail.SendProcessCompleteEmail(notificationEmail,
                            "Price Level Publish failed, please check logs or contact support",
                            "Price Level Publish",
                            response => Logger.Instance.Info(response));
                }
            }
        }

        private void syncPricing_Click(object sender, EventArgs e)
        {
            var bw = new BackgroundWorker();


            bw.DoWork += (bwSender, bwEventArgs) =>
            {
                var notificationEmail = Settings.GetNotificationEmail();
                var priceLevelPrices = new PriceLevelPrices();
                priceLevelPrices.UpdateTemporaryTables();
                priceLevelPrices.Empty();

                bw.ReportProgress(0, "Populating Local Repository\r\nPlease Wait");

                string mappedDsnName = Mapping.GetDsnName("PriceLevelPrices");
                var newMapping = new Mapping(mappedDsnName);
                if (newMapping.MigrateData("PriceLevelPrices"))
                {
                    bw.ReportProgress(0, "Preparing to publish to LinkGreen\r\nPlease Wait");
                    if (priceLevelPrices.Publish(out var publishDetails, bw))
                    {
                        bwEventArgs.Result = new
                        {
                            Message = "Prices have been published successfully",
                            Title = "Published Successfully",
                            Error = "",
                            InfoMessage = string.Empty
                        };

                        Logger.Instance.Info("Pricing synced.");

                        if (!string.IsNullOrWhiteSpace(notificationEmail))
                            Mail.SendProcessCompleteEmail(notificationEmail, publishDetails, "Pricing Publish",
                                response => Logger.Instance.Info(response));
                    }
                    else
                    {
                        bwEventArgs.Result = new
                        {
                            Message = "Unable to publish your prices",
                            Title = "Publish Failed",
                            Error = "Unable to publish your prices",
                            InfoMessage = string.Empty
                        };

                        Logger.Instance.Error("Pricing failed to sync.");

                        if (!string.IsNullOrWhiteSpace(notificationEmail))
                            Mail.SendProcessCompleteEmail(notificationEmail,
                                "Pricing Publish failed, please check logs or contact support", "Pricing Publish",
                                response => Logger.Instance.Info(response));
                    }
                }
                else
                {
                    if (!newMapping._validFields)
                    {
                        bwEventArgs.Result = new
                        {
                            Message = "Mapping failed, please validate your field mapping",
                            Title = "Publish Failed",
                            Error = "Mapping failed, please validate your field mapping",
                            InfoMessage = string.Empty
                        };

                        if (!string.IsNullOrWhiteSpace(notificationEmail))
                            Mail.SendProcessCompleteEmail(notificationEmail,
                                "Pricing Publish failed, please validate your field mapping.", "Pricing Publish",
                                response => Logger.Instance.Info(response));

                    }
                    else
                    {
                        bwEventArgs.Result = new
                        {
                            Message = "An unexepcted error has occurred, please check your logs or contct support",
                            Title = "Publish Failed",
                            Error = "An unexepcted error has occurred, please check your logs or contct support",
                            InfoMessage = string.Empty
                        };

                        Logger.Instance.Error("Pricing failed to migrate.");

                        if (!string.IsNullOrWhiteSpace(notificationEmail))
                            Mail.SendProcessCompleteEmail(notificationEmail,
                                "Product Publish failed, please check logs or contact support", "Pricing Publish",
                                response => Logger.Instance.Info(response));
                    }
                }
            };

            bw.ProgressChanged += Status_BackgroundWorker_ProgressChanged;
            bw.RunWorkerCompleted += Status_BackgroundWorker_Completed;

            bw.WorkerReportsProgress = true;
            bw.RunWorkerAsync();
        }

        private void taskManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var taskManager = new TaskManager();
            taskManager.ShowDialog();
        }

        private void downloadOrders_Click(object sender, EventArgs e)
        {
            var syncCustomerResponse = MessageBox.Show(this, "Would you like to do a Customer sync before downloading the Orders?", "Customer Sync?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (syncCustomerResponse == DialogResult.Cancel) return;

            var skipCustomers = syncCustomerResponse == DialogResult.No;

            var bw = new BackgroundWorker();
            lblStatus.Text = "Processing customer sync (Preparing)\n\rPlease wait";

            var notificationEmail = Settings.GetNotificationEmail();

            SetButtonState(false);
            bw.DoWork += (bwSender, bwEventArgs) =>
            {

                var customers = new Customers();

                if (!skipCustomers)
                {
                    customers.UpdateTemporaryTables();
                    customers.Empty();
                }

                string mappedDsnName = Mapping.GetDsnName("Customers");
                var newMapping = new Mapping(mappedDsnName);
                if (skipCustomers || newMapping.MigrateData("Customers"))
                {
                    if (!skipCustomers)
                    {
                        ((BackgroundWorker)bwSender).ReportProgress(0, "Processing customer sync (Pushing)\n\rPlease wait");
                    }

                    var publishDetails = new List<string>();
                    if (skipCustomers || customers.Publish(out publishDetails, (BackgroundWorker)bwSender))
                    {
                        if (!skipCustomers && !string.IsNullOrWhiteSpace(notificationEmail))
                        {
                            Mail.SendProcessCompleteEmail(notificationEmail, publishDetails, "Customers Publish",
                                response => Logger.Instance.Info(response));
                        }

                        bwEventArgs.Result = new DownloadOrderRunner().Run((BackgroundWorker)bwSender);

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

                        if (!string.IsNullOrWhiteSpace(notificationEmail))
                            Mail.SendProcessCompleteEmail(notificationEmail, "Customers Publish failed, please check logs or contact support", $"Customers Publish",
                                response => Logger.Instance.Info(response));
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

            bw.ProgressChanged += Status_BackgroundWorker_ProgressChanged;
            bw.RunWorkerCompleted += Status_BackgroundWorker_Completed;

            bw.WorkerReportsProgress = true;
            bw.RunWorkerAsync();
        }
    }
}
