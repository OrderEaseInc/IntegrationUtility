namespace LinkGreenODBCUtility
{
    partial class UtilityDashboard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UtilityDashboard));
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.syncCategories = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.settingsMappingMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eventLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.BackColor = System.Drawing.SystemColors.MenuBar;
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(384, 24);
            this.mainMenuStrip.TabIndex = 0;
            this.mainMenuStrip.Text = "menuStrip1";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsMappingMenuItem,
            this.eventLogToolStripMenuItem,
            this.generalToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.settingsToolStripMenuItem.Text = "File";
            // 
            // syncCategories
            // 
            this.syncCategories.Location = new System.Drawing.Point(13, 50);
            this.syncCategories.Name = "syncCategories";
            this.syncCategories.Size = new System.Drawing.Size(102, 23);
            this.syncCategories.TabIndex = 1;
            this.syncCategories.Text = "Sync Categories";
            this.syncCategories.UseVisualStyleBackColor = true;
            this.syncCategories.Click += new System.EventHandler(this.syncCategories_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImage = global::LinkGreenODBCUtility.Properties.Resources.logo_transparent;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.ErrorImage = null;
            this.pictureBox1.InitialImage = global::LinkGreenODBCUtility.Properties.Resources.logo_transparent;
            this.pictureBox1.Location = new System.Drawing.Point(272, 114);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(100, 100);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // settingsMappingMenuItem
            // 
            this.settingsMappingMenuItem.Image = global::LinkGreenODBCUtility.Properties.Resources.logo;
            this.settingsMappingMenuItem.Name = "settingsMappingMenuItem";
            this.settingsMappingMenuItem.Size = new System.Drawing.Size(152, 22);
            this.settingsMappingMenuItem.Text = "Mappings";
            this.settingsMappingMenuItem.Click += new System.EventHandler(this.settingsMappingMenuItem_Click);
            // 
            // eventLogToolStripMenuItem
            // 
            this.eventLogToolStripMenuItem.Image = global::LinkGreenODBCUtility.Properties.Resources.debug_log;
            this.eventLogToolStripMenuItem.Name = "eventLogToolStripMenuItem";
            this.eventLogToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.eventLogToolStripMenuItem.Text = "Event Log";
            this.eventLogToolStripMenuItem.Click += new System.EventHandler(this.eventLogToolStripMenuItem_Click);
            // 
            // generalToolStripMenuItem
            // 
            this.generalToolStripMenuItem.Image = global::LinkGreenODBCUtility.Properties.Resources.gear;
            this.generalToolStripMenuItem.Name = "generalToolStripMenuItem";
            this.generalToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.generalToolStripMenuItem.Text = "Settings";
            this.generalToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // UtilityDashboard
            // 
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuBar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 226);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.syncCategories);
            this.Controls.Add(this.mainMenuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mainMenuStrip;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(400, 265);
            this.MinimumSize = new System.Drawing.Size(400, 265);
            this.Name = "UtilityDashboard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Integration Utility";
            this.Load += new System.EventHandler(this.UtilityDashboard_Load);
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsMappingMenuItem;
        private System.Windows.Forms.Button syncCategories;
        private System.Windows.Forms.ToolStripMenuItem generalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem eventLogToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}