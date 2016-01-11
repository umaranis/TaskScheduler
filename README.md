# TaskSchedular

TaskScheduler is a simple and efficient C# .Net library that runs given tasks at the specified date and time.
 
- Efficient : There is no polling. Only runs when a task is due. This is achieved though AutoResetEvent.
- Simple  : Merely 8 KB in size. Pretty easy to use but addresses limited number of use cases.

[![Build status](https://ci.appveyor.com/api/projects/status/t5h2qs9mgaeu12ma/branch/master?svg=true)](https://ci.appveyor.com/project/umaranis/taskscheduler/branch/master)

# Background
 
.Net Framework comes with various Timer classes which give us the ability to run a task periodically.
 
- [System.Windows.Forms.Timer](https://msdn.microsoft.com/en-us/library/system.windows.forms.timer%28v=vs.110%29.aspx)
- [System.Timers.Timer](https://msdn.microsoft.com/en-us/library/system.timers.timer%28v=vs.110%29.aspx)  
- [System.Threading.Timer](https://msdn.microsoft.com/en-us/library/system.threading.timer%28v=vs.110%29.aspx)
 
Apart from Timers which run periodically, we don't have any class which executes tasks at a given time. A usual work around is to use Timer of a second or so (based on your need), then keep checking if any task is due in timer's event and execute the task when due. In order to be more resource friendly, TaskSchedular takes a different approach instead of continuous polling for task due date.

# How TaskScheduler Works
 
It runs in it's own thread and doesn't consume any CPU resouces till a task is due. This is acheived through Wait Handle ([AutoResetEvent](https://msdn.microsoft.com/en-us/library/system.threading.autoresetevent%28v=vs.110%29.aspx)). All scheduled tasks are executed in the same thread, which means:
- tasks are not running in GUI thread so any code which is related to GUI should be invoked on the GUI thread. See [this blog post](http://umaranis.com/2014/05/25/keeping-ui-responsive-c-net/) for running code in GUI thread using Control.Invoke.
- tasks are never executed in parallel as there is only one thread. This has following upshots:
  - saves us from thread synhronization issues within tasks.
  - this library might not the the right one for you if you need more parallelism. In such a case, check out alternatives like [Quartz.NET](http://www.quartz-scheduler.net/) and [FluentScheduler](https://github.com/jgeurts/FluentScheduler).

# Usage 
 
#### Starting TaskScheduler
 
```
var schedular = new TaskSchedular.TaskSchedular();
schedular.Start();
```

#### Adding task

``` 
schedular.AddTask(new TaskSchedular.Task()
    {
        StartTime = DateTime.Now.AddSeconds(30),
        TaskAction = () =>
        {
            // do some work here
            System.Threading.Thread.Sleep(300);
        },
        Recurrance = TimeSpan.FromSeconds(30)
    });
```

#Notes

- TaskSchedular has a tolerance of 1 second by default, that is, if a task is due within a second, it will execute it right away.
- Tasks can be added to scheduler before starting it. Once the scheduler is started, any overdue task will be executed immediately.
