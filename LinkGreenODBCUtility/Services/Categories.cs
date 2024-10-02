using DataTransfer.AccessDatabase;
using LinkGreen.Applications.Common;
using LinkGreen.Applications.Common.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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


        private int? GetParentCategoryID(ICollection<PrivateCategory> existingCategories, ProductCategory category)
        {
            var parentCategory = existingCategories.FirstOrDefault(p =>
                p.Name == category.ParentCategoryName && p.Depth == 0);
            if (parentCategory == null)
            {
                var newParentCategory = WebServiceHelper.PushCategory(new CreateCategoryRequest
                {
                    data = category.ParentCategoryName,
                    Depth = 0
                });

                existingCategories.Add(newParentCategory);
                parentCategory = newParentCategory;
            }

            return parentCategory.Id;
        }

        public bool Publish(out List<string> publishDetails, BackgroundWorker bw = null)
        {
            publishDetails = new List<string>();

            var apiKey = Settings.GetApiKey();

            if (string.IsNullOrEmpty(apiKey))
            {
                Logger.Instance.Warning("No Api Key set.");

                return false;
            }

            try
            {
                var categoriesToImport = new ProductCategoryRepository(Settings.ConnectionString).GetAll().ToList();

                var existingCategories = WebServiceHelper.GetAllCategories();

                //create all categories if they don't exist
                var numOfPublishedCategories = 0;

                foreach (var category in categoriesToImport)
                {
                    if (existingCategories.All(s => s?.Name != category.Category))
                    {
                        var pushableCategory = new CreateCategoryRequest { data = category.Category };
                        //if (!string.IsNullOrWhiteSpace(category.ParentCategoryName))
                        //{
                        //    pushableCategory.ParentCategoryId = GetParentCategoryID(existingCategories, category);
                        //}
                        //else
                        //{
                        pushableCategory.Depth = 0;
                        //}
                        try
                        {
                            existingCategories.Add(WebServiceHelper.PushCategory(pushableCategory));
                        }
                        catch (Exception ex)
                        {
                            Logger.Instance.Error(
                                JsonConvert.SerializeObject(pushableCategory) + Environment.NewLine +
                                ex.Message + Environment.NewLine + ex.StackTrace);
                            publishDetails.Add($"There was an error adding category: {category.Category}");
                        }

                        publishDetails.Add($"Added category: {category.Category}");
                        numOfPublishedCategories++;
                    }
                }


                if (numOfPublishedCategories == 0)
                {
                    Logger.Instance.Warning("No categories were found to import.");
                }

                publishDetails.Insert(0, $"{numOfPublishedCategories} Categories published.");

                Logger.Instance.Info($"{numOfPublishedCategories} Categories published.");
                Logger.Instance.Debug($"{numOfPublishedCategories} Categories published. ApiKey: {apiKey}");

            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
    }
}
