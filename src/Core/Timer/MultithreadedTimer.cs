using System;

namespace Chronos.Timer.Core
{
    public class MultithreadedTimer : ITimer
    {
        public TimeSpan ElapsedTime => _timeTrackingStrategy.GetTimeElapsed();

        public TimeSpan TimeLeft => throw new NotImplementedException();

        Guid ITimer.Id => _id;

        private Guid _id = Guid.NewGuid();
        private ITimeTrackingStrategy _timeTrackingStrategy;
        private ITimerTask _timerTask;

        public MultithreadedTimer()
        {
        }

        public void Clear()
        {
            _timeTrackingStrategy.Clear();
        }

        public bool IsStopped()
            => _timeTrackingStrategy.IsStopped();

        public void Start()
        {
            _timeTrackingStrategy.Start();
            // TODO Register with Runtime
        }

        public void Stop()
        {
            _timeTrackingStrategy.Stop();
            // TODO unregister from runtime
        }

        void ITimer.Initialize(ITimeTrackingStrategy timeTrackingStrategy, ITimerTask timerTask)
        {
            _timeTrackingStrategy = timeTrackingStrategy;
            _timerTask = timerTask;
        }

        void ITimer.Update()
        {
            _timeTrackingStrategy.Update();
            if (_timerTask.CanExecute())
            {
                _timerTask.Execute();

                if (_timerTask.NumberOfExecutions <= 0)
                {
                    //TODO Unregister from runtime since the timer is done its job.
                }
            }
        }
    }
}
