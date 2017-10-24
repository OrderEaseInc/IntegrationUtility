using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.Odbc;
using System.IO;
using Microsoft.CSharp.RuntimeBinder;

namespace DataTransfer.AccessDatabase
{
    public class ProductInventoryRepository : AdoRepository<ProductInventory>
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
            using (var command = new OdbcCommand($"SELECT * FROM {TableName}"))
            {
                return GetRecords(command);
            }
        }

        public ProductInventory GetByLocalSku(string sku)
        {
            // PARAMETERIZED QUERIES!
            using (var command = new OdbcCommand($"SELECT * FROM {TableName} WHERE {TableKey} = @sku"))
            {
                command.Parameters.Add(new ObjectParameter("sku", sku));
                return GetRecord(command);
            }
        }

        #endregion

        public void ClearAll()
        {
            using (var command = new OdbcCommand($"DELETE * FROM {TableName}"))
            {
                ExecuteCommand(command);
            }
        }

        public override void SaveFieldMapping(string fieldName, string mappingName)
        {
            using (var command = new OdbcCommand($"UPDATE `FieldMappings` SET `MappingName` = '{mappingName}' WHERE `FieldName` = '{fieldName}' AND `TableName` = '{TableName}'"))
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
                    Active = reader.Active != null && (reader.Active.Equals(true) || Convert.ToString(reader.Active).ToLower() == "true" || Convert.ToString(reader.Active).ToLower() == "1"),
                    Category = reader.Category ?? "",
                    Description = reader.Description ?? "",
                    Comments = reader.Comments ?? "",
                    DirectDeliveryCode = reader.DirectDeliveryCode ?? "",
                    DirectDeliveryMinQuantity = reader.DirectDeliveryMinQuantity,
                    FreightFactor = reader.FreightFactor ?? "",
                    IsDirectDelivery = reader.IsDirectDelivery != null && (reader.IsDirectDelivery.Equals(true) || Convert.ToString(reader.IsDirectDelivery).ToLower() == "true" || Convert.ToString(reader.IsDirectDelivery).ToLower() == "1"),
                    MasterQuantityDescription = reader.MasterQuantityDescription ?? "",
                    MinOrderSpring = reader.MinOrderSpring,
                    MinOrderSummer = reader.MinOrderSummer,
                    NetPrice = reader.NetPrice != null ? (decimal)reader.NetPrice : 0,
                    OpenSizeDescription = reader.OpenSizeDescription ?? "",
                    PrivateSKU = reader.PrivateSKU.ToString(),
                    QuantityAvailable = reader.QuantityAvailable,
                    SlaveQuantityDescription = reader.SlaveQuantityDescription ?? "",
                    SlaveQuantityPerMaster = reader.SlaveQuantityPerMaster,
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