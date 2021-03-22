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
            // In the case that the runtime goes out of scope,
            // this will ensure the underlying timers are not leaked.
            _cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Creates a new instance of <see cref="TimerRuntime"/>. 
        /// </summary>
        /// <param name="timerFactory">The factory leveraged to create the underlying timers.</param>
        public TimerRuntime(ITimerFactory timerFactory)
        {
            _timerFactory = timerFactory;
            _cancellationTokenSource = null;
            _timers = new Dictionary<Guid, ITimer>();
        }

        /// <summary>
        /// Creates a new timer and registers it with the runtime.
        /// </summary>
        /// <typeparam name="T">The timer type to register.</typeparam>
        /// <param name="period">The amount of time before a given action will be executed.</param>
        /// <param name="actionToExecute">The action to execute.</param>
        /// <param name="numberOfExecution">The number of time an action should be executed.</param>
        /// <returns>A registered timer.</returns>
        /// <returns></returns>
        public T Register<T>(TimeSpan period, int numberOfExecution, Action actionToExecute) where T : ITimer
            => Register(_timerFactory
                    .CreateTimer<T>(new BasicTimerAction(actionToExecute, period, numberOfExecution)));

        /// <summary>
        /// Registers the timer with the runtime.
        /// </summary>
        /// <typeparam name="T">The timer type to register.</typeparam>
        /// <param name="timer">The timer to register.</param>
        /// <returns>The registered timer.</returns>
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

        /// <summary>
        /// Unregister a timer from the timer runtime. This will ensure that the timer is not updated/executed anymore.
        /// </summary>
        /// <remarks>
        /// Unregistering a timer which is not currently registered with the runtime will result in a no-op.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if the timer passed in is null.</exception>
        /// <param name="timer">The timer to unregister.</param>
        public void Unregister(ITimer timer)
        {
            timer = timer ?? throw new ArgumentNullException(nameof(timer));
            Unregister(timer.Id);
        }

        /// <summary>
        /// Unregister a timer from the timer runtime. This will ensure that the timer is not updated/executed anymore.
        /// </summary>
        /// <remarks>
        /// Unregistering a timer which is not currently registered with the runtime will result in a no-op.
        /// </remarks>
        /// <param name="timerId">The Id of the timer.</param>
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

        /// <summary>
        /// Gets a timer which is currently registered with this runtime.
        /// </summary>
        /// <param name="timerId">The id of the timer registered with this runtime.</param>
        /// <returns>Returns the timer with the given id, or null if the timer isn't found.</returns>
        public ITimer GetTimer(Guid timerId)
        {
            if (_timers.TryGetValue(timerId, out ITimer timer))
                return timer;
            return null;
        }

        /// <summary>
        /// Gets all of the timers currently registered to this runtime.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of all timers registered with this runtime.</returns>
        public IEnumerable<ITimer> GetTimers()
            => _timers.Values;

        /// <summary>
        /// Starts the update thread.
        /// </summary>
        private void StartUpdate()
        {
            // Ensures that if StartUpdate is called whilst an update thread is already running,
            // it will cancel it and then start a new thread.
            if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
            }

            _cancellationTokenSource = new CancellationTokenSource();

            Task.Run(() => Update(_cancellationTokenSource.Token));
        }

        /// <summary>
        /// Stops the update thread. Should only be called when no more timers are registered.
        /// </summary>
        private void StopUpdate()
        {
            _cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Updates the underlying timers
        /// </summary>
        /// <param name="token"></param>
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
    }
}
