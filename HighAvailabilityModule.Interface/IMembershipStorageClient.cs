namespace HighAvailabilityModule.Interface
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    public interface IMembershipStorageClient
    {
        TimeSpan OperationTimeout { get; set; }

        Task<(string, string)> GetDataEntryAsync(string path, string key);
        Task<Guid> TryGetGuid(string path, string key);
        Task<string> TryGetString(string path, string key);
        Task<int> TryGetInt(string path, string key);
        Task<long> TryGetLong(string path, string key);
        Task<double> TryGetDouble(string path, string key);
        Task<string[]> TryGetStringArray(string path, string key);
        Task<byte[]> TryGetByteArray(string path, string key);

        Task SetDataEntryAsync(string path, string key, string value, string type);
        Task SetGuid(string path, string key, Guid value);
        Task SetString(string path, string key, string value);
        Task SetInt(string path, string key, int value);
        Task SetLong(string path, string key, long value);
        Task SetDouble(string path, string key, double value);
        Task SetStringArray(string path, string key, string[] value);
        Task SetByteArray(string path, string key, byte[] value);

        Task DeleteDataEntry(string path, string key);

        Task<List<string>> EnumerateDataEntryAsync(string path);

        Task Monitor(string path, string key, Action<string,string> callback);
    }
}
