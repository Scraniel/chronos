using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chronos.Timer.Core
{
    /// <summary>
    /// A timer which runs in the Chronos timer runtime.
    /// </summary>
    public interface ITimer
    {
        /// <summary>
        /// The elapsed time since the timer was initially started.
        /// </summary>
        public TimeSpan ElapsedTime { get; }

        /// <summary>
        /// The time left before the execution of the next action.
        /// </summary>
        public TimeSpan TimeLeft { get; }

        /// <summary>
        /// The Timer's UID used to register/unregister with the Chronos timer runtime.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Unpauses the timer and restarts tracking time.
        /// </summary>
        public void Unpause();

        /// <summary>
        /// Pauses the timer and stops tracking time.
        /// </summary>
        public void Pause();

        /// <summary>
        /// Returns true if the tracker is not running.
        /// </summary>
        /// <returns></returns>
        public bool IsPaused();

        /// <summary>
        /// Wipes the elapsed time stored in the tracker. Does not pause the timer.
        /// </summary>
        public void Clear();

        /// <summary>
        /// Initializes the timer with the given time tracking strategy and its associated tasks.
        /// </summary>
        /// <param name="timeTrackingStrategy">The strategy which will provide the elapsed time.</param>
        /// <param name="timerTasks">List of tasks to be executed by this timer.</param>
        internal void Initialize(ITimeTrackingStrategy timeTrackingStrategy, List<ITimerAction> timerTasks);

        /// <summary>
        /// Updates the elapsed time stored in the timer one step forward in time as dictated by its 
        /// TimeTrackingStrategyand and runs tasks if enough time has passed.
        /// </summary>
        internal Task UpdateAsync();
    }
}
