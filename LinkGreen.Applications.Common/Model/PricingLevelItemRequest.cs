namespace LinkGreen.Applications.Common.Model
{
    public class PricingLevelItemRequest
    {
        public int SupplierInventoryItemId { get; set; }

        public decimal Price { get; set; }

        public int MinimumPurchase { get; set; }
    }
}
