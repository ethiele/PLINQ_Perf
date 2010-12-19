using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PLINQ_Benchmark
{
    class StatCounter
    {
        System.Threading.Barrier bar = new System.Threading.Barrier(2);
        public void start()
        {
            running = true;
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(startStatCount));
            t.Start();
            bar.SignalAndWait();
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
            f.Add(new List<float>());

            bar.SignalAndWait();
            for (int i = 0; i < logicalCores; i++)
            {
                lst.Add(new PerformanceCounter("Processor", "% Processor Time", i.ToString(), true));
                f.Add(new List<float>()) ;
            }

            while (running)
            {
                float cpu = p1.NextValue();
                List<float> cpuLst = new List<float>();
                //Console.Write("Processor Total : {0}%",  cpu);
                cpuLst.Add(cpu);
                f[0].Add(cpu);
                for (int i = 0; i < logicalCores; i++)
                {
                    cpu = lst[i].NextValue();
                    f[i + 1].Add(cpu);
                    //Console.Write(" Core: {0} : {1}%", i, cpu);
                    cpuLst.Add(cpu);
                }
                Program.cvm.SetCPUStats(cpuLst);
                //Console.WriteLine();
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
