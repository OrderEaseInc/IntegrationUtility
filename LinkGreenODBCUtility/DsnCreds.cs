using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataTransfer.AccessDatabase;

namespace LinkGreenODBCUtility
{
    public static class DsnCreds
    {
        private static string DsnName = Settings.DsnName;

        public static void SaveDsnCreds(string dsn, string user, string pass)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            string encryptionKey = config.AppSettings.Settings["EncryptionKey"].Value;

            pass = Encryption.Encrypt(pass, encryptionKey);

            var _connection = new OdbcConnection();
            _connection.ConnectionString = "DSN=" + DsnName;
            var deleteCommand = new OdbcCommand($"DELETE * FROM DsnCredentials WHERE DsnName = '{dsn}' AND Username = '{user}'")
            {
                Connection = _connection
            };
            var insertCommand = new OdbcCommand($"INSERT INTO DsnCredentials (DsnName, Username, Password) VALUES ('{dsn}', '{user}', '{pass}')")
            {
                Connection = _connection
            };

            _connection.Open();
            try
            {
                deleteCommand.ExecuteNonQuery();
                insertCommand.ExecuteNonQuery(); 
            }
            catch (Exception e)
            {
                Logger.Instance.Error($"An error occured while saving Dsn Credentials.");
            }
            finally
            {
                _connection.Close();
            }
        }

        public static Credentials GetDsnCreds(string dsn)
        {
            var _connection = new OdbcConnection();
            _connection.ConnectionString = "DSN=" + DsnName;
            var command = new OdbcCommand($"SELECT Username, Password FROM DsnCredentials WHERE DsnName = '{dsn}'", _connection);
            _connection.Open();
            OdbcDataReader reader = command.ExecuteReader();
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
