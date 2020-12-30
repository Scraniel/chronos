using System;
using System.Threading.Tasks;

namespace Chronos.Core
{
    /// <summary>
    /// Defines a task that should happen at some point in the future.
    /// </summary>
    public interface ITimerEvent
    {
        /// <summary>
        /// How often this task should run.
        /// </summary>
        TimeSpan ExecutionPeriod { get; }

        /// <summary>
        /// How long until this should stop repeating.
        /// </summary>
        TimeSpan TimeUntilFinalExecution { get; }

        /// <summary>
        /// How long until the next time this runs.
        /// </summary>
        TimeSpan TimeUntilNextExecution { get; }

        /// <summary>
        /// The actual task to run. 
        /// </summary>
        /// <returns>The currently running task.</returns>
        Task Execute();
    }
}