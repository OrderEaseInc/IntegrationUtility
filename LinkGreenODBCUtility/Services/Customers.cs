using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows.Forms;
using DataTransfer.AccessDatabase;
using LinkGreen.Applications.Common;

namespace LinkGreenODBCUtility
{
    class Customers : IOdbcTransfer
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
            BatchTaskManager batchTaskManager = new BatchTaskManager("Customers");
            List<string> commands = batchTaskManager.GetCommandsByTrigger();
            foreach (string cmd in commands)
            {
                Batch.Exec(cmd);
            }
        }
        public bool Publish()
        {
            // string apiKey = ConfigurationManager.AppSettings["ApiKey"];
            string apiKey = Settings.GetApiKey();

            if (!string.IsNullOrEmpty(apiKey))
            {
                var customers = new CustomerRepository(Settings.ConnectionString).GetAll().ToList();

                var skip = 0;
                var take = 4; //was experiencing timeouts with > 4 customers at a time
                int total = 0;

                int numOfPublishedCustomers = 0;
                while (skip < customers.Count)
                {
                    var batch = customers.Skip(skip).Take(take).ToList();
                    total += batch.Count;

                    try
                    {
                        var response = WebServiceHelper.InviteBuyers(batch);
                        Logger.Instance.Info(response);
                        numOfPublishedCustomers++;
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.Error(e.Message);
                    }

                    skip += take;
                }

                if (numOfPublishedCustomers == 0)
                {
                    Logger.Instance.Warning("No customers were found to import.");
                    // MessageBox.Show("No customers were published.", "No Customers published.");
                }

                Logger.Instance.Info($"{total} Customers published.");
                Logger.Instance.Debug($"{total} Customers published. ApiKey: {apiKey}");

                return true;
            }

            Logger.Instance.Warning("No Api Key set while executing customers publish.");

            return false;
        }
    }
}
