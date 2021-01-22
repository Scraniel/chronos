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
    public class BasicTimerTask : ITimerTask
    {
        #region private 
        private Action _task;
        private bool _finished;
        #endregion

        #region properties
        public TimeSpan Period { get; private set; }
        public int TimesToExecute { get; private set; }

        public bool CanExecute
        {
            get { return !_finished; }
        }
        public bool IsFinished
        {
            get { return _finished; }
        }
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="task">The job to run.</param>
        /// <param name="period">How long until the first and subsequent executions.</param>
        /// <param name="numTimesToRun">How long to repeat the task for (default is one execution).</param>
        public BasicTimerTask(Action task, TimeSpan period, int numTimesToRun)
        {
            if(numTimesToRun <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numTimesToRun), numTimesToRun, "Value must be greater than 0.");
            }

            _task = task;
            _finished = false;
            TimesToExecute = numTimesToRun;
        }
        public async Task ExecuteAsync()
        {
            if (!CanExecute)
            {
                return;
            }

            // TODO: Can this be called from multiple threads? Interlocked.Decrement
            // may be required.
            //
            if (--TimesToExecute <= 0)
            {
                _finished = true;
            }

            await Task.Run(_task);
        }
    }
}
