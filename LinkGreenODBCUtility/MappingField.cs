using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkGreenODBCUtility
{
    public class MappingField
    {
        public string TableName { get; set; }

        public string FieldName { get; set; }

        public string MappingName { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public string DataType { get; set; }
        
        public bool Required { get; set; }

        public bool Updatable { get; set; }
    }
}
