using System;
using System.Collections.Generic;
using System.Text;

namespace Chronos.Core
{
    /// <summary>
    /// Factory to create TimerTasks.
    /// </summary>
    interface ITimerTaskFactory
    {
        /// <summary>
        /// Creates a timer.
        /// </summary>
        /// <param name="task">The task to perform.</param>
        /// <param name="timeUntilFirstExecution">Time to wait until first execution.</param>
        /// <param name="interval">Interval between executions.</param>
        /// <param name="repeatsFor">How long to repeat the action for.</param>
        /// <param name="timeTracker">Used to measure passage of time.</param>
        /// <returns></returns>
        ITimerTask CreateTimer(Action task, TimeSpan timeUntilFirstExecution, TimeSpan? interval = null, TimeSpan? repeatsFor = null, ITimeTracker timeTracker = null);
    }
}
