using System;

namespace Chronos.Timer.Core
{
    /// <summary>
    /// Default Tracker that uses System time from DateTime.
    /// 
    /// NOTE: Not thread safe.
    /// </summary>
    public class SystemTimeTrackingStrategy : ITimeTrackingStrategy
    {
        private TimeSpan _elapsed;
        private DateTime _lastUpdate;

        /// <summary>
        /// Constructor. Initializes values.
        /// </summary>
        public SystemTimeTrackingStrategy()
        {
            Clear();
            Start();
        }

        /// <summary>
        /// Resets _lastUpdate, allowing elapsed time to be incremented.
        /// </summary>
        public void Start()
        {
            _lastUpdate = DateTime.Now;
        }

        /// <summary>
        /// Returns time elapsed since start, excluding periods where it was stopped.
        /// </summary>
        /// <returns>Time elapsed.</returns>
        public TimeSpan GetTimeElapsed()
        {
            return _elapsed;
        }

        /// <summary>
        /// Prevents timer from being updated.
        /// </summary>
        public void Stop()
        {
            _lastUpdate = DateTime.MinValue;
        }

        /// <summary>
        /// Returns whether timer is stopped.
        /// </summary>
        /// <returns>True if stopped.</returns>
        public bool IsStopped()
        {
            return _lastUpdate == DateTime.MinValue;
        }

        /// <summary>
        /// Resets _elapsed.
        /// </summary>
        public void Clear()
        {
            _elapsed = TimeSpan.Zero;
        }

        /// <summary>
        /// If not stopped, updates _elapsed using DateTime.Now.
        /// </summary>
        public void Update() 
        {
            if(IsStopped())
            {
                return;
            }

            _elapsed += DateTime.Now - _lastUpdate;
        }
    }
}
