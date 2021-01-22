﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Chronos.Timer.Core.Timer
{
    public class BasicTimer : ITimer
    {
        #region private
        
        private List<KeyValuePair<TimeSpan, ITimerTask>> _timerTasks;
        private ITimeTrackingStrategy _timeTrackingStrategy;
        private Dictionary<int, Task> _currentlyRunning;
        private int _taskCounter = 0;

        // TODO: Decide how to handle config values
        //
        private TimeSpan _epsilon = TimeSpan.FromMilliseconds(100);
        private TimeSpan MaxTimespan(TimeSpan first, TimeSpan second)
        {
            return first > second ? first : second;
        }
        private int CompareByStartTime(KeyValuePair<TimeSpan, ITimerTask> first, KeyValuePair<TimeSpan, ITimerTask> second)
        {
            return first.Key.CompareTo(second.Key);
        }
        #endregion
        public TimeSpan ElapsedTime { get; private set; }
        public TimeSpan TimeLeft { get; private set; }
        public Guid Id { get; private set; }

        #region Explicitly implemented internal
        void ITimer.Initialize(ITimeTrackingStrategy timeTrackingStrategy, List<ITimerTask> timerTasks)
        {
            Id = Guid.NewGuid();
            foreach(ITimerTask timerTask in timerTasks)
            {
                // TODO: consider enforcing minimum period.
                //
                _timerTasks.Add(new KeyValuePair<TimeSpan, ITimerTask>(timerTask.Period, timerTask));
            }

            _timerTasks.Sort(CompareByStartTime);
            _timeTrackingStrategy = timeTrackingStrategy;
        }
        public BasicTimer()
        {
            _timerTasks = new List<KeyValuePair<TimeSpan, ITimerTask>>();
            _currentlyRunning = new Dictionary<int, Task>();
            _timeTrackingStrategy = null;
        }

        /// <summary>
        /// Stores the task in a list while it's running and removes it once it's done.
        /// </summary>
        /// <param name="taskToRun">The task to execute asynchronously.</param>
        /// <returns>The running task.</returns>
        async Task SaveAndRunAsync(ITimerTask taskToRun)
        {
            int id = Interlocked.Increment(ref _taskCounter);
            Task runningTask = taskToRun.ExecuteAsync();
            _currentlyRunning.Add(id, runningTask);
            await runningTask;
            _currentlyRunning.Remove(id);
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
        async Task ITimer.UpdateAsync()
        {
            if (_timerTasks.Count == 0)
            {
                throw new InvalidOperationException("Empty Timers cannot be updated.");
            }

            _timeTrackingStrategy.Update();
            ElapsedTime = _timeTrackingStrategy.GetTimeElapsed();

            TimeSpan nextTime = _timerTasks[0].Key;
            TimeLeft = MaxTimespan(TimeSpan.Zero, nextTime - _epsilon - ElapsedTime);

            if(TimeLeft > TimeSpan.Zero)
            {
                return;
            }
            
            List<KeyValuePair<TimeSpan, ITimerTask>> toAdd = new List<KeyValuePair<TimeSpan, ITimerTask>>();
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
                ITimerTask taskToRun = _timerTasks[0].Value;
                runningTasks.Add(SaveAndRunAsync(taskToRun));
                _timerTasks.RemoveAt(0);

                // NOTE: A subtle change here to elapsed + period would mean if a task was 
                // run early/late, it's next scheduled time would also be. This could lead to 
                // drifting if it happens multiple times.
                //
                // nextTime + period will mean things are always scheduled for when they're
                // *supposed* to have been run given it always runs on time.
                //
                toAdd.Add(KeyValuePair.Create(nextTime + taskToRun.Period, taskToRun));

                if(_timerTasks.Count > 0)
                {
                    nextTime = _timerTasks[0].Key;
                    TimeLeft = MaxTimespan(TimeSpan.Zero, nextTime - _epsilon - ElapsedTime);
                }
                else
                {
                    // TODO: This probably isn't the best way to do this.
                    //
                    TimeLeft = TimeSpan.MaxValue;
                }
            }

            foreach (KeyValuePair<TimeSpan, ITimerTask> newTask in toAdd)
            {
                if (newTask.Value.IsFinished)
                {
                    continue;
                }

                _timerTasks.Add(new KeyValuePair<TimeSpan, ITimerTask>(newTask.Key, newTask.Value));
            }

            // TODO: If performance becomes an issue, we should replace the list with a heap or actual pqueue.
            // SortedList doesn't allow for duplicates, and storing lists of tasks for a given timestamp will
            // introduce a lot of unnecessary overhead.
            //
            _timerTasks.Sort(CompareByStartTime);

            await Task.WhenAll(runningTasks);            
        }
        #endregion
        public void Clear()
        {
            _timeTrackingStrategy.Clear();
            ElapsedTime = TimeSpan.Zero;
            List<KeyValuePair<TimeSpan, ITimerTask>> listCopy = _timerTasks.ToList();
            _timerTasks.Clear();
            foreach (var pair in listCopy)
            {
                _timerTasks.Add(new KeyValuePair<TimeSpan, ITimerTask>(pair.Value.Period, pair.Value));
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
    }
}
