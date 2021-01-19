using System;
using System.Collections.Generic;

namespace Chronos.Timer.Core
{
    public class TimerFactory : ITimerFactory
    {
        /// <summary>
        /// Creates a timer for the task using the strategy given.
        /// </summary>
        /// <typeparam name="TTimer">The type of timer to create.</typeparam>
        /// <param name="timeTracker">The strategy which will provide the elapsed time.</param>
        /// <returns>A new ITimer.</returns>
        public ITimer CreateTimer<TTimer>(ITimeTrackingStrategy timeTracker = null) where TTimer : ITimer
        {
            return CreateTimer<TTimer>(new List<ITimerTask>(), timeTracker);
        }

        /// <summary>
        /// Creates a timer for the task using the strategy given.
        /// </summary>
        /// <typeparam name="TTimer">The type of timer to create.</typeparam>
        /// <param name="task">The task to perform.</param>
        /// <param name="timeTracker">The strategy which will provide the elapsed time.</param>
        /// <returns>A new ITimer.</returns>
        public ITimer CreateTimer<TTimer>(ITimerTask task, ITimeTrackingStrategy timeTracker = null) where TTimer : ITimer
        {
            if (task == null)
                return CreateTimer<TTimer>(timeTracker);

            return CreateTimer<TTimer>(new List<ITimerTask> { task }, timeTracker);
        }

        /// <summary>
        /// Creates a timer for the list of tasks using the strategy given.
        /// </summary>
        /// <typeparam name="TTimer">The type of timer to create.</typeparam>
        /// <param name="tasks">The list of tasks to perform.</param>
        /// <param name="timeTracker">The strategy which will provide the elapsed time.</param>
        /// <returns>A new ITimer.</returns>
        public ITimer CreateTimer<TTimer>(List<ITimerTask> tasks, ITimeTrackingStrategy timeTracker = null) where TTimer : ITimer
        {
            TTimer timer = (TTimer)Activator.CreateInstance(typeof(TTimer));
            timer.Initialize(timeTracker, tasks);
            return timer;
        }
    }
}
