using System.Collections.Generic;

namespace LinkGreen.Applications.Common.Model
{
    public class InventoryItemRequest
    {

        public int Id { get; set; }

        public int? CategoryId { get; set; }

        public int? LocationId { get; set; }

        public string PrivateSKU { get; set; }

        public string UPC { get; set; }

        public string CaseUPC { get; set; }

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

        public bool NetPriceLocked { get; set; }

        public int? SlaveQuantityPerMaster { get; set; }

        public int? ShippingUnits { get; set; }

        public string SlaveQuantityDescription { get; set; }

        public string MasterQuantityDescription { get; set; }

        public string DirectDeliveryCode { get; set; }

        public bool Inactive { get; set; }

        public bool IsDirectDelivery { get; set; }

        public string AccountingReference { get; set; }

        public decimal RetailPrice { get; set; }

        public decimal RetailSalePrice { get; set; }

        public bool RetailSell { get; set; }

        public int RetailOrderLevel { get; set; }

        public bool AmazonSell { get; set; }

        public bool ShopifySell { get; set; }

        public long? ShopifyId { get; set; }

        public bool DropShipSell { get; set; }

        public decimal? DropShipPrice { get; set; }
        // ReSharper disable once InconsistentNaming

        public decimal? MAPPrice { get; set; }
        // ReSharper disable once InconsistentNaming

        public int MAPSupplierInventoryId { get; set; }

        public ICollection<ProductFeatureRequest> ProductFeatures { get; set; }

        public int? PrivateLabelGroupId { get; set; }

        public string UpstreamSKU { get; set; }

        public double ProdHeight { get; set; }

        public double ProdLength { get; set; }

        public double ProdWeight { get; set; }

        public double ProdWidth { get; set; }

        public bool Taxable { get; set; }

        public FulfillmentMethod FulfillmentMethod { get; set; }

        public RetailPriceSource RetailPriceSource { get; set; }
    }
}
