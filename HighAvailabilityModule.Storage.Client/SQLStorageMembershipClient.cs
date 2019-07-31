namespace HighAvailabilityModule.Storage.Client
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using System.Data;
    using System.Data.SqlClient;

    using HighAvailabilityModule.Interface;
    using System.Diagnostics.Tracing;
    using System.Runtime.CompilerServices;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System.Diagnostics;

    public class SQLStorageMembershipClient : IMembershipStorageClient
    {
        public string ConStr { get; set; }

        public TimeSpan OperationTimeout { get; set; }

        private string timeFormat = "yyyy-MM-dd HH:mm:ss.fff";

        private static string DefaultTime = "1753-01-01 12:00:00.000";

        private const string GetDataEntrySpName = "dbo.GetDataEntry";

        private const string SetDataEntrySpName = "dbo.SetDataEntry";

        private const string DeleteDataEntrySpName = "dbo.DeleteDataEntry";

        private const string EnumerateDataEntrySpName = "dbo.EnumerateDataEntry";

        private const string GetDataTimeSpName = "dbo.GetDataTime";

        public SQLStorageMembershipClient(string conStr, TimeSpan operationTimeout)
        {
            this.OperationTimeout = operationTimeout;
            this.ConStr = (conStr.IndexOf("Connect Timeout") == -1 ? conStr: conStr.Substring(0, conStr.IndexOf("Connect Timeout"))) 
                + "Connect Timeout=" + Convert.ToInt32(Math.Ceiling(this.OperationTimeout.TotalSeconds)).ToString(); ;
        }

        public async Task <(string value, string type)> GetDataEntryAsync(string path, string key)
        {
            string value;
            string type;
            SqlConnection con = new SqlConnection(this.ConStr);
            string StoredProcedure = GetDataEntrySpName;
            SqlCommand comStr = new SqlCommand(StoredProcedure, con);
            comStr.CommandType = CommandType.StoredProcedure;
            comStr.CommandTimeout = Convert.ToInt32(Math.Ceiling(this.OperationTimeout.TotalSeconds));

            comStr.Parameters.Add("@dpath", SqlDbType.NVarChar).Value = path;
            comStr.Parameters.Add("@dkey", SqlDbType.NVarChar).Value = key;

            try
            {
                await con.OpenAsync();
                SqlDataReader ReturnedValue = await comStr.ExecuteReaderAsync();
                if (ReturnedValue.HasRows)
                {
                    ReturnedValue.Read();
                    value = ReturnedValue[0].ToString();
                    type = ReturnedValue[1].ToString();
                    ReturnedValue.Close();
                }
                else
                {
                    value = string.Empty;
                    type = string.Empty;
                }
                return (value, type);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Error occured when getting data entry: {ex.ToString()}");
                throw new InvalidOperationException($"Error occured when getting data entry: {ex.ToString()}");
            }
            finally
            {
                con.Close();
                con.Dispose();
                comStr.Dispose();
            }
        }

        public async Task<Guid> TryGetGuid(string path, string key)
        {
            var getDataEntry = await GetDataEntryAsync(path, key);
            string value = getDataEntry.Item1;
            string type = getDataEntry.Item2;

            if (type == "System.Guid")
            {
                return Guid.Parse(value);
            }
            else
            {
                throw new InvalidOperationException("Input value is not Guid.");
            }
        }

        public async Task<string> TryGetString(string path, string key)
        {
            var getDataEntry = await GetDataEntryAsync(path, key);
            string value = getDataEntry.Item1;
            string type = getDataEntry.Item2;

            if (type == "System.String")
            {
                return value.ToString();
            }
            else
            {
                throw new InvalidOperationException("Input value is not string.");
            }
        }

        public async Task<int> TryGetInt(string path, string key)
        {
            var getDataEntry = await GetDataEntryAsync(path, key);
            string value = getDataEntry.Item1;
            string type = getDataEntry.Item2;

            if (type == "System.Int32")
            {
                return Int32.Parse(value); 
            }
            else
            {
                throw new InvalidOperationException("Input value is not int.");
            }
        }

        public async Task<long> TryGetLong(string path, string key)
        {
            var getDataEntry = await GetDataEntryAsync(path, key);
            string value = getDataEntry.Item1;
            string type = getDataEntry.Item2;

            if (type == "System.Int64")
            {
                return Int64.Parse(value);
            }
            else
            {
                throw new InvalidOperationException("Input value is not long.");
            }
        }

        public async Task<double> TryGetDouble(string path, string key)
        {
            var getDataEntry = await GetDataEntryAsync(path, key);
            string value = getDataEntry.Item1;
            string type = getDataEntry.Item2;

            if (type == "System.Double")
            {
                return Double.Parse(value);
            }
            else
            {
                throw new InvalidOperationException("Input value is not double.");
            }
        }

        public async Task<string[]> TryGetStringArray(string path, string key)
        {
            var getDataEntry = await GetDataEntryAsync(path, key);
            string value = getDataEntry.Item1;
            string type = getDataEntry.Item2;

            if (type == "System.String[]")
            {
                return value.Split(",");
            }
            else
            {
                throw new InvalidOperationException("Input value is not string[].");
            }
        }

        public async Task<byte[]> TryGetByteArray(string path, string key)
        {
            var getDataEntry = await GetDataEntryAsync(path, key);
            string valueTmp = getDataEntry.Item1;
            string type = getDataEntry.Item2;

            if (type == "System.Byte[]")
            {
                string[] s = valueTmp.Split(",");
                byte[] value = new byte[s.Length];
                for (int i=0; i<s.Length; i++)
                {
                    value[i] = byte.Parse(s[i]);
                }
                return value;
            }
            else
            {
                throw new InvalidOperationException("Input value is not byte[].");
            }
        }

        private async Task<DateTime> GetDataTimeAsync(string path, string key)
        {
            string lastOperationTime;
            SqlConnection con = new SqlConnection(this.ConStr);
            string StoredProcedure = GetDataTimeSpName;
            SqlCommand comStr = new SqlCommand(StoredProcedure, con);
            comStr.CommandType = CommandType.StoredProcedure;
            comStr.CommandTimeout = Convert.ToInt32(Math.Ceiling(this.OperationTimeout.TotalSeconds));

            comStr.Parameters.Add("@dpath", SqlDbType.NVarChar).Value = path;
            comStr.Parameters.Add("@dkey", SqlDbType.NVarChar).Value = key;

            try
            {
                await con.OpenAsync();
                SqlDataReader ReturnedValue = await comStr.ExecuteReaderAsync();
                if (ReturnedValue.HasRows)
                {
                    ReturnedValue.Read();
                    lastOperationTime = ReturnedValue[0].ToString();
                    ReturnedValue.Close();
                }
                else
                {
                    lastOperationTime = DefaultTime;
                }
                return Convert.ToDateTime(Convert.ToDateTime(lastOperationTime).ToString(this.timeFormat));
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Error occured when getting last operation time: {ex.ToString()}");
                throw new InvalidOperationException($"Error occured when getting last operation time: {ex.ToString()}");
            }
            finally
            {
                con.Close();
                con.Dispose();
                comStr.Dispose();
            }
        }

        public async Task SetDataEntryAsync(string path, string key, string value, string type)
        {
            DateTime lastOperationTime = await GetDataTimeAsync(path, key);

            SqlConnection con = new SqlConnection(this.ConStr);
            string StoredProcedure = SetDataEntrySpName;
            SqlCommand comStr = new SqlCommand(StoredProcedure, con);
            comStr.CommandType = CommandType.StoredProcedure;
            comStr.CommandTimeout = Convert.ToInt32(Math.Ceiling(this.OperationTimeout.TotalSeconds));

            comStr.Parameters.Add("@dpath", SqlDbType.NVarChar).Value = path;
            comStr.Parameters.Add("@dkey", SqlDbType.NVarChar).Value = key;
            comStr.Parameters.Add("@dvalue", SqlDbType.NVarChar).Value = value;
            comStr.Parameters.Add("@dtype", SqlDbType.NVarChar).Value = type;
            comStr.Parameters.Add("@lastOperationTime", SqlDbType.NVarChar).Value = lastOperationTime;

            try
            {
                await con.OpenAsync();
                await comStr.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Error occured when setting data entry: {ex.ToString()}");
                throw new InvalidOperationException($"Error occured when setting data entry: {ex.ToString()}");
            }
            finally
            {
                con.Close();
                con.Dispose();
                comStr.Dispose();
            }
        }

        public async Task SetGuid(string path, string key, Guid value)
        {
            await SetDataEntryAsync(path, key, value.ToString(), "System.Guid");
        }

        public async Task SetString(string path, string key, string value)
        {
            await SetDataEntryAsync(path, key, value, "System.String");
        }

        public async Task SetInt(string path, string key, int value)
        {
            await SetDataEntryAsync(path, key, value.ToString(), "System.Int32");
        }

        public async Task SetLong(string path, string key, long value)
        {
            await SetDataEntryAsync(path, key, value.ToString(), "System.Int64");
        }

        public async Task SetDouble(string path, string key, double value)
        {
            await SetDataEntryAsync(path, key, value.ToString(), "System.Double");
        }

        public async Task SetStringArray(string path, string key, string[] value)
        {
            await SetDataEntryAsync(path, key, string.Join(",", value), "System.String[]");
        }

        public async Task SetByteArray(string path, string key, byte[] value)
        {
            await SetDataEntryAsync(path, key, string.Join(",", value), "System.Byte[]");
        }

        public async Task DeleteDataEntry(string path, string key)
        {
            SqlConnection con = new SqlConnection(this.ConStr);
            string StoredProcedure = DeleteDataEntrySpName;
            SqlCommand comStr = new SqlCommand(StoredProcedure, con);
            comStr.CommandType = CommandType.StoredProcedure;
            comStr.CommandTimeout = Convert.ToInt32(Math.Ceiling(this.OperationTimeout.TotalSeconds));

            comStr.Parameters.Add("@dpath", SqlDbType.NVarChar).Value = path;
            comStr.Parameters.Add("@dkey", SqlDbType.NVarChar).Value = key;

            try
            {
                await con.OpenAsync();
                await comStr.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Error occured when deleting data entry: {ex.ToString()}");
                throw new InvalidOperationException($"Error occured when deleting data entry: {ex.ToString()}");
            }
            finally
            {
                con.Close();
                con.Dispose();
                comStr.Dispose();
            }
        }

        public async Task<List<string>> EnumerateDataEntryAsync(string path)
        {
            List<string> keyList = new List<string>();

            SqlConnection con = new SqlConnection(this.ConStr);
            string StoredProcedure = EnumerateDataEntrySpName;
            SqlCommand comStr = new SqlCommand(StoredProcedure, con);
            comStr.CommandType = CommandType.StoredProcedure;
            comStr.CommandTimeout = Convert.ToInt32(Math.Ceiling(this.OperationTimeout.TotalSeconds));

            comStr.Parameters.Add("@dpath", SqlDbType.NVarChar).Value = path;

            try
            {
                await con.OpenAsync();
                SqlDataReader ReturnedValue = await comStr.ExecuteReaderAsync();
                if (ReturnedValue.HasRows)
                {
                    while (ReturnedValue.Read())
                    {
                        keyList.Add(ReturnedValue[0].ToString());
                    }
                    ReturnedValue.Close();
                }
                return keyList;
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Error occured when enumerating data entry: {ex.ToString()}");
                throw new InvalidOperationException($"Error occured when enumerating data entry: {ex.ToString()}");
            }
            finally
            {
                con.Close();
                con.Dispose();
                comStr.Dispose();
            }
        }

        public async Task Monitor(string path, string key, TimeSpan interval, Action<string, string> callback)
        {
            string lastSeenValue = string.Empty;
            string lastSeenType = string.Empty;
            while (true)
            {
                try
                {
                    var getEntry = await GetDataEntryAsync(path, key);
                    string value = getEntry.Item1;
                    string type = getEntry.Item2;

                    if (DataChanged(value, type, lastSeenValue, lastSeenType))
                    {
                        callback(value, type);
                        lastSeenValue = value;
                        lastSeenType = type;
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError($"Error occured {ex.ToString()}");
                    throw;
                }
                finally
                {
                    await Task.Delay(interval);
                }
            }
        }

        public void Callback(string value, string type)
        {
            Console.WriteLine($"[Monitor] Value: {value}    Type: {type}");
        }

        private bool DataChanged(string value, string type, string lastSeenValue, string lastSeenType)
        {
            return lastSeenValue != value
                || lastSeenType != type
                || (lastSeenValue == string.Empty && value != string.Empty)
                || (lastSeenType == string.Empty && type != string.Empty);
        }
    }
}