using Chronos.Timer.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Timer.Mocks
{
    public class TestTimerAction : ITimerAction
    {
        public TimeSpan Period { get; set; }

        public int TimesToExecute { get; set; }

        public bool CanExecute { get; set; }

        public bool IsFinished { get; set; }

        public Action TaskToRun { get; set; }

        public TestTimerAction(TimeSpan period, int timesToExecute, Action taskToRun)
        {
            Period = period;
            TimesToExecute = timesToExecute;
            TaskToRun = taskToRun;
            CanExecute = true;
            IsFinished = false;
        }

        public async Task ExecuteAsync()
        {
            await Task.Run(TaskToRun);
        }
    }
}
