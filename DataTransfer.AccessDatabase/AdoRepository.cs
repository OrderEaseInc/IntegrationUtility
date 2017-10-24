using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;

namespace DataTransfer.AccessDatabase
{
    public abstract class AdoRepository<T> where T : class
    {
        // ReSharper disable once StaticMemberInGenericType b/c it's a repository per type we can definitely keep one connection per repo
        private static OdbcConnection _connection;

        protected AdoRepository(string connectionString)
        {
            _connection = new OdbcConnection(connectionString);
        }

        protected static OdbcConnection Connection => _connection;

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
            command.Connection = _connection;
            _connection.Open();
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
                _connection.Close();
            }
            return list;
        }

        protected T GetRecord(OdbcCommand command)
        {
            T record = null;
            command.Connection = _connection;
            _connection.Open();
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
                _connection.Close();
            }
            return record;
        }
        protected IEnumerable<T> ExecuteStoredProc(OdbcCommand command)
        {
            var list = new List<T>();
            command.Connection = _connection;
            command.CommandType = CommandType.StoredProcedure;
            _connection.Open();
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
                _connection.Close();
            }
            return list;
        }

        protected void ExecuteCommand(OdbcCommand command)
        {
            command.Connection = _connection;
            command.CommandType = CommandType.Text;
            _connection.Open();
            try
            {
                command.ExecuteNonQuery();
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}