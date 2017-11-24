using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
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
            ExecuteCommand($"DELETE * FROM `TableMappings` WHERE `TableName` = '{linkGreenTableName}'");
            ExecuteCommand($"INSERT INTO `TableMappings` (`DsnName`, `TableName`, `MappingName`) VALUES ('{dsnName}', '{linkGreenTableName}', '{tableName}')");
        }

        public virtual void SaveFieldMapping(string fieldName, string mappingName)
        {
            throw new NotImplementedException();
        }

        protected IEnumerable<T> GetRecords(string sql)
        {
            var list = new List<T>();

            using (var connection = new OdbcConnection(_connectionString)) {
                using (var command = new OdbcCommand(sql, connection)) {
                    connection.Open();
                    try {
                        dynamic reader = new DynamicDataReader(command.ExecuteReader());
                        //var reader = command.ExecuteReader();

                        try {
                            while (reader.Read())
                                list.Add(PopulateRecord(reader));
                        } finally {
                            // Always call Close when done reading.
                            reader.Close();
                        }
                    } finally {
                        connection.Close();
                    }
                    return list;
                }
            }
        }

        protected T GetRecord(string sql, params ObjectParameter[] parameters)
        {
            T record = null;
            using (var connection = new OdbcConnection(_connectionString)) {
                using (var command = new OdbcCommand(sql, connection)) {
                    foreach (var parameter in parameters) {
                        command.Parameters.Add(parameter);
                    }
                    connection.Open();

                    try {
                        //var reader = command.ExecuteReader();
                        dynamic reader = new DynamicDataReader(command.ExecuteReader());

                        try {
                            while (reader.Read()) {
                                record = PopulateRecord(reader);
                                break;
                            }
                        } finally {
                            // Always call Close when done reading.
                            reader.Close();
                        }
                    } finally {
                        connection.Close();
                    }
                    return record;
                }
            }
        }

        protected void ExecuteCommand(string sql)
        {
            using (var connection = new OdbcConnection(_connectionString)) {
                using (var command = new OdbcCommand(sql, connection)) {

                    command.CommandType = CommandType.Text;
                    connection.Open();
                    try {
                        command.ExecuteNonQuery();
                    } finally {
                        connection.Close();
                    }
                }
            }
        }

        public static string NullableString(string value) => value == null ? "null" : $"'{value.Replace("'", "''").Replace("\"", "\\\"")}'";
        public static string NullableInt(int? value) => value.HasValue ? value.ToString() : "null";
        public static string NullableDecimal(decimal? value) => value.HasValue ? value.ToString() : "null";
    }
}