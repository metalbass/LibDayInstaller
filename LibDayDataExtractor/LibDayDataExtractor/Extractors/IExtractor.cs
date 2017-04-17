using LibDayDataExtractor.Progress;

namespace LibDayDataExtractor.Extractors
{
    public interface IExtractor
    {
        /// <summary>
        /// Extracts files in the given path and reports progress.
        /// </summary>
        /// <param name="paths">
        /// Input and output paths to be used during extraction.
        /// </param>
        /// <param name="progress">
        /// ProgressReporter used to report progress.
        /// Each extractor must either add subprogress or call Report on its completion.
        /// </param>
        void Extract(ExtractionPaths paths, ProgressReporter progress = null);
    }
}
