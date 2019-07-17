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

        private string Utype { get; }

        private string Uname { get; }

        private IMembershipClient Client { get; }

        private TimeSpan HeartBeatInterval { get; }

        private TimeSpan HeartBeatTimeout { get; }

        private (HeartBeatEntry Entry, DateTime QueryTime) lastSeenHeartBeat;

        private object heartbeatLock = new object();

        private CancellationTokenSource AlgorithmCancellationTokenSource { get; } = new CancellationTokenSource();

        internal CancellationToken AlgorithmCancellationToken => this.AlgorithmCancellationTokenSource.Token;

        public MembershipWithWitness(IMembershipClient client, TimeSpan heartBeatInterval, TimeSpan heartBeatTimeout)
        {
            this.Client = client;
            this.Client.OperationTimeout = heartBeatInterval;
            this.Uuid = client.GenerateUuid();
            this.Utype = client.Utype;
            this.Uname = client.Uname;
            this.HeartBeatInterval = heartBeatInterval;
            this.HeartBeatTimeout = heartBeatTimeout;
        }

        public async Task RunAsync(Func<Task> onStartAsync, Func<Task> onErrorAsync)
        {
            await this.GetPrimaryAsync();
            if (onStartAsync != null)
            {
                onStartAsync();
            }

            await this.KeepPrimaryAsync();
            if (onErrorAsync != null)
            {
                await onErrorAsync();
            }
        }

        public void Stop()
        {
            this.AlgorithmCancellationTokenSource.Cancel();
            Trace.TraceWarning($"[{DateTime.UtcNow:O}][Protocol][{this.Uuid}] Algorithm stopped");
        }

        internal async Task GetPrimaryAsync()
        {
            var token = this.AlgorithmCancellationToken;
            while (!this.RunningAsPrimary(DateTime.UtcNow))
            {
                token.ThrowIfCancellationRequested();
                await Task.Delay(this.HeartBeatInterval, token);
                await this.CheckPrimaryAsync(DateTime.UtcNow);

                if (!this.PrimaryUp)
                {
                    Trace.TraceWarning($"[{DateTime.UtcNow:O}][Protocol][{this.Uuid}] Primary down");
                    await this.HeartBeatAsPrimaryAsync();
                }
            }
        }

        internal async Task CheckPrimaryAsync(DateTime now)
        {
            try
            {
                var entry = await this.Client.GetHeartBeatEntryAsync(this.Utype);
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

                Trace.TraceInformation($"[{DateTime.UtcNow:O}][Protocol][{this.Uuid}] lastSeenHeartBeat = {this.lastSeenHeartBeat.Entry.Uuid}, {this.lastSeenHeartBeat.Entry.TimeStamp:O}");
            }
            catch (Exception ex)
            {
                Trace.TraceWarning($"[{DateTime.UtcNow:O}][Protocol][{this.Uuid}] Error occured when getting heartbeat entry: {ex.ToString()}");
            }
        }

        internal async Task HeartBeatAsPrimaryAsync()
        {
            if (this.lastSeenHeartBeat.Entry == null)
            {
                throw new InvalidOperationException($"[Protocol][{this.Uuid}] Can't send heartbeat before querying current primary.");
            }

            try
            {
                var sendTime = DateTime.UtcNow;
                Trace.TraceInformation($"[{sendTime:O}][Protocol][{this.Uuid}] Sending heartbeat with UUID = {this.Uuid} at localtime {sendTime:O}, lastSeenHeartBeat = {this.lastSeenHeartBeat.Entry.Uuid}, {this.lastSeenHeartBeat.Entry.TimeStamp:O}");
                await this.Client.HeartBeatAsync(new HeartBeatEntryDTO (this.Uuid, this.Utype, this.Uname, this.lastSeenHeartBeat.Entry));
                Trace.TraceInformation($"[{DateTime.UtcNow:O}][Protocol][{this.Uuid}] Sending heartbeat with UUID = {this.Uuid} at localtime {sendTime:O} completed");

            }
            catch (Exception ex)
            {
                Trace.TraceWarning($"[{DateTime.UtcNow:O}][Protocol][{this.Uuid}] Error occured when updating heartbeat entry: {ex.ToString()}");
            }
        }

        /// <summary>
        /// Checks if current process is primary process
        /// </summary>
        internal async Task KeepPrimaryAsync()
        {
            var token = this.AlgorithmCancellationToken;
            while (this.RunningAsPrimary(DateTime.UtcNow))
            {
                token.ThrowIfCancellationRequested();
                this.HeartBeatAsPrimaryAsync();
                this.CheckPrimaryAsync(DateTime.UtcNow);
                await Task.Delay(this.HeartBeatInterval, token);
            }

            Trace.TraceWarning($"[{DateTime.UtcNow:O}][Protocol][{this.Uuid}] Lost Primary");
        }

        private bool PrimaryUp => this.lastSeenHeartBeat != default && !this.lastSeenHeartBeat.Entry.IsEmpty;

        internal bool RunningAsPrimary(DateTime now)
        {
            var primary = this.PrimaryUp && this.lastSeenHeartBeat.Entry.Uuid == this.Uuid && now - this.lastSeenHeartBeat.QueryTime < (this.HeartBeatTimeout - this.HeartBeatInterval);
            if (!primary)
            {
                Trace.TraceWarning(this.Dump() + $", UtcNow = {now:O}");
            }

            return primary;
        }

        public string Dump() => $"PrimaryUp = {this.PrimaryUp}, SelfUuid = {this.Uuid ?? string.Empty}, LastSeenUuid = {this.lastSeenHeartBeat.Entry?.Uuid ?? string.Empty}, LastSeenQueryTime = {this.lastSeenHeartBeat.QueryTime:O}";
    }
}