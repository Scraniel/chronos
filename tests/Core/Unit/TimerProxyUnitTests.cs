using Chronos.Timer.Core;
using Chronos.Timer.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Chronos.Timer.Tests.Core
{
    [TestClass]
    public class TimerProxyUnitTests
    {

        [TestMethod]
        public void Constructor_NullTimer_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new TimerProxy(null));
        }

        [TestMethod]
        public void Properties_LeveragesTimerProperties()
        {
            Guid targetId = Guid.NewGuid();
            TimeSpan targetTimeLeft = TimeSpan.FromSeconds(2);
            TimeSpan targetTotalElapsedTime = TimeSpan.FromSeconds(4);

            MockTimer mockTimer = new MockTimer
            {
                Id = targetId,
                TimeLeft = targetTimeLeft,
                TotalElapsedTime = targetTotalElapsedTime
            };

            TimerProxy target = new TimerProxy(mockTimer);
            
            Assert.AreEqual(target.Id, target.Id);
            Assert.AreEqual(target.TimeLeft, targetTimeLeft);
            Assert.AreEqual(target.TotalElapsedTime, targetTotalElapsedTime);
        }

        [TestMethod]
        public void Clear_UnderlyingTimerClears()
        {
            bool clearedCalled = false;
            MockTimer mockTimer = new MockTimer
            {
                OnClear = () => clearedCalled = true
            };

            TimerProxy target = new TimerProxy(mockTimer);
            target.Clear();

            Assert.IsTrue(clearedCalled);
        }

        [TestMethod]
        public void Pause_UnderlyingTimerPauses()
        {
            bool pauseCalled = false;
            MockTimer mockTimer = new MockTimer
            {
                OnPause = () => pauseCalled = true
            };

            TimerProxy target = new TimerProxy(mockTimer);
            target.Pause();

            Assert.IsTrue(pauseCalled);
        }

        [TestMethod]
        public void Unpause_UnderlyingTimerUnpauses()
        {
            bool unpauseCalled = false;
            MockTimer mockTimer = new MockTimer
            {
                OnUnpause = () => unpauseCalled = true
            };

            TimerProxy target = new TimerProxy(mockTimer);
            target.Unpause();

            Assert.IsTrue(unpauseCalled);
        }
    }
}
