using System;
using System.Data;
using System.Windows.Forms;

namespace LinkGreenODBCUtility
{
    public partial class EventLog : Form
    {
        public EventLog()
        {
            InitializeComponent();
        }

        private void EventLog_Load(object sender, EventArgs e)
        {
            var log = new Log();
            var logTable = log.LoadLog();

            eventLogDataGrid.DataSource = logTable.DefaultView;
            eventLogDataGrid.AutoGenerateColumns = true;

            eventLogDataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            eventLogDataGrid.Columns[0].MinimumWidth = 120;
            eventLogDataGrid.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            eventLogDataGrid.AutoResizeColumns();
        }
    }
}
