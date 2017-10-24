using DataTransfer.AccessDatabase;

namespace LinkGreenODBCUtility
{
    public class Buyers : IOdbcTransfer
    {
        private readonly BuyerRepository buyerRepository;

        public Buyers()
        {
            buyerRepository = new BuyerRepository($"DSN={Settings.DsnName}");
        }

        public bool Empty()
        {
            buyerRepository.ClearAll();
            Logger.Instance.Info("Buyers LinkGreen transfer table emptied.");
            Logger.Instance.Debug($"{Settings.DsnName}.Buyers emptied.");
            return true;
        }

        public void SaveTableMapping(string dsnName, string tableName)
        {
            buyerRepository.SaveTableMapping(dsnName, tableName, "Buyers");
            Logger.Instance.Debug($"Buyers table mapping saved: (DSN: {dsnName}, Table: {tableName})");
        }

        public void SaveFieldMapping(string fieldName, string mappingName)
        {
            buyerRepository.SaveFieldMapping(fieldName, mappingName);
            Logger.Instance.Debug($"Buyers field mapping saved: (Field: {fieldName}, MappingField: {mappingName})");
        }

        public bool Publish()
        {
            var result = buyerRepository.SyncAllBuyers();
            foreach (var pair in result) {
                Logger.Instance.Debug($"Buyer Sync: {pair.Value} buyers {pair.Key}");
            }
            return true;
        }
    }
}