using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Windows.Forms;
using LinkGreenODBCUtility.RunnableActivities;

namespace LinkGreenODBCUtility
{
    static class Program
    {
        public const string ServiceName = "LinkGreen ODBC Sync Service";
        public class TaskService : ServiceBase
        {
            public TaskService()
            {
                ServiceName = Program.ServiceName;
            }

            protected override void OnStart(string[] args)
            {
                var tasks = new Tasks();
                tasks.RestoreTasks();

                base.OnStart(args);
            }

            protected override void OnStop()
            {
                base.OnStop();
                JobManager.Dispose();
            }

            protected override void OnPause()
            {
                base.OnPause();
                var tasks = new Tasks();
                tasks.RestoreTasks();
            }

            protected override void OnContinue()
            {
                var tasks = new Tasks();
                tasks.RestoreTasks();

                base.OnContinue();
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
            Init(args);
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

        private static void Init(string[] args)
        {
            // Uncomment next line to enable debugging of a service
            // System.Diagnostics.Debugger.Launch();

            var isService = args.Any(a => a.Equals("/service", StringComparison.OrdinalIgnoreCase));

            var isCommandLine = args.Any(a => a.ToLower().StartsWith("/run="));

            Utility.Mapper.InitMapper();

            string dsnName = Settings.ConnectViaDsnName;
            string dsPath = AppDomain.CurrentDomain.BaseDirectory + $"{Settings.ConnectViaDsnName}.mdb";
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
                        if (isService)
                        {
                            using (var service = new TaskService())
                            {
                                ServiceBase.Run(service);
                                return;
                            }
                        }

                        if (isCommandLine)
                        {
                            RunAsCommandLine(args);
                            return;
                        }


                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        Application.Run(new UtilityDashboard());
                    }
                    else
                    {
                        if (isService)
                            WriteToEventLog($@"Failed to create {logDsnName} DSN");
                        else
                            MessageBox.Show($@"Failed to create {logDsnName} DSN", @"Emptied Successfully");
                    }
                }
                else
                {
                    if (isService)
                        WriteToEventLog($@"Failed to connect to {Settings.ConnectionString} DSN");
                    else
                        MessageBox.Show($@"Failed to connect to {Settings.ConnectionString} DSN", @"Emptied Successfully");
                }
            }
            else
            {
                if (isService)
                    WriteToEventLog($@"Failed to create {Settings.ConnectionString} DSN");
                else
                    MessageBox.Show($@"Failed to create {Settings.ConnectionString} DSN", @"Emptied Successfully");
            }

        }

        private static void RunAsCommandLine(IEnumerable<string> args)
        {
            var cli = args.First(a => a.ToLower().StartsWith("/run="));
            var split = cli.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length < 2)
                return;
            var toRun = split[1].Trim().ToLower();

            IRunnableActivity cliActivity = null;
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (toRun)
            {
                case "orders":
                    cliActivity = new DownloadOrderRunner();
                    break;
                case "products":
                    break;
                case "customers":
                    break;
                // ReSharper disable once StringLiteralTypo
                case "pricelevels":
                    break;
                case "prices":
                    break;
            }

            var bw = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = false
            };

            bw.ProgressChanged += (bwSender, bwEventArgs) =>
            {
                Console.WriteLine(bwEventArgs.UserState.ToString());
            };

            if (cliActivity != null)
            {
                var result = cliActivity.Run(bw);
                Console.WriteLine(result.Message);
                WriteToEventLog(result.Message);
                if (result.Error != null)
                {
                    WriteToEventLog(result.Error);
                    Console.WriteLine(result.Error);
                }
            }
            else
            {
                Console.WriteLine(@"This CLI command is not enabled yet.");
            }
        }

        private static void WriteToEventLog(string message)
        {
            if (!System.Diagnostics.EventLog.SourceExists(ServiceName))
                System.Diagnostics.EventLog.CreateEventSource(ServiceName, "Application");

            System.Diagnostics.EventLog.WriteEntry(ServiceName, message, EventLogEntryType.Warning);
        }


        // ReSharper disable InconsistentNaming
        // ReSharper disable UnusedMember.Global
        public static class ODBC_Request_Modes
        {
            public static int ODBC_ADD_DSN = 1;
            public static int ODBC_CONFIG_DSN = 2;
            public static int ODBC_REMOVE_DSN = 3;
            public static int ODBC_ADD_SYS_DSN = 4;
            public static int ODBC_CONFIG_SYS_DSN = 5;
            public static int ODBC_REMOVE_SYS_DSN = 6;
        }
        // ReSharper restore InconsistentNaming
        // ReSharper restore UnusedMember.Global

        private static class Utils
        {
            /// <summary>
            /// Win32 API Imports
            /// </summary>
            // ReSharper disable once StringLiteralTypo
            [DllImport("ODBCCP32.dll")]
            // ReSharper disable once IdentifierTypo
            private static extern bool SQLConfigDataSource(IntPtr hwndParent, int fRequest, string lpszDriver, string lpszAttributes);

            // ReSharper disable once IdentifierTypo
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
