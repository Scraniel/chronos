using System;

namespace Chronos.Timer.Core.TimerInfo
{
    public class TimerInfo
    {
        private ITimer _timer;
        public TimerInfo(ITimer timer)
        {
            _timer = timer;
        }

        /// <summary>
        /// The elapsed time since the timer was initially started.
        /// </summary>
        public TimeSpan ElapsedTime
        {
            get => _timer.ElapsedTime;
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
        public Guid Id { get
            {
                return _timer.Id;
            }
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
