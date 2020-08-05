using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLog
{
    public static class LogScopesProvider
    {
        [ThreadStatic]
        static Stack<object> scopes;

        public static IDisposable CreateScope(object state)
        {
            if (scopes == null)
                scopes = new Stack<object>();

            scopes.Push(state);
            return new LogScope();
        }

        internal static void RemoveScope()
        {
            scopes.Pop();
        }

        public static object GetState()
        {
            return scopes?.Peek();
        }
    }

}
