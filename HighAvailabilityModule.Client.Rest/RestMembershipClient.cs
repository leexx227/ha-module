namespace HighAvailabilityModule.Client.Rest
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using HighAvailabilityModule.Interface;

    public class RestMembershipClient : IMembershipClient
    {
        private RestClientImpl impl;

        private HttpClient httpClient;

        public RestMembershipClient(TimeSpan operationTimeout)
        {
            this.httpClient = new HttpClient { Timeout = operationTimeout };
            this.impl = new RestClientImpl(this.httpClient);
        }

        public string BaseUri
        {
            get => this.impl.BaseUrl;
            set => this.impl.BaseUrl = value;
        }

        public Task HeartBeatAsync(string uuid, HeartBeatEntry lastSeenEntry) => this.impl.HeartBeatAsync(uuid, lastSeenEntry);

        public Task<HeartBeatEntry> GetHeartBeatEntryAsync() => this.impl.GetHeartBeatEntryAsync();

        public string GenerateUuid() => Guid.NewGuid().ToString();

        public TimeSpan OperationTimeout
        {
            get => this.httpClient.Timeout;
            set => this.httpClient.Timeout = value;
        }
    }
}