using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSchedular
{
    public class RecurringTask : ITask
    {
        public string TaskId { get; set; }

        public DateTime StartTime { get; set; }

        public Action TaskAction { get; set; }

        /// <summary>
        /// TimeSpan.Zero mean null
        /// </summary>
        public TimeSpan Recurrence { get; set; }

        public RecurringTask(Action taskAction, DateTime startTime, TimeSpan recurrence, string taskId = null)
        {
            TaskAction = taskAction;
            StartTime = startTime;
            Recurrence = recurrence;
            TaskId = taskId;
        }               

        public void Run()
        {
            TaskAction();
        }

        public DateTime GetNextRunTime(DateTime lastExecutionTime)
        {
            if (Recurrence != TimeSpan.Zero)
                return lastExecutionTime.Add(Recurrence);
            else
                return DateTime.MinValue;

        }
    }
    

}
