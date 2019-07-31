namespace HighAvailablityModule.Storage.Sample.Client
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using HighAvailabilityModule.Storage.Client;
    class Program
    {
        static async Task Main(string[] args)
        {
            string conStr = "server=.;database=HighAvailabilityStorage;Trusted_Connection=SSPI;Connect Timeout=30";
            if (args.Length>1)
            {
                conStr = args[1];
            }

            var timeout = TimeSpan.FromSeconds(0.2);
            var interval = TimeSpan.FromSeconds(0.1);

            SQLStorageMembershipClient client = new SQLStorageMembershipClient(conStr, timeout);

            //Monitor
            string path = "local\\hpc";
            string keyA = "A";
            string keyB = "B";
            string value = "111";
            client.Monitor(path, keyA, interval, client.Callback);

            //SetMethod
            try
            {
                await client.SetString(path, keyA, value);
                await client.SetString(path, keyB, value);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured when setting data entry: {ex.ToString()}");
                throw;
            }
            finally
            {
                await Task.Delay(TimeSpan.FromSeconds(2));
            }

            //GetMethod
            try
            {
                var result = await client.TryGetString(path, keyA);
                Console.WriteLine($"Get value: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured when getting data entry: {ex.ToString()}");
                throw;
            }
            finally
            {
                await Task.Delay(TimeSpan.FromSeconds(2));
            }

            //EnumerateMethod
            try
            {
                List<string> getKey = new List<string>();
                getKey = await client.EnumerateDataEntryAsync(path);
                foreach (string k in getKey)
                {
                    Console.WriteLine($"Get key: {k}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured when enumerating data entry: {ex.ToString()}");
                throw;
            }
            finally
            {
                await Task.Delay(TimeSpan.FromSeconds(2));
            }


            //DeleteMethod
            try
            {
                await client.DeleteDataEntry(path, keyA);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured when deleting data entry: {ex.ToString()}");
                throw;
            }
            finally
            {
                await Task.Delay(TimeSpan.FromSeconds(2));
            }
        }
    }
}
