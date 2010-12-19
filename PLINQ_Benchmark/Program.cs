using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLINQ_Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var vals = LoadList("vals.cvs");
            List<long> lst = new List<long>();
            Console.WriteLine("Running on a system with {0} logical cores", System.Environment.ProcessorCount);
            Console.WriteLine("Starting evaluation");
            for (int i = 1; i < 4; i++)
            {
                var time = ProcessList(vals, i);
                lst.Add(time);
                Console.WriteLine("Using {0} core(s) took {1} ms", i.ToString(), time.ToString());
            }
            SaveList(lst, "results.cvs");
            Console.ReadLine();
            
        }

        static long ProcessList(List<long> lst, int plvl)
        {
            StatCounter sc = new StatCounter();
            sc.start();
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var result = from p in lst.AsParallel().WithDegreeOfParallelism(plvl)
                         select pfac.prime_factorization(p);
            foreach (var item in result)
            {
                
            }
            sw.Stop();
            var counts = sc.end();
            SaveList(counts[0], "CPU_total.csv");
            for (int i = 1; i < counts.Count; i++)
            {
                SaveList(counts[0], string.Format("CPU_{0}_plvl{1}.csv", i - 1, plvl));
            }
            return sw.ElapsedMilliseconds;   
        }

        static List<long> CreateRandomDoubles(int count)
        {
            List<long> lst = new List<long>();
            Random rnd = new Random();
            for (int i = 0; i < count; i++)
            {
                long upper = rnd.Next();
                long lower = rnd.Next();
                upper = upper << 32;
                lst.Add(lower | upper);
            }
            return lst;
        }

        static void SaveList(List<long> lst, string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in lst)
            {
                sb.Append(item);
                sb.Append(",");
            }
            if (lst.Count > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            System.IO.File.WriteAllText(str, sb.ToString());
        }

        static void SaveList(List<float> lst, string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in lst)
            {
                sb.Append(item);
                sb.Append(",");
            }
            if (lst.Count > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            System.IO.File.WriteAllText(str, sb.ToString());
        }

        static List<long> LoadList(string str)
        {
            string strLst = System.IO.File.ReadAllText(str);
            return strLst.Split(',').Select(p => long.Parse(p)).ToList();
        }
    }
}