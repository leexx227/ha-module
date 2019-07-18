namespace HighAvailabilityModule.E2ETest.Runner
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using HighAvailabilityModule.Algorithm;
    using HighAvailabilityModule.Client.Rest;
    using HighAvailabilityModule.E2ETest.TestCases;

    public class Program
    {
        private static readonly TimeSpan Interval = TimeSpan.FromSeconds(1);

        private static readonly TimeSpan Timeout = Interval * 3;

        public static async Task Main(string[] args)
        {
            var judge = new RestMembershipClient();
            int clientCount = 2;
            MembershipWithWitness[] algos = new MembershipWithWitness[clientCount];

            for (int i = 0; i != clientCount; ++i)
            {
                algos[i] = BuildAlgoInstance("A", i.ToString());
            }

            Task.Run(
                async () =>
                    {
                        while (true)
                        {
                            Console.WriteLine(await judge.GetHeartBeatEntryAsync("A"));
                            await Task.Delay(1000);
                        }
                    });

            int k = 0;
            while (true)
            {
                ++k;
                k = k % clientCount;
                await CrashRestart(algos, k);
                await Task.Delay(5000);
            }
        }

        private static MembershipWithWitness BuildAlgoInstance(string utype, string uname)
        {
            TestClient client = new TestClient(new RestMembershipClient(utype, uname, Timeout));
            MembershipWithWitness algo = new MembershipWithWitness(client, Interval, Timeout);
            algo.RunAsync(null, null);
            return algo;
        }

        private static async Task CrashRestart(MembershipWithWitness[] algos, int num)
        {
            Console.WriteLine("Fail instance " + num);
            algos[num].Stop();
            await Task.Delay(1000);
            algos[num] = BuildAlgoInstance("A", num.ToString());
        }
    }
}
