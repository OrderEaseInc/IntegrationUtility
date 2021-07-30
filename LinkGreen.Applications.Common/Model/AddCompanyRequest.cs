using System.Runtime.Serialization;

namespace LinkGreen.Applications.Common.Model
{
    [DataContract]
    public class AddCompanyRequest
    {
        [DataMember]
        public CompanyViewModel Company { get; set; }

        [DataMember]
        public string ContactName { get; set; }

        [DataMember]
        public string OurCompanyNumber { get; set; }

        [DataMember]
        public string OurBillToNumber { get; set; }

        [DataMember]
        public string SalesRepEmail { get; set; }

        [DataMember]
        public bool? ReplaceSalesRep { get; set; }
    }
}