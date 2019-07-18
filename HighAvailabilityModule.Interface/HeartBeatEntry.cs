namespace HighAvailabilityModule.Interface
{
    using System;

    public class HeartBeatEntry
    {
        public HeartBeatEntry(string uuid, string utype, string uname, DateTime timeStamp)
        {
            this.Uuid = uuid;
            this.Utype = utype;
            this.Uname = uname;
            this.TimeStamp = timeStamp;
        }

        public string Uuid { get; }

        public string Utype { get; }

        public string Uname { get; }

        public DateTime TimeStamp { get; }

        public bool IsEmpty => string.IsNullOrEmpty(this.Uuid);

        public static HeartBeatEntry Empty { get; } = new HeartBeatEntry(string.Empty, string.Empty, string.Empty, default);

        public override bool Equals(object obj)
        {
            if (obj is HeartBeatEntry that)
            {
                return this.Uuid == that.Uuid && this.Utype == that.Utype && this.Uname == that.Uname && this.TimeStamp == that.TimeStamp;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.Uuid.GetHashCode() ^ this.TimeStamp.GetHashCode();
        }

        public override string ToString() => $"{this.Uuid} - {this.Utype} - {this.Uname} - {this.TimeStamp}";
    }
}
