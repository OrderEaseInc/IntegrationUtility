using DataTransfer.AccessDatabase;
using LinkGreen.Applications.Common;
using LinkGreen.Applications.Common.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace LinkGreenODBCUtility
{
    internal class Customers : IOdbcTransfer
    {
        //public string ConnectionString = $"DSN={Settings.DsnName}";
        public string ClientConnectionString;

        public Customers()
        { }

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

            if (string.IsNullOrEmpty(apiKey))
            {
                publishDetails.Insert(0, "No Api Key set while executing products publish.");
                Logger.Instance.Warning("No Api Key set while executing products publish.");

                return false;
            }

            List<SupplierBuyerGroupBuyerParticipationRemoteModel> allBuyerGroups;
            try
            {
                allBuyerGroups = WebServiceHelper.GetAllBuyerGroups();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Could not load buyer groups " + ex.Message);
                return false;
            }

            var customers = new CustomerRepository(Settings.ConnectionString).GetAll().ToList();

            var companyItemImport = new List<CompanyItemImport>();

            var total = 0;
            var numOfPublishedCustomers = 0;
            foreach (var customer in customers)
            {
                total += 1;
                bw?.ReportProgress(0, $"Processing customer sync (Pushing {total} / {customers.Count}\r\nPlease wait");
                try
                {
                    var request = MapCustomers(customer, allBuyerGroups);
                    if (request != null)
                        companyItemImport.Add(request);

                    if (companyItemImport.Count == 400)
                    {
                        ProcessCompanyImport(companyItemImport);
                        companyItemImport.Clear();
                    }

                }
                catch (Exception ex)
                {
                    Logger.Instance.Error(ex.Message);
                }

                numOfPublishedCustomers++;
            }

            if (companyItemImport.Count < 400)
                ProcessCompanyImport(companyItemImport);

            if (numOfPublishedCustomers == 0)
            {
                Logger.Instance.Warning("No customers were found to import.");
            }

            publishDetails.Insert(0, $"{total} customers published to OrderEase");

            Logger.Instance.Info($"{total} Customers published.");
            Logger.Instance.Debug($"{total} Customers published. ApiKey: {apiKey}");

            return true;
        }

        private static int ProcessCompanyImport(List<CompanyItemImport> companyToImport)
        {
            var importRequest = new CompanyImportRequest()
            {
                Companies = companyToImport,
                ImportConfig = new CompanyImportConfig()
            };

            importRequest.ImportConfig.ForceMode = UserMode.Buyer;

            importRequest.ImportConfig.IntegrationKey = Guid.Parse("7201CCB7-AF24-4B16-9459-4DFCF8F9CB8A");

            importRequest.ImportConfig.MappedColumns = new List<string>()
            {
                nameof(CompanyItemImport.Name),
                nameof(CompanyItemImport.FormattedPhone1),
                nameof(CompanyItemImport.Email1),
                nameof(CompanyItemImport.Address1),
                nameof(CompanyItemImport.Address2),
                nameof(CompanyItemImport.City),
                nameof(CompanyItemImport.Country),
                nameof(CompanyItemImport.ProvState),
                nameof(CompanyItemImport.PostalCode),
                nameof(CompanyItemImport.Web),
                nameof(CompanyItemImport.BillToAddress1),
                nameof(CompanyItemImport.BillToAddress2),
                nameof(CompanyItemImport.BillToAddressCity),
                nameof(CompanyItemImport.BillToAddressCountry),
                nameof(CompanyItemImport.BillToAddressProvState),
                nameof(CompanyItemImport.BillToAddressPostalCode),
                nameof(CompanyItemImport.ExternalReference1),
                nameof(CompanyItemImport.ExternalReference2),
                nameof(CompanyItemImport.ExternalSource),
                nameof(CompanyItemImport.PaymentTerm),
                nameof(CompanyItemImport.AddToBuyerGroups),
                nameof(CompanyItemImport.RemoveFromBuyerGroups),
                nameof(CompanyItemImport.Contacts),
                nameof(CompanyItemImport.OurCompanyNumber)
            };

            importRequest.ImportConfig.IgnoreEmptyValuesColumns = new List<string>()
            {
                nameof(CompanyItemImport.OurBillToNumber),
                nameof(CompanyItemImport.UserDefinedField1),
                nameof(CompanyItemImport.UserDefinedField2),
                nameof(CompanyItemImport.UserDefinedField3),
                nameof(CompanyItemImport.UserDefinedField4),
                nameof(CompanyItemImport.PaymentTerm)
            };

            long timestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            importRequest.ImportConfig.FileName = $"ODBC_Customers_{timestamp}_{companyToImport.Count}.json";

            return WebServiceHelper.ImportCustomers(importRequest);
        }

        private static CompanyItemImport MapCustomers(CompanyAndRelationshipResult customer, List<SupplierBuyerGroupBuyerParticipationRemoteModel> allBuyerGroups)
        {
            var companyItemImport = new CompanyItemImport();

            companyItemImport.Name = customer.Name;
            companyItemImport.OurCompanyNumber = customer.OurCompanyNumber;
            companyItemImport.FormattedPhone1 = customer.FormattedPhone1;
            companyItemImport.Email1 = customer.Email1;
            companyItemImport.Address1 = customer.Address1;
            companyItemImport.Address2 = customer.Address2;
            companyItemImport.City = customer.City;
            companyItemImport.Country = customer.Country;
            companyItemImport.ProvState = customer.ProvState;
            companyItemImport.PostalCode = customer.PostalCode;
            companyItemImport.Web = customer.Web;
            companyItemImport.SalesRep = customer.SalesRepEmail;
            companyItemImport.AddToBuyerGroups = new List<BuyerGroupInfo>();

            if (!string.IsNullOrWhiteSpace(customer.BuyerGroup))
            {
                var groups = customer.BuyerGroup.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                var toAdd = new List<int>();
                foreach (var currentGroup in groups)
                {
                    var group = allBuyerGroups.FirstOrDefault(g => g.Name.Trim().Equals(currentGroup.Trim(), StringComparison.CurrentCultureIgnoreCase));
                    if (group == null)
                    {
                        Logger.Instance.Info($"Unable to find group {customer.BuyerGroup}");
                    }
                    else
                    {
                        //Add customer to the location group
                        companyItemImport.AddToBuyerGroups.Add(new BuyerGroupInfo
                        {
                            Name = group.Name
                        });

                    };
                }
            }

            //Add contacts
            companyItemImport.Contacts = new List<RelationshipContact>()
            {
                new RelationshipContact()
                {
                    FirstName = customer.ContactName,
                    LastName = "",
                    EmailAddress = customer.ContactEmail,
                    Phone = customer.ContactPhone,
                    ExternalReference = customer.Contact1,
                    ExternalSource = "201"
                }
            };

            companyItemImport.ExternalReference1 = customer.Id.ToString();
            companyItemImport.ExternalSource = "201";

            return companyItemImport;
        }

    }
}
