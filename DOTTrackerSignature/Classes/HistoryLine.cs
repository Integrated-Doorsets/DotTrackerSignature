using System;

namespace IdslTracker
{
    internal class HistoryLine
    {
        public string ManfSite { get; internal set; }
        public string ProductionCommentDoor { get; internal set; }
        public int? DoorQty { get; internal set; }
        public int? FrameQty { get; internal set; }
        public int? ScreenQty { get; internal set; }
        public DateTime? DeliveryDate { get; internal set; }
        public DateTime? Timestamp { get; internal set; }
        public string Username { get; internal set; }
        public int? PanelQty { get; internal set; }
        public int? MiscQty { get; internal set; }
        public int? IronmongeryQty { get; internal set; }
        public DateTime? FilePrintedDate { get; internal set; }
        public string FilePrintedBy { get; internal set; }
        public bool ManufactureCompleted { get; internal set; }
        public string ProductionCommentFrame { get; internal set; }
        public string ManualMaterialComment { get; internal set; }
    }
}