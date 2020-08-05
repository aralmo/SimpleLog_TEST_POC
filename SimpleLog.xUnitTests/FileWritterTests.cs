using SimpleLog.FileTarget;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SimpleLog.xUnitTests
{
    [Trait("","LogFilesRepository")]
    public class FileWritterTests
    {
        [Fact(DisplayName = "Multiple threads writting to file work")]
        public void MultipleThreadsWrittingToFile()
        {
            if (File.Exists(".\\test.log"))
                File.Delete(".\\test.log");

            var repo = new LogFilesRepository();
            Task[] tasks = new Task[10];
            for (int t = 0; t < 10; t++)
            {
                tasks[t] = Task.Run(() =>
                {
                    for (int n = 0; n < 1000; n++)
                    { 
                        repo.WriteLine(".\\test.log", $"{Thread.CurrentThread.ManagedThreadId} - {n}", Encoding.UTF8);
                    }
                });
            }
            
            Task.WaitAll(tasks);
            repo.Flush();
            Assert.Equal(10000, File.ReadAllLines(".\\test.log").Length);
        }

        [Fact(DisplayName = "All lines get logged")]
        public void WriteToFile()
        {
            if (File.Exists(".\\test2.log"))
                File.Delete(".\\test2.log");

            var repo = new LogFilesRepository();
            for (int n = 0; n < 10000; n++)
            {
                repo.WriteLine(".\\test2.log", $"Log entry number {n}", Encoding.UTF8);
            }

            repo.Flush();
            
            Assert.Equal(10000, File.ReadAllLines(".\\test2.log").Length);
        }
    }
}
