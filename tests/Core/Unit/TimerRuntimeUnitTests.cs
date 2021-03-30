using Chronos.Timer.Core;
using Chronos.Timer.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Chronos.Timer.Tests.Core
{
    [TestClass]
    public class TimerRuntimeUnitTests
    {
        private TimerRuntime _target;
        private MockTimerFactory _mockTimerFactory;
        private TimeSpan _testTimeout = TimeSpan.FromMilliseconds(250);
        
        [TestInitialize]
        public void Initialize()
        {
            _mockTimerFactory = new MockTimerFactory();
            _target = new TimerRuntime(_mockTimerFactory);
        }

        [TestMethod]
        public void Register_CreatesTimerSuccessfully_TimerIsRegistered()
        {
            TimerProxy timer = _target.Register<MockTimer>(TimeSpan.FromSeconds(1), 1, () => { });
            Assert.IsNotNull(timer);
            Assert.IsNotNull(_target.GetTimer(timer.Id));
        }

        [TestMethod]
        public async Task Register_FirstTimerCreated_StartsUpdatingTimer()
        {
            SemaphoreSlim slim = new SemaphoreSlim(1, 1);
            slim.Wait();

            bool isUpdated = false;
            _mockTimerFactory.MockTimer = new MockTimer()
            {
                OnUpdateAsync = () =>
                {
                    isUpdated = true;
                    slim.Release();
                    return Task.CompletedTask;
                }
            };

            _target.Register<MockTimer>(TimeSpan.FromSeconds(1), 1, () => { });
            
            await slim.WaitAsync(_testTimeout).ConfigureAwait(false);
            Assert.IsTrue(isUpdated);
        }

        [TestMethod]
        public void Register_RegisterSameTimerTwice_SingleRegistration()
        {
            MockTimer timer = _mockTimerFactory.CreateTimer<MockTimer>(
                new BasicTimerAction(() => { }, TimeSpan.FromSeconds(1), 1));

            TimerProxy timer1 = _target.Register(timer);
            Assert.IsNotNull(timer);
            
            TimerProxy timer2 = _target.Register(timer);
            
            Assert.IsNotNull(timer2);
            Assert.AreEqual(timer.Id, timer2.Id);

            List<TimerProxy> timers = _target.GetTimers().ToList();
            Assert.AreEqual(1, timers.Count);
        }

        [TestMethod]
        public void Register_RegisterTwoUniqueTimers_CreatesTwoTimers()
        {
            int createCount = 0;
            _mockTimerFactory.OnCreateTimer = () =>
            {
                createCount++;
                return new MockTimer();
            };

            _target.Register<MockTimer>(TimeSpan.FromSeconds(1), 1, () => { });
            _target.Register<MockTimer>(TimeSpan.FromSeconds(1), 1, () => { });

            Assert.AreEqual(2, createCount);
        }

        [TestMethod]
        public void Unregister_TimerIsNotRegistered_NoOp()
        {
            _target.Register<MockTimer>(TimeSpan.FromSeconds(1), 1, () => { });
            
            Assert.AreEqual(1, _target.GetTimers().ToList().Count);

            _target.Unregister(Guid.NewGuid());

            Assert.AreEqual(1, _target.GetTimers().ToList().Count);
        }

        [TestMethod]
        public void GetTimer_TimerIsRegistered_ReturnsTimer()
        {
            TimerProxy timer = _target.Register<MockTimer>(TimeSpan.FromSeconds(1), 1, () => { });

            Assert.IsNotNull(_target.GetTimer(timer.Id));
        }

        [TestMethod]
        public void GetTimer_TimerIsNotRegistered_ReturnsNull()
        {
            TimerProxy timer = _target.GetTimer(Guid.NewGuid());
            Assert.IsNull(timer);
        }

        [TestMethod]
        public void ListTimers_NoTimers_ReturnsEmptyList()
        {
            List<TimerProxy> timers = _target
                .GetTimers()
                .ToList();

            Assert.AreEqual(0, timers.Count);
        }

        [TestMethod]
        public void ListTimers_ContainsSingleTimer_SingleElement()
        {
            TimerProxy timer = _target.Register<MockTimer>(TimeSpan.FromSeconds(1), 1, () => { });

            List<TimerProxy> timers = _target
                .GetTimers()
                .ToList();

            Assert.AreEqual(1, timers.Count);
            Assert.AreEqual(timers.First().Id, timer.Id);
        }

        [TestMethod]
        public void RegisterUnregisterList_ReturnsExpectedTimers()
        {
            List<TimerProxy> timers = new List<TimerProxy>();

            int totalTimers = 10;

            for (int i = 0; i < totalTimers; ++i)
            {
                timers.Add(_target.Register<MockTimer>(TimeSpan.FromSeconds(1), 1, () => { }));
            }

            for (int i = 0; i < totalTimers/2; ++i)
            {
                _target.Unregister(timers[i].Id);
            }

            List<TimerProxy> runtimeTimers = _target.GetTimers().ToList();
            
            Assert.AreEqual(totalTimers - totalTimers / 2, runtimeTimers.Count);
            
            for (int i = totalTimers / 2; i < timers.Count; ++i)
            {
                Assert.IsNotNull(_target.GetTimer(timers[i].Id));
            }
        }
    }
}
