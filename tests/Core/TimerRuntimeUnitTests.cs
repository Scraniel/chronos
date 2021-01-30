using Chronos.Timer.Core;
using Chronos.Timer.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
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
            ITimer timer = _target.Register<MockTimer>(TimeSpan.FromSeconds(1), 1, () => { });
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
            ITimer timer1 = _target.Register<MockTimer>(TimeSpan.FromSeconds(1), 1, () => { });
            Assert.IsNotNull(timer1);
            
            ITimer timer2 = _target.Register(timer1);
            
            Assert.IsNotNull(timer2);
            Assert.AreEqual(timer1, timer2);

            List<ITimer> timers = _target.GetTimers().ToList();
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
        public async Task Unregister_TimerIsRegistered_StopsUpdating()
        {
            // since this test is time based, we need to minimize the time needed to wait.
            _target.TimeStep = TimeSpan.Zero;

            SemaphoreSlim updateLock = new SemaphoreSlim(1, 1);
            updateLock.Wait();

            bool isUpdated = false;
            _mockTimerFactory.MockTimer = new MockTimer()
            {
                OnUpdateAsync = () =>
                {
                    isUpdated = true;
                    updateLock.Release();
                    return Task.CompletedTask;
                }
            };

            MockTimer timer = _target.Register<MockTimer>(TimeSpan.FromSeconds(1), 1, () => { });
            await updateLock.WaitAsync(_testTimeout).ConfigureAwait(false);
            Assert.IsTrue(isUpdated);
            
            _target.Unregister(timer.Id);
            isUpdated = false;

            Thread.Sleep(TimeSpan.FromMilliseconds(50));
            Assert.IsFalse(isUpdated);
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
            ITimer timer = _target.Register<MockTimer>(TimeSpan.FromSeconds(1), 1, () => { });

            Assert.IsNotNull(_target.GetTimer(timer.Id));
        }

        [TestMethod]
        public void GetTimer_TimerIsNotRegistered_ReturnsNull()
        {
            ITimer timer = _target.GetTimer(Guid.NewGuid());
            Assert.IsNull(timer);
        }

        [TestMethod]
        public void ListTimers_NoTimers_ReturnsEmptyList()
        {
            List<ITimer> timers = _target
                .GetTimers()
                .ToList();

            Assert.AreEqual(0, timers.Count);
        }

        [TestMethod]
        public void ListTimers_ContainsSingleTimer_SingleElement()
        {
            ITimer timer = _target.Register<MockTimer>(TimeSpan.FromSeconds(1), 1, () => { });

            List<ITimer> timers = _target
                .GetTimers()
                .ToList();

            Assert.AreEqual(1, timers.Count);
            Assert.AreEqual(timers.First().Id, timer.Id);
        }

        [TestMethod]
        public void RegisterUnregisterList_ReturnsExpectedTimers()
        {
            List<ITimer> timers = new List<ITimer>();

            int totalTimers = 10;

            for (int i = 0; i < totalTimers; ++i)
            {
                timers.Add(_target.Register<MockTimer>(TimeSpan.FromSeconds(1), 1, () => { }));
            }

            for (int i = 0; i < totalTimers/2; ++i)
            {
                _target.Unregister(timers[i].Id);
            }

            List<ITimer> runtimeTimers = _target.GetTimers().ToList();
            
            Assert.AreEqual(totalTimers - totalTimers / 2, runtimeTimers.Count);
            
            for (int i = totalTimers / 2; i < timers.Count; ++i)
            {
                Assert.IsNotNull(_target.GetTimer(timers[i].Id));
            }
        }
    }
}
