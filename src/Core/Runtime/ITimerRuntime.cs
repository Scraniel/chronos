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
        T Register<T>(TimeSpan period, int numberOfExecution, Action actionToExecute)
            where T : ITimer;

        /// <summary>
        /// Registers the timer with the runtime.
        /// </summary>
        /// <typeparam name="T">The timer type to register.</typeparam>
        /// <param name="timer">The timer to register.</param>
        /// <returns>The registered timer.</returns>
        T Register<T>(T timer)
            where T : ITimer;

        /// <summary>
        /// Unregister a timer from the timer runtime. This will ensure that the timer is not updated/executed anymore.
        /// </summary>
        /// <param name="timer">The timer to unregister.</param>
        void Unregister(ITimer timer);

        /// <summary>
        /// Unregister a timer from the timer runtime. This will ensure that the timer is not updated/executed anymore.
        /// </summary>
        /// <param name="timerId">The Id of the timer.</param>
        void Unregister(Guid timerId);

        ITimer GetTimer(Guid timerId);

        IEnumerable<ITimer> ListTimers();
    }
}
