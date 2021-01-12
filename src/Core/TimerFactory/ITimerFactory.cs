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
        /// <param name="timeTracker">The time tracking strategy to use.</param>
        /// <returns>The Timer.</returns>
        ITimer CreateTimer<TTimer>(ITimerTask task, ITimeTrackingStrategy timeTracker = null)
            where TTimer : ITimer;

        /// <summary>
        /// Creates a timer for the list of tasks using the strategy given.
        /// </summary>
        /// <param name="tasks">The list of tasks to perform.</param>
        /// <param name="timeTracker">The time tracking strategy to use.</param>
        /// <returns>The Timer.</returns>
        ITimer CreateTimer<TTimer>(List<ITimerTask> tasks, ITimeTrackingStrategy timeTracker = null)
            where TTimer : ITimer;

        /// <summary>
        /// Creates a timer for the task using the strategy given.
        /// </summary>
        /// <param name="task">The task to perform.</param>
        /// <param name="timeTracker">The time tracking strategy to use.</param>
        /// <returns>The Timer.</returns>
        ITimer CreateTimer(ITimerTask task, ITimeTrackingStrategy timeTracker = null);

        /// <summary>
        /// Creates a timer for the list of tasks using the strategy given.
        /// </summary>
        /// <param name="tasks">The list of tasks to perform.</param>
        /// <param name="timeTracker">The time tracking strategy to use.</param>
        /// <returns>The Timer.</returns>
        ITimer CreateTimer(List<ITimerTask> tasks, ITimeTrackingStrategy timeTracker = null);
    }
}
