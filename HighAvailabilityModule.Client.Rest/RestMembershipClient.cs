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

        public string Uuid { get; }

        public string Utype { get; }

        public string Uname { get; }

        public RestMembershipClient(string utype, string uname, TimeSpan operationTimeout)
        {
            this.httpClient = new HttpClient { Timeout = operationTimeout };
            this.impl = new RestClientImpl(this.httpClient);
            this.Uuid = Guid.NewGuid().ToString();
            this.Utype = utype;
            this.Uname = uname;
        }

        public string BaseUri
        {
            get => this.impl.BaseUrl;
            set => this.impl.BaseUrl = value;
        }

        public Task HeartBeatAsync(HeartBeatEntryDTO entryDTO) => this.impl.HeartBeatAsync(entryDTO);

        public Task<HeartBeatEntry> GetHeartBeatEntryAsync(string utype) => this.impl.GetHeartBeatEntryAsync(utype);

        public string GenerateUuid() => this.Uuid;

        public TimeSpan OperationTimeout
        {
            get => this.httpClient.Timeout;
            set => this.httpClient.Timeout = value;
        }
    }
}