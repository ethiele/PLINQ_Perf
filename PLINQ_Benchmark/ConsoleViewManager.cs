using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLINQ_Benchmark
{
    public class ConsoleViewManager
    {
        List<TaskInfo> TaskList = new List<TaskInfo>();
        List<float> cpuStats;
        System.Diagnostics.Stopwatch globalSW = new System.Diagnostics.Stopwatch();
        int activeTasks = 0;

        public List<TaskInfo> ListOfTasks
        {
            get
            {
                return TaskList;
            }
        }

        public void AddTask(long ID)
        {
            TaskList.Add(new TaskInfo(){ ID= ID, State= TaskState.NotStarted, TimeTaken = 0});
            PrintScreen();
        }

        public void ClearList()
        {
            TaskList.Clear();
            globalSW.Stop();
            globalSW.Start();
        }

        public void SetWorking(long ID)
        {
            var result = TaskList.Where(p => p.ID == ID).Single();
            activeTasks++;
            result.State = TaskState.Working;
            result.Start = globalSW.ElapsedMilliseconds;
            PrintScreen();
        }

        public void SetDone(long ID, long TimeTaken)
        {
            var result = TaskList.Where(p => p.ID == ID).Single();
            activeTasks--;
            result.State = TaskState.Done;
            result.TimeTaken = TimeTaken;
            result.End = globalSW.ElapsedMilliseconds;
            PrintScreen();
        }

        public void SetCPUStats(List<float> stats)
        {
            cpuStats = stats;
            PrintScreen();
        }
        public StringBuilder OutputMessages = new StringBuilder();

        public void WriteLine(string str)
        {
            OutputMessages.Append(str);
            PrintScreen();
        }

        public void WriteLine(string str, params object[] objs)
        {
            OutputMessages.Append(string.Format(str, objs));
            PrintScreen();
        }
        System.Threading.Semaphore s = new System.Threading.Semaphore(1, 1);
        public void PrintScreen()
        {
            s.WaitOne();
            //Console.Clear();
            int cx;
            int cy;
            cx = Console.CursorLeft;
            cy = Console.CursorTop;
         

            foreach (var t in TaskList)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(t.ID.ToString());
                sb.Append(' ', 20 - sb.Length);
                Console.Write(sb.ToString());
                sb.Clear();

                if (t.State == TaskState.NotStarted) Console.BackgroundColor = ConsoleColor.Red;
                if (t.State == TaskState.Done) Console.BackgroundColor = ConsoleColor.Green;
                if (t.State == TaskState.Working) Console.BackgroundColor = ConsoleColor.DarkCyan;

                sb.Append(t.State.ToString());
                sb.Append(' ', 20 - sb.Length);
                Console.Write(sb.ToString());
                sb.Clear();

                Console.BackgroundColor = ConsoleColor.Black;
                sb.Append(t.TimeTaken.ToString());
                sb.Append(' ', 20 - sb.Length);
                Console.WriteLine(sb.ToString());
            }

            if (cpuStats != null)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in cpuStats)
                {
                    //Console.Write(item.ToString());
                    //Console.Write(" ");
                    sb.Append(item.ToString());
                    sb.Append(" ");
                }
                sb.Append(' ', 60 - sb.Length);
                Console.Write(sb.ToString());
                
            }
            Console.WriteLine();
            Console.WriteLine("Active Tasks: {0}", activeTasks);
            Console.WriteLine(OutputMessages.ToString());
            Console.SetCursorPosition(cx, cy);
            s.Release();
        }
    }

    public class TaskInfo
    {
        public long ID;
        public TaskState State;
        public long Start;
        public long End;
        public long TimeTaken;

        public override string ToString()
        {
            return ID.ToString() + "," + Start.ToString() + "," + End.ToString();
        }
    }

    public enum TaskState
    {
        NotStarted,
        Working,
        Done
    }
}
