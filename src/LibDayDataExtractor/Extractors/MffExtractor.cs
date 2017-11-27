using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LibDayDataExtractor.Extensions;
using LibDayDataExtractor.Progress;

namespace LibDayDataExtractor.Extractors
{
    /// <summary>
    /// Extracts SMK2 files out of MFF files.
    /// </summary>
    /// <remarks>There's only 1 MFF file in Liberation Day, and its extension is *.FF.</remarks>
    public class MffExtractor : IExtractor
    {
        public MffExtractor(SmackerVideoExtractor smkExtractor)
        {
            m_smkExtractor = smkExtractor;
        }

        public void Extract(ExtractionPaths paths, ProgressReporter progress = null)
        {
            using (var file = File.OpenRead(paths.OriginalFilePath))
            using (BinaryReader reader = new BinaryReader(file, Encoding.ASCII))
            {
                var smkFiles = SmkFilesIn(paths, reader);

                int extractionProgress = 0;

                foreach (var smkFile in smkFiles)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(smkFile.TempSmkPath));

                    ExtractSmkFile(file, reader, smkFile.Offset, smkFile.TempSmkPath);

                    progress?.Report(50 * (++extractionProgress + 1) / smkFiles.Count);
                }

                Parallel.ForEach(smkFiles, smkFile =>
                {
                    m_smkExtractor.Extract(new ExtractionPaths
                    {
                        OriginalFilePath = smkFile.TempSmkPath,
                        OriginalFileName = Path.GetFileName(smkFile.OriginalPath),
                        OutputDirectory  = smkFile.OutputPath,
                        TempDirectory    = paths.TempDirectory,
                    });

                    progress?.Report(50 * (Interlocked.Increment(ref extractionProgress) + 1) / smkFiles.Count);
                });
            }
        }
        
        private class SmkFileInfo
        {
            public uint   Offset;
            public string OriginalPath;
            public string TempSmkPath;
            public string OutputPath;
        }

        private void ExtractSmkFile(
            FileStream file, BinaryReader reader, uint smkOffset, string outputPath)
        {
            using (var tempFileStream = File.OpenWrite(outputPath))
            {
                file.Seek(smkOffset, SeekOrigin.Begin);

                uint size = m_smkExtractor.ComputeSizeOfSmkFile(reader);

                file.Seek(smkOffset, SeekOrigin.Begin);

                file.CopyTo(tempFileStream, bufferSize: 81920, count: (int)size);
            }
        }

        private static List<SmkFileInfo> SmkFilesIn(ExtractionPaths paths, BinaryReader reader)
        {
            reader.ReadBytes(4); // Magic word MFF
            uint headerCount = reader.ReadUInt32();

            var files = new List<SmkFileInfo>();

            for (int i = 0; i < headerCount; ++i)
            {
                string originalPath = DataExtractor.ReadString(reader.ReadBytes(256));
                uint offset = reader.ReadUInt32();

                reader.ReadBytes(4); // unknown data

                string outputPath = Path.Combine(
                    paths.OutputDirectory,
                    paths.OriginalFileName,
                    Path.GetDirectoryName(originalPath)
                );

                files.Add(new SmkFileInfo()
                {
                    Offset       = offset,
                    OriginalPath = originalPath,
                    TempSmkPath  = Path.Combine(paths.TempDirectory, originalPath),
                    OutputPath   = outputPath
                });
            }

            return files;
        }

        private SmackerVideoExtractor m_smkExtractor;
    }
}
