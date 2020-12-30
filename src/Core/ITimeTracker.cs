using System;
using System.Collections.Generic;
using System.Text;

namespace Chronos.Core
{
    public interface ITimeTracker
    {
        void Start();

        TimeSpan GetTimeElapsed();
    }
}
