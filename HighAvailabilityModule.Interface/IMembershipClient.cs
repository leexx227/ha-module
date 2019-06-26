namespace HighAvailabilityModule.Interface
{
    using System;

    public interface IMembershipClient : IMembership
    {
        string GenerateUuid();

        string Utype { get; set; }

        string Unum { get; set; }

        TimeSpan OperationTimeout { get; set; }
    }
}
