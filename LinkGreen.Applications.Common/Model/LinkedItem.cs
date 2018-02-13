namespace LinkGreen.Applications.Common.Model
{
    public class LinkedItem
    {
        public string BuyerSku { get; set; }
        public string SupplierSku { get; set; }
        public int? SupplierId { get; set; }
        public bool Processed { get; set; }
    }
}