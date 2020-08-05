using SimpleLog.FileTarget;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace SimpleLog.xUnitTests
{
    [Trait("","FileTarget")]
    public class FileTargetTests
    {
        [Fact(DisplayName = "File sequence increases when maxsize is reach")]
        void FilesSplitByMaxSize()
        {
            var repository = new FilesRepository();
            var fileTarget = new FileTarget.FileTarget(repository)
            {
                Formatter = (f) => $"{f.Message}",
                FileName = (f) => $"{f.Sequence}.log",
                FolderPath = ".\\log",
                MaxFileSize = 4,
                Encoding = Encoding.Default
            };

            var l = Encoding.Default.GetBytes("aaaa").Length;

            fileTarget.Log(new LogEntry()
            {
                Message = "AAAA"
            });

            fileTarget.Log(new LogEntry()
            {
                Message = "BB"
            });

            fileTarget.Log(new LogEntry()
            {
                Message = "BB"
            });

            Assert.Equal(2, repository.files.Count);
            Assert.Equal(".\\log\\0.log", repository.files.First().Key);
            Assert.Equal(".\\log\\1.log", repository.files.Last().Key);
            Assert.Equal("AAAA\n", repository.files.First().Value);
            Assert.Equal("BB\nBB\n", repository.files.Last().Value);
        }
        class FilesRepository : ILogFilesRepository
        {
            public Dictionary<string, string> files = new Dictionary<string, string>();
            public long GetFileSize(string filename)
            {
                var file = files.GetValueOrDefault(filename)??string.Empty;
                return Encoding.Default.GetBytes(file).Length;
            }

            public void WriteLine(string filename, string message,Encoding _)
            {
                if (files.ContainsKey(filename))
                {
                    files[filename] = $"{files[filename]}{message}\n";
                }
                else
                {
                    files.Add(filename, message + "\n");
                }
            }
        }
    }
}
