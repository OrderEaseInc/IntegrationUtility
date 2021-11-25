using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DataTransfer.AccessDatabase;
using LinkGreen.Applications.Common;
using LinkGreen.Applications.Common.Model;

// ReSharper disable once CheckNamespace
namespace LinkGreenODBCUtility
{
    internal class PriceLevels : IOdbcTransfer
    {
        //public string ConnectionString = $"DSN={Settings.DsnName}";
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
            var priceLevelRepository = new PriceLevelRepository(Settings.ConnectionString);
            priceLevelRepository.ClearAll();
            return true;
        }

        public void SaveTableMapping(string dsnName, string tableName)
        {
            var priceLevelRepository = new PriceLevelRepository(Settings.ConnectionString);
            priceLevelRepository.SaveTableMapping(dsnName, tableName, "PriceLevels");
            Logger.Instance.Debug($"Pricing Levels table mapping saved: (DSN: {dsnName}, Table: {tableName})");
        }

        public void SaveFieldMapping(string fieldName, string mappingName)
        {
            var priceLevelRepository = new PriceLevelRepository(Settings.ConnectionString);
            priceLevelRepository.SaveFieldMapping(fieldName, mappingName);
            Logger.Instance.Debug($"Pricing Levels field mapping saved: (Field: {fieldName}, MappingField: {mappingName})");
        }

        public void UpdateTemporaryTables()
        {
            var batchTaskManager = new BatchTaskManager("PriceLevels");
            var commands = batchTaskManager.GetCommandsByTrigger();
            foreach (var cmd in commands)
            {
                Batch.Exec(cmd);
            }
        }

        public bool Publish(out List<string> publishDetails, BackgroundWorker bw = null)
        {
            publishDetails = new List<string>();
            var apiKey = Settings.GetApiKey();

            if (!string.IsNullOrEmpty(apiKey))
            {
                var priceLevelRepo = new PriceLevelRepository(Settings.ConnectionString);
                var levelsToImport = priceLevelRepo.GetAll().ToList();
                var existingLevels = WebServiceHelper.GetExistingPricingLevels();
                var importCounter = 0;
                var existingCounter = 0;

                foreach (var level in levelsToImport)
                {
                    if (existingLevels.Any(lvl => lvl.Name == level.Name))
                    {
                        existingCounter++;
                        continue;
                    }

                    var effectiveDate = level.EffectiveDate ?? DateTime.Now.ToUniversalTime();
                    if (level.EndDate < effectiveDate)
                    {
                        level.EndDate = null;
                    }
                    var request = new PricingLevelRequest
                    {
                        Name = level.Name,
                        ExternalReference = level.ExternalReference,
                        InventoryItems = new List<PricingLevelItemRequest>(),
                        EffectiveDate = effectiveDate,
                        EndDate = level.EndDate
                    };

                    WebServiceHelper.PushPricingLevel(request);
                    importCounter++;
                }

                publishDetails.Insert(0,
                    $"{existingCounter} price levels already existing in LinkGreen and were not pushed.");
                publishDetails.Insert(0, $"{importCounter} price levels have been pushed to LinkGreen");

                return true;
            }

            Logger.Instance.Warning("No Api Key set while executing price level publish.");
            return false;
        }
    }
}
