using System;

namespace SimpleLog
{
    public class LogScope : IDisposable
    {
        public void Dispose()
        {
            //remove the last scope
            LogScopesProvider.RemoveScope();
        }
    }

}
