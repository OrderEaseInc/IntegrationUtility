using System;
using System.Data.OleDb;
using System.Configuration;

using DataTransfer.AccessDatabase;

using LinkGreen.Applications.Common;
using LinkGreen.Applications.Common.Model;

namespace LinkGreenODBCUtility
{
    public static class Settings
    {
        private const string SendWithUsLiveKey = "live_78b03caa9c4e1284a1191bb1e8ddf2dcd9c3469f";

        public static string ConnectViaDsnName = "LinkGreenDataTransfer";
        public static bool DebugMode = false;
        public static readonly string ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=LinkGreenDataTransfer.mdb;Persist Security Info=True";


        public static void Init()
        {
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var encryptionKey = config.AppSettings.Settings["EncryptionKey"].Value;
                if (string.IsNullOrEmpty(encryptionKey))
                {
                    var g = Guid.NewGuid();
                    var guidString = Convert.ToBase64String(g.ToByteArray());
                    guidString = guidString.Replace("=", "");
                    guidString = guidString.Replace("+", "");

                    config.AppSettings.Settings["EncryptionKey"].Value = guidString;
                    config.Save(ConfigurationSaveMode.Modified);
                }

                if (GetSandboxMode())
                {
                    config.AppSettings.Settings["BaseUrl"].Value = "http://dev.linkgreen.ca/";
                    config.Save(ConfigurationSaveMode.Modified);
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Error($"An error occured while initializing the app config: {e.Message}");
            }
        }

        public static bool TryConnect()
        {
            //var _connection = ConnectionInstance.Instance.GetConnection($"DSN={DsnName}");
            using (var cInstance = new OleDbConnectionInstance(ConnectionString))
            {
                var connection = cInstance.GetConnection();
                try
                {
                    connection.Open();
                }
                catch (OleDbException e)
                {
                    Logger.Instance.Error($"Could not connect to the DSN {ConnectionString}: {e.Message}");
                    return false;
                }
                finally
                {
                    cInstance.CloseConnection();
                }
            }

            return true;
        }

        public static T GetSettingValue<T>(string settingName, bool isEncrypted = false)
        {
            using (var cInstance = new OleDbConnectionInstance(ConnectionString))
            {
                var command = new OleDbCommand($"SELECT `{settingName}` FROM `Settings` WHERE `Id` = 1", cInstance.GetConnection());
                command.Connection.Open();
                try
                {
                    var key = command.ExecuteScalar();
                    if (key == null) return default(T);
                    return (T)Convert.ChangeType(key, typeof(T));
                }
                catch (Exception e)
                {
                    Logger.Instance.Error($"An error occurred while retrieving the {settingName}: {e.Message}");
                }
                finally
                {
                    command.Connection.Close();
                    cInstance.CloseConnection();
                }
            }

            return default(T);
        }

        public static void SaveSettingValue(string dbSettingName, string appSettingName, object value, bool isEncrypted = false)
        {
            if (string.IsNullOrEmpty(appSettingName)) appSettingName = dbSettingName;

            using (var cInstance = new OleDbConnectionInstance(ConnectionString))
            {
                var command = new OleDbCommand($"UPDATE `Settings` SET `{dbSettingName}` = '{value}' WHERE `ID` = 1",
                    cInstance.GetConnection());

                command.Connection.Open();
                try
                {
                    command.ExecuteNonQuery();

                    var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    config.AppSettings.Settings[appSettingName].Value = value.ToString();
                    config.Save(ConfigurationSaveMode.Modified);

                    Logger.Instance.Debug($"{dbSettingName} saved: '{value}'");
                }
                catch (Exception e)
                {
                    Logger.Instance.Error($"An error occured while updating {dbSettingName}. {e.Message}");
                }
                finally
                {
                    command.Connection.Close();
                    cInstance.CloseConnection();
                }
            }
        }

        public static string GetApiKey() =>
            GetSettingValue<string>("ApiKey") ?? string.Empty;

        public static void SaveApiKey(string apiKey) =>
            SaveSettingValue("ApiKey", "ApiKey", apiKey);

        public static string GetNotificationEmail() =>
            GetSettingValue<string>("NotificationEmail") ?? string.Empty;

        public static void SaveNotificationEmail(string emailAddress) =>
            SaveSettingValue("NotificationEmail", null, emailAddress);

        internal static string GetSendwithusApiKey()
        {
            var overrideKey = GetSettingValue<string>("SendwithusApiKey", true);
            return string.IsNullOrWhiteSpace(overrideKey) ? SendWithUsLiveKey : overrideKey;
        }

        internal static void SaveSendwithusApiKey(string value) =>
            SaveSettingValue("SendwithusApiKey", null, value, true);

        public static string GetInstallationId()
        {
            var installationId = GetSettingValue<string>("InstallationId");
            if (!string.IsNullOrEmpty(installationId)) return installationId;

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            installationId = !string.IsNullOrEmpty(config.AppSettings.Settings["InstallationId"].Value)
                ? config.AppSettings.Settings["InstallationId"].Value
                : null;

            return installationId;
        }

        public static void SaveInstallationId()
        {
            var guid = Guid.NewGuid();
            SaveSettingValue("InstallationId", null, guid.ToString());
        }

        public static bool GetUpdateCategories()
        {
            var dbUpdateCategories = GetSettingValue<int?>("UpdateCategories");

            if (dbUpdateCategories.HasValue)
                return dbUpdateCategories == 1;

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var appConfigUpdateCategories = config.AppSettings.Settings["UpdateCategories"].Value;
            return appConfigUpdateCategories == null || appConfigUpdateCategories == "1";
        }

        public static void SaveUpdateCategories(bool updateCategories) =>
            SaveSettingValue("UpdateCategories", null, updateCategories ? "1" : "0");

        public static bool GetSanitizeLog()
        {
            var dbSanitizeLog = GetSettingValue<int?>("SanitizeLog");

            if (dbSanitizeLog.HasValue)
                return dbSanitizeLog == 1;

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var appConfigSanitizeLog = config.AppSettings.Settings["SanitizeLog"].Value;
            return appConfigSanitizeLog != null && appConfigSanitizeLog == "1";

        }

        public static bool GetSandboxMode()
        {
            var dbSandboxMode = GetSettingValue<int?>("SandboxMode");

            if (dbSandboxMode.HasValue)
                return dbSandboxMode == 1;

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var appConfigSandboxMode = config.AppSettings.Settings["SandboxMode"].Value;
            return appConfigSandboxMode == null || appConfigSandboxMode == "1";
        }

        public static void SaveSandboxMode(bool sandboxMode) =>
            SaveSettingValue("SandboxMode", null, sandboxMode ? "1" : "0");

        public static void SetupUserConfig(string apiKey)
        {
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                UserAndCompany user = WebServiceHelper.GetUserInfoByApiKey(apiKey);

                if (user != null)
                {
                    config.AppSettings.Settings["ClientId"].Value = user.CompanyId.ToString();
                    //                    config.AppSettings.Settings["ClientName"].Value = user.CompanyName;
                    config.AppSettings.Settings["UserName"].Value = user.FullName;
                    config.AppSettings.Settings["EmailAddress"].Value = user.EmailAddress;
                    config.AppSettings.Settings["PhoneNumber"].Value = user.FormattedPhone;
                    config.Save(ConfigurationSaveMode.Modified);

                    if (GetInstallationId() == null)
                    {
                        SaveInstallationId();
                    }

                    Logger.Instance.Debug($"Application setup completed using: (CompanyId: {user.CompanyId}, FullName: {user.FullName}, EmailAddress: {user.EmailAddress}, FormattedPhone: {user.FormattedPhone}");
                }
                else
                {
                    Logger.Instance.Error($"Failed to retrieve the user information for apiKey: {apiKey}");
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Error($"An error occured while setting up the user config: {e.Message}");
            }
        }
    }
}
