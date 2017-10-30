namespace LinkGreen.Applications.Common.Model
{
    public class BuyerInventory
    {
        public int? Id { get; set; }
        public int? CategoryId { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string PrivateSku { get; set; }
        public int? LocationId { get; set; }
        public string UPC { get; set; }
        public int? MinOrderSpring { get; set; }
        public int? MinOrderSummer { get; set; }
        public decimal? FreightFactor { get; set; }
        public int? QuantityAvailable { get; set; }
        public string Comments { get; set; }
        public decimal? SuggestedRetailPrice { get; set; }
        public string OpenSizeDescription { get; set; }
        public decimal? NetPrice { get; set; }
        public int? SlaveQuantityPerMaster { get; set; }
        public string SlaveQuantityDescription { get; set; }
        public string MasterQuantityDescription { get; set; }
        public bool? Inactive { get; set; }
        public decimal? RetailPrice { get; set; }
        public int? RetailOrderLevel { get; set; }
        public bool? AmazonSell { get; set; }
        public bool? OnlineSell { get; set; }
        public int? SupplierId { get; set; }
        public string SupplierSku { get; set; }
    }
}