using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using LibDayDataExtractor.Progress;

namespace LibDayDataExtractor.Extractors
{
    public class DbiExtractor : IExtractor
    {
        private struct DbiHeader
        {
            public uint elementCount;
            public uint uData1;
            public uint uData2;
            public uint uData3;
            public uint uData4;
            public uint uData5;
        }

        public void Extract(ExtractionPaths paths, ProgressReporter progress = null)
        {
            Debug.WriteLine(paths.OriginalFilePath);

            using (var file = File.OpenRead(paths.OriginalFilePath))
            using (BinaryReader reader = new BinaryReader(file, Encoding.ASCII))
            {
                byte[] unknownBytes = null;

                //  0x0000 (00.000) start of file

                Assert(ReadCstring(reader.ReadBytes(4)) == "dbi");
                Assert(reader.ReadUInt32() == 3);

                DbiHeader header;
                header.elementCount = reader.ReadUInt32();

                header.uData1 = reader.ReadUInt32();
                header.uData2 = reader.ReadUInt32();
                header.uData3 = reader.ReadUInt32();
                header.uData4 = reader.ReadUInt32();
                header.uData5 = reader.ReadUInt32();

                if (header.uData3 == 0) // LISTBOX.DBI
                {
                    Assert(header.uData1 == 38);
                    Assert(header.uData2 == 38);
                    Assert(header.uData3 == 0);
                    Assert(header.uData4 == 0);
                    Assert(header.uData5 == 0);
                }
                else
                {
                    Assert(header.uData1 == 54);
                    Assert(header.uData2 == 56);
                    Assert(header.uData3 == 1);
                    Assert(header.uData4 == 0);
                    Assert(header.uData5 == 0);
                }

                //  0x0020 (00.032) end of header

                unknownBytes = reader.ReadBytes(1024); // palette? I think I remember this

                //  0x0420 (01.056) first element. Size of 0x2000 (08.192)

                uint entrySize = 8 + 4;
                uint mapSize = entrySize * header.elementCount;

                file.Seek(-mapSize, SeekOrigin.End);

                //  0x8420 (33.824) <Name, ID> map

                string[] names = new string[header.elementCount];

                for (int i = 0; i < header.elementCount; ++i)
                {
                    string name = ReadCstring(reader.ReadBytes(8));
                    uint id = reader.ReadUInt32();

                    names[id] = name;
                }

                for (int i = 0; i < header.elementCount; ++i)
                {
                    file.Seek(0x420 + 0x2000 * i, SeekOrigin.Begin);
                    string elementName = ReadCstring(reader.ReadBytes(8));

                    Assert(elementName == names[i], "Elements should start at 0x420 and be 0x2000 long");
                }

                file.Seek(0x420, SeekOrigin.Begin);

                byte[] firstElement = reader.ReadBytes(0x2000);
                Directory.CreateDirectory(paths.OutputDirectory);
                using (var newFile = File.Create(Path.Combine(paths.OutputDirectory, names[0])))
                using (var writer = new BinaryWriter(newFile, Encoding.ASCII))
                {
                    writer.Write(firstElement);
                }

                ProcessElement(Path.Combine(paths.OutputDirectory, names[0]));
            }
        }

        private void ProcessElement(string elementPath)
        {
            using (var file = File.OpenRead(elementPath))
            using (BinaryReader reader = new BinaryReader(file, Encoding.ASCII))
            {
                // Using WALLS.DBI and WALLC000 as reference

                //  0x0000 (00) start of file
                string name = ReadCstring(reader.ReadBytes(8));
                //  0x0008 (08)
                UInt16 width = reader.ReadUInt16();
                //  0x000A (10)
                UInt16 height = reader.ReadUInt16();
                //  0x000C (12)
                UInt16 sizeImage = reader.ReadUInt16();
                //  0x000E (14)
                byte[] unknownData = reader.ReadBytes(6);
                //  0x0014 (20) end of header

                byte[] imageData = reader.ReadBytes(sizeImage);

                // 0x077B (1.915) end of image data

                using (var newFile = File.Create(elementPath + "_data"))
                using (var writer = new BinaryWriter(newFile, Encoding.ASCII))
                {
                    writer.Write(imageData);
                }
            }
        }

        // TODO: this is copy pasted from MffExtractor
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

        [DebuggerHidden]
        private static void Assert(bool condition, string message = "")
        {
            if (!condition)
            {
                throw new Exception(message);
            }
        }
    }
}
