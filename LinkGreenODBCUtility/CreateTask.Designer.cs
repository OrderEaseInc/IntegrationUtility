namespace LinkGreenODBCUtility
{
    partial class CreateTask
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateTask));
            this.taskComboBox = new System.Windows.Forms.ComboBox();
            this.taskLabel = new System.Windows.Forms.Label();
            this.startDateTime = new System.Windows.Forms.DateTimePicker();
            this.startDateTimeLabel = new System.Windows.Forms.Label();
            this.repeatComboBox = new System.Windows.Forms.ComboBox();
            this.repeatLabel = new System.Windows.Forms.Label();
            this.create = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // taskComboBox
            // 
            this.taskComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.taskComboBox.FormattingEnabled = true;
            this.taskComboBox.Location = new System.Drawing.Point(16, 29);
            this.taskComboBox.Name = "taskComboBox";
            this.taskComboBox.Size = new System.Drawing.Size(189, 21);
            this.taskComboBox.TabIndex = 0;
            this.taskComboBox.SelectedIndexChanged += new System.EventHandler(this.taskComboBox_SelectedIndexChanged);
            // 
            // taskLabel
            // 
            this.taskLabel.AutoSize = true;
            this.taskLabel.Location = new System.Drawing.Point(13, 13);
            this.taskLabel.Name = "taskLabel";
            this.taskLabel.Size = new System.Drawing.Size(34, 13);
            this.taskLabel.TabIndex = 1;
            this.taskLabel.Text = "Task:";
            // 
            // startDateTime
            // 
            this.startDateTime.CustomFormat = "MM/dd/yyyy hh:mm:ss";
            this.startDateTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.startDateTime.Location = new System.Drawing.Point(16, 79);
            this.startDateTime.Name = "startDateTime";
            this.startDateTime.Size = new System.Drawing.Size(189, 20);
            this.startDateTime.TabIndex = 2;
            this.startDateTime.Value = new System.DateTime(2017, 11, 9, 11, 32, 8, 0);
            // 
            // startDateTimeLabel
            // 
            this.startDateTimeLabel.AutoSize = true;
            this.startDateTimeLabel.Location = new System.Drawing.Point(13, 63);
            this.startDateTimeLabel.Name = "startDateTimeLabel";
            this.startDateTimeLabel.Size = new System.Drawing.Size(86, 13);
            this.startDateTimeLabel.TabIndex = 3;
            this.startDateTimeLabel.Text = "Start Date/Time:";
            // 
            // repeatComboBox
            // 
            this.repeatComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.repeatComboBox.FormattingEnabled = true;
            this.repeatComboBox.Location = new System.Drawing.Point(16, 127);
            this.repeatComboBox.Name = "repeatComboBox";
            this.repeatComboBox.Size = new System.Drawing.Size(189, 21);
            this.repeatComboBox.TabIndex = 4;
            // 
            // repeatLabel
            // 
            this.repeatLabel.AutoSize = true;
            this.repeatLabel.Location = new System.Drawing.Point(13, 111);
            this.repeatLabel.Name = "repeatLabel";
            this.repeatLabel.Size = new System.Drawing.Size(45, 13);
            this.repeatLabel.TabIndex = 5;
            this.repeatLabel.Text = "Repeat:";
            // 
            // create
            // 
            this.create.Location = new System.Drawing.Point(129, 170);
            this.create.Name = "create";
            this.create.Size = new System.Drawing.Size(75, 23);
            this.create.TabIndex = 6;
            this.create.Text = "Create";
            this.create.UseVisualStyleBackColor = true;
            // 
            // cancel
            // 
            this.cancel.Location = new System.Drawing.Point(48, 170);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 23);
            this.cancel.TabIndex = 7;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            // 
            // CreateTask
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(220, 205);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.create);
            this.Controls.Add(this.repeatLabel);
            this.Controls.Add(this.repeatComboBox);
            this.Controls.Add(this.startDateTimeLabel);
            this.Controls.Add(this.startDateTime);
            this.Controls.Add(this.taskLabel);
            this.Controls.Add(this.taskComboBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateTask";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Create Task";
            this.Load += new System.EventHandler(this.CreateTask_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox taskComboBox;
        private System.Windows.Forms.Label taskLabel;
        private System.Windows.Forms.DateTimePicker startDateTime;
        private System.Windows.Forms.Label startDateTimeLabel;
        private System.Windows.Forms.ComboBox repeatComboBox;
        private System.Windows.Forms.Label repeatLabel;
        private System.Windows.Forms.Button create;
        private System.Windows.Forms.Button cancel;
    }
}