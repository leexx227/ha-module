namespace HighAvailabilityModule.E2ETest.TestCases.Infrastructure
{
    using System;
    using System.ComponentModel.Design;
    using System.Diagnostics;
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

        private async Task LoseMessage()
        {
            if (this.MessageLost)
            {
                Trace.TraceInformation($"Message Lost: {this.membershipClientImplementation.Utype} - {this.membershipClientImplementation.Uname}");
                Console.WriteLine($"Message Lost: {this.membershipClientImplementation.Utype} - {this.membershipClientImplementation.Uname}");

                await Task.Delay(this.membershipClientImplementation.OperationTimeout);
                throw new TimeoutException();
            }
        }

        public async Task HeartBeatAsync(HeartBeatEntryDTO entryDTO)
        {
            await this.LoseMessage();
            await this.membershipClientImplementation.HeartBeatAsync(entryDTO);
        }

        public async Task<HeartBeatEntry> GetHeartBeatEntryAsync(string utype)
        {
            await this.LoseMessage();
            var res = await this.membershipClientImplementation.GetHeartBeatEntryAsync(utype);
            await this.LoseMessage();
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