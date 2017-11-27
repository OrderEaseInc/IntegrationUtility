using System;
using System.Collections.Generic;
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

        private const string TableName = "Categories";
        private const string TableKey = "";

        #endregion

        #region Get

        public IEnumerable<ProductCategory> GetAll()
        {
            // DBAs across the country are having strokes 
            //  over this next command!
            return GetRecords($"SELECT * FROM `{TableName}`");
        }

        #endregion

        public void ClearAll()
        {
            ExecuteCommand($"DELETE * FROM `{TableName}`");
        }

        public override void SaveFieldMapping(string fieldName, string mappingName)
        {
            ExecuteCommand($"UPDATE `FieldMappings` SET `MappingName` = '{mappingName}' WHERE `FieldName` = '{fieldName}' AND `TableName` = '{TableName}'");
        }

        // NOTE : this is the wire-up of the local odbc table to strongly typed object to be sent via api to LG db
        protected override ProductCategory PopulateRecord(dynamic reader)
        {
            try
            {
                return new ProductCategory
                {
                    Category = reader.Name
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