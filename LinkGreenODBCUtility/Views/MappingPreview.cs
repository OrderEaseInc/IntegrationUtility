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
    public partial class MappingPreview : Form
    {
        private string TableName;
        private string DsnName;

        public MappingPreview(string tableName, string dsnName)
        {
            InitializeComponent();
            TableName = tableName;
            DsnName = dsnName;
        }

        private void MappingPreview_Load(object sender, EventArgs e)
        {
            var mapping = new Mapping(DsnName);
            DataTable previewTable = mapping.PreviewMapping(TableName);

            previewDataGridView.DataSource = previewTable.DefaultView;
            previewDataGridView.AutoGenerateColumns = true;
        }
    }
}
