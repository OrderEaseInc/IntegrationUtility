using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using DataTransfer.AccessDatabase;
using LinkGreen.Applications.Common;
using LinkGreen.Applications.Common.Model;

namespace LinkGreenODBCUtility
{
    class Categories : IOdbcTransfer
    {
        // public static string ConnectionString = $"DSN={Settings.DsnName}";
        public string ClientConnectionString;

        public Categories()
        {

        }

        public Categories(string clientDsnName)
        {
            ClientConnectionString = "DSN=" + clientDsnName;
        }

        public bool Empty()
        {
            var categoriesRepository = new ProductCategoryRepository(Settings.ConnectionString);
            categoriesRepository.ClearAll();
            Logger.Instance.Info("Categories LinkGreen transfer table emptied.");
            Logger.Instance.Debug($"{Settings.ConnectionString}.Categories emptied.");
            return true;
        }

        public void SaveTableMapping(string dsnName, string tableName)
        {
            var categoryRepository = new ProductCategoryRepository(Settings.ConnectionString);
            categoryRepository.SaveTableMapping(dsnName, tableName, "Categories");
            Logger.Instance.Debug($"Categories table mapping saved: (DSN: {dsnName}, Table: {tableName})");
        }

        public void SaveFieldMapping(string fieldName, string mappingName)
        {
            var categoryRepository = new ProductCategoryRepository(Settings.ConnectionString);
            categoryRepository.SaveFieldMapping(fieldName, mappingName);
            Logger.Instance.Debug($"Categories field mapping saved: (Field: {fieldName}, MappingField: {mappingName})");
        }

        public void UpdateTemporaryTables()
        {
            BatchTaskManager batchTaskManager = new BatchTaskManager("Categories");
            List<string> commands = batchTaskManager.GetCommandsByTrigger();
            foreach (string cmd in commands)
            {
                Batch.Exec(cmd);
            }
        }

        public bool Publish(BackgroundWorker bw = null)
        {
            string apiKey = Settings.GetApiKey();

            if (!string.IsNullOrEmpty(apiKey))
            {
                var categoriesToImport = new ProductCategoryRepository(Settings.ConnectionString).GetAll().ToList();

                var existingCategories = WebServiceHelper.GetAllCategories();

                //create all categories if they don't exist
                int numOfPublishedCategories = 0;
                foreach (var category in categoriesToImport)
                {
                    var existingCategory = existingCategories.FirstOrDefault(s => s?.Name == category.Category);

                    if (existingCategory == null)
                    {
                        existingCategories.Add(
                            WebServiceHelper.PushCategory(new PrivateCategory { Name = category.Category, Depth = 0 }));
                        numOfPublishedCategories++;
                    }
                }

                if (numOfPublishedCategories == 0)
                {
                    Logger.Instance.Warning("No categories were found to import.");
                }

                Logger.Instance.Info($"{numOfPublishedCategories} Categories published.");
                Logger.Instance.Debug($"{numOfPublishedCategories} Categories published. ApiKey: {apiKey}");

                return true;
            }

            Logger.Instance.Warning("No Api Key set.");

            return false;
        }
    }
}
