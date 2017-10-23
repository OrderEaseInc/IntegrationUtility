using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkGreenODBCUtility
{
    interface IOdbcTransfer
    {
        bool Empty();

        void SaveTableMapping(string dsnName, string tableName);

        bool Publish();
    }
}
