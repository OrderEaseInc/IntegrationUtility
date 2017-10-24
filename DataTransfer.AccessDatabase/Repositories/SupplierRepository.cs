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
        private const string TableKey = "SupplierId";

        public SupplierRepository(string connectionString) : base(connectionString) { }

        public int DownloadAllSuppliers()
        {
            var suppliers = GetAll();
            foreach (var supplier in suppliers) {
                Insert(supplier);
            }
            return suppliers.Count();
        }

        public Dictionary<SupplierSyncStates, int> SyncAllSuppliers()
        {
            var result = new Dictionary<SupplierSyncStates, int> {
                { SupplierSyncStates.Added, 0 },
                { SupplierSyncStates.Unchanged, 0 },
                { SupplierSyncStates.Mapped, 0 },
                { SupplierSyncStates.MappingRequired, 0 }
            };
            var suppliers = GetAll();
            foreach (var supplier in suppliers) {
                var state = Sync(supplier);
                result[state]++;
            }

            return result;
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

        private IEnumerable<Supplier> GetAll()
        {
            var suppliers = WebServiceHelper.GetAllSuppliers();
            return suppliers;
        }
        
        private SupplierSyncStates Sync(Supplier supplier)
        {
            var supplierNumber = supplier.OurContactInfo?.OurSupplierNumber;
            var state = SupplierSyncStates.Unchanged;
            var existingSupplier = GetRecord(new OdbcCommand($"SELECT * FROM {TableName} Where {TableKey} = {supplier.Id}"));
            var existingSupplierNumber = existingSupplier?.OurContactInfo?.OurSupplierNumber;
            if (existingSupplier == null) {
                Insert(supplier);
                state = SupplierSyncStates.Added;
            } else if (string.IsNullOrWhiteSpace(supplierNumber) && string.IsNullOrWhiteSpace(existingSupplierNumber)) {                
                state = SupplierSyncStates.MappingRequired;
            } else if (string.IsNullOrWhiteSpace(supplierNumber) && !string.IsNullOrWhiteSpace(existingSupplierNumber)) {
                WebServiceHelper.UpdateSupplierContactInfo(supplier.Id, existingSupplierNumber);
                state = SupplierSyncStates.Mapped;
            }

            return state;
        }

        private void Insert(Supplier supplier)
        {
            var sql =
                $"INSERT INTO {TableName} (SupplierId, SupplierName, ContactName, Email, Phone, OurBillToNumber, OurSupplierNumber) " +
                $"Values ({supplier.Id}, {StringOrNull(supplier.Name)}, {StringOrNull(supplier.OurContactInfo?.ContactName)}, " +
                $"{StringOrNull(supplier.OurContactInfo?.Email)}, {StringOrNull(supplier.OurContactInfo?.Phone)}, " +
                $"{StringOrNull(supplier.OurContactInfo?.OurBillToNumber)}, {StringOrNull(supplier.OurContactInfo?.OurSupplierNumber)})";
            using (var command = new OdbcCommand(sql)) {
                ExecuteCommand(command);
            }
        }

        private static string StringOrNull(string value) => string.IsNullOrEmpty(value) ? "null" : $"'{value.Replace("'", "''").Replace("\"", "\\\"")}'";

        protected override Supplier PopulateRecord(dynamic reader)
        {
            try {

                return new Supplier {
                    Id = reader.Id,
                    Name = reader.Name,
                    OurContactInfo = new SupplierContact {
                        Id = reader.Id,
                        ContactName = reader.OurContactInfo?.ContactName,
                        Phone = reader.OurContactInfo?.Phone,
                        Email = reader.OurContactInfo?.Email,
                        OurBillToNumber = reader.OurContactInfo?.OurBillToNumber,
                        OurSupplierNumber = reader.OurContactInfo?.OurSupplierNumber
                    }
                };

            } catch (RuntimeBinderException exception) {
                Console.WriteLine(exception);
                throw new InvalidDataException("One of the fields in the source ODBC database has an invalid column type or value", exception);
            }
        }
    }

    public enum SupplierSyncStates
    {
        Added,
        Unchanged,
        MappingRequired,
        Mapped
    }
}