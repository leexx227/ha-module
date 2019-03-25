namespace HighAvailabilityModule.Algorithm
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using HighAvailabilityModule.Interface;

    public class MembershipWithWitness
    {
        private string Uuid { get; }

        private IMembershipClient Client { get; }

        private TimeSpan HeartBeatInterval { get; }

        private TimeSpan HeartBeatTimeout { get; }

        private (HeartBeatEntry Entry, DateTime QueryTime) lastSeenHeartBeat;

        private object heartbeatLock = new object();

        public MembershipWithWitness(IMembershipClient client, TimeSpan heartBeatInterval, TimeSpan heartBeatTimeout)
        {
            this.Client = client;
            this.Client.OperationTimeout = heartBeatInterval;
            this.Uuid = client.GenerateUuid();
            this.HeartBeatInterval = heartBeatInterval;
            this.HeartBeatTimeout = heartBeatTimeout;
        }

        public async Task RunAsync(Func<Task> onStartAsync, Func<Task> onErrorAsync)
        {
            await this.GetPrimaryAsync();
            await onStartAsync();
            await this.KeepPrimaryAsync();
            await onErrorAsync();
        }

        internal async Task GetPrimaryAsync()
        {
            while (!this.RunningAsPrimary(DateTime.UtcNow))
            {
                await Task.Delay(this.HeartBeatInterval);
                await this.CheckPrimaryAsync(DateTime.UtcNow);

                if (!this.PrimaryUp)
                {
                    await this.HeartBeatAsPrimaryAsync();
                }
            }
        }

        internal async Task CheckPrimaryAsync(DateTime now)
        {
            try
            {
                var entry = await this.Client.GetHeartBeatEntryAsync();
                if (now > this.lastSeenHeartBeat.QueryTime)
                {
                    lock (this.heartbeatLock)
                    {
                        if (now > this.lastSeenHeartBeat.QueryTime)
                        {
                            this.lastSeenHeartBeat = (entry, now);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceWarning($"Error occured when getting heartbeat entry: {ex.ToString()}");
            }
        }

        internal async Task HeartBeatAsPrimaryAsync()
        {
            if (this.lastSeenHeartBeat.Entry == null)
            {
                throw new InvalidOperationException($"Can't send heartbeat before querying current primary.");
            }

            try
            {
                await this.Client.HeartBeatAsync(this.Uuid, this.lastSeenHeartBeat.Entry);
            }
            catch (Exception ex)
            {
                Trace.TraceWarning($"Error occured when updating heartbeat entry: {ex.ToString()}");
            }
        }

        /// <summary>
        /// Checks if current process is primary process
        /// </summary>
        private async Task KeepPrimaryAsync()
        {
            while (this.RunningAsPrimary(DateTime.UtcNow))
            {
                this.HeartBeatAsPrimaryAsync();
                this.CheckPrimaryAsync(DateTime.UtcNow);
                await Task.Delay(this.HeartBeatInterval);
            }
        }

        private bool PrimaryUp => this.lastSeenHeartBeat != default && !this.lastSeenHeartBeat.Entry.IsEmpty;

        internal bool RunningAsPrimary(DateTime now) => this.PrimaryUp && this.lastSeenHeartBeat.Entry.Uuid == this.Uuid && now - this.lastSeenHeartBeat.QueryTime < (this.HeartBeatTimeout - this.HeartBeatInterval);
    }
}
