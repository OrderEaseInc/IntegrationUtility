﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinkGreen.Applications.Common.Model
{

    public class SupplierInventoryItem
    {
        public bool HasImage { get; set; }
        public int CompanyId { get; set; }
        public int? CategoryId { get; set; }
        public int? LocationId { get; set; }
        public string PrivateSKU { get; set; }
        public int MinOrderSpring { get; set; }
        public int MinOrderSummer { get; set; }
        public decimal FreightFactor { get; set; }
        public Single QuantityAvailable { get; private set; }
        private Single _qtyAvailableEachesDecimal;
        public Single QtyAvailableEachesDecimal
        {
            get => _qtyAvailableEachesDecimal;
            set
            {
                QuantityAvailable = value / (float)(SlaveQuantityPerMaster ?? 1);
                _qtyAvailableEachesDecimal = value;
            }
        }
        public string Comments { get; set; }
        public decimal? SuggestedRetailPrice { get; set; }
        public string Description { get; set; }
        public int DirectDeliveryMinQuantity { get; set; }
        public decimal NetPrice { get; set; }
        public decimal? SlaveQuantityPerMaster { get; set; }
        public string SlaveQuantityDescription { get; set; }
        public string MasterQuantityDescription { get; set; }
        public string DirectDeliveryCode { get; set; }
        public int? PrivateLabelGroupId { get; set; }
        public bool IsPlant { get; set; }
        public int? SizeId { get; set; }
        public int? FormatId { get; set; }
        public int? GradeId { get; set; }
        public int? CultivarId { get; set; }
        public string CultivarName { get; set; }
        public int? ShapeId { get; set; }
        public string CommonSKU { get; set; }
        public bool IsPrivateLabel { get; set; }
        public string UPC { get; set; }
        public string CaseUPC { get; set; }
        public int? LinkedToId { get; set; }
        public string OpenSizeDescription { get; set; }
        public FeatureLevel FeatureLevel { get; set; }
        public bool Inactive { get; set; }
        public bool IsDirectDelivery { get; set; }
        public string ImageFilenameOrUrl { get; set; }
        public string AccountingReference { get; set; }
        public string DescriptionAlpha { get; set; }
        public string DescriptionAlphaFr { get; set; }
        public bool IsAutoGeneratedProduct { get; set; }
        public string UpstreamSKU { get; set; }
        public double ProdWeight { get; set; }
        public double ProdHeight { get; set; }
        public double ProdWidth { get; set; }
        public double ProdLength { get; set; }
        public decimal RetailPrice { get; set; }
        public decimal RetailSalePrice { get; set; }
        public bool RetailSell { get; set; }
        public int RetailOrderLevel { get; set; }
        public bool AmazonSell { get; set; }
        public bool NetPriceLocked { get; set; }
        public DateTime? GoodUntil { get; set; }
        public int? ShippingUnits { get; set; }
        public bool ShopifySell { get; set; }
        public long? ShopifyId { get; set; }
        public bool DropShipSell { get; set; }
        public decimal? DropShipPrice { get; set; }
        public decimal? Mapprice { get; set; }
        public int MapsupplierInventoryId { get; set; }
        public int? PrimaryProductMediaId { get; set; }

        public FulfillmentMethod FulfillmentMethod { get; set; }

        public RetailPriceSource RetailPriceSource { get; set; }

        public bool Taxable { get; set; }

        public string ExternalReference { get; set; }

        public string ExternalSource { get; set; }

        /// <summary>
        /// Markup for reselling products
        /// </summary>
        public decimal RetailMarkup { get; set; }

        public PrivateCategory Category { get; set; }

        [NotMapped]
        public IList<string> VendorNames { get; set; }

        [MaxLength(50)]
        public string UnitOfMeasureWeight { get; set; }

        [MaxLength(50)]
        public string UnitOfMeasureDimension { get; set; }

        public double CaseWeight { get; set; }
        public double CaseHeight { get; set; }
        public double CaseWidth { get; set; }
        public double CaseLength { get; set; }
        [MaxLength(50)]
        public string CaseUnitOfMeasureWeight { get; set; }
        [MaxLength(50)]
        public string CaseUnitOfMeasureDimension { get; set; }

        public double PackWeight { get; set; }
        public double PackHeight { get; set; }
        public double PackWidth { get; set; }
        public double PackLength { get; set; }

        [MaxLength(50)]
        public string PackUnitOfMeasureWeight { get; set; }
        [MaxLength(50)]
        public string PackUnitOfMeasureDimension { get; set; }
    }

    public enum FeatureLevel
    {
        NotSpecified = 0,
        [Description("Top Seller")]
        TopSeller = 1,
        [Description("New Product")]
        NewProduct = 2,
        [Description("Featured Product")]
        FeaturedProduct = 3,
        [Description("On Promotion")]
        OnPromotion = 4,
        [Description("Hot Buys")]
        HotBuys = 5
    }
}
