using System;

namespace IdslTracker
{
    public class ManualsLine
    {
        public int Id { get; internal set; }
        public string JobType { get; internal set; }
        public string ContractName { get; internal set; }
        public string ContractNumber { get; internal set; }
        public string Scheduler { get; internal set; }
        public DateTime? Month { get; internal set; }
        public decimal Value { get; internal set; }
    }
}