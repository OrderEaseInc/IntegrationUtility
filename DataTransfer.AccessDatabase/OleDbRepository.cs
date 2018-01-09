using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

namespace DataTransfer.AccessDatabase
{
    public abstract class OleDbRepository<T> where T : class
    {
        private readonly string _connectionString;

        protected OleDbRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected virtual T PopulateRecord(dynamic reader)
        {
            return null;
        }

        public virtual void SaveTableMapping(string dsnName, string tableName, string linkGreenTableName)
        {
            using (var command = new OleDbCommand($"DELETE * FROM `TableMappings` WHERE `TableName` = '{linkGreenTableName}'"))
            {
                ExecuteCommand(command);
            }
            using (var command = new OleDbCommand($"INSERT INTO `TableMappings` (`DsnName`, `TableName`, `MappingName`) VALUES ('{dsnName}', '{linkGreenTableName}', '{tableName}')"))
            {
                ExecuteCommand(command);
            }
        }

        public virtual void SaveFieldMapping(string fieldName, string mappingName)
        {
            throw new NotImplementedException();
        }

        protected IEnumerable<T> GetRecords(OleDbCommand command)
        {
            var list = new List<T>();

            command.Connection = new OleDbConnectionInstance(_connectionString).GetConnection();
            command.Connection.Open();
            try
            {
                dynamic reader = new DynamicDataReader(command.ExecuteReader());
                //var reader = command.ExecuteReader();

                try
                {
                    while (reader.Read())
                        list.Add(PopulateRecord(reader));
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }
            finally
            {
                command.Connection.Close();
            }

            return list;
        }

        protected T GetRecord(OleDbCommand command)
        {
            T record = null;
            command.Connection = new OleDbConnectionInstance(_connectionString).GetConnection();
            command.Connection.Open();
            try
            {
                //var reader = command.ExecuteReader();
                dynamic reader = new DynamicDataReader(command.ExecuteReader());

                try
                {
                    while (reader.Read())
                    {
                        record = PopulateRecord(reader);
                        break;
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }
            finally
            {
                command.Connection.Close();
            }
            return record;
        }
        protected IEnumerable<T> ExecuteStoredProc(OleDbCommand command)
        {
            var list = new List<T>();
            command.Connection = new OleDbConnectionInstance(_connectionString).GetConnection();
            command.CommandType = CommandType.StoredProcedure;
            command.Connection.Open();
            try
            {
                //var reader = command.ExecuteReader();
                dynamic reader = new DynamicDataReader(command.ExecuteReader());

                try
                {
                    while (reader.Read())
                    {
                        var record = PopulateRecord(reader);
                        if (record != null) list.Add(record);
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }
            finally
            {
                command.Connection.Close();
            }
            return list;
        }

        protected void ExecuteCommand(OleDbCommand command)
        {
            command.Connection = new OleDbConnectionInstance(_connectionString).GetConnection();
            command.CommandType = CommandType.Text;
            command.Connection.Open();
            try
            {
                command.ExecuteNonQuery();
            }
            finally
            {
                command.Connection.Close();
            }
        }

        public static string NullableString(string value) => value == null ? "null" : $"'{value.Replace("'", "''").Replace("\"", "\\\"")}'";
        public static string NullableInt(int? value) => value.HasValue ? value.ToString() : "null";
        public static string NullableDecimal(decimal? value) => value.HasValue ? value.ToString() : "null";
    }
}