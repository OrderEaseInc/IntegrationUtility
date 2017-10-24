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
    public class PriceLevel
    {
        public string Name { get; set; }

        public string ExternalReference { get; set; }

        public DateTime? EffectiveDate { get; set; }

        public DateTime? EndDate { get; set; }
    }

}
