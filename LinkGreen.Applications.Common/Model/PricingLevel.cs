using System;

namespace LinkGreen.Applications.Common.Model
{
    public class PricingLevel
    {
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Name { get; set; }

        public string ExternalReference { get; set; }

        public string Comment { get; set; }

        public string FormattedCreationDate { get; set; }

        public DateTime? EffectiveDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string FormattedEffectiveDate { get; set; }

        public string FormattedEndDate { get; set; }

        public int? AnnouncementId { get; set; }

        public bool? AnnounceThis { get; set; }

        public string PreviewEmailAddress { get; set; }

        public bool IsSpecialPricing { get; set; }

        public string Status { get; set; }
    }
}
