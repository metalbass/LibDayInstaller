using System.Collections.Generic;
using System.IO;

namespace LibDayDataExtractor.Extractors
{
    /// <summary>
    /// Extracts data from the original game into the given installation path.
    /// </summary>
    public class DataExtractor
    {
        public DataExtractor(string originalFilesPath, string newFilesPath)
        {
            m_originalFilesPath = originalFilesPath;
            m_newFilesPath      = newFilesPath;
        }

        /// <summary>
        /// Starts the extraction process from the original game's files.
        /// </summary>
        public void Start()
        {
            var mdbExtractor = new MdbExtractor();
            var smackerVideoExtractor = new SmackerVideoExtractor();

            foreach (ExtractionPath path in EnumerateFiles("*.mdb"))
            {
                mdbExtractor.ExtractToTsv(path);
            }

            foreach (ExtractionPath path in EnumerateFiles("*.smk"))
            {
                smackerVideoExtractor.Extract(path);
            }
        }

        private IEnumerable<ExtractionPath> EnumerateFiles(string searchPattern)
        {
            var files = Directory.EnumerateFiles(
                m_originalFilesPath, searchPattern, SearchOption.AllDirectories);

            foreach (string fullFilePath in files)
            {
                string outputDirectory = GetOutputDirectory(fullFilePath);

                yield return new ExtractionPath
                {
                    FilePath = fullFilePath,
                    OutputDirectory = outputDirectory
                };
            }
        }

        private string GetOutputDirectory(string filePath)
        {
            string originalDirectory = Path.GetDirectoryName(filePath);

            return Path.Combine(
                m_newFilesPath, originalDirectory.Substring(m_originalFilesPath.Length));
        }

        private string m_originalFilesPath;
        private string m_newFilesPath;
    }
}
