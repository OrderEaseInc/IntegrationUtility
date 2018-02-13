using System.Collections.Generic;
using System.ComponentModel;

namespace LinkGreenODBCUtility
{
    internal interface IOdbcTransfer
    {
        bool Empty();

        void SaveTableMapping(string dsnName, string tableName);

        bool Publish(out List<string> processDetails, BackgroundWorker bw);
    }
}
