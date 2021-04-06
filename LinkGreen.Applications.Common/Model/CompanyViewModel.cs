using System;
using System.Runtime.Serialization;
using System.Text;

namespace LinkGreen.Applications.Common.Model
{
    public class CompanyViewModel
    {
        [DataMember]
        public virtual int CompanyTypeId { get; set; }

        [DataMember]
        public virtual int? IndustryTypeId { get; set; }

        [DataMember]
        public virtual string Name { get; set; }

        public virtual string CommonName { get; set; }

        [DataMember]
        public string Address1 { get; set; }

        [DataMember]
        public string Address2 { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public virtual string ProvState { get; set; }

        [DataMember]
        public string PostalCode { get; set; }

        [DataMember]
        public string Country { get; set; }



        [DataMember]
        public string BillToAddress1 { get; set; }

        [DataMember]
        public string BillToAddress2 { get; set; }

        [DataMember]
        public string BillToCity { get; set; }

        [DataMember]
        public virtual string BillToProvState { get; set; }

        [DataMember]
        public string BillToPostalCode { get; set; }

        [DataMember]
        public string BillToCountry { get; set; }


        [DataMember]
        public string FormattedPhone1 { get; set; }

        [DataMember]
        public string FormattedPhone2 { get; set; }


        [DataMember]
        public string Phone1Desc { get; set; }


        [DataMember]
        public string Phone2Desc { get; set; }

        [DataMember(IsRequired = true)]
        public string Email1 { get; set; }


        [DataMember]
        public string Email2 { get; set; }

        [DataMember]
        public string Email1Desc { get; set; }


        [DataMember]
        public string Email2Desc { get; set; }

        [DataMember]
        public string Contact1 { get; set; }

        [DataMember]
        public string Contact2 { get; set; }


        [DataMember]
        public string Contact1Desc { get; set; }

        [DataMember]
        public string Contact2Desc { get; set; }


        [DataMember]
        public string Web { get; set; }

        [DataMember]
        public string GeneralDescription { get; set; }

        [DataMember]
        public string ProductsAndSpecialties { get; set; }

        [DataMember]
        public string DeliveryAreaFeesAndMethod { get; set; }

        [DataMember]
        public string CompanyType { get; set; }


        [DataMember]
        public string FullPhone1 { get; set; }

        [DataMember]
        public string FullEmail { get; set; }

        [DataMember]
        public bool OffersDropShip { get; set; }


        [DataMember]
        public bool IsBuyer { get; set; }

        [DataMember]
        public bool IsSupplier { get; set; }

        [DataMember]
        public int? SignupSource { get; set; }


        [DataMember]
        public int? SellerPlanId { get; set; }


        [DataMember]
        public bool SellsToPublic { get; set; }

        [DataMember]
        public bool SellsToTrade { get; set; }

        [DataMember]
        public bool SellsToRetailers { get; set; }

        [DataMember]
        public bool SellsToDistributors { get; set; }

        [DataMember]
        public int InviteLimit { get; set; }

        [DataMember]
        public int HoldingId { get; set; }

        [DataMember]
        public int ShowSetupMenu { get; set; }

        [DataMember]
        public string LastUpdatedSource { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var nl = Environment.NewLine;
            var sb = new StringBuilder();

            sb.Append($"Name: {Name} {nl}");
            sb.Append($"Address1: {Address1} {nl}");
            sb.Append($"Address2: {Address2} {nl}");
            sb.Append($"City: {City} {nl}");
            sb.Append($"Prov/State: {ProvState} {nl}");
            sb.Append($"Phone: {FormattedPhone1} {nl}");
            sb.Append($"Email: {Email1} {nl}");
            sb.Append($"Website: {Web} {nl}");

            sb.Append(nl);

            return sb.ToString();
        }
    }
}