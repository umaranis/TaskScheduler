using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSchedular
{
    public interface ITask
    {
        string TaskId { get; set; }

        /// <summary>
        /// Time when task is going to start running.
        /// StartTime is updated after every execution based on NextRunTime (recurrence)<see cref="GetNextRunTime(DateTime)"/>
        /// </summary>
        DateTime StartTime { get; set; }

        /// <summary>
        /// Method executed when task is due
        /// </summary>
        void Run();

        /// <summary>
        /// Provide the NextRunTime based on <paramref name="lastExecutionTime"/>.
        /// Returns DateTime.MinValue if task doesn't recurr.
        /// </summary>
        /// <param name="lastExecutionTime"></param>
        /// <returns>Returns DateTime.MinValue if task doesn't recurr</returns>
        DateTime GetNextRunTime(DateTime lastExecutionTime);
    }


    public class TaskComparer : IComparer<ITask>
    {
        public int Compare(ITask x, ITask y)
        {
            return (int)(x.StartTime.Ticks - y.StartTime.Ticks);
        }
    }
}
