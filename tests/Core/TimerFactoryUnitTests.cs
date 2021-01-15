using Chronos.Timer.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        public void Execute_TimeRemaining_NoOp()
        {
            //_target.CreateTimer<BasicTimer>(
        }

        private class TestTimer
        {
        }
    }
}
