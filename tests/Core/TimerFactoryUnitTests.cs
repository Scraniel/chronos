using Chronos.Timer.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chronos.Timer.Tests.Core
{
    [TestClass]
    public class TimerFactoryUnitTests
    {
        private readonly TimerFactory _target;

        public TimerFactoryUnitTests()
        {
            _target = new TimerFactory();
        }

        [TestMethod]
        public void CreateTimer_TimeTrackingIsNull_CreatesValidNewTimer()
        {
            TestTimer timer = _target.CreateTimer<TestTimer>() as TestTimer;

            AssertValidCreation(timer);
            Assert.IsNull(timer.TimeTrackingStrategy);
            Assert.AreEqual(0, timer.Tasks.Count);
        }

        [TestMethod]
        public void CreateTimer_CreateTwoTimer_CreatesDifferentTimers()
        {
            ITimer timer1 = _target.CreateTimer<TestTimer>();
            ITimer timer2 = _target.CreateTimer<TestTimer>();

            Assert.AreNotEqual(timer1, timer2);
        }

        [TestMethod]
        public void CreateTimer_SystemTimeTracking_CreatesValidNewTimer()
        {
            TestTimer timer = _target.CreateTimer<TestTimer>(new SystemTimeTrackingStrategy()) as TestTimer;

            AssertValidCreation(timer);
            Assert.IsInstanceOfType(timer.TimeTrackingStrategy, typeof(SystemTimeTrackingStrategy));
        }

        [TestMethod]
        public void CreateTimer_NullTimerTask_CreatesValidNewTimer()
        {
            TestTimer timer = _target.CreateTimer<TestTimer>(new SystemTimeTrackingStrategy()) as TestTimer;

            AssertValidCreation(timer);
            Assert.AreEqual(0, timer.Tasks.Count);
        }

        [TestMethod]
        public void CreateTimer_SingleTimerTask_CreatesValidNewTimer()
        {
            TestTimer timer = _target.CreateTimer<TestTimer>(
                new BasicTimerTask(() => { }, TimeSpan.FromSeconds(1), 1),
                new SystemTimeTrackingStrategy()) as TestTimer;

            AssertValidCreation(timer);
            Assert.AreEqual(1, timer.Tasks.Count);
        }

        [TestMethod]
        public void CreateTimer_MultipleTimerTask_CreatesValidNewTimer()
        {
            List<ITimerTask> tasks = new List<ITimerTask>
            {
                new BasicTimerTask(() => { }, TimeSpan.FromSeconds(1), 1),
                new BasicTimerTask(() => { }, TimeSpan.FromSeconds(1), 1)
            };

            TestTimer timer = _target.CreateTimer<TestTimer>(
                tasks,
                new SystemTimeTrackingStrategy()) as TestTimer;

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

            void ITimer.Initialize(ITimeTrackingStrategy timeTrackingStrategy, List<ITimerTask> timerTasks)
            {
                Initialized = true;
                TimeTrackingStrategy = timeTrackingStrategy;
                Tasks = timerTasks;
            }
            Task ITimer.Update()
            {
                throw new NotImplementedException();
            }
        }
    }
}
