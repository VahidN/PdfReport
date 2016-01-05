using System;
using System.Diagnostics;

namespace DemosBrowser.Toolkit.Helper
{
    public static class Performance
    {
        public static Tuple<long, TimeSpan> RunActionMeasurePerformance(Action action)
        {
            GC.Collect();
            long initMemUsage = Process.GetCurrentProcess().WorkingSet64;

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            action();
            stopwatch.Stop();

            var currentMemUsage = Process.GetCurrentProcess().WorkingSet64;
            var memUsage = currentMemUsage - initMemUsage;
            if (memUsage < 0) memUsage = 0;

            return new Tuple<long, TimeSpan>(memUsage, stopwatch.Elapsed);
        }
    }
}
