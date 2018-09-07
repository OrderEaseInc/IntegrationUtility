using System;
using System.Text;

namespace LinkGreen.Applications.Common.Model
{
    public class Company 
    {
        public Company()
        {
            Initialize();
        }

        private void Initialize()
        {
        }

        public virtual int Id { get; set; }

        public virtual int CompanyTypeId { get; set; }

        public virtual string Name { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public virtual string ProvState { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        public string FormattedPhone1 { get; set; }

        public string FormattedPhone2 { get; set; }

        public string Phone1Desc { get; set; }

        public string Phone2Desc { get; set; }

        public string Email1 { get; set; }

        public string Email2 { get; set; }

        public string Email1Desc { get; set; }

        public string Email2Desc { get; set; }

        public string Contact1 { get; set; }

        public string Contact2 { get; set; }

        public string Contact1Desc { get; set; }

        public string Contact2Desc { get; set; }

        public string Web { get; set; }

        public string GeneralDescription { get; set; }

        public string ProductsAndSpecialties { get; set; }

        public string DeliveryAreaFeesAndMethod { get; set; }

        public string CompanyType { get; set; }

        public string OurCompanyNumber { get; set; }
        
        public string OurBillToNumber { get; set; }

        public string FullPhone1
        {
            get
            {
                return $"{(string.IsNullOrEmpty(Phone1Desc) ? "Phone" : Phone1Desc)}: {FormattedPhone1}";
            }
            set { }
        }

        public string FullEmail
        {
            get
            {
                return $"{(string.IsNullOrEmpty(Email1Desc) ? "Email" : Email1Desc)}: {Email1}";
            }
            set { }
        }


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

            sb.Append("Name: " + Name + nl);
            sb.Append("Address 1: " + Address1 + nl);
            sb.Append("Address 2: " + Address2 + nl);
            sb.Append("City: " + City + nl);
            sb.Append("Province: " + ProvState + nl);
            sb.Append("Phone: " + FormattedPhone1 + nl);
            sb.Append("Email Address: " + Email1 + nl);
            sb.Append("Website: " + Web + nl);

            sb.Append(nl);

            return sb.ToString();
        }


    }
}
