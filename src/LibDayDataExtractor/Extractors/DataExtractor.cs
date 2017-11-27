using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LibDayDataExtractor.Extractors.Dbi;
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
            m_tempFilesPath     = Path.Combine(m_newFilesPath, "Temp");
        }

        /// <summary>
        /// Starts the extraction process from the original game's files.
        /// </summary>
        /// <param name="progress">ProgressReporter to report the progress to.</param>
        public void Extract(ProgressReporter progress)
        {
            SafeNativeMethods.SetDllDirectory("libs/");

            progress.AddSubProgress(5, weight: 1);
            progress.AddSubProgress(1, weight: 100); // there are 5k SMK files in the MFF file.

            var dbiExtractor = new DbiExtractor();

            var cdMusicExtractor = new CompactDiscMusicExtractor();
            var mdbExtractor     = new MdbExtractor();
            var zipMdbExtractor  = new ZippedMdbExtractor(mdbExtractor);
            var smkExtractor     = new SmackerVideoExtractor();
            var mffExtractor     = new MffExtractor(smkExtractor);

            cdMusicExtractor.Extract(new ExtractionPaths
            {
                OriginalFileName = $@"{m_originalFilesPath[0]}:\",
                OriginalFilePath = $@"{m_originalFilesPath[0]}:\",
                OutputDirectory  = Path.Combine(m_newFilesPath, "Music"),
                TempDirectory    = m_tempFilesPath
            }, progress[0]);

            ExtractFiles(GetMdbFolders()     , "*.mdb", mdbExtractor   , progress[1]);
            ExtractFiles(GetMdbFolders()     , "*.zip", zipMdbExtractor, progress[2]);
            ExtractFiles(GetDbiFolders()     , "*.dbi", dbiExtractor,    progress[3]);
            ExtractFiles(GetSmkImageFolders(), "*.smk", smkExtractor   , progress[4]);
            ExtractFiles(GetMffFolders()     , "*.ff" , mffExtractor   , progress[5]);

            Directory.Delete(m_tempFilesPath, recursive: true);
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
                        TempDirectory    = m_tempFilesPath,
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
        private string m_tempFilesPath;
    }
}
