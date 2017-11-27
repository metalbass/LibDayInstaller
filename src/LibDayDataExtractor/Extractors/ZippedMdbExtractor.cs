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
            using (var stream = File.OpenRead(path.OriginalFilePath))
            using (ZipArchive archive = new ZipArchive(stream))
            {
                Directory.CreateDirectory(path.TempDirectory);

                for (int i = 0; i < archive.Entries.Count; i++)
                {
                    Extract(path, archive.Entries[i]);

                    progress?.Report(100 * (i + 1) / archive.Entries.Count);
                }
            }
        }

        private void Extract(ExtractionPaths path, ZipArchiveEntry entry)
        {
            if (!entry.Name.Contains(".mdb", CompareOptions.IgnoreCase))
            {
                Console.WriteLine($"Ignoring {path.OriginalFileName}/{entry.Name}");
                return;
            }

            string mdbTempFilePath = Path.Combine(path.TempDirectory, entry.Name);
            entry.ExtractToFile(mdbTempFilePath);

            m_mdbExtractor.Extract(new ExtractionPaths
            {
                OriginalFilePath = mdbTempFilePath,
                OriginalFileName = entry.Name,
                OutputDirectory  = Path.Combine(path.OutputDirectory, path.OriginalFileName),
                TempDirectory    = path.TempDirectory,
            });
        }

        private MdbExtractor m_mdbExtractor;
    }
}
