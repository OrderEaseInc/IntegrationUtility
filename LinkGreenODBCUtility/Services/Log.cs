using System;
using System.Data;
using System.Data.Odbc;
using DataTransfer.AccessDatabase;

namespace LinkGreenODBCUtility
{
    class Log
    {
        private static readonly string LogTable = "Log";
        private static readonly string DsnName = Logger._loggerDsnName;
        private static DateTime _deadDate = DateTime.Now.AddDays(-15);

        public Log()
        {
            PurgeLog();
        }

        public static void PurgeLog()
        {
            var connection = ConnectionInstance.Instance.GetConnection($"DSN={DsnName}");
            using (var command =
                   new OdbcCommand($"DELETE * FROM `{LogTable}` WHERE `Timestamp` < {_deadDate.ToOADate()}")
                   { Connection = connection })
            {
                connection.Open();
                try
                {
                    var affectedRows = command.ExecuteNonQuery();

                    if (affectedRows > 0)
                    {
                        Logger.Instance.Debug(
                            $"Purged {affectedRows} log entries that had timestamps before {_deadDate}.");
                    }
                }
                catch (OdbcException e)
                {
                    Logger.Instance.Error(
                        $"An error occurred while purging log entries with timestamps before {_deadDate}: {e.Message}");
                }
                finally
                {
                    ConnectionInstance.CloseConnection($"DSN={DsnName}");
                }
            }
        }

        public DataTable LoadLog()
        {
            var connection = ConnectionInstance.Instance.GetConnection($"DSN={DsnName}");
            var query = $"SELECT `Timestamp`, `Level`, `Message` FROM `{LogTable}` WHERE `Level` NOT LIKE 'DEBUG' ORDER BY `Timestamp` DESC";
            if (Settings.DebugMode)
            {
                query = $"SELECT `Timestamp`, `Level`, `Message` FROM `{LogTable}` ORDER BY `Timestamp` DESC";
            }
            connection.Open();
            using (var adapter = new OdbcDataAdapter(query, connection))
            {
                var table = new DataTable();
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
                    Logger.Instance.Error($"An error occurred while retrieving the log: {e.Message}");
                }
                finally
                {
                    ConnectionInstance.CloseConnection($"DSN={DsnName}");
                }
            }

            var mapping = new Mapping();
            var logColumns = mapping.GetColumns(LogTable, DsnName);
            var logTable = new DataTable();

            foreach (var logColumn in logColumns)
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
