namespace LinkGreenODBCUtility
{
    partial class MappingPreview
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MappingPreview));
            this.previewDataGridView = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.previewDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // previewDataGridView
            // 
            this.previewDataGridView.AllowUserToAddRows = false;
            this.previewDataGridView.AllowUserToDeleteRows = false;
            this.previewDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.previewDataGridView.Location = new System.Drawing.Point(13, 13);
            this.previewDataGridView.Name = "previewDataGridView";
            this.previewDataGridView.ReadOnly = true;
            this.previewDataGridView.Size = new System.Drawing.Size(585, 392);
            this.previewDataGridView.TabIndex = 0;
            // 
            // MappingPreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(610, 417);
            this.Controls.Add(this.previewDataGridView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MappingPreview";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Mapping Preview";
            this.Load += new System.EventHandler(this.MappingPreview_Load);
            ((System.ComponentModel.ISupportInitialize)(this.previewDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView previewDataGridView;
    }
}