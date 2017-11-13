using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransfer.AccessDatabase;

namespace LinkGreenODBCUtility
{
    class BatchTaskManager
    {
        public string Trigger;
        public string Task;

        public BatchTaskManager(string trigger, string task = null)
        {
            Trigger = trigger;
            Task = task;
        }

        public List<string> GetCommandsByTrigger()
        {
            var _connection = ConnectionInstance.GetConnection($"DSN={Settings.DsnName}");
            var command = new OdbcCommand($"SELECT `Command` FROM `BatchTasks` WHERE (`Trigger` = '{Trigger}' OR `Task` = '{Task}') AND `Priority` <> -1 ORDER BY `Priority` DESC", _connection);
            _connection.Open();
            OdbcDataReader reader = command.ExecuteReader();
            try
            {
                List<string> commands = new List<string>();
                while (reader.Read())
                {
                    commands.Add(reader[0].ToString());
                }

                return commands;
            }
            finally
            {
                reader.Close();
                _connection.Close();
            }
        }
    }
}
