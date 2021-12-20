using System;
using System.Configuration;
using System.Data.OleDb;
using System.Windows.Forms;
using DataTransfer.AccessDatabase;

// ReSharper disable once CheckNamespace
namespace LinkGreenODBCUtility
{
    public static class DsnCreds
    {
        //private static string DsnName = Settings.DsnName;
        private static readonly string ConnectionString = Settings.ConnectionString;

        public static void SaveDsnCreds(string dsn, string user, string pass)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var encryptionKey = config.AppSettings.Settings["EncryptionKey"].Value;

            pass = Encryption.Encrypt(pass, encryptionKey);

            // var _connection = ConnectionInstance.Instance.GetConnection($"DSN={DsnName}");
            using (var connection = new OleDbConnectionInstance(ConnectionString).GetConnection())
            using (var deleteCommand = new OleDbCommand($"DELETE * FROM DsnCredentials WHERE DsnName = '{dsn}' AND Username = '{user}'")
            { Connection = connection })
            using (var insertCommand = new OleDbCommand($"INSERT INTO DsnCredentials ([DsnName], [Username], [Password]) VALUES (?,?,?)")
            { Connection = connection })
            {

                insertCommand.Parameters.AddWithValue("Dsn", dsn);
                insertCommand.Parameters.AddWithValue("Username", user);
                insertCommand.Parameters.AddWithValue("Password", pass);

                connection.Open();
                try
                {
                    deleteCommand.ExecuteNonQuery();
                    insertCommand.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Logger.Instance.Error($"An error occurred while saving Dsn Credentials.");
                    Logger.Instance.Error(e.GetBaseException().Message);
                }
                finally
                {
                    deleteCommand.Dispose();
                    insertCommand.Dispose();

                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public static Credentials GetDsnCreds(string dsn)
        {
            Credentials credentials = null;
            using (var connection = new OleDbConnectionInstance(ConnectionString).GetConnection())
            using (var command =
                   new OleDbCommand($"SELECT Username, Password FROM DsnCredentials WHERE DsnName = '{dsn}'",
                       connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {

                    try
                    {
                        string username = null;
                        string password = null;
                        while (reader.Read())
                        {
                            username = reader[0].ToString();
                            password = reader[1].ToString();
                        }

                        var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                        var encryptionKey = config.AppSettings.Settings["EncryptionKey"].Value;

                        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                        {
                            credentials = new Credentials
                            {
                                Username = username,
                                Password = Encryption.Decrypt(password, encryptionKey)
                            };
                        }
                    }
                    catch (Exception)
                    {
                        Logger.Instance.Error($"An error occurred while retrieving the Credentials for DSN {dsn}");
                        MessageBox.Show($@"An error occurred while retrieving the Credentials for DSN {dsn}",
                            @"Emptied Successfully");
                    }
                    finally
                    {
                        reader.Close();
                        command.Dispose();
                        connection.Close();
                        connection.Dispose();
                    }
                }
            }

            return credentials;
        }
    }
}
