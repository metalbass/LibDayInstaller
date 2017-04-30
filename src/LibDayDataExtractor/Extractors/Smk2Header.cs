using System;
using System.IO;
using System.Runtime.InteropServices;

namespace LibDayDataExtractor.Extractors
{
    [StructLayout(LayoutKind.Explicit, Size = 104)]
    public struct Smk2Header
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        [FieldOffset(0)] public string Signature;

        [FieldOffset(4)]  [MarshalAs(UnmanagedType.U4)] public uint Width;
        [FieldOffset(8)]  [MarshalAs(UnmanagedType.U4)] public uint Height;
        [FieldOffset(12)] [MarshalAs(UnmanagedType.U4)] public uint Frames;
        [FieldOffset(16)] [MarshalAs(UnmanagedType.U4)] public uint FrameRate;
        [FieldOffset(20)] [MarshalAs(UnmanagedType.U4)] public uint Flags;
        [FieldOffset(24)] [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)] public uint[] AudioSize;
        [FieldOffset(52)] [MarshalAs(UnmanagedType.U4)] public uint TreesSize;
        [FieldOffset(56)] [MarshalAs(UnmanagedType.U4)] public uint MMap_Size;
        [FieldOffset(60)] [MarshalAs(UnmanagedType.U4)] public uint MClr_Size;
        [FieldOffset(64)] [MarshalAs(UnmanagedType.U4)] public uint Full_Size;
        [FieldOffset(68)] [MarshalAs(UnmanagedType.U4)] public uint Type_Size;
        [FieldOffset(72)] [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)] public uint[] AudioRate;
        [FieldOffset(100)] [MarshalAs(UnmanagedType.U4)] public uint Dummy;

        public static Smk2Header Read(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var size = Marshal.SizeOf<Smk2Header>();

            if (data.Length < size)
                throw new ArgumentOutOfRangeException(nameof(data));

            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                return (Smk2Header)Marshal.PtrToStructure(
                    handle.AddrOfPinnedObject(), typeof(Smk2Header));
            }
            finally
            {
                handle.Free();
            }
        }

        public static Smk2Header Read(BinaryReader reader)
        {
            var size = Marshal.SizeOf<Smk2Header>();

            return Read(reader.ReadBytes(size));
        }
    }
}
