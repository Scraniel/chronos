﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chronos.Timer.Core;
using Chronos.Timer.Mocks;
using System.Linq;

namespace Chronos.Timer.Tests.Core
{
    [TestClass]
    public class BasicTimerUnitTests
    {
        private TimeSpan _timeStep = TimeSpan.FromSeconds(10);
        private TimeSpan _taskPeriod = TimeSpan.FromSeconds(5);
        private Action _noOpAction = () => { };

        private ITimer GetBasicTimerAndInitialize(List<Action> tasksToRun)
        {
            ITimer timer = new BasicTimer();
            ITimeTrackingStrategy timeTrackingStrategy = new TestTimeTrackingStrategy(_timeStep);

            List<ITimerAction> tasks = 
                tasksToRun
                    .ConvertAll((Action task) => { return new TestTimerAction(_timeStep, 1, task); })
                    .ToList<ITimerAction>();
            
            timer.Initialize(tasks, timeTrackingStrategy);

            return timer;
        }

        [TestMethod]
        public void Initialize_NullTimeTrackingStrategy_ThrowsArgumentNullException()
        {
            BasicTimer timer = new BasicTimer();

            Assert.ThrowsException<ArgumentNullException>(
                () => timer.Initialize(new List<ITimerAction>(), null));
        }

        [TestMethod]
        public void Pause_WhileStarted_TimePauses()
        {
            ITimer timer = GetBasicTimerAndInitialize(new List<Action> { _noOpAction });
            timer.Pause();
            timer.UpdateAsync().Wait();
            Assert.AreEqual(TimeSpan.Zero, timer.TotalElapsedTime);
        }

        [TestMethod]
        public void UnPause_WhilePaused_TimeStarts()
        {
            ITimer timer = GetBasicTimerAndInitialize(new List<Action> { _noOpAction });
            timer.Pause();
            timer.UpdateAsync().Wait();
            Assert.AreEqual(TimeSpan.Zero, timer.TotalElapsedTime);
            timer.Unpause();
            timer.UpdateAsync().Wait();
            Assert.AreEqual(_timeStep, timer.TotalElapsedTime);
        }

        [TestMethod]
        public void IsPaused_WhilePaused_ReturnTrue()
        {
            ITimer timer = GetBasicTimerAndInitialize(new List<Action> { _noOpAction });
            timer.Pause();
            Assert.IsTrue(timer.IsPaused());
        }
        
        [TestMethod]
        public void IsPaused_WhileStarted_ReturnFalse()
        {
            ITimer timer = GetBasicTimerAndInitialize(new List<Action> { _noOpAction });
            Assert.IsFalse(timer.IsPaused());
        }

        [TestMethod]
        public void Clear_WhileStarted_ResetsElapsed()
        {
            ITimer timer = GetBasicTimerAndInitialize(new List<Action> { _noOpAction });

            for(int i = 0; i < 5; i++)
            {
                timer.UpdateAsync().Wait();
            }
            Assert.IsTrue(timer.TotalElapsedTime > TimeSpan.Zero);

            timer.Clear();
            Assert.AreEqual(TimeSpan.Zero, timer.TotalElapsedTime);
        }

        [TestMethod]
        public void UpdateAsync_SeveralUpdates_ElapsedIncreases()
        {
            ITimer timer = GetBasicTimerAndInitialize(new List<Action> { _noOpAction });
            int numUpdates = 5;
            for (int i = 0; i < numUpdates; i++)
            {
                timer.UpdateAsync().Wait();
            }
            Assert.AreEqual(_timeStep * numUpdates, timer.TotalElapsedTime);
        }

        [TestMethod]
        public void UpdateAsync_OneTask_TaskRuns()
        {
            int counter = 0; 
            ITimer timer = GetBasicTimerAndInitialize(new List<Action> { () => { counter++; } });

            int numUpdates = (int)Math.Ceiling(_taskPeriod / _timeStep);
            for (int i = 0; i < numUpdates; i++)
            {
                timer.UpdateAsync().Wait();
            }
            Assert.AreEqual(1, counter);
        }

        [TestMethod]
        public void UpdateAsync_MultipleTasksSamePeriod_TasksRun()
        {
            int firstCounter = 0;
            int secondCounter = 0;
            int thirdCounter = 0;
            List<Action> tasksToRun = new List<Action> {
                () => { firstCounter++; },
                () => { secondCounter++; },
                () => { thirdCounter++; }
            };

            ITimer timer = GetBasicTimerAndInitialize(tasksToRun);

            int numUpdates = (int)Math.Ceiling(_taskPeriod / _timeStep);
            for (int i = 0; i < numUpdates; i++)
            {
                timer.UpdateAsync().Wait();
            }
            Assert.AreEqual(1, firstCounter);
            Assert.AreEqual(1, secondCounter);
            Assert.AreEqual(1, thirdCounter);
        }

        [TestMethod]
        public void UpdateAsync_MultipleTasksDifferentPeriods_TasksRun()
        {
            int firstCounter = 0;
            int secondCounter = 0;
            int thirdCounter = 0;
            ITimer timer = new BasicTimer();
            ITimeTrackingStrategy timeTrackingStrategy = new TestTimeTrackingStrategy(_timeStep);

            // If we run 3 updates, first counter should update 3 times, second 2 times, first 1 time.
            //
            List<ITimerAction> tasks = new List<ITimerAction>
            {
                new TestTimerAction(_timeStep, 3, () => { firstCounter++; }),
                new TestTimerAction(_timeStep * 2, 2, () => { secondCounter++; }),
                new TestTimerAction(_timeStep * 3, 1, () => { thirdCounter++; }),
            };          
            timer.Initialize(tasks, timeTrackingStrategy);

            timer.UpdateAsync().Wait();
            Assert.AreEqual(1, firstCounter);
            Assert.AreEqual(0, secondCounter);
            Assert.AreEqual(0, thirdCounter);

            timer.UpdateAsync().Wait();
            Assert.AreEqual(2, firstCounter);
            Assert.AreEqual(1, secondCounter);
            Assert.AreEqual(0, thirdCounter);

            timer.UpdateAsync().Wait();
            Assert.AreEqual(3, firstCounter);
            Assert.AreEqual(1, secondCounter);
            Assert.AreEqual(1, thirdCounter);
        }
    }
}
