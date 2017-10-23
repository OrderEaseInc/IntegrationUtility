namespace LinkGreen.Applications.Common.Model
{
    public class Relationship
    {
        public int Id { get; set; }

        public string OurCompanyNumber { get; set; }

        public string OurBillToNumber { get; set; }

        public string ContactName { get; set; }

        public string ContactPhone { get; set; }

        public string ContactEmail { get; set; }

        public string SerializedTaxInfo { get; set; }
    }
}
