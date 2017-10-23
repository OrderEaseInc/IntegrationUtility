using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.IO;
using Microsoft.CSharp.RuntimeBinder;

namespace DataTransfer.AccessDatabase
{
    public class ProductCategoryRepository : AdoRepository<ProductCategory>
    {
        public ProductCategoryRepository(string connectionString) : base(connectionString)
        {
        }

        #region Private Consts

        private const string TableName = "Category";
        private const string TableKey = "";

        #endregion

        #region Get

        public IEnumerable<ProductCategory> GetAll()
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
        protected override ProductCategory PopulateRecord(dynamic reader)
        {
            try
            {
                return new ProductCategory
                {
                    Group = reader.Group,
                    Category = reader.Category
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