using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DataTransfer.AccessDatabase
{
    public sealed class OleDbConnectionInstance : IDisposable
    {
        public void Dispose() {
            CloseConnection();
        }

        public OleDbConnectionInstance(string connectionString) {
            _connectionString = connectionString;
        }

        private OleDbConnection _connection = null;
        private string _connectionString = String.Empty;
        public OleDbConnection GetConnection()
        {
            _connection = new OleDbConnection(_connectionString);
            return _connection;
        }

        public void CloseConnection()
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
                _connection = null;
            }

        }

        public OleDbConnection RefreshConnection()
        {
            CloseConnection();
            _connection = GetConnection();
            return _connection;
        }
    }
}