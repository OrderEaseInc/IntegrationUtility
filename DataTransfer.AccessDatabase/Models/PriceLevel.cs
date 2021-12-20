using System;

namespace DataTransfer.AccessDatabase
{
    /// <summary>
    /// Gets turned into InventoryItemRequest for API push to LinkGreen app
    /// </summary>
    public class PriceLevel
    {
        public string Name { get; set; }

        public string ExternalReference { get; set; }

        public DateTime? EffectiveDate { get; set; }

        public DateTime? EndDate { get; set; }
    }

}
