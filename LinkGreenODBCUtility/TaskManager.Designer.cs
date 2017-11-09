namespace LinkGreenODBCUtility
{
    partial class TaskManager
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TaskManager));
            this.addTask = new System.Windows.Forms.Button();
            this.tasksGridView = new System.Windows.Forms.DataGridView();
            this.activeTasksLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.tasksGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // addTask
            // 
            this.addTask.Location = new System.Drawing.Point(507, 12);
            this.addTask.Name = "addTask";
            this.addTask.Size = new System.Drawing.Size(89, 31);
            this.addTask.TabIndex = 0;
            this.addTask.Text = "Create Task";
            this.addTask.UseVisualStyleBackColor = true;
            this.addTask.Click += new System.EventHandler(this.addTask_Click);
            // 
            // tasksGridView
            // 
            this.tasksGridView.AllowUserToAddRows = false;
            this.tasksGridView.AllowUserToDeleteRows = false;
            this.tasksGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tasksGridView.Location = new System.Drawing.Point(12, 49);
            this.tasksGridView.Name = "tasksGridView";
            this.tasksGridView.ReadOnly = true;
            this.tasksGridView.Size = new System.Drawing.Size(584, 234);
            this.tasksGridView.TabIndex = 1;
            // 
            // activeTasksLabel
            // 
            this.activeTasksLabel.AutoSize = true;
            this.activeTasksLabel.Location = new System.Drawing.Point(12, 29);
            this.activeTasksLabel.Name = "activeTasksLabel";
            this.activeTasksLabel.Size = new System.Drawing.Size(72, 13);
            this.activeTasksLabel.TabIndex = 2;
            this.activeTasksLabel.Text = "Active Tasks:";
            // 
            // TaskManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(608, 297);
            this.Controls.Add(this.activeTasksLabel);
            this.Controls.Add(this.tasksGridView);
            this.Controls.Add(this.addTask);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "TaskManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Task Manager";
            this.Load += new System.EventHandler(this.TaskManager_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tasksGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button addTask;
        private System.Windows.Forms.DataGridView tasksGridView;
        private System.Windows.Forms.Label activeTasksLabel;
    }
}