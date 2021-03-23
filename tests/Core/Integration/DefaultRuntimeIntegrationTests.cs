using Chronos.Timer.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace Chronos.Timer.Tests.Core
{
    [TestClass]
    public class DefaultRuntimeIntegrationTests
    {
        private TimerRuntime _target;

        [TestInitialize]
        public void Initialize()
        {
            _target = new TimerRuntime();
        }

        [TestMethod, Timeout(50)]
        public void Execute_TimeRemaining_Executes()
        {
            bool hasExecuted = false;
            _target.Register<BasicTimer>(TimeSpan.FromMilliseconds(20), 1, () => { hasExecuted = true; });

            while (!hasExecuted)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(10));
            }
        }
    }
}
