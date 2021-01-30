using Chronos.Timer.Core;
using System;
using System.Collections.Generic;

namespace Chronos.Timer.Mocks
{
    public class MockTimerFactory : ITimerFactory
    {
        public MockTimer MockTimer { get; set; } = new MockTimer();

        public Func<ITimer> OnCreateTimer { get; set; }

        public MockTimerFactory()
        {
            OnCreateTimer = () =>
            {
                MockTimer t = MockTimer?.Clone();
                if (t != null)
                {
                    t.Id = Guid.NewGuid();
                }
                return t;
            };
        }

        public T CreateTimer<T>(ITimerAction task, ITimeTrackingStrategy timeTracker = null)
            where T : ITimer
            => (T)OnCreateTimer?.Invoke();

        public T CreateTimer<T>(IEnumerable<ITimerAction> tasks, ITimeTrackingStrategy timeTracker = null) where T : ITimer
            => (T)OnCreateTimer?.Invoke();
    }
}
