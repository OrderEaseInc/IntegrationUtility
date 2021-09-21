using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;

namespace LinkGreen.Applications.Common.Model
{
    public class Order
    {
        public IEnumerable<OrderDetail> Details { get; set; }

        public Company SupplierCompany { get; set; }

        public Company BuyerCompany { get; set; }

        public virtual int Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ShippingDate { get; set; }

        public DateTime? RequestedShippingDate { get; set; }

        public DateTime? AnticipatedShipDate { get; set; }

        public OrderStatus Status { get; set; }

        public string SupplierStatus { get; set; }

        public string BuyerStatus { get; set; }

        public string PaymentTerm { get; set; }

        public int? PaymentTermId { get; set; }

        public int SupplierStatusId { get; set; }

        public int BuyerStatusId { get; set; }

        public string BuyerComment { get; set; }

        public string SupplierComment { get; set; }

        public decimal? Freight { get; set; }

        public string SupplierPO { get; set; }

        public string BuyerPO { get; set; }

        public string ContactName { get; set; }

        public string OrderNumber { get; set; }

        public bool IsDirectDelivery { get; set; }

        public bool SupplierCanExport { get; set; }

        public int SupplierCompanyId { get; set; }

        public int BuyerCompanyId { get; set; }

        public string Name { get; set; }

        public decimal OrderTotalBeforeSubmitting => Details.Sum(o => o.DiscountedPrice * o.QuantityRequested.GetValueOrDefault() * o.Units);

        public decimal OrderTotal => Details.Sum(o => o.Amount);

        public AllNotesToAndFromCompanyRemoteResult Notes { get; set; }

        public string ConsolidatedNote
        {
            get
            {

                if (Notes.OurNotes == null) Notes.OurNotes = new List<NoteViewModel>();
                if (Notes.TheirNotes == null) Notes.TheirNotes = new List<NoteViewModel>();
                var combined = Notes.OurNotes?.Union(Notes.TheirNotes).Select(r =>
                {
                    r.Content = (r.CreatingCompanyId == r.SupplierId ? "Supplier: " : "Buyer: ") + r.Content;
                    return r;
                });

                var note = string.Join(" - ", combined
                    .OrderBy(n => n.CreatedDate)
                    .Select(n => n.Content).ToArray());

                return note;
            }
        }
    }
}
