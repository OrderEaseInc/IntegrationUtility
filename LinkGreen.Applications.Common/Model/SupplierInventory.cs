using System.Collections.Generic;

namespace LinkGreen.Applications.Common.Model
{
    public class SupplierInventory
    {
        public string BuyerLinkedSku { get; set; }
        public decimal? CatalogPrice { get; set; }
        public string Description { get; set; }
        public int? Inventory { get; set; }
        public int ItemId { get; set; }
        public string SizeDescription { get; set; }
        public ICollection<QuantityPricingBreak> Pricing { get; set; }
        public string SupplierSku { get; set; }
    }
}