using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.OleDb;
using System.IO;
using Microsoft.CSharp.RuntimeBinder;

namespace DataTransfer.AccessDatabase
{
    public class ProductInventoryRepository : OleDbRepository<ProductInventory>
    {
        public ProductInventoryRepository(string connectionString) : base(connectionString)
        {
        }

        #region Private Consts

        private const string TableName = "Products";
        private const string TableKey = "Id";

        #endregion

        #region Get

        public IEnumerable<ProductInventory> GetAll()
        {
            // DBAs across the country are having strokes 
            //  over this next command!
            using (var command = new OleDbCommand($"SELECT * FROM {TableName}"))
            {
                return GetRecords(command);
            }
        }

        public ProductInventory GetByLocalSku(string sku)
        {
            // PARAMETERIZED QUERIES!
            using (var command = new OleDbCommand($"SELECT * FROM {TableName} WHERE {TableKey} = @sku"))
            {
                command.Parameters.Add(new ObjectParameter("sku", sku));
                return GetRecord(command);
            }
        }

        #endregion

        public void ClearAll()
        {
            using (var command = new OleDbCommand($"DELETE * FROM {TableName}"))
            {
                ExecuteCommand(command);
            }
        }

        public override void SaveFieldMapping(string fieldName, string mappingName)
        {
            using (var command = new OleDbCommand($"UPDATE `FieldMappings` SET `MappingName` = '{mappingName}' WHERE `FieldName` = '{fieldName}' AND `TableName` = '{TableName}'"))
            {
                ExecuteCommand(command);
            }
        }

        // NOTE : this is the wire-up of the local odbc table to strongly typed object to be sent via api to LG db
        protected override ProductInventory PopulateRecord(dynamic reader)
        {
            try
            {
                return new ProductInventory
                {
                    Id = reader.PrivateSKU.ToString(),
                    Inactive = reader.Inactive != null && (reader.Inactive.Equals(true) || Convert.ToString(reader.Inactive).ToLower() == "true" || Convert.ToString(reader.Inactive).ToLower() == "1" || Convert.ToString(reader.Inactive).ToLower() == "y" || Convert.ToString(reader.Inactive).ToLower() == "yes"),
                    Category = reader.Category ?? "",
                    Description = reader.Description ?? "",
                    Comments = reader.Comments ?? "",
                    DirectDeliveryCode = reader.DirectDeliveryCode ?? "",
                    DirectDeliveryMinQuantity = reader.DirectDeliveryMinQuantity ?? 0,
                    FreightFactor = reader.FreightFactor ?? 0,
                    IsDirectDelivery = reader.IsDirectDelivery != null && (reader.IsDirectDelivery.Equals(true) || Convert.ToString(reader.IsDirectDelivery).ToLower() == "true" || Convert.ToString(reader.IsDirectDelivery).ToLower() == "1" || Convert.ToString(reader.IsDirectDelivery).ToLower() == "y" || Convert.ToString(reader.IsDirectDelivery).ToLower() == "yes"),
                    MasterQuantityDescription = reader.MasterQuantityDescription ?? "",
                    MinOrderSpring = reader.MinOrderSpring ?? 0,
                    MinOrderSummer = reader.MinOrderSummer ?? 0,
                    NetPrice = reader.NetPrice != null ? (decimal)reader.NetPrice : 0,
                    OpenSizeDescription = reader.OpenSizeDescription ?? "",
                    PrivateSKU = reader.PrivateSKU.ToString(),
                    QuantityAvailable = reader.QuantityAvailable ?? 0,
                    SlaveQuantityDescription = reader.SlaveQuantityDescription ?? "",
                    SlaveQuantityPerMaster = reader.SlaveQuantityPerMaster ?? 0,
                    SuggestedRetailPrice = reader.SuggestedRetailPrice != null ? (decimal)reader.SuggestedRetailPrice : 0,
                    UPC = reader.UPC
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