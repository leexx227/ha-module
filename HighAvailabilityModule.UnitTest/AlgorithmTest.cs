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
    public class AlgorithmTest
    {
        private static TimeSpan Timeout => TimeSpan.FromSeconds(1.5);

        private static TimeSpan Interval => TimeSpan.FromSeconds(0.5);

        private static DateTime Now { get; } = DateTime.Parse("2019-09-27T12:00:00.2965246Z");

        private static string Client1Uuid => "cdca5b45-6ea1-4d91-81f6-d39f4821e791";

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
        public async Task RunningAsPrimaryTest1()
        {
            this.server.Current = null;
            Assert.IsFalse(this.algo.RunningAsPrimary(Now));
        }

        [TestMethod]
        public async Task RunningAsPrimaryTest2()
        {
            this.server.Current = null;
            await this.algo.CheckPrimaryAsync(Now);
            Assert.IsFalse(this.algo.RunningAsPrimary(Now));
        }

        [TestMethod]
        public async Task RunningAsPrimaryTest3()
        {
            this.server.Current = new HeartBeatEntry(Client1Uuid, Now);
            await this.algo.CheckPrimaryAsync(Now);
            Assert.IsTrue(this.algo.RunningAsPrimary(Now));
        }

        [TestMethod]
        public async Task RunningAsPrimaryTest4()
        {
            DateTime now = DateTime.UtcNow;
            this.server.Current = new HeartBeatEntry(Client1Uuid, now - Timeout);
            await this.algo.CheckPrimaryAsync(now);
            Assert.IsFalse(this.algo.RunningAsPrimary(now));
        }

        [TestMethod]
        public async Task RunningAsPrimaryTest5()
        {
            this.server.Current = new HeartBeatEntry(Client1Uuid, Now);
            await this.algo.CheckPrimaryAsync(Now - Timeout + Interval);
            Assert.IsFalse(this.algo.RunningAsPrimary(Now));
        }

        [TestMethod]
        public async Task RunningAsPrimaryTest6()
        {
            this.server.Current = new HeartBeatEntry(Client1Uuid, Now);
            await this.algo.CheckPrimaryAsync(Now);
            await this.algo.CheckPrimaryAsync(Now - Timeout + Interval);
            Assert.IsTrue(this.algo.RunningAsPrimary(Now));
        }

        [TestMethod]
        public async Task RunningAsPrimaryTest7()
        {
            this.server.Current = new HeartBeatEntry(Client1Uuid, Now);
            Assert.IsFalse(this.algo.RunningAsPrimary(Now));
        }

        [TestMethod]
        public async Task HeartBeatAsPrimaryTest1()
        {
            await this.algo.CheckPrimaryAsync(Now);
            await this.algo.HeartBeatAsPrimaryAsync();
            Assert.IsTrue(this.server.Current != null);
            Assert.IsTrue(this.server.Current.Uuid == Client1Uuid);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task HeartBeatAsPrimaryTest2()
        {
            await this.algo.HeartBeatAsPrimaryAsync();
        }

        [TestMethod]
        public async Task HeartBeatAndCheckTest1()
        {
            await this.algo.CheckPrimaryAsync(DateTime.UtcNow);
            await this.algo.HeartBeatAsPrimaryAsync();
            await this.algo.CheckPrimaryAsync(DateTime.UtcNow);
            Assert.IsTrue(this.algo.RunningAsPrimary(DateTime.UtcNow));
        }
    }
}