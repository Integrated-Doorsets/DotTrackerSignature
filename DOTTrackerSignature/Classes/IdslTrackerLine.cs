using System;

namespace IdslTracker
{
    public class IdslTrackerLine
    {
        public Int32 id { get; internal set; }
        public DateTime? DeliveryDate { get; internal set; }
        public string ContractNumber { get; internal set; }
        public string ContractName { get; internal set; }
        public string PostCode { get; internal set; }
        public string ContactName { get; internal set; }
        public string ContactTelephoneNumber { get; internal set; }
        public string VehicleReg { get; internal set; }
        public string VehicleSizeUsed { get; internal set; }

        public string EtaOnSite { get; internal set; }
        public string ActualOnSite { get; internal set; }
        public string Comments { get; internal set; }
        public string CustomerSiteEmailAddress { get; internal set; }
        public string CustomerAccountEmailAddress { get; internal set; }
        public string OtherEmailAddress { get; internal set; }
        public string FTBDeliveryVehicle { get; internal set; }
        public string DriverName { get; internal set; }

        public Boolean NotesEmailed { get; internal set; }
        public Boolean OnTracker { get; internal set; }
        public Boolean OnTime { get; internal set; }
        public Boolean Active { get; internal set; }
        public Boolean Processed { get; internal set; }
        public Boolean Completed { get; internal set; }
        public Boolean Signed { get; internal set; }

        public byte[] SignatureCopy { get; internal set; }

      
    }
}