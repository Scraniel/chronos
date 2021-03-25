using Chronos.Timer.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chronos.Timer.Mocks
{
    public class MockTimer : ITimer
    {
        public TimeSpan TotalElapsedTime { get; set; }

        public TimeSpan TimeLeft { get; set; }

        public Guid Id { get; set; } = Guid.NewGuid();

        #region Events
        public Action OnClear { get; set; }
        public Func<bool> OnIsPaused { get; set; } = () => false;
        public Action OnPause { get; set; }
        public Action OnUnPause { get; set; }
        public Func<Task> OnUpdateAsync { get; set; }
        public Action OnInitialize { get; set; }
        #endregion

        public void Clear()
            => OnClear?.Invoke();

        public bool IsPaused()
            => OnIsPaused?.Invoke() ?? throw new NotImplementedException();

        public void Pause()
            => OnPause?.Invoke();

        public void Unpause()
            => OnUnPause?.Invoke();

        public Task UpdateAsync()
            => OnUpdateAsync?.Invoke();

        public void Initialize(IEnumerable<ITimerAction> timerTasks, ITimeTrackingStrategy timeTrackingStrategy)
            => OnInitialize?.Invoke();

        /// <summary>
        /// Creates a shallow copy of the mock timer.
        /// </summary>
        /// <returns></returns>
        public MockTimer Clone()
        {
            return new MockTimer
            {
                Id = Id,
                TotalElapsedTime = TotalElapsedTime,
                TimeLeft = TimeLeft,
                OnClear = (Action)OnClear?.Clone(),
                OnInitialize = (Action)OnInitialize?.Clone(),
                OnIsPaused = (Func<bool>)OnIsPaused?.Clone(),
                OnPause = (Action)OnPause?.Clone(),
                OnUnPause = (Action)OnUnPause?.Clone(),
                OnUpdateAsync = (Func<Task>)OnUpdateAsync?.Clone(),
            };
        }

    }
}
