using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkGreen.Applications.Common.Model
{
    public class PricingLevelItemRequest
    {
        public string PriceLevelName { get; set; }

        public int SupplierInventoryItemId { get; set; }

        public decimal Price { get; set; }

        public int MinimumPurchase { get; set; }
    }
}
