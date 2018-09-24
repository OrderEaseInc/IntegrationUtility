using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using LinkGreen.Applications.Common;

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

            FillOrderStatuses();
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
            if (cboDownloadOrderStatus.SelectedValue is int) {
                var status = (int) cboDownloadOrderStatus.SelectedValue;
                Settings.SaveStatusIdForOrderDownload(status);
            }
            Close();
            Logger.Instance.Debug($"Settings saved: (ApiKey: '{apiKey.Text}', DebugMode: {debugMode.Checked}, SandboxMode: {sandboxMode.Checked}, UpdateCategories: {updateCategories.Checked})");
        }

        private void apiKeyDetailLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(e.Link.LinkData as string);
        }

        private void apiKey_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(apiKey.Text)) {
                cboDownloadOrderStatus.DataSource = null;
            } else {
                FillOrderStatuses();
            }
        }

        private void FillOrderStatuses()
        {
            try {

                WebServiceHelper.Key = apiKey.Text;
                var statuses = WebServiceHelper.GetAllOrderStatuses();
                if (statuses == null) {
                    Logger.Instance.Warning("Retrieving order statuses returned a null result");
                    cboDownloadOrderStatus.DataSource = null;
                    cboDownloadOrderStatus.Enabled = false;
                } else {
                    cboDownloadOrderStatus.Enabled = true;
                    cboDownloadOrderStatus.DataSource = statuses.OrderBy(s => s.Status).ToList();
                    var savedStatus = Settings.GetStatusIdForOrderDownload();
                    if (savedStatus.HasValue && statuses.Exists(s => s.Id == savedStatus.Value)) {
                        cboDownloadOrderStatus.SelectedValue = savedStatus.Value;
                    }
                }

            } catch (Exception ex) {
                Logger.Instance.Error($"Error retrieving order statuses: {ex.GetBaseException().Message}");
            }
        }
    }
}
