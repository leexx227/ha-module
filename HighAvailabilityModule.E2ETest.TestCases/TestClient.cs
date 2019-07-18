namespace HighAvailabilityModule.E2ETest.TestCases
{
    using System;
    using System.Threading.Tasks;

    using HighAvailabilityModule.Interface;

    public class TestClient : IMembershipClient
    {
        public TestClient(IMembershipClient impl)
        {
            this.membershipClientImplementation = impl;
        }

        private readonly IMembershipClient membershipClientImplementation;

        public async Task HeartBeatAsync(HeartBeatEntryDTO entryDTO)
        {
            await this.membershipClientImplementation.HeartBeatAsync(entryDTO);
        }

        public async Task<HeartBeatEntry> GetHeartBeatEntryAsync(string utype) => await this.membershipClientImplementation.GetHeartBeatEntryAsync(utype);

        public string GenerateUuid() => this.membershipClientImplementation.GenerateUuid();

        public string Utype => this.membershipClientImplementation.Utype;

        public string Uname => this.membershipClientImplementation.Uname;

        public TimeSpan OperationTimeout
        {
            get => this.membershipClientImplementation.OperationTimeout;
            set => this.membershipClientImplementation.OperationTimeout = value;
        }
    }
}
