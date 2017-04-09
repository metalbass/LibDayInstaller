using System;
using System.IO;
using System.Runtime.InteropServices;

namespace LibDayDataExtractor.Utils
{
    /// <summary>
    /// This class is original from the FFmpeg.AutoGen.Example project.
    /// </summary>
    public static class InteropHelper
    {
        public const string LD_LIBRARY_PATH = "LD_LIBRARY_PATH";

        public static void RegisterLibrariesSearchPath(string path)
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                    SafeNativeMethods.SetDllDirectory(path);
                    break;
                case PlatformID.Unix:
                case PlatformID.MacOSX:
                    string currentValue = Environment.GetEnvironmentVariable(LD_LIBRARY_PATH);
                    if (string.IsNullOrWhiteSpace(currentValue) == false && currentValue.Contains(path) == false)
                    {
                        string newValue = currentValue + Path.PathSeparator + path;
                        Environment.SetEnvironmentVariable(LD_LIBRARY_PATH, newValue);
                    }
                    break;
            }
        }


    }
}
