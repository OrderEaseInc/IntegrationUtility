using LinkGreen.Applications.Common.Model;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;

namespace DataTransfer.AccessDatabase
{
    public class CustomerRepository : OleDbRepository<CompanyAndRelationshipResult>
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
            using (var command = new OleDbCommand($"SELECT * FROM {TableName} WHERE Active = True"))
            {
                return GetRecords(command);
            }
        }

        #endregion

        public void ClearAll()
        {
            using (var command = new OleDbCommand($"DELETE * FROM {TableName}"))
            {
                ExecuteCommand(command);
            }
        }

        public override void SaveFieldMapping(string fieldName, string mappingName)
        {
            using (var command = new OleDbCommand($"UPDATE `FieldMappings` SET `MappingName` = '{mappingName}' WHERE `FieldName` = '{fieldName}' AND `TableName` = '{TableName}'"))
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

        private object CleanNull(object value)
        {
            if (value == null) return value;

            return (value.ToString().Trim().ToLower().Equals("null") ? null : value) ?? string.Empty;
        }

        // NOTE : this is the wire-up of the local odbc table to strongly typed object to be sent via api to LG db
        protected override CompanyAndRelationshipResult PopulateRecord(dynamic reader)
        {
            try
            {
                return new CompanyAndRelationshipResult
                {
                    //RelationshipId = reader.RelationshipId ?? null,
                    //CompanyId = CleanNull(reader.CompanyId),
                    Id = reader.Id,
                    ContactName = CleanNull(reader.ContactName),
                    ContactPhone = CleanNull(reader.ContactPhone),
                    ContactEmail = CleanNull(reader.ContactEmail),
                    OurCompanyNumber = CleanNull(reader.OurCompanyNumber),
                    OurBillToNumber = CleanNull(reader.OurBillToNumber),
                    SerializedTaxInfo = CleanNull(reader.SerializedTaxInfo),
                    Name = CleanNull(reader.Name),
                    Address1 = CleanNull(reader.Address1),
                    Address2 = CleanNull(reader.Address2),
                    City = CleanNull(reader.City),
                    ProvState = CleanNull(reader.ProvState),
                    PostalCode = CleanNull(reader.PostalCode),
                    Country = CleanNull(reader.Country),
                    FormattedPhone1 = CleanNull(reader.FormattedPhone1),
                    Email1 = CleanNull(reader.Email1),
                    Contact1 = CleanNull(reader.Contact1),
                    Contact2 = CleanNull(reader.Contact2),
                    Web = CleanNull(reader.Web),
                    BuyerGroup = CleanNull(reader.BuyerGroup),
                    UserDefinedField1 = CleanNull(reader.UserDefinedField1),
                    UserDefinedField2 = CleanNull(reader.UserDefinedField2),
                    UserDefinedField3 = CleanNull(reader.UserDefinedField3),
                    UserDefinedField4 = CleanNull(reader.UserDefinedField4),
                    SalesRepEmail = CleanNull(reader.SalesRepEmail)
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