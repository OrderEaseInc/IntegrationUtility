using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.Odbc;
using System.Data.SqlClient;
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

        private const string TableName = "Product";
        private const string TableKey = "IDNumber";

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

        // NOTE : this is the wire-up of the local odbc table to strongly typed object to be sent via api to LG db
        protected override ProductInventory PopulateRecord(dynamic reader)
        {
            try
            {
                return new ProductInventory
                {
                    Id = reader.IDNumber.ToString(),
                    Group = reader.Group,
                    Category = reader.Category,
                    Description1 = reader.Description1,
                    Description2 = reader.Description2,
                    Description3 = reader.Description3,
                    Color = reader.Color,
                    FullCaseQty = (int)reader.CaseSellUICapacity,
                    HalfCaseQty = (int)reader.HalfCaseSellUICapacity,
                    Sell = reader.SellUI,
                    Active = (bool)reader.Active,
                    CaseBoxName = reader.CaseBoxName ?? string.Empty,
                    HalfCaseBoxName = reader.HalfCaseBoxName ?? string.Empty
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