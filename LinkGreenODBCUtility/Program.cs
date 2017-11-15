using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataTransfer.AccessDatabase;

namespace LinkGreenODBCUtility
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Init();
        }

        // Use this Init if adding the DSN is handled during installation setup
//        private static void Init()
//        {
//            if (Settings.TryConnect())
//            {
//                Application.EnableVisualStyles();
//                Application.SetCompatibleTextRenderingDefault(false);
//                Application.Run(new UtilityDashboard());
//            }
//            else
//            {
//                MessageBox.Show("Failed to connect to LinkGreenDataTransfer DSN", "Connection Failed");
//            }
//        }

        private static void Init()
        {
            //            DialogResult dialogResult = MessageBox.Show("Are you running as administrator?", "Must be run as Administrator", MessageBoxButtons.YesNo);
            //            if (dialogResult == DialogResult.Yes)
            //            {
            //                // do something
            //            }
            //            else if (dialogResult == DialogResult.No)
            //            {
            //                Environment.Exit(0);
            //            }

            string dsnName = Settings.DsnName;
            string dsPath = AppDomain.CurrentDomain.BaseDirectory + $"{Settings.DsnName}.mdb";
            bool success = Utils.CreateDataSource((IntPtr)0,
                ODBC_Request_Modes.ODBC_ADD_SYS_DSN,
                "Microsoft Access Driver (*.mdb)\0",
                "DSN=" + dsnName + "\0DBQ=" + dsPath + "\0");
            if (success)
            {
                if (Settings.TryConnect())
                {
                    string logDsnName = Logger._loggerDsnName;
                    string logDsPath = AppDomain.CurrentDomain.BaseDirectory + $"{logDsnName}.mdb";
                    bool logConnectSuccess = Utils.CreateDataSource((IntPtr)0,
                        ODBC_Request_Modes.ODBC_ADD_SYS_DSN,
                        "Microsoft Access Driver (*.mdb)\0",
                        "DSN=" + logDsnName + "\0DBQ=" + logDsPath + "\0");
                    if (logConnectSuccess)
                    {
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        Application.Run(new UtilityDashboard());
                    }
                    else
                    {
                        MessageBox.Show($"Failed to create {logDsnName} DSN", "DSN Creation Failed");
                    }
                }
                else
                {
                    MessageBox.Show($"Failed to connect to {Settings.DsnName} DSN", "Connection Failed");
                }
            }
            else
            {
                MessageBox.Show($"Failed to create {Settings.DsnName} DSN", "DSN Creation Failed");
            }
        }

        public static class ODBC_Request_Modes
        {
            public static int ODBC_ADD_DSN = 1;
            public static int ODBC_CONFIG_DSN = 2;
            public static int ODBC_REMOVE_DSN = 3;
            public static int ODBC_ADD_SYS_DSN = 4;
            public static int ODBC_CONFIG_SYS_DSN = 5;
            public static int ODBC_REMOVE_SYS_DSN = 6;
        }

        private static class Utils
        {
            /// <summary>
            /// Win32 API Imports
            /// </summary>
            [DllImport("ODBCCP32.dll")]
            private static extern bool SQLConfigDataSource(IntPtr hwndParent, int fRequest, string lpszDriver, string lpszAttributes);

            public static bool CreateDataSource(IntPtr hwndParent,
                int fRequest,
                string lpszDriver,
                string lpszAttributes)
            {
                return SQLConfigDataSource(hwndParent,
                    fRequest,
                    lpszDriver,
                    lpszAttributes);
            }
        }
    }
}
