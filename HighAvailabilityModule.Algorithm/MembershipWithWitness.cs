namespace HighAvailabilityModule.Algorithm
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using HighAvailabilityModule.Interface;

    public class MembershipWithWitness
    {
        private string Uuid { get; }

        private IMembershipClient Client { get; }

        private TimeSpan HeartBeatInterval { get; }

        private int TimeoutTolerance { get; }

        private HeartBeatEntry lastSeenHeartBeat = null;

        public MembershipWithWitness(IMembershipClient client, TimeSpan heartBeatInterval, int timeoutTolerance)
        {
            this.Client = client;
            this.Client.OperationTimeout = heartBeatInterval;
            this.Uuid = client.GenerateUuid();
            this.HeartBeatInterval = heartBeatInterval;
            this.TimeoutTolerance = timeoutTolerance;
        }

        public async Task RunAsync(Func<Task> onStartAsync, Func<Task> onErrorAsync)
        {
            await this.GetPrimary();
            await onStartAsync();
            await this.KeepPrimary();
            await onErrorAsync();
        }

        private async Task GetPrimary()
        {
            while (!this.RunningAsPrimary)
            {
                await Task.Delay(this.HeartBeatInterval);
                try
                {
                    this.lastSeenHeartBeat = await this.Client.GetHeartBeatEntryAsync();
                }
                catch (Exception ex)
                {
                    Trace.TraceWarning($"[{nameof(this.GetPrimary)}] Error occured when getting heartbeat entry: {ex.ToString()}");
                }

                if (!this.PrimaryUp)
                {
                    try
                    {
                        await this.Client.SendHeartBeatAsync(this.Uuid, this.lastSeenHeartBeat);
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceWarning($"[{nameof(this.GetPrimary)}] Error occured when updating heartbeat entry: {ex.ToString()}");
                    }
                }
            }
        }


        /// <summary>
        /// Checks if current process is primary process
        /// </summary>
        private async Task KeepPrimary()
        {
            int retryCount = 0;

            while (retryCount < this.TimeoutTolerance)
            {
                this.Client.SendHeartBeatAsync(this.Uuid, this.lastSeenHeartBeat);

                try
                {
                    this.lastSeenHeartBeat = await this.Client.GetHeartBeatEntryAsync();
                    retryCount = 0;
                }
                catch (Exception ex)
                {
                    Trace.TraceWarning($"[{nameof(this.KeepPrimary)}] Error occured when getting heartbeat entry: {ex.ToString()}");
                    retryCount++;
                }

                if (!this.RunningAsPrimary)
                {
                    return;
                }
            }
        }

        private bool PrimaryUp => this.lastSeenHeartBeat == null || !this.lastSeenHeartBeat.IsEmpty;

        private bool RunningAsPrimary => this.PrimaryUp && this.lastSeenHeartBeat.Uuid == this.Uuid;
    }
}
