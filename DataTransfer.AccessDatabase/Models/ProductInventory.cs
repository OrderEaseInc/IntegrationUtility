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
        public string Group { get; set; }
        public string Category { get; set; }
        public string Description1 { get; set; }
        public string Description2 { get; set; }
        public string Description3 { get; set; }
        public string Color { get; set; }
        public int HalfCaseQty { get; set; }
        public int FullCaseQty { get; set; }
        public string Sell { get; set; }
        public decimal NetPrice { get; set; }
        public bool Active { get; set; }
        public string CaseBoxName { get; set; }
        public string HalfCaseBoxName { get; set; }
    }

}
