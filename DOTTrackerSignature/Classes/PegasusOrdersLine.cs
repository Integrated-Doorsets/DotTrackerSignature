using System;

namespace IdslTracker
{
    internal class PegasusOrdersLine
    {
        public string Reference { get; internal set; }
        public string CostCode { get; internal set; }
        public string StockCode { get; internal set; }
        public string Desc { get; internal set; }
        public string ExtendedDesc { get; internal set; }
        public decimal? Quantity { get; internal set; }
        public decimal Price { get; internal set; }
        public string SupplierName { get; internal set; }
        public DateTime? DateRequired { get; internal set; }
        public DateTime? DateReceived { get; internal set; }
        public int RowERR { get; internal set; }
        public string Warehouse { get; internal set; }
        public decimal? QuantityReceived { get; internal set; }
        public string ProcurementComments { get; internal set; }
        public DateTime? DateQuoted { get; internal set; }
        public DateTime? DateCreated { get; internal set; }
    }
}