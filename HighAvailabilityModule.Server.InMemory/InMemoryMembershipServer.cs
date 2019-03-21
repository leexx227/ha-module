namespace HighAvailabilityModule.Server.InMemory
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.ExceptionServices;
    using System.Threading.Tasks;

    using HighAvailabilityModule.Interface;

    public class InMemoryMembershipServer : IMembership
    {
        public HeartBeatEntry Current { get; set; }

        private TimeSpan Timeout { get; }

        public InMemoryMembershipServer(TimeSpan timeout)
        {
            this.Timeout = timeout;
        }

        public Task HeartBeatAsync(string uuid, HeartBeatEntry lastSeenEntry) => this.HeartBeatAsync(uuid, lastSeenEntry, DateTime.UtcNow);


        public async Task HeartBeatAsync(string uuid, HeartBeatEntry lastSeenEntry, DateTime now)
        {
            bool ValidInput()
            {
                return this.Current == null || (lastSeenEntry != null && this.Current.Uuid == lastSeenEntry.Uuid && this.Current.TimeStamp == lastSeenEntry.TimeStamp);
            }

            if (!ValidInput())
            {
                return;
            }

            if (this.HeartbeatInvalid(now))
            {
                this.Current = new HeartBeatEntry(uuid, now);
            }
            else if (this.Current.Uuid == uuid)
            {
                this.Current = new HeartBeatEntry(uuid, now);
            }
        }

        public Task<HeartBeatEntry> GetHeartBeatEntryAsync() => this.GetHeartBeatEntryAsync(DateTime.UtcNow);


        public async Task<HeartBeatEntry> GetHeartBeatEntryAsync(DateTime now)
        {
            if (this.HeartbeatInvalid(now))
            {
                return HeartBeatEntry.Empty;
            }
            else
            {
                return this.Current;
            }
        }
        

        private bool HeartbeatInvalid(DateTime now) => this.Current == null || (now - this.Current.TimeStamp > this.Timeout);
    }
}
