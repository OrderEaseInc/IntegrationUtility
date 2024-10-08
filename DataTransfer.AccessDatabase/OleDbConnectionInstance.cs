using System;
using System.Data.OleDb;

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

        private OleDbConnection _connection;
        private readonly string _connectionString;
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