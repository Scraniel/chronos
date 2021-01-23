using System;

namespace Chronos.Timer.Core
{
    internal static class Guards
    {
        /// <summary>
        /// Ensures that a value is not null.
        /// </summary>
        /// <exception cref="ArgumentNullException">Passed argument is null.</exception>
        /// <param name="argument">The </param>
        /// <param name="argumentName"></param>
        public static void ArgumentNotNull(object argument, string argumentName)
        {
            if (argument == null)
                throw new ArgumentNullException(argumentName);
        }
    }
}
