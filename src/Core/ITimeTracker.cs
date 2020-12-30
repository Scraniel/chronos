using System;
using System.Collections.Generic;
using System.Text;

namespace Chronos.Core
{
    /// <summary>
    /// Provides a way to measure time elapsed.
    /// </summary>
    public interface ITimeTracker
    {
        /// <summary>
        /// Initializes or resets the tracker.
        /// </summary>
        void Start();

        /// <summary>
        /// Gets the time since last started.
        /// </summary>
        /// <returns>Timespan since tracker was started.</returns>
        TimeSpan GetTimeElapsed();
    }
}
