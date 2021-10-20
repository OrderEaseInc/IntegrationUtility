using System.Data.OleDb;
using LinkGreen.Applications.Common;
using LinkGreen.Applications.Common.Model;

namespace DataTransfer.AccessDatabase
{
    public class OrdersFromLinkGreenRepository : OleDbRepository<OrderFromLinkGreen>
    {
        public const string TableName = "Orders_FromLinkGreen";
        public const string TableKey = "Id";
        public const string ItemsTableName = "OrderItem_FromLinkGreen";

        public OrdersFromLinkGreenRepository(string connectionString) : base(connectionString) { }

        public void ClearAll()
        {
            using (var command = new OleDbCommand($"DELETE * FROM {ItemsTableName}"))
            {
                ExecuteCommand(command);
            }
            using (var command = new OleDbCommand($"DELETE * FROM {TableName}"))
            {
                ExecuteCommand(command);
            }
        }

        public void Download(int[] status)
        {
            var orders = WebServiceHelper.GetOrdersForStatus(status);

            foreach (var order in orders)
            {
                var detail = WebServiceHelper.DownloadOrderDetails(order.Id);
                if (detail.BuyerCompany != null)
                {
                    order.OurCompanyNumber = detail.BuyerCompany.OurCompanyNumber;
                    order.OurBillToNumber = detail.BuyerCompany.OurBillToNumber;
                }

                if (!string.IsNullOrWhiteSpace(detail.ConsolidatedNote))
                    order.ConsolidatedNote = detail.ConsolidatedNote;

                Insert(order);

                foreach (var item in detail.Details)
                {
                    InsertItem(item);
                }
            }
        }

        public override void SaveFieldMapping(string fieldName, string mappingName)
        {
            using (var command = new OleDbCommand($"UPDATE `FieldMappings` SET `MappingName` = '{mappingName}' WHERE `FieldName` = '{fieldName}' AND `TableName` = '{TableName}'"))
            {
                ExecuteCommand(command);
            }
        }

        public void SaveItemFieldMapping(string fieldName, string mappingName)
        {
            using (var command = new OleDbCommand($"UPDATE `FieldMappings` SET `MappingName` = '{mappingName}' WHERE `FieldName` = '{fieldName}' AND `TableName` = '{ItemsTableName}'"))
            {
                ExecuteCommand(command);
            }
        }

        private void Insert(OrderFromLinkGreen order)
        {
            var sql =
                $"INSERT INTO {TableName} (Id, CreatedDate, ShippingDate, RequestedShippingDate, AnticipatedShipDate, " +
                "Status, SupplierStatus, PaymentTerm, PaymentTermId, SupplierStatusId, BuyerStatusId, BuyerComment, " +
                "Freight, SupplierPO, BuyerPO, OrderNumber, ContactName, IsDirectDelivery, SupplierCanExport, " +
                "SupplierCompanyId, BuyerCompanyId, BuyerCompanyName, OurCompanyNumber, OurBillToNumber, Name, " +
                "UseAlternateAddress, AlternateProvince, AlternateReceiverName, " +
                "AlternateAddress, AlternateCity, AlternatePostalCode, AlternatePhone, AlternateSpecialInstructions, ConsolidatedNote) " +
                $"VALUES ({order.Id}, {Date(order.CreatedDate)}, {NullableDate(order.ShippingDate)}, {NullableDate(order.RequestedShippingDate)}, " +
                $"{NullableDate(order.AnticipatedShipDate)}, {NullableString(order.Status)}, {NullableString(order.SupplierStatus)}, " +
                $"{NullableString(order.PaymentTerm)}, {NullableInt(order.PaymentTermId)}, {NullableInt(order.SupplierStatusId)}, " +
                $"{NullableInt(order.BuyerStatusId)}, {NullableString(order.BuyerComment)}, {NullableDecimal(order.Freight)}, {NullableString(order.SupplierPO)}, " +
                $"{NullableString(order.BuyerPO)}, {NullableString(order.OrderNumber)}, {NullableString(order.ContactName)}, {Boolean(order.IsDirectDelivery)}, " +
                $"{Boolean(order.SupplierCanExport)}, {NullableInt(order.SupplierCompanyId)}, {NullableInt(order.BuyerCompanyId)}, {NullableString(order.BuyerCompanyName)}, " +
                $"{NullableString(order.OurCompanyNumber)}, {NullableString(order.OurBillToNumber)}, {NullableString(order.Name)}, " +
                $"{Boolean(order.UseAlternateAddress)}, {NullableString(order.AlternateProvince)}, {NullableString(order.AlternateReceiverName)}, " +
                $"{NullableString(order.AlternateAddress)}, {NullableString(order.AlternateCity)}, {NullableString(order.AlternatePostalCode)}, " +
                $"{NullableString(order.AlternatePhone)}, {NullableString(order.AlternateSpecialInstructions)}, {NullableString(order.ConsolidatedNote)})";

            using (var command = new OleDbCommand(sql))
            {
                ExecuteCommand(command);
            }
        }

        private void InsertItem(OrderDetail item)
        {
            var sql =
                $"INSERT INTO {ItemsTableName} (OrderId, AddedBySupplier, AdditionalDiscount, CatalogPrice, CommonSKU, " +
                "Id, IsQtyLocked, ItemId, ItemIsDirectDelivery, ItemName, MasterQuantityDescription, OurNote, Price, PrivateSKU, " +
                "QuantityAvailable, QuantityConfirmed, QuantityRequested, QuantitySent, RelatedProductId, [Size], SlaveQuantityPerMaster, " +
                "SortOrder, SubstitutedById, SubstitutedForId, SupplierAccountingReference) " +
                $"VALUES ({item.OrderId}, {Boolean(item.AddedBySupplier)}, {NullableDecimal(item.AdditionalDiscount)}, {NullableDecimal(item.CatalogPrice)}, " +
                $"{NullableString(item.CommonSKU)}, {item.Id}, {Boolean(item.IsQtyLocked)}, {item.ItemId}, {Boolean(item.ItemIsDirectDelivery)}, " +
                $"{NullableString(item.ItemName)}, {NullableString(item.MasterQuantityDescription)}, {NullableString(item.OurNote)}, {NullableDecimal(item.Price)}, " +
                $"{NullableString(item.PrivateSKU)}, {NullableInt(item.QuantityAvailable)}, {NullableInt(item.QuantityConfirmed)}, {NullableInt(item.QuantityRequested)}, " +
                $"{NullableInt(item.QuantitySent)}, {NullableInt(item.RelatedProductId)}, {NullableString(item.Size)}, {NullableInt(item.SlaveQuantityPerMaster)}, " +
                $"{NullableInt(item.SortOrder)}, {NullableInt(item.SubstitutedById)}, {NullableInt(item.SubstitutedForId)}, {NullableString(item.SupplierAccountingReference)})";
            using (var command = new OleDbCommand(sql))
            {
                ExecuteCommand(command);
            }
        }
    }
}