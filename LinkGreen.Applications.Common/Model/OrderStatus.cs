namespace LinkGreen.Applications.Common.Model
{
    public class OrderStatus
    {
        public int Id { get; set; }

        public string Status { get; set; }

        public bool DefaultVisible { get; set; }

        public bool WarningOnChangingTo { get; set; }

        public bool WarningOnChangingFrom { get; set; }

        public string GroupName { get; set; }

        public bool IsSupplierStatus { get; set; }

        public bool IsBuyerStatus { get; set; }
    }
}
