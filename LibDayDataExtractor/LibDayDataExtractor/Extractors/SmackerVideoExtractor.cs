using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using FFmpeg.AutoGen;
using LibDayDataExtractor.Utils;

namespace LibDayDataExtractor.Extractors
{
    /// <summary>
    /// Extracts images out of smacker video files
    /// </summary>
    public class SmackerVideoExtractor
    {
        public SmackerVideoExtractor()
        {
            string ffmpegPath = "FFmpeg/bin/x86";
            SafeNativeMethods.SetDllDirectory(ffmpegPath);

            ffmpeg.av_register_all();
            ffmpeg.avcodec_register_all();
            ffmpeg.avformat_network_init();
        }

        public unsafe void Extract(ExtractionPaths path)
        {
            AVFormatContext* formatContext = CreateFormatContext(path.OriginalFilePath);
            AVStream* stream = GetStream(formatContext);

            var destinationPixelFormat = AVPixelFormat.AV_PIX_FMT_BGR24;

            fixed (byte* convertedFrameBuffer = new byte[GetFrameSize(stream, destinationPixelFormat)])
            {
                var dstData = new byte_ptrArray4();
                var dstLinesize = new int_array4();

                ffmpeg.av_image_fill_arrays(ref dstData, ref dstLinesize, convertedFrameBuffer,
                    destinationPixelFormat, stream->codec->width, stream->codec->height, 1);

                OpenCodec(stream->codec);

                var decodedFrame = ffmpeg.av_frame_alloc();

                var packet = new AVPacket();
                ffmpeg.av_init_packet(&packet);

                SwsContext* convertContext = CreateConversionContext(stream, destinationPixelFormat);

                using (var bitmap = CreateBitmap(stream, PixelFormat.Format24bppRgb))
                {
                    Directory.CreateDirectory(Path.Combine(path.OutputDirectory, path.OriginalFileName));

                    long frameCount = stream->nb_frames;
                    for (int frame = 0; frame < frameCount || frameCount == 0; ++frame)
                    {
                        try
                        {
                            if (ffmpeg.av_read_frame(formatContext, &packet) < 0)
                            {
                                Console.WriteLine(@"Could not read frame");
                                break;
                            }

                            if (!DecodeFrame(stream, &packet, decodedFrame, frame))
                            {
                                continue;
                            }

                            ffmpeg.sws_scale(convertContext, decodedFrame->data,
                                decodedFrame->linesize, 0, stream->codec->height, dstData, dstLinesize);

                            SaveFrame(bitmap, GenerateOutputPath(path, frame),
                                dstLinesize[0], (IntPtr)convertedFrameBuffer);
                        }
                        finally
                        {
                            ffmpeg.av_packet_unref(&packet);
                            ffmpeg.av_frame_unref(decodedFrame);
                        }
                    }
                }

                ffmpeg.sws_freeContext(convertContext);

                ffmpeg.av_free(decodedFrame);
                ffmpeg.avcodec_close(stream->codec);
                ffmpeg.avformat_close_input(&formatContext);
            }
        }

        private unsafe static Bitmap CreateBitmap(AVStream* stream, PixelFormat pixelFormat)
        {
            return new Bitmap(
                stream->codec->width, stream->codec->height, pixelFormat);
        }

        private static void SaveFrame(Bitmap bitmap, string path, int stride, IntPtr frameBuffer)
        {
            BitmapData bmpData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height), 
                ImageLockMode.WriteOnly, bitmap.PixelFormat);

            bmpData.Stride = stride;

            SafeNativeMethods.CopyMemory(bmpData.Scan0, frameBuffer, (uint)(bmpData.Stride * bitmap.Height));

            bitmap.UnlockBits(bmpData);

            bitmap.Save(path, ImageFormat.Png);
        }

        public uint ComputeSizeOfSmkFile(BinaryReader reader)
        {
            var header = Smk2Header.Read(reader);

            uint fileSize = (uint)Marshal.SizeOf<Smk2Header>() + header.TreesSize;

            for (int frame = 0; frame < header.Frames; ++frame)
            {
                uint frameSize = reader.ReadUInt32();
                frameSize &= 0xffffffcu;

                fileSize += frameSize;
                fileSize += 4; // frameSize we just read
                fileSize += 1; // FrameType. 1 byte/frame
            }

            return fileSize;
        }

        private unsafe static AVFormatContext* CreateFormatContext(string filePath)
        {
            AVFormatContext* pFormatContext = ffmpeg.avformat_alloc_context();

            if (ffmpeg.avformat_open_input(&pFormatContext, filePath, null, null) != 0)
                throw new ApplicationException(@"Could not open file");

            if (ffmpeg.avformat_find_stream_info(pFormatContext, null) != 0)
                throw new ApplicationException(@"Could not find stream info");

            return pFormatContext;
        }

        private unsafe static AVStream* GetStream(AVFormatContext* formatContext)
        {
            AVStream* stream = null;
            for (var i = 0; i < formatContext->nb_streams; i++)
            {
                if (formatContext->streams[i]->codec->codec_type == AVMediaType.AVMEDIA_TYPE_VIDEO)
                {
                    stream = formatContext->streams[i];
                    break;
                }
            }

            if (stream == null)
                throw new ApplicationException(@"Could not found video stream");

            return stream;
        }

        private unsafe static SwsContext* CreateConversionContext(
            AVStream* stream, AVPixelFormat destinationPixelFormat)
        {
            AVCodecContext* codecContext = stream->codec;

            SwsContext* convertContext = ffmpeg.sws_getContext(
                codecContext->width, codecContext->height, codecContext->pix_fmt,
                codecContext->width, codecContext->height, destinationPixelFormat,
                ffmpeg.SWS_FAST_BILINEAR, null, null, null);

            if (convertContext == null)
                throw new ApplicationException(@"Could not initialize the conversion context");

            return convertContext;
        }

        private unsafe static int GetFrameSize(AVStream* stream, AVPixelFormat destinationPixelFormat)
        {
            return ffmpeg.av_image_get_buffer_size(destinationPixelFormat,
                stream->codec->width, stream->codec->height, 1);
        }

        private unsafe static void OpenCodec(AVCodecContext* codec)
        {
            var pCodec = ffmpeg.avcodec_find_decoder(codec->codec_id);
            if (pCodec == null)
                throw new ApplicationException(@"Unsupported codec");

            if ((pCodec->capabilities & ffmpeg.AV_CODEC_CAP_TRUNCATED) == ffmpeg.AV_CODEC_CAP_TRUNCATED)
            {
                codec->flags |= ffmpeg.AV_CODEC_FLAG_TRUNCATED;
            }

            if (ffmpeg.avcodec_open2(codec, pCodec, null) < 0)
                throw new ApplicationException(@"Could not open codec");
        }

        private unsafe static bool DecodeFrame(
            AVStream* stream, AVPacket* packet, AVFrame* frame, int frameNumber)
        {
            if (packet->stream_index != stream->index)
                return false;

            if (ffmpeg.avcodec_send_packet(stream->codec, packet) < 0)
                throw new ApplicationException($"Error while sending packet {frameNumber}");

            if (ffmpeg.avcodec_receive_frame(stream->codec, frame) < 0)
                throw new ApplicationException($"Error while receiving frame {frameNumber}");

            return true;
        }

        private static string GenerateOutputPath(ExtractionPaths path, int frame)
        {
            return Path.Combine(path.OutputDirectory, path.OriginalFileName, $"{frame}.png");
        }
    }
}
