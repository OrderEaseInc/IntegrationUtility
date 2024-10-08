using System;
using System.ComponentModel;
using System.Data.OleDb;
using System.Configuration;
using System.Linq;
using DataTransfer.AccessDatabase;

using LinkGreen.Applications.Common;
using LinkGreen.Applications.Common.Model;

// ReSharper disable once CheckNamespace
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
                var encryptionKey = config.AppSettings.Settings[Keys.EncryptionKey].Value;
                if (string.IsNullOrEmpty(encryptionKey))
                {
                    var g = Guid.NewGuid();
                    var guidString = Convert.ToBase64String(g.ToByteArray());
                    guidString = guidString.Replace("=", "");
                    guidString = guidString.Replace("+", "");

                    config.AppSettings.Settings[Keys.EncryptionKey].Value = guidString;
                    config.Save(ConfigurationSaveMode.Modified);
                }

                if (GetSandboxMode())
                {
                    config.AppSettings.Settings[Keys.BaseUrl].Value = "https://dev.linkgreen.ca/";
                    config.AppSettings.Settings[Keys.NewApiBaseUrl].Value = "https://linkgreen-coreapi-dev-coreapi.azurewebsites.net/";
                    config.Save(ConfigurationSaveMode.Modified);
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Error($"An error occurred while initializing the app config: {e.Message}");
            }
        }

        public static bool TryConnect()
        {
            System.IO.Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
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

        internal enum SettingsTable
        {
            Settings,
            MigrationTableSettings
        }

        private static string GetTableName(SettingsTable settingsTable)
        {
            switch (settingsTable)
            {
                case SettingsTable.MigrationTableSettings:
                    return "MigrationTableSettings";
                case SettingsTable.Settings:
                default:
                    return "Settings";
            }
        }

        internal static T GetSettingValue<T>(string settingName, SettingsTable settingTableName = SettingsTable.Settings)
        {
            if (settingTableName == SettingsTable.MigrationTableSettings)
                return GetMappingSettingValue<T>(settingName);

            using (var cInstance = new OleDbConnectionInstance(ConnectionString))
            using (var command = new OleDbCommand($"SELECT `{settingName}` FROM `{GetTableName(settingTableName)}` WHERE `Id` = 1", cInstance.GetConnection()))
            {

                command.Connection.Open();
                try
                {
                    var key = command.ExecuteScalar();
                    if (key == null || key == DBNull.Value) return default(T);
                    var conv = TypeDescriptor.GetConverter(typeof(T));
                    return (T)conv.ConvertFrom(key);
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

            return default;
        }


        private static void SaveMappingSettingValue(string dbSettingName, object value)
        {
            using (var cInstance = new OleDbConnectionInstance(ConnectionString))
            using (var command = new OleDbCommand(
                       $"SELECT COUNT(*) FROM `{GetTableName(SettingsTable.MigrationTableSettings)}` WHERE SettingName = '{dbSettingName}'",
                       cInstance.GetConnection()))
            {
                try
                {
                    command.Connection.Open();
                    var rowCount = command.ExecuteScalar();
                    var rowExists = rowCount != null && Convert.ToInt32(rowCount) > 0;
                    command.CommandText = rowExists
                        ? $"UPDATE `{GetTableName(SettingsTable.MigrationTableSettings)}` SET SettingValue = '{value}' WHERE SettingName = '{dbSettingName}'"
                        : $"INSERT INTO `{GetTableName(SettingsTable.MigrationTableSettings)}` (SettingName, SettingValue) VALUES ('{dbSettingName}', '{value}')";

                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Logger.Instance.Error($"An error occurred while updating MappingTable {dbSettingName}. {e.Message}");
                }
                finally
                {
                    command.Connection.Close();
                }
            }
        }

        private static T GetMappingSettingValue<T>(string dbSettingName)
        {
            using (var cInstance = new OleDbConnectionInstance(ConnectionString))
            using (var command = new OleDbCommand(
                       $"SELECT SettingValue FROM `{GetTableName(SettingsTable.MigrationTableSettings)}` WHERE SettingName = '{dbSettingName}'",
                       cInstance.GetConnection()))
            {

                try
                {
                    command.Connection.Open();
                    var value = command.ExecuteScalar();
                    if (value == null)
                        return default;
                    else
                        return (T)Convert.ChangeType(value, typeof(T));
                }
                catch (Exception e)
                {
                    Logger.Instance.Error($"An error occurred while updating MappingTable {dbSettingName}. {e.Message}");
                }
                finally
                {
                    command.Connection.Close();
                }

                return default;
            }
        }

        private static void SaveSettingValue(string dbSettingName, string appSettingName, object value, SettingsTable settingsTable = SettingsTable.Settings)
        {
            if (string.IsNullOrEmpty(appSettingName)) appSettingName = dbSettingName;

            if (settingsTable == SettingsTable.MigrationTableSettings)
            {
                SaveMappingSettingValue(dbSettingName, value);
                return;
            }

            using (var cInstance = new OleDbConnectionInstance(ConnectionString))
            using (var command = new OleDbCommand($"UPDATE `{GetTableName(settingsTable)}` SET `{dbSettingName}` = @value WHERE `ID` = 1",
                       cInstance.GetConnection()))
            {

                command.Parameters.AddWithValue("@value", value);

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
                    Logger.Instance.Error($"An error occurred while updating {dbSettingName}. {e.Message}");
                }
                finally
                {
                    command.Connection.Close();
                    cInstance.CloseConnection();
                }
            }
        }

        public static string GetApiKey() =>
            GetSettingValue<string>(Keys.ApiKey) ?? string.Empty;

        public static void SaveApiKey(string apiKey) =>
            SaveSettingValue(Keys.ApiKey, Keys.ApiKey, apiKey);

        public static string GetNotificationEmail() =>
            GetSettingValue<string>(Keys.NotificationEmail) ?? string.Empty;

        public static void SaveNotificationEmail(string emailAddress) =>
            SaveSettingValue(Keys.NotificationEmail, null, emailAddress);

        public static int[] GetStatusIdForOrderDownload()
        {
            var val = GetSettingValue<string>(Keys.StatusIdForOrderDownload);
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (string.IsNullOrWhiteSpace(val)) return null;

            return val.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
        }


        public static void SaveStatusIdForOrderDownload(int[] id)
        {
            SaveSettingValue(Keys.StatusIdForOrderDownload, Keys.StatusIdForOrderDownload, string.Join(",", id));
        }

        public static string GetSendwithusApiKey()
        {
            var overrideKey = GetSettingValue<string>(Keys.SendwithusApiKey);
            return string.IsNullOrWhiteSpace(overrideKey) ? SendWithUsLiveKey : overrideKey;
        }

        public static bool GetUpdateExistingProducts()
        {
            var dbUpdateCategories = GetSettingValue<string>(Keys.UpdateExistingProducts, SettingsTable.MigrationTableSettings);

            if (!string.IsNullOrWhiteSpace(dbUpdateCategories))
                return dbUpdateCategories == "1";

            return true;
        }

        public static void SaveUpdateExistingProducts(bool updateExistingProducts) =>
            SaveSettingValue(Keys.UpdateExistingProducts, null, updateExistingProducts ? "1" : "0", SettingsTable.MigrationTableSettings);

        public static string GetInstallationId()
        {
            var installationId = GetSettingValue<string>(Keys.InstallationId);
            if (!string.IsNullOrEmpty(installationId)) return installationId;

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            installationId = !string.IsNullOrEmpty(config.AppSettings.Settings[Keys.InstallationId].Value)
                ? config.AppSettings.Settings[Keys.InstallationId].Value
                : null;

            return installationId;
        }

        public static void SaveInstallationId()
        {
            var guid = Guid.NewGuid();
            SaveSettingValue(Keys.InstallationId, null, guid.ToString());
        }

        public static bool GetUpdateCategories()
        {
            var dbUpdateCategories = GetSettingValue<string>(Keys.UpdateCategories);

            if (string.IsNullOrWhiteSpace(dbUpdateCategories))
                return dbUpdateCategories == "1";

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var appConfigUpdateCategories = config.AppSettings.Settings[Keys.UpdateCategories].Value;
            return appConfigUpdateCategories == null || appConfigUpdateCategories == "1";
        }

        public static void SaveUpdateCategories(bool updateCategories) =>
            SaveSettingValue(Keys.UpdateCategories, null, updateCategories ? "1" : "0");

        public static bool GetSanitizeLog()
        {
            var dbSanitizeLog = GetSettingValue<string>(Keys.SanitizeLog);

            if (!string.IsNullOrWhiteSpace(dbSanitizeLog))
                return dbSanitizeLog == "1";

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var appConfigSanitizeLog = config.AppSettings.Settings[Keys.SanitizeLog].Value;
            return appConfigSanitizeLog != null && appConfigSanitizeLog == "1";

        }

        public static bool GetSandboxMode()
        {
            var dbSandboxMode = GetSettingValue<int?>(Keys.SandboxMode);

            //if (!string.IsNullOrWhiteSpace(dbSandboxMode))
            //    return dbSandboxMode == "1";
            if (dbSandboxMode.HasValue)
                return dbSandboxMode.Value == 1;

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var appConfigSandboxMode = config.AppSettings.Settings[Keys.SandboxMode].Value;
            return appConfigSandboxMode == null || appConfigSandboxMode == "1";
        }

        public static void SaveSandboxMode(bool sandboxMode) =>
            SaveSettingValue(Keys.SandboxMode, null, sandboxMode ? "1" : "0");

        public static bool DetailedLogging()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var appConfigDetailedLogging = config.AppSettings.Settings[Keys.DetailedLogging].Value;
            return appConfigDetailedLogging != null && appConfigDetailedLogging == "1";
        }


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
                Logger.Instance.Error($"An error occurred while setting up the user config: {e.Message}");
            }
        }

        private static class Keys
        {
            internal const string ApiKey = nameof(ApiKey);
            internal const string BaseUrl = nameof(BaseUrl);
            internal const string NewApiBaseUrl = nameof(NewApiBaseUrl);
            internal const string StatusIdForOrderDownload = nameof(StatusIdForOrderDownload);
            internal const string EncryptionKey = nameof(EncryptionKey);
            internal const string InstallationId = nameof(InstallationId);
            internal const string NotificationEmail = nameof(NotificationEmail);
            internal const string SandboxMode = nameof(SandboxMode);
            internal const string SanitizeLog = nameof(SanitizeLog);
            internal const string SendwithusApiKey = nameof(SendwithusApiKey);
            internal const string UpdateCategories = nameof(UpdateCategories);
            internal const string UpdateExistingProducts = nameof(UpdateExistingProducts);
            internal const string DetailedLogging = nameof(DetailedLogging);
        }
    }
}
