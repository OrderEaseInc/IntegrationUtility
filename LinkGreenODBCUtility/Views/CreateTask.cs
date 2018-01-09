using System;
using System.Windows.Forms;

namespace LinkGreenODBCUtility
{
    public partial class CreateTask : Form
    {
        private TaskManager _taskManager;

        public CreateTask()
        {
            InitializeComponent();
        }

        private void CreateTask_Load(object sender, EventArgs e)
        {
            // Tasks
            taskComboBox.DisplayMember = "Text";
            taskComboBox.ValueMember = "Value";

            taskComboBox.Items.AddRange(new object[]
            {
                new ListItem { Text = "Sync Categories",  Value = "Categories" },
                new ListItem { Text = "Sync Customers", Value = "Customers" },
                new ListItem { Text = "Sync Products", Value = "Products" },
                new ListItem { Text = "Sync Inventory Quantities",  Value = "InventoryQuantities" },
                new ListItem { Text = "Sync Price Levels", Value = "Price Levels" },
                new ListItem { Text = "Sync Pricing", Value = "Pricing" },
                new ListItem { Text = "Sync Suppliers", Value = "Suppliers" },
                new ListItem { Text = "Sync Supplier Inventory", Value = "SupplierInventory" },
                new ListItem { Text = "Sync Linked Skus", Value = "LinkedSkus" },
                new ListItem { Text = "Sync Buyer Inventory", Value = "BuyerInventory" }
            });

            // Start Date/Time
            startDateTime.Value = DateTime.Now;

            // Repeat intervals
            repeatComboBox.DisplayMember = "Text";
            repeatComboBox.ValueMember = "Value";

            repeatComboBox.Items.AddRange(new object[]
            {
                new ListItem { Text = "Every 5 Minutes", Value = "5" },
                new ListItem { Text = "Every 15 Minutes",  Value = "15" },
                new ListItem { Text = "Every 30 Minutes", Value = "30" },
                new ListItem { Text = "Every Hour", Value = "60" },
                new ListItem { Text = "Every 12 Hours", Value = "720" },
                new ListItem { Text = "Daily", Value = "1440" },
                new ListItem { Text = "Weekly", Value = "10080" },
                new ListItem { Text = "Bi-Weekly", Value = "20160" },
                new ListItem { Text = "Every 4 Weeks", Value = "40320" }
            });


        }

        private void CreateTask_FormClosed(object sender, FormClosedEventArgs e)
        {
            TaskManager.LoadTasks();
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            ActiveForm.Close();
        }

        private void create_Click(object sender, EventArgs e)
        {
            string jobName = (taskComboBox.SelectedItem as ListItem).Value;
            string jobDisplayName = (taskComboBox.SelectedItem as ListItem).Text;
            DateTime startDateTime = this.startDateTime.Value;
            int repeatInterval = Convert.ToInt32((repeatComboBox.SelectedItem as ListItem).Value);

            try
            {
                var Tasks = new Tasks();
                if (Tasks.TaskExists(jobName))
                {
                    DialogResult dialogResult = MessageBox.Show($"Are you sure you want to overwrite the task {jobName}?", "Overwrite Task?", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        if (JobManager.DeleteJob(jobName))
                        {
                            Tasks.DeleteTask(jobName);
                        }
                    }
                }

                if (JobManager.ScheduleJob(jobName, startDateTime, repeatInterval))
                {
                    if (Tasks.CreateTask(jobName, jobDisplayName, startDateTime, repeatInterval))
                    {
                        MessageBox.Show($"Task created: {jobName}");
                    }
                    else
                    {
                        MessageBox.Show($"Task {jobName} created but will be lost if application is closed.");
                    }
                    ActiveForm.Close();
                    TaskManager.LoadTasks();
                }
            }
            catch (ArgumentException ex)
            {
                Logger.Instance.Error($"An error occured while creating the task {jobName}: {ex}");
                MessageBox.Show($"An error occured while creating the task {jobName}");
                TaskManager.LoadTasks();
            }
        }
    }
}
