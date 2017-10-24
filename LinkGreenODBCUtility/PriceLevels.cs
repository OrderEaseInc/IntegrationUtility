using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using DataTransfer.AccessDatabase;
using LinkGreen.Applications.Common;
using LinkGreen.Applications.Common.Model;

namespace LinkGreenODBCUtility
{
    class PriceLevels : IOdbcTransfer
    {
        public string ConnectionString = $"DSN={Settings.DsnName}";
        public string ClientConnectionString;

        public PriceLevels()
        {

        }

        public PriceLevels(string clientDsnName)
        {
            ClientConnectionString = "DSN=" + clientDsnName;
        }

        public bool Empty()
        {
            var priceLevelRepository = new PriceLevelRepository(ConnectionString);
            priceLevelRepository.ClearAll();
            return true;
        }

        public void SaveTableMapping(string dsnName, string tableName)
        {
            var priceLevelRepository = new PriceLevelRepository(ConnectionString);
            priceLevelRepository.SaveTableMapping(dsnName, tableName, "PriceLevels");
            Logger.Instance.Debug($"Pricing Levels table mapping saved: (DSN: {dsnName}, Table: {tableName})");
        }

        public void SaveFieldMapping(string fieldName, string mappingName)
        {
            var priceLevelRepository = new PriceLevelRepository(ConnectionString);
            priceLevelRepository.SaveFieldMapping(fieldName, mappingName);
            Logger.Instance.Debug($"Pricing Levels field mapping saved: (Field: {fieldName}, MappingField: {mappingName})");
        }

        public void UpdateTemporaryTables()
        {
            TaskManager taskManager = new TaskManager("PriceLevels");
            List<string> commands = taskManager.GetCommandsByTrigger();
            foreach (string cmd in commands)
            {
                Batch.Exec(cmd);
            }
        }

        public bool Publish()
        {
            string apiKey = ConfigurationManager.AppSettings["ApiKey"];

            if (!string.IsNullOrEmpty(apiKey))
            {
                var priceLevelRepo = new PriceLevelRepository(ConnectionString);
                var levelsToImport = priceLevelRepo.GetAll().ToList();

                foreach (var level in levelsToImport)
                {
                    if (level.EndDate < DateTime.Now)
                    {
                        level.EndDate = null;
                    }
                    var request = new PricingLevelRequest
                    {
                        Name = level.Name,
                        InventoryItems = new List<PricingLevelItemRequest>(),
                        EffectiveDate = DateTime.Now,
                        EndDate = level.EndDate
                    };

                    WebServiceHelper.PushPricingLevel(request);
                }

                return true;
            }

            Logger.Instance.Warning("No Api Key set while executing price level publish.");
            return false;
        }
    }
}
