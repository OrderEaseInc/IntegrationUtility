using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LinkGreenODBCUtility
{
    public partial class UtilitySettings : Form
    {
        public UtilitySettings()
        {
            InitializeComponent();
        }

        private void UtilitySettings_Load(object sender, EventArgs e)
        {
            apiKey.Text = Settings.GetApiKey();
            LinkLabel.Link link = new LinkLabel.Link();
            link.LinkData = "https://app.linkgreen.ca/Login/Manage";
            apiKeyDetailLink.Links.Add(link);

            if (Settings.DebugMode)
            {
                debugMode.Checked = true;
            }
            
            updateCategories.Checked = Settings.GetUpdateCategories();
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            ActiveForm.Close();
        }

        private void saveSettings_Click(object sender, EventArgs e)
        {
            Settings.SaveApiKey(apiKey.Text);
            Settings.SaveUpdateCategories(updateCategories.Checked ? "1" : "0");
            Settings.DebugMode = debugMode.Checked;
            ActiveForm.Close();
            Logger.Instance.Debug($"Settings saved: (ApiKey: '{apiKey.Text}', DebugMode: {debugMode.Checked}, UpdateCategories: {updateCategories.Checked})");
        }

        private void apiKeyDetailLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(e.Link.LinkData as string);
        }
    }
}
