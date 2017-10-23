namespace LinkGreen.Applications.Common.Model
{
    public class OrderDetail
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public string ItemName { get; set; }

        public bool HasImage { get; set; }

        public decimal SortOrder { get; set; }

        public string Size { get; set; }

        public string PrivateSKU { get; set; }

        public string CommonSKU { get; set; }

        public int ItemId { get; set; }

        public int QuantityRequested { get; set; }

        public int? QuantityConfirmed { get; set; }

        public int? QuantitySent { get; set; }

        public int? RelatedProductId { get; set; }

        public bool BackOrder { get; set; }

        public string HowPrice { get; set; }

        public decimal Price { get; set; }

        public decimal CatalogPrice { get; set; }

        public decimal AdditionalDiscount { get; set; }

        public int QuantityAvailable { get; set; }

        public bool AddedBySupplier { get; set; }

        public string FormattedPrice => Price.ToString("C");

        public string FormattedCatalogPrice => CatalogPrice.ToString("C");

        public string FormattedBackOrder => BackOrder ? "Yes" : "No";

        public int AdjustedSlaveQuantityPerMaster => SlaveQuantityPerMaster == null || SlaveQuantityPerMaster == 0 ? 1 : SlaveQuantityPerMaster.Value;

        public int QuantityToUse => QuantitySent ?? QuantityConfirmed ?? QuantityRequested;

        public int ExtendedQuantity => QuantityToUse * AdjustedSlaveQuantityPerMaster;
        
        public decimal Amount => UnitsToUse * DiscountedPrice;

        public decimal LineTotal => Amount;

        public decimal DiscountedPrice => Price - AdditionalDiscount;

        public int UnitsToUse => QuantityToUse * Units;

        public int Units => SlaveQuantityPerMaster.HasValue && SlaveQuantityPerMaster.Value > 1
            ? SlaveQuantityPerMaster.GetValueOrDefault()
            : 1;

        public int? SubstitutedById { get; set; }

        public int? SubstitutedForId { get; set; }

        public int? SlaveQuantityPerMaster { get; set; }

        public string MasterQuantityDescription { get; set; }

        public bool ItemIsDirectDelivery { get; set; }

        public string OurNote { get; set; }

        public string SupplierAccountingReference { get; set; }
    }
}
