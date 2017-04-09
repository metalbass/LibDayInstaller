using System;
using System.IO;

namespace LibDayDataExtractor.Extensions
{
    public static class StreamExtensions
    {
        /// <summary>
        /// Reads the specified amount of bytes from the current stream and
        /// writes them to another stream, using the specified buffer size.
        /// </summary>
        /// <param name="self">The current stream, to read from.</param>
        /// <param name="destination">The stream to which the contents of
        /// the current stream will be copied.</param>
        /// <param name="bufferSize">The size of the buffer. This value must be greater than zero.
        /// The default size is 81920.</param>
        /// <param name="count">The amount of data to copy.</param>
        public static void CopyTo(this Stream self, Stream destination, int bufferSize, int count)
        {
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            if (bufferSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSize));

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            byte[] buffer = new byte[bufferSize];

            int read;
            while ((read = self.Read(buffer, 0, Math.Min(buffer.Length, count))) > 0)
            {
                destination.Write(buffer, 0, read);
                count -= read;
            }
        }
    }
}
