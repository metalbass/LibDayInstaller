using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using LibDayDataExtractor.Progress;
using static LibDayDataExtractor.Utils.Debug;

namespace LibDayDataExtractor.Extractors.Dbi
{
    public class DbiExtractor : IExtractor
    {
        public void Extract(ExtractionPaths paths, ProgressReporter progress = null)
        {
            string outputDirectory = Path.Combine(paths.OutputDirectory, paths.OriginalFileName);
            Directory.CreateDirectory(outputDirectory);

            using (var file = File.OpenRead(paths.OriginalFilePath))
            using (BinaryReader reader = new BinaryReader(file, Encoding.ASCII))
            {
                DbiHeader header = ReadDbiHeader(reader);

                byte[] palette = reader.ReadBytes(1024);

                string[] names = GetElementNames(file, reader, header);

                for (int i = 0; i < header.ElementCount; ++i)
                {
                    file.Seek(0x420 + 0x2000 * i, SeekOrigin.Begin);

                    byte[] content = reader.ReadBytes(0x2000);
                    string elementName = DataExtractor.ReadString(content, 0, 8);

                    Assert(elementName == names[i], "Elements should start at 0x420 and be 0x2000 long");

                    ProcessElement(outputDirectory, header, palette, content);

                    progress?.Report(100 * (i + 1) / (int)header.ElementCount);
                }
            }
        }

        private static string[] GetElementNames(FileStream file, BinaryReader reader, DbiHeader header)
        {
            uint entrySize = 8 + 4;
            uint mapSize = entrySize * header.ElementCount;

            file.Seek(-mapSize, SeekOrigin.End);

            string[] names = new string[header.ElementCount];

            for (int i = 0; i < header.ElementCount; ++i)
            {
                string name = DataExtractor.ReadString(reader.ReadBytes(8));
                uint id = reader.ReadUInt32();

                names[id] = name;
            }

            return names;
        }

        private static DbiHeader ReadDbiHeader(BinaryReader reader)
        {
            DbiHeader header;

            header.MagicHeaderD = reader.ReadByte();
            header.MagicHeaderB = reader.ReadByte();
            header.MagicHeaderI = reader.ReadByte();
            header.MagicHeader0 = reader.ReadByte();

            Assert(header.MagicHeaderD == 'd');
            Assert(header.MagicHeaderB == 'b');
            Assert(header.MagicHeaderI == 'i');
            Assert(header.MagicHeader0 == '\0');

            header.VersionNumber = reader.ReadUInt32();
            Assert(header.VersionNumber == 3);

            header.ElementCount = reader.ReadUInt32();

            header.Width = reader.ReadUInt32();
            header.Height = reader.ReadUInt32();
            header.Compressed = reader.ReadUInt32();
            header.Zero = reader.ReadUInt64();

            if (header.Compressed == 1)
            {
                Assert(header.Width  == 54);
                Assert(header.Height == 56);
            }
            else // LISTBOX.DBI
            {
                Assert(header.Width  == 38);
                Assert(header.Height == 38);
                Assert(header.Compressed == 0);
            }

            Assert(header.Zero == 0);

            return header;
        }

        private void ProcessElement(string outputDirectory,
            DbiHeader header, byte[] palette, byte[] content)
        {
            using (var stream = new MemoryStream(content))
            using (BinaryReader reader = new BinaryReader(stream, Encoding.ASCII))
            {
                ReadImage(outputDirectory, header, palette, reader);

                // TODO: there's actually some elements that contain more than 1 image

                //if (header.compressed == 1)
                //{
                //    stream.Seek(0x1000, SeekOrigin.Begin);
                //    ReadImage(outputDirectory, fileName, header, paletteBytes, reader);
                //}
            }
        }

        private static void ReadImage(string outputDirectory,
            DbiHeader header, byte[] palette, BinaryReader reader)
        {
            string name = DataExtractor.ReadString(reader.ReadBytes(8));

            UInt16 width       = reader.ReadUInt16();
            UInt16 height      = reader.ReadUInt16();
            UInt16 sizeImage   = reader.ReadUInt16();
            byte[] unknownData = reader.ReadBytes(6);

            byte[] imageData = reader.ReadBytes(sizeImage);

            if (header.Compressed == 1)
            {
                imageData = RleDecoder.Decode(imageData, width, height);
            }

            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format8bppIndexed);

            SetPalette  (bitmap, palette);
            SetImageData(bitmap, imageData);

            bitmap.Save($"{Path.Combine(outputDirectory, name)}.bmp");
        }

        private static void SetPalette(Bitmap bitmap, byte[] paletteBytes)
        {
            ColorPalette palette = bitmap.Palette;

            for (int i = 0; i < palette.Entries.Length; i++)
            {
                palette.Entries[i] = Color.FromArgb(255,
                    paletteBytes[i * 4 + 2],
                    paletteBytes[i * 4 + 1],
                    paletteBytes[i * 4 + 0]);
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

                int startIndex = i * fileStride + 8;
                if (startIndex + bitmap.Width > imageData.Length)
                {
                    break;
                }

                Marshal.Copy(imageData, startIndex, currentPosition, bitmap.Width);
                currentPosition += bitmapData.Stride;
            }

            bitmap.UnlockBits(bitmapData);
        }
    }
}
