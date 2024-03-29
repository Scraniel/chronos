﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chronos.Timer.Core;
using Chronos.Timer.Mocks;

namespace Chronos.Timer.Tests.Core
{
    [TestClass]
    public class BasicTimerTaskUnitTests
    {
        [TestMethod]
        public void Execute_TimeRemaining_Executes()
        {
            int counter = 0;
            Action task = () => counter++;
            TimeSpan interval = TimeSpan.FromMinutes(5);
            ITimerAction timerEvent = new BasicTimerAction(task, interval, 5);

            timerEvent.ExecuteAsync().Wait();
            Assert.AreEqual(1, counter);
            Assert.AreEqual(4, timerEvent.TimesToExecute);
            Assert.AreEqual(false, timerEvent.IsFinished);
        }

        [TestMethod]
        public void Execute_LastExecution_ExecutesOnce()
        {
            int counter = 0;
            Action task = () => counter++;
            TimeSpan interval = TimeSpan.FromMinutes(5);
            ITimerAction timerEvent = new BasicTimerAction(task, interval, 1);

            for(int i = 0; i < 5; i++)
            {
                timerEvent.ExecuteAsync().Wait();
                Assert.AreEqual(1, counter);
                Assert.AreEqual(0, timerEvent.TimesToExecute);
            }
        }

        [TestMethod]
        public void CanExecute_TimeRemaining_ReturnsTrue()
        {
            Action task = () => { };
            TimeSpan interval = TimeSpan.FromMinutes(5);
            ITimerAction timerEvent = new BasicTimerAction(task, interval, 1);

            Assert.AreEqual(true, timerEvent.CanExecute);
        }

        [TestMethod]
        public void CanExecute_NoTimeRemaining_ReturnsFalse()
        {
            Action task = () => { };
            TimeSpan interval = TimeSpan.FromMinutes(5);
            ITimerAction timerEvent = new BasicTimerAction(task, interval, 1);
            timerEvent.ExecuteAsync().Wait();

            Assert.AreEqual(false, timerEvent.CanExecute);
        }

        [TestMethod]
        public void CanExecute_NoTimeRemaining_Throws()
        {
            Action task = () => { };
            TimeSpan interval = TimeSpan.FromMinutes(5);

            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => new BasicTimerAction(task, interval, 0)
            );
        }

        [TestMethod]
        public void IsFinished_TimeRemaining_ReturnsFalse()
        {
            Action task = () => { };
            TimeSpan interval = TimeSpan.FromMinutes(5);
            ITimerAction timerEvent = new BasicTimerAction(task, interval, 1);

            Assert.AreEqual(false, timerEvent.IsFinished);
        }

        [TestMethod]
        public void IsFinished_NoTimeRemaining_ReturnsTrue()
        {
            Action task = () => { };
            TimeSpan interval = TimeSpan.FromMinutes(5);
            ITimeTrackingStrategy tracker = new TestTimeTrackingStrategy(interval);
            ITimerAction timerEvent = new BasicTimerAction(task, interval, 1);
            timerEvent.ExecuteAsync().Wait();

            Assert.AreEqual(true, timerEvent.IsFinished);
        }
    }
}
