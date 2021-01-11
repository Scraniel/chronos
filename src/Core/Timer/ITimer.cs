using System;

namespace Chronos.Timer.Core
{
    public interface ITimer
    {
        public TimeSpan ElapsedTime { get; }
        public TimeSpan TimeLeft { get; }
        internal Guid Id { get; }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        public void Start();

        /// <summary>
        /// Stops the timer.
        /// </summary>
        public void Stop();

        /// <summary>
        /// Returns true if the tracker is not running.
        /// </summary>
        /// <returns></returns>
        public bool IsStopped();

        /// <summary>
        /// Wipes the elapsed time stored in the tracker. Does not stop the timer.
        /// </summary>
        public void Clear();

        internal void Initialize(ITimeTrackingStrategy timeTrackingStrategy, ITimerTask timerTask);

        /// <summary>
        /// Updates the elapsed time stored in the timer.
        /// </summary>
        internal void Update();
    }
}
