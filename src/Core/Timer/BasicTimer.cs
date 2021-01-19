using System;
using System.Collections.Generic;

namespace Chronos.Timer.Core
{
    //TODO Replace with real implementation
    public class BasicTimer : ITimer
    {
        public TimeSpan ElapsedTime => throw new NotImplementedException();

        public TimeSpan TimeLeft => throw new NotImplementedException();

        Guid ITimer.Id => throw new NotImplementedException();

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool IsStopped()
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Unpause()
        {
            throw new NotImplementedException();
        }

        void ITimer.Initialize(ITimeTrackingStrategy timeTrackingStrategy, List<ITimerTask> timerTasks)
        {
        }

        void ITimer.Update()
        {
            throw new NotImplementedException();
        }
    }
}
