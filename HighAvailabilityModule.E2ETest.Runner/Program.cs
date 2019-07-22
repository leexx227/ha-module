namespace HighAvailabilityModule.E2ETest.Runner
{
    using System;
    using System.Threading.Tasks;
    using System.Diagnostics;

    using HighAvailabilityModule.Client.Rest;
    using HighAvailabilityModule.Client.SQL;
    using HighAvailabilityModule.E2ETest.TestCases;
    using HighAvailabilityModule.Interface;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            string clientType;
            string testType;
            string conStr;

            if (args.Length < 3)
            {
                Console.WriteLine("Please give the test client type(rest/sql) and test type(basic/chaos).");
                return;
            }
            else
            {
                clientType = args[1];
                testType = args[2];

                string logFileName = "LogFile_" + clientType + "_" + testType + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                Trace.Listeners.Add(new TextWriterTraceListener(System.IO.File.CreateText(logFileName)));
                Trace.WriteLine($"Test client type: {clientType}");
                Trace.WriteLine($"Test type: {testType}");
            }

            if (clientType == "rest")
            {
                var judge = new RestMembershipClient();
                if (testType == "basic")
                {
                    var basictest = new BasicTest(RestMembershipClient.CreateNew, judge);
                    await basictest.Start();
                }
                else if (testType == "chaos")
                {
                    var chaostest = new ChaosTest(RestMembershipClient.CreateNew, judge);
                    await chaostest.Start();
                }
                else
                {
                    Console.WriteLine("Please give the supported test type.(basic/chaos)");
                    return;
                }
            }
            else if (clientType == "sql")
            {
                conStr = "Data Source=10.0.0.4;User ID= hpcadmin;pwd=!!123abc!!123abc;database=HighAvailabilityWitness;Connect Timeout=30";

                var judge = new SQLMembershipClient(conStr);
                if (testType == "basic")
                {
                    var basictest = new BasicTest((utype, uname, timeout) => SQLMembershipClient.CreateNew(utype, uname, timeout, conStr), judge);
                    await basictest.Start();
                }
                else if (testType == "chaos")
                {
                    var chaostest = new ChaosTest((utype, uname, timeout) => SQLMembershipClient.CreateNew(utype, uname, timeout, conStr), judge);
                    await chaostest.Start();
                }
                else
                {
                    Console.WriteLine("Please give the supported test type.(basic/chaos)");
                    return;
                }
            }
            else
            {
                Console.WriteLine("Please give the supported test client type.(rest/sql)");
                return;
            }
        }
    }
}