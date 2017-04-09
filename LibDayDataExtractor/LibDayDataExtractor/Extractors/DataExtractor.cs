using System.Collections.Generic;
using System.ComponentModel;
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
        public void Start(BackgroundWorker worker)
        {
            var tasksProgress = new ProgressReporter(new BackgroundWorkerProgressReporter(worker));
            tasksProgress.AddSubProgress(3, weight: 1);
            tasksProgress.AddSubProgress(1, weight: 100); // there are 5k SMK files in the MFF file.

            var mdbExtractor    = new MdbExtractor();
            var zipMdbExtractor = new ZippedMdbExtractor(mdbExtractor);
            var smkExtractor    = new SmackerVideoExtractor();
            var mffExtractor    = new MffExtractor(smkExtractor);

            foreach (var path in EnumerateFiles(GetSmkImageFolders(), "*.smk", tasksProgress[0]))
            {
                smkExtractor.Extract(path);
            }

            foreach (var path in EnumerateFiles(GetMdbFolders(), "*.mdb", tasksProgress[1]))
            {
                mdbExtractor.ExtractToTsv(path);
            }

            foreach (var path in EnumerateFiles(GetMdbFolders(), "*.zip", tasksProgress[2]))
            {
                zipMdbExtractor.Extract(path);
            }

            foreach (var path in EnumerateFiles(new List<string> { "ANMSUNIT" }, "*.FF", tasksProgress[3]))
            {
                mffExtractor.Extract(path, tasksProgress[3]);
            }
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

        /// <summary>
        /// Enumerates files while reporting progress
        /// </summary>
        private IEnumerable<ExtractionPaths> EnumerateFiles(
            IEnumerable<string> folders, string searchPattern, ProgressReporter progress)
        {
            List<ExtractionPaths> paths = EnumerateFiles(folders, searchPattern).ToList();

            for (int i = 0; i < paths.Count; ++i)
            {
                yield return paths[i];

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
