namespace HighAvailabilityModule.E2ETest.TestCases
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using HighAvailabilityModule.Interface;

    public class TestServer : IMembership
    {
        public TestServer(IMembership impl)
        {
            this.membershipImplementation = impl;
        }

        private readonly IMembership membershipImplementation;

        public async Task HeartBeatAsync(HeartBeatEntryDTO entryDTO)
        {
            await this.membershipImplementation.HeartBeatAsync(entryDTO);
        }

        public async Task<HeartBeatEntry> GetHeartBeatEntryAsync(string utype) => await this.membershipImplementation.GetHeartBeatEntryAsync(utype);
    }
}
