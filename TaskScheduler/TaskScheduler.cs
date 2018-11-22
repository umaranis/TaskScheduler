using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskScheduler
{
    /// <summary>
    /// Features: Fast, Small, Doens't poll, Recurring Tasks
    /// </summary>
    public class TaskScheduler : IDisposable
    {
        private TaskCollection taskQueue;

        private AutoResetEvent autoResetEvent;

        private Thread thread;

        private bool started = false;
        public bool Started { get { return started; } }

        public TaskScheduler()
        {
            taskQueue = new TaskCollection();
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task">Once ITask object is added, it should never be updated from outside TaskScheduler</param>
        public void AddTask(ITask task)
        {
            ITask earliestTask;

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

        public void AddTask(Action taskAction, DateTime startTime)
        {
            AddTask(new RecurringTask(taskAction, startTime, TimeSpan.Zero));
        }

        public void AddTask(Action taskAction, DateTime startTime, TimeSpan recurrence)
        {
            AddTask(new RecurringTask(taskAction, startTime, recurrence));
        }

        private void ReScheduleRecurringTask(ITask task)
        {
            DateTime nextRunTime = task.GetNextRunTime(task.StartTime);
            if (nextRunTime != DateTime.MinValue)
            {
                task.StartTime = nextRunTime;
                lock (taskQueue)
                    taskQueue.Add(task);
                WriteLog("Recurring task # " + task.TaskId + " scheduled for " + task.StartTime.ToString());
            }
        }

        private ITask GetEarliestScheduledTask()
        {
            lock(taskQueue)
            {
                return taskQueue.First();
            }
        }

        public int TaskCount { get { return taskQueue.Count; } }

        public bool RemoveTask(ITask task)
        {
            WriteLog("Removing task # " + task.TaskId);
            lock(taskQueue)
                return taskQueue.Remove(task);
        }

        public bool RemoveTask(string taskId)
        {
            lock(taskQueue)
                return taskQueue.Remove(taskQueue.First(n => n.TaskId == taskId));
        }

        public bool UpdateTask(ITask task, DateTime startTime)
        {
            WriteLog("Updating task # " + task.TaskId + " (Remove & Add)");
            lock(taskQueue)
            {
                if (RemoveTask(task))
                {
                    task.StartTime = startTime;
                    AddTask(task);
                    return true;
                }
            }
            return false;
        }

        private void Run()
        {
            WriteLog("Task Schedular thread starting");
            TimeSpan tolerance = TimeSpan.FromSeconds(1);
            while (started)
            {
                try
                {
                    ITask task = GetEarliestScheduledTask();
                    if(task != null)
                    {
                        if (task.StartTime - DateTime.Now < tolerance)
                        {
                            WriteLog("Starting task " + task.TaskId);
                            try
                            {
                                task.Run();
                            }
                            catch (Exception e)
                            {
                                WriteLog("Exception while running Task # " + task.TaskId);
                                WriteLog(e.ToString());
                            }
                            WriteLog("Completed task " + task.TaskId);
                            lock (taskQueue) taskQueue.Remove(task);
                            ReScheduleRecurringTask(task);
                        }
                        else
                        {
                            TimeSpan waitTime = (task.StartTime - DateTime.Now);
                            TimeSpan min15 = TimeSpan.FromMinutes(15);
                            if (waitTime > min15) waitTime = min15; 

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
                catch (Exception e)
                {
                    WriteLog("Exception: " + e.ToString());
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
