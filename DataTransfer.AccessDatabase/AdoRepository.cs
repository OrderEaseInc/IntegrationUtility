using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;

namespace DataTransfer.AccessDatabase
{
    public abstract class AdoRepository<T> where T : class
    {
        private readonly string _connectionString;

        protected AdoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected virtual T PopulateRecord(dynamic reader)
        {
            return null;
        }

        public virtual void SaveTableMapping(string dsnName, string tableName, string linkGreenTableName)
        {
            using (var command = new OdbcCommand($"DELETE * FROM `TableMappings` WHERE `TableName` = '{linkGreenTableName}'"))
            {
                ExecuteCommand(command);
            }
            using (var command = new OdbcCommand($"INSERT INTO `TableMappings` (`DsnName`, `TableName`, `MappingName`) VALUES ('{dsnName}', '{linkGreenTableName}', '{tableName}')"))
            {
                ExecuteCommand(command);
            }
        }

        public virtual void SaveFieldMapping(string fieldName, string mappingName)
        {
            throw new NotImplementedException();
        }

        protected IEnumerable<T> GetRecords(OdbcCommand command, OdbcConnection connection = null)
        {
            var list = new List<T>();

            if (connection == null)
            {
                command.Connection = ConnectionInstance.Instance.GetConnection(_connectionString);
                command.Connection.Open();
            }
            else
            {
                command.Connection = connection;
            }

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
                // using connection string - means we opened the connection, so we should close id
                if (connection != null)
                    ConnectionInstance.CloseConnection(_connectionString);
            }

            return list;
        }

        protected T GetRecord(OdbcCommand command)
        {
            T record = null;
            command.Connection = ConnectionInstance.Instance.GetConnection(_connectionString);
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
                ConnectionInstance.CloseConnection(_connectionString);
            }
            return record;
        }
        protected IEnumerable<T> ExecuteStoredProc(OdbcCommand command)
        {
            var list = new List<T>();
            command.Connection = ConnectionInstance.Instance.GetConnection(_connectionString);
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
                ConnectionInstance.CloseConnection(_connectionString);
            }
            return list;
        }

        protected void ExecuteCommand(OdbcCommand command)
        {
            command.Connection = ConnectionInstance.Instance.GetConnection(_connectionString);
            command.CommandType = CommandType.Text;
            command.Connection.Open();
            try
            {
                command.ExecuteNonQuery();
            }
            finally
            {
                ConnectionInstance.CloseConnection(_connectionString);
            }
        }

        public static string NullableString(string value) => value == null ? "null" : $"'{value.Replace("'", "''").Replace("\"", "\\\"")}'";
        public static string NullableInt(int? value) => value.HasValue ? value.ToString() : "null";
        public static string NullableDecimal(decimal? value) => value.HasValue ? value.ToString() : "null";
    }
}