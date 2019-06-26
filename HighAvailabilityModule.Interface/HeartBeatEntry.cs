namespace HighAvailabilityModule.Interface
{
    using System;

    public class HeartBeatEntry
    {
        public HeartBeatEntry(string uuid, string utype, string unum, DateTime timeStamp)
        {
            this.Uuid = uuid;
            this.Utype = utype;
            this.Unum = unum;
            this.TimeStamp = timeStamp;
        }

        public string Uuid { get; }

        public string Utype { get; }

        public string Unum { get; }

        public DateTime TimeStamp { get; }

        public bool IsEmpty => string.IsNullOrEmpty(this.Uuid);

        public static HeartBeatEntry Empty { get; } = new HeartBeatEntry(string.Empty, string.Empty, string.Empty, default);
    }
}
