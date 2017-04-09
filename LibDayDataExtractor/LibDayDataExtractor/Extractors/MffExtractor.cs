using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LibDayDataExtractor.Extensions;

namespace LibDayDataExtractor.Extractors
{
    /// <summary>
    /// Extracts SMK2 files out of MFF files.
    /// </summary>
    /// <remarks>There's only 1 MFF file in Liberation Day, and its extension is *.FF.</remarks>
    public class MffExtractor
    {
        public MffExtractor(SmackerVideoExtractor smkExtractor)
        {
            m_smkExtractor = smkExtractor;
        }

        public void Extract(ExtractionPaths path)
        {
            using (var file = File.OpenRead(path.OriginalFilePath))
            using (BinaryReader reader = new BinaryReader(file, Encoding.ASCII))
            {
                foreach (var fileInfo in SmkFilesIn(reader))
                {
                    string smkFileName = fileInfo.Item2;

                    string tempFilePath = Path.Combine(path.TempDirectory, smkFileName);

                    string newOutputDirectory = Path.Combine(
                        path.OutputDirectory, path.OriginalFileName, Path.GetDirectoryName(smkFileName));

                    Directory.CreateDirectory(path.TempDirectory);

                    uint smkOffset = fileInfo.Item1;

                    ExtractSmkFile(file, reader, smkOffset, tempFilePath);

                    m_smkExtractor.Extract(new ExtractionPaths
                    {
                        OriginalFilePath = tempFilePath,
                        OriginalFileName = Path.GetFileName(smkFileName),
                        OutputDirectory  = newOutputDirectory,
                        TempDirectory    = path.TempDirectory, 
                    });

                    File.Delete(tempFilePath);
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

        private static IEnumerable<Tuple<uint, string>> SmkFilesIn(BinaryReader reader)
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
