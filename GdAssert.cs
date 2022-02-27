using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Godot;
using JetBrains.Annotations;

namespace GodotExt
{
    [PublicAPI]
    public static class GdAssert
    {
        [Conditional("DEBUG")]
        [ContractAnnotation("assertion:false => stop")]
        public static void That(bool assertion, string message, [CallerFilePath] string file = "<unknown>", [CallerLineNumber] int line = -1)
        {
            if (assertion)
            {
                return;
            }

            GD.PrintErr($"Assertion failed: {message} at {file}:{line}");
            var stackTrace = new StackTrace();
            GD.PrintErr(stackTrace.ToString());
            throw new ApplicationException($"Assertion failed: {message}");
        }
   
    }
}