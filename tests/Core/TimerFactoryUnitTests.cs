using Chronos.Timer.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chronos.Timer.Tests.Core
{
    [TestClass]
    public class TimerFactoryUnitTests
    {
        private readonly TimerFactory _target;
        private readonly TimerTask _timerTask;
        public TimerFactoryUnitTests()
        {
            _target = new TimerFactory();
            _timerTask = new TimerTask(() => { }, TimeSpan.FromSeconds(1));
        }

        [TestMethod]
        public void CreateTimer_CreateTwoTimer_CreatesDifferentTimers()
        {
            ITimer timer1 = _target.CreateTimer<TestTimer>(_timerTask);
            ITimer timer2 = _target.CreateTimer<TestTimer>(_timerTask);

            Assert.AreNotEqual(timer1, timer2);
        }

        [TestMethod]
        public void CreateTimer_WithSystemTimeTracking_CreatesValidNewTimer()
        {
            TestTimer timer = _target.CreateTimer<TestTimer>(_timerTask, new SystemTimeTracker());

            AssertValidCreation(timer);
            Assert.IsInstanceOfType(timer.TimeTrackingStrategy, typeof(SystemTimeTracker));
        }

        [TestMethod]
        public void CreateTimer_NullTimerTask_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                {
                    _target.CreateTimer<TestTimer>(
                        task: null,
                        timeTracker: null);
                });
        }

        [TestMethod]
        public void CreateTimer_SingleTimerTask_CreatesValidNewTimer()
        {
            TestTimer timer = _target.CreateTimer<TestTimer>(
                new TimerTask(() => { }, TimeSpan.FromSeconds(1)),
                new SystemTimeTracker());

            AssertValidCreation(timer);
            Assert.AreEqual(1, timer.Tasks.Count);
        }

        [TestMethod]
        public void CreateTimer_MultipleTimerTask_CreatesValidNewTimer()
        {
            List<ITimerTask> tasks = new List<ITimerTask>
            {
                new TimerTask(() => { }, TimeSpan.FromSeconds(1)),
                new TimerTask(() => { }, TimeSpan.FromSeconds(1))
            };

            TestTimer timer = _target.CreateTimer<TestTimer>(
                tasks,
                new SystemTimeTracker());

            AssertValidCreation(timer);
            Assert.AreEqual(2, timer.Tasks.Count);
        }

        private void AssertValidCreation(TestTimer timer)
        {
            Assert.IsNotNull(timer);
            Assert.IsTrue(timer.Initialized);
        }

        // TODO Replace with mock implementation
        private class TestTimer : ITimer
        {
            public TimeSpan ElapsedTime => throw new NotImplementedException();

            public TimeSpan TimeLeft => throw new NotImplementedException();

            public bool Initialized { get; set; } = false;

            public ITimeTrackingStrategy TimeTrackingStrategy { get; set; }
            
            public List<ITimerTask> Tasks { get; set; }

            Guid ITimer.Id => Guid.NewGuid();

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

            void ITimer.Initialize(ITimeTrackingStrategy timeTrackingStrategy, IEnumerable<ITimerTask> timerTasks)
            {
                Initialized = true;
                TimeTrackingStrategy = timeTrackingStrategy;
                Tasks = timerTasks.ToList();
            }

            void ITimer.Update()
            {
                throw new NotImplementedException();
            }
        }
    }
}
