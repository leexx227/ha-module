// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
namespace HighAvailabilityModule.E2ETest.TestCases
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using HighAvailabilityModule.E2ETest.TestCases.Infrastructure;
    using HighAvailabilityModule.Interface;

    public class BasicTest
    {
        private readonly Func<string, string, TimeSpan, IMembershipClient> clientFactory;

        private readonly IMembershipClient judge;

        public BasicTest(Func<string, string, TimeSpan, IMembershipClient> clientFactory, IMembershipClient judge)
        {
            this.clientFactory = clientFactory;
            this.judge = judge;
        }

        public async Task Start()
        {
            AlgorithmController controller = new AlgorithmController(2, "A", TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(3), this.clientFactory, this.judge);
            Task.Run(controller.Start);
            await Task.Run(controller.WatchResult);
        }

        private async Task ShowLeader()
        {
            while (true)
            {
                Console.WriteLine(await this.judge.GetHeartBeatEntryAsync("A"));
                await Task.Delay(1000).ConfigureAwait(false);
            }
        }
    }
}
