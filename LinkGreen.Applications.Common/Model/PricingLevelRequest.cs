using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkGreen.Applications.Common.Model
{
    public class PricingLevelRequest
    {
        public string Name { get; set; }

        public string ExternalReference { get; set; }

        public IList<PricingLevelItemRequest> InventoryItems { get; set; }

        public DateTime? EffectiveDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
