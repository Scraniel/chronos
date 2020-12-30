using System;
using System.Threading.Tasks;

namespace Chronos.Core
{
    public interface ITimerEvent
    {
        TimeSpan ExecutionPeriod { get; }
        TimeSpan TimeUntilFinalExecution { get; }
        TimeSpan TimeUntilNextExecution { get; }

        Task Execute();
    }
}