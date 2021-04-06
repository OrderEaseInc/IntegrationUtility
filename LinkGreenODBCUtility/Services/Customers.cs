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
    internal class Customers : IOdbcTransfer
    {
        //public string ConnectionString = $"DSN={Settings.DsnName}";
        public string ClientConnectionString;

        public Customers()
        {

        }

        public Customers(string clientDsnName)
        {
            ClientConnectionString = "DSN=" + clientDsnName;
        }

        public bool Empty()
        {
            var customerRepository = new CustomerRepository(Settings.ConnectionString);
            customerRepository.ClearAll();
            Logger.Instance.Info("Customers LinkGreen transfer table emptied.");
            Logger.Instance.Debug($"{Settings.ConnectionString}.Customers emptied.");
            return true;
        }

        public void SaveTableMapping(string dsnName, string tableName)
        {
            var customerRepository = new CustomerRepository(Settings.ConnectionString);
            customerRepository.SaveTableMapping(dsnName, tableName, "Customers");
            Logger.Instance.Debug($"Customers table mapping saved: (DSN: {dsnName}, Table: {tableName})");
        }

        public void SaveFieldMapping(string fieldName, string mappingName)
        {
            var categoryRepository = new CustomerRepository(Settings.ConnectionString);
            categoryRepository.SaveFieldMapping(fieldName, mappingName);
            Logger.Instance.Debug($"Customers field mapping saved: (Field: {fieldName}, MappingField: {mappingName})");
        }

        public void UpdateTemporaryTables()
        {
            var batchTaskManager = new BatchTaskManager("Customers");
            var commands = batchTaskManager.GetCommandsByTrigger();
            foreach (string cmd in commands)
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
                var customers = new CustomerRepository(Settings.ConnectionString).GetAll().ToList();

                var total = 0;
                var numOfPublishedCustomers = 0;
                foreach (var customer in customers)
                {
                    total += 1;
                    bw?.ReportProgress(0,
                        $"Processing customer sync (Pushing {total} / {customers.Count}\r\nPlease wait");
                    try
                    {
                        var response = string.IsNullOrWhiteSpace(customer.OurCompanyNumber)
                            ? WebServiceHelper.InviteBuyers(new List<CompanyAndRelationshipResult> { customer })
                            : WebServiceHelper.AddOrUpdateBuyer(customer);
                        Logger.Instance.Info(response);

                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Error(ex.Message);
                    }

                    numOfPublishedCustomers++;
                }

                if (numOfPublishedCustomers == 0)
                {
                    Logger.Instance.Warning("No customers were found to import.");
                }

                publishDetails.Insert(0, $"{total} customers published to LinkGreen");

                Logger.Instance.Info($"{total} Customers published.");
                Logger.Instance.Debug($"{total} Customers published. ApiKey: {apiKey}");


                return true;
            }

            Logger.Instance.Warning("No Api Key set while executing customers publish.");

            return false;
        }
    }
}
