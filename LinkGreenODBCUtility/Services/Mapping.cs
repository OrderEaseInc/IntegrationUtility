using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DataTransfer.AccessDatabase;

// ReSharper disable once CheckNamespace
namespace LinkGreenODBCUtility
{
    public class Mapping
    {
        public string DsnName;
        public static string TransferDsnName = Settings.ConnectViaDsnName;
        // ReSharper disable once InconsistentNaming
        public bool _validFields = true;
        // ReSharper disable once InconsistentNaming
        public bool _validPushFields = true;
        // ReSharper disable once InconsistentNaming
        public bool _validUpdateFields;

        private readonly Dictionary<string, string> _fieldProperties = new Dictionary<string, string>();

        public Mapping()
        {
            DsnName = TransferDsnName;
        }

        public Mapping(string dsnName)
        {
            if (!string.IsNullOrEmpty(dsnName))
            {
                DsnName = dsnName;
            }
        }

        public List<string> GetTableNames()
        {
            using (var db = new ActiveDbConnection(DsnName))
            {
                var connection = db.Connection;

                try
                {
                    connection.Open();
                }
                catch (OdbcException e)
                {
                    Logger.Instance.Error($"Failed to connect using connection string {connection.ConnectionString}.");
                    Logger.Instance.Error(e.GetBaseException().Message);
                    MessageBox.Show($@"Failed to connect to DSN {DsnName}. Are your credentials set?", @"Emptied Successfully");
                    ConnectionInstance.CloseConnection($"DSN={DsnName}");
                    return new List<string>();
                }


                var tableNames = new List<string>();

                var tables = connection.GetSchema("Tables");
                tableNames.AddRange(from DataRow row in tables.Rows
                                    where row["TABLE_TYPE"].ToString() != "SYSTEM TABLE"
                                    select row["TABLE_NAME"].ToString());


                var views = connection.GetSchema("Views");
                foreach (DataRow row in views.Rows)
                {
                    tableNames.Add(row["TABLE_NAME"].ToString());
                }

                return tableNames;

            }
        }

        public string GetDsnName(string tableName)
        {
            using (var db = new ActiveDbConnection(TransferDsnName))
            {
                var connection = db.Connection;
                var command =
                    new OdbcCommand($"SELECT `DsnName` FROM `TableMappings` WHERE `TableName` = '{tableName}'",
                        connection);

                try
                {
                    connection.Open();
                }
                catch (OdbcException e)
                {
                    Logger.Instance.Error($"Failed to connect using connection string {connection.ConnectionString}.");
                    Logger.Instance.Error(e.GetBaseException().Message);
                    ConnectionInstance.CloseConnection($"DSN={TransferDsnName}");
                    return null;
                }

                using (var reader = command.ExecuteReader())
                {
                    try
                    {
                        while (reader.Read())
                        {
                            return reader[0].ToString();
                        }

                        return null;
                    }
                    finally
                    {
                        reader.Close();

                    }
                }
            }
        }

        public string GetTableMapping(string tableName)
        {
            using (var db = new ActiveDbConnection(TransferDsnName))
            {
                var connection = db.Connection;
                var command =
                    new OdbcCommand($"SELECT `MappingName` FROM `TableMappings` WHERE `TableName` = '{tableName}'",
                        connection);

                try
                {
                    connection.Open();
                }
                catch (OdbcException e)
                {
                    Logger.Instance.Error($"Failed to connect using connection string {connection.ConnectionString}.");
                    Logger.Instance.Error(e.GetBaseException().Message);
                    ConnectionInstance.CloseConnection($"DSN={TransferDsnName}");
                    return null;
                }

                using (var reader = command.ExecuteReader())
                {
                    try
                    {
                        while (reader.Read())
                        {
                            return reader[0].ToString();
                        }

                        return null;
                    }
                    finally
                    {
                        reader.Close();
                        ConnectionInstance.CloseConnection($"DSN={TransferDsnName}");
                    }
                }
            }
        }

        public string GetFieldMapping(string tableName, string fieldName)
        {
            using (var db = new ActiveDbConnection(TransferDsnName))
            {
                var connection = db.Connection;
                var command =
                    new OdbcCommand(
                        $"SELECT `MappingName` FROM `FieldMappings` WHERE `TableName` = '{tableName}' AND `FieldName` = '{fieldName}'",
                        connection);

                try
                {
                    connection.Open();
                }
                catch (OdbcException e)
                {
                    Logger.Instance.Error($"Failed to connect using connection string {connection.ConnectionString}.");
                    Logger.Instance.Error(e.GetBaseException().Message);
                    return null;
                }

                using (var reader = command.ExecuteReader())
                {
                    try
                    {
                        while (reader.Read())
                        {
                            return reader[0].ToString();
                        }

                        return null;
                    }
                    finally
                    {
                        reader.Close();
                        ConnectionInstance.CloseConnection($"DSN={TransferDsnName}");
                    }
                }
            }
        }

        public string GetMappingField(string tableName, string mappingName)
        {
            using (var db = new ActiveDbConnection(TransferDsnName))
            {
                var connection = db.Connection;
                var command =
                    new OdbcCommand(
                        $"SELECT `FieldName` FROM `FieldMappings` WHERE `TableName` = '{tableName}' AND `MappingName` = '{mappingName}'",
                        connection);

                try
                {
                    connection.Open();
                }
                catch (OdbcException e)
                {
                    Logger.Instance.Error($"Failed to connect using connection string {connection.ConnectionString}.");
                    Logger.Instance.Error(e.GetBaseException().Message);
                    return null;
                }

                using (var reader = command.ExecuteReader())
                {
                    try
                    {
                        while (reader.Read())
                        {
                            return reader[0].ToString();
                        }

                        return null;
                    }
                    finally
                    {
                        reader.Close();
                        ConnectionInstance.CloseConnection($"DSN={TransferDsnName}");
                    }
                }
            }
        }

        public string GetFieldProperty(string tableName, string fieldName, string property, OdbcConnection connection = null)
        {
            if (_fieldProperties.ContainsKey($"{tableName}.{fieldName}.{property}"))
                return _fieldProperties[$"{tableName}.{fieldName}.{property}"];

            if (connection == null)
            {
                connection = ConnectionInstance.Instance.GetConnection($"DSN={TransferDsnName}");
            }
            var command = new OdbcCommand($"SELECT `{property}` FROM `FieldMappings` WHERE `TableName` = '{tableName}' AND (`FieldName` = '{fieldName}' OR `MappingName` = '{fieldName}')", connection);

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            OdbcDataReader reader = command.ExecuteReader();

            try
            {
                while (reader.Read())
                {
                    _fieldProperties[$"{tableName}.{fieldName}.{property}"] = reader[0].ToString();
                    return reader[0].ToString();
                }

                return null;
            }
            finally
            {
                reader.Close();
                ConnectionInstance.CloseConnection($"DSN={TransferDsnName}");
            }
        }

        public List<MappingField> GetTableFields(string tableName)
        {
            using (var db = new ActiveDbConnection(TransferDsnName))
            {
                var connection = db.Connection;
                var command =
                    new OdbcCommand(
                        $"SELECT `TableName`, `FieldName`, `MappingName`, `DisplayName`, `Description`, `DataType`, `Required` FROM `FieldMappings` WHERE `TableName` = '{tableName}'",
                        connection);

                try
                {
                    connection.Open();
                }
                catch (OdbcException e)
                {
                    Logger.Instance.Error($"Failed to connect using connection string {connection.ConnectionString}.");
                    Logger.Instance.Error(e.GetBaseException().Message);
                    return new List<MappingField>();
                }

                var columnIndexes = new Dictionary<string, int>();
                using (var reader = command.ExecuteReader())
                {

                    for (var x = 0; x < reader.FieldCount; x++)
                    {
                        if (!string.IsNullOrEmpty(reader.GetName(x)))
                        {
                            columnIndexes.Add(reader.GetName(x), x);
                        }
                    }


                    if (columnIndexes.Count > 0)
                    {
                        Logger.Instance.Debug(
                            $"Column indexes created for retrieving required fields from table: {TransferDsnName}.{tableName}.");
                    }
                    else
                    {
                        Logger.Instance.Warning(
                            $"No column indexes created for retrieving required fields from table: {TransferDsnName}.{tableName}.");
                    }

                    var rows = new List<MappingField>();
                    try
                    {
                        while (reader.Read())
                        {
                            var row = new MappingField
                            {
                                TableName = reader[columnIndexes["TableName"]].ToString(),
                                FieldName = reader[columnIndexes["FieldName"]].ToString(),
                                MappingName = reader[columnIndexes["MappingName"]].ToString(),
                                DisplayName = reader[columnIndexes["DisplayName"]].ToString(),
                                Description = reader[columnIndexes["Description"]].ToString(),
                                DataType = reader[columnIndexes["DataType"]].ToString(),
                                Required = (bool)reader[columnIndexes["Required"]]
                            };
                            rows.Add(row);
                        }

                        if (rows.Count > 0)
                        {
                            Logger.Instance.Debug(
                                $"{rows.Count} required fields were returned for table: {TransferDsnName}.{tableName}");
                        }
                        else
                        {
                            Logger.Instance.Warning(
                                $"No required fields were found for table: {TransferDsnName}.{tableName}");
                        }

                        return rows;
                    }
                    finally
                    {
                        reader.Close();
                        ConnectionInstance.CloseConnection($"DSN={TransferDsnName}");
                    }
                }
            }
        }

        public List<MappingField> GetUnmappedFields(string tableName)
        {
            using (var db = new ActiveDbConnection(TransferDsnName))
            {
                var connection = db.Connection;
                var command = new OdbcCommand(
                    $"SELECT `TableName`, `FieldName`, `MappingName`, `DisplayName`, `Description`, `DataType`, `Required` " +
                    $"FROM `FieldMappings` " +
                    $"WHERE `TableName` = '{tableName}' " +
                    $"AND (`MappingName` = '' OR `MappingName` IS NULL)", connection);
                try
                {
                    connection.Open();
                }
                catch (OdbcException e)
                {
                    Logger.Instance.Error($"Failed to connect using connection string {connection.ConnectionString}.");
                    Logger.Instance.Error(e.GetBaseException().Message);
                    return new List<MappingField>();
                }

                var columnIndexes = new Dictionary<string, int>();
                using (var reader = command.ExecuteReader())
                {

                    for (int x = 0; x < reader.FieldCount; x++)
                    {
                        if (!string.IsNullOrEmpty(reader.GetName(x)))
                        {
                            columnIndexes.Add(reader.GetName(x), x);
                        }
                    }

                    if (columnIndexes.Count > 0)
                    {
                        Logger.Instance.Debug(
                            $"Column indexes created for retrieving unmapped fields from table: {TransferDsnName}.{tableName}.");
                    }
                    else
                    {
                        Logger.Instance.Warning(
                            $"No column indexes created for retrieving unmapped fields from table: {TransferDsnName}.{tableName}.");
                    }

                    var rows = new List<MappingField>();
                    try
                    {
                        while (reader.Read())
                        {
                            var row = new MappingField
                            {
                                TableName = reader[columnIndexes["TableName"]].ToString(),
                                FieldName = reader[columnIndexes["FieldName"]].ToString(),
                                MappingName = reader[columnIndexes["MappingName"]].ToString(),
                                DisplayName = reader[columnIndexes["DisplayName"]].ToString(),
                                Description = reader[columnIndexes["Description"]].ToString(),
                                DataType = reader[columnIndexes["DataType"]].ToString(),
                                Required = (bool)reader[columnIndexes["Required"]]
                            };
                            rows.Add(row);
                        }

                        Logger.Instance.Debug(
                            $"{rows.Count} unmapped fields were returned for table: {TransferDsnName}.{tableName}");

                        return rows;
                    }
                    finally
                    {
                        reader.Close();
                        ConnectionInstance.CloseConnection($"DSN={TransferDsnName}");
                    }
                }
            }
        }

        public List<MappingField> GetMappedFields(string tableName)
        {
            using (var db = new ActiveDbConnection(TransferDsnName))
            {
                var connection = db.Connection;
                var command = new OdbcCommand(
                    $"SELECT `TableName`, `FieldName`, `MappingName`, `DisplayName`, `Description`, `DataType`, `Required`, `Updatable` " +
                    $"FROM `FieldMappings` " +
                    $"WHERE `TableName` = '{tableName}' " +
                    $"AND `MappingName` IS NOT NULL", connection);

                try
                {
                    connection.Open();
                }
                catch (OdbcException e)
                {
                    Logger.Instance.Error($"Failed to connect using connection string {connection.ConnectionString}.");
                    Logger.Instance.Error(e.GetBaseException().Message);
                    return new List<MappingField>();
                }

                using (var reader = command.ExecuteReader())
                {
                    var columnIndexes = new Dictionary<string, int>();

                    for (var x = 0; x < reader.FieldCount; x++)
                    {
                        if (!string.IsNullOrEmpty(reader.GetName(x)))
                        {
                            columnIndexes.Add(reader.GetName(x), x);
                        }
                    }

                    if (columnIndexes.Count > 0)
                    {
                        Logger.Instance.Debug(
                            $"Column indexes created for retrieving unmapped fields from table: {TransferDsnName}.{tableName}.");
                    }
                    else
                    {
                        Logger.Instance.Warning(
                            $"No column indexes created for retrieving unmapped fields from table: {TransferDsnName}.{tableName}.");
                    }

                    var rows = new List<MappingField>();
                    try
                    {
                        while (reader.Read())
                        {
                            var row = new MappingField
                            {
                                TableName = reader[columnIndexes["TableName"]].ToString(),
                                FieldName = reader[columnIndexes["FieldName"]].ToString(),
                                MappingName = reader[columnIndexes["MappingName"]].ToString(),
                                DisplayName = reader[columnIndexes["DisplayName"]].ToString(),
                                Description = reader[columnIndexes["Description"]].ToString(),
                                DataType = reader[columnIndexes["DataType"]].ToString(),
                                Required = (bool)reader[columnIndexes["Required"]],
                                Updatable = (bool)reader[columnIndexes["Updatable"]]
                            };
                            rows.Add(row);
                        }

                        Logger.Instance.Debug(
                            $"{rows.Count} unmapped fields were returned for table: {TransferDsnName}.{tableName}");

                        return rows;
                    }
                    finally
                    {
                        reader.Close();
                        ConnectionInstance.CloseConnection($"DSN={TransferDsnName}");
                    }
                }
            }
        }

        public List<string> GetColumns(string tableName, string dsnName = "")
        {
            if (string.IsNullOrEmpty(dsnName))
            {
                dsnName = Settings.ConnectViaDsnName;
            }

            using (var db = new ActiveDbConnection(dsnName))
            {
                var connection = db.Connection;

                try
                {
                    connection.Open();
                }
                catch (OdbcException e)
                {
                    Logger.Instance.Error($"Failed to connect using connection string {connection.ConnectionString}.");
                    Logger.Instance.Error(e.GetBaseException().Message);
                    return new List<string>();
                }

                try
                {
                    var columns = connection.GetSchema("Columns");
                    var columnNames = new List<string>();

                    foreach (DataRow row in columns.Rows)
                    {
                        if (!row["TABLE_NAME"].ToString().Equals(tableName)) continue;

                        if (!row["COLUMN_NAME"].ToString().ToUpper().Equals("ID"))
                        {
                            columnNames.Add(row["COLUMN_NAME"].ToString());
                        }
                    }

                    if (!columnNames.Any())
                    {
                        Logger.Instance.Warning($"No column names found for {dsnName}.{tableName}.");
                    }

                    return columnNames;
                }
                catch (Exception e)
                {
                    Logger.Instance.Error($"Unable to retrieve column names from {dsnName}.{tableName}: {e.Message}");
                }
                finally
                {
                    ConnectionInstance.CloseConnection($"DSN={dsnName}");
                }
            }

            return new List<string>();
        }

        public bool MigrateData(string tableName, bool nuke = true)
        {
            if (ValidateRequiredFields(tableName))
            {
                var tableMappingName = GetTableMapping(tableName);

                var fromColumns = GetMappedFields(tableName);
                var toColumnNames = new List<string>();
                var fromColumnNames = new List<string>();

                foreach (var fromColumn in fromColumns)
                {
                    toColumnNames.Add(fromColumn.FieldName);
                    fromColumnNames.Add(fromColumn.MappingName);
                }

                var chainedToColumnNames = string.Join(",", toColumnNames);

                var chainedFromColumnNames = string.Join(",", fromColumnNames);

                using (var db = new ActiveDbConnection(DsnName))
                {
                    var connection = db.Connection;

                    var sql = $"SELECT {chainedFromColumnNames} FROM {tableMappingName}";
                    var command = new OdbcCommand(sql, connection);

                    var rowCount = 0;

                    var useSanitizeLog = false;
                    try
                    {
                        useSanitizeLog = Settings.GetSanitizeLog();
                    }
                    catch
                    {
                        // ignored
                    }

                    try
                    {
                        connection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            var columnIndexes = new List<KeyValuePair<string, int>>();
                            try
                            {
                                for (var x = 0; x < reader.FieldCount; x++)
                                {
                                    var fieldName = GetMappingField(tableName, reader.GetName(x));
                                    if (string.IsNullOrEmpty(fieldName)) continue;

                                    var columnIndex = new KeyValuePair<string, int>(fieldName, x);
                                    columnIndexes.Add(columnIndex);
                                }

                                using (var dbTransfer = new ActiveDbConnection(TransferDsnName))
                                {
                                    var transferConnection = dbTransfer.Connection;
                                    var nukeCommand = new OdbcCommand($"DELETE * FROM {tableName}", transferConnection);

                                    try
                                    {
                                        if (nuke)
                                        {
                                            transferConnection.Open();
                                            nukeCommand.ExecuteNonQuery();
                                            Logger.Instance.Debug($"{Settings.ConnectViaDsnName}.{tableName} nuked.");
                                        }
                                    }
                                    catch (OdbcException e)
                                    {
                                        Logger.Instance.Error(
                                            $"Failed to nuke {Settings.ConnectViaDsnName}{tableName}: {e.Message}");
                                    }

                                    while (reader.Read())
                                    {
                                        if (columnIndexes.Count == toColumnNames.Count)
                                        {
                                            var readerColumns = new List<string>();

                                            foreach (var colIndex in columnIndexes)
                                            {
                                                var text = reader[colIndex.Value].ToString();
                                                text = text.Replace("'", "''").Replace("\"", "\\\"");
                                                var original = text;
                                                text = SanitizeField(tableName, colIndex.Key, text, transferConnection);
                                                if (!string.IsNullOrEmpty(text) && useSanitizeLog && original != text)
                                                {
                                                    File.AppendAllText(@"log-sanitized.txt",
                                                        $"{DateTime.Now} {tableName}:{colIndex} [{original} -> {text}] {Environment.NewLine}");
                                                }

                                                if (string.IsNullOrEmpty(text))
                                                {
                                                    text = "NULL";
                                                    readerColumns.Add(text);
                                                }
                                                else
                                                {
                                                    if (text.ToLower() == "true" || text.ToLower() == "yes")
                                                    {
                                                        text = "1";
                                                    }
                                                    else if (text.ToLower() == "false" || text.ToLower() == "no")
                                                    {
                                                        text = "0";
                                                    }

                                                    readerColumns.Add("'" + text + "'");
                                                }
                                            }

                                            var readerColumnValues = string.Join(",", readerColumns);
                                            var stmt =
                                                $"INSERT INTO {tableName} ({chainedToColumnNames}) VALUES ({readerColumnValues})";

                                            var comm = new OdbcCommand(stmt)
                                            {
                                                Connection = transferConnection
                                            };

                                            try
                                            {
                                                comm.ExecuteNonQuery();
                                                rowCount++;
                                            }
                                            catch (OdbcException e)
                                            {
                                                Logger.Instance.Error(
                                                    $"Failed to insert record into {Settings.ConnectViaDsnName}.{tableName}: {e.Message} \n\nCommand: {comm.CommandText}");
                                            }

                                        }
                                    }

                                    if (rowCount > 0)
                                    {
                                        Logger.Instance.Debug(
                                            $"{rowCount} records inserted into {Settings.ConnectViaDsnName}.{tableName}.");
                                    }
                                    else
                                    {
                                        Logger.Instance.Warning(
                                            $"No records inserted into {Settings.ConnectViaDsnName}.{tableName}.");
                                    }

                                    return true;
                                }
                            }
                            finally
                            {
                                reader.Close();
                                ConnectionInstance.CloseConnection($"DSN={TransferDsnName}");
                            }
                        }
                    }
                    catch (OdbcException e)
                    {
                        Logger.Instance.Error(
                            $"Failed to connect using connection string {connection.ConnectionString}.");
                        Logger.Instance.Error(e.GetBaseException().Message);
                        return false;
                    }
                    finally
                    {
                        ConnectionInstance.CloseConnection($"DSN={DsnName}");
                        ConnectionInstance.CloseConnection($"DSN={TransferDsnName}");
                    }
                }
            }

            _validFields = false;
            return false;
        }

        /// <summary>
        /// Pushes data from the Access database into 
        /// </summary>
        /// <param name="tableName">Name of the Access table to push</param>
        /// <param name="tableKey">Name of the Field Name that maps to the primary key of the mapped table</param>
        /// <param name="clearProduction">Whether to clear out the production table before populating it.</param>
        /// <returns>True if successful</returns>
        public bool PushData(string tableName, string tableKey, bool clearProduction = false)
        {
            if (ValidateRequiredFields(tableName))
            {
                var tableMappingName = GetTableMapping(tableName);

                var fromColumns = GetMappedFields(tableName);
                var toColumns = new List<MappingField>();
                var fromColumnNames = new List<string>();

                foreach (var fromColumn in fromColumns)
                {
                    toColumns.Add(fromColumn);
                    fromColumnNames.Add(fromColumn.FieldName);
                }

                var chainedToColumnNames = string.Join(",", toColumns.Select(c => c.MappingName));

                var chainedFromColumnNames = string.Join(",", fromColumnNames);

                if (clearProduction)
                {
                    using (var db = new ActiveDbConnection(DsnName))
                    {
                        using (var clearCommand = new OdbcCommand($"DELETE FROM {tableMappingName}", db.Connection))
                        {
                            try
                            {
                                db.Connection.Open();
                                clearCommand.ExecuteNonQuery();
                                Logger.Instance.Debug($"{DsnName}.{tableMappingName} cleared.");
                            }
                            catch (OdbcException e)
                            {
                                Logger.Instance.Error($"Failed to clear {DsnName}.{tableMappingName}: {e.Message}");
                            }
                            finally
                            {
                                ConnectionInstance.CloseConnection($"DSN={DsnName}");
                            }
                        }
                    }
                }

                int fieldCount;
                var columnIndexes = new Dictionary<string, int>();
                var fields = new Dictionary<int, string>();

                using (var db = new ActiveDbConnection(TransferDsnName))
                {
                    var sql = $"SELECT {chainedFromColumnNames} FROM {tableName}";
                    using (var command = new OdbcCommand(sql, db.Connection))
                    {
                        db.Connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            fieldCount = reader.FieldCount;
                            try
                            {
                                for (var x = 0; x < fieldCount; x++)
                                {
                                    fields.Add(x, reader.GetName(x));
                                }

                            }
                            finally
                            {
                                reader.Close();
                                ConnectionInstance.CloseConnection($"DSN={TransferDsnName}");
                            }
                        }
                    }
                }


                for (var x = 0; x < fieldCount; x++)
                {
                    var fieldName = GetFieldMapping(tableName, fields[x]);
                    if (!string.IsNullOrEmpty(fieldName))
                    {
                        columnIndexes.Add(fieldName, x);
                    }
                }

                if (columnIndexes.Count > 0)
                {
                    Logger.Instance.Debug(
                        $"Column indexes created for migrating data to {DsnName}.{tableMappingName}.");
                }
                else
                {
                    Logger.Instance.Warning(
                        $"No column indexes were created for migrating data to {DsnName}.{tableMappingName}.");
                }

                // we need to ensure that this record doesn't exist in the production db already
                var mappedKey = GetFieldMapping(tableName, tableKey);
                string existsSql = null;

                using (var db = new ActiveDbConnection(TransferDsnName))
                {
                    var sql = $"SELECT {chainedFromColumnNames} FROM {tableName}";
                    using (var command = new OdbcCommand(sql, db.Connection))
                    {
                        try
                        {
                            db.Connection.Open();
                            using (var reader = command.ExecuteReader())
                            {
                                var rowCount = 0;
                                while (reader.Read())
                                {
                                    if (columnIndexes.Count == toColumns.Count)
                                    {
                                        var readerColumns = new List<string>();
                                        foreach (var col in toColumns)
                                        {
                                            var text = ValueOrNull(reader[columnIndexes[col.MappingName]].ToString(), col.DataType);
                                            readerColumns.Add(text);
                                            if (col.MappingName == mappedKey)
                                            {
                                                existsSql = $"SELECT * FROM {tableMappingName} WHERE {mappedKey} = {text}";
                                            }
                                        }

                                        if (!string.IsNullOrEmpty(existsSql))
                                        {
                                            using (var dnsDb = new ActiveDbConnection(DsnName))
                                            {
                                                var existsCommand = new OdbcCommand(existsSql, dnsDb.Connection);
                                                try
                                                {
                                                    dnsDb.Connection.Open();
                                                    var existsReader = existsCommand.ExecuteReader();
                                                    if (existsReader.Read())
                                                    {
                                                        // there's already a record with this key. move along...
                                                        continue;
                                                    }
                                                }
                                                finally
                                                {
                                                    ConnectionInstance.CloseConnection($"DSN={DsnName}");
                                                }
                                            }
                                        }

                                        using (var dnsDb = new ActiveDbConnection(DsnName))
                                        {
                                            var readerColumnValues = string.Join(",", readerColumns);
                                            var stmt = $"INSERT INTO {tableMappingName} ({chainedToColumnNames}) VALUES ({readerColumnValues})";
                                            using (var comm = new OdbcCommand(stmt, dnsDb.Connection))
                                            {
                                                try
                                                {
                                                    dnsDb.Connection.Open();
                                                    comm.ExecuteNonQuery();
                                                    rowCount++;
                                                }
                                                catch (OdbcException e)
                                                {
                                                    Logger.Instance.Error(
                                                        $"Failed to insert record into {Settings.ConnectViaDsnName}.{tableName}: {e.Message} \n\nCommand: {comm.CommandText}");
                                                }
                                                finally
                                                {
                                                    ConnectionInstance.CloseConnection($"DSN={DsnName}");
                                                }
                                            }
                                        }
                                    }

                                    if (rowCount > 0)
                                    {
                                        Logger.Instance.Debug($"{rowCount} records inserted into {DsnName}.{tableMappingName}.");
                                    }
                                    else
                                    {
                                        Logger.Instance.Warning($"No records inserted into {DsnName}.{tableMappingName}.");
                                    }
                                }
                            }
                        }
                        finally
                        {
                            ConnectionInstance.CloseConnection($"DSN={TransferDsnName}");
                        }
                    }
                }

                return true;
            }

            _validPushFields = false;
            return false;
        }

        /// <summary>
        /// Updates updatable fields in the Access database from the production data
        /// </summary>
        /// <param name="tableName">Name of the Access table to push</param>
        /// <param name="tableKey">Name of the Field Name that maps to the primary key of the mapped table</param>
        /// <returns>True if successful</returns>
        public bool UpdateData(string tableName, string tableKey)
        {
            if (ValidateRequiredFields(tableName))
            {
                var tableMappingName = GetTableMapping(tableName);
                var keyMappingName = GetFieldMapping(tableName, tableKey);

                var updatableColumns = GetMappedFields(tableName).Where(c => c.Updatable).ToList();
                var fromColumnNames = new List<string>();

                foreach (var fromColumn in updatableColumns)
                {
                    fromColumnNames.Add(fromColumn.MappingName);
                }

                var chainedFromColumnNames = string.Join(",", fromColumnNames.Select(c => $"{c}"));
                var columnIndexes = new Dictionary<string, int>();
                var fields = new Dictionary<int, string>();
                int fieldCount;
                using (var db = new ActiveDbConnection(DsnName))
                {

                    try
                    {
                        var sql = $"SELECT {keyMappingName}, {chainedFromColumnNames} FROM {tableMappingName}";
                        using (var command = new OdbcCommand(sql, db.Connection))
                        {

                            try
                            {
                                db.Connection.Open();
                            }
                            catch (OdbcException e)
                            {
                                Logger.Instance.Error(
                                    $"Failed to connect using connection string {db.Connection.ConnectionString}.");
                                Logger.Instance.Error(e.GetBaseException().Message);
                                return false;
                            }

                            using (var reader = command.ExecuteReader())
                            {
                                fieldCount = reader.FieldCount;
                                for (var x = 0; x < fieldCount; x++)
                                {
                                    fields.Add(x, reader.GetName(x));
                                }
                            }
                        }
                    }
                    finally
                    {
                        ConnectionInstance.CloseConnection($"DSN={DsnName}");
                    }
                }

                for (var x = 0; x < fieldCount; x++)
                {
                    var fieldName = GetMappingField(tableName, fields[x]);
                    if (!string.IsNullOrEmpty(fieldName))
                    {
                        columnIndexes.Add(fieldName, x);
                    }
                }

                if (columnIndexes.Count > 0)
                {
                    Logger.Instance.Debug(
                        $"Column indexes created for migrating data to {TransferDsnName}.{tableName}.");
                }
                else
                {
                    Logger.Instance.Warning(
                        $"No column indexes were created for migrating data to {TransferDsnName}.{tableName}.");
                }

                var rowCount = 0;
                using (var db = new ActiveDbConnection(DsnName))
                {
                    try
                    {
                        var sql = $"SELECT {keyMappingName}, {chainedFromColumnNames} FROM {tableMappingName}";
                        using (var command = new OdbcCommand(sql, db.Connection))
                        {

                            db.Connection.Open();

                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if (columnIndexes.Count != updatableColumns.Count + 1) continue;

                                    var readerColumns = new List<string>();
                                    foreach (var col in updatableColumns)
                                    {
                                        var colName = col.FieldName;
                                        var value = ValueOrNull(reader[columnIndexes[colName]].ToString(), col.DataType);
                                        if (value != "null")
                                        {
                                            readerColumns.Add($"{colName} = {value}");
                                        }
                                    }
                                    if (!readerColumns.Any())
                                    {
                                        continue;
                                    }

                                    using (var transferDb = new ActiveDbConnection(TransferDsnName))
                                    {
                                        var transferConnection = transferDb.Connection;
                                        var keyValue = ValueOrNull(reader[columnIndexes[tableKey]].ToString(), "Number");
                                        var readerColumnValues = string.Join(",", readerColumns);
                                        var stmt = $"UPDATE {tableName} SET {readerColumnValues} WHERE {tableKey} = {keyValue}";

                                        using (var comm = new OdbcCommand(stmt, transferConnection))
                                        {

                                            try
                                            {
                                                transferConnection.Open();
                                            }
                                            catch (OdbcException e)
                                            {
                                                Logger.Instance.Error($"Failed to connect using connection string {transferConnection.ConnectionString}.");
                                                Logger.Instance.Error(e.GetBaseException().Message);
                                                return false;
                                            }

                                            try
                                            {
                                                comm.ExecuteNonQuery();
                                                rowCount++;
                                            }
                                            catch (OdbcException e)
                                            {
                                                Logger.Instance.Error(
                                                    $"Failed to update record in {DsnName}.{tableMappingName}: {e.Message} \n\nCommand: {comm.CommandText}");
                                            }
                                            finally
                                            {
                                                ConnectionInstance.CloseConnection($"DSN={TransferDsnName}");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    finally
                    {
                        ConnectionInstance.CloseConnection($"DSN={DsnName}");
                    }
                }

                if (rowCount > 0)
                {
                    Logger.Instance.Debug($"{rowCount} records updated in {TransferDsnName}.{tableName}.");
                }
                else
                {
                    Logger.Instance.Warning($"No records updated in {TransferDsnName}.{tableName}.");
                }

                return true;
            }

            _validUpdateFields = false;
            return false;
        }

        private static string ValueOrNull(string value, string fieldType = "Short Text")
        {
            var delimiter = (fieldType == "Number" || fieldType == "Decimal") ? "" : "'";
            return string.IsNullOrEmpty(value) ? "null" : $"{delimiter}{value.Replace("'", "''").Replace("\"", "\\\"")}{delimiter}";
        }

        private string SanitizeField(string tableName, string field, string text, OdbcConnection connection)
        {
            connection.Close();
            connection.Open();
            var sanitizeNumbersOnly = GetFieldProperty(tableName, field, "SanitizeNumbersOnly", connection);
            var sanitizeEmail = GetFieldProperty(tableName, field, "SanitizeEmail", connection);
            var sanitizePrice = GetFieldProperty(tableName, field, "SanitizePrice", connection);
            var sanitizeAlphanumeric = GetFieldProperty(tableName, field, "SanitizeAlphaNumeric", connection);
            var sanitizeUniqueId = GetFieldProperty(tableName, field, "SanitizeUniqueId", connection);
            var sanitizeCountry = GetFieldProperty(tableName, field, "SanitizeCountry", connection);
            var sanitizeProvince = GetFieldProperty(tableName, field, "SanitizeProvince", connection);

            if (string.IsNullOrEmpty(text)) return text;

            if (Convert.ToBoolean(sanitizeNumbersOnly))
            {
                text = Tools.CleanStringOfNonDigits(text);
            }

            if (Convert.ToBoolean(sanitizeEmail))
            {
                text = Tools.CleanEmail(text);
            }

            if (Convert.ToBoolean(sanitizePrice))
            {
                text = Tools.FormatDecimal(text);
            }

            if (Convert.ToBoolean(sanitizeAlphanumeric))
            {
                text = Tools.CleanAlphanumeric(text);
            }

            if (Convert.ToBoolean(sanitizeUniqueId))
            {
                text = Tools.CleanUniqueId(text);
            }

            if (Convert.ToBoolean(sanitizeCountry))
                text = Tools.CleanCountry(text, connection.ConnectionString);

            if (Convert.ToBoolean(sanitizeProvince))
                text = Tools.CleanProvince(text, connection.ConnectionString);

            return !string.IsNullOrEmpty(text) ? Tools.CleanStringForSql(text) : "";

        }

        private static bool ValidateRequiredFields(string tableName)
        {
            using (var db = new ActiveDbConnection(TransferDsnName))
            {
                var command = new OdbcCommand($"SELECT * FROM `FieldMappings` " +
                                              $"WHERE `TableName` = '{tableName}' " +
                                              $"AND (`MappingName` = '' OR `MappingName` IS NULL)" +
                                              $"AND `Required` = TRUE", db.Connection);

                try
                {
                    db.Connection.Open();
                }
                catch (OdbcException e)
                {
                    Logger.Instance.Error($"Failed to connect using connection string {db.Connection.ConnectionString}.");
                    Logger.Instance.Error(e.GetBaseException().Message);
                    return false;
                }

                OdbcDataReader reader = command.ExecuteReader();
                try
                {
                    if (reader.HasRows)
                    {
                        return false;
                    }
                }
                finally
                {
                    reader.Close();
                    ConnectionInstance.CloseConnection($"DSN={TransferDsnName}");
                }
            }
            return true;
        }

        public DataTable PreviewMapping(string tableName)
        {
            var tableMappingName = GetTableMapping(tableName);
            var columns = GetColumns(tableMappingName, DsnName);
            var mappingColumns = new List<string>();

            var mappedColumns = GetMappedFields(tableName);

            foreach (var column in columns) {
                var field = mappedColumns.FirstOrDefault(c => c.MappingName == column)?.FieldName;
                var fieldDisplayName = mappedColumns.FirstOrDefault(c => c.MappingName == column)?.DisplayName;
                var combinedColumnName = $"`{column}`  AS \"{fieldDisplayName} : {column}\"";
                if (!string.IsNullOrEmpty(field))
                {
                    mappingColumns.Add(combinedColumnName);
                }
            }

            var columnNames = string.Join(",", mappingColumns);

            if (mappingColumns.Count > 0)
            {
                using (var dnsConnection = new ActiveDbConnection(DsnName))
                {

                    var query = $"SELECT TOP 20 {columnNames} FROM `{tableMappingName}`";
                    var command = new OdbcCommand(query, dnsConnection.Connection);

                    try
                    {
                        dnsConnection.Connection.Open();
                    }
                    catch (OdbcException e)
                    {
                        Logger.Instance.Error(
                            $"Failed to connect using connection string {dnsConnection.Connection.ConnectionString}.");
                        Logger.Instance.Error(e.GetBaseException().Message);
                        return new DataTable();
                    }

                    using (var adapter = new OdbcDataAdapter(query, dnsConnection.Connection))
                    {
                        var table = new DataTable();

                        var items = GetUnmappedFields(tableName);
                        foreach (var item in items)
                        {
                            table.Columns.Add(item.DisplayName);
                        }

                        try
                        {
                            adapter.Fill(table);
                            var rowCount = table.Rows.Count;

                            for (var i = rowCount - 1; i > 20; i--)
                            {
                                table.Rows.RemoveAt(i);
                            }

                            table.DefaultView.AllowDelete = false;
                            table.DefaultView.AllowEdit = false;
                            table.DefaultView.AllowNew = false;

                            return table;
                        }
                        catch (Exception e)
                        {
                            Logger.Instance.Error($"An error occured while populating the preview table: {e.Message}");
                        }
                        finally
                        {
                            ConnectionInstance.CloseConnection($"DSN={DsnName}");
                        }
                    }
                }
            }

            var previewTable = new DataTable();

            var previewItems = GetUnmappedFields(tableName);
            foreach (var item in previewItems)
            {
                previewTable.Columns.Add(item.DisplayName);
            }

            return previewTable;
        }


        private class ActiveDbConnection : IDisposable
        {
            private readonly string _dsnName;
            public OdbcConnection Connection { get; private set; }

            public ActiveDbConnection(string dsnName)
            {
                _dsnName = dsnName;
                Connection = ConnectionInstance.Instance.GetConnection($"DSN={_dsnName}");

                var creds = DsnCreds.GetDsnCreds(_dsnName);

                if (!string.IsNullOrEmpty(creds?.Username) && !string.IsNullOrEmpty(creds.Password))
                {
                    Connection.ConnectionString = $"DSN={_dsnName};Uid={creds.Username};Pwd={creds.Password}";
                }
            }

            public void Dispose()
            {
                ConnectionInstance.CloseConnection($"DSN={_dsnName}");
            }
        }
    }

}
