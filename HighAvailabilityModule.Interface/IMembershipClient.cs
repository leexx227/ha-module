namespace HighAvailabilityModule.Interface
{
    using System;

    public interface IMembershipClient : IMembership
    {
        string GenerateUuid();

        string GenerateUtype();

        TimeSpan OperationTimeout { get; set; }
    }
}
