using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PLINQ_Benchmark
{
    class StatCounter
    {
        public void start()
        {
            running = true;
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(startStatCount));
            t.Start();
        }

        public List<List<float>> end()
        {
            running = false;
            return f;
        }

        bool running;
        List<List<float>> f;
        void startStatCount()
        {
            f = new List<List<float>>();
            int logicalCores = Environment.ProcessorCount;
            var p1 = new PerformanceCounter("Processor", "% Processor Time", "_Total", true);
            var lst = new List<PerformanceCounter>();
            //var p2 = new PerformanceCounter("Processor", "% Processor Time", "0", true);
            //p3 = new PerformanceCounter("Processor", "% Processor Time", "1", true);
            f.Add(new List<float>());
            for (int i = 0; i < logicalCores; i++)
            {
                lst.Add(new PerformanceCounter("Processor", "% Processor Time", i.ToString(), true));
                f.Add(new List<float>()) ;
            }

            while (running)
            {
                float cpu = p1.NextValue();
                Console.WriteLine("Processor Total : {0}%",  cpu);
                f[0].Add(cpu);
                for (int i = 0; i < logicalCores; i++)
                {
                    cpu = lst[i].NextValue();
                    f[i + 1].Add(cpu);
                    Console.WriteLine("Processor {0} : {1}%", i, cpu);
                }
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
