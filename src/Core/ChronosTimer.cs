using System;
using System.Collections.Generic;
using System.Text;

namespace Chronos.Timer.Core
{
    /// <summary>
    /// Holds information about a task to be run.
    /// </summary>
    public class ChronosTimer
    {
        /// <summary>
        /// List of tasks to be run.
        /// </summary>
        public IList<ITimerTask> TimerTask { get; set; }

        /// <summary>
        /// The time tracking strategy to use. 
        /// </summary>
        public ITimeTrackingStrategy TimeStrategy { get; set; }
    }
}
