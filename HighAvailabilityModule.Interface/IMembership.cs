namespace HighAvailabilityModule.Interface
{
    using System.Threading.Tasks;

    public interface IMembership
    {
        Task HeartBeatAsync(HeartBeatEntryDTO entryDTO);

        Task<HeartBeatEntry> GetHeartBeatEntryAsync(string utype);
    }
}
