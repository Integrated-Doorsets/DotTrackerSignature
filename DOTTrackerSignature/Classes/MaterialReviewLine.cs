using System;

namespace IdslTracker
{
    internal class MaterialReviewLine
    {
        public string MaterialStatus { get; internal set; }
        public string Supplier { get; internal set; }
        public string PorNr { get; internal set; }
        public DateTime? DateCreated { get; internal set; }
        public DateTime? DateRequired { get; internal set; }
        public DateTime? DateQuoted { get; internal set; }
        public string Description { get; internal set; }
        public string ExtendedDescription { get; internal set; }
        public int? QuantityOrdered { get; internal set; }
        public int? QuantityReceived { get; internal set; }
        public DateTime? ManufactureStartDate { get; internal set; }
        public DateTime? DeliveryDate { get; internal set; }
        public string Contract { get; internal set; }
        public string JobNr { get; internal set; }
        public string ProcurementComment { get; internal set; }
        public string PjlSupplier { get; internal set; }
        public int WeekNum { get; internal set; }
    }
}