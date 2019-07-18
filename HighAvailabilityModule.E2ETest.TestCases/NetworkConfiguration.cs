namespace HighAvailabilityModule.E2ETest.TestCases
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class NetworkConfiguration
    {
        public long LatencyLowerBound { get; set; } = 0;

        public long LatencyUpperBound { get; set; } = 0;

        public double MessageLostRate { get; set; } = 0;
    }
}
