using System;
using LinkGreen.Applications.Common.Model;

namespace LinkGreen.Applications.Common.Model
{
    public class NoteViewModel
    {
        public int Id { get; set; }

        public string FormattedCreatedDate => CreatedDate.ToString("MMM dd yyyy");

        public DateTime CreatedDate { get; set; }

        public int CreatingCompanyId { get; set; }

        public int? ReplyToId { get; set; }

        public int? BuyerId { get; set; }

        public int? OrderId { get; set; }

        public CompanyViewModel Buyer { get; set; }

        public int? SupplierId { get; set; }

        public CompanyViewModel Supplier { get; set; }

        public int ItemId { get; set; }

        public string ItemIdentifier { get; set; }

        public string Content { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedByName { get; set; }

        //This is a generated note, non-user created
        public bool System { get; set; }

        //If true, only visible to users who belong to the company which created the note.
        public bool Private { get; set; }

    }
}