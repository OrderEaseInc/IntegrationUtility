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
    public class ProductPrice : OdbcDataTransferObjectBase
    {
        public string IDNumber { get; set; }
        public string Group { get; set; }
        public string Territory { get; set; }
        public decimal? CaseSellPrice { get; set; }
        public decimal? HalfCaseSellPrice { get; set; }
        public decimal? SingleSellPrice { get; set; }
    }

    public class ProductCategory : OdbcDataTransferObjectBase
    {
        public string Group { get; set; }
        public string Category { get; set; }
    }

    public class Customer : OdbcDataTransferObjectBase
    {
        public string CompanyName { get; set; }
        public string ContactFirst { get; set; }
        public string ContactLast { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string ProvState { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public string ContactPhone { get; set; }
        public string CompanyNumber { get; set; }
        public string BillToNumber { get; set; }
        public string Email { get; set; }
        public string BuyerGroup { get; set; }

        public string BillAddress { get; set; }
        public string BillCity { get; set; }
        public string BillCountry { get; set; }
        public string BillProvState { get; set; }
        public string BillPostalCode { get; set; }
    }
}
