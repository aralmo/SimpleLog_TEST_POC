using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLog.FileTarget
{
    /// <summary>
    /// Interface for taking care of file operations inside the FileTarget class
    /// </summary>
    public interface ILogFilesRepository
    {
        /// <summary>
        /// Return current file size for the indicated filename
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        long GetFileSize(string filename);

        /// <summary>
        /// Writes the message to the filename using the indicated encoding
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="message"></param>
        /// <param name="encoding"></param>
        void WriteLine(string filename, string message, Encoding encoding);

    }
}
