using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using LinkGreen.Applications.Common;
using LinkGreen.Applications.Common.Model;
using Microsoft.CSharp.RuntimeBinder;

namespace DataTransfer.AccessDatabase
{
    public class LinkedSkusRepository : AdoRepository<LinkedItem>
    {
        private const string TableName = "LinkedSkus";

        public LinkedSkusRepository(string connectionString) : base(connectionString) { }

        public override void SaveFieldMapping(string fieldName, string mappingName)
        {
            using (OdbcCommand command = new OdbcCommand($"UPDATE `FieldMappings` SET `MappingName` = '{mappingName}' WHERE `FieldName` = '{fieldName}' AND `TableName` = '{TableName}'")) {
                ExecuteCommand(command);
            }
        }

        public void ClearAll()
        {
            using (OdbcCommand command = new OdbcCommand($"DELETE * FROM {TableName}")) {
                ExecuteCommand(command);
            }
        }

        public int SyncAllLinkedSkus()
        {
            var count = 0;
            var suppliers = WebServiceHelper.GetAllSuppliers();
            // NOTE: just pulling inventories for suppliers we have internal records for.
            //  Questionable - maybe this should be configurable?
            foreach (var supplier in suppliers.Where(s => !string.IsNullOrEmpty(s.OurContactInfo.OurSupplierNumber))) {
                var lgSupplierInventories = WebServiceHelper.GetSupplierInventory(supplier.Id).ToDictionary(si => si.SupplierSku);
                var command = new OdbcCommand($"SELECT * FROM {TableName} Where SupplierId = {supplier.Id}");
                var skusToLink = GetRecords(command);
                foreach (var link in skusToLink.Where(i => !string.IsNullOrEmpty(i.BuyerSku) && lgSupplierInventories.ContainsKey(i.SupplierSku))) {
                    var lgSupplierInventory = lgSupplierInventories[link.SupplierSku];
                    if (!string.IsNullOrEmpty(link.BuyerSku) && (lgSupplierInventory.BuyerLinkedSkus == null || !lgSupplierInventory.BuyerLinkedSkus.Any(sku => sku == link.BuyerSku))) {
                        // this didn't come in from the web service
                        lgSupplierInventory.SupplierId = supplier.Id;
                        WebServiceHelper.UpdateSupplierInventory(lgSupplierInventory, link.BuyerSku);
                        count++;
                    }
                }
            }
            return count;
        }

        protected override LinkedItem PopulateRecord(dynamic reader)
        {
            try {

                return new LinkedItem {
                    BuyerSku = reader.BuyerSku,
                    SupplierSku = reader.SupplierSku,
                    SupplierId = reader.SupplierId
                };

            } catch (RuntimeBinderException exception) {
                Console.WriteLine(exception);
                throw new InvalidDataException("One of the fields in the source ODBC database has an invalid column type or value", exception);
            }
        }
    }
}