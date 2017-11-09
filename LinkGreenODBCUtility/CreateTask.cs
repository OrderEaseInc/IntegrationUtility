using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LinkGreenODBCUtility
{
    public partial class CreateTask : Form
    {
        public CreateTask()
        {
            InitializeComponent();
        }

        private void CreateTask_Load(object sender, EventArgs e)
        {
            // Tasks
            taskComboBox.DisplayMember = "Text";
            taskComboBox.ValueMember = "Value";
            var taskOne = new ListItem();
            taskOne.Text = "Sync Categories";
            taskOne.Value = "Categories";
            taskComboBox.Items.Add(taskOne);

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

        private void taskComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
