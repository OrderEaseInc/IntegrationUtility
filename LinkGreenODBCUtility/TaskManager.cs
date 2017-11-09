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
    public partial class TaskManager : Form
    {
        public TaskManager()
        {
            InitializeComponent();
        }

        private void addTask_Click(object sender, EventArgs e)
        {
            var CreateTask = new CreateTask();
            CreateTask.ShowDialog();
        }
    }
}
