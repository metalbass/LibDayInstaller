using System;
using System.Diagnostics;

namespace LibDayDataExtractor.Utils
{
    public static class Debug
    {
        [DebuggerHidden]
        public static void Assert(bool condition, string message = "")
        {
            if (!condition)
            {
                throw new Exception(message);
            }
        }
    }
}
