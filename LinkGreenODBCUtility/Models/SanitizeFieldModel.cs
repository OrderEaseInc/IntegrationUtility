using System.Data;

namespace LinkGreenODBCUtility
{
    public class SanitizeFieldModel
    {
        public string TableName { get; set; }
        public string FieldName { get; set; }
        public string MappingName { get; set; }
        private bool _sanitizeNumbersOnly;
        public bool SanitizeNumbersOnly
        {
            get => _sanitizeNumbersOnly;
            set => _sanitizeNumbersOnly = value;
        }

        private bool _sanitizeAlphaNumeric;
        public bool SanitizeAlphaNumeric
        {
            get => _sanitizeAlphaNumeric;
            set => _sanitizeAlphaNumeric = value;
        }

        private bool _sanitizeEmail;
        public bool SanitizeEmail
        {
            get => _sanitizeEmail;
            set => _sanitizeEmail = value;
        }

        private bool _sanitizePrice;
        public bool SanitizePrice
        {
            get => _sanitizePrice;
            set => _sanitizePrice = value;
        }

        private bool _sanitizeUniqueId;
        public bool SanitizeUniqueId
        {
            get => _sanitizeUniqueId;
            set => _sanitizeUniqueId = value;
        }

        private bool _sanitizeCountry;
        public bool SanitizeCountry
        {
            get => _sanitizeCountry;
            set => _sanitizeCountry = value;
        }

        private bool _sanitizeProvince;
        public bool SanitizeProvince
        {
            get => _sanitizeProvince;
            set => _sanitizeProvince = value;
        }

        public SanitizeFieldModel(IDataRecord reader)
        {
            TableName = reader["TableName"].ToString();
            FieldName = reader["FieldName"].ToString();
            MappingName = reader["MappingName"].ToString();
            bool.TryParse(reader["SanitizeNumbersOnly"].ToString(), out _sanitizeNumbersOnly);
            bool.TryParse(reader["SanitizeAlphaNumeric"].ToString(), out _sanitizeAlphaNumeric);
            bool.TryParse(reader["SanitizeEmail"].ToString(), out _sanitizeEmail);
            bool.TryParse(reader["SanitizePrice"].ToString(), out _sanitizePrice);
            bool.TryParse(reader["SanitizeUniqueId"].ToString(), out _sanitizeUniqueId);
            bool.TryParse(reader["SanitizeCountry"].ToString(), out _sanitizeCountry);
            bool.TryParse(reader["SanitizeProvince"].ToString(), out _sanitizeProvince);
        }
    }
}
