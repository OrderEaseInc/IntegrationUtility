using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace LinkGreen.Applications.Common.Model
{
    public class ProductImportRequest
    {
        public ProductImportConfig ImportConfig { get; set; }
        public List<SupplierInventoryItemImport> Products { get; set; }
    }


    public class ProductImportConfig
    {
        public List<ProductOptions> ProductImportOptions { get; set; } = new List<ProductOptions>();
        public List<string> MappedColumns { get; set; } = new List<string>();
        public List<string> IgnoreEmptyValuesColumns { get; set; } = new List<string>();
        public int UserId { get; set; }
        public string FileName { get; set; }
        public bool SendEmailNotificationOnError { get; set; }
        public Guid IntegrationKey { get; set; }
    }

    public class SupplierInventoryItemImport : SupplierInventoryItem
    {
        public string CategoryName { get; set; }
        public string LocationName { get; set; }
        public string PrivateLabelGroupName { get; set; }
        public string ImageUrl { get; set; }
        public string CatalogIntegrationReference { get; set; }
        public List<string> CatalogIntegrationReferences { get; set; } = new List<string>();
        public List<ProductFeatureItem> ProductFeatureItems { get; set; } = new List<ProductFeatureItem>();
        public List<ImportImage> Images { get; set; } = new List<ImportImage>();

        public string NewProduct { get; set; }
        public string Description_FR { get; set; }
        public string OpenSizeDescription_FR { get; set; }
        public string Comments_FR { get; set; }
        public string SlaveQuantityDescription_FR { get; set; }
        public string MasterQuantityDescription_FR { get; set; }
        public string FamilyExternalReference { get; set; }

        //Overriden properties.
        public new decimal? NetPrice { get; set; }
        public new int? MinOrderSummer { get; set; }
        public new double? ProdWeight { get; set; }
        public new double? ProdHeight { get; set; }
        public new double? ProdWidth { get; set; }
        public new double? ProdLength { get; set; }
        public new bool? NetPriceLocked { get; set; }
        public new decimal? RetailSalePrice { get; set; }
        public new Single? QuantityAvailable { get; set; }
        public new FeatureLevel? FeatureLevel { get; set; }
        public new bool? Inactive { get; set; }
        public new int? OrderMultiple { get; set; }

        public new double? CaseWeight { get; set; }
        public new double? CaseHeight { get; set; }
        public new double? CaseWidth { get; set; }
        public new double? CaseLength { get; set; }

        public new double? PackWeight { get; set; }
        public new double? PackHeight { get; set; }
        public new double? PackWidth { get; set; }
        public new double? PackLength { get; set; }
        public decimal? RetailPrice { get; set; }
        public new Single? QtyAvailableEachesDecimal { get; set; }


        /// <summary>
        /// Partial update of <see cref="SupplierInventoryPackSize"/>
        /// </summary>
        public List<JObject> PackSizes { get; set; } = new List<JObject>();
    }


    public class ProductFeatureItem
    {
        public string FeatureId { get; set; }
        public string FeatureGroupName { get; set; }
        public string Value_EN { get; set; }
        public string Value_FR { get; set; }
    }

    public class ImportImage
    {
        public string Url { get; set; }
        public bool IsPrimary { get; set; }
    }

    public enum ProductImportOptions
    {
        CreateCategoryIfNotExists,
        ManageInventory,
        CatalogAssignment
    }

    public class ProductOptions
    {
        public ProductImportOptions Name { get; set; }
        public dynamic Value { get; set; }
        public bool Selected { get; set; }
        public string Text { get; set; }
    }
}
