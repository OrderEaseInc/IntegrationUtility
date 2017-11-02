using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LinkGreenODBCUtility
{
    public class Mapping
    {
        public string DsnName;
        public static string TransferDsnName = Settings.DsnName;

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
            var _connection = new OdbcConnection();
            Credentials creds = DsnCreds.GetDsnCreds(DsnName);
            _connection.ConnectionString = $"DSN={DsnName}";
            if (creds != null)
            {
                if (!string.IsNullOrEmpty(creds.Username) && !string.IsNullOrEmpty(creds.Password))
                {
                    _connection.ConnectionString = $"DSN={DsnName};Uid={creds.Username};Pwd={creds.Password}";
                }
            }

            try
            {
                _connection.Open();
            }
            catch (OdbcException e)
            {
                Logger.Instance.Error($"Failed to connect using connection string {_connection.ConnectionString}.");
                MessageBox.Show($"Failed to connect to DSN {DsnName}. Are your credentials set?", "Failed to Connect");
                return new List<string>();
            }

            try
            {
                var tables = _connection.GetSchema("Tables");
                List<string> tableNames = new List<string>();

                foreach (DataRow row in tables.Rows)
                {
                    tableNames.Add(row["TABLE_NAME"].ToString());
                }

                return tableNames;
            }
            finally
            {
                _connection.Close();
            }
        }

        public string GetDsnName(string tableName)
        {
            var _connection = new OdbcConnection();
            _connection.ConnectionString = $"DSN={TransferDsnName}";
            var command = new OdbcCommand($"SELECT `DsnName` FROM `TableMappings` WHERE `TableName` = '{tableName}'", _connection);

            try
            {
                _connection.Open();
            }
            catch (OdbcException e)
            {
                Logger.Instance.Error($"Failed to connect using connection string {_connection.ConnectionString}.");
                return null;
            }

            OdbcDataReader reader = command.ExecuteReader();
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
                _connection.Close();
            }
        }

        public string GetTableMapping(string tableName)
        {
            var _connection = new OdbcConnection();
            _connection.ConnectionString = $"DSN={TransferDsnName}";
            var command = new OdbcCommand($"SELECT `MappingName` FROM `TableMappings` WHERE `TableName` = '{tableName}'", _connection);

            try
            {
                _connection.Open();
            }
            catch (OdbcException e)
            {
                Logger.Instance.Error($"Failed to connect using connection string {_connection.ConnectionString}.");
                return null;
            }

            OdbcDataReader reader = command.ExecuteReader();
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
                _connection.Close();
            }
        }

        public string GetFieldMapping(string tableName, string fieldName)
        {
            var _connection = new OdbcConnection();
            _connection.ConnectionString = $"DSN={TransferDsnName}";
            var command = new OdbcCommand($"SELECT `MappingName` FROM `FieldMappings` WHERE `TableName` = '{tableName}' AND `FieldName` = '{fieldName}'", _connection);

            try
            {
                _connection.Open();
            }
            catch (OdbcException e)
            {
                Logger.Instance.Error($"Failed to connect using connection string {_connection.ConnectionString}.");
                return null;
            }

            OdbcDataReader reader = command.ExecuteReader();
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
                _connection.Close();
            }
        }

        public string GetMappingField(string tableName, string mappingName)
        {
            var _connection = new OdbcConnection();
            _connection.ConnectionString = $"DSN={TransferDsnName}";
            var command = new OdbcCommand($"SELECT `FieldName` FROM `FieldMappings` WHERE `TableName` = '{tableName}' AND `MappingName` = '{mappingName}'", _connection);

            try
            {
                _connection.Open();
            }
            catch (OdbcException e)
            {
                Logger.Instance.Error($"Failed to connect using connection string {_connection.ConnectionString}.");
                return null;
            }

            OdbcDataReader reader = command.ExecuteReader();
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
                _connection.Close();
            }
        }

        public string GetFieldProperty(string tableName, string fieldName, string property)
        {
            var _connection = new OdbcConnection();
            _connection.ConnectionString = $"DSN={TransferDsnName}";
            var command = new OdbcCommand($"SELECT `{property}` FROM `FieldMappings` WHERE `TableName` = '{tableName}' AND (`FieldName` = '{fieldName}' OR `MappingName` = '{fieldName}')", _connection);

            try
            {
                _connection.Open();
            }
            catch (OdbcException e)
            {
                Logger.Instance.Error($"Failed to connect using connection string {_connection.ConnectionString}.");
                return null;
            }

            OdbcDataReader reader = command.ExecuteReader();
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
                _connection.Close();
            }
        }

        public List<MappingField> GetTableFields(string tableName)
        {
            var _connection = new OdbcConnection();
            _connection.ConnectionString = $"DSN={TransferDsnName}";
            var command = new OdbcCommand($"SELECT `TableName`, `FieldName`, `MappingName`, `DisplayName`, `Description`, `DataType`, `Required` FROM `FieldMappings` WHERE `TableName` = '{tableName}'", _connection);

            try
            {
                _connection.Open();
            }
            catch (OdbcException e)
            {
                Logger.Instance.Error($"Failed to connect using connection string {_connection.ConnectionString}.");
                return new List<MappingField>();
            }

            OdbcDataReader reader = command.ExecuteReader();
            Dictionary<string, int> columnIndexes = new Dictionary<string, int>();

            for (int x = 0; x < reader.FieldCount; x++)
            {
                if (!string.IsNullOrEmpty(reader.GetName(x)))
                {
                    columnIndexes.Add(reader.GetName(x), x);
                }
            }

            if (columnIndexes.Count > 0)
            {
                Logger.Instance.Debug($"Column indexes created for retrieving required fields from table: {TransferDsnName}.{tableName}.");
            }
            else
            {
                Logger.Instance.Warning($"No column indexes created for retrieving required fields from table: {TransferDsnName}.{tableName}.");
            }

            List<MappingField> rows = new List<MappingField>();
            try
            {
                int count = 0;
                while (reader.Read())
                {
                    var row = new MappingField();
                    row.TableName = reader[columnIndexes["TableName"]].ToString();
                    row.FieldName = reader[columnIndexes["FieldName"]].ToString();
                    row.MappingName = reader[columnIndexes["MappingName"]].ToString();
                    row.DisplayName = reader[columnIndexes["DisplayName"]].ToString();
                    row.Description = reader[columnIndexes["Description"]].ToString();
                    row.DataType = reader[columnIndexes["DataType"]].ToString();
                    row.Required = (bool) reader[columnIndexes["Required"]];
                    count++;
                    rows.Add(row);
                }

                if (rows.Count > 0)
                {
                    Logger.Instance.Debug($"{rows.Count} required fields were returned for table: {TransferDsnName}.{tableName}");
                }
                else
                {
                    Logger.Instance.Warning($"No required fields were found for table: {TransferDsnName}.{tableName}");
                }

                return rows;
            }
            finally
            {
                reader.Close();
                _connection.Close();
            }
        }

        public List<MappingField> GetUnmappedFields(string tableName)
        {
            var _connection = new OdbcConnection();
            _connection.ConnectionString = $"DSN={TransferDsnName}";
            var command = new OdbcCommand($"SELECT `TableName`, `FieldName`, `MappingName`, `DisplayName`, `Description`, `DataType`, `Required` " +
                                          $"FROM `FieldMappings` " +
                                          $"WHERE `TableName` = '{tableName}' " +
                                            $"AND (`MappingName` = '' OR `MappingName` IS NULL)", _connection);
            try
            {
                _connection.Open();
            }
            catch (OdbcException e)
            {
                Logger.Instance.Error($"Failed to connect using connection string {_connection.ConnectionString}.");
                return new List<MappingField>();
            }

            OdbcDataReader reader = command.ExecuteReader();
            Dictionary<string, int> columnIndexes = new Dictionary<string, int>();

            for (int x = 0; x < reader.FieldCount; x++)
            {
                if (!string.IsNullOrEmpty(reader.GetName(x)))
                {
                    columnIndexes.Add(reader.GetName(x), x);
                }
            }

            if (columnIndexes.Count > 0)
            {
                Logger.Instance.Debug($"Column indexes created for retrieving unmapped fields from table: {TransferDsnName}.{tableName}.");
            }
            else
            {
                Logger.Instance.Warning($"No column indexes created for retrieving unmapped fields from table: {TransferDsnName}.{tableName}.");
            }

            List<MappingField> rows = new List<MappingField>();
            try
            {
                while (reader.Read())
                {
                    var row = new MappingField();
                    row.TableName = reader[columnIndexes["TableName"]].ToString();
                    row.FieldName = reader[columnIndexes["FieldName"]].ToString();
                    row.MappingName = reader[columnIndexes["MappingName"]].ToString();
                    row.DisplayName = reader[columnIndexes["DisplayName"]].ToString();
                    row.Description = reader[columnIndexes["Description"]].ToString();
                    row.DataType = reader[columnIndexes["DataType"]].ToString();
                    row.Required = (bool)reader[columnIndexes["Required"]];
                    rows.Add(row);
                }

                Logger.Instance.Debug($"{rows.Count} unmapped fields were returned for table: {TransferDsnName}.{tableName}");

                return rows;
            }
            finally
            {
                reader.Close();
                _connection.Close();
            }
        }

        public List<MappingField> GetMappedFields(string tableName)
        {
            var _connection = new OdbcConnection();
            _connection.ConnectionString = $"DSN={TransferDsnName}";
            var command = new OdbcCommand($"SELECT `TableName`, `FieldName`, `MappingName`, `DisplayName`, `Description`, `DataType`, `Required` " +
                                          $"FROM `FieldMappings` " +
                                          $"WHERE `TableName` = '{tableName}' " +
                                          $"AND `MappingName` IS NOT NULL", _connection);

            try
            {
                _connection.Open();
            }
            catch (OdbcException e)
            {
                Logger.Instance.Error($"Failed to connect using connection string {_connection.ConnectionString}.");
                return new List<MappingField>();
            }

            OdbcDataReader reader = command.ExecuteReader();
            Dictionary<string, int> columnIndexes = new Dictionary<string, int>();

            for (int x = 0; x < reader.FieldCount; x++)
            {
                if (!string.IsNullOrEmpty(reader.GetName(x)))
                {
                    columnIndexes.Add(reader.GetName(x), x);
                }
            }

            if (columnIndexes.Count > 0)
            {
                Logger.Instance.Debug($"Column indexes created for retrieving unmapped fields from table: {TransferDsnName}.{tableName}.");
            }
            else
            {
                Logger.Instance.Warning($"No column indexes created for retrieving unmapped fields from table: {TransferDsnName}.{tableName}.");
            }

            List<MappingField> rows = new List<MappingField>();
            try
            {
                while (reader.Read())
                {
                    var row = new MappingField();
                    row.TableName = reader[columnIndexes["TableName"]].ToString();
                    row.FieldName = reader[columnIndexes["FieldName"]].ToString();
                    row.MappingName = reader[columnIndexes["MappingName"]].ToString();
                    row.DisplayName = reader[columnIndexes["DisplayName"]].ToString();
                    row.Description = reader[columnIndexes["Description"]].ToString();
                    row.DataType = reader[columnIndexes["DataType"]].ToString();
                    row.Required = (bool)reader[columnIndexes["Required"]];
                    rows.Add(row);
                }

                Logger.Instance.Debug($"{rows.Count} unmapped fields were returned for table: {TransferDsnName}.{tableName}");

                return rows;
            }
            finally
            {
                reader.Close();
                _connection.Close();
            }
        }

        public List<string> GetColumns(string tableName, string dsnName = "")
        {
            if (string.IsNullOrEmpty(dsnName))
            {
                dsnName = Settings.DsnName;
            }
            var _connection = new OdbcConnection();
            Credentials creds = DsnCreds.GetDsnCreds(dsnName);
            _connection.ConnectionString = $"DSN={dsnName}";
            if (creds != null)
            {
                if (!string.IsNullOrEmpty(creds.Username) && !string.IsNullOrEmpty(creds.Password))
                {
                    _connection.ConnectionString = $"DSN={dsnName};Uid={creds.Username};Pwd={creds.Password}";
                }
            }

            try
            {
                _connection.Open();
            }
            catch (OdbcException e)
            {
                Logger.Instance.Error($"Failed to connect using connection string {_connection.ConnectionString}.");
                return new List<string>();
            }

            try
            {
                var columns = _connection.GetSchema("Columns");
                List<string> columnNames = new List<string>();

                foreach (DataRow row in columns.Rows)
                {
                    if (row["TABLE_NAME"].ToString().Equals(tableName))
                    {
                        if (!row["COLUMN_NAME"].ToString().ToUpper().Equals("ID"))
                        {
                            columnNames.Add(row["COLUMN_NAME"].ToString());
                        }
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
                _connection.Close();
            }

            return new List<string>();
        }

        public bool MigrateData(string tableName, bool nuke = true)
        {
            if (ValidateRequiredFields(tableName))
            {
                string tableMappingName = GetTableMapping(tableName);

                List<MappingField> fromColumns = GetMappedFields(tableName);
                List<string> toColumnNames = new List<string>();
                List<string> fromColumnNames = new List<string>();

                foreach (MappingField fromColumn in fromColumns)
                {
                    toColumnNames.Add(fromColumn.FieldName);
                    fromColumnNames.Add(fromColumn.MappingName);
                }

                string chainedToColumnNames = "`" + string.Join("`,`", toColumnNames) + "`";

                string chainedFromColumnNames = "`" + string.Join("`,`", fromColumnNames) + "`";

                var _connection = new OdbcConnection();
                Credentials creds = DsnCreds.GetDsnCreds(DsnName);
                _connection.ConnectionString = $"DSN={DsnName}";
                if (creds != null)
                {
                    if (!string.IsNullOrEmpty(creds.Username) && !string.IsNullOrEmpty(creds.Password))
                    {
                        _connection.ConnectionString = $"DSN={DsnName};Uid={creds.Username};Pwd={creds.Password}";
                    }
                }

                string sql = $"SELECT {chainedFromColumnNames} FROM `{tableMappingName}`";
                var command = new OdbcCommand(sql)
                {
                    Connection = _connection
                };

                try
                {
                    _connection.Open();
                }
                catch (OdbcException e)
                {
                    Logger.Instance.Error($"Failed to connect using connection string {_connection.ConnectionString}.");
                    return false;
                }

                OdbcDataReader reader = command.ExecuteReader();
                var columnIndexes = new List<KeyValuePair<string, int>>();
                try
                {
                    for (int x = 0; x < reader.FieldCount; x++)
                    {
                        string fieldName = GetMappingField(tableName, reader.GetName(x));
                        if (!string.IsNullOrEmpty(fieldName))
                        {
                            var columnIndex = new KeyValuePair<string, int>(fieldName, x);
                            columnIndexes.Add(columnIndex);
                        }
                    }

                    if (columnIndexes.Count > 0)
                    {
                        Logger.Instance.Debug($"Column indexes created for migrating data to {Settings.DsnName}.{tableName}.");
                    }
                    else
                    {
                        Logger.Instance.Warning($"No column indexes were created for migrating data to {Settings.DsnName}.{tableName}.");
                    }

                    var _conn = new OdbcConnection();
                    _conn.ConnectionString = "DSN=" + TransferDsnName;
                    var nukeCommand = new OdbcCommand($"DELETE * FROM `{tableName}`")
                    {
                        Connection = _conn
                    };

                    _conn.Open();
                    try
                    {
                        if (nuke)
                        {
                            nukeCommand.ExecuteNonQuery();
                            Logger.Instance.Debug($"{Settings.DsnName}.{tableName} nuked.");
                        }
                    }
                    catch (OdbcException e)
                    {
                        Logger.Instance.Error($"Failed to nuke {Settings.DsnName}{tableName}: {e.Message}");
                    }
                    finally
                    {
                        _conn.Close();
                    }

                    var rowCount = 0;
                    while (reader.Read())
                    {
                        if (columnIndexes.Count == toColumnNames.Count)
                        {
                            List<string> readerColumns = new List<string>();

                            foreach (KeyValuePair<string, int> colIndex in columnIndexes)
                            {
                                string text = reader[colIndex.Value].ToString();
                                text = text.Replace("'", "''").Replace("\"", "\\\"");
                                string original = text;
                                text = SanitizeField(tableName, colIndex.Key, text);
                                if (!string.IsNullOrEmpty(text) && Settings.GetSanitizeLog() && original != text)
                                {
                                    File.AppendAllText(@"log-sanitized.txt", $"{DateTime.Now} {tableName}:{colIndex} [{original} -> {text}] {Environment.NewLine}");
                                }
                                if (string.IsNullOrEmpty(text))
                                {
                                    text = "null";
                                    readerColumns.Add(text);
                                }
                                else
                                {
                                    readerColumns.Add("'" + text + "'");
                                }
                            }

                            string readerColumnValues = string.Join(",", readerColumns);
                            string stmt = $"INSERT INTO `{tableName}` ({chainedToColumnNames}) VALUES ({readerColumnValues})";

                            var comm = new OdbcCommand(stmt)
                            {
                                Connection = _conn
                            };

                            _conn.Open();
                            try
                            {
                                comm.ExecuteNonQuery();
                                rowCount++;
                            }
                            catch (OdbcException e)
                            {
                                Logger.Instance.Error($"Failed to insert record into {Settings.DsnName}.{tableName}: {e.Message} \n\nCommand: {comm.CommandText}");
                            }
                            finally
                            {
                                _conn.Close();
                            }

                        }
                    }

                    if (rowCount > 0)
                    {
                        Logger.Instance.Debug($"{rowCount} records inserted into {Settings.DsnName}.{tableName}.");
                    }
                    else
                    {
                        Logger.Instance.Warning($"No records inserted into {Settings.DsnName}.{tableName}.");
                    }

                    return true;
                }
                finally
                {
                    reader.Close();
                    _connection.Close();
                }
            }

            MessageBox.Show("All required fields indicated with a * must be mapped.", "Map Required Fields");
            return false;
        }

        private string SanitizeField(string tableName, string field, string text)
        {
            string sanitizeNumbersOnly = GetFieldProperty(tableName, field, "SanitizeNumbersOnly");
            string sanitizeEmail = GetFieldProperty(tableName, field, "SanitizeEmail");
            string sanitizePrice = GetFieldProperty(tableName, field, "SanitizePrice");
            string sanitizeAlphanumeric = GetFieldProperty(tableName, field, "SanitizeAlphaNumeric");
            string sanitizeUniqueId = GetFieldProperty(tableName, field, "SanitizeUniqueId");

            if (!string.IsNullOrEmpty(text))
            {
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

                return !string.IsNullOrEmpty(text) ? Tools.CleanStringForSql(text) : "";
            }

            return text;
        }

        private bool ValidateRequiredFields(string tableName)
        {
            var _connection = new OdbcConnection();
            _connection.ConnectionString = $"DSN={TransferDsnName}";
            var command = new OdbcCommand($"SELECT * FROM `FieldMappings` " +
                                          $"WHERE `TableName` = '{tableName}' " +
                                          $"AND (`MappingName` = '' OR `MappingName` IS NULL)" +
                                          $"AND `Required` = TRUE", _connection);

            try
            {
                _connection.Open();
            }
            catch (OdbcException e)
            {
                Logger.Instance.Error($"Failed to connect using connection string {_connection.ConnectionString}.");
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
                _connection.Close();
            }

            return true;
        }

        public DataTable PreviewMapping(string tableName)
        {
            string tableMappingName = GetTableMapping(tableName);
            List<string> columns = GetColumns(tableMappingName, DsnName);
            List<string> mappingColumns = new List<string>();

            foreach (string column in columns)
            {
                string field = GetMappingField(tableName, column);
                string fieldDisplayName = GetFieldProperty(tableName, field, "DisplayName");
                string combinedColumnName = "`" + column + "`" + " AS \"" + fieldDisplayName + " : " + column + "\""; 
                if (!string.IsNullOrEmpty(field))
                {
                    mappingColumns.Add(combinedColumnName);
                }
            }

            string columnNames = string.Join(",", mappingColumns);

            if (mappingColumns.Count > 0)
            {
                var _connection = new OdbcConnection();
                Credentials creds = DsnCreds.GetDsnCreds(DsnName);
                _connection.ConnectionString = $"DSN={DsnName}";
                if (creds != null)
                {
                    if (!string.IsNullOrEmpty(creds.Username) && !string.IsNullOrEmpty(creds.Password))
                    {
                        _connection.ConnectionString = $"DSN={DsnName};Uid={creds.Username};Pwd={creds.Password}";
                    }
                }

                string query = $"SELECT {columnNames} FROM `{tableMappingName}`";
                var command = new OdbcCommand(query)
                {
                    Connection = _connection
                };

                try
                {
                    _connection.Open();
                }
                catch (OdbcException e)
                {
                    Logger.Instance.Error($"Failed to connect using connection string {_connection.ConnectionString}.");
                    return new DataTable();
                }

                OdbcDataAdapter adapter = new OdbcDataAdapter(query, _connection);
                DataTable table = new DataTable();

                var items = GetUnmappedFields(tableName);
                foreach (MappingField item in items)
                {
                    table.Columns.Add(item.DisplayName);
                }

                try
                {
                    adapter.Fill(table);
                    int rowCount = table.Rows.Count;
                    for (int i = 20; i < rowCount; i++)
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
                    _connection.Close();
                }
            }

            DataTable previewTable = new DataTable();

            var previewItems = GetUnmappedFields(tableName);
            foreach (MappingField item in previewItems)
            {
                previewTable.Columns.Add(item.DisplayName);
            }

            return previewTable;
        }
    }
}
