namespace LinkGreen.Applications.Common.Model
{
    public class InventoryItemRequest
    {
        public int Id { get; set; }
        
        public int? CategoryId { get; set; }
        
        public int? LocationId { get; set; }
        
        public string PrivateSKU { get; set; }
        
        public string UPC { get; set; }
        
        public int MinOrderSpring { get; set; }
        
        public int MinOrderSummer { get; set; }
        
        public decimal FreightFactor { get; set; }
        
        public int QuantityAvailable { get; set; }
        
        public string Comments { get; set; }
        
        public decimal? SuggestedRetailPrice { get; set; }
        
        public string Description { get; set; }
        
        public string OpenSizeDescription { get; set; }
        
        public int DirectDeliveryMinQuantity { get; set; }
        
        public decimal NetPrice { get; set; }
        
        public int? SlaveQuantityPerMaster { get; set; }
        
        public string SlaveQuantityDescription { get; set; }
        
        public string MasterQuantityDescription { get; set; }
        
        public string DirectDeliveryCode { get; set; }
        
        public bool Inactive { get; set; }
        
        public bool IsDirectDelivery { get; set; }

        public string AccountingReference { get; set; }
    }
}
