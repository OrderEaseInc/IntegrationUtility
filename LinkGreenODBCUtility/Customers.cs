using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransfer.AccessDatabase;
using LinkGreen.Applications.Common;
using Common.Utilities;
using LinkGreen.Applications.Common.Model;

namespace LinkGreenODBCUtility
{
    class Customers : IOdbcTransfer
    {
        public string ConnectionString = $"DSN={Settings.DsnName}";
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
            var customerRepository = new CustomerRepository(ConnectionString);
            customerRepository.ClearAll();
            Logger.Instance.Info("Customers LinkGreen transfer table emptied.");
            Logger.Instance.Debug($"{Settings.DsnName}.Customers emptied.");
            return true;
        }

        public void SaveTableMapping(string dsnName, string tableName)
        {
            var customerRepository = new CustomerRepository(ConnectionString);
            customerRepository.SaveTableMapping(dsnName, tableName, "Customers");
            Logger.Instance.Debug($"Customers table mapping saved: (DSN: {dsnName}, Table: {tableName})");
        }

        public void SaveFieldMapping(string fieldName, string mappingName)
        {
            var categoryRepository = new CustomerRepository(ConnectionString);
            categoryRepository.SaveFieldMapping(fieldName, mappingName);
            Logger.Instance.Debug($"Customers field mapping saved: (Field: {fieldName}, MappingField: {mappingName})");
        }

        public void UpdateTemporaryTables()
        {
            TaskManager taskManager = new TaskManager("Customers");
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
                var customers = new CustomerRepository(ConnectionString).GetAll().ToList();

                var skip = 0;
                var take = 5; //was experiencing timeouts with > 5 customers at a time
                int total = 0;

                int numOfPublishedCustomers = 0;
                while (skip < customers.Count)
                {
                    var batch = customers.Skip(skip).Take(take).ToList();
                    total += batch.Count;

                    try
                    {
                        WebServiceHelper.InviteBuyers(batch);
                        numOfPublishedCustomers++;
                    }
                    catch (Exception e)
                    {
                        //ignore timeout
                    }

                    skip += take;
                }

                if (numOfPublishedCustomers == 0)
                {
                    Logger.Instance.Warning("No customers were found to import.");
                }

                Logger.Instance.Info($"{numOfPublishedCustomers} Customers published.");
                Logger.Instance.Debug($"{numOfPublishedCustomers} Customers published. ApiKey: {apiKey}");

                return true;
            }

            Logger.Instance.Warning("No Api Key set while executing customers publish.");

            return false;
        }
    }
}
