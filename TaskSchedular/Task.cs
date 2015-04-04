using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSchedular
{
    public class Task
    {
        public string TaskId { get; set; }

        public DateTime StartTime { get; set; }

        public Action TaskAction { get; set; }

        /// <summary>
        /// TimeSpan.Zero mean null
        /// </summary>
        public TimeSpan Recurrance { get; set; }
        
    }

    public class TaskComparer : IComparer<Task>
    {
        public int Compare(Task x, Task y)
        {
            return (int)(x.StartTime.Ticks - y.StartTime.Ticks);
        }
    }

}
