namespace HighAvailabilityModule.Client.InMemory
{
    using System;
    using System.Threading.Tasks;

    using HighAvailabilityModule.Interface;

    public class InMemoryMembershipClient : IMembershipClient
    {
        public InMemoryMembershipClient(IMembership membershipServer) : this(membershipServer, Guid.NewGuid().ToString())
        {
        }

        public InMemoryMembershipClient(IMembership membershipServer, string uuid)
        {
            this.serverImplementation = membershipServer;
            this.Uuid = uuid;
        }

        private IMembership serverImplementation;

        public string Uuid { get; }

        public Task HeartBeatAsync(string uuid, HeartBeatEntry lastSeenEntry) => this.serverImplementation.HeartBeatAsync(uuid, lastSeenEntry);

        public Task<HeartBeatEntry> GetHeartBeatEntryAsync() => this.serverImplementation.GetHeartBeatEntryAsync();

        public string GenerateUuid() => this.Uuid;

        public TimeSpan OperationTimeout { get; set; }
    }
}
