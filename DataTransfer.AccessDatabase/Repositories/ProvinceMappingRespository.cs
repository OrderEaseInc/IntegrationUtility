using System.Data.Odbc;
using System.Collections.Generic;
using LinkGreen.Applications.Common.Model;

namespace DataTransfer.AccessDatabase
{
    public class ProvinceMappingRepository : AdoRepository<ProvinceMapping>
    {
        private const string TableName = "ProvinceMappings";
        private readonly OdbcConnection _connection;

        /// <summary>
        /// Create a new instance of CountryMappingRepository
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="connection"></param>
        public ProvinceMappingRepository(string connectionString, OdbcConnection connection = null) : base(
            connectionString)
        {
            _connection = connection;
        }

        /// <summary>
        /// Get all country mapping rules
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ProvinceMapping> GetAll()
        {
            using (var command = new OdbcCommand($"SELECT * FROM {TableName}"))
                return GetRecords(command, _connection);
        }

        protected override ProvinceMapping PopulateRecord(dynamic reader)
        {
            return new ProvinceMapping
            {
                Id = reader.id,
                Source = reader.source,
                Destination = reader.destination
            };
        }
    }
}
