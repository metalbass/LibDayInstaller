using System;
using LibDayDataExtractor.Progress;
using Ripper;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using NAudio.Lame;
using NAudio.Wave;
using LibDayDataExtractor.Utils;

namespace LibDayDataExtractor.Extractors
{
    public class CompactDiscMusicExtractor : IExtractor
    {
        public CompactDiscMusicExtractor()
        {
            SafeNativeMethods.SetDllDirectory("AudioRipper/bin");

            m_drive = new CDDrive();

            m_drive.CDRemoved += OnCompactDiscRemoved;
        }

        private void OnCompactDiscRemoved(object sender, EventArgs e)
        {
            throw new InvalidOperationException("Disc removed while reading");
        }

        public void Extract(ExtractionPaths paths, ProgressReporter progress = null)
        {
            char driveLetter = Path.GetFullPath(paths.OriginalFilePath)[0];

            if (!m_drive.Open(driveLetter))
            {
                throw new InvalidOperationException($"Cannot open drive {driveLetter}");
            }

            if (!m_drive.Refresh())
            {
                throw new InvalidOperationException($"Cannot refresh drive {driveLetter}");
            }

            var audioTracks = GetAudioTracks().ToList();

            if (progress != null)
            {
                progress.AddSubProgress(audioTracks.Count);
            }

            for (int i = 0; i < audioTracks.Count; i++)
            {
                ProgressReporter subprogress = null;
                if (progress != null)
                {
                    subprogress = progress[i];
                }

                ReadAudioTrack(audioTracks[i], paths, subprogress);
            }
        }

        private IEnumerable<int> GetAudioTracks()
        {
            return Enumerable.Range(0, m_drive.GetNumTracks())
                .Where(track => m_drive.IsAudioTrack(track));
        }

        private void ReadAudioTrack(
            int track, ExtractionPaths paths, ProgressReporter progress)
        {
            Directory.CreateDirectory(paths.OutputDirectory);

            string outputPath = GenerateOutputPath(paths, track - 1);
            ID3TagData metadata = GenerateTrackMetadata(track - 1);

            using (var mp3Writer = new LameMP3FileWriter(
                outputPath, new WaveFormat(), LAMEPreset.STANDARD, metadata))
            {
                CdDataReadEventHandler     onDataRead = (x, y) => OnDataRead(x, y, mp3Writer);
                CdReadProgressEventHandler onProgress = (x, y) => OnTrackProgress(x, y, progress);

                if (m_drive.ReadTrack(track, onDataRead, onProgress) == 0)
                {
                    throw new InvalidOperationException($"Cannot read track {track}");
                }
            }
        }

        private string GenerateOutputPath(ExtractionPaths paths, int track)
        {
            return Path.Combine(paths.OutputDirectory, GetTrackName(track) + ".mp3");
        }

        private ID3TagData GenerateTrackMetadata(int track)
        {
            // TODO: we're missing the OST artist.

            return new ID3TagData()
            {
                Album    = "Liberation Day OST",
                Genre    = "OST",
                Title    = GetTrackName(track),
                Track    = track.ToString(),
                Year     = "1998",
            };
        }

        private string GetTrackName(int track)
        {
            var trackNames = new Dictionary<int, string>()
            {
                { 1, "Mistic Behaviour" },
                { 2, "Fearless" },
                { 3, "Explorer" },
                { 4, "Remote World" },
                { 5, "Double Star" },
                { 6, "Intor" },
                { 7, "Bravour behind the Glory" },
                { 8, "Fractum" },
                { 9, "Def Con 56" },
            };

            return trackNames[track];
        }

        private void OnDataRead(object sender, DataReadEventArgs args, LameMP3FileWriter writer)
        {
            writer.Write(args.Data, 0, (int)args.DataSize);
        }

        private void OnTrackProgress(
            object sender, ReadProgressEventArgs args, ProgressReporter progress)
        {
            if (progress != null)
            {
                ulong percent = ((ulong)100 * args.BytesRead) / args.Bytes2Read;

                progress.Report((int)percent);
            }
        }

        private CDDrive m_drive;
    }
}
