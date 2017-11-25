using System;
using System.Collections.Generic;
using System.IO;
using LibDayDataExtractor.Extractors;

namespace LibDayDataExtractor.Extractors.Dbi
{
    public class RleDecoder
    {
        public static byte[] Decode(byte[] input, int width, int height)
        {
            int stride = width + 8;

            int expectedOutputSize = stride * height;

            using (MemoryStream inputStream  = new MemoryStream(input))
            using (BinaryReader reader       = new BinaryReader(inputStream))
            using (MemoryStream outputStream = new MemoryStream(expectedOutputSize))
            using (BinaryWriter writer       = new BinaryWriter(outputStream))
            {
                bool finishedLine = false;

                while (!finishedLine && inputStream.Position < inputStream.Length)
                {
                    finishedLine = reader.ReadByte() == 0xCD;

                    if (finishedLine || inputStream.Position == inputStream.Length)
                    {
                        break;
                    }

                    inputStream.Seek(-1, SeekOrigin.Current);

                    writer.Write(stride);
                    writer.Write(width);

                    ReadLine(inputStream, reader, outputStream, writer);
                }

                return outputStream.ToArray();
            }
        }

        private enum ChunkBehavior : UInt16
        {
            Transparent = 0x0001,
            Color       = 0x0002,
        }

        private static void ReadLine(MemoryStream inputStream, BinaryReader reader, MemoryStream outputStream, BinaryWriter writer)
        {
            UInt32 lineLength = reader.ReadUInt32();

            long lineEnd = Math.Min(inputStream.Position + lineLength - 4, inputStream.Length);

            for (int chunkNumber = 0; inputStream.Position < lineEnd; ++chunkNumber)
            {
                if (lineEnd - inputStream.Position == 1)
                {
                    break;
                }

                ReadChunk(inputStream, reader, outputStream, writer);
            }
        }

        private static void ReadChunk(MemoryStream inputStream, BinaryReader reader, MemoryStream outputStream, BinaryWriter writer)
        {
            UInt16 chunkLength = reader.ReadUInt16();
            UInt16 behavior    = reader.ReadUInt16();

            if (behavior == (UInt16)ChunkBehavior.Transparent)
            {
                for (int i = 0; i < chunkLength; i++)
                {
                    writer.Write(TransparentColor);
                }
            }
            else if (behavior == (UInt16)ChunkBehavior.Color)
            {
                writer.Write(reader.ReadBytes(chunkLength));
            }
            else
            {
                throw new InvalidDataException("Incorrect behaviour");
            }
        }

        private const byte TransparentColor = 0;
    }
}
