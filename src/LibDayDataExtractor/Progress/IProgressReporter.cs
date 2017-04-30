using System;

namespace LibDayDataExtractor.Progress
{
    public interface IProgressReporter : IProgress<int>
    {
        int Progress { get; }
    }
}
