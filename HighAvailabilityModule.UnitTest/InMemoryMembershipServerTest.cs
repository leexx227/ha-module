namespace HighAvailabilityModule.UnitTest
{
    using System;
    using System.Threading.Tasks;

    using HighAvailabilityModule.Interface;
    using HighAvailabilityModule.Server.InMemory;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class InMemoryMembershipServerTest
    {
        private static TimeSpan Timeout => TimeSpan.FromSeconds(1.5);

        private static DateTime Now { get; } = DateTime.Parse("2019-09-27T12:00:00.2965246Z");

        private static string Client1Uuid => "cdca5b45-6ea1-4d91-81f6-d39f4821e791";
        private static string Client1Utype => "A";

        private static string Client2Uuid => "39a78df0-e101-49b9-8c56-ec2fea2e47df";
        private static string Client2Utype => "A";

        private static string Client3Uuid => "33253ab7-27b6-478c-a359-4eca7df83b80";
        private static string Client3Utype => "B";

        private InMemoryMembershipServer server;

        [TestInitialize]
        public void Initialize()
        {
            this.server = new InMemoryMembershipServer(Timeout);
        }

        [TestMethod]
        public async Task HeartbeatTest1()
        {
            await this.server.HeartBeatAsync(Client1Uuid, Client1Utype, null);
            Assert.IsTrue(this.server.CurrentTable.ContainsKey(Client1Utype));
            Assert.IsTrue(this.server.CurrentTable[Client1Utype].Uuid == Client1Uuid);
            Assert.IsTrue(this.server.CurrentTable[Client1Utype].Utype == Client1Utype);
        }

        [TestMethod]
        public async Task HeartbeatTest2()
        {
            await this.server.HeartBeatAsync(Client1Uuid, Client1Utype, null, Now - Timeout);
            await this.server.HeartBeatAsync(Client2Uuid, Client2Utype, await this.server.GetHeartBeatEntryAsync(Client2Utype,Now), Now);
            Assert.IsTrue(this.server.CurrentTable.ContainsKey(Client2Utype));
            Assert.IsTrue(this.server.CurrentTable[Client2Utype].Uuid == Client2Uuid);
        }

        [TestMethod]
        public async Task HeartbeatTest3()
        {
            await this.server.HeartBeatAsync(Client1Uuid, Client1Utype, null, Now - Timeout + TimeSpan.FromSeconds(1));
            await this.server.HeartBeatAsync(Client2Uuid, Client2Utype, await this.server.GetHeartBeatEntryAsync(Client2Utype, Now), Now);
            Assert.IsTrue(this.server.CurrentTable.ContainsKey(Client1Utype));
            Assert.IsTrue(this.server.CurrentTable[Client1Utype].Uuid == Client1Uuid);
        }

        [TestMethod]
        public async Task HeartbeatTest4()
        {
            await this.server.HeartBeatAsync(Client1Uuid, Client1Utype, null, Now - Timeout);
            await this.server.HeartBeatAsync(Client2Uuid, Client2Utype, null, Now);
            Assert.IsTrue(this.server.CurrentTable.ContainsKey(Client1Utype));
            Assert.IsTrue(this.server.CurrentTable[Client1Utype].Uuid == Client1Uuid);
        }

        [TestMethod]
        public async Task HeartbeatTest5()
        {
            await this.server.HeartBeatAsync(Client1Uuid, Client1Utype, null, Now - Timeout);
            HeartBeatEntry GetEntry = await this.server.GetHeartBeatEntryAsync(Client1Utype, Now);
            var entry = new HeartBeatEntry(GetEntry.Uuid, GetEntry.Utype, Now);
            await this.server.HeartBeatAsync(Client2Uuid, Client2Utype, entry, Now);
            Assert.IsTrue(this.server.CurrentTable.ContainsKey(Client2Utype));
            Assert.IsTrue(this.server.CurrentTable[Client2Utype].Uuid == Client2Uuid);
        }

        [TestMethod]
        public async Task HeartbeatTest6()
        {
            await this.server.HeartBeatAsync(Client1Uuid, Client1Utype, null, Now - TimeSpan.FromSeconds(1));
            await this.server.HeartBeatAsync(Client1Uuid, Client1Utype, await this.server.GetHeartBeatEntryAsync(Client1Utype, Now), Now);
            Assert.IsTrue(this.server.CurrentTable.ContainsKey(Client1Utype));
            Assert.IsTrue(this.server.CurrentTable[Client1Utype].Uuid == Client1Uuid);
            Assert.IsTrue(this.server.CurrentTable[Client1Utype].Utype == Client1Utype);
            Assert.IsTrue(this.server.CurrentTable[Client1Utype].TimeStamp == Now);
        }

        [TestMethod]
        public async Task HeartbeatTest7()
        {
            Task[] tasks = new[] { this.server.HeartBeatAsync(Client1Uuid, Client1Utype, null, Now), this.server.HeartBeatAsync(Client2Uuid, Client2Utype, null, Now) };
            await Task.WhenAll(tasks);
            Assert.IsTrue(this.server.CurrentTable.ContainsKey(Client1Utype));
            Assert.IsTrue(this.server.CurrentTable[Client1Utype].Uuid == Client1Uuid);
        }

        [TestMethod]
        public async Task HeartbeatTest8()
        {
            await this.server.HeartBeatAsync(Client1Uuid, Client1Utype, null, Now - TimeSpan.FromSeconds(1));
            var entry = await this.server.GetHeartBeatEntryAsync(Client1Utype, Now);
            Task[] tasks = new[] { this.server.HeartBeatAsync(Client1Uuid, Client1Utype, entry, Now), this.server.HeartBeatAsync(Client2Uuid, Client2Utype, entry, Now) };
            await Task.WhenAll(tasks);
            Assert.IsTrue(this.server.CurrentTable[Client1Utype].Uuid == Client1Uuid);
        }

        [TestMethod]
        public async Task HeartbeatTest9()
        {
            await this.server.HeartBeatAsync(Client1Uuid, Client1Utype, null, Now - Timeout);
            var entry = await this.server.GetHeartBeatEntryAsync(Client1Utype, Now);
            Task[] tasks = new[] { this.server.HeartBeatAsync(Client2Uuid, Client2Utype, entry, Now), this.server.HeartBeatAsync(Client1Uuid, Client1Utype, entry, Now) };
            await Task.WhenAll(tasks);
            Assert.IsTrue(this.server.CurrentTable[Client2Utype].Uuid == Client2Uuid);
        }

        [TestMethod]
        public async Task HeartbeatTest10()
        {
            await this.server.HeartBeatAsync(Client1Uuid, Client1Utype, null, Now);
            var entry1 = await this.server.GetHeartBeatEntryAsync(Client1Utype, Now);
            await this.server.HeartBeatAsync(Client3Uuid, Client3Utype, null, Now);
            var entry2 = await this.server.GetHeartBeatEntryAsync(Client3Utype, Now);
            Assert.IsTrue(this.server.CurrentTable[Client1Utype].Uuid == Client1Uuid);
            Assert.IsTrue(this.server.CurrentTable[Client3Utype].Uuid == Client3Uuid);
        }
    }
}