using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using LinkGreen.Applications.Common;
using LinkGreen.Applications.Common.Model;

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
            var statuses = new List<int>();
            foreach (var item in lstDownloadOrderStatus.CheckedItems)
            {
                var data = (OrderStatus)item;
                statuses.Add(data.Id);
            }

            Settings.SaveStatusIdForOrderDownload(statuses.ToArray());

            Close();

            Logger.Instance.Debug($"Settings saved: (ApiKey: '{apiKey.Text}', DebugMode: {debugMode.Checked}, SandboxMode: {sandboxMode.Checked}, UpdateCategories: {updateCategories.Checked})");
        }

        private void apiKeyDetailLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(e.Link.LinkData as string);
        }

        private void apiKey_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(apiKey.Text))
            {
                lstDownloadOrderStatus.DataSource = null;
            }
            else
            {
                FillOrderStatuses();
            }
        }

        private void FillOrderStatuses()
        {
            try
            {
                lstDownloadOrderStatus.ValueMember = "Id";
                lstDownloadOrderStatus.DisplayMember = "Status";

                WebServiceHelper.Key = apiKey.Text;
                var statuses = WebServiceHelper.GetAllOrderStatuses();
                if (statuses == null)
                {
                    Logger.Instance.Warning("Retrieving order statuses returned a null result");
                    lstDownloadOrderStatus.DataSource = null;
                    lstDownloadOrderStatus.Enabled = false;
                }
                else
                {
                    lstDownloadOrderStatus.DataSource = statuses.OrderBy(s => s.Status).ToList(); ;
                    lstDownloadOrderStatus.Enabled = true;

                    var savedStatus = Settings.GetStatusIdForOrderDownload();
                    if (savedStatus != null && savedStatus.Any())
                    {


                        for (var i = 0; i < lstDownloadOrderStatus.Items.Count; i++)
                        {
                            var data = (OrderStatus)lstDownloadOrderStatus.Items[i];
                            lstDownloadOrderStatus.SetItemCheckState(i,
                                savedStatus.Any(ss => ss == data.Id) ? CheckState.Checked : CheckState.Unchecked);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"Error retrieving order statuses: {ex.GetBaseException().Message}");
            }
        }
    }
}
