using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DataTransfer.AccessDatabase
{
    public sealed class ConnectionInstance
    {
        private static ConnectionInstance instance = null;
        private static readonly object padlock = new object();
        private static Dictionary<string, OdbcConnection> _connectionContainer;

        private ConnectionInstance()
        {
            _connectionContainer = new Dictionary<string, OdbcConnection>();
        }

        public static ConnectionInstance Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new ConnectionInstance();
                        }
                    }
                }
                return instance;
            }
        }

        internal void AddConnectionIfNotExists(string connectionString)
        {
            if (!_connectionContainer.ContainsKey(connectionString))
            {
                _connectionContainer.Add(connectionString, new OdbcConnection(connectionString));
            }
        }
        
        public static OdbcConnection GetConnection(string connectionString)
        {
            Instance.AddConnectionIfNotExists(connectionString);
            var connection = _connectionContainer[connectionString];
            
            Stopwatch sw = new Stopwatch();
            sw.Start();
            bool keepTrying = true;

            while (keepTrying)
            {
                if (sw.Elapsed > TimeSpan.FromSeconds(3600))
                {
                    connection.Close();
                    // Handle this and inform user
                }
                if (connection.State == ConnectionState.Closed)
                {
                    keepTrying = false;
                }

                Thread.Sleep(1000);
            }

            sw.Stop();
            
            return connection;
        }
    }
}