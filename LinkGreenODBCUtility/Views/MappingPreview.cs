using System;
using System.Data;
using System.Windows.Forms;

namespace LinkGreenODBCUtility
{
    public partial class MappingPreview : Form
    {
        private readonly string _tableName;
        private readonly string _dsnName;

        public MappingPreview(string tableName, string dsnName)
        {
            InitializeComponent();
            _tableName = tableName;
            _dsnName = dsnName;
        }

        private void MappingPreview_Load(object sender, EventArgs e)
        {
            var mapping = new Mapping(_dsnName);
            var previewTable = mapping.PreviewMapping(_tableName);

            previewDataGridView.DataSource = previewTable.DefaultView;
            previewDataGridView.AutoGenerateColumns = true;
        }
    }
}
