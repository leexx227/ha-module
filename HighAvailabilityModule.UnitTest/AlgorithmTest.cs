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

        private static string Client2Uuid => "33253ab7-27b6-478c-a359-4eca7df83b80";

        private InMemoryMembershipServer server;

        private InMemoryMembershipClient client;
        private InMemoryMembershipClient client2;

        private MembershipWithWitness algo;
        private MembershipWithWitness algo2;

        [TestInitialize]
        public void Initialize()
        {
            this.server = new InMemoryMembershipServer(Timeout);
            this.client = new InMemoryMembershipClient(this.server, Client1Uuid);
            this.algo = new MembershipWithWitness(this.client, Interval, Timeout);
            this.client2 = new InMemoryMembershipClient(this.server, Client2Uuid);
            this.algo2 = new MembershipWithWitness(this.client2, Interval, Timeout);
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

        [TestMethod]
        [Timeout(2000)]
        public async Task GetPrimaryTest1()
        {
            await this.algo.GetPrimaryAsync();
            Assert.IsTrue(this.algo.RunningAsPrimary(DateTime.UtcNow));
        }

        [TestMethod]
        [Timeout(2000)]
        public async Task GetPrimaryTest2()
        {
            var getPrimaryTask = this.algo.GetPrimaryAsync();
            this.algo.Stop();
            await Assert.ThrowsExceptionAsync<TaskCanceledException>(() => getPrimaryTask);
            Assert.IsFalse(this.algo.RunningAsPrimary(DateTime.UtcNow));
        }

        [TestMethod]
        [Timeout(5000)]
        public async Task GetPrimaryTest3()
        {
            await this.algo.GetPrimaryAsync();
            this.algo.KeepPrimaryAsync();
            this.algo2.GetPrimaryAsync();

            this.algo.Stop();
            this.algo2.Stop();

            Assert.IsTrue(this.algo.RunningAsPrimary(DateTime.UtcNow));
            Assert.IsFalse(this.algo2.RunningAsPrimary(DateTime.UtcNow));
        }

        [TestMethod]
        [Timeout(5000)]
        public async Task GetPrimaryTest4()
        {
            await this.algo.GetPrimaryAsync();
            await this.algo2.GetPrimaryAsync();

            this.algo.Stop();
            this.algo2.Stop();

            Assert.IsFalse(this.algo.RunningAsPrimary(DateTime.UtcNow));
            Assert.IsTrue(this.algo2.RunningAsPrimary(DateTime.UtcNow));
        }

        [TestMethod]
        [Timeout(5000)]
        public async Task GetPrimaryTest5()
        {
            await this.algo.GetPrimaryAsync();
            this.server.RemoveCurrent();
            await this.algo.KeepPrimaryAsync();

            Assert.IsFalse(this.algo.RunningAsPrimary(DateTime.UtcNow));
        }

        [TestMethod]
        [Timeout(5000)]
        public async Task GetPrimaryTest6()
        {
            await this.algo.GetPrimaryAsync();
            var task = this.algo.KeepPrimaryAsync();
            await Task.Delay(TimeSpan.FromSeconds(1));
            this.server.RemoveCurrent();
            await task;

            Assert.IsFalse(this.algo.RunningAsPrimary(DateTime.UtcNow));
        }
    }
}