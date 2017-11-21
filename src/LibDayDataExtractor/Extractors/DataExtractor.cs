using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LibDayDataExtractor.Progress;
using LibDayDataExtractor.Utils;

namespace LibDayDataExtractor.Extractors
{
    /// <summary>
    /// Extracts data from the original game into the given installation path.
    /// </summary>
    public class DataExtractor
    {
        public DataExtractor(string originalFilesPath, string newFilesPath)
        {
            m_originalFilesPath = originalFilesPath;
            m_newFilesPath      = newFilesPath;
        }

        /// <summary>
        /// Starts the extraction process from the original game's files.
        /// </summary>
        /// <param name="progress">ProgressReporter to report the progress to.</param>
        public void Extract(ProgressReporter progress)
        {
            SafeNativeMethods.SetDllDirectory("libs/");

            progress.AddSubProgress(4, weight: 1);
            progress.AddSubProgress(1, weight: 100); // there are 5k SMK files in the MFF file.

            var dbiExtractor = new DbiExtractor();
            ExtractFiles(GetDbiFolders(), "MINES.dbi", dbiExtractor, progress[1]);

            return;

            var mdbExtractor     = new MdbExtractor();
            var zipMdbExtractor  = new ZippedMdbExtractor(mdbExtractor);
            var smkExtractor     = new SmackerVideoExtractor();
            var mffExtractor     = new MffExtractor(smkExtractor);
            var cdMusicExtractor = new CompactDiscMusicExtractor();

            cdMusicExtractor.Extract(new ExtractionPaths
            {
                OriginalFileName = $@"{m_originalFilesPath[0]}:\",
                OriginalFilePath = $@"{m_originalFilesPath[0]}:\",
                OutputDirectory  = Path.Combine(m_newFilesPath, "Music"),
                TempDirectory    = Path.Combine(m_newFilesPath, "Temp")
            }, progress[0]);

            ExtractFiles(GetSmkImageFolders(), "*.smk", smkExtractor   , progress[1]);
            ExtractFiles(GetMdbFolders()     , "*.mdb", mdbExtractor   , progress[2]);
            ExtractFiles(GetMdbFolders()     , "*.zip", zipMdbExtractor, progress[3]);
            ExtractFiles(GetMffFolders()     , "*.ff" , mffExtractor   , progress[4]);
        }

        public static string ReadString(byte[] asciiBytes)
        {
            return ReadString(asciiBytes, 0, asciiBytes.Length);
        }

        public static string ReadString(byte[] asciiBytes, int offset, int count)
        {
            int strLength = 0;
            while (strLength < count)
            {
                if (asciiBytes[offset + strLength] == 0)
                {
                    break;
                }

                ++strLength;
            }

            return Encoding.ASCII.GetString(asciiBytes, offset, strLength);
        }

        private static IEnumerable<string> GetMffFolders()
        {
            yield return "ANMSUNIT";
        }

        private IEnumerable<string> GetSmkImageFolders()
        {
            yield return "COLANMS";
            yield return "ENCYCL";
            yield return "INTERF";
        }

        private IEnumerable<string> GetMdbFolders()
        {
            yield return "COLONIES";
            yield return "GLOBALS";
            yield return "SCENS";
        }

        private IEnumerable<string> GetDbiFolders()
        {
            yield return "IMGS";
        }

        private void ExtractFiles(IEnumerable<string> folders, string searchPattern,
            IExtractor extractor, ProgressReporter progress)
        {
            List<ExtractionPaths> paths = EnumerateFiles(folders, searchPattern).ToList();

            progress.AddSubProgress(paths.Count);

            for (int i = 0; i < paths.Count; ++i)
            {
                extractor.Extract(paths[i], progress[i]);
            }
        }

        private IEnumerable<ExtractionPaths> EnumerateFiles(
            IEnumerable<string> folders, string searchPattern)
        {
            foreach (string folderName in folders)
            {
                var files = Directory.EnumerateFiles(
                    Path.Combine(m_originalFilesPath, folderName), searchPattern);

                foreach (string fullFilePath in files)
                {
                    yield return new ExtractionPaths
                    {
                        OriginalFilePath = fullFilePath,
                        OriginalFileName = Path.GetFileName(fullFilePath),
                        OutputDirectory  = GetOutputDirectory(fullFilePath),
                        TempDirectory    = Path.Combine(m_newFilesPath, "Temp"),
                    };
                }
            }
        }

        private string GetOutputDirectory(string filePath)
        {
            string originalDirectory = Path.GetDirectoryName(filePath);

            return Path.Combine(
                m_newFilesPath, originalDirectory.Substring(m_originalFilesPath.Length));
        }

        private string m_originalFilesPath;
        private string m_newFilesPath;
    }
}
