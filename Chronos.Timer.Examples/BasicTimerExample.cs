using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Chronos.Timer.Core;

namespace Chronos.Timer.Examples
{
    class BasicTimerExample
    {
        static void Main(string[] args)
        {
            int numRepeats = 10; 
            long eventPeriodMs = 200;
            long idealTimeMs = 0;
            _endLine = numRepeats + _startLine + 1;

            Console.WriteLine(_lineBreak);
            Console.WriteLine(string.Format("This example should trigger an event every {0} ms, repeating {1} times.", eventPeriodMs, numRepeats));
            Console.WriteLine(_lineBreak);

            // The ITimerRuntime maintains a list of your timers and ensures they get run at the correct times for the correct duration.
            //
            ITimerRuntime timerRuntime = new TimerRuntime();

            Stopwatch watch = new Stopwatch();
            watch.Start();

            // This is the action we want the timer to execute: it simply logs how close we were to ideal timings.
            //
            Action logIdealTime = () =>
            {
                long currentTime = watch.ElapsedMilliseconds;
                long delta = Math.Abs(Interlocked.Add(ref idealTimeMs, eventPeriodMs) - currentTime);
                Interlocked.Add(ref _totalDelay, delta);
                long executions = Interlocked.Increment(ref _executionCount);

                lock (_writeLock)
                {
                    Console.SetCursorPosition(0, _startLine + _executionCount);
                    Console.WriteLine(string.Format("Execution #{0} after {1} ms ({2}ms off ideal time)", executions, currentTime, delta));
                }
            };

            // TODO: fix bug in runtime. 
            //
            // If all you want to do is get events running periodically, all that's required is a registration.
            //
            // timerRuntime.Register<BasicTimer>(TimeSpan.FromMilliseconds(eventPeriodMs), numRepeats, logIdealTime);

            // For the more advanced use-case:
            // - ITimerAction wraps your regular action, encapsulating timer info alongside the delegate.
            // - ITimerFactory tells the runtime what type of ITimer to create
            // - ITimeTrackingStrategy tells the runtime how to measure the amount of time that passes
            //     - For example, SystemTimeTrackingStrategy simply uses the system clock, whereas a "TurnBasedTrackingStrategy" might use discrete turns
            //
            ITimerAction action = new BasicTimerAction(logIdealTime, TimeSpan.FromMilliseconds(eventPeriodMs), numRepeats);
            ITimerFactory factory = new TimerFactory();
            ITimeTrackingStrategy strategy = new SystemTimeTrackingStrategy();
            timerRuntime.Register(factory.CreateTimer<BasicTimer>(action, strategy));

            PrintEndLine();

            // Run task async, allow break early if key is pressed.
            //
            _ = PrintStats();
            Console.ReadKey();
        }

        private static int _endLine;
        private static int _startLine = 2;
        private static string _lineBreak = "=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=";
        private static object _writeLock = new object();

        private static long _totalDelay = 0;
        private static int _executionCount = 0;

        private static void PrintEndLine()
        {
            lock (_writeLock)
            {
                Console.SetCursorPosition(0, _endLine);
                Console.WriteLine(_lineBreak);
                Console.WriteLine("\n\nPress any key to Exit.");
                Console.SetCursorPosition(0, _startLine);
            }
        }

        static async Task PrintStats()
        {
            int refreshRateMs = 500;

            await Task.Run(() =>
            {
                while (true)
                {
                    lock (_writeLock)
                    {
                        int executions = _executionCount == 0 ? 1 : _executionCount;
                        Console.SetCursorPosition(0, _endLine + 1);
                        Console.WriteLine(string.Format("TOTAL DELAY: {0} ms         AVERAGE DELAY: {1} ms         ", _totalDelay, _totalDelay / executions));
                    }

                    Thread.Sleep(refreshRateMs);
                }
            });
        }
    }
}
