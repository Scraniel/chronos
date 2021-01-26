using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Chronos.Timer.Core.Runtime
{
    public class TimerRuntime : ITimerRuntime
    {
        /// <summary>
        /// The minimum time an update loop will take to complete. Defaults at 10 milliseconds.
        /// </summary>
        public TimeSpan TimeStep { get; set; } = TimeSpan.FromMilliseconds(10);

        private Dictionary<Guid, ITimer> _timers = new Dictionary<Guid, ITimer>();
        private bool _isPaused = false;
        private CancellationTokenSource _cancellationTokenSource = null;
        private ITimerFactory _timerFactory = new TimerFactory();

        public void Pause()
        {
            _isPaused = true;
        }

        public void Unpause()
        {
            _isPaused = false;
        }

        public T Register<T>(TimeSpan timeUntilExecution, Action actionToExecute)
            where T : ITimer
            => Register(_timerFactory.CreateTimer<T>(new TimerTask(actionToExecute, timeUntilExecution)));

        public T Register<T>(TimeSpan timeUntilExecution, Action actionToExecute, ITimeTrackingStrategy strategy)
            where T : ITimer
            => Register(_timerFactory.CreateTimer<T>(new TimerTask(actionToExecute, timeUntilExecution), strategy));

        public T Register<T>(TimeSpan timeUntilExecution, Action actionToExecute, ITimeTrackingStrategy strategy, ITimerFactory timerFactory) 
            where T : ITimer
            => Register(timerFactory.CreateTimer<T>(new TimerTask(actionToExecute, timeUntilExecution), strategy));

        public T Register<T>(ITimeTrackingStrategy strategy, ITimerTask actionToExecute)
            where T : ITimer
            => Register(_timerFactory.CreateTimer<T>(actionToExecute, strategy));

        public T Register<T>(ITimeTrackingStrategy strategy, ITimerTask actionToExecute, ITimerFactory timerFactory)
            where T : ITimer
            => Register(timerFactory.CreateTimer<T>(actionToExecute, strategy));

        public T Register<T>(ITimeTrackingStrategy strategy, IEnumerable<ITimerTask> actionsToExecute, ITimerFactory timerFactory) where T : ITimer
            => Register(timerFactory.CreateTimer<T>(actionsToExecute, strategy));

        public T Register<T>(T timer) where T : ITimer
        {
            if (!_timers.ContainsKey(timer.Id))
                _timers.Add(timer.Id, timer);

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
                _timers.Remove(timerId);

            // We want to stop executing the update thread when no timers are registered.
            if (_timers.Count == 0)
                StopUpdate();
        }

        private void StartUpdate()
        {
            // Ensures that any previous run is terminated.
            if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
            }

            _cancellationTokenSource = new CancellationTokenSource();

            Task.Run(
                () =>
                {
                    Update(_cancellationTokenSource.Token);
                });
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
                if (!_isPaused)
                {
                    foreach (ITimer timer in _timers.Values)
                    {
                        if (!timer.IsStopped())
                            timer.Update();
                    }
                }
                sw.Stop();

                if (sw.Elapsed < TimeStep)
                {
                    Thread.Sleep(TimeStep - sw.Elapsed);
                }
            }
        }
    }
}
