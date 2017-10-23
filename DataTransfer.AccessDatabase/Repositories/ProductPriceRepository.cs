using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.Odbc;
using System.Data.SqlClient;
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

        private const string TableName = "ProductPrices";
        private const string TableKey = "IDNumber";

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

        // NOTE : this is the wire-up of the local odbc table to strongly typed object to be sent via api to LG db
        protected override ProductPrice PopulateRecord(dynamic reader)
        {
            try
            {
                return new ProductPrice
                {
                    IDNumber = reader.IDNumber.ToString(),
                    Group = reader.Group,
                    Territory = reader.Territory,
                    CaseSellPrice = reader.CaseSellPrice != null ? (decimal?)reader.CaseSellPrice : null,
                    HalfCaseSellPrice = reader.HalfCaseSellPrice != null ? (decimal?)reader.HalfCaseSellPrice : null,
                    SingleSellPrice = reader.SingleSellPrice != null ? (decimal?)reader.SingleSellPrice : null
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