namespace HighAvailabilityModule.Server.InMemory
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.ExceptionServices;
    using System.Threading.Tasks;

    using HighAvailabilityModule.Interface;

    public class InMemoryMembershipServer : IMembership
    {
        private HeartBeatEntry Current;

        public Dictionary<string, HeartBeatEntry> CurrentTable { get; set; } = new Dictionary<string, HeartBeatEntry>() {};

        private TimeSpan Timeout { get; }

        private static object heartbeatLock = new object();

        public InMemoryMembershipServer(TimeSpan timeout)
        {
            this.Timeout = timeout;
        }

        public void RemoveCurrent()
        {
            lock (heartbeatLock)
            {
                List<string> key = new List<string>(this.CurrentTable.Keys);
                for (int i = 0;i<key.Count;i++)
                {
                    this.CurrentTable[key[i]] = HeartBeatEntry.Empty;
                }
            }
        }

        public Task HeartBeatAsync(string uuid, string utype, HeartBeatEntry lastSeenEntry) => this.HeartBeatAsync(uuid, utype, lastSeenEntry, DateTime.UtcNow);

        public async Task HeartBeatAsync(string uuid, string utype, HeartBeatEntry lastSeenEntry, DateTime now)
        {
            bool ValidInput()
            {
                if (this.CurrentTable.ContainsKey(utype))
                {
                    return this.CurrentTable[utype] == null
                           || (this.HeartbeatInvalid(utype, now) && lastSeenEntry != null && lastSeenEntry.IsEmpty)
                           || (lastSeenEntry != null && this.CurrentTable[utype].Uuid == lastSeenEntry.Uuid &&
                           this.CurrentTable[utype].TimeStamp == lastSeenEntry.TimeStamp && this.CurrentTable[utype].Uuid == uuid);
                }
                else
                {
                    return true;
                }

            }

            if (!ValidInput())
            {
                return;
            }


            lock (heartbeatLock)
            {
                if (!ValidInput())
                {
                    return;
                }

                this.Current = new HeartBeatEntry(uuid, utype, now);

                if (this.CurrentTable.ContainsKey(utype))
                {
                    this.CurrentTable[utype] = this.Current;
                }
                else
                {
                    this.CurrentTable.Add(utype, this.Current);
                }
            }
        }

        public Task<HeartBeatEntry> GetHeartBeatEntryAsync(string utype) => this.GetHeartBeatEntryAsync(utype, DateTime.UtcNow);

        public async Task<HeartBeatEntry> GetHeartBeatEntryAsync(string utype, DateTime now)
        {
            if (this.HeartbeatInvalid(utype, now))
            {
                return HeartBeatEntry.Empty;
            }
            else
            {
                if (this.CurrentTable.ContainsKey(utype))
                {
                    return this.CurrentTable[utype];
                }
                    
                else
                    return HeartBeatEntry.Empty;
            }                
        }

        private bool HeartbeatInvalid(string utype, DateTime now)
        {
            if (this.CurrentTable.ContainsKey(utype))
                return this.CurrentTable[utype] == null || (now - this.CurrentTable[utype].TimeStamp >= this.Timeout);
            else
                return true;
        }
    }
}