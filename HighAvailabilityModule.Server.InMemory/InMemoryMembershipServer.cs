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

        private object heartbeatLock = new object();

        public InMemoryMembershipServer(TimeSpan timeout)
        {
            this.Timeout = timeout;
        }

        public void RemoveCurrent()
        {
            lock (heartbeatLock)
            {
                List<string> key = new List<string>(this.CurrentTable.Keys);
                for (int i = 0; i < key.Count; i++)
                {
                    this.CurrentTable[key[i]] = HeartBeatEntry.Empty;
                }
            }
        }

        public Task HeartBeatAsync(HeartBeatEntryDTO entryDTO) => this.HeartBeatAsync(entryDTO, DateTime.UtcNow);

        public async Task HeartBeatAsync(HeartBeatEntryDTO entryDTO, DateTime now)
        {
            bool ValidInput()
            {
                return !this.CurrentTable.ContainsKey(entryDTO.Utype) || this.CurrentTable[entryDTO.Utype] == null
                        || (this.HeartbeatInvalid(entryDTO.Utype, now) && entryDTO.LastSeenEntry != null && entryDTO.LastSeenEntry.IsEmpty)
                        || (this.LastSeenEntryValid(entryDTO.Utype, entryDTO.LastSeenEntry) && this.CurrentTable[entryDTO.Utype].Uuid == entryDTO.Uuid && this.CurrentTable[entryDTO.Utype].Utype == entryDTO.Utype);
            }

            if (!ValidInput())
            {
                return;
            }

            lock (this.heartbeatLock)
            {
                if (!ValidInput())
                {
                    return;
                }

                this.Current = new HeartBeatEntry(entryDTO.Uuid, entryDTO.Utype, entryDTO.Uname, now);

                this.CurrentTable[entryDTO.Utype] = this.Current;
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
                return this.CurrentTable[utype];
            }                
        }

        private bool HeartbeatInvalid(string utype, DateTime now)
        {
            return !this.CurrentTable.ContainsKey(utype) || this.CurrentTable[utype] == null || (now - this.CurrentTable[utype].TimeStamp >= this.Timeout);
        }

        private bool LastSeenEntryValid(string utype, HeartBeatEntry LastSeenEntry)
        {
            return this.CurrentTable.ContainsKey(utype) && LastSeenEntry != null && this.CurrentTable[utype].Uuid == LastSeenEntry.Uuid &&
            this.CurrentTable[utype].Utype == LastSeenEntry.Utype && this.CurrentTable[utype].TimeStamp == LastSeenEntry.TimeStamp;
        }
    }
}