using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Quartz;
using Quartz.Impl;

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
            var taskCategories = new ListItem();
            taskCategories.Text = "Sync Categories";
            taskCategories.Value = "Categories";
            taskComboBox.Items.Add(taskCategories);
            var taskCustomers = new ListItem();
            taskCustomers.Text = "Sync Customers";
            taskCustomers.Value = "Customers";
            taskComboBox.Items.Add(taskCustomers);
            var taskProducts = new ListItem();
            taskProducts.Text = "Sync Products";
            taskProducts.Value = "Products";
            taskComboBox.Items.Add(taskProducts);
            var taskInventoryQuantities = new ListItem();
            taskInventoryQuantities.Text = "Sync Inventory Quantities";
            taskInventoryQuantities.Value = "InventoryQuantities";
            taskComboBox.Items.Add(taskInventoryQuantities);
            var taskPriceLevels = new ListItem();
            taskPriceLevels.Text = "Sync Price Levels";
            taskPriceLevels.Value = "Price Levels";
            taskComboBox.Items.Add(taskPriceLevels);
            var taskPricing = new ListItem();
            taskPricing.Text = "Sync Pricing";
            taskPricing.Value = "Pricing";
            taskComboBox.Items.Add(taskPricing);
            var taskSuppliers = new ListItem();
            taskSuppliers.Text = "Sync Suppliers";
            taskSuppliers.Value = "Suppliers";
            taskComboBox.Items.Add(taskSuppliers);
            var taskSupplierInventory = new ListItem();
            taskSupplierInventory.Text = "Sync Supplier Inventory";
            taskSupplierInventory.Value = "SupplierInventory";
            taskComboBox.Items.Add(taskSupplierInventory);
            var taskLinkedSkus = new ListItem();
            taskLinkedSkus.Text = "Sync Linked Skus";
            taskLinkedSkus.Value = "LinkedSkus";
            taskComboBox.Items.Add(taskLinkedSkus);
            var taskBuyerInventory = new ListItem();
            taskBuyerInventory.Text = "Sync Buyer Inventory";
            taskBuyerInventory.Value = "BuyerInventory";
            taskComboBox.Items.Add(taskBuyerInventory);

            // Start Date/Time
            startDateTime.Value = DateTime.Now;

            // Repeat intervals
            repeatComboBox.DisplayMember = "Text";
            repeatComboBox.ValueMember = "Value";
            var repeatFiveMin = new ListItem();
            repeatFiveMin.Text = "Every 5 Minutes";
            repeatFiveMin.Value = "5";
            repeatComboBox.Items.Add(repeatFiveMin);
            var repeatFifteenMin = new ListItem();
            repeatFifteenMin.Text = "Every 15 Minutes";
            repeatFifteenMin.Value = "15";
            repeatComboBox.Items.Add(repeatFifteenMin);
            var repeatThirtyMin = new ListItem();
            repeatThirtyMin.Text = "Every 30 Minutes";
            repeatThirtyMin.Value = "30";
            repeatComboBox.Items.Add(repeatThirtyMin);
            var repeatOneHour = new ListItem();
            repeatOneHour.Text = "Every Hour";
            repeatOneHour.Value = "60";
            repeatComboBox.Items.Add(repeatOneHour);
            var repeatTwelveHours = new ListItem();
            repeatTwelveHours.Text = "Every 12 Hours";
            repeatTwelveHours.Value = "720";
            repeatComboBox.Items.Add(repeatTwelveHours);
            var repeatOneDay = new ListItem();
            repeatOneDay.Text = "Daily";
            repeatOneDay.Value = "1440";
            repeatComboBox.Items.Add(repeatOneDay);
            var repeatOneWeek = new ListItem();
            repeatOneWeek.Text = "Weekly";
            repeatOneWeek.Value = "10080";
            repeatComboBox.Items.Add(repeatOneWeek);
            var repeatTwoWeeks = new ListItem();
            repeatTwoWeeks.Text = "Bi-Weekly";
            repeatTwoWeeks.Value = "20160";
            repeatComboBox.Items.Add(repeatTwoWeeks);
            var repeatFourWeeks = new ListItem();
            repeatFourWeeks.Text = "Every 4 Weeks";
            repeatFourWeeks.Value = "40320";
            repeatComboBox.Items.Add(repeatFourWeeks);
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
