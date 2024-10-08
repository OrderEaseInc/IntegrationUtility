using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Globalization;
using DataTransfer.AccessDatabase;
using DataTransfer.AccessDatabase.Models;

// ReSharper disable once CheckNamespace
namespace LinkGreenODBCUtility
{
    internal class Tasks
    {
        public string Task;
        private DateTime? _executionStartDateTime;

        public Tasks(string task = null)
        {
            Task = task;
        }

        public bool CreateTask(string taskName, string displayName, DateTime startDateTime, int repeatInterval,
            string externalExecutable, string jobParameters)
        {
            var taskCreated = false;

            using (var connection = new OleDbConnectionInstance(Settings.ConnectionString).GetConnection())
            using (var command = new OleDbCommand(
                           $"INSERT INTO Tasks (TaskName, TaskDisplayName, StartDateTime, MinuteRepeatInterval, externalExecutable, JobParameters) VALUES ('{taskName}', '{displayName}', '{startDateTime.ToString(CultureInfo.InvariantCulture)}', {repeatInterval}, '{externalExecutable.Replace("'", "''")}', '{jobParameters.Replace("'", "''")}')")
            { Connection = connection })
            {

                connection.Open();
                try
                {
                    command.ExecuteNonQuery();
                    Logger.Instance.Debug($"Task saved: '{taskName}'");

                    taskCreated = true;
                }
                catch (Exception)
                {
                    Logger.Instance.Error($"An error occurred while creating the task {taskName}.");
                }
                finally
                {
                    command.Dispose();

                    connection.Close();
                    connection.Dispose();
                }
            }

            return taskCreated;
        }

        public bool SetStatus(string taskName, string status)
        {
            var statusSet = false;

            using (var connection = new OleDbConnectionInstance(Settings.ConnectionString).GetConnection())
            using (var command = new OleDbCommand($"UPDATE Tasks SET Status = '{status}' WHERE TaskName = '{taskName}'")
            { Connection = connection })
            {

                connection.Open();
                try
                {
                    command.ExecuteNonQuery();
                    Logger.Instance.Debug($"Task status updated: '{taskName}' {status}");

                    statusSet = true;
                }
                catch (Exception)
                {
                    Logger.Instance.Error($"An error occurred while updating the status of task {taskName}.");
                }
                finally
                {
                    command.Dispose();

                    connection.Close();
                    connection.Dispose();
                }
            }

            return statusSet;
        }

        public bool StartTask(string taskName)
        {
            var dateNow = DateTime.Now;
            var now = dateNow.ToString(CultureInfo.InvariantCulture);
            using (var connection = new OleDbConnectionInstance(Settings.ConnectionString).GetConnection())
            using (var command =
                   new OleDbCommand($"UPDATE Tasks SET ExecutionStartDateTime = '{now}' WHERE TaskName = '{taskName}'")
                   {
                       Connection = connection
                   })
            {

                connection.Open();
                try
                {
                    _executionStartDateTime = dateNow;
                    command.ExecuteNonQuery();

                    return true;
                }
                catch (Exception)
                {
                    Logger.Instance.Error($"An error occurred while updating the start datetime of task {taskName}.");
                }
                finally
                {
                    command.Dispose();

                    connection.Close();
                    connection.Dispose();
                }
            }

            return false;
        }

        public bool EndTask(string taskName)
        {
            var now = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            using (var connection = new OleDbConnectionInstance(Settings.ConnectionString).GetConnection())
            using (var command = new OleDbCommand { Connection = connection })
            {
                if (_executionStartDateTime != null)
                {
                    var executionDuration = DateTime.Now - (_executionStartDateTime ?? DateTime.Now);
                    var executionDurationSeconds = executionDuration.TotalSeconds;

                    command.CommandText =
                        $"UPDATE Tasks SET ExecutionEndDateTime = '{now}', ExecutionDuration = {executionDurationSeconds} WHERE TaskName = '{taskName}'";
                }
                else
                {
                    command.CommandText =
                        $"UPDATE Tasks SET ExecutionEndDateTime = '{now}' WHERE TaskName = '{taskName}'";
                }

                connection.Open();
                try
                {
                    command.ExecuteNonQuery();

                    return true;
                }
                catch (Exception)
                {
                    Logger.Instance.Error($"An error occurred while updating the end datetime of task {taskName}.");
                }
                finally
                {
                    command.Dispose();

                    connection.Close();
                    connection.Dispose();
                }
            }

            return false;
        }

        public bool DeleteTask(string taskName)
        {
            using (var connection = new OleDbConnectionInstance(Settings.ConnectionString).GetConnection())
            using (var command = new OleDbCommand($"DELETE FROM Tasks WHERE TaskName = '{taskName}'")
            { Connection = connection })
            {

                connection.Open();
                try
                {
                    command.ExecuteNonQuery();
                    Logger.Instance.Debug($"Task deleted: '{taskName}'");

                    return true;
                }
                catch (Exception)
                {
                    Logger.Instance.Error($"An error occurred while deleting the task {taskName}.");
                }
                finally
                {
                    command.Dispose();

                    connection.Close();
                    connection.Dispose();
                }
            }

            return false;
        }

        public IEnumerable<Task> GetAll()
        {
            var taskRepo = new TaskRepository(Settings.ConnectionString);
            return taskRepo.GetAll();
        }

        public Task GetTask(string taskName)
        {
            var taskRepo = new TaskRepository(Settings.ConnectionString);
            return taskRepo.GetTask(taskName);
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
            if (string.IsNullOrEmpty(task) && string.IsNullOrEmpty(Task)) return false;


            if (string.IsNullOrEmpty(task))
            {
                task = Task;
            }

            var taskExists = false;

            using (var connection = new OleDbConnectionInstance(Settings.ConnectionString).GetConnection())
            using (var command = new OleDbCommand($"SELECT * FROM `Tasks` WHERE TaskName = '{task}'", connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {

                    try
                    {
                        if (reader.Read())
                            taskExists = true;
                    }
                    finally
                    {
                        reader.Close();
                        reader.Dispose();

                        command.Dispose();

                        connection.Close();
                        connection.Dispose();
                    }
                }
            }

            return taskExists;
        }
    }
}
