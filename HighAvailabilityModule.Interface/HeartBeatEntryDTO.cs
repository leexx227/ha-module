using System;
using System.Collections.Generic;
using System.Text;

namespace HighAvailabilityModule.Interface
{
    using System;
    public class HeartBeatEntryDTO
    {
        public HeartBeatEntryDTO(string uuid, string utype, string unum, HeartBeatEntry lastSeenEntry)
        {
            this.Uuid = uuid;
            this.Utype = utype;
            this.Unum = unum;
            this.LastSeenEntry = lastSeenEntry;
        }

        public string Uuid { get; }

        public string Utype { get; }

        public string Unum { get; }

        public HeartBeatEntry LastSeenEntry { get; }
    }
}
