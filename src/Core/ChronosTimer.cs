using System;
using System.Collections.Generic;
using System.Text;

namespace Chronos.Core
{
    /// <summary>
    /// Holds information about a task to be run.
    /// </summary>
    class ChronosTimer
    {
        /// <summary>
        /// List of tasks to be run.
        /// </summary>
        IList<ITimerTask> TimerTask { get; set; }

        /// <summary>
        /// The time tracking strategy to use. 
        /// </summary>
        ITimeTrackingStrategy TimeStrategy { get; set; }
    }
}
