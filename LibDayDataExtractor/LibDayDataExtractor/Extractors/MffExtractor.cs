using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

        public void Extract(ExtractionPaths path, ProgressReporter progress)
        {
            using (var file = File.OpenRead(path.OriginalFilePath))
            using (BinaryReader reader = new BinaryReader(file, Encoding.ASCII))
            {
                var smkFiles = SmkFilesIn(reader);

                for (int i = 0; i < smkFiles.Count; i++)
                {
                    string smkFileName = smkFiles[i].Item2;

                    string tempFilePath = Path.Combine(path.TempDirectory, smkFileName);

                    string newOutputDirectory = Path.Combine(
                        path.OutputDirectory, path.OriginalFileName, Path.GetDirectoryName(smkFileName));

                    Directory.CreateDirectory(Path.GetDirectoryName(tempFilePath));

                    uint smkOffset = smkFiles[i].Item1;
                    ExtractSmkFile(file, reader, smkOffset, tempFilePath);

                    m_smkExtractor.Extract(new ExtractionPaths
                    {
                        OriginalFilePath = tempFilePath,
                        OriginalFileName = Path.GetFileName(smkFileName),
                        OutputDirectory  = newOutputDirectory,
                        TempDirectory    = path.TempDirectory, 
                    }, progress);

                    File.Delete(tempFilePath);

                    progress.Report((i + 1f) / smkFiles.Count);
                }
            }
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

        private static List<Tuple<uint, string>> SmkFilesIn(BinaryReader reader)
        {
            reader.ReadBytes(4); // Magic word MFF
            uint headerCount = reader.ReadUInt32();

            var files = new List<Tuple<uint, string>>();

            for (int i = 0; i < headerCount; ++i)
            {
                string smkPath = ReadCstring(reader.ReadBytes(256));
                uint offset = reader.ReadUInt32();

                reader.ReadBytes(4); // unknown data

                files.Add(Tuple.Create(offset, smkPath));
            }

            return files;
        }

        private static string ReadCstring(byte[] asciiBytes)
        {
            int strLength = 0;
            while (strLength < asciiBytes.Length)
            {
                if (asciiBytes[strLength] == 0)
                {
                    break;
                }

                ++strLength;
            }

            return Encoding.ASCII.GetString(asciiBytes, 0, strLength);
        }

        private SmackerVideoExtractor m_smkExtractor;
    }
}
