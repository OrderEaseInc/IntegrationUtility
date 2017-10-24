using System.Collections.Generic;
using System.Data.Odbc;
using LinkGreen.Applications.Common;
using LinkGreen.Applications.Common.Model;

namespace DataTransfer.AccessDatabase
{
    public class BuyerRepository : AdoRepository<Buyer>
    {
        private const string TableName = "Buyers";
        private readonly string _connectionString;

        public BuyerRepository(string connectionString) : base(connectionString)
        {
            _connectionString = connectionString;
        }

        public Dictionary<BuyerSyncStates, int> SyncAllBuyers()
        {
            var result = new Dictionary<BuyerSyncStates, int> {
                { BuyerSyncStates.Added, 0 },
                { BuyerSyncStates.Unchanged, 0 },
                { BuyerSyncStates.Mapped, 0 },
                { BuyerSyncStates.MappingRequired, 0 }
            };
            var buyers = GetAll();
            var querier = GetQuerier();
            foreach (var buyer in buyers) {
                var state = Sync(querier, buyer);
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

        private IEnumerable<Buyer> GetAll()
        {
            var buyers = WebServiceHelper.GetAllBuyers();
            return buyers;
        }

        private BuyerQuerier GetQuerier()
        {
            string dsnName;
            string tableName;
            var fields = new Dictionary<string, string>();

            using (var connection = new OdbcConnection(_connectionString)) {
                connection.Open();
                using (var command = new OdbcCommand($"SELECT FieldName,MappingName FROM FieldMappings WHERE TableName = '{TableName}'", connection)) {
                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            var fieldName = reader["FieldName"].ToString();
                            var mappingName = reader["MappingName"].ToString();
                            fields.Add(fieldName, mappingName);
                        }
                    }
                }
                using (var command = new OdbcCommand($"SELECT DsnName,MappingName FROM TableMappings WHERE TableName = '{TableName}'", connection)) {
                    using (var reader = command.ExecuteReader()) {
                        reader.Read();
                        dsnName = reader["DsnName"].ToString();
                        tableName = reader["MappingName"].ToString();
                    }
                }
            }

            var querier = new BuyerQuerier(dsnName, tableName, fields);
            return querier;
        }

        private BuyerSyncStates Sync(BuyerQuerier querier, Buyer buyer)
        {
            var supplierNumber = buyer.OurContactInfo?.OurSupplierNumber;
            var state = BuyerSyncStates.Unchanged;
            var existingBuyer = querier.Query(buyer.Id);
            var existingSupplierNumber = existingBuyer?.OurContactInfo?.OurSupplierNumber;
            if (existingBuyer == null) {
                querier.Insert(buyer);
                state = BuyerSyncStates.Added;
            } else if (string.IsNullOrWhiteSpace(supplierNumber) && string.IsNullOrWhiteSpace(existingSupplierNumber)) {                
                state = BuyerSyncStates.MappingRequired;
            } else if (string.IsNullOrWhiteSpace(supplierNumber) && !string.IsNullOrWhiteSpace(existingSupplierNumber)) {
                WebServiceHelper.UpdateBuyerWithReference(buyer.Id, existingSupplierNumber);
                state = BuyerSyncStates.Mapped;
            }

            return state;
        }
    }

    public enum BuyerSyncStates
    {
        Added,
        Unchanged,
        MappingRequired,
        Mapped
    }

    public class BuyerQuerier
    {
        private readonly string _dsnName;
        private readonly string _tableName;
        private readonly string _buyerIdField;
        private readonly string _buyerNameField;
        private readonly string _buyerReferenceField;

        public BuyerQuerier(string dsnName, string tableName, Dictionary<string, string> fields)
        {
            _dsnName = dsnName;
            _tableName = tableName;
            _buyerIdField = fields["BuyerId"];
            _buyerNameField = fields["Name"];
            _buyerReferenceField = fields["BuyerReference"];
        }

        public Buyer Query(int buyerId)
        {
            var sql = $"SELECT {_buyerNameField}, {_buyerReferenceField} FROM {_tableName} WHERE {_buyerIdField} = {buyerId}";
            using (var connection = new OdbcConnection($"DSN={_dsnName}")) {
                connection.Open();
                using (var command = new OdbcCommand(sql, connection)) {
                    using (var reader = command.ExecuteReader()) {
                        if (!reader.Read()) return null;

                        var buyerName = reader[0].ToString();
                        var buyerReference = reader[1].ToString();
                        var buyer = new Buyer {
                            Id = buyerId, Name = buyerName,
                            OurContactInfo = new BuyerContact {
                                OurSupplierNumber = buyerReference
                            }
                        };
                        return buyer;
                    }
                }
            }
        }

        public void Insert(Buyer buyer)
        {
            var supplierNumber = buyer.OurContactInfo?.OurSupplierNumber;
            var buyerReference = string.IsNullOrWhiteSpace(supplierNumber) ? "null" : $"'{supplierNumber}'";
            var sql = $"INSERT INTO {_tableName} ({_buyerIdField}, {_buyerNameField}, {_buyerReferenceField}) VALUES ({buyer.Id}, '{buyer.Name}', {buyerReference})";
            using (var connection = new OdbcConnection($"DSN={_dsnName}")) {
                connection.Open();
                using (var command = new OdbcCommand(sql, connection)) {
                    command.ExecuteNonQuery();                    
                }
            }
        }
    }    
}