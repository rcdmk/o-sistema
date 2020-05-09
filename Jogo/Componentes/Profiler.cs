using System.Collections.Generic;
using System.Diagnostics;

namespace Jogo.Componentes
{
    class Profiler
    {
        public static List<Profiler> AllProfilers = new List<Profiler>();

        string name;
        double elapsedTime;
        Stopwatch stopwatch;

        public Profiler(string name)
        {
            this.name = name;
            AllProfilers.Add(this);
        }

        public void Start()
        {
            stopwatch = Stopwatch.StartNew();
        }

        public void Stop()
        {
            elapsedTime += stopwatch.Elapsed.TotalSeconds;
        }

        public void Print(double totalTime)
        {
            Trace.WriteLine(string.Format("{0}: {1:F2}%", name, elapsedTime * 100 / totalTime));
            elapsedTime = 0;
        }
    }
}
