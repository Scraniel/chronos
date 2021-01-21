using System;
using System.Collections.Generic;

namespace Chronos.Timer.Core
{
    public interface ITimerRuntime
    {
        /// <summary>
        /// Registers a timer with the timer runtime.
        /// </summary>
        /// <param name="timeUntilExecution">The amount of time before the given action will be executed.</param>
        /// <param name="actionToExecute">The action to execute.</param>
        /// <returns>A registered timer.</returns>
        ITimer Register(TimeSpan timeUntilExecution, Action actionToExecute);

        ITimer Register(TimeSpan timeUntilExecution, Action actionToExecute, ITimeTrackingStrategy strategy);
        ITimer Register<T>(TimeSpan timeUntilExecution, Action actionToExecute, ITimeTrackingStrategy strategy, ITimerFactory timerFactory)
            where T : ITimer;
        ITimer Register(ITimeTrackingStrategy strategy, ITimerTask actionToExecute);
        ITimer Register<T>(ITimeTrackingStrategy strategy, ITimerTask actionToExecute, ITimerFactory timerFactory)
            where T : ITimer;

        ITimer Register<T>(ITimeTrackingStrategy strategy, IEnumerable<ITimerTask> actionsToExecute, ITimerFactory timerFactory)
            where T : ITimer;
        
        /// <summary>
        /// Registers the timer with the runtime.
        /// </summary>
        /// <param name="timer">The timer to register.</param>
        /// <returns>The registered timer.</returns>
        ITimer Register(ITimer timer);


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

        /// <summary>
        /// Pauses all timers within the runtime.
        /// </summary>
        /// <remarks>Action which are executed at pause time will finish executing.</remarks>
        void Pause();

        /// <summary>
        /// Unpauses all timers within the runtime.
        /// </summary>
        void Unpause();
    }
}
