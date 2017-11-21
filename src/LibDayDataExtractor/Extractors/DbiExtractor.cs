using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using LibDayDataExtractor.Progress;

namespace LibDayDataExtractor.Extractors
{
    public class DbiExtractor : IExtractor
    {
        private struct DbiHeader
        {
            public byte magicHeaderD;
            public byte magicHeaderB;
            public byte magicHeaderI;
            public byte magicHeader0;
            public uint versionNumber;
            public uint elementCount;
            public uint width;
            public uint height;
            public uint compressed;
            public UInt64 zero;
        }

        public void Extract(ExtractionPaths paths, ProgressReporter progress = null)
        {
            Debug.WriteLine(paths.OriginalFilePath);

            using (var file = File.OpenRead(paths.OriginalFilePath))
            using (BinaryReader reader = new BinaryReader(file, Encoding.ASCII))
            {
                DbiHeader header = ReadDbiHeader(reader);

                byte[] palette = reader.ReadBytes(1024);

                uint entrySize = 8 + 4;
                uint mapSize = entrySize * header.elementCount;

                file.Seek(-mapSize, SeekOrigin.End);

                string[] names = new string[header.elementCount];

                for (int i = 0; i < header.elementCount; ++i)
                {
                    string name = DataExtractor.ReadString(reader.ReadBytes(8));
                    uint id = reader.ReadUInt32();

                    names[id] = name;
                }

                for (int i = 0; i < header.elementCount; ++i)
                {
                    file.Seek(0x420 + 0x2000 * i, SeekOrigin.Begin);

                    byte[] content = reader.ReadBytes(0x2000);
                    string elementName = DataExtractor.ReadString(content, 0, 8);

                    Assert(elementName == names[i], "Elements should start at 0x420 and be 0x2000 long");
                    Directory.CreateDirectory(paths.OutputDirectory);

                    ProcessElement(Path.Combine(paths.OutputDirectory, names[i]), content, header, palette);
                }
            }
        }

        private static DbiHeader ReadDbiHeader(BinaryReader reader)
        {
            DbiHeader header;

            header.magicHeaderD = reader.ReadByte();
            header.magicHeaderB = reader.ReadByte();
            header.magicHeaderI = reader.ReadByte();
            header.magicHeader0 = reader.ReadByte();

            Assert(header.magicHeaderD == 'd');
            Assert(header.magicHeaderB == 'b');
            Assert(header.magicHeaderI == 'i');
            Assert(header.magicHeader0 == '\0');

            header.versionNumber = reader.ReadUInt32();
            Assert(header.versionNumber == 3);

            header.elementCount = reader.ReadUInt32();

            header.width = reader.ReadUInt32();
            header.height = reader.ReadUInt32();
            header.compressed = reader.ReadUInt32();
            header.zero = reader.ReadUInt64();

            if (header.compressed == 1)
            {
                Assert(header.width  == 54);
                Assert(header.height == 56);
            }
            else // LISTBOX.DBI
            {
                Assert(header.width  == 38);
                Assert(header.height == 38);
                Assert(header.compressed == 0);
            }

            Assert(header.zero == 0);

            return header;
        }

        private void ProcessElement(string path, byte[] content, DbiHeader header, byte[] paletteBytes)
        {
            using (var stream = new MemoryStream(content))
            using (BinaryReader reader = new BinaryReader(stream, Encoding.ASCII))
            {
                //  0x0000 (00) start of file
                string name = DataExtractor.ReadString(reader.ReadBytes(8));
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

                Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format8bppIndexed);

                SetPalette  (bitmap, paletteBytes);
                SetImageData(bitmap, imageData);

                bitmap.Save($"{path}.bmp");
            }
        }

        private static void SetPalette(Bitmap bitmap, byte[] paletteBytes)
        {
            ColorPalette palette = bitmap.Palette;

            for (int i = 0; i < palette.Entries.Length; i++)
            {
                palette.Entries[i] = Color.FromArgb(255,
                    paletteBytes[i * 4 + 0],
                    paletteBytes[i * 4 + 1],
                    paletteBytes[i * 4 + 2]);
            }

            bitmap.Palette = palette;
        }

        private static void SetImageData(Bitmap bitmap, byte[] imageData)
        {
            var bitmapData = bitmap.LockBits
            (
                new Rectangle(Point.Empty, bitmap.Size),
                ImageLockMode.WriteOnly,
                PixelFormat.Format8bppIndexed
            );

            IntPtr currentPosition = bitmapData.Scan0;
            for (int i = 0; i < bitmap.Height; ++i)
            {
                int fileStride = 8 + bitmap.Width;

                Marshal.Copy(imageData, i * fileStride + 8, currentPosition, bitmap.Width);
                currentPosition += bitmapData.Stride;
            }

            bitmap.UnlockBits(bitmapData);
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
