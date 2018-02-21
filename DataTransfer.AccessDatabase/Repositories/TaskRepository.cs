using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using DataTransfer.AccessDatabase.Models;
using Microsoft.CSharp.RuntimeBinder;

namespace DataTransfer.AccessDatabase
{
    public class TaskRepository : OleDbRepository<Task>
    {
        public TaskRepository(string connectionString) : base(connectionString)
        {
        }

        #region Private Consts

        private const string TableName = "Tasks";

        #endregion

        #region Get

        public IEnumerable<Task> GetAll()
        {
            // DBAs across the country are having strokes 
            //  over this next command!
            using (var command = new OleDbCommand($"SELECT " +
                                                 $"TaskName, " +
                                                 $"TaskDisplayName, " +
                                                 $"StartDateTime, " +
                                                 $"MinuteRepeatInterval, " +
                                                 $"Status, " +
                                                 $"ExecutionStartDateTime, " +
                                                 $"ExecutionEndDateTime, " +
                                                 $"ExecutionDuration, " +
                                                 $"JobParameters, " +
                                                 $"ExternalExecutable " +
                                                 $"FROM {TableName}"))
            {
                return GetRecords(command);
            }
        }

        public Task GetTask(string taskName)
        {
            using (var command = new OleDbCommand($"SELECT " +
                                                 $"TaskName, " +
                                                 $"TaskDisplayName, " +
                                                 $"StartDateTime, " +
                                                 $"MinuteRepeatInterval, " +
                                                 $"Status, " +
                                                 $"ExecutionStartDateTime, " +
                                                 $"ExecutionEndDateTime, " +
                                                 $"ExecutionDuration, " +
                                                 $"JobParameters, " +
                                                 $"ExternalExecutable " +
                                                 $"FROM {TableName} WHERE TaskName = '{taskName}'"))
            {
                return GetRecord(command);
            }
        }

        #endregion

        public void ClearAll()
        {
            using (var command = new OleDbCommand($"DELETE * FROM {TableName}"))
            {
                ExecuteCommand(command);
            }
        }

        // NOTE : this is the wire-up of the local odbc table to strongly typed object to be sent via api to LG db
        protected override Task PopulateRecord(dynamic reader)
        {
            try
            {
                return new Task
                {
                    TaskName = reader.TaskName,
                    TaskDisplayName = reader.TaskDisplayName,
                    StartDateTime = reader.StartDateTime,
                    RepeatInterval = reader.MinuteRepeatInterval,
                    LastExecuteStatus = reader.Status,
                    ExecutionStartDateTime = reader.ExecutionStartDateTime,
                    ExecutionEndDateTime = reader.ExecutionEndDateTime,
                    ExecutionDuration = TimeSpan.FromSeconds(reader.ExecutionDuration),
                    ExternalExecutable = reader.ExternalExecutable,
                    JobParameters = reader.JobParameters
                };
            }
            catch (RuntimeBinderException exception)
            {
                Console.WriteLine(exception);
                throw new InvalidDataException("One of the fields in the source ODBC database has an invalid column type or value", exception);
            }
        }
    }
}