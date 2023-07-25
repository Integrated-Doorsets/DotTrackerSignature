using System;

namespace IdslTracker
{
    public class CreditStopReviewLine
    {
        public string Customer { get; internal set; }
        public DateTime DeliveryMonth { get; internal set; }
        public decimal CreditLimit { get; internal set; }
        public string CustomerStatus { get; internal set; }
        public string Contract { get; internal set; }
        public string Comment { get; internal set; }
        public string DocNr { get; internal set; }
        public DateTime EarliestDeliveryDate { get; internal set; }
        public decimal CreditLimitIncVat { get; internal set; }
        public decimal CurrentDebtBalance { get; internal set; }
        public decimal CumulativeGrossSales { get; internal set; }
        public decimal PotentialDebtBalance { get; internal set; }
        public decimal Sales { get; internal set; }
        public bool PotentialOverLimit { get; internal set; }
        public string AccountNumber { get; internal set; }
    }
}