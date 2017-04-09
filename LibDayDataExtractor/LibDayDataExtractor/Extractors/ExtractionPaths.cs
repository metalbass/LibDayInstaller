namespace LibDayDataExtractor.Extractors
{
    /// <summary>
    /// Paths used during extraction, grouped together for convenience.
    /// </summary>
    public struct ExtractionPaths
    {
        public string OriginalFilePath { get; set; }
        public string OriginalFileName { get; set; }
        public string OutputDirectory  { get; set; }

        public string TempDirectory { get; set; }
    }
}
