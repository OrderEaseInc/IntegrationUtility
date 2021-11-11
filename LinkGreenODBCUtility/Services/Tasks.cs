using System;
using System.Collections.Generic;
using System.Data.OleDb;

using DataTransfer.AccessDatabase;
using DataTransfer.AccessDatabase.Models;

namespace LinkGreenODBCUtility
{
    class Tasks
    {
        public string Task;
        private DateTime? _executionStartDateTime;

        public Tasks(string task = null)
        {
            Task = task;
        }

        public bool CreateTask(string taskName, string displayName, DateTime startDateTime, int repeatInterval, string externalExecutable, string jobParameters)
        {
            // var _connection = ConnectionInstance.Instance.GetConnection($"DSN={Settings.DsnName}");
            var _connection = new OleDbConnectionInstance(Settings.ConnectionString).GetConnection();
            var command = new OleDbCommand($"INSERT INTO Tasks (TaskName, TaskDisplayName, StartDateTime, MinuteRepeatInterval, externalExecutable, JobParameters) VALUES ('{taskName}', '{displayName}', '{startDateTime.ToString()}', {repeatInterval}, '{externalExecutable.Replace("'", "''")}', '{jobParameters.Replace("'", "''")}')")
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
            catch (Exception)
            {
                Logger.Instance.Error($"An error occured while creating the task {taskName}.");
            }
            finally
            {
                _connection.Close();
            }

            return false;
        }

        public bool SetStatus(string taskName, string status)
        {
            var now = DateTime.Now.ToString();
            var _connection = new OleDbConnectionInstance(Settings.ConnectionString).GetConnection();
            var command = new OleDbCommand($"UPDATE Tasks SET Status = '{status}' WHERE TaskName = '{taskName}'")
            {
                Connection = _connection
            };

            _connection.Open();
            try
            {
                command.ExecuteNonQuery();
                Logger.Instance.Debug($"Task status updated: '{taskName}' {status}");

                return true;
            }
            catch (Exception)
            {
                Logger.Instance.Error($"An error occured while updating the status of task {taskName}.");
            }
            finally
            {
                _connection.Close();
            }

            return false;
        }

        public bool StartTask(string taskName)
        {
            var dateNow = DateTime.Now;
            var now = dateNow.ToString();
            var _connection = new OleDbConnectionInstance(Settings.ConnectionString).GetConnection();
            var command = new OleDbCommand($"UPDATE Tasks SET ExecutionStartDateTime = '{now}' WHERE TaskName = '{taskName}'")
            {
                Connection = _connection
            };

            _connection.Open();
            try
            {
                _executionStartDateTime = dateNow;
                command.ExecuteNonQuery();

                return true;
            }
            catch (Exception)
            {
                Logger.Instance.Error($"An error occured while updating the start datetime of task {taskName}.");
            }
            finally
            {
                _connection.Close();
            }

            return false;
        }

        public bool EndTask(string taskName)
        {
            var now = DateTime.Now.ToString();
            var _connection = new OleDbConnectionInstance(Settings.ConnectionString).GetConnection();
            var command = new OleDbCommand()
            {
                Connection = _connection
            };
            if (_executionStartDateTime != null)
            {
                TimeSpan executionDuration = DateTime.Now - (_executionStartDateTime ?? DateTime.Now);
                double executionDurationSeconds = executionDuration.TotalSeconds;

                command.CommandText = $"UPDATE Tasks SET ExecutionEndDateTime = '{now}', ExecutionDuration = {executionDurationSeconds} WHERE TaskName = '{taskName}'";
            }
            else
            {
                command.CommandText = $"UPDATE Tasks SET ExecutionEndDateTime = '{now}' WHERE TaskName = '{taskName}'";
            }

            _connection.Open();
            try
            {
                command.ExecuteNonQuery();

                return true;
            }
            catch (Exception)
            {
                Logger.Instance.Error($"An error occured while updating the end datetime of task {taskName}.");
            }
            finally
            {
                _connection.Close();
            }

            return false;
        }

        public bool DeleteTask(string taskName)
        {
            var _connection = new OleDbConnectionInstance(Settings.ConnectionString).GetConnection();
            var command = new OleDbCommand($"DELETE FROM Tasks WHERE TaskName = '{taskName}'")
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
            catch (Exception)
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
            var TaskRepo = new TaskRepository(Settings.ConnectionString);
            return TaskRepo.GetAll();
        }

        public Task GetTask(string taskName)
        {
            var TaskRepo = new TaskRepository(Settings.ConnectionString);
            return TaskRepo.GetTask(taskName);
        }

        public void RestoreTasks()
        {
            var savedTasks = GetAll();

            foreach (var task in savedTasks)
            {
                JobManager.ScheduleJob(task.TaskName, task.StartDateTime, task.RepeatInterval, task.ExternalExecutable, task.JobParameters);
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

                var _connection = new OleDbConnectionInstance(Settings.ConnectionString).GetConnection();
                var command = new OleDbCommand($"SELECT * FROM `Tasks` WHERE TaskName = '{task}'", _connection);
                _connection.Open();
                var reader = command.ExecuteReader();
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
