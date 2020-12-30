using System;
using Chronos.Core;

namespace Chronos.Tests.Mocks
{
    public class TestTimeTracker : ITimeTracker
    {
        private TimeSpan _elapsed;
        private TimeSpan _timeToPassOnUpdate;
        private bool _isStopped;

        /// <summary>
        /// Constructor - takes in the amount to pass on each call to update.
        /// </summary>
        /// <param name="timeToPassOnUpdate">How much time to simulate passing on update calls.</param>
        public TestTimeTracker(TimeSpan timeToPassOnUpdate)
        {
            _timeToPassOnUpdate = timeToPassOnUpdate;
        }

        /// <summary>
        /// Getter. _elapsed Only gets increased on calls to update.
        /// </summary>
        /// <returns>Returns _elapsed.</returns>
        public TimeSpan GetTimeElapsed()
        {
            return _elapsed;
        }

        /// <summary>
        /// Allows update calls to increase _elapsed.
        /// </summary>
        public void Start()
        {
            _isStopped = false;
        }

        /// <summary>
        /// Prevents update from increasing _elapsed.
        /// </summary>
        public void Stop() 
        {
            _isStopped = true;
        }

        /// <summary>
        /// Returns whether timer is stopped.
        /// </summary>
        /// <returns>True if stopped was called.</returns>
        public bool IsStopped()
        {
            return _isStopped;
        }

        /// <summary>
        /// Updates _elapsed by the configured amount.
        /// </summary>
        public void Update()
        {
            if(_isStopped)
            {
                return;
            }

            _elapsed += _timeToPassOnUpdate;
        }

        /// <summary>
        /// Resets _elapsed.
        /// </summary>
        public void Clear()
        {
            _elapsed = TimeSpan.Zero;
        }
    }
}
