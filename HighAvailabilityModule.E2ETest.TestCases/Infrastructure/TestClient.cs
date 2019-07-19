namespace HighAvailabilityModule.E2ETest.TestCases.Infrastructure
{
    using System;
    using System.ComponentModel.Design;
    using System.Threading.Tasks;

    using HighAvailabilityModule.Interface;

    public class TestClient : IMembershipClient
    {
        public TestClient(IMembershipClient impl, NetworkConfiguration net = null)
        {
            this.membershipClientImplementation = impl;
            if (net == null)
            {
                this.net = NetworkConfiguration.Reliable;
            }
            else
            {
                this.net = net;
            }
        }

        private readonly IMembershipClient membershipClientImplementation;

        private readonly NetworkConfiguration net;

        private bool MessageLost => new Random().NextDouble() < this.net.MessageLostRate;

        public async Task HeartBeatAsync(HeartBeatEntryDTO entryDTO)
        {
            if (this.MessageLost)
            {
                await Task.Delay(this.membershipClientImplementation.OperationTimeout);
                throw new TimeoutException();
            }

            await this.membershipClientImplementation.HeartBeatAsync(entryDTO);
        }

        public async Task<HeartBeatEntry> GetHeartBeatEntryAsync(string utype)
        {
            if (this.MessageLost)
            {
                await Task.Delay(this.membershipClientImplementation.OperationTimeout);
                throw new TimeoutException();
            }

            var res = await this.membershipClientImplementation.GetHeartBeatEntryAsync(utype);

            if (this.MessageLost)
            {
                await Task.Delay(this.membershipClientImplementation.OperationTimeout);
                throw new TimeoutException();
            }

            return res;
        }

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