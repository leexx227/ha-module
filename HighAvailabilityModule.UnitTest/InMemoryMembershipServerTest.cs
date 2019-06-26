namespace HighAvailabilityModule.UnitTest
{
    using System;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using HighAvailabilityModule.Interface;
    using HighAvailabilityModule.Server.InMemory;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class InMemoryMembershipServerTest
    {
        private static TimeSpan Timeout => TimeSpan.FromSeconds(1.5);

        private static DateTime Now { get; } = DateTime.Parse("2019-09-27T12:00:00.2965246Z");

        private static string Client1Uuid => "cdca5b45-6ea1-4d91-81f6-d39f4821e791";
        private static string ClientUtypeA => "A";
        private static string ClientUnum1 => "1";

        private static string Client2Uuid => "39a78df0-e101-49b9-8c56-ec2fea2e47df";
        private static string ClientUnum2 => "2";

        private static string Client3Uuid => "33253ab7-27b6-478c-a359-4eca7df83b80";
        private static string ClientUtypeB => "B";

        private InMemoryMembershipServer server;

        private void TestFunc(Dictionary<string, HeartBeatEntry> Current, string Uuid, string Utype, string Unum)
        {
            Assert.IsTrue(Current != null);
            Assert.IsTrue(Current.ContainsKey(Utype));
            Assert.IsTrue(Current[Utype] != null);
            Assert.IsTrue(Current[Utype].Uuid == Uuid);
            Assert.IsTrue(Current[Utype].Utype == Utype);
            Assert.IsTrue(Current[Utype].Unum == Unum);
        }

        [TestInitialize]
        public void Initialize()
        {
            this.server = new InMemoryMembershipServer(Timeout);
        }

        [TestMethod]
        public async Task HeartbeatTest1()
        {
            await this.server.HeartBeatAsync(new HeartBeatEntryDTO(Client1Uuid, ClientUtypeA, ClientUnum1, null));
            TestFunc(this.server.CurrentTable, Client1Uuid, ClientUtypeA, ClientUnum1);
        }

        [TestMethod]
        public async Task HeartbeatTest2()
        {
            await this.server.HeartBeatAsync(new HeartBeatEntryDTO(Client1Uuid, ClientUtypeA, ClientUnum1, null), Now - Timeout);
            await this.server.HeartBeatAsync(new HeartBeatEntryDTO(Client2Uuid, ClientUtypeA, ClientUnum2, await this.server.GetHeartBeatEntryAsync(ClientUtypeA, Now)), Now);
            TestFunc(this.server.CurrentTable, Client2Uuid, ClientUtypeA, ClientUnum2);
        }

        [TestMethod]
        public async Task HeartbeatTest3()
        {
            await this.server.HeartBeatAsync(new HeartBeatEntryDTO(Client1Uuid, ClientUtypeA, ClientUnum1, null), Now - Timeout + TimeSpan.FromSeconds(1));
            await this.server.HeartBeatAsync(new HeartBeatEntryDTO(Client2Uuid, ClientUtypeA, ClientUnum2, await this.server.GetHeartBeatEntryAsync(ClientUtypeA, Now)), Now);
            TestFunc(this.server.CurrentTable, Client1Uuid, ClientUtypeA, ClientUnum1);
        }

        [TestMethod]
        public async Task HeartbeatTest4()
        {
            await this.server.HeartBeatAsync(new HeartBeatEntryDTO(Client1Uuid, ClientUtypeA, ClientUnum1, null), Now - Timeout);
            await this.server.HeartBeatAsync(new HeartBeatEntryDTO(Client2Uuid, ClientUtypeA, ClientUnum2, null), Now);
            TestFunc(this.server.CurrentTable, Client1Uuid, ClientUtypeA, ClientUnum1);
        }

        [TestMethod]
        public async Task HeartbeatTest5()
        {
            await this.server.HeartBeatAsync(new HeartBeatEntryDTO(Client1Uuid, ClientUtypeA, ClientUnum1, null), Now - Timeout);
            HeartBeatEntry getEntry = await this.server.GetHeartBeatEntryAsync(ClientUtypeA, Now);
            var entry = new HeartBeatEntry(getEntry.Uuid, getEntry.Utype, getEntry.Unum, Now);
            await this.server.HeartBeatAsync(new HeartBeatEntryDTO(Client2Uuid, ClientUtypeA, ClientUnum2, entry), Now);
            TestFunc(this.server.CurrentTable, Client2Uuid, ClientUtypeA, ClientUnum2);
        }

        [TestMethod]
        public async Task HeartbeatTest6()
        {
            await this.server.HeartBeatAsync(new HeartBeatEntryDTO(Client1Uuid, ClientUtypeA, ClientUnum1, null), Now - TimeSpan.FromSeconds(1));
            await this.server.HeartBeatAsync(new HeartBeatEntryDTO(Client1Uuid, ClientUtypeA, ClientUnum1, await this.server.GetHeartBeatEntryAsync(ClientUtypeA, Now)), Now);
            TestFunc(this.server.CurrentTable, Client1Uuid, ClientUtypeA, ClientUnum1);
            Assert.IsTrue(this.server.CurrentTable[ClientUtypeA].TimeStamp == Now);
        }

        [TestMethod]
        public async Task HeartbeatTest7()
        {
            Task[] tasks = new[] { this.server.HeartBeatAsync(new HeartBeatEntryDTO(Client1Uuid, ClientUtypeA, ClientUnum1, null), Now),
                this.server.HeartBeatAsync(new HeartBeatEntryDTO(Client2Uuid, ClientUtypeA, ClientUnum2, null), Now) };
            await Task.WhenAll(tasks);
            TestFunc(this.server.CurrentTable, Client1Uuid, ClientUtypeA, ClientUnum1);
        }

        [TestMethod]
        public async Task HeartbeatTest8()
        {
            await this.server.HeartBeatAsync(new HeartBeatEntryDTO(Client1Uuid, ClientUtypeA, ClientUnum1, null), Now - TimeSpan.FromSeconds(1));
            var entry = await this.server.GetHeartBeatEntryAsync(ClientUtypeA, Now);
            Task[] tasks = new[] { this.server.HeartBeatAsync(new HeartBeatEntryDTO(Client1Uuid, ClientUtypeA, ClientUnum1, entry), Now),
                this.server.HeartBeatAsync(new HeartBeatEntryDTO(Client2Uuid, ClientUtypeA, ClientUnum2, entry), Now) };
            await Task.WhenAll(tasks);
            TestFunc(this.server.CurrentTable, Client1Uuid, ClientUtypeA, ClientUnum1);
        }

        [TestMethod]
        public async Task HeartbeatTest9()
        {
            await this.server.HeartBeatAsync(new HeartBeatEntryDTO(Client1Uuid, ClientUtypeA, ClientUnum1, null), Now - Timeout);
            var entry = await this.server.GetHeartBeatEntryAsync(ClientUtypeA, Now);
            Task[] tasks = new[] { this.server.HeartBeatAsync(new HeartBeatEntryDTO(Client2Uuid, ClientUtypeA, ClientUnum2, entry), Now),
                this.server.HeartBeatAsync(new HeartBeatEntryDTO(Client1Uuid, ClientUtypeA, ClientUnum1, entry), Now) };
            await Task.WhenAll(tasks);
            TestFunc(this.server.CurrentTable, Client2Uuid, ClientUtypeA, ClientUnum2);
        }

        [TestMethod]
        public async Task HeartbeatTest10()
        {
            await this.server.HeartBeatAsync(new HeartBeatEntryDTO(Client1Uuid, ClientUtypeA, ClientUnum1, null), Now);
            var entry1 = await this.server.GetHeartBeatEntryAsync(ClientUtypeA, Now);
            await this.server.HeartBeatAsync(new HeartBeatEntryDTO(Client3Uuid, ClientUtypeB, ClientUnum1, null), Now);
            var entry2 = await this.server.GetHeartBeatEntryAsync(ClientUtypeB, Now);
            TestFunc(this.server.CurrentTable, Client1Uuid, ClientUtypeA, ClientUnum1);
            TestFunc(this.server.CurrentTable, Client3Uuid, ClientUtypeB, ClientUnum1);
        }
    }
}