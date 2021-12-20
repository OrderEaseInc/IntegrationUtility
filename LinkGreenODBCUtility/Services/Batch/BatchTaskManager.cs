using System.Collections.Generic;
using System.Data.OleDb;
using DataTransfer.AccessDatabase;

// ReSharper disable once CheckNamespace
namespace LinkGreenODBCUtility
{
    internal class BatchTaskManager
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
            using (var connection = new OleDbConnectionInstance(Settings.ConnectionString).GetConnection())
            using (var command =
                   new OleDbCommand(
                       $"SELECT `Command` FROM `BatchTasks` WHERE (`Trigger` = '{Trigger}' OR `Task` = '{Task}') AND `Priority` <> -1 ORDER BY `Priority` DESC",
                       connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
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
                        connection.Close();
                    }
                }
            }
        }
    }
}
