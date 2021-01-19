using System;
using System.Collections.Generic;

namespace Chronos.Timer.Core
{
    public interface ITimerRuntime
    {
        ITimer Register(TimeSpan timeUntilExecution, Action actionToExecute);
        ITimer Register(TimeSpan timeUntilExecution, Action actionToExecute, ITimeTrackingStrategy strategy);
        ITimer Register<T>(TimeSpan timeUntilExecution, Action actionToExecute, ITimeTrackingStrategy strategy, ITimerFactory timerFactory)
            where T : ITimer;
        ITimer Register(ITimeTrackingStrategy strategy, ITimerTask actionToExecute);
        ITimer Register<T>(ITimeTrackingStrategy strategy, ITimerTask actionToExecute, ITimerFactory timerFactory)
            where T : ITimer;
        ITimer Register<T>(ITimeTrackingStrategy strategy, IEnumerable<ITimerTask> actionsToExecute, ITimerFactory timerFactory)
            where T : ITimer;
        
        ITimer Register(ITimer timer);
        void Unregister(ITimer timer);
        void Unregister(Guid timerId);
        void Pause();
        void Unpause();
    }
}
