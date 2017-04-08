namespace LibDayDataExtractor.Extractors
{
    /// <summary>
    /// Groups the path of a file that needs to be extracted and the folder where it
    /// must be extracted to, so it can be used by multiple extractors.
    /// </summary>
    public struct ExtractionPath
    {
        public string FilePath        { get; set; }
        public string OutputDirectory { get; set; }
    }
}
