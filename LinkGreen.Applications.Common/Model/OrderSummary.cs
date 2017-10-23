using System;

namespace LinkGreen.Applications.Common.Model
{
    public class OrderSummary
    {
        public int Id { get; set; }

        public int RelationshipId { get; set; }

        public DateTime CreatedDate { get; set; }

        public string BuyerCompanyName { get; set; }

        public string SupplierCompanyName { get; set; }

        public int SupplierCompanyId { get; set; }

        public int BuyerCompanyId { get; set; }

        public DateTime? ShippingDate { get; set; }

        public DateTime? RequestedShippingDate { get; set; }

        public DateTime? AnticipatedShipDate { get; set; }

        public int? PaymentTermId { get; set; }

        public int SupplierStatusId { get; set; }

        public int BuyerStatusId { get; set; }

        public string SupplierPO { get; set; }

        public string BuyerPO { get; set; }

        public string OrderNumber { get; set; }

        public bool IsDirectDelivery { get; set; }

    }
}
