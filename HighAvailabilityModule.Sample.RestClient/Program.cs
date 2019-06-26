namespace HighAvailabilityModule.Sample.RestClient
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices.ComTypes;
    using System.Threading.Tasks;

    using HighAvailabilityModule.Algorithm;
    using HighAvailabilityModule.Client.Rest;

    class Program
    {
        static async Task Main(string[] args)
        {
            // Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            string utype;
            string unum;

            string[] AllType = new string[] { "A", "B" };

            if (args.Length != 0)
            {
                utype = args[1];
                if (utype == "query")
                    unum = "-1";
                else
                    unum = args[2];
            }
            else
            {
                Console.WriteLine("Please give the client's type and machine num!");
                return;
            }

            var interval = TimeSpan.FromSeconds(1);
            var timeout = TimeSpan.FromSeconds(5);

            RestMembershipClient client = new RestMembershipClient(utype, unum, interval);

            MembershipWithWitness algo = new MembershipWithWitness(client, interval, timeout);

            Console.WriteLine("Uuid:{0}",client.Uuid);
            Console.WriteLine("Type:{0}",client.Utype);
            Console.WriteLine("Machine Num:{0}", client.Unum);

            if (client.Utype == "query")
            {
                while (true)
                {
                    foreach (string qtype in AllType)
                    {
                        var primary = await client.GetHeartBeatEntryAsync(qtype);
                        if (!primary.IsEmpty)
                        {
                            Console.WriteLine($"[Query Result] Type:{primary.Utype}. Machine Num:{primary.Unum}. Running as primary. [{primary.TimeStamp}]");
                            await Task.Delay(TimeSpan.FromSeconds(2));
                        }
                    }
                }
            }
            else
            {
                await algo.RunAsync(
                () => Task.Run(
                    async () =>
                    {
                        while (true)
                        {
                            if (utype != "query")
                            {
                                Console.WriteLine($"Type:{client.Utype}. Machine Num:{client.Unum}. Running as primary. [{DateTime.UtcNow}]");
                                await Task.Delay(TimeSpan.FromSeconds(2));
                            }
                        }
                    }),
                null);
            }
        }
    }
}