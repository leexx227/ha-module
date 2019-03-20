namespace HighAvailabilityModule.Interface
{
    using System;

    public interface IMembershipClient : IMembership
    {
        string GenerateUuid();

        TimeSpan OperationTimeout { get; set; }
    }
}
