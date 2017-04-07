using System;
using System.IO;

namespace LibDayDataExtractor
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

            new MdbExtractor().ExtractToTsv(
                Path.Combine(m_originalFilesPath, "SCENS", "SKIRMISH.MDB"),
                Path.Combine(m_newFilesPath, "SCENS"));
        }

        private string m_originalFilesPath;
        private string m_newFilesPath;
    }
}
