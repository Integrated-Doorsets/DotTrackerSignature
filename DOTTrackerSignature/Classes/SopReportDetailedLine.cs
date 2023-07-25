using System;

namespace IdslTracker
{
    internal class SopReportDetailedLine
    {
        public string Source { get; internal set; }
        public string Department { get; internal set; }
        public decimal Sales { get; internal set; }
        public DateTime Date { get; internal set; }
        public string Type { get; internal set; }
        public string Job { get; internal set; }
        public string ContractName { get; internal set; }
        public bool FilteredOut { get; internal set; }
        public string WipStatus { get; internal set; }
        public int FtbIndexId { get; internal set; }
        public string Scheduler { get; internal set; }
    }
}