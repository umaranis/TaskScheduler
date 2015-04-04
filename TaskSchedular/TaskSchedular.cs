using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskSchedular
{
    /// <summary>
    /// Features: Fast, Small, Doens't poll, Recurring Tasks
    /// </summary>
    public class TaskSchedular : IDisposable
    {
        private SortedSet<Task> taskQueue;

        private AutoResetEvent autoResetEvent;

        private Thread thread;

        private bool started = false;
        public bool Started { get { return started; } }

        public TaskSchedular()
        {
            taskQueue = new SortedSet<Task>(new TaskComparer());
            autoResetEvent = new AutoResetEvent(false);            
        }

        /// <summary>
        /// Start running tasks
        /// </summary>
        public void Start()
        {
            lock (taskQueue)
            {
                if (!started)
                {
                    started = true;
                    thread = new Thread(Run);
                    thread.Start();
                }
            }
        }

        public void Stop()
        {
            WriteLog("Task Schedular thread stopping");
            started = false;            
            autoResetEvent.Set();
            WriteLog("AutoResetEvent set called");
            thread.Join();
            WriteLog("Task Schedular thread stopped");
        }

        public void Dispose()
        {
            Stop();
            autoResetEvent.Dispose();
        }

        public void AddTask(Task task)
        {
            Task earliestTask;

            lock(taskQueue)
            {
                earliestTask = GetEarliestScheduledTask();                
                taskQueue.Add(task);
            }
            WriteLog("Added task # " + task.TaskId);
            
            if (earliestTask == null || task.StartTime < earliestTask.StartTime)
            {
                autoResetEvent.Set();
                WriteLog("AutoResetEvent is Set");
            }
            
        }

        private void ReScheduleRecurringTask(Task task)
        {
            if (task.Recurrance != TimeSpan.Zero)
            {
                task.StartTime = task.StartTime.Add(task.Recurrance);
                lock (taskQueue)
                    taskQueue.Add(task);
                WriteLog("Recurring task # " + task.TaskId + " scheduled for " + task.StartTime.ToString());
            }
        }

        public Task GetEarliestScheduledTask()
        {
            lock(taskQueue)
            {
                using (IEnumerator<Task> e = taskQueue.GetEnumerator())
                {
                    if (e.MoveNext()) 
                        return e.Current;
                    else 
                        return null;
                }
            }
        }

        public int TaskCount { get { return taskQueue.Count; } }

        private void Run()
        {
            Console.WriteLine(DateTime.Now.ToString() + ": Task Schedular thread starting");
            TimeSpan tolerance = TimeSpan.FromSeconds(1);
            while (started)
            {
                try
                {
                    Task task = GetEarliestScheduledTask();
                    if(task != null)
                    {
                        if (task.StartTime - DateTime.Now < tolerance)
                        {
                            WriteLog("Starting task " + task.TaskId);
                            task.TaskAction();
                            WriteLog("Completed task " + task.TaskId);
                            lock (taskQueue) taskQueue.Remove(task);
                            ReScheduleRecurringTask(task);
                        }
                        else
                        {
                            TimeSpan waitTime = (task.StartTime - DateTime.Now);

                            WriteLog("Schedular thread waiting for " + waitTime.ToString());
                            autoResetEvent.WaitOne(waitTime);
                            WriteLog("Schedular thread awakening from sleep " + waitTime.ToString());
                        }
                    }
                    else
                    {
                        WriteLog("Schedular thread waiting indefinitely");
                        autoResetEvent.WaitOne();
                        WriteLog("Schedular thread awakening from indefinite sleep");
                    }
                }
                catch (Exception)
                {

                } 
            }
                
        }

        [System.Diagnostics.Conditional("DEBUG")]
        private void SetupConsoleListener()
        {
            System.Diagnostics.Debug.Listeners.Clear();
            System.Diagnostics.Debug.Listeners.Add(new System.Diagnostics.ConsoleTraceListener());
        }

        [System.Diagnostics.Conditional("DEBUG")]
        private void WriteLog(string message)
        {
            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": " + message);
        }
    }
}
