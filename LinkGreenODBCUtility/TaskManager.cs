using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DataTransfer.AccessDatabase.Models;
using Quartz;

namespace LinkGreenODBCUtility
{
    public partial class TaskManager : Form
    {
        private static DataGridView _tasksGridView;
        private static BindingSource _taskGridDataBindingSource;

        public TaskManager()
        {
            InitializeComponent();
        }

        public static void LoadTasks()
        {
            var Tasks = new Tasks();
            IEnumerable<Task> tasks = Tasks.GetAll();
            foreach (Task task in tasks)
            {
                var job = JobManager.GetJob(task.TaskName);
                var runningJobContexts = JobManager.GetCurrentlyExecutingJobs();
                List<IJobDetail> runningJobs = new List<IJobDetail>();
                List<IJobDetail> pausedJobs = new List<IJobDetail>();
                foreach (IJobExecutionContext runningJobContext in runningJobContexts)
                {
                    if (!JobManager.IsTriggerPaused(runningJobContext.Trigger.Key.Name, runningJobContext.Trigger.Key.Group))
                    {
                        runningJobs.Add(runningJobContext.JobDetail);
                    }
                    else
                    {
                        pausedJobs.Add(runningJobContext.JobDetail);
                    }
                }

                if (job != null)
                {
                    task.Status = "Scheduled";
                    if (runningJobs.Contains(job) && !pausedJobs.Contains(job))
                    {
                        task.Status = "Running";
                    }
                    else if (pausedJobs.Contains(job))
                    {
                        task.Status = "Paused";
                    }
                }
                else
                {
                    task.Status = "Not scheduled";
                }
            }
            
            _taskGridDataBindingSource = new BindingSource();
            _taskGridDataBindingSource.DataSource = tasks.ToList();
            _tasksGridView.AutoGenerateColumns = true;
            _tasksGridView.DataSource = _taskGridDataBindingSource;

            if (_tasksGridView.Columns["Pause"] != null)
            {
                _tasksGridView.Columns.Remove(_tasksGridView.Columns["Pause"]);
            }
            if (_tasksGridView.Columns["Delete"] != null)
            {
                _tasksGridView.Columns.Remove(_tasksGridView.Columns["Delete"]);
            }

            DataGridViewButtonColumn PauseColumn = new DataGridViewButtonColumn();
            PauseColumn.Text = "Pause/Resume";
            PauseColumn.Name = "Pause";
            PauseColumn.DataPropertyName = "Pause";
            PauseColumn.UseColumnTextForButtonValue = true;
            _tasksGridView.Columns.Add(PauseColumn);

            DataGridViewButtonColumn DelColumn = new DataGridViewButtonColumn();
            DelColumn.Text = "Delete";
            DelColumn.Name = "Delete";
            DelColumn.DataPropertyName = "Delete";
            DelColumn.UseColumnTextForButtonValue = true;
            _tasksGridView.Columns.Add(DelColumn);

            _tasksGridView.Refresh();
        }

        private void addTask_Click(object sender, EventArgs e)
        {
            var CreateTask = new CreateTask();
            CreateTask.ShowDialog();
        }

        private void TaskManager_Load(object sender, EventArgs e)
        {
            _tasksGridView = tasksGridView;
            LoadTasks();
        }

        private void tasksGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (e.ColumnIndex == senderGrid.Columns["Pause"].Index 
                && senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn 
                && e.RowIndex >= 0)
            {
                // Pause
                DataGridViewRow row = senderGrid.CurrentCell.OwningRow;
                string jobName = row.Cells[senderGrid.Columns["TaskName"].Index].Value.ToString();

                var job = JobManager.GetJob(jobName);
                var runningJobContexts = JobManager.GetCurrentlyExecutingJobs();
                List<IJobDetail> pausedJobs = new List<IJobDetail>();
                foreach (IJobExecutionContext runningJobContext in runningJobContexts)
                {
                    if (JobManager.IsTriggerPaused(runningJobContext.Trigger.Key.Name, runningJobContext.Trigger.Key.Group))
                    {
                        pausedJobs.Add(runningJobContext.JobDetail);
                    }
                }

                if (pausedJobs.Contains(job))
                {
                    if (JobManager.ResumeJob(jobName))
                    {
                        MessageBox.Show($"Task {jobName} resumed.", "Success");
                    }
                    else
                    {
                        MessageBox.Show($"Task {jobName} failed to resume.", "Failed");
                    }
                }
                else
                {
                    if (JobManager.PauseJob(jobName))
                    {
                        MessageBox.Show($"Task {jobName} paused.", "Success");
                    }
                    else
                    {
                        MessageBox.Show($"Task {jobName} failed to pause.", "Failed");
                    }
                }
                

                LoadTasks();
            }

            if (e.ColumnIndex == senderGrid.Columns["Delete"].Index
                && senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn
                && e.RowIndex >= 0)
            {
                try
                {
                    DataGridViewRow row = senderGrid.CurrentCell.OwningRow;
                    string jobName = row.Cells[senderGrid.Columns["TaskName"].Index].Value.ToString();
                    if (JobManager.DeleteJob(jobName))
                    {
                        var Tasks = new Tasks();
                        Tasks.DeleteTask(jobName);
                        MessageBox.Show($"Task {jobName} deleted.", "Success");
                    }
                    else
                    {
                        MessageBox.Show($"Task {jobName} failed to delete.", "Failed");
                    }

                    LoadTasks();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occured while deleting the task.", "Delete Error");
                }
            }
        }
    }
}
