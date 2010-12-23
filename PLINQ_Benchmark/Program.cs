using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLINQ_Benchmark
{
    class Program
    {
        public static ConsoleViewManager cvm = new ConsoleViewManager();
        static void Main(string[] args)
        {
            var vals = LoadList("valsmore.cvs");
            List<long> lst = new List<long>();

            Console.WriteLine("Running on a system with {0} logical cores", System.Environment.ProcessorCount);
            Console.WriteLine("Press enter to start");
            Console.ReadLine();
            Console.WriteLine("Starting evaluation");
            System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.RealTime;
            for (int i = 1; i < 8; i++)
            {
                var time = ProcessList(vals, i);
                lst.Add(time);
                Program.cvm.WriteLine("Using {0} core(s) took {1} ms", i.ToString(), time.ToString());
            }
            SaveList(lst, "results.csv");
            Console.ReadLine();
            
        }

        static long ProcessList(List<long> lst, int plvl)
        {
            cvm.ClearList();
            StatCounter sc = new StatCounter();
            foreach (long num in lst)
            {
                Program.cvm.AddTask(num);
            }
            sc.start();
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var lstP =  System.Collections.Concurrent.Partitioner.Create(lst, true);
            var result = from p in lstP.AsParallel().WithDegreeOfParallelism(plvl).WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                         select primeFactor(p);
            foreach (var item in result)
            {
                
            }
            sw.Stop();
            var counts = sc.end();
            //SaveList(counts[0], "CPU_total.csv");
            List<string> headers = new List<string>();
            headers.Add("CPU_total.csv");
            for (int i = 1; i < counts.Count; i++)
            {
                //SaveList(counts[0], string.Format("CPU_{0}_plvl{1}.csv", i - 1, plvl));
                headers.Add(string.Format("CPU_{0}_plvl{1}", i - 1, plvl));
            }
            SaveCSV(counts, headers.ToArray(), string.Format("CPU_Table_P{0}.csv", plvl));
            SaveTaskList(cvm.ListOfTasks, string.Format("TaskData_PLevel{0}.csv", plvl));
            return sw.ElapsedMilliseconds;   
        }

        private static List<long> primeFactor(long p)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            //Console.WriteLine("Starting " + p.ToString() + "at" + System.DateTime.Now.Ticks);
            Program.cvm.SetWorking(p);
            sw.Start();
            var result =  pfac.prime_factorization(p);
            sw.Stop();
            //Console.WriteLine("Finished " + p.ToString() + "at" + System.DateTime.Now.Ticks);
            //Console.WriteLine("It took{0} seconds", sw.ElapsedMilliseconds);
            Program.cvm.SetDone(p, sw.ElapsedMilliseconds);
            return result;
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

        static void SaveTaskList(List<TaskInfo> lst, string path)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in lst)
	        {
                sb.AppendLine(item.ToString());
	        }
            System.IO.File.WriteAllText(path, sb.ToString());
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

        static void SaveCSV(List<List<float>> tbl, string[] Headers, string filePath)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string str in Headers)
            {
                sb.Append(str);
                sb.Append(",");
            }
            if (Headers.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }

            sb.AppendLine();

            int longestCol = (from p in tbl
                              select p.Count).Max();

            for (int y = 0; y < longestCol; y++)
            {
                for (int x = 0; x < tbl.Count; x++)
                {
                    if (tbl[x].Count > y)
                    {
                        sb.Append(tbl[x][y]);
                    }
                    sb.Append(',');
                }
                sb.AppendLine();
            }
            System.IO.File.WriteAllText(filePath, sb.ToString());
        }

        static List<long> LoadList(string str)
        {
            string strLst = System.IO.File.ReadAllText(str);
            return strLst.Split(',').Select(p => long.Parse(p)).ToList();
        }
    }
}