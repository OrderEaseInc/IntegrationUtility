namespace LinkGreenODBCUtility
{
    partial class UtilitySettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UtilitySettings));
            this.apiKey = new System.Windows.Forms.TextBox();
            this.apiKeyLabel = new System.Windows.Forms.Label();
            this.saveSettings = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.apiKeyDescriptionLabel = new System.Windows.Forms.Label();
            this.apiKeyDetailLink = new System.Windows.Forms.LinkLabel();
            this.debugMode = new System.Windows.Forms.CheckBox();
            this.updateCategories = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // apiKey
            // 
            this.apiKey.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.apiKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.apiKey.Location = new System.Drawing.Point(12, 95);
            this.apiKey.MinimumSize = new System.Drawing.Size(4, 25);
            this.apiKey.Name = "apiKey";
            this.apiKey.Size = new System.Drawing.Size(331, 21);
            this.apiKey.TabIndex = 0;
            // 
            // apiKeyLabel
            // 
            this.apiKeyLabel.AutoSize = true;
            this.apiKeyLabel.Location = new System.Drawing.Point(9, 79);
            this.apiKeyLabel.Name = "apiKeyLabel";
            this.apiKeyLabel.Size = new System.Drawing.Size(46, 13);
            this.apiKeyLabel.TabIndex = 1;
            this.apiKeyLabel.Text = "Api Key:";
            // 
            // saveSettings
            // 
            this.saveSettings.Location = new System.Drawing.Point(268, 181);
            this.saveSettings.Name = "saveSettings";
            this.saveSettings.Size = new System.Drawing.Size(75, 23);
            this.saveSettings.TabIndex = 2;
            this.saveSettings.Text = "Save";
            this.saveSettings.UseVisualStyleBackColor = true;
            this.saveSettings.Click += new System.EventHandler(this.saveSettings_Click);
            // 
            // cancel
            // 
            this.cancel.Location = new System.Drawing.Point(187, 181);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 23);
            this.cancel.TabIndex = 3;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // apiKeyDescriptionLabel
            // 
            this.apiKeyDescriptionLabel.Location = new System.Drawing.Point(9, 9);
            this.apiKeyDescriptionLabel.Name = "apiKeyDescriptionLabel";
            this.apiKeyDescriptionLabel.Size = new System.Drawing.Size(343, 31);
            this.apiKeyDescriptionLabel.TabIndex = 4;
            this.apiKeyDescriptionLabel.Text = "Use the following link to find your API Key. We suggest creating another user exp" +
    "licitly for use within the LinkGreen Integration Utility.";
            // 
            // apiKeyDetailLink
            // 
            this.apiKeyDetailLink.AccessibleRole = System.Windows.Forms.AccessibleRole.Link;
            this.apiKeyDetailLink.AutoSize = true;
            this.apiKeyDetailLink.Location = new System.Drawing.Point(12, 44);
            this.apiKeyDetailLink.Name = "apiKeyDetailLink";
            this.apiKeyDetailLink.Size = new System.Drawing.Size(197, 13);
            this.apiKeyDetailLink.TabIndex = 5;
            this.apiKeyDetailLink.TabStop = true;
            this.apiKeyDetailLink.Text = "https://app.linkgreen.ca/Login/Manage";
            this.apiKeyDetailLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.apiKeyDetailLink_LinkClicked);
            // 
            // debugMode
            // 
            this.debugMode.AutoSize = true;
            this.debugMode.Location = new System.Drawing.Point(12, 122);
            this.debugMode.Name = "debugMode";
            this.debugMode.Size = new System.Drawing.Size(88, 17);
            this.debugMode.TabIndex = 7;
            this.debugMode.Text = "Debug Mode";
            this.debugMode.UseVisualStyleBackColor = true;
            // 
            // updateCategories
            // 
            this.updateCategories.AutoSize = true;
            this.updateCategories.Checked = true;
            this.updateCategories.CheckState = System.Windows.Forms.CheckState.Checked;
            this.updateCategories.Location = new System.Drawing.Point(12, 141);
            this.updateCategories.Name = "updateCategories";
            this.updateCategories.Size = new System.Drawing.Size(192, 17);
            this.updateCategories.TabIndex = 8;
            this.updateCategories.Text = "Update Categories (Product Import)";
            this.updateCategories.UseVisualStyleBackColor = true;
            // 
            // UtilitySettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 216);
            this.Controls.Add(this.updateCategories);
            this.Controls.Add(this.debugMode);
            this.Controls.Add(this.apiKeyDetailLink);
            this.Controls.Add(this.apiKeyDescriptionLabel);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.saveSettings);
            this.Controls.Add(this.apiKeyLabel);
            this.Controls.Add(this.apiKey);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(375, 255);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(375, 215);
            this.Name = "UtilitySettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "General Settings";
            this.Load += new System.EventHandler(this.UtilitySettings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox apiKey;
        private System.Windows.Forms.Label apiKeyLabel;
        private System.Windows.Forms.Button saveSettings;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Label apiKeyDescriptionLabel;
        private System.Windows.Forms.LinkLabel apiKeyDetailLink;
        private System.Windows.Forms.CheckBox debugMode;
        private System.Windows.Forms.CheckBox updateCategories;
    }
}