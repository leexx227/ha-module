namespace HighAvailabilityModule.Interface
{
    using System.Threading.Tasks;

    public interface IMembership
    {
        Task HeartBeatAsync(string uuid, HeartBeatEntry lastSeenEntry);

        Task<HeartBeatEntry> GetHeartBeatEntryAsync();
    }
}
