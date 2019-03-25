namespace HighAvailabilityModule.UnitTest
{
    using System;
    using System.Threading.Tasks;

    using HighAvailabilityModule.Algorithm;
    using HighAvailabilityModule.Client.InMemory;
    using HighAvailabilityModule.Interface;
    using HighAvailabilityModule.Server.InMemory;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class InMemoryMembershipServerTest
    {
        private static TimeSpan Timeout => TimeSpan.FromSeconds(1.5);

        private static TimeSpan Interval => TimeSpan.FromSeconds(0.5);

        private static DateTime Now { get; } = DateTime.Parse("2019-09-27T12:00:00.2965246Z");

        private static string Client1Uuid => "cdca5b45-6ea1-4d91-81f6-d39f4821e791";

        private static string Client2Uuid => "33253ab7-27b6-478c-a359-4eca7df83b80";

        private InMemoryMembershipServer server;

        private InMemoryMembershipClient client;

        private MembershipWithWitness algo;

        [TestInitialize]
        public void Initialize()
        {
            this.server = new InMemoryMembershipServer(Timeout);
            this.client = new InMemoryMembershipClient(this.server, Client1Uuid);
            this.algo = new MembershipWithWitness(this.client, Interval, Timeout);
        }

        [TestMethod]
        public async Task HeartbeatTest1()
        {
            await this.server.HeartBeatAsync(Client1Uuid, null);
            Assert.IsTrue(this.server.Current.Uuid == Client1Uuid);
        }

        [TestMethod]
        public async Task HeartbeatTest2()
        {
            await this.server.HeartBeatAsync(Client1Uuid, null, Now - Timeout);
            await this.server.HeartBeatAsync(Client2Uuid, this.server.Current, Now);
            Assert.IsTrue(this.server.Current.Uuid == Client2Uuid);
        }

        [TestMethod]
        public async Task HeartbeatTest3()
        {
            await this.server.HeartBeatAsync(Client1Uuid, null, Now - Timeout + TimeSpan.FromSeconds(1));
            await this.server.HeartBeatAsync(Client2Uuid, this.server.Current, Now);
            Assert.IsTrue(this.server.Current.Uuid == Client1Uuid);
        }


        [TestMethod]
        public async Task HeartbeatTest4()
        {
            await this.server.HeartBeatAsync(Client1Uuid, null, Now - Timeout);
            await this.server.HeartBeatAsync(Client2Uuid, null, Now);
            Assert.IsTrue(this.server.Current.Uuid == Client1Uuid);
        }

        [TestMethod]
        public async Task HeartbeatTest5()
        {
            await this.server.HeartBeatAsync(Client1Uuid, null, Now - Timeout);
            var entry =  new HeartBeatEntry(this.server.Current.Uuid, Now);
            await this.server.HeartBeatAsync(Client2Uuid, entry, Now);
            Assert.IsTrue(this.server.Current.Uuid == Client1Uuid);
        }

        [TestMethod]
        public async Task HeartbeatTest6()
        {
            await this.server.HeartBeatAsync(Client1Uuid, null, Now - TimeSpan.FromSeconds(1));
            await this.server.HeartBeatAsync(Client1Uuid, this.server.Current, Now);
            Assert.IsTrue(this.server.Current.Uuid == Client1Uuid);
            Assert.IsTrue(this.server.Current.TimeStamp == Now);
        }
    }
}
