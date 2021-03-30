using System;
using System.Collections.Generic;

namespace Chronos.Timer.Core
{
    public class TimerFactory : ITimerFactory
    {
        /// <summary>
        /// Creates a timer for the task using the strategy given.
        /// </summary>
        /// <typeparam name="T">The type of timer to create.</typeparam>
        /// <param name="task">The task to perform.</param>
        /// <param name="timeTracker">The strategy which will provide the elapsed time.</param>
        /// <returns>A new ITimer.</returns>
        public T CreateTimer<T>(ITimerAction task, ITimeTrackingStrategy timeTracker = null) where T : ITimer
        {
            task = task ?? throw new ArgumentNullException(nameof(task));
            return CreateTimer<T>(new List<ITimerAction> { task }, timeTracker);
        }

        /// <summary>
        /// Creates a timer for the list of tasks using the strategy given.
        /// </summary>
        /// <typeparam name="T">The type of timer to create.</typeparam>
        /// <param name="tasks">The list of tasks to perform.</param>
        /// <param name="timeTracker">The strategy which will provide the elapsed time.</param>
        /// <returns>A new ITimer.</returns>
        public T CreateTimer<T>(IEnumerable<ITimerAction> tasks, ITimeTrackingStrategy timeTracker = null) where T : ITimer
        {
            T timer = (T)Activator.CreateInstance(typeof(T));
            timer.Initialize(tasks, timeTracker);
            return timer;
        }
    }
}
