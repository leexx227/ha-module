namespace HighAvailabilityModule.Interface
{
    using System.Threading.Tasks;

    public interface IMembership
    {
        Task HeartBeatAsync(string uuid, string utype, HeartBeatEntry lastSeenEntry);

        Task<HeartBeatEntry> GetHeartBeatEntryAsync(string utype);
    }
}
