namespace HighAvailabilityModule.E2ETest.Runner
{
    using System.Threading.Tasks;

    using HighAvailabilityModule.Client.Rest;
    using HighAvailabilityModule.E2ETest.TestCases;

    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var judge = new RestMembershipClient();
     
            var basictest = new BasicTest(RestMembershipClient.CreateNew, judge);
            await basictest.Start();
        }
    }
}
