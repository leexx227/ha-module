namespace HighAvailabilityModule.Client.InMemory
{
    using System;
    using System.Threading.Tasks;

    using HighAvailabilityModule.Interface;

    public class InMemoryMembershipClient : IMembershipClient
    {
        public InMemoryMembershipClient(IMembership membershipServer) : this(membershipServer, Guid.NewGuid().ToString(), "A")
        {
        }

        public InMemoryMembershipClient(IMembership membershipServer, string uuid, string utype)
        {
            this.serverImplementation = membershipServer;
            this.Uuid = uuid;
            this.Utype = utype;
        }

        private IMembership serverImplementation;

        public string Uuid { get; }
        public string Utype { get; }

        public Task HeartBeatAsync(string uuid, string utype, HeartBeatEntry lastSeenEntry) => this.serverImplementation.HeartBeatAsync(uuid, utype, lastSeenEntry);

        public Task<HeartBeatEntry> GetHeartBeatEntryAsync(string utype) => this.serverImplementation.GetHeartBeatEntryAsync(utype);

        public string GenerateUuid() => this.Uuid;

        public string GenerateUtype() => this.Utype;

        public TimeSpan OperationTimeout { get; set; }
    }
}
