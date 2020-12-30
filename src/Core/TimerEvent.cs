using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Core
{
    /// <summary>
    /// Default Tracker that just uses System time.
    /// </summary>
    internal class DefaultTimeTracker : ITimeTracker
    {
        DateTime _startTime;

        public void Start()
        {
            _startTime = DateTime.Now;
        }

        public TimeSpan GetTimeElapsed()
        {
            return DateTime.Now - _startTime;
        }
    }

    /// <summary>
    /// Represents a task that should be run at some point in the future.
    /// </summary>
    public class TimerEvent : ITimerEvent
    {
        #region private 
        private Action _task;
        private ITimeTracker _timeTracker;

        // TODO: Figure out how we want to to config
        private TimeSpan _epsilon = TimeSpan.FromMilliseconds(500);
        #endregion

        #region properties
        public TimeSpan TimeUntilNextExecution { get; private set; }
        public TimeSpan TimeUntilFinalExecution { get; private set; }
        public TimeSpan ExecutionPeriod { get; private set; }
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="task">The job to run.</param>
        /// <param name="timeUntilExecution">How long until the first execution.</param>
        /// <param name="repeatsFor">How long to repeat the task for (default is one execution).</param>
        /// <param name="timeProvider">Optional time provider to use for guaging remaining time.</param>
        public TimerEvent(Action task, TimeSpan timeUntilExecution, TimeSpan? repeatsFor = null, ITimeTracker timeTracker = null)
        {
            _task = task;
            ExecutionPeriod = timeUntilExecution;
            TimeUntilNextExecution = timeUntilExecution;

            TimeUntilFinalExecution = repeatsFor ?? TimeUntilNextExecution;
            _timeTracker = timeTracker ?? new DefaultTimeTracker();

            _timeTracker.Start();
        }

        /// <summary>
        /// Runs the specified task if the time is close enough. Otherwise, no-op.
        /// </summary>
        /// <returns>Awaitable task running the job.</returns>
        public Task Execute()
        {
            if (TimeUntilFinalExecution <= TimeSpan.Zero)
            {
                return Task.CompletedTask;
            }

            // If we're close enough and final time hasn't passed, run. Otherwise, no-op.
            //
            Task runningTask;
            TimeSpan elapsedTime = _timeTracker.GetTimeElapsed();
            TimeUntilNextExecution -= elapsedTime;
            TimeUntilFinalExecution -= elapsedTime;

            if (TimeUntilNextExecution < _epsilon)
            {
                TimeUntilNextExecution += ExecutionPeriod;
                runningTask = Task.Factory.StartNew(_task);
            }
            else
            {
                runningTask = Task.CompletedTask;
            }

            return runningTask;
        }
    }
}
