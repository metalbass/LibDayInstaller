using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

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
            var mdbExtractor = new MdbExtractor();
            var smackerVideoExtractor = new SmackerVideoExtractor();

            foreach (var path in EnumerateFiles(GetSmkImageFolders(), "*.smk", worker, 0, 50))
            {
                smackerVideoExtractor.Extract(path);
            }

            foreach (var path in EnumerateFiles(GetMdbFolders(), "*.mdb", worker, 50, 50))
            {
                mdbExtractor.ExtractToTsv(path);
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
        private IEnumerable<ExtractionPath> EnumerateFiles(
            IEnumerable<string> folders, string searchPattern, BackgroundWorker worker,
            int currentProgress, int weight)
        {
            List<ExtractionPath> paths = EnumerateFiles(folders, searchPattern).ToList();

            for (int i = 0; i < paths.Count; ++i)
            {
                yield return paths[i];

                float relativeProgress = i / (paths.Count - 1f);

                worker.ReportProgress((int)(currentProgress + relativeProgress * weight));
            }
        }

        private IEnumerable<ExtractionPath> EnumerateFiles(
            IEnumerable<string> folders, string searchPattern)
        {
            foreach (string folderName in folders)
            {
                var files = Directory.EnumerateFiles(
                    Path.Combine(m_originalFilesPath, folderName), searchPattern);

                foreach (string fullFilePath in files)
                {
                    yield return new ExtractionPath
                    {
                        FilePath = fullFilePath,
                        OutputDirectory = GetOutputDirectory(fullFilePath)
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
