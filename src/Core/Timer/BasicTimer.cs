using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Chronos.Timer.Core.Timer
{
    public class BasicTimer : ITimer
    {
        #region private
        
        private SortedList<TimeSpan, ITimerTask> _timerTasks;
        private ITimeTrackingStrategy _timeTrackingStrategy;
        private Dictionary<int, Task> _currentlyRunning;
        private int _taskCounter = 0;
        #endregion
        public TimeSpan ElapsedTime => throw new NotImplementedException();
        public TimeSpan TimeLeft => throw new NotImplementedException();
        public Guid Id { get; private set; }

        #region Explicitly implemented internal
        void ITimer.Initialize(ITimeTrackingStrategy timeTrackingStrategy, List<ITimerTask> timerTasks)
        {
            Id = Guid.NewGuid();
            foreach(ITimerTask timerTask in timerTasks)
            {
                // TODO: consider enforcing minimum period.
                //
                _timerTasks.Add(timerTask.Period, timerTask);
            }

            _timeTrackingStrategy = timeTrackingStrategy;
        }

        /// <summary>
        /// Schedules tasks based on ideal timings. For example, if a task has a period of
        /// 1s but doesn't get run until T = 1.1s, the next schedule will be
        /// for T = 2s rather than 2.1s.  
        /// 
        /// Maintains a list of currently running tasks to potentially be cancelled later.
        /// </summary>
        /// <returns>Awaitable list of tasks running during this update.</returns>
        async Task ITimer.Update()
        {
            _timeTrackingStrategy.Update();
            TimeSpan elapsed = _timeTrackingStrategy.GetTimeElapsed();
            TimeSpan nextTime = _timerTasks.Keys[0];

            // Avoid creating on every update, as this might be happening very frequently.
            //
            List<KeyValuePair<TimeSpan, ITimerTask>> toAdd = null;
            List<Task> runningTasks = null;

            // NOTE: This works under the assumption we only want tasks to be able to be
            // run once per update loop.
            //
            while (nextTime <= elapsed)
            {
                if(toAdd == null)
                {
                    toAdd = new List<KeyValuePair<TimeSpan, ITimerTask>>();
                    runningTasks = new List<Task>();
                }

                ITimerTask taskToRun = _timerTasks.Values[0];
                Action saveAndRun = async () =>
                {
                    int id = Interlocked.Increment(ref _taskCounter);
                    Task runningTask = taskToRun.Execute();
                    _currentlyRunning.Add(id, runningTask);
                    await runningTask;
                    _currentlyRunning.Remove(id);
                };

                runningTasks.Add(Task.Run(saveAndRun));

                _timerTasks.RemoveAt(0);

                // NOTE: A subtle change here to elapsed + period would mean if a task was 
                // run early/late, it's next scheduled time would also be. This could lead to 
                // drifting if it happens multiple times.
                //
                // nextTime + period will mean things are always scheduled for when they're
                // *supposed* to have been run given it always runs on time.
                //
                toAdd.Add(KeyValuePair.Create(nextTime + taskToRun.Period, taskToRun));
                nextTime = _timerTasks.Keys[0];
            }

            foreach (KeyValuePair<TimeSpan, ITimerTask> newTask in toAdd)
            {
                if (newTask.Value.IsFinished)
                {
                    continue;
                }

                _timerTasks.Add(newTask.Key, newTask.Value);
            }

            await Task.WhenAll(runningTasks);            
        }
        #endregion
        public void Clear()
        {
            _timeTrackingStrategy.Clear();
        }
        public bool IsStopped()
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
