namespace HighAvailabilityModule.Client.InMemory
{
    using System;
    using System.Threading.Tasks;

    using HighAvailabilityModule.Interface;

    public class InMemoryMembershipClient : IMembershipClient
    {
        public InMemoryMembershipClient(IMembership membershipServer)
        {
            this.serverImplementation = membershipServer;
            this.Uuid = Guid.NewGuid().ToString();
        }

        private IMembership serverImplementation;

        public string Uuid { get; }

        public Task HeartBeatAsync(string uuid, HeartBeatEntry lastSeenEntry) => this.serverImplementation.HeartBeatAsync(uuid, lastSeenEntry);

        public Task<HeartBeatEntry> GetHeartBeatEntryAsync() => this.serverImplementation.GetHeartBeatEntryAsync();

        public string GenerateUuid() => this.Uuid;

        public TimeSpan OperationTimeout { get; set; }
    }
}
