using System;
using System.Data;
using System.Data.Odbc;
using LinkGreen.Applications.Common.Model;

namespace DataTransfer.AccessDatabase
{
    [Obsolete("proto?")]
    public abstract class OdbcDataTransferObjectBase
    {
        public virtual InventoryItemRequest ToInventoryItemRequest() { throw new NotImplementedException(); }

        // this was just a prototype probably not necessary
        public bool TryConnectToOdbcSource(string source)
        {
            using (OdbcConnection odbcConn = new OdbcConnection(source))
            using (OdbcCommand cmd = new OdbcCommand())
            {

                //open connection 
                if (odbcConn.State != ConnectionState.Open)
                {
                    odbcConn.Open();
                }
            }

            return true;
        }
    }
}