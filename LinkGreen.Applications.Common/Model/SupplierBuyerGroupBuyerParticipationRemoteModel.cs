namespace LinkGreen.Applications.Common.Model
{
    public class SupplierBuyerGroupBuyerParticipationRemoteModel
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public string PaymentTerms { get; set; }
        public decimal DiscountAmount { get; set; }
    }
}