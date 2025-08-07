using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.MemoryMappedFiles;
using K5E_Memory_Map.UIModule;
using System.IO;


//////////////////////////////////////////////////////////////
using SFACore.Engine.Common;
using SFACore.Engine.Common.DataStructures;
using SFACore.Engine.Common.Logging;
using SFACore.Engine.Memory;
//using SFACore.Source.Docking;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using K5E_Memory_Map.HeapVisualizer;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Security.Policy;
//////////////////////////////////////////////////////////////


namespace K5E_Memory_Map
{
    public class InfoLoop
    {
        
        private CancellationTokenSource _cancellationTokenSource;



        static readonly UInt32 FoxPointerAddress = 0x803428F8;
        static readonly UInt32 FoxXOffset = 0xC;
        static readonly UInt32 FoxYOffset = 0x10;
        static readonly UInt32 FoxZOffset = 0x14;

        static readonly UInt32 MountPointerAddress = 0x803428F8;
        static readonly UInt32 MountOffset = 0x908;
        static readonly UInt32 BikeXOffset = 0x18;
        static readonly UInt32 BikeYOffset = 0x1C;
        static readonly UInt32 BikeZOffset = 0x20;
        static readonly Int32 BikeSize = 3104;

        static readonly UInt32 ESWPointerAddress = 0x803DD49C;
        static readonly UInt32 ESWXOffset = 0x694;
        static readonly UInt32 ESWYOffset = 0x698;
        static readonly UInt32 ESWZOffset = 0x69C;

        static bool success;

        public MainWindow _mainwindow;


        static readonly List<UInt32> HeapAddresses = new List<UInt32> { 0x80526020, 0x8112FF80, 0x812EFFA0, 0x8138E1E0, 0x81800000 /* End address */ };
        static readonly Int32 HeapCount = 4;
        static readonly UInt32 HeapTableAddress = 0x80340698;
        static readonly Int32 HeapImageWidth = 4096;
        static readonly Int32 HeapImageHeight = 1;
        static readonly Int32 DPI = 72;

        FullyObservableCollection<HeapVisualizer.HeapViewInfo> HeapViews;
        private MD5 md5 = MD5.Create();

        UInt32 Buffer;
        string? hash;

        string? CurrentMem = null;
        string? Inhash = null;
        bool FirstRun;


        public InfoLoop(MainWindow _MainWindow)
        {

            //_MemDisplay = memDisplay;
            _cancellationTokenSource = new CancellationTokenSource();

            _MainWindow.GetMemDisplay(this);
            _mainwindow = _MainWindow;


            HeapViews = new FullyObservableCollection<HeapVisualizer.HeapViewInfo>();
            try
            {
                for (Int32 index = 0; index < HeapCount; index++)
                {
                    HeapViews.Add(new HeapVisualizer.HeapViewInfo());
                    HeapViews[index].HeapBitmap = new WriteableBitmap(HeapImageWidth, HeapImageHeight, DPI, DPI, PixelFormats.Bgr24, null);
                    HeapViews[index].HeapBitmapBuffer = new Byte[HeapViews[index].HeapBitmap.BackBufferStride * HeapImageHeight];
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "Error initializing heap bitmap", ex);
                Debug.WriteLine("Error making heap bitmap");
            }
        }

        public MemDisplay _MemDisplay;

        private static string? GetFoxCoords()
        {
            


            UInt32 MainPointer = MemoryReader.Instance.Read<UInt32>(
                                SessionManager.Session.OpenedProcess,
                                MemoryQueryer.Instance.EmulatorAddressToRealAddress(SessionManager.Session.OpenedProcess, FoxPointerAddress, EmulatorType.Dolphin),
                                out success);

            // X
            var BufferPointer = BinaryPrimitives.ReverseEndianness(MainPointer) + FoxXOffset;
            string foxX = BinaryPrimitives.ReverseEndianness(MemoryReader.Instance.Read<Int32>(
                SessionManager.Session.OpenedProcess,
                MemoryQueryer.Instance.EmulatorAddressToRealAddress(SessionManager.Session.OpenedProcess, BufferPointer, EmulatorType.Dolphin),
                out success)).ToString().PadLeft(8, '0');
                                         //Debug.WriteLine(foxX);
                                         //Debug.WriteLine(BitConverter.ToSingle(BitConverter.GetBytes(Convert.ToInt32(foxX)), 0));

            // Y
            BufferPointer = BinaryPrimitives.ReverseEndianness(MainPointer) + FoxYOffset;
            string foxY = BinaryPrimitives.ReverseEndianness(MemoryReader.Instance.Read<Int32>(
                SessionManager.Session.OpenedProcess,
                MemoryQueryer.Instance.EmulatorAddressToRealAddress(SessionManager.Session.OpenedProcess, BufferPointer, EmulatorType.Dolphin),
                out success)).ToString().PadLeft(8, '0');
                                         //Debug.WriteLine(BitConverter.ToSingle(BitConverter.GetBytes(Convert.ToInt32(foxY)), 0));

            // Z
            BufferPointer = BinaryPrimitives.ReverseEndianness(MainPointer) + FoxZOffset;
            string foxZ = BinaryPrimitives.ReverseEndianness(MemoryReader.Instance.Read<Int32>(
                SessionManager.Session.OpenedProcess,
                MemoryQueryer.Instance.EmulatorAddressToRealAddress(SessionManager.Session.OpenedProcess, BufferPointer, EmulatorType.Dolphin),
                out success)).ToString().PadLeft(8, '0');
            //foxZ =(BitConverter.ToSingle(BitConverter.GetBytes(Convert.ToInt32(foxZ)), 0));

            string FoxCoords = foxX + " " + foxY + " " + foxZ;
            return (FoxCoords);

        }

        private static string? GetMountESWCoords()
        {
            UInt32 MainPointer = MemoryReader.Instance.Read<UInt32>(
                                SessionManager.Session.OpenedProcess,
                                MemoryQueryer.Instance.EmulatorAddressToRealAddress(SessionManager.Session.OpenedProcess, FoxPointerAddress, EmulatorType.Dolphin),
                                out success);

            MainPointer = BinaryPrimitives.ReverseEndianness(MainPointer) + MountOffset;
            MainPointer = MemoryReader.Instance.Read<UInt32>(
            SessionManager.Session.OpenedProcess,
            MemoryQueryer.Instance.EmulatorAddressToRealAddress(SessionManager.Session.OpenedProcess, MainPointer, EmulatorType.Dolphin),
            out success);


            // X
            UInt32 BufferPointer = BinaryPrimitives.ReverseEndianness(MainPointer) + BikeXOffset;
            string BikeX = BinaryPrimitives.ReverseEndianness(MemoryReader.Instance.Read<Int32>(
                SessionManager.Session.OpenedProcess,
                MemoryQueryer.Instance.EmulatorAddressToRealAddress(SessionManager.Session.OpenedProcess, BufferPointer, EmulatorType.Dolphin),
                out success)).ToString();
            //Debug.WriteLine(BitConverter.ToSingle(BitConverter.GetBytes(Convert.ToInt32(BikeX)), 0));

            // Y
            BufferPointer = BinaryPrimitives.ReverseEndianness(MainPointer) + BikeYOffset;
            string BikeY = BinaryPrimitives.ReverseEndianness(MemoryReader.Instance.Read<Int32>(
                SessionManager.Session.OpenedProcess,
                MemoryQueryer.Instance.EmulatorAddressToRealAddress(SessionManager.Session.OpenedProcess, BufferPointer, EmulatorType.Dolphin),
                out success)).ToString();
            //Debug.WriteLine(BitConverter.ToSingle(BitConverter.GetBytes(Convert.ToInt32(BikeY)), 0));

            // Z
            BufferPointer = BinaryPrimitives.ReverseEndianness(MainPointer) + BikeZOffset;
            string BikeZ = BinaryPrimitives.ReverseEndianness(MemoryReader.Instance.Read<Int32>(
                SessionManager.Session.OpenedProcess,
                MemoryQueryer.Instance.EmulatorAddressToRealAddress(SessionManager.Session.OpenedProcess, BufferPointer, EmulatorType.Dolphin),
                out success)).ToString();




            //ESW

            MainPointer = MemoryReader.Instance.Read<UInt32>(
            SessionManager.Session.OpenedProcess,
            MemoryQueryer.Instance.EmulatorAddressToRealAddress(SessionManager.Session.OpenedProcess, ESWPointerAddress, EmulatorType.Dolphin),
            out success);

            // X
            BufferPointer = BinaryPrimitives.ReverseEndianness(MainPointer) + ESWXOffset;
            string ESWX = BinaryPrimitives.ReverseEndianness(MemoryReader.Instance.Read<Int32>(
                SessionManager.Session.OpenedProcess,
                MemoryQueryer.Instance.EmulatorAddressToRealAddress(SessionManager.Session.OpenedProcess, BufferPointer, EmulatorType.Dolphin),
                out success)).ToString();
            //Debug.WriteLine(BitConverter.ToSingle(BitConverter.GetBytes(Convert.ToInt32(ESWX)), 0));

            BufferPointer = BinaryPrimitives.ReverseEndianness(MainPointer) + ESWYOffset;
            string ESWY = BinaryPrimitives.ReverseEndianness(MemoryReader.Instance.Read<Int32>(
                SessionManager.Session.OpenedProcess,
                MemoryQueryer.Instance.EmulatorAddressToRealAddress(SessionManager.Session.OpenedProcess, BufferPointer, EmulatorType.Dolphin),
                out success)).ToString();
            //Debug.WriteLine(BitConverter.ToSingle(BitConverter.GetBytes(Convert.ToInt32(ESWY)), 0));

            BufferPointer = BinaryPrimitives.ReverseEndianness(MainPointer) + ESWZOffset;
            string ESWZ = BinaryPrimitives.ReverseEndianness(MemoryReader.Instance.Read<Int32>(
                SessionManager.Session.OpenedProcess,
                MemoryQueryer.Instance.EmulatorAddressToRealAddress(SessionManager.Session.OpenedProcess, BufferPointer, EmulatorType.Dolphin),
                out success)).ToString();



            return BikeX + " " + BikeY + " " + BikeZ + " " + ESWX + " " + ESWY + " " + ESWZ;
        }

        private bool DolphinTest()
        {
            bool success;
            var Rdec = BinaryPrimitives.ReverseEndianness(MemoryReader.Instance.Read<UInt64>(
                                        SessionManager.Session.OpenedProcess,
                                        MemoryQueryer.Instance.EmulatorAddressToRealAddress(SessionManager.Session.OpenedProcess, 0x80000000, EmulatorType.Dolphin),
                                        out success));
            
            if (Rdec != 0)
            {
                return (true);
            }

            _MemDisplay.Dispatcher.Invoke(() =>
            {
                _MemDisplay.ForcePause();
            });

            return false;
        }

        private string ScanHash()
        {
            try
            {
                bool success = false;

                Int32 heapStructSize = typeof(SFAHeap).StructLayoutAttribute.Size;
                Byte[] heapArrayRaw = MemoryReader.Instance.ReadBytes(
                    SessionManager.Session.OpenedProcess,
                    MemoryQueryer.Instance.EmulatorAddressToRealAddress(SessionManager.Session.OpenedProcess, HeapTableAddress, EmulatorType.Dolphin),
                    HeapCount * typeof(SFAHeap).StructLayoutAttribute.Size, out success);



                SFAHeap[] heaps = new SFAHeap[4];


                for (Int32 heapIndex = 0; heapIndex < HeapCount; heapIndex++)
                {
                    Byte[] heapData = new Byte[heapStructSize];
                    Array.Copy(heapArrayRaw, heapIndex * heapStructSize, heapData, 0, heapStructSize);
                    heaps[heapIndex] = SFAHeap.FromByteArray(heapData);
                }

                for (Int32 heapIndex = 0; heapIndex < HeapCount; heapIndex++)
                {
                    Int32 bytesPerPixel = HeapViews[heapIndex].HeapBitmap.Format.BitsPerPixel / 8;
                    // UInt32 heapSize = heaps[heapIndex].totalSize == 0 ? (UInt32)(heaps[heapIndex + 1].heapPtr - heaps[heapIndex].heapPtr) : heaps[heapIndex].totalSize;
                    UInt32 heapSize = HeapAddresses[heapIndex + 1] - HeapAddresses[heapIndex];

                    HeapViews[heapIndex].HeapTotalSize = heaps[heapIndex].totalSize;
                    HeapViews[heapIndex].HeapUsedSize = heaps[heapIndex].usedSize;
                    HeapViews[heapIndex].HeapTotalBlocks = heaps[heapIndex].totalBlocks;
                    HeapViews[heapIndex].HeapUsedBlocks = heaps[heapIndex].usedBlocks;
                    HeapViews[heapIndex].HeapBaseAddress = heaps[heapIndex].heapPtr;
                    HeapViews[heapIndex].HeapHash = Convert.ToHexString(md5.ComputeHash(System.Text.Encoding.ASCII.GetBytes(HeapViews[heapIndex].HeapBaseAddress.ToString())));


                    Int32 heapEntrySize = typeof(SFAHeapEntry).StructLayoutAttribute.Size;

                    if (!success)
                    {
                        continue;
                    }



                    HeapViews[heapIndex].HeapMountBlockStart = 0;
                    HeapViews[heapIndex].HeapMountBlockEnd = 0;
                    HeapViews[heapIndex].HeapMountStatus = "";

                    for (int blockIndex = 0; blockIndex < heaps[heapIndex].totalBlocks; blockIndex++)
                    {
                        UInt32 nextBlockAddress = heaps[heapIndex].heapEntryPtr + (UInt32)(blockIndex * heapEntrySize);
                        Byte[] heapEntryDataRaw = MemoryReader.Instance.ReadBytes(
                            SessionManager.Session.OpenedProcess,
                            MemoryQueryer.Instance.EmulatorAddressToRealAddress(SessionManager.Session.OpenedProcess, nextBlockAddress, EmulatorType.Dolphin),
                            heapEntrySize,
                            out success);

                        if (!success)
                        {
                            continue;
                        }

                        SFAHeapEntry heapEntry = SFAHeapEntry.FromByteArray(heapEntryDataRaw);

                        Int32 offset = (Int32)(heapEntry.entryPtr - heaps[heapIndex].heapPtr);

                        HeapViews[heapIndex].HeapHash = Convert.ToHexString(md5.ComputeHash(System.Text.Encoding.ASCII.GetBytes(HeapViews[heapIndex].HeapHash + heapEntry.entryPtr.ToString())));
                        HeapViews[heapIndex].HeapHash = Convert.ToHexString(md5.ComputeHash(System.Text.Encoding.ASCII.GetBytes(HeapViews[heapIndex].HeapHash + heapEntry.size.ToString())));
                    }

                }



                if (this.HeapViews != null && this.HeapViews.Count > 1 && this.HeapViews[1] != null)
                {
                    if (this.HeapViews[1].HeapHash != null)
                    {
                        Inhash = HeapViews[1].HeapHash?.ToString();
                        if (true)
                        {
                            hash = Inhash;
                            CurrentMem = Inhash;
                        }
                        else
                        {
                            CurrentMem = Inhash;
                            return ("X");
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "Error updating the Heap Visualizer", ex);
            }


            return (hash);
        }




        private string Scan()
        {
                bool success;
            
                string frame = BinaryPrimitives.ReverseEndianness(MemoryReader.Instance.Read<UInt32>(
                                        SessionManager.Session.OpenedProcess,
                                        MemoryQueryer.Instance.EmulatorAddressToRealAddress(SessionManager.Session.OpenedProcess, 0x803DCB1C, EmulatorType.Dolphin),
                                        out success)).ToString().PadLeft(8, '0');

                return frame;
            
        }

        bool Attached = false;
        int Frame;

        public void Run()
        {
            int Cycle = 0;
            var token = _cancellationTokenSource.Token;

            while ((!_mainwindow.Attatched) && (!token.IsCancellationRequested))
            {
                Thread.Sleep(10);
            }

            while (!token.IsCancellationRequested)
            {
                if (Cycle % 10 == 0)
                {
                    Attached = DolphinTest();
                }

                if (Attached)
                {

                    string Fox = GetFoxCoords();
                    string MountESW = GetMountESWCoords();
                    
                    //string _hash = ScanHash();
                    //Debug.WriteLine(_hash);
                    //if (_hash != "X") {
                    //    _mainwindow.Hash = _hash;
                    //}
                    
                    Int32.TryParse(Scan(), out Frame);

                    _mainwindow.Frame = Frame;
                    
                    _MemDisplay.FoxCoords = Fox;
                    _MemDisplay.MountESWCoords = MountESW;


                    _MemDisplay.Dispatcher.Invoke(() =>
                    {
                        _MemDisplay.UpdateLoop();
                    });
                    Thread.Sleep(10);
                }
                else
                {
                    Thread.Sleep(5);
                }

                    
                Cycle++;
            }

        }

            public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
