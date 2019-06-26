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

            Console.WriteLine("Uuid:{0}",client.Uuid);
            Console.WriteLine("Type:{0}",client.Utype);

            await algo.RunAsync(
                () => Task.Run(
                    async () =>
                        {
                            while (true)
                            {
                                Console.WriteLine($"Type:{client.Utype}. Running as primary. [{DateTime.UtcNow}]");
                                await Task.Delay(TimeSpan.FromSeconds(2));
                            }
                        }),
                null);
        }
    }
}