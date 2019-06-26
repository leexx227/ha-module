namespace HighAvailabilityModule.Client.InMemory
{
    using System;
    using System.Threading.Tasks;

    using HighAvailabilityModule.Interface;

    public class InMemoryMembershipClient : IMembershipClient
    {
        public InMemoryMembershipClient(IMembership membershipServer) : this(membershipServer, Guid.NewGuid().ToString(), "A", "1")
        {
        }

        public InMemoryMembershipClient(IMembership membershipServer, string uuid, string utype, string unum)
        {
            this.serverImplementation = membershipServer;
            this.Uuid = uuid;
            this.Utype = utype;
            this.Unum = unum;
        }

        private IMembership serverImplementation;

        public string Uuid { get; }
        public string Utype { get; set; }
        public string Unum { get; set; }

        public Task HeartBeatAsync(HeartBeatEntryDTO entryDTO) => this.serverImplementation.HeartBeatAsync(entryDTO);

        public Task<HeartBeatEntry> GetHeartBeatEntryAsync(string utype) => this.serverImplementation.GetHeartBeatEntryAsync(utype);

        public string GenerateUuid() => this.Uuid;

        public TimeSpan OperationTimeout { get; set; }
    }
}
