using System;

namespace LibDayDataExtractor.Extractors.Dbi
{
    public struct DbiHeader
    {
        public byte   MagicHeaderD;
        public byte   MagicHeaderB;
        public byte   MagicHeaderI;
        public byte   MagicHeader0;
        public UInt32 VersionNumber;
        public UInt32 ElementCount;
        public UInt32 Width;
        public UInt32 Height;
        public UInt32 Compressed;
        public UInt64 Zero;
    }
}
