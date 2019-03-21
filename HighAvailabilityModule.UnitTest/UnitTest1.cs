using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HighAvailabilityModule.UnitTest
{
    using System;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    using HighAvailabilityModule.Algorithm;
    using HighAvailabilityModule.Client.InMemory;
    using HighAvailabilityModule.Interface;
    using HighAvailabilityModule.Server.InMemory;

    [TestClass]
    public class UnitTest1
    {
        private static TimeSpan Timeout => TimeSpan.FromSeconds(1.5);

        private static TimeSpan Interval => TimeSpan.FromSeconds(0.5);

        private static int Tolerance => 5;

        private InMemoryMembershipServer server;

        private InMemoryMembershipClient client;

        private MembershipWithWitness algo;

        [TestInitialize]
        public void Initialize()
        {
            this.server = new InMemoryMembershipServer(Timeout);
            this.client = new InMemoryMembershipClient(this.server);
            this.algo = new MembershipWithWitness(this.client, Interval, Timeout);
        }

        [TestMethod]
        [Timeout(2000)]
        public async Task BasicTest()
        {
            this.server.Current = null;
            await this.algo.GetPrimaryAsync();
            Assert.AreEqual(this.client.Uuid, this.server.Current.Uuid);
        }
    }
}
