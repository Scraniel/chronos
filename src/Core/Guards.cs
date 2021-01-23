using System;

namespace Chronos.Timer.Core
{
    /// <summary>
    /// Helper class which ensures pre-determined contraints are met.
    /// </summary>
    internal static class Guards
    {
        /// <summary>
        /// Ensures that a value is not null.
        /// </summary>
        /// <param name="argument">The object which will be validated.</param>
        /// <param name="argumentName">The name of the object.</param>
        /// <exception cref="ArgumentNullException">Passed argument is null.</exception>
        public static void ArgumentNotNull(object argument, string argumentName)
        {
            if (argument == null)
                throw new ArgumentNullException(argumentName);
        }
    }
}
