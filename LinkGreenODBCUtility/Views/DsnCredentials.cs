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
    public partial class DsnCredentials : Form
    {
        private static string _dsn;
        public DsnCredentials(string dsn)
        {
            _dsn = dsn;
            InitializeComponent();
        }

        private void DsnCredentials_Load(object sender, EventArgs e)
        {
            username.Text = DsnCreds.GetDsnCreds(_dsn)?.Username ?? "";
        }

        private void save_Click(object sender, EventArgs e)
        {
            var categories = new Categories();
            if (!string.IsNullOrEmpty(username.Text) && !string.IsNullOrEmpty(password.Text))
            {
                DsnCreds.SaveDsnCreds(_dsn, username.Text, password.Text);
                ActiveForm.Close();
            }
            else
            {
                MessageBox.Show("Username & Password must be set.", "Missing Username or Password");
            }
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            ActiveForm.Close();
        }
    }
}
