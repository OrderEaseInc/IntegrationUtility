using System;

namespace LinkGreen.Applications.Common.Model
{
    public class OrderFromLinkGreen
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ShippingDate { get; set; }
        public DateTime? RequestedShippingDate { get; set; }
        public DateTime? AnticipatedShipDate { get; set; }
        public string Status { get; set; }
        public string SupplierStatus { get; set; }
        public string PaymentTerm { get; set; }
        public int? PaymentTermId { get; set; }
        public int? SupplierStatusId { get; set; }
        public int? BuyerStatusId { get; set; }
        public string BuyerComment { get; set; }
        public decimal? Freight { get; set; }
        public string SupplierPO { get; set; }
        public string BuyerPO { get; set; }
        public string OrderNumber { get; set; }
        public string ContactName { get; set; }
        public bool IsDirectDelivery { get; set; }
        public bool SupplierCanExport { get; set; }
        public int? SupplierCompanyId { get; set; }
        public int? BuyerCompanyId { get; set; }
        public string BuyerCompanyName { get; set; }        
        public string OurCompanyNumber { get; set; }
        public string OurBillToNumber { get; set; }
        public string Name { get; set; }
        public bool UseAlternateAddress { get; set; }
        public string AlternateProvince { get; set; }
        public string AlternateReceiverName { get; set; }
        public string AlternateAddress { get; set; }
        public string AlternateCity { get; set; }
        public string AlternatePostalCode { get; set; }
        public string AlternatePhone { get; set; }
        public string AlternateSpecialInstructions { get; set; }

        public string ConsolidatedNote { get; set; }
    }
}