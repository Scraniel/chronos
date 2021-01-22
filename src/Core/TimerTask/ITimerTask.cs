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
        TimeSpan Period { get; }

        /// <summary>
        /// How many times to run this task.
        /// </summary>
        int TimesToExecute { get; }

        /// <summary>
        /// True if task can execute, false otherwise.
        /// </summary>
        bool CanExecute { get; }

        /// <summary>
        /// True if task is finished, false otherwise.
        /// </summary>
        bool IsFinished { get; }

        /// <summary>
        /// The actual task to run. 
        /// </summary>
        /// <returns>The currently running task.</returns>
        Task ExecuteAsync();
    }
}