using System;

namespace IdslTracker
{
    public class AccrualsLine
    {
        public int Id { get; internal set; }
        public string ContractName { get; internal set; }
        public string JobNr { get; internal set; }
        public decimal TrackerValue { get; internal set; }
        public decimal Value2099 { get; internal set; }
        public decimal InvoicedValue { get; internal set; }
        public string StatusPerTracker { get; internal set; }
        public DateTime? Date { get; internal set; }
        public decimal DbVal { get; internal set; }
        public string DocNr { get; internal set; }
        public string Category { get; internal set; }
    }
}