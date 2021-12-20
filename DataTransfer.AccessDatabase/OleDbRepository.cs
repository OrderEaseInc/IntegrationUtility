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
                using (var dataReader = command.ExecuteReader())
                {
                    dynamic reader = new DynamicDataReader(dataReader);
                    try
                    {
                        while (reader.Read())
                            list.Add(PopulateRecord(reader));
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                        dataReader.Close();
                    }
                }
            }
            finally
            {
                command.Connection.Close();
                command.Connection.Dispose();
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
                using (var dataReader = command.ExecuteReader())
                {
                    dynamic reader = new DynamicDataReader(dataReader);

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
                        dataReader.Close();
                    }
                }
            }
            finally
            {
                command.Connection.Close();
                command.Connection.Dispose();
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
                using (var dataReader = command.ExecuteReader())
                {
                    dynamic reader = new DynamicDataReader(dataReader);

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
                        dataReader.Close();
                    }
                }
            }
            finally
            {
                command.Connection.Close();
                command.Connection.Dispose();
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
                command.Connection.Dispose();
            }
        }

        public static string NullableString(string value) => value == null ? "null" : $"'{value.Replace("'", "''").Replace("\"", "\\\"")}'";
        public static string NullableInt(int? value) => value.HasValue ? value.ToString() : "null";
        public static string NullableDecimal(decimal? value) => value.HasValue ? value.ToString() : "null";
        public static string Date(DateTime value) => $"'{value:yyyy-MM-dd}'";
        public static string NullableDate(DateTime? value) => value.HasValue ? Date(value.Value) : "null";
        public static string Boolean(bool value) => value ? "1" : "0";
    }
}