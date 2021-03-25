using System;

namespace Chronos.Timer.Core
{
    /// <summary>
    /// Encapsulates timer-related method and information.
    /// </summary>
    public class TimerProxy
    {
        private ITimer _timer;

        /// <summary>
        /// The elapsed time since the timer was initially started.
        /// </summary>
        public TimeSpan TotalElapsedTime
        {
            get => _timer.TotalElapsedTime;
        }

        /// <summary>
        /// The time left before the execution of the next action.
        /// </summary>
        public TimeSpan TimeLeft
        {
            get => _timer.TimeLeft;
        }

        public bool IsPaused
        {
            get => _timer.IsPaused();
        }

        /// <summary>
        /// The Timer's UID used to register/unregister with the Chronos timer runtime.
        /// </summary>
        public Guid Id
        {
            get => _timer.Id;
        }

        public TimerProxy(ITimer timer)
        {
            _timer = timer;
        }

        /// <summary>
        /// Unpauses the timer and restarts tracking time.
        /// </summary>
        public void Unpause()
            => _timer.Unpause();

        /// <summary>
        /// Pauses the timer and stops tracking time.
        /// </summary>
        public void Pause()
            => _timer.Pause();

        /// <summary>
        /// Wipes the elapsed time stored in the tracker. Does not pause the timer.
        /// </summary>
        public void Clear()
            => _timer.Clear();
    }
}