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

            // TODO: Restore scroll position
//            int horizontalOffset = FirstCompletelyVisibleColumnIndex;
            _tasksGridView.Refresh();
            _tasksGridView.ClearSelection();
//            _tasksGridView.FirstDisplayedScrollingColumnIndex = horizontalOffset;
        }

        private void addTask_Click(object sender, EventArgs e)
        {
            var CreateTask = new CreateTask();
            CreateTask.ShowDialog();
        }

        private void TaskManager_Load(object sender, EventArgs e)
        {
            _tasksGridView = tasksGridView;

            Timer timer = new Timer();
            timer.Interval = (10 * 1000); // 10 secs
            timer.Tick += taskTimer_Tick;
            timer.Start();

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
                        MessageBox.Show($@"Task {jobName} resumed.", @"Emptied Successfully");
                    }
                    else
                    {
                        MessageBox.Show($@"Task {jobName} failed to resume.", @"Emptied Successfully");
                    }
                }
                else
                {
                    if (JobManager.PauseJob(jobName))
                    {
                        MessageBox.Show($@"Task {jobName} paused.", @"Emptied Successfully");
                    }
                    else
                    {
                        MessageBox.Show($@"Task {jobName} failed to pause.", @"Emptied Successfully");
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
                        MessageBox.Show($@"Task {jobName} deleted.", @"Emptied Successfully");
                    }
                    else
                    {
                        MessageBox.Show($@"Task {jobName} failed to delete.", @"Emptied Successfully");
                    }

                    LoadTasks();
                }
                catch (Exception)
                {
                    MessageBox.Show($@"An error occured while deleting the task.", @"Emptied Successfully");
                }
            }
        }

        private void taskTimer_Tick(object sender, EventArgs e)
        {
            LoadTasks();
        }

        public static int FirstCompletelyVisibleColumnIndex
        {
            get
            {
                // Number of visible columns.
                int count = _tasksGridView.DisplayedColumnCount(false);

                // Index of first displayed column.
                int index = _tasksGridView.FirstDisplayedScrollingColumnIndex;

                // Return index of first visible row (if only one visible), else the second one.
                // (This to be sure that we return an index of a column that is completely visible).
                // return index == count - 1 ? index : index + 1;

                return index == count - 1 ? index : index + 0;
            }
        }
    }
}
