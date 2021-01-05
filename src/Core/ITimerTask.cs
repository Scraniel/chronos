using System;
using System.Threading.Tasks;

namespace Chronos.Timer.Core
{
    /// <summary>
    /// Defines a task that should happen at some point in the future.
    /// </summary>
    public interface ITimerTask
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
        /// Now many times this has been run.
        /// </summary>
        int NumberOfExecutions { get; }

        /// <summary>
        /// Returns true if task is ready to execute.
        /// </summary>
        /// <returns>True if task can execute, false otherwise.</returns>
        bool CanExecute();

        /// <summary>
        /// The actual task to run. 
        /// </summary>
        /// <returns>The currently running task.</returns>
        Task Execute();
    }
}