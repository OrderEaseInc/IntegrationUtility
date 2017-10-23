using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkGreenODBCUtility
{
    class Log
    {
        private static string LogTable = "Log";
        private static string DsnName = Settings.DsnName;
        private static DateTime DeadDate = DateTime.Now.AddDays(-30);

        public Log()
        {
            PurgeLog();
        }

        public static void PurgeLog()
        {
            var _connection = new OdbcConnection();
            _connection.ConnectionString = "DSN=" + DsnName;
            var command = new OdbcCommand($"DELETE * FROM `{LogTable}` WHERE `Timestamp` < {DeadDate.ToOADate()}")
            {
                Connection = _connection
            };

            _connection.Open();
            try
            {
                int affectedRows = command.ExecuteNonQuery();

                if (affectedRows > 0)
                {
                    Logger.Instance.Debug($"Purged {affectedRows} log entries that had timestamps before {DeadDate}.");
                }
            }
            catch (OdbcException e)
            {
                Logger.Instance.Error($"An error occured while purging log entries with timestamps before {DeadDate}: {e.Message}");
            }
            finally
            {
                _connection.Close();
            }
        }

        public DataTable LoadLog()
        {
            var _connection = new OdbcConnection();
            _connection.ConnectionString = "DSN=" + DsnName;

            string query = $"SELECT `Timestamp`, `Level`, `Message` FROM `{LogTable}` WHERE `Level` NOT LIKE 'DEBUG' ORDER BY `Timestamp` DESC";
            if (Settings.DebugMode)
            {
                query = $"SELECT `Timestamp`, `Level`, `Message` FROM `{LogTable}` ORDER BY `Timestamp` DESC";
            }
            _connection.Open();
            OdbcDataAdapter adapter = new OdbcDataAdapter(query, _connection);
            DataTable table = new DataTable();

            try
            {
                adapter.Fill(table);
                table.DefaultView.AllowDelete = false;
                table.DefaultView.AllowEdit = false;
                table.DefaultView.AllowNew = false;

                return table;
            }
            catch (Exception e)
            {
                Logger.Instance.Error($"An error occured while retrieving the log: {e.Message}");
            }
            finally
            {
                _connection.Close();
            }

            var mapping = new Mapping();
            List<string> logColumns = mapping.GetColumns(LogTable, DsnName);
            DataTable logTable = new DataTable();

            foreach (string logColumn in logColumns)
            {
                logTable.Columns.Add(logColumn);
            }

            logTable.DefaultView.AllowDelete = false;
            logTable.DefaultView.AllowEdit = false;
            logTable.DefaultView.AllowNew = false;

            return logTable;
        }
    }
}
