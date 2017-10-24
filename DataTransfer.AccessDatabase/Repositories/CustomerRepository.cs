using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using Microsoft.CSharp.RuntimeBinder;
using LinkGreen.Applications.Common.Model;

namespace DataTransfer.AccessDatabase
{
    public class CustomerRepository : AdoRepository<CompanyAndRelationshipResult>
    {
        public CustomerRepository(string connectionString) : base(connectionString)
        {
        }

        #region Private Consts

        private const string TableName = "Customers";
        private const string TableKey = "CustomerId";

        #endregion

        #region Get

        public IEnumerable<CompanyAndRelationshipResult> GetAll()
        {
            // DBAs across the country are having strokes 
            //  over this next command!
            using (var command = new OdbcCommand($"SELECT * FROM {TableName} WHERE Active = True"))
            {
                return GetRecords(command);
            }
        }

        #endregion

        public void ClearAll()
        {
            using (var command = new OdbcCommand($"DELETE * FROM {TableName}"))
            {
                ExecuteCommand(command);
            }
        }

        public override void SaveFieldMapping(string fieldName, string mappingName)
        {
            using (var command = new OdbcCommand($"UPDATE `FieldMappings` SET `MappingName` = '{mappingName}' WHERE `FieldName` = '{fieldName}' AND `TableName` = '{TableName}'"))
            {
                ExecuteCommand(command);
            }
        }

        private string CleanProvince(string province)
        {
            switch (province.ToUpper())
            {
                case "ALABAMA": return "AL";
                case "ALASKA": return "AK";
                case "ARIZONA": return "AZ";
                case "ARKANSAS": return "AR";
                case "CALIFORNIA": return "CA";
                case "COLORADO": return "CO";
                case "CONNECTICUT": return "CT";
                case "DELAWARE": return "DE";
                case "FLORIDA": return "FL";
                case "GEORGIA": return "GA";
                case "HAWAII": return "HI";
                case "IDAHO": return "ID";
                case "ILLINOIS": return "IL";
                case "INDIANA": return "IN";
                case "IOWA": return "IA";
                case "KANSAS": return "KS";
                case "KENTUCKY": return "KY";
                case "LOUISIANA": return "LA";
                case "MAINE": return "ME";
                case "MARYLAND": return "MD";
                case "MASSACHUSETTS": return "MA";
                case "MICHIGAN": return "MI";
                case "MINNESOTA": return "MN";
                case "MISSISSIPPI": return "MS";
                case "MISSOURI": return "MO";
                case "MONTANA": return "MT";
                case "NEBRASKA": return "NE";
                case "NEVADA": return "NV";
                case "NEW HAMPSHIRE": return "NH";
                case "NEW JERSEY": return "NJ";
                case "NEW MEXICO": return "NM";
                case "NEW YORK": return "NY";
                case "NORTH CAROLINA": return "NC";
                case "NORTH DAKOTA": return "ND";
                case "OHIO": return "OH";
                case "OKLAHOMA": return "OK";
                case "OREGON": return "OR";
                case "PENNSYLVANIA": return "PA";
                case "RHODE ISLAND": return "RI";
                case "SOUTH CAROLINA": return "SC";
                case "SOUTH DAKOTA": return "SD";
                case "TENNESSEE": return "TN";
                case "TEXAS": return "TX";
                case "UTAH": return "UT";
                case "VERMONT": return "VT";
                case "VIRGINIA": return "VA";
                case "WASHINGTON": return "WA";
                case "WEST VIRGINIA": return "WV";
                case "WISCONSIN": return "WI";
                case "WYOMING": return "WY";


                case "ALBERTA": return "AB";
                case "BRITISH COLUMBIA": return "BC";
                case "MANITOBA": return "MB";
                case "NEW BRUNSWICK": return "NB";
                case "NEWFOUNDLAND AND LABRADOR": return "NL";
                case "NOVA SCOTIA": return "NS";
                case "NORTHWEST TERRITORIES": return "NT";
                case "NUNAVUT": return "NU";
                case "ONTARIO": return "ON";
                case "PRINCE EDWARD ISLAND": return "PE";
                case "QUEBEC": return "QC";
                case "SASKATCHEWAN": return "SK";
                case "YUKON": return "YT";

                case "SASKTCHEWAN": return "SK";
                case "N.S.": return "NS";
                case "NWT": return "NT";
            }

            return province;
        }

        // NOTE : this is the wire-up of the local odbc table to strongly typed object to be sent via api to LG db
        protected override CompanyAndRelationshipResult PopulateRecord(dynamic reader)
        {
            try
            {
                return new CompanyAndRelationshipResult
                {
                    //RelationshipId = reader.RelationshipId ?? null,
                    //CompanyId = reader.CompanyId ?? null,
                    ContactName = reader.ContactName ?? string.Empty,
                    ContactPhone = reader.ContactPhone ?? string.Empty,
                    ContactEmail = reader.ContactEmail ?? string.Empty,
                    OurCompanyNumber = reader.OurCompanyNumber ?? string.Empty,
                    OurBillToNumber = reader.OurBillToNumber ?? string.Empty,
                    SerializedTaxInfo = reader.SerializedTaxInfo ?? string.Empty,
                    Name = reader.Name ?? string.Empty,
                    Address1 = reader.Address1 ?? string.Empty,
                    Address2 = reader.Address2 ?? string.Empty,
                    City = reader.City ?? string.Empty,
                    ProvState = reader.ProvState ?? string.Empty,
                    PostalCode = reader.PostalCode ?? string.Empty,
                    Country = reader.Country ?? string.Empty,
                    FormattedPhone1 = reader.FormattedPhone1 ?? string.Empty,
                    FormattedPhone2 = reader.FormattedPhone2 ?? string.Empty,
                    Email1 = reader.Email1 ?? string.Empty,
                    Email2 = reader.Email2 ?? string.Empty,
                    Contact1 = reader.Contact1 ?? string.Empty,
                    Contact2 = reader.Contact2 ?? string.Empty,
                    Web = reader.Web ?? string.Empty,
                    BuyerGroup = reader.BuyerGroup ?? string.Empty 
                };
            }
            catch (RuntimeBinderException exception)
            {
                Console.WriteLine(exception);
                throw new InvalidDataException("One of the fields in the source ODBC database has an invalid column type or value", exception);
            }
        }
    }
}