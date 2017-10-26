using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["ApiKey"].Value = Settings.GetApiKey();
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");

            if (!string.IsNullOrEmpty(config.AppSettings.Settings["ApiKey"].Value))
            {
                Settings.SetupAppConfig(config.AppSettings.Settings["ApiKey"].Value);
            }
            
            Log.PurgeLog();
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
            if (newMapping.MigrateData("Categories") && categories.Sync())
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
            if (newMapping.MigrateData("Customers") && customers.Sync())
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

        private void syncProducts_Click(object sender, EventArgs e)
        {
            var products = new Products();
            products.UpdateTemporaryTables();
            products.Empty();
            
            string mappedDsnName = Mapping.GetDsnName("Products");
            var newMapping = new Mapping(mappedDsnName);
            if (newMapping.MigrateData("Products") && products.Publish())
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

        private void syncPriceLevels_Click(object sender, EventArgs e)
        {
            var priceLevels = new PriceLevels();
            priceLevels.UpdateTemporaryTables();
            priceLevels.Empty();
            
            string mappedDsnName = Mapping.GetDsnName("PriceLevels");
            var newMapping = new Mapping(mappedDsnName);
            if (newMapping.MigrateData("PriceLevels") && priceLevels.Sync())
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

        private void syncPricing_Click(object sender, EventArgs e)
        {
            var priceLevelPrices = new PriceLevelPrices();
            priceLevelPrices.UpdateTemporaryTables();
            priceLevelPrices.Empty();

            string mappedDsnName = Mapping.GetDsnName("PriceLevelPrices");
            var newMapping = new Mapping(mappedDsnName);
            if (newMapping.MigrateData("PriceLevelPrices") && priceLevelPrices.Sync())
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
    }
}
