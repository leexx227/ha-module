namespace HighAvailabilityModule.Interface
{
    using System.Threading.Tasks;

    public interface IMembership
    {
        Task SendHeartBeatAsync(string uuid, HeartBeatEntry lastSeenEntry);

        Task<HeartBeatEntry> GetHeartBeatEntryAsync();
    }
}
