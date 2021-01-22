using System;

namespace Chronos.Timer.Core
{
    internal static class Guards
    {
        public static void ArgumentNotNull(object argument, string argumentName)
        {
            if (argument == null)
                throw new ArgumentNullException(argumentName);
        }
    }
}
