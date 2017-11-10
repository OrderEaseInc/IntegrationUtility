using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using DataTransfer.AccessDatabase.Models;
using Microsoft.CSharp.RuntimeBinder;

namespace DataTransfer.AccessDatabase
{
    public class TaskRepository : AdoRepository<Task>
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
            using (var command = new OdbcCommand($"SELECT TaskName, TaskDisplayName, StartDateTime, MinuteRepeatInterval FROM {TableName}"))
            {
                return GetRecords(command);
            }
        }

        #endregion

        public void ClearAll()
        {
            using (var command = new OdbcCommand($"DELETE * FROM {TableName}"))
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
                    RepeatInterval = reader.MinuteRepeatInterval
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