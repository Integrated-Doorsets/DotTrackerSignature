namespace IdslTracker
{
    internal class SalesOrderLine
    {
        public string Stock { get; internal set; }
        public string Description { get; internal set; }
        public int? Ordered { get; internal set; }
        public int? Delivered { get; internal set; }
        public int? Invoiced { get; internal set; }
        public string Memo { get; internal set; }
    }
}