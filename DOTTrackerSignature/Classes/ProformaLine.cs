using System;

namespace IdslTracker
{
    public class ProformaLine
    {
        public string JobNumber { get; internal set; }
        public string CustomerCode { get; internal set; }
        public string CustomerName { get; internal set; }
        public string ContractName { get; internal set; }
        public string ProformaRef { get; internal set; }
        public string DocRef { get; internal set; }
        public DateTime? DocDate { get; internal set; }
        public decimal GoodsValue { get; internal set; }
        public decimal SalesValue { get; internal set; }
        public string Comments { get; internal set; }
        public DateTime? DeliveryDate { get; internal set; }
        public bool Complete { get; internal set; }
        public int Id { get; internal set; }
    }
}