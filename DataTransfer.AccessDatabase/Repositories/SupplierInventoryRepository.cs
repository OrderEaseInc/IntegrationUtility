using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using LinkGreen.Applications.Common;
using LinkGreen.Applications.Common.Model;
using Microsoft.CSharp.RuntimeBinder;

namespace DataTransfer.AccessDatabase
{
    public class SupplierInventoryRepository : OleDbRepository<SupplierInventory>
    {
        private const string TableName = "SupplierInventories";

        public SupplierInventoryRepository(string connectionString) : base(connectionString) { }

        public void DownloadSupplierInventory()
        {
            var suppliers = WebServiceHelper.GetAllSuppliers();

            // NOTE: just pulling inventories for suppliers we have internal records for.
            //  Questionable - maybe this should be configurable?
            foreach (var supplier in suppliers.Where(s => !string.IsNullOrEmpty(s.OurContactInfo.OurSupplierNumber)))
            {
                var inventory = WebServiceHelper.GetSupplierInventory(supplier.Id);
                if (inventory != null)
                {
                    foreach (var item in inventory)
                    {
                        Insert(item, supplier);
                    }
                }
            }
        }

        public override void SaveFieldMapping(string fieldName, string mappingName)
        {
            using (var command = new OleDbCommand($"UPDATE `FieldMappings` SET `MappingName` = '{mappingName}' WHERE `FieldName` = '{fieldName}' AND `TableName` = '{TableName}'"))
            {
                ExecuteCommand(command);
            }
        }

        public void ClearAll()
        {
            using (var command = new OleDbCommand($"DELETE * FROM {TableName}"))
            {
                ExecuteCommand(command);
            }
        }

        private void Insert(SupplierInventory inventory, Supplier supplier)
        {
            var buyerLinkedSkus = inventory.BuyerLinkedSkus?.Any() == true ? inventory.BuyerLinkedSkus : new[] { default(string) };
            foreach (var buyerLinkedSku in buyerLinkedSkus)
            {
                var sql =
                    $"INSERT INTO {TableName} (BuyerLinkedSku, CatalogPrice, Description, Inventory, ItemId, SizeDescription, SupplierSku, SupplierId, OurSupplierNumber, ModifiedDate) " +
                    $"VALUES ({NullableString(buyerLinkedSku)}, {NullableDecimal(inventory.CatalogPrice)}, " +
                    $"{NullableString(inventory.Description)}, {NullableInt(inventory.Inventory)}, {inventory.ItemId}," +
                    $"{NullableString(inventory.SizeDescription)}, {NullableString(inventory.SupplierSku)}, {NullableInt(supplier.Id)}, {NullableString(supplier.OurContactInfo.OurSupplierNumber)}, {NullableString(inventory.ModifiedDate?.ToString("yyyy-MM-dd hh:mm:ss"))})";
                using (var command = new OleDbCommand(sql))
                {
                    ExecuteCommand(command);
                }
            }
        }

        public int SyncAllSupplierInventories()
        {

            var count = 0;
            var suppliers = WebServiceHelper.GetAllSuppliers();
            // NOTE: just pulling inventories for suppliers we have internal records for.
            //  Questionable - maybe this should be configurable?
            foreach (var supplier in suppliers.Where(s => !string.IsNullOrEmpty(s.OurContactInfo.OurSupplierNumber)))
            {
                var lgSupplierInventories = WebServiceHelper.GetSupplierInventory(supplier.Id).ToDictionary(si => si.ItemId);
                var command = new OleDbCommand($"SELECT * FROM {TableName} Where SupplierId = {supplier.Id}");
                var updatedSupplierInventories = GetRecords(command);
                foreach (var inventory in updatedSupplierInventories.Where(i => !string.IsNullOrEmpty(i.BuyerLinkedSku) && lgSupplierInventories.ContainsKey(i.ItemId)))
                {
                    var lgSupplierInventory = lgSupplierInventories[inventory.ItemId];
                    if (!string.IsNullOrEmpty(inventory.BuyerLinkedSku) && (lgSupplierInventory.BuyerLinkedSkus == null || !lgSupplierInventory.BuyerLinkedSkus.Any(sku => sku == inventory.BuyerLinkedSku)))
                    {
                        // this didn't come in from the web service
                        lgSupplierInventory.SupplierId = supplier.Id;
                        WebServiceHelper.UpdateSupplierInventory(lgSupplierInventory, inventory.BuyerLinkedSku);
                        count++;
                    }
                }
            }
            return count;
        }

        protected override SupplierInventory PopulateRecord(dynamic reader)
        {
            try
            {

                return new SupplierInventory
                {
                    BuyerLinkedSku = reader.BuyerLinkedSku,
                    CatalogPrice = reader.CatalogPrice,
                    Description = reader.Description,
                    Inventory = reader.Inventory,
                    ItemId = reader.ItemId,
                    SizeDescription = reader.SizeDescription,
                    SupplierId = reader.SupplierId,
                    OurSupplierNumber = reader.OurSupplierNumber,
                    Pricing = new List<QuantityPricingBreak>()
                };

            }
            catch (RuntimeBinderException exception)
            {
                Console.WriteLine(exception);
                throw new InvalidDataException("One of the fields in the source ODBC database has an invalid column type or value", exception);
            }
        }
    }
}