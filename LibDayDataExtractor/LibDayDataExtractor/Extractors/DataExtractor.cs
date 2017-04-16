using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibDayDataExtractor.Progress;

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
        /// <param name="worker">BackgroundWorker to report the progress to.</param>
        public void Extract(ProgressReporter progress)
        {
            progress.AddSubProgress(3, weight: 1);
            progress.AddSubProgress(1, weight: 100); // there are 5k SMK files in the MFF file.

            var mdbExtractor = new MdbExtractor();
            var zipMdbExtractor = new ZippedMdbExtractor(mdbExtractor);
            var smkExtractor = new SmackerVideoExtractor();
            var mffExtractor = new MffExtractor(smkExtractor);

            ExtractFiles(GetSmkImageFolders(), "*.smk", smkExtractor   , progress[0]);
            ExtractFiles(GetMdbFolders()     , "*.mdb", mdbExtractor   , progress[1]);
            ExtractFiles(GetMdbFolders()     , "*.zip", zipMdbExtractor, progress[2]);
            ExtractFiles(GetMffFolders()     , "*.ff" , mffExtractor   , progress[3]);
        }

        private static IEnumerable<string> GetMffFolders()
        {
            yield return "ANMSUNIT";
        }

        private IEnumerable<string> GetSmkImageFolders()
        {
            yield return "COLANMS";
            yield return "ENCYCL";
            yield return "INTERF";
        }

        private IEnumerable<string> GetMdbFolders()
        {
            yield return "COLONIES";
            yield return "GLOBALS";
            yield return "SCENS";
        }

        private void ExtractFiles(IEnumerable<string> folders, string searchPattern,
            IExtractor extractor, ProgressReporter progress)
        {
            List<ExtractionPaths> paths = EnumerateFiles(folders, searchPattern).ToList();

            for (int i = 0; i < paths.Count; ++i)
            {
                extractor.Extract(paths[i], progress);

                progress.Report((i + 1f) / paths.Count);
            }
        }

        private IEnumerable<ExtractionPaths> EnumerateFiles(
            IEnumerable<string> folders, string searchPattern)
        {
            foreach (string folderName in folders)
            {
                var files = Directory.EnumerateFiles(
                    Path.Combine(m_originalFilesPath, folderName), searchPattern);

                foreach (string fullFilePath in files)
                {
                    yield return new ExtractionPaths
                    {
                        OriginalFilePath = fullFilePath,
                        OriginalFileName = Path.GetFileName(fullFilePath),
                        OutputDirectory  = GetOutputDirectory(fullFilePath),
                        TempDirectory    = Path.Combine(m_newFilesPath, "Temp"),
                    };
                }
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
