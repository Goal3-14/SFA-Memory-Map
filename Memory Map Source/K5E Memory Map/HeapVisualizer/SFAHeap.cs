namespace K5E_Memory_Map.HeapVisualizer
{
    using SFACore.Engine.Common;
    using SFACore.Engine.Memory;
    using System;
    using System.Buffers.Binary;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 20)]
    public class SFAHeap
    {
        [MarshalAs(UnmanagedType.I4)]
        public UInt32 totalSize;

        [MarshalAs(UnmanagedType.I4)]
        public UInt32 usedSize;

        [MarshalAs(UnmanagedType.I4)]
        public UInt32 totalBlocks;

        [MarshalAs(UnmanagedType.I4)]
        public UInt32 usedBlocks;

        [MarshalAs(UnmanagedType.I4)]
        public UInt32 heapEntryPtr;

        [MarshalAs(UnmanagedType.I4)]
        public UInt32 heapPtr;

        public static SFAHeap FromByteArray(byte[] bytes)
        {
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                SFAHeap result = Marshal.PtrToStructure<SFAHeap>(handle.AddrOfPinnedObject());

                // Fix GC endianness
                result.totalSize = BinaryPrimitives.ReverseEndianness(result.totalSize);
                result.usedSize = BinaryPrimitives.ReverseEndianness(result.usedSize);
                result.totalBlocks = BinaryPrimitives.ReverseEndianness(result.totalBlocks);
                result.usedBlocks = BinaryPrimitives.ReverseEndianness(result.usedBlocks);
                result.heapEntryPtr = BinaryPrimitives.ReverseEndianness(result.heapEntryPtr);

                result.heapPtr = BinaryPrimitives.ReverseEndianness(MemoryReader.Instance.Read<UInt32>(
                    SessionManager.Session.OpenedProcess,
                    MemoryQueryer.Instance.EmulatorAddressToRealAddress(SessionManager.Session.OpenedProcess, result.heapEntryPtr, EmulatorType.Dolphin),
                    out _));

                return result;
            }
            finally
            {
                handle.Free();
            }
        }
    }
    //// End class
}
//// End namespace
