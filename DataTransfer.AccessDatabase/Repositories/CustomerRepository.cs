using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.IO;
using Microsoft.CSharp.RuntimeBinder;

namespace DataTransfer.AccessDatabase
{
    public class CustomerRepository : AdoRepository<Customer>
    {
        public CustomerRepository(string connectionString) : base(connectionString)
        {
        }

        #region Private Consts

        private const string TableName = "Customers";
        private const string TableKey = "CustomerId";

        #endregion

        #region Get

        public IEnumerable<Customer> GetAll()
        {
            // DBAs across the country are having strokes 
            //  over this next command!
            using (var command = new OdbcCommand($"SELECT * FROM {TableName} WHERE Active = True AND OnHold = False AND Whilma = True"))
            {
                return GetRecords(command);
            }
        }

        #endregion

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
        protected override Customer PopulateRecord(dynamic reader)
        {
            try
            {
                return new Customer
                {
                    CompanyName = reader.CompanyName ?? string.Empty,
                    CompanyNumber = reader.CustomerID == null ? string.Empty : reader.CustomerID.ToString(),
                    ContactFirst = reader.ContactFirstName ?? string.Empty,
                    ContactLast = reader.ContactLastName ?? string.Empty,

                    BillAddress = reader.Address ?? string.Empty,
                    BillCity = reader.City ?? string.Empty,
                    BillProvState = CleanProvince(reader.Province ?? string.Empty),
                    BillCountry = reader.Country == null ? "Canada" : reader.Country.ToString().ToUpper() == "US" ? "US" : "Canada",
                    BillPostalCode = reader.PostalCode ?? string.Empty,

                    Phone = reader.ShipPhoneNumber ?? string.Empty,
                    ContactPhone = reader.WorkPhone ?? string.Empty,
                    Email = reader.EmailAddress ?? string.Empty,
                    BuyerGroup = reader.Territory,
                    BillToNumber = reader.BillingID == null ? string.Empty : reader.BillingID.ToString(),


                    Address = reader.ShipAddress ?? string.Empty,
                    City = reader.ShipCity ?? string.Empty,
                    ProvState = CleanProvince(reader.ShipProvince ?? string.Empty),
                    Country = reader.ShipCountry == null ? "Canada" : reader.ShipCountry.ToString().ToUpper() == "US" ? "US" : "Canada",
                    PostalCode = reader.ShipPostalCode ?? string.Empty
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