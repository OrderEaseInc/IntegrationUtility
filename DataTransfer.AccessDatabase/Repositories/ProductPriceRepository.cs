﻿using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using Microsoft.CSharp.RuntimeBinder;

namespace DataTransfer.AccessDatabase
{
    public class ProductPriceRepository : AdoRepository<ProductPrice>
    {
        public ProductPriceRepository(string connectionString) : base(connectionString)
        {
        }

        #region Private Consts

        private const string TableName = "PriceLevelPrices";
        private const string TableKey = "Id";

        #endregion

        #region Get

        public IEnumerable<ProductPrice> GetAll()
        {
            // DBAs across the country are having strokes 
            //  over this next command!
            using (var command = new OdbcCommand($"SELECT * FROM {TableName}"))
            {
                return GetRecords(command);
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
        protected override ProductPrice PopulateRecord(dynamic reader)
        {
            try
            {
                return new ProductPrice
                {
                    Id = reader.ProductId.ToString(),
                    PriceLevel = reader.PriceLevel.ToString(),
                    Price = (decimal)reader.Price,
                    MinimumPurchase = (int)reader.MinimumPurchase
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