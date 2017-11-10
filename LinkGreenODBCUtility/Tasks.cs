using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using DataTransfer.AccessDatabase;
using DataTransfer.AccessDatabase.Models;

namespace LinkGreenODBCUtility
{
    class Tasks
    {
        public string Task;

        public Tasks(string task = null)
        {
            Task = task;
        }

        public bool CreateTask(string taskName, string displayName, DateTime startDateTime, int repeatInterval)
        {
            var _connection = new OdbcConnection();
            _connection.ConnectionString = $"DSN={Settings.DsnName}";
            var command = new OdbcCommand($"INSERT INTO Tasks (TaskName, TaskDisplayName, StartDateTime, MinuteRepeatInterval) VALUES ('{taskName}', '{displayName}', '{startDateTime.ToString()}', {repeatInterval})")
            {
                Connection = _connection
            };

            _connection.Open();
            try
            {
                command.ExecuteNonQuery();
                Logger.Instance.Debug($"Task saved: '{taskName}'");

                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.Error($"An error occured while creating the task {taskName}.");
            }
            finally
            {
                _connection.Close();
            }

            return false;
        }

        public bool DeleteTask(string taskName)
        {
            var _connection = new OdbcConnection();
            _connection.ConnectionString = $"DSN={Settings.DsnName}";
            var command = new OdbcCommand($"DELETE FROM Tasks WHERE TaskName = '{taskName}'")
            {
                Connection = _connection
            };

            _connection.Open();
            try
            {
                command.ExecuteNonQuery();
                Logger.Instance.Debug($"Task deleted: '{taskName}'");

                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.Error($"An error occured while deleting the task {taskName}.");
            }
            finally
            {
                _connection.Close();
            }

            return false;
        }

        public IEnumerable<Task> GetAll()
        {
            var TaskRepo = new TaskRepository($"DSN={Settings.DsnName}");
            return TaskRepo.GetAll();
        }

        public void RestoreTasks()
        {
            IEnumerable<Task> savedTasks = GetAll();

            foreach (Task task in savedTasks)
            {
                JobManager.ScheduleJob(task.TaskName, task.StartDateTime, task.RepeatInterval);
            }
        }

        public bool TaskExists(string task = null)
        {
            if (!string.IsNullOrEmpty(task) || !string.IsNullOrEmpty(Task))
            {
                if (string.IsNullOrEmpty(task))
                {
                    task = Task;
                }
                var _connection = new OdbcConnection();
                _connection.ConnectionString = $"DSN={Settings.DsnName}";
                var command = new OdbcCommand($"SELECT * FROM `Tasks` WHERE TaskName = '{task}'", _connection);
                _connection.Open();
                OdbcDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        return true;
                    }

                    return false;
                }
                finally
                {
                    reader.Close();
                    _connection.Close();
                }
            }

            return false;
        }
    }
}
