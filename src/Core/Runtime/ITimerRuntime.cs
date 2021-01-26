using System;
using System.Collections.Generic;

namespace Chronos.Timer.Core
{
    public interface ITimerRuntime
    {
        /// <summary>
        /// The minimum time an update loop will take to complete.
        /// </summary>
        TimeSpan TimeStep { get; set; }

        /// <summary>
        /// Registers a timer with the runtime, ensuring it to update & trigger its actions automatically.
        /// </summary>
        /// <param name="timeUntilExecution">The amount of time before the given action will be executed.</param>
        /// <param name="actionToExecute">The action to execute.</param>
        /// <returns>A registered timer.</returns>
        T Register<T>(TimeSpan timeUntilExecution, Action actionToExecute)
            where T : ITimer;

        /// <summary>
        /// Registers a timer with the runtime, ensuring it to update & trigger its actions automatically.
        /// </summary>
        /// <param name="timeUntilExecution">The amount of time before the given action will be executed.</param>
        /// <param name="actionToExecute">The action to execute.</param>
        /// <param name="strategy">The strategy providing the time.</param>
        /// <returns>A registered timer.</returns>
        T Register<T>(TimeSpan timeUntilExecution, Action actionToExecute, ITimeTrackingStrategy strategy)
            where T : ITimer;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="timeUntilExecution"></param>
        /// <param name="actionToExecute"></param>
        /// <param name="strategy"></param>
        /// <param name="timerFactory"></param>
        /// <returns></returns>
        T Register<T>(TimeSpan timeUntilExecution, Action actionToExecute, ITimeTrackingStrategy strategy, ITimerFactory timerFactory)
            where T : ITimer;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strategy"></param>
        /// <param name="actionToExecute"></param>
        /// <returns></returns>
        T Register<T>(ITimeTrackingStrategy strategy, ITimerTask actionToExecute)
            where T : ITimer;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strategy"></param>
        /// <param name="actionToExecute"></param>
        /// <param name="timerFactory"></param>
        /// <returns></returns>
        T Register<T>(ITimeTrackingStrategy strategy, ITimerTask actionToExecute, ITimerFactory timerFactory)
            where T : ITimer;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strategy"></param>
        /// <param name="actionsToExecute"></param>
        /// <param name="timerFactory"></param>
        /// <returns></returns>
        T Register<T>(ITimeTrackingStrategy strategy, IEnumerable<ITimerTask> actionsToExecute, ITimerFactory timerFactory)
            where T : ITimer;
        
        /// <summary>
        /// Registers the timer with the runtime.
        /// </summary>
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
