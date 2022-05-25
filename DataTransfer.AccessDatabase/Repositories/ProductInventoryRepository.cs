using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.OleDb;
using System.IO;
using Microsoft.CSharp.RuntimeBinder;

// ReSharper disable once CheckNamespace
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

        /// <summary>
        /// Try to get the RetailSell value from the reader - may not exist on reader. and will throw exception if it doesn't exist.  
        /// So just catch and assign false if it doesn't exist
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static bool TryGetRetailSell(dynamic reader)
        {
            try
            {
                return reader.RetailSell != null &&
                                         (reader.RetailSell.Equals(true) ||
                                          Convert.ToString(reader.RetailSell).ToLower() == "true" ||
                                          Convert.ToString(reader.RetailSell).ToLower() == "1" ||
                                          Convert.ToString(reader.RetailSell).ToLower() == "y" ||
                                          Convert.ToString(reader.RetailSell).ToLower() == "yes");
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Try to get the RetailSell value from the reader - may not exist on reader. and will throw exception if it doesn't exist.  
        /// So just catch and assign false if it doesn't exist
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static bool TryGetDropShipSell(dynamic reader)
        {
            try
            {
                return reader.DropShipSell != null &&
                       (reader.DropShipSell.Equals(true) ||
                        Convert.ToString(reader.DropShipSell).ToLower() == "true" ||
                        Convert.ToString(reader.DropShipSell).ToLower() == "1" ||
                        Convert.ToString(reader.DropShipSell).ToLower() == "y" ||
                        Convert.ToString(reader.DropShipSell).ToLower() == "yes");
            }
            catch
            {
                return false;
            }
        }

        // NOTE : this is the wire-up of the local odbc table to strongly typed object to be sent via api to LG db
        protected override ProductInventory PopulateRecord(dynamic reader)
        {
            try
            {
                var pi = new ProductInventory
                {
                    Id = reader.PrivateSKU.ToString(),
                    Inactive = reader.Inactive != null && (reader.Inactive.Equals(true) ||
                                                           Convert.ToString(reader.Inactive).ToLower() == "true" ||
                                                           Convert.ToString(reader.Inactive).ToLower() == "1" ||
                                                           Convert.ToString(reader.Inactive).ToLower() == "y" ||
                                                           Convert.ToString(reader.Inactive).ToLower() == "yes"),
                    Category = reader.Category ?? "",
                    Description = reader.Description ?? "",
                    Comments = reader.Comments ?? "",
                    DirectDeliveryCode = reader.DirectDeliveryCode ?? "",
                    DirectDeliveryMinQuantity = reader.DirectDeliveryMinQuantity ?? 0,
                    FreightFactor = reader.FreightFactor ?? 0,
                    IsDirectDelivery = reader.IsDirectDelivery != null && (reader.IsDirectDelivery.Equals(true) ||
                                                                           Convert.ToString(reader.IsDirectDelivery)
                                                                               .ToLower() == "true" ||
                                                                           Convert.ToString(reader.IsDirectDelivery)
                                                                               .ToLower() == "1" ||
                                                                           Convert.ToString(reader.IsDirectDelivery)
                                                                               .ToLower() == "y" ||
                                                                           Convert.ToString(reader.IsDirectDelivery)
                                                                               .ToLower() == "yes"),
                    MasterQuantityDescription = reader.MasterQuantityDescription ?? "",
                    MinOrderSpring = reader.MinOrderSpring ?? 0,
                    MinOrderSummer = reader.MinOrderSummer ?? 0,
                    NetPrice = reader.NetPrice != null ? (decimal)reader.NetPrice : 0,
                    OpenSizeDescription = reader.OpenSizeDescription ?? "",
                    PrivateSKU = reader.PrivateSKU.ToString(),
                    QuantityAvailable = reader.QuantityAvailable ?? 0,
                    SlaveQuantityDescription = reader.SlaveQuantityDescription ?? "",
                    SlaveQuantityPerMaster = reader.SlaveQuantityPerMaster ?? 0,
                    SuggestedRetailPrice =
                        reader.SuggestedRetailPrice != null ? (decimal)reader.SuggestedRetailPrice : 0,
                    UPC = reader.UPC,
                    RetailSell = TryGetRetailSell(reader),
                    DropShipSell = TryGetDropShipSell(reader)
                };

                var t = (Type)reader.GetType();

                pi.ProductFeatures = new List<KeyValuePair<string, object>>();
                if (!string.IsNullOrWhiteSpace(reader.ProductFeature_1_Name))
                    pi.ProductFeatures.Add(new KeyValuePair<string, object>(reader.ProductFeature_1_Name, reader.ProductFeature_1_Value));
                if (!string.IsNullOrWhiteSpace(reader.ProductFeature_2_Name))
                    pi.ProductFeatures.Add(new KeyValuePair<string, object>(reader.ProductFeature_2_Name, reader.ProductFeature_2_Value));
                if (!string.IsNullOrWhiteSpace(reader.ProductFeature_3_Name))
                    pi.ProductFeatures.Add(new KeyValuePair<string, object>(reader.ProductFeature_3_Name, reader.ProductFeature_3_Value));
                if (!string.IsNullOrWhiteSpace(reader.ProductFeature_4_Name))
                    pi.ProductFeatures.Add(new KeyValuePair<string, object>(reader.ProductFeature_4_Name, reader.ProductFeature_4_Value));
                if (!string.IsNullOrWhiteSpace(reader.ProductFeature_5_Name))
                    pi.ProductFeatures.Add(new KeyValuePair<string, object>(reader.ProductFeature_5_Name, reader.ProductFeature_5_Value));
                if (!string.IsNullOrWhiteSpace(reader.ProductFeature_6_Name))
                    pi.ProductFeatures.Add(new KeyValuePair<string, object>(reader.ProductFeature_6_Name, reader.ProductFeature_6_Value));
                if (!string.IsNullOrWhiteSpace(reader.ProductFeature_7_Name))
                    pi.ProductFeatures.Add(new KeyValuePair<string, object>(reader.ProductFeature_7_Name, reader.ProductFeature_7_Value));
                if (!string.IsNullOrWhiteSpace(reader.ProductFeature_8_Name))
                    pi.ProductFeatures.Add(new KeyValuePair<string, object>(reader.ProductFeature_8_Name, reader.ProductFeature_8_Value));
                if (!string.IsNullOrWhiteSpace(reader.ProductFeature_9_Name))
                    pi.ProductFeatures.Add(new KeyValuePair<string, object>(reader.ProductFeature_9_Name, reader.ProductFeature_9_Value));
                if (!string.IsNullOrWhiteSpace(reader.ProductFeature_10_Name))
                    pi.ProductFeatures.Add(new KeyValuePair<string, object>(reader.ProductFeature_10_Name, reader.ProductFeature_10_Value));


                return pi;
            }
            catch (RuntimeBinderException exception)
            {
                Console.WriteLine(exception);
                throw new InvalidDataException("One of the fields in the source ODBC database has an invalid column type or value", exception);
            }
        }
    }
}