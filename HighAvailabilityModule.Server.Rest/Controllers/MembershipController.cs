namespace HighAvailabilityModule.Server.Rest.Controllers
{
    using System.Threading.Tasks;

    using HighAvailabilityModule.Interface;

    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class MembershipController : ControllerBase, IMembership
    {
        private IMembership membershipImpl;

        public MembershipController(IMembership membershipImplementation)
        {
            this.membershipImpl = membershipImplementation;
        }

        [HttpGet("ping")]
        public async Task<bool> Ping()
        {
            return true;
        }

        [HttpPost("heartbeat/{uuid}")]
        public async Task HeartBeatAsync(string uuid, string utype, [FromBody] HeartBeatEntry lastSeenEntry)
        {
            await this.membershipImpl.HeartBeatAsync(uuid, utype, lastSeenEntry);
        }

        [HttpGet("heartbeat")]
        public async Task<HeartBeatEntry> GetHeartBeatEntryAsync(string utype) => await this.membershipImpl.GetHeartBeatEntryAsync(utype);
    }
}