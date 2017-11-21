using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;

namespace DataTransfer.AccessDatabase
{
    public abstract class AdoRepository<T> where T : class
    {
        private OdbcConnection Connection;
        private readonly string _connectionString;

        protected AdoRepository(string connectionString)
        {
            _connectionString = connectionString;
            Connection = ConnectionInstance.Instance.GetConnection(_connectionString);
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

        protected IEnumerable<T> GetRecords(OdbcCommand command)
        {
            var list = new List<T>();

            command.Connection = Connection;
            Connection.Open();
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
                ConnectionInstance.CloseConnection(_connectionString);
            }
            return list;
        }

        protected T GetRecord(OdbcCommand command)
        {
            T record = null;
            command.Connection = Connection;
            Connection.Open();
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
            command.Connection = Connection;
            command.CommandType = CommandType.StoredProcedure;
            Connection.Open();
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
            command.Connection = Connection;
            command.CommandType = CommandType.Text;
            Connection.Open();
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