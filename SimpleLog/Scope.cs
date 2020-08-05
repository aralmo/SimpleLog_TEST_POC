using System;

namespace SimpleLog
{
        public class LogScope : IDisposable
        {
            public LogScope()
            {
            }
            public void Dispose()
            {
                LogScopesProvider.RemoveScope(this);
            }
        }

}
