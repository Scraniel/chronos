using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Chronos.Timer.Core
{
    public class BasicTimer : ITimer
    {
        public TimeSpan ElapsedTime { get; private set; }
        public TimeSpan TimeLeft { get; private set; }
        public Guid Id { get; private set; }

        public BasicTimer()
        {
            Id = Guid.NewGuid();
            _timerTasks = new List<KeyValuePair<TimeSpan, ITimerAction>>();
            _currentlyRunning = new Dictionary<int, Task>();
            _timeTrackingStrategy = null;
        }

        #region Explicitly implemented internal
        void ITimer.Initialize(ITimeTrackingStrategy timeTrackingStrategy, List<ITimerAction> timerTasks)
        {
            // TODO: consider enforcing minimum period.
            //
            _timerTasks = timerTasks
                .Select((task) => KeyValuePair.Create(task.Period, task)).ToList();

            _timerTasks.Sort(CompareByStartTime);
            _timeTrackingStrategy = timeTrackingStrategy;
        }

        /// <summary>
        /// Schedules tasks based on ideal timings. For example, if a task has a period of
        /// 1s but doesn't get run until T = 1.1s, the next schedule will be for T = 2s 
        /// rather than 2.1s. Likewise for T = 0.9: second execution would be for T = 2s 
        /// rather than 1.9.
        /// 
        /// Maintains a list of currently running tasks to potentially be cancelled later.
        /// </summary>
        /// <returns>Awaitable list of tasks running during this update.</returns>
        /// <exception cref="InvalidOperationException">Thrown if updated with no tasks.</exception>
        async Task ITimer.UpdateAsync()
        {
            if (_timerTasks.Count == 0)
            {
                throw new InvalidOperationException("Timers must contain Actions in order to be updated.");
            }

            _timeTrackingStrategy.Update();
            ElapsedTime = _timeTrackingStrategy.GetTimeElapsed();

            TimeSpan nextTime = _timerTasks[0].Key;
            TimeLeft = MaxTimespan(TimeSpan.Zero, nextTime - _epsilon - ElapsedTime);

            if (TimeLeft > TimeSpan.Zero)
            {
                return;
            }

            List<KeyValuePair<TimeSpan, ITimerAction>> toAdd = new List<KeyValuePair<TimeSpan, ITimerAction>>();
            List<Task> runningTasks = new List<Task>();

            // NOTES: This works under the assumption we only want tasks to be able to be
            // run once per update loop.
            //
            // Also, epsilon values allow for things to be run prior to their scheduled time.
            // This value is intended to be very small and is meant to avoid tasks having
            // to wait an entire update loop to run.
            //
            while (TimeLeft == TimeSpan.Zero)
            {
                ITimerAction taskToRun = _timerTasks[0].Value;
                runningTasks.Add(SaveAndRunAsync(taskToRun));

                // TODO: consider removing from tail instead of head.
                //
                _timerTasks.RemoveAt(0);

                // NOTE: A subtle change here to elapsed + period would mean if a task was 
                // run early/late, it's next scheduled time would also be. This could lead to 
                // drifting if it happens multiple times.
                //
                // nextTime + period will mean things are always scheduled for when they're
                // *supposed* to have been run given it always runs on time.
                //
                if (!taskToRun.IsFinished)
                {
                    toAdd.Add(KeyValuePair.Create(nextTime + taskToRun.Period, taskToRun));
                }

                if (_timerTasks.Count > 0)
                {
                    nextTime = _timerTasks[0].Key;
                    TimeLeft = MaxTimespan(TimeSpan.Zero, nextTime - _epsilon - ElapsedTime);
                }
                else
                {
                    // TODO: Decide if Zero makes more sense here.
                    //
                    TimeLeft = TimeSpan.MaxValue;
                    break;
                }
            }

            // Don't bother adding tasks that are finished to avoid removing in next iteration.
            //
            _timerTasks.AddRange(
                toAdd.Where(task => !task.Value.IsFinished));

            // TODO: If performance becomes an issue, we should replace the list with a heap or actual pqueue.
            // SortedList doesn't allow for duplicates, and storing lists of tasks for a given timestamp will
            // introduce a lot of unnecessary overhead.
            //
            _timerTasks.Sort(CompareByStartTime);

            await Task.WhenAll(runningTasks).ConfigureAwait(false);
        }
        #endregion

        /// <summary>
        /// Stores the task in a list while it's running and removes it once it's done.
        /// </summary>
        /// <param name="taskToRun">The task to execute asynchronously.</param>
        /// <returns>The running task.</returns>
        async Task SaveAndRunAsync(ITimerAction taskToRun)
        {
            int id = Interlocked.Increment(ref _taskCounter);
            Task runningTask = taskToRun.ExecuteAsync();
            _currentlyRunning.Add(id, runningTask);
            await runningTask.ConfigureAwait(false);
            _currentlyRunning.Remove(id);
        }
        public void Clear()
        {
            _timeTrackingStrategy.Clear();
            ElapsedTime = TimeSpan.Zero;
            List<KeyValuePair<TimeSpan, ITimerAction>> listCopy = _timerTasks.ToList();
            _timerTasks.Clear();
            foreach (var pair in listCopy)
            {
                _timerTasks.Add(KeyValuePair.Create(pair.Value.Period, pair.Value));
            }
        }
        public bool IsPaused()
        {
            return _timeTrackingStrategy.IsStopped();
        }
        public void Pause()
        {
            _timeTrackingStrategy.Stop();
        }
        public void Unpause()
        {
            _timeTrackingStrategy.Start();
        }

        #region private
        private List<KeyValuePair<TimeSpan, ITimerAction>> _timerTasks;
        private ITimeTrackingStrategy _timeTrackingStrategy;
        private Dictionary<int, Task> _currentlyRunning;
        private int _taskCounter = 0;

        // TODO: Decide how to handle config values
        //
        private TimeSpan _epsilon = TimeSpan.FromMilliseconds(10);
        private TimeSpan MaxTimespan(TimeSpan first, TimeSpan second)
        {
            return first > second ? first : second;
        }
        private int CompareByStartTime(KeyValuePair<TimeSpan, ITimerAction> first, KeyValuePair<TimeSpan, ITimerAction> second)
        {
            return first.Key.CompareTo(second.Key);
        }
        #endregion
    }
}
