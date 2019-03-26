namespace HighAvailabilityModule.Sample.RestClient
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using HighAvailabilityModule.Algorithm;
    using HighAvailabilityModule.Client.Rest;

    class Program
    {
        static async Task Main(string[] args)
        {
            // Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

            var interval = TimeSpan.FromSeconds(1);
            var timeout = TimeSpan.FromSeconds(5);

            RestMembershipClient client = new RestMembershipClient(interval);

            MembershipWithWitness algo = new MembershipWithWitness(client, interval, timeout);

            await algo.RunAsync(
                () => Task.Run(
                    async () =>
                        {
                            while (true)
                            {
                                Console.WriteLine($"Running as primary. [{DateTime.UtcNow}]");
                                await Task.Delay(TimeSpan.FromSeconds(2));
                            }
                        }),
                null);
        }
    }
}