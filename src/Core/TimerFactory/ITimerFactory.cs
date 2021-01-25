using System;
using System.Collections.Generic;
using System.Text;

namespace Chronos.Timer.Core
{
    /// <summary>
    /// Factory to create Timers.
    /// </summary>
    public interface ITimerFactory
    {
        /// <summary>
        /// Creates a timer for the task using the strategy given.
        /// </summary>
        /// <param name="task">The task to perform.</param>
        /// <param name="timeTracker">The strategy which will provide the elapsed time.</param>
        /// <returns>A new ITimer.</returns>
        T CreateTimer<T>(ITimerTask task, ITimeTrackingStrategy timeTracker = null)
            where T : ITimer;

        /// <summary>
        /// Creates a timer for the list of tasks using the strategy given.
        /// </summary>
        /// <param name="tasks">The list of tasks to perform.</param>
        /// <param name="timeTracker">The strategy which will provide the elapsed time.</param>
        /// <returns>A new ITimer</returns>
        T CreateTimer<T>(IEnumerable<ITimerTask> tasks, ITimeTrackingStrategy timeTracker = null)
            where T : ITimer;
    }
}
