using System;
using System.Collections.Generic;
using System.Text;

namespace Chronos.Core
{
    /// <summary>
    /// Factory to create Timers.
    /// </summary>
    interface ITimerFactory
    {
        /// <summary>
        /// Creates a timer for the task using the strategy given.
        /// </summary>
        /// <param name="task">The task to perform.</param>
        /// <param name="timeTracker">The time tracking strategy to use.</param>
        /// <returns>The Timer.</returns>
        ChronosTimer CreateTimer(ITimerTask task, ITimeTrackingStrategy timeTracker = null);

        /// <summary>
        /// Creates a timer for the list of tasks using the strategy given.
        /// </summary>
        /// <param name="tasks">The list of tasks to perform.</param>
        /// <param name="timeTracker">The time tracking strategy to use.</param>
        /// <returns>The Timer.</returns>
        ChronosTimer CreateTimer(List<ITimerTask> tasks, ITimeTrackingStrategy timeTracker = null);
    }
}
