using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using LibDayDataExtractor.Extensions;
using LibDayDataExtractor.Progress;

namespace LibDayDataExtractor.Extractors
{
    /// <summary>
    /// Extracts MDB files out ZIP archives and process them normally.
    /// </summary>
    public class ZippedMdbExtractor : IExtractor
    {
        public ZippedMdbExtractor(MdbExtractor mdbExtractor)
        {
            m_mdbExtractor = mdbExtractor;
        }

        public void Extract(ExtractionPaths path, ProgressReporter progress = null)
        {
            var zippedFiles = ZippedFilesIn(path.OriginalFilePath).ToList();

            for (int i = 0; i < zippedFiles.Count; i++)
            {
                Directory.CreateDirectory(path.TempDirectory);

                string mdbTempFilePath = Path.Combine(path.TempDirectory, zippedFiles[i].Name);
                zippedFiles[i].ExtractToFile(mdbTempFilePath);

                m_mdbExtractor.Extract(new ExtractionPaths
                {
                    OriginalFilePath = mdbTempFilePath,
                    OriginalFileName = zippedFiles[i].Name,
                    OutputDirectory  = Path.Combine(path.OutputDirectory, path.OriginalFileName),
                    TempDirectory    = path.TempDirectory,
                });

                File.Delete(mdbTempFilePath);

                if (progress != null)
                {
                    progress.Report(100 * (i + 1) / zippedFiles.Count);
                }
            }
        }

        private static IEnumerable<ZipArchiveEntry> ZippedFilesIn(string filePath)
        {
            using (var stream = File.OpenRead(filePath))
            using (ZipArchive archive = new ZipArchive(stream))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (!entry.Name.Contains(".mdb", CompareOptions.IgnoreCase))
                    {
                        Console.WriteLine($"Ignoring {filePath}/{entry.Name}");
                        continue;
                    }

                    yield return entry;
                }
            }
        }

        private MdbExtractor m_mdbExtractor;
    }
}
