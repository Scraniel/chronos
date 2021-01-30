using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Chronos.Timer.Core
{
    /// <summary>
    /// Default runtime which handles timer lifecycle.
    /// </summary>
    public class TimerRuntime : ITimerRuntime
    {

        #region Private
        /// <summary>
        /// The collection of timers, keyed by their IDs.
        /// </summary>
        private Dictionary<Guid, ITimer> _timers;

        /// <summary>
        /// Cancellation token used to cancel the update thread started whenever a timer is first registered.
        /// </summary>
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// Default factory leveraged to create timers.
        /// </summary>
        private ITimerFactory _timerFactory;
        #endregion

        #region Properties
        /// <summary>
        /// The minimum time an update loop will take to complete. Defaults at 10 milliseconds.
        /// </summary>
        public TimeSpan TimeStep { get; set; } = TimeSpan.FromMilliseconds(10);
        #endregion

        public TimerRuntime()
            : this(new TimerFactory())
        {
        }

        ~TimerRuntime()
        {
            _cancellationTokenSource.Cancel();
        }

        public TimerRuntime(ITimerFactory timerFactory)
        {
            _timerFactory = timerFactory;
            _cancellationTokenSource = null;
            _timers = new Dictionary<Guid, ITimer>();
        }

        public T Register<T>(TimeSpan period, int numberOfExecution, Action actionToExecute) where T : ITimer
            => Register(_timerFactory
                    .CreateTimer<T>(new BasicTimerAction(actionToExecute, period, numberOfExecution)));

        public T Register<T>(T timer) where T : ITimer
        {
            if (!_timers.ContainsKey(timer.Id))
            {
                _timers.Add(timer.Id, timer);
            }

            // Ensure that the update loop has been started.
            if (_cancellationTokenSource == null || _cancellationTokenSource.IsCancellationRequested)
            {
                StartUpdate();
            }

            return timer;
        }

        public void Unregister(ITimer timer)
        {
            timer = timer ?? throw new ArgumentNullException(nameof(timer));
            Unregister(timer.Id);
        }

        public void Unregister(Guid timerId)
        {
            if (_timers.ContainsKey(timerId))
            {
                _timers.Remove(timerId);
            }

            // We want to stop executing the update thread when no timers are registered.
            if (_timers.Count == 0)
            {
                StopUpdate();
            }
        }

        private void StartUpdate()
        {
            // Ensures that any previous run is terminated.
            if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
            }

            _cancellationTokenSource = new CancellationTokenSource();

            Task.Run(() => Update(_cancellationTokenSource.Token));
        }

        private void StopUpdate()
        {
            _cancellationTokenSource.Cancel();
        }

        private void Update(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Stopwatch sw = Stopwatch.StartNew();
                foreach (ITimer timer in _timers.Values)
                {
                    if (!timer.IsPaused())
                        timer.UpdateAsync();
                }
                sw.Stop();

                if (sw.Elapsed < TimeStep)
                {
                    Thread.Sleep(TimeStep - sw.Elapsed);
                }
            }
        }

        public ITimer GetTimer(Guid timerId)
        {
            if (_timers.TryGetValue(timerId, out ITimer timer))
                return timer;
            return null;
        }

        public IEnumerable<ITimer> ListTimers()
            => _timers.Values;
    }
}
