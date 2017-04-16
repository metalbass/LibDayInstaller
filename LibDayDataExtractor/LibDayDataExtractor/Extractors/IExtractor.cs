using LibDayDataExtractor.Progress;

namespace LibDayDataExtractor.Extractors
{
    public interface IExtractor
    {
        void Extract(ExtractionPaths paths, ProgressReporter progress);
    }
}
