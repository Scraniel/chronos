using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chronos.Timer.Core
{
    /// <summary>
    /// Represents a task that should be run at some point in the future.
    /// </summary>
    public class TimerTask : ITimerTask
    {
        #region private 
        private Action _task;
        private ITimeTrackingStrategy _timeTracker;
        private bool _completed;

        // TODO: Figure out how we want to to config
        private TimeSpan _epsilon = TimeSpan.FromMilliseconds(500);
        #endregion

        #region properties
        public TimeSpan TimeUntilNextExecution { get; private set; }
        public TimeSpan TimeUntilFinalExecution { get; private set; }
        public TimeSpan ExecutionPeriod { get; private set; }

        public int NumberOfExecutions { get; private set; }
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="task">The job to run.</param>
        /// <param name="timeUntilExecution">How long until the first execution.</param>
        /// <param name="repeatsFor">How long to repeat the task for (default is one execution).</param>
        /// <param name="timeTracker">Optional time provider to use for guaging remaining time.</param>
        public TimerTask(Action task, TimeSpan timeUntilExecution, TimeSpan? repeatsFor = null, ITimeTrackingStrategy timeTracker = null)
        {
            _task = task;
            ExecutionPeriod = timeUntilExecution;
            TimeUntilNextExecution = timeUntilExecution;
            _completed = false;

            TimeUntilFinalExecution = repeatsFor ?? TimeUntilNextExecution;
            _timeTracker = timeTracker ?? new SystemTimeTracker();

            _timeTracker.Start();
        }

        /// <summary>
        /// Runs the specified task if the time is close enough. Otherwise, no-op.
        /// </summary>
        /// <returns>Awaitable task running the job.</returns>
        public Task Execute()
        {
            Task runningTask = Task.CompletedTask;
            if (_completed)
            {
                return runningTask;
            }

            // If we're close enough and final time hasn't passed, run. Otherwise, no-op.
            //
            TimeSpan elapsedTime = _timeTracker.GetTimeElapsed();
            TimeUntilNextExecution -= elapsedTime;
            TimeUntilFinalExecution -= elapsedTime;

            if (CanExecute())
            {
                // TODO: decide how we want to deal with missing this. If we run a bit too 
                // soon or late, this will bump the next execution up or down to maintain
                // the average. This assumes the calling function polls this relatively 
                // frequently.
                //
                TimeUntilNextExecution += ExecutionPeriod;
                runningTask = Task.Factory.StartNew(_task);
                NumberOfExecutions++;

                if(TimeUntilFinalExecution <= TimeSpan.Zero)
                {
                    _completed = true;
                }
            }
                
            return runningTask;
        }

        /// <summary>
        /// Whether the task can currently be executed.
        /// </summary>
        /// <returns>True if the task can be executed.</returns>
        public bool CanExecute()
        {
            return !_completed && TimeUntilNextExecution < _epsilon;
        }
    }
}
