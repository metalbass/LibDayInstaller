using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using LibDayDataExtractor.Extensions;

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

            foreach (var path in EnumerateFiles(GetSmkImageFolders(), "*.smk", worker, 0, 35))
            {
                smackerVideoExtractor.Extract(path);
            }

            foreach (var path in EnumerateFiles(GetMdbFolders(), "*.mdb", worker, 35, 70))
            {
                mdbExtractor.ExtractToTsv(path);
            }

            foreach (var path in EnumerateFiles(GetMdbFolders(), "*.zip", worker, 70, 100))
            {
                using (var stream = File.OpenRead(path.OriginalFilePath))
                using (ZipArchive archive = new ZipArchive(stream))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (!entry.Name.Contains(".mdb", CompareOptions.IgnoreCase))
                        {
                            Console.WriteLine($"Ignoring {path.OriginalFilePath}/{entry.Name}");
                            continue;
                        }

                        string tempDirectory = Path.Combine(m_newFilesPath, "Temp");
                        string mdbTempFilePath = Path.Combine(tempDirectory, entry.Name);

                        string newOutputDirectory =
                            Path.Combine(path.OutputDirectory, path.OriginalFileName);

                        Directory.CreateDirectory(tempDirectory);

                        entry.ExtractToFile(mdbTempFilePath);

                        mdbExtractor.ExtractToTsv(new ExtractionPaths
                        {
                            OriginalFilePath = mdbTempFilePath,
                            OriginalFileName = entry.Name,
                            OutputDirectory  = newOutputDirectory
                        });

                        File.Delete(mdbTempFilePath);
                    }
                }
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
            IEnumerable<string> folders, string searchPattern, BackgroundWorker worker,
            int startingProgress, int endingProgress)
        {
            List<ExtractionPaths> paths = EnumerateFiles(folders, searchPattern).ToList();

            for (int i = 0; i < paths.Count; ++i)
            {
                yield return paths[i];

                float relativeProgress = i / (paths.Count - 1f);

                float difference = endingProgress - startingProgress;
                worker.ReportProgress((int)(startingProgress + relativeProgress * difference));
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
