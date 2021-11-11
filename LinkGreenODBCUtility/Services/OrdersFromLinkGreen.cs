using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using DataTransfer.AccessDatabase;

// ReSharper disable once CheckNamespace
namespace LinkGreenODBCUtility
{
    public class OrdersFromLinkGreen : IOdbcTransfer
    {
        private readonly OrdersFromLinkGreenRepository _repository;

        public OrdersFromLinkGreen()
        {
            _repository = new OrdersFromLinkGreenRepository(Settings.ConnectionString);
        }

        public bool Empty()
        {
            _repository.ClearAll();
            Logger.Instance.Info("Orders from LinkGreen table emptied");
            Logger.Instance.Debug($"{Settings.ConnectionString}.{OrdersFromLinkGreenRepository.TableName} emptied");
            return true;
        }

        public void SaveTableMapping(string dsnName, string tableName)
        {
            _repository.SaveTableMapping(dsnName, tableName, OrdersFromLinkGreenRepository.TableName);
            Logger.Instance.Debug($"Orders from LinkGreen table mapping saved: (DSN: {dsnName}, Table: {OrdersFromLinkGreenRepository.TableName})");
        }

        public void SaveItemsTableMapping(string dsnName, string tableName)
        {
            _repository.SaveTableMapping(dsnName, tableName, OrdersFromLinkGreenRepository.ItemsTableName);
            Logger.Instance.Debug($"Order items from LinkGreen table mapping saved: (DSN: {dsnName}, Table: {OrdersFromLinkGreenRepository.ItemsTableName})");
        }

        public void SaveFieldMapping(string fieldName, string mappingName)
        {
            _repository.SaveFieldMapping(fieldName, mappingName);
            Logger.Instance.Debug($"Orders from LinkGreen field mapping saved: (Field: {fieldName}, MappingField: {mappingName})");
        }

        public void SaveItemFieldMapping(string fieldName, string mappingName)
        {
            _repository.SaveItemFieldMapping(fieldName, mappingName);
            Logger.Instance.Debug($"Order items from LinkGreen field mapping saved: (Field: {fieldName}, MappingField: {mappingName})");
        }

        public bool Download()
        {
            var status = Settings.GetStatusIdForOrderDownload();
            if (status == null || !status.Any())
            {
                Logger.Instance.Error("Status ID for Order Downloads missing");
                return false;
            }

            _repository.Download(status);
            Logger.Instance.Debug($"Downloaded from LinkGreen to Transfer table {OrdersFromLinkGreenRepository.TableName}");
            return true;
        }

        public bool Publish(out List<string> processDetails, BackgroundWorker bw = null)
        {
            processDetails = new List<string>();
            try
            {
                Empty();
                Download();

                var mappedDsnName = new Mapping().GetDsnName(OrdersFromLinkGreenRepository.TableName);
                var newMapping = new Mapping(mappedDsnName);
                var pushed = newMapping.PushData(OrdersFromLinkGreenRepository.TableName, OrdersFromLinkGreenRepository.TableKey, true);
                if (pushed)
                {
                    Logger.Instance.Debug("Orders migrated from utility to mapped production database.");

                    pushed = newMapping.PushData(OrdersFromLinkGreenRepository.ItemsTableName, OrdersFromLinkGreenRepository.TableKey, true);

                    if (pushed)
                    {
                        Logger.Instance.Debug("Orders migrated from utility to mapped production database.");
                        processDetails.Add("Orders migrated from utility to mapped production database.");
                        return true;
                    }

                    Logger.Instance.Error("Failed to migrate order items from utility to mapped production database.");
                    processDetails.Add("Failed to migrate order items from utility to mapped production database.");
                    return false;
                }

                Logger.Instance.Error("Failed to migrate orders from utility to mapped production database.");
                processDetails.Add("Failed to migrate orders from utility to mapped production database.");
                return false;

            }
            catch (ConfigurationErrorsException ex)
            {
                Logger.Instance.Error($"Error Publishing Orders From LinkGreen: {ex.GetBaseException().Message}");
                processDetails.Add(ex.GetBaseException().Message);
                return false;
            }
        }
    }
}