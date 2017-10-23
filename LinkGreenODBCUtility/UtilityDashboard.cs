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

            var mapping = new Mapping();
            string mappedDsnName = mapping.GetDsnName("Categories");
            var newMapping = new Mapping(mappedDsnName);
            if (newMapping.MigrateData("Categories") && categories.Publish())
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
    }
}
