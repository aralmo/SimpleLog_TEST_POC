using System;

namespace SimpleLog
{
    public class LogScope : IDisposable
    {
#warning hold scope state and check while removing the scope.

        public LogScope()
        {
        }
        public void Dispose()
        {
            //remove the last scope
            LogScopesProvider.RemoveScope();
        }
    }

}
