using System;
using System.Runtime.CompilerServices;

using Godot;

using JetBrains.Annotations;

namespace GodotExt
{
    /// <summary>
    /// Contains extension methods for <see cref="Error"/>.
    /// </summary>
    [PublicAPI]
    public static class ErrorExt
    {
        /// <summary>
        /// Returns <see langword="true"/> if there is no error.
        /// </summary>
        /// <param name="error">The <see cref="Error"/> to check.</param>
        /// <returns><see langword="true"/> if there is no error, else <see langword="false"/>.</returns>
        [Pure]
        public static bool Success(this Error error)
        {
            return error is Error.Ok;
        }
        
        /// <summary>
        /// Throws an <see cref="Exception"/> if <paramref name="error"/> is not <see cref="Error.Ok"/>.
        /// </summary>
        /// <param name="error">The <see cref="Error"/> to check.</param>
        /// <param name="message">An optional message to include if an <see cref="Exception"/> is thrown.</param>
        /// <param name="filePath">The file path of the code that failed.</param>
        /// <param name="line">The line number of the code that failed.</param>
        public static void ThrowIfFailed(this Error error, [CanBeNull] string message = null, [CallerFilePath] string filePath = "<unknown>", [CallerLineNumber] int line = -1)
        {
            if (error is Error.Ok)
            {
                return;
            }
            string fullMessage = message is null ? $"Error ({error}) at {filePath}:{line}" : $"Error ({error}) at {filePath}:{line} - {message}";
            throw new Exception(fullMessage);
        }
    }
}