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

        public void Extract(ExtractionPaths path, ProgressReporter progress)
        {
            foreach (ZipArchiveEntry entry in ZippedFilesIn(path.OriginalFilePath))
            {
                Directory.CreateDirectory(path.TempDirectory);

                string mdbTempFilePath = Path.Combine(path.TempDirectory, entry.Name);
                entry.ExtractToFile(mdbTempFilePath);

                m_mdbExtractor.Extract(new ExtractionPaths
                {
                    OriginalFilePath = mdbTempFilePath,
                    OriginalFileName = entry.Name,
                    OutputDirectory  = Path.Combine(path.OutputDirectory, path.OriginalFileName),
                    TempDirectory    = path.TempDirectory,
                }, progress);

                File.Delete(mdbTempFilePath);
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
