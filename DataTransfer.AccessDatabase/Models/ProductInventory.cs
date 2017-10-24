using LinkGreen.Applications.Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransfer.AccessDatabase
{
    /// <summary>
    /// Gets turned into InventoryItemRequest for API push to LinkGreen app
    /// </summary>
    public class ProductInventory
    {
        public string Id { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Comments { get; set; }
        public decimal NetPrice { get; set; }
        public bool Active { get; set; }
        public string PrivateSKU { get; set; }
        public string UPC { get; set; }
        public int MinOrderSpring { get; set; }
        public int MinOrderSummer { get; set; }
        public decimal FreightFactor { get; set; }
        public int QuantityAvailable { get; set; }
        public decimal SuggestedRetailPrice { get; set; }
        public string OpenSizeDescription { get; set; }
        public int DirectDeliveryMinQuantity { get; set; }
        public int SlaveQuantityPerMaster { get; set; }
        public string SlaveQuantityDescription { get; set; }
        public string MasterQuantityDescription { get; set; }
        public string DirectDeliveryCode { get; set; }
        public bool IsDirectDelivery { get; set; }
    }

}
