namespace HighAvailabilityModule.Interface
{
    using System;

    public class HeartBeatEntry
    {
        public HeartBeatEntry(string uuid, DateTime timeStamp)
        {
            this.Uuid = uuid;
            this.TimeStamp = timeStamp;
        }

        public string Uuid { get; }

        public DateTime TimeStamp { get; }

        public bool IsEmpty => string.IsNullOrEmpty(this.Uuid);

        public static HeartBeatEntry Empty { get; } = new HeartBeatEntry(string.Empty, default);
    }
}
