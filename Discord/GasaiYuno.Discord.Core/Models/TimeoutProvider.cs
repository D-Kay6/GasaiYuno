using System;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace GasaiYuno.Discord.Core.Models
{
    public sealed class TimeoutProvider
    {
        private bool _disposed;
        private readonly Timer _timer;
        private readonly TaskCompletionSource<object> _timeoutSource;

        public double Delay { get; }

        public TimeoutProvider(TimeSpan delay) : this(delay.TotalMilliseconds) { }

        public TimeoutProvider(double delay)
        {
            Delay = delay;
            _timeoutSource = new TaskCompletionSource<object>();
            _timer = new Timer(delay) { AutoReset = false };
            _timer.Elapsed += HandleTimerElapsed;
            _timer.Start();
        }

        private void HandleTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _disposed = true;
            _timer.Dispose();
            _timeoutSource.SetResult(null);
        }

        public void Reset()
        {
            if (_disposed) return;

            _timer.Stop();
            _timer.Start();
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;
            _timer.Dispose();
            _timeoutSource.TrySetCanceled();
        }

        public Task WaitAsync() => _timeoutSource.Task;
    }
}