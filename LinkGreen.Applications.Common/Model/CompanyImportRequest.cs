using System;
using System.Collections.Generic;

namespace LinkGreen.Applications.Common.Model
{

    public class CompanyImportRequest
    {
        public CompanyImportConfig ImportConfig { get; set; }
        public List<CompanyItemImport> Companies { get; set; }
    }

    public class CompanyImportConfig
    {
        public UserMode ForceMode { get; set; } = UserMode.Buyer;
        public List<string> MappedColumns { get; set; }
        public List<string> IgnoreEmptyValuesColumns { get; set; }
        public List<CompanySetupOption> CompanySetupOptions { get; set; } = new List<CompanySetupOption>();
        public int? ConsortiumForCompanyMatching { get; set; }
        public int UserId { get; set; }
        public string FileName { get; set; }
        public bool SendEmailNotificationOnError { get; set; }
        public Guid IntegrationKey { get; set; }
    }

    public class CompanyItemImport
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string FormattedPhone1 { get; set; }
        public string Email1 { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string ProvState { get; set; }
        public string PostalCode { get; set; }
        public string Web { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string BuyerGroupOne { get; set; }
        public string BuyerGroupTwo { get; set; }
        public string BuyerGroupThree { get; set; }
        public string SalesRep { get; set; }
        public string OurCompanyNumber { get; set; }
        public string OurBillToNumber { get; set; }
        public string UserDefinedField1 { get; set; }
        public string UserDefinedField2 { get; set; }
        public string UserDefinedField3 { get; set; }
        public string UserDefinedField4 { get; set; }
        public bool SendEmails { get; set; }
        public string GeocachingLocationId { get; set; }
        public int? ContactUserId { get; set; }
        public string ReasonCode { get; set; }
        public string BillToAddress1 { get; set; }
        public string BillToAddress2 { get; set; }
        public string BillToAddressCity { get; set; }
        public string BillToAddressProvState { get; set; }
        public string BillToAddressPostalCode { get; set; }
        public string BillToAddressCountry { get; set; }
        public string ExternalReference1 { get; set; }
        public string ExternalReference2 { get; set; }
        public string ExternalSource { get; set; }
        public string PaymentTerm { get; set; }
        public List<BuyerGroupInfo> AddToBuyerGroups { get; set; }
        public List<BuyerGroupInfo> RemoveFromBuyerGroups { get; set; }
        public List<RelationshipContact> Contacts { get; set; }
    }

    public class BuyerGroupInfo
    {
        public int? Id { get; set; }
        public string ExternalReference { get; set; }
        public string Name { get; set; }
    }

    public enum UserMode
    {
        Buyer,
        Supplier
    }

    public enum CompanySetupOption
    {
        ShowSetupMenu,
        CreateSupportUser,
        CreateAllProductsCatalog,
        CreateBooth,
        CreateConsortiumCatalog,
        CreateConsortiumGroup,
        PublishBooth,
        AddConsortiumManagerAsUser,
        SuppressOutboundWelcomeEmail
    };

    public class RelationshipContact
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EmailAddress { get; set; }

        public string Phone { get; set; }

        public int? UserId { get; set; }

        public string ExternalReference { get; set; }

        public string ExternalSource { get; set; }

    }

}
