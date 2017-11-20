﻿using System;
using System.Collections.Concurrent;
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
        private static ConcurrentDictionary<string, OdbcConnection> _connectionContainer;

        private ConnectionInstance()
        {
            _connectionContainer = new ConcurrentDictionary<string, OdbcConnection>();
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
                _connectionContainer.GetOrAdd(connectionString, new OdbcConnection(connectionString));
            }
        }
        
        /// <summary>
        /// Used to retrieve a new connection. If a connection is not active (fetching or executing) then it will wait for 15 minutes before timing out.
        /// NOTE: Do not try and call GetConnection if another useage has not opened it's connection already
        /// </summary>
        /// <param name="connectionString">The string to be used for the connection</param>
        /// <returns>OdbcConnection</returns>
        public OdbcConnection GetConnection(string connectionString)
        {
            Instance.AddConnectionIfNotExists(connectionString);
            var connection = _connectionContainer[connectionString];
            
            Stopwatch sw = new Stopwatch();
            sw.Start();
            bool keepTrying = true;

            while (keepTrying)
            {
                if (sw.Elapsed > TimeSpan.FromSeconds(900)) // 15 mins
                {
                    if (connection.State != ConnectionState.Executing && connection.State != ConnectionState.Fetching)
                    {
                        connection = RefreshConnection(connectionString);
                    }
                    sw.Restart();
                }

                if (connection.State == ConnectionState.Closed)
                {
                    keepTrying = false;
                }
            }

            sw.Stop();

            if (connection.State != ConnectionState.Executing && connection.State != ConnectionState.Fetching)
            {
                connection = RefreshConnection(connectionString);
            }

            return connection;
        }

        public static void CloseConnection(string connectionString)
        {
            var connection = _connectionContainer[connectionString];
            connection.Close();
        }

        public static OdbcConnection RefreshConnection(string connectionString)
        {
            var connection = _connectionContainer[connectionString];
            connection.Close();
            connection.Dispose();
            _connectionContainer.TryRemove(connectionString, out connection);
            Instance.AddConnectionIfNotExists(connectionString);
            return _connectionContainer[connectionString];
        }
    }
}