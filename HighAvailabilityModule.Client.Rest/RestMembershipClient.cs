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

        private string[] AllType = new string[] {"A","B"};

        public RestMembershipClient(TimeSpan operationTimeout)
        {
            this.httpClient = new HttpClient { Timeout = operationTimeout };
            this.impl = new RestClientImpl(this.httpClient);
            this.Uuid = Guid.NewGuid().ToString();
            this.Utype = this.GenerateUtypeImpl();
            //this.Utype = "A";
        }

        public string BaseUri
        {
            get => this.impl.BaseUrl;
            set => this.impl.BaseUrl = value;
        }

        public Task HeartBeatAsync(string uuid, string utype, HeartBeatEntry lastSeenEntry) => this.impl.HeartBeatAsync(uuid, utype, lastSeenEntry);

        public Task<HeartBeatEntry> GetHeartBeatEntryAsync(string utype) => this.impl.GetHeartBeatEntryAsync(utype);

        public string GenerateUuid() => this.Uuid;

        public string GenerateUtype() => this.Utype;

        public string GenerateUtypeImpl()
        {
            int length = AllType.Length;
            Random random = new Random();
            return AllType[random.Next(0,length)];
        }


        public TimeSpan OperationTimeout
        {
            get => this.httpClient.Timeout;
            set => this.httpClient.Timeout = value;
        }

    }
}