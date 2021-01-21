using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Timer.Core.Runtime
{
    public class TimerRuntime : ITimerRuntime
    {

        private List<ITimer> _timers = new List<ITimer>();
        private bool _isPaused = false;
        private Task _bleh;

        public void Pause()
        {
            _isPaused = true;
        }

        public void Unpause()
        {
            _isPaused = false;
        }

        public ITimer Register(TimeSpan timeUntilExecution, Action actionToExecute)
        {
            throw new NotImplementedException();
        }

        public ITimer Register(TimeSpan timeUntilExecution, Action actionToExecute, ITimeTrackingStrategy strategy)
        {
            throw new NotImplementedException();
        }

        public ITimer Register<T>(TimeSpan timeUntilExecution, Action actionToExecute, ITimeTrackingStrategy strategy, ITimerFactory timerFactory) where T : ITimer
        {
            throw new NotImplementedException();
        }

        public ITimer Register(ITimeTrackingStrategy strategy, ITimerTask actionToExecute)
        {
            throw new NotImplementedException();
        }

        public ITimer Register<T>(ITimeTrackingStrategy strategy, ITimerTask actionToExecute, ITimerFactory timerFactory) where T : ITimer
        {
            throw new NotImplementedException();
        }

        public ITimer Register<T>(ITimeTrackingStrategy strategy, IEnumerable<ITimerTask> actionsToExecute, ITimerFactory timerFactory) where T : ITimer
        {
            throw new NotImplementedException();
        }

        public ITimer Register(ITimer timer)
        {
            _timers.Add(timer);

            if(_bleh
        }



        public void Unregister(ITimer timer)
        {
            throw new NotImplementedException();
        }

        public void Unregister(Guid timerId)
        {
            throw new NotImplementedException();
        }

        private void Update()
        {
            while (true)
            {

            }
        }
    }
}
