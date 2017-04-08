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
            var files = Directory.EnumerateFiles(
                m_originalFilesPath, "*.mdb", SearchOption.AllDirectories);

            foreach (string fullFilePath in files)
            {
                string outputDirectory = GetOutputDirectory(
                    fullFilePath, m_originalFilesPath, m_newFilesPath);

                new MdbExtractor().ExtractToTsv(fullFilePath, outputDirectory);
            }
        }

        private static string GetOutputDirectory(
            string filePath, string originalFilesPath, string newFilesPath)
        {
            string originalDirectory = Path.GetDirectoryName(filePath);

            return Path.Combine(newFilesPath, originalDirectory.Substring(originalFilesPath.Length));
        }

        private string m_originalFilesPath;
        private string m_newFilesPath;
    }
}
