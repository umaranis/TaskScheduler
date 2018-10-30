using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TaskSchedularTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private TaskScheduler.TaskScheduler schedular = new TaskScheduler.TaskScheduler();

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            schedular.Start();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            schedular.Stop();
        }

        private string taskNumber = "1";
        private void btnAddTask_Click(object sender, RoutedEventArgs e)
        {
            TaskScheduler.ITask task = new TaskScheduler.RecurringTask(
                () => {
                    System.Threading.Thread.Sleep(300);
                },
                DateTime.Now.AddSeconds(30),
                TimeSpan.FromSeconds(30)
                );
            task.TaskId = taskNumber;
            schedular.AddTask(task);
            taskNumber = (int.Parse(taskNumber) + 1).ToString();
        }
    }
}
