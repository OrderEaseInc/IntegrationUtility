using System.Data.Odbc;
using System.Collections.Generic;
using LinkGreen.Applications.Common.Model;

namespace DataTransfer.AccessDatabase
{
    public class CountryMappingRepository : AdoRepository<CountryMapping>
    {
        private const string TableName = "countrymapping";
        private readonly OdbcConnection _connection;

        /// <summary>
        /// Create a new instance of CountryMappingRepository
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="connection"></param>
        public CountryMappingRepository(string connectionString, OdbcConnection connection) : base(connectionString)
        {
            _connection = connection;
        }

        /// <summary>
        /// Get all country mapping rules
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CountryMapping> GetAll()
        {
            using (var command = new OdbcCommand($"SELECT * FROM {TableName}"))
                return GetRecords(command, _connection);
        }

        protected override CountryMapping PopulateRecord(dynamic reader)
        {
            return new CountryMapping
            {
                Id = reader.id,
                Source = reader.source,
                Destination = reader.destination
            };
        }
    }
}
