using System;
using System.Collections.Generic;

namespace Chronos.Timer.Core
{
    public class TimerFactory : ITimerFactory
    {
        public ITimer CreateTimer<TTimer>(ITimeTrackingStrategy timeTracker = null) where TTimer : ITimer
        {
            return CreateTimer<TTimer>(new List<ITimerTask>(), timeTracker);
        }

        public ITimer CreateTimer<TTimer>(ITimerTask task, ITimeTrackingStrategy timeTracker = null) where TTimer : ITimer
        {
            if (task == null)
                return CreateTimer<TTimer>(timeTracker);

            return CreateTimer<TTimer>(new List<ITimerTask> { task }, timeTracker);
        }

        public ITimer CreateTimer<TTimer>(List<ITimerTask> tasks, ITimeTrackingStrategy timeTracker = null) where TTimer : ITimer
        {
            TTimer timer = (TTimer)Activator.CreateInstance(typeof(TTimer));
            timer.Initialize(timeTracker, tasks);
            return timer;
        }

        public ITimer CreateTimer(ITimerTask task, ITimeTrackingStrategy timeTracker = null)
        {
            return CreateTimer(new List<ITimerTask> { task }, timeTracker);
        }

        public ITimer CreateTimer(List<ITimerTask> tasks, ITimeTrackingStrategy timeTracker = null)
        {
            ITimer timer = new BasicTimer();
            timer.Initialize(timeTracker, tasks);
            return timer;
        }
    }
}
