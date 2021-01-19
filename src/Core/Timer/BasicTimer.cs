using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                _timerTasks.Add(timerTask.Period, timerTask);
            }

            _timeTrackingStrategy = timeTrackingStrategy;
        }
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

                // NOTE: We should think about this. elapsed + period means that:  If a task 
                // is run early/late, it's next scheduled time also will. This could lead to 
                // drifting if it happens multiple times.
                //
                // nextTime + period will mean things are always scheduled for when they're
                // *supposed* to have been run given it always runs on time.
                //
                toAdd.Add(KeyValuePair.Create(nextTime + taskToRun.Period, taskToRun));
                nextTime = _timerTasks.Keys[0];
            }

            foreach (KeyValuePair<TimeSpan, ITimerTask> pair in toAdd)
            {
                _timerTasks.Add(pair.Key, pair.Value);
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
