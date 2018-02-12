using System;
using System.Diagnostics;
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
            txtNotificationEmail.Text = Settings.GetNotificationEmail();
            var link = new LinkLabel.Link { LinkData = "https://app.linkgreen.ca/Login/Manage" };
            apiKeyDetailLink.Links.Add(link);

            if (Settings.DebugMode)
            {
                debugMode.Checked = true;
            }

            updateCategories.Checked = Settings.GetUpdateCategories();
            sandboxMode.Checked = Settings.GetSandboxMode();
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void saveSettings_Click(object sender, EventArgs e)
        {
            Settings.SaveApiKey(apiKey.Text);
            Settings.SaveUpdateCategories(updateCategories.Checked);
            Settings.SaveSandboxMode(sandboxMode.Checked);
            Settings.DebugMode = debugMode.Checked;
            Settings.SaveNotificationEmail(txtNotificationEmail.Text);
            Close();
            Logger.Instance.Debug($"Settings saved: (ApiKey: '{apiKey.Text}', DebugMode: {debugMode.Checked}, SandboxMode: {sandboxMode.Checked}, UpdateCategories: {updateCategories.Checked})");
        }

        private void apiKeyDetailLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(e.Link.LinkData as string);
        }
    }
}
