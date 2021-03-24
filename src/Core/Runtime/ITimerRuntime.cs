using System;
using System.Collections.Generic;

namespace Chronos.Timer.Core
{
    /// <summary>
    /// Timer runtime which handles timer lifecycle.
    /// </summary>
    public interface ITimerRuntime
    {
        /// <summary>
        /// The minimum time an update loop will take to complete.
        /// </summary>
        TimeSpan TimeStep { get; set; }

        /// <summary>
        /// Registers a timer with the runtime, ensuring it to update & trigger its actions automatically.
        /// </summary>
        /// <typeparam name="T">The timer type to register.</typeparam>
        /// <param name="period">The amount of time before a given action will be executed.</param>
        /// <param name="actionToExecute">The action to execute.</param>
        /// <param name="numberOfExecution">The number of time an action should be executed.</param>
        /// <returns>A registered timer.</returns>
        TimerInfo Register<T>(TimeSpan period, int numberOfExecution, Action actionToExecute)
            where T : ITimer;

        /// <summary>
        /// Registers the timer with the runtime.
        /// </summary>
        /// <typeparam name="T">The timer type to register.</typeparam>
        /// <param name="timer">The timer to register.</param>
        /// <returns>The registered timer.</returns>
        TimerInfo Register<T>(T timer)
            where T : ITimer;

        /// <summary>
        /// Unregister a timer from the timer runtime. This will ensure that the timer is not updated/executed anymore.
        /// </summary>
        /// <param name="timer">The timer to unregister.</param>
        void Unregister(TimerInfo timer);

        /// <summary>
        /// Unregister a timer from the timer runtime. This will ensure that the timer is not updated/executed anymore.
        /// </summary>
        /// <param name="timerId">The Id of the timer.</param>
        void Unregister(Guid timerId);

        /// <summary>
        /// Gets a timer which is currently registered with this runtime.
        /// </summary>
        /// <param name="timerId">The id of the timer registered with this runtime.</param>
        /// <returns>Returns the timer with the given id.</returns>
        TimerInfo GetTimer(Guid timerId);

        /// <summary>
        /// Gets all of the timers currently registered to this runtime.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of all timers registered with this runtime.</returns>
        IEnumerable<TimerInfo> GetTimers();
    }
}
