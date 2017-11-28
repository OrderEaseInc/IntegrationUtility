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
    public class SupplierRepository : AdoRepository<Supplier>
    {
        private const string TableName = "Suppliers";

        public SupplierRepository(string connectionString) : base(connectionString) { }

        public int DownloadAllSuppliers()
        {
            var suppliers = GetAll();
            if (suppliers == null) {
                return 0;
            }

            foreach (var supplier in suppliers) {
                Insert(supplier);
            }

            return suppliers.Count;
        }

        public int SyncAllSuppliers()
        {
            var count = 0;
            var lgSuppliers = WebServiceHelper.GetAllSuppliers().ToDictionary(s => s.Id);

            IEnumerable<Supplier> updatedSuppliers;
            using (var command = new OdbcCommand($"SELECT * FROM {TableName}")) {
                updatedSuppliers = GetRecords(command);
            }

            foreach (var supplier in updatedSuppliers.Where(s => lgSuppliers.ContainsKey(s.Id))) {
                var lgSupplier = lgSuppliers[supplier.Id];
                WebServiceHelper.UpdateSupplierContactInfo(lgSupplier, supplier.OurContactInfo.OurSupplierNumber);
                count++;
            }

            return count;
        }

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

        private List<Supplier> GetAll()
        {
            var suppliers = WebServiceHelper.GetAllSuppliers();
            return suppliers;
        }
        
        private void Insert(Supplier supplier)
        {
            var sql =
                $"INSERT INTO {TableName} (SupplierId, SupplierName, ContactName, Email, Phone, OurBillToNumber, OurSupplierNumber) " +
                $"Values ({supplier.Id}, {NullableString(supplier.Name)}, {NullableString(supplier.OurContactInfo?.ContactName)}, " +
                $"{NullableString(supplier.OurContactInfo?.Email)}, {NullableString(supplier.OurContactInfo?.Phone)}, " +
                $"{NullableString(supplier.OurContactInfo?.OurBillToNumber)}, {NullableString(supplier.OurContactInfo?.OurSupplierNumber)})";
            using (var command = new OdbcCommand(sql)) {
                ExecuteCommand(command);
            }
        }

        protected override Supplier PopulateRecord(dynamic reader)
        {
            try {

                return new Supplier {
                    Id = reader.SupplierId,
                    Name = reader.SupplierName,
                    OurContactInfo = new SupplierContact {
                        Id = reader.SupplierId,
                        OurSupplierNumber = reader.OurSupplierNumber
                    }
                };

            } catch (RuntimeBinderException exception) {
                Console.WriteLine(exception);
                throw new InvalidDataException("One of the fields in the source ODBC database has an invalid column type or value", exception);
            }
        }
    }
}