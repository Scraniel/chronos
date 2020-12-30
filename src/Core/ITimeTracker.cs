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
        /// Stops tracker from updating.
        /// </summary>
        void Stop();

        /// <summary>
        /// Returns true if the tracker is not running.
        /// </summary>
        /// <returns></returns>
        bool IsStopped();

        /// <summary>
        /// Updates the elapsed time stored in the tracker.
        /// </summary>
        void Update();

        /// <summary>
        /// Wipes the elapsed time stored in the tracker.
        /// </summary>
        void Clear();

        /// <summary>
        /// Gets the time since last started.
        /// </summary>
        /// <returns>Timespan since tracker was started.</returns>
        TimeSpan GetTimeElapsed();
    }
}
