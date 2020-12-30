using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chronos.Core;
using System;

namespace Chronos.Tests.Core
{
    [TestClass]
    public class TimerTaskUnitTests
    {
        class TestTimeTracker : ITimeTracker
        {
            private TimeSpan _timePassed;
            public TimeSpan GetTimeElapsed()
            {
                return _timePassed;
            }

            public void Start()
            {
                _timePassed = TimeSpan.Zero;
            }

            public void PassTime(TimeSpan timeToPass)
            {
                _timePassed += timeToPass;
            }
        }

        [TestMethod]
        public void Execute_TimeRemaining_NoOp()
        {
            int counter = 0;
            Action task = () => counter++;
            TimeSpan interval = TimeSpan.FromMinutes(5);
            ITimeTracker tracker = new TestTimeTracker();
            ITimerTask timerEvent = new TimerTask(task, interval, timeTracker: tracker);

            timerEvent.Execute().Wait();
            Assert.AreEqual(0, counter);
        }

        [TestMethod]
        public void Execute_PastTime_Execute()
        {
            int counter = 0;
            Action task = () => counter++;
            TimeSpan interval = TimeSpan.FromMinutes(5);
            TestTimeTracker tracker = new TestTimeTracker();
            ITimerTask timerEvent = new TimerTask(task, interval, timeTracker: tracker);

            tracker.PassTime(interval);
            timerEvent.Execute().Wait();
            Assert.AreEqual(1, counter);
        }

        [TestMethod]
        public void Execute_RepeatsEnabled_ExecuteMultipleTimes()
        {
            int counter = 0;
            Action task = () => counter++;
            TimeSpan interval = TimeSpan.FromMinutes(5);
            TimeSpan repeatsFor = TimeSpan.FromMinutes(20);
            TestTimeTracker tracker = new TestTimeTracker();
            ITimerTask timerEvent = new TimerTask(task, interval, repeatsFor, tracker);

            for(int i = 0; i < 4; i++)
            {
                tracker.PassTime(TimeSpan.FromMinutes(5));
                timerEvent.Execute().Wait();
            }

            Assert.AreEqual(3, counter);
        }

        [TestMethod]
        public void Execute_RepeatsEnabled_NoOpPastBoundary()
        {
            int counter = 0;
            Action task = () => counter++;
            TimeSpan interval = TimeSpan.FromMinutes(5);
            TimeSpan repeatsFor = TimeSpan.FromMinutes(20);
            TestTimeTracker tracker = new TestTimeTracker();
            ITimerTask timerEvent = new TimerTask(task, interval, repeatsFor, tracker);

            for (int i = 0; i < 10; i++)
            {
                tracker.PassTime(TimeSpan.FromMinutes(5));
                timerEvent.Execute().Wait();
            }

            Assert.AreEqual(3, counter);
        }

        [TestMethod]
        public void Execute_RepeatsDisabled_ExecuteOnce()
        {
            int counter = 0;
            Action task = () => counter++;
            TimeSpan interval = TimeSpan.FromMinutes(5);
            TestTimeTracker tracker = new TestTimeTracker();
            ITimerTask timerEvent = new TimerTask(task, interval, timeTracker: tracker);

            for (int i = 0; i < 10; i++)
            {
                tracker.PassTime(TimeSpan.FromMinutes(5));
                timerEvent.Execute().Wait();
            }

            Assert.AreEqual(1, counter);
        }
    }
}
