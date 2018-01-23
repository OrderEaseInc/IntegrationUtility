using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataTransfer.AccessDatabase;

namespace LinkGreenODBCUtility
{
    public static class DsnCreds
    {
        //private static string DsnName = Settings.DsnName;
        private static string ConnectionString = Settings.ConnectionString;

        public static void SaveDsnCreds(string dsn, string user, string pass)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            string encryptionKey = config.AppSettings.Settings["EncryptionKey"].Value;

            pass = Encryption.Encrypt(pass, encryptionKey);

            // var _connection = ConnectionInstance.Instance.GetConnection($"DSN={DsnName}");
            var _connection = new OleDbConnectionInstance(ConnectionString).GetConnection();
            var deleteCommand = new OleDbCommand($"DELETE * FROM DsnCredentials WHERE DsnName = '{dsn}' AND Username = '{user}'")
            {
                Connection = _connection
            };
            var insertCommand = new OleDbCommand($"INSERT INTO DsnCredentials ([DsnName], [Username], [Password]) VALUES (?,?,?)")
            {
                Connection = _connection
            };
            insertCommand.Parameters.AddWithValue("Dsn", dsn);
            insertCommand.Parameters.AddWithValue("Username", user);
            insertCommand.Parameters.AddWithValue("Password", pass);

            _connection.Open();
            try
            {
                deleteCommand.ExecuteNonQuery();
                insertCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.Instance.Error($"An error occured while saving Dsn Credentials.");
                Logger.Instance.Error(e.GetBaseException().Message);
            }
            finally
            {
                _connection.Close();
            }
        }

        public static Credentials GetDsnCreds(string dsn)
        {
            // var _connection = ConnectionInstance.Instance.GetConnection($"DSN={DsnName}");
            var _connection = new OleDbConnectionInstance(ConnectionString).GetConnection();
            var command = new OleDbCommand($"SELECT Username, Password FROM DsnCredentials WHERE DsnName = '{dsn}'", _connection);
            _connection.Open();
            var reader = command.ExecuteReader();
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
                string encryptionKey = config.AppSettings.Settings["EncryptionKey"].Value;

                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    return new Credentials()
                    {
                        Username = username,
                        Password = Encryption.Decrypt(password, encryptionKey)
                    };
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Error($"An error occured while retrieving the Credentials for DSN {dsn}");
                MessageBox.Show($"An error occured while retrieving the Credentials for DSN {dsn}", "Error Retrieving Credentials");
            }
            finally
            {
                reader.Close();
                _connection.Close();
            }

            return null;
        }
    }
}
