namespace LinkGreen.Applications.Common.Model
{
    public class CompanyAndRelationshipResult
    {
       
        public int RelationshipId { get; set; }

       
        public int CompanyId { get; set; }

       
        public string ContactName { get; set; }

       
        public string ContactPhone { get; set; }

       
        public string ContactEmail { get; set; }

       
        public string OurCompanyNumber { get; set; }

       
        public string OurBillToNumber { get; set; }

       
        public string SerializedTaxInfo { get; set; }

       
        public virtual string Name { get; set; }

       
        public string Address1 { get; set; }

       
        public string Address2 { get; set; }

       
        public string City { get; set; }

       
        public virtual string ProvState { get; set; }

       
        public string PostalCode { get; set; }

       
        public string Country { get; set; }

       
        public string FormattedPhone1 { get; set; }

       
        public string FormattedPhone2 { get; set; }

       
        public string Email1 { get; set; }

       
        public string Email2 { get; set; }

       
        public string Contact1 { get; set; }

       
        public string Contact2 { get; set; }

       
        public string Web { get; set; }

        public string BuyerGroup { get; set; }
    }
}
