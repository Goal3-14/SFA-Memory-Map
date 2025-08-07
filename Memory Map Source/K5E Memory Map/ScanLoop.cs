using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SFACore.Engine.Common;
using SFACore.Engine.Memory;
using System.Buffers.Binary;
using System.Windows;




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
using System.Collections.Concurrent;
using System.Windows.Media.Animation;
using System.Security.Policy;
//////////////////////////////////////////////////////////////













namespace K5E_Memory_Map
{
    public class ScanLoop()
    {
        static readonly uint FoxPointerAddress = 0x803428F8;
        static readonly uint FoxXOffset = 0xC;
        static readonly uint FoxYOffset = 0x10;
        static readonly uint FoxZOffset = 0x14;


        private Thread _scanThread;
        private bool _isRunning = false;


        /// <summary>
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

        /// </summary>

        // Event to send scanned data to the rest of the program
        public event Action<string[]> OnMemoryRead;


        public bool Paused = false;

        public ConcurrentQueue<(int,string)> Queue { get; } = new ConcurrentQueue<(int,string)>();

        MainWindow _mainwindow;

        public void Start(MainWindow Main)
        {
            if (_isRunning) return; // Prevent multiple starts

            _isRunning = true;
            _mainwindow = Main;
            _scanThread = new Thread(ReadLoop)
            {
                IsBackground = true, // Stops when the app closes
                Priority = ThreadPriority.Highest // Ensures smooth execution
            };
            _scanThread.Start();
        }


        public void Stop()
        {
            _isRunning = false;
            _scanThread?.Join(); // Gracefully stop the thread
        }


        private void ReadLoop() 
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            long targetTime = TimeSpan.TicksPerSecond / 120; // 60Hz frame target
            Debug.WriteLine($"ReadLoop started on thread: {Thread.CurrentThread.ManagedThreadId}");

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



            string CurrentF="";

            while (_isRunning)
            {

                if (_mainwindow.Attatched)
                {
                    var data = ReadMemory(); // Call your memory reading function

                    if (!_mainwindow.Paused)
                    {
                        if (data[0] != CurrentF) ;
                        {
                            if (data[1] != "X")
                            {
                                Queue.Enqueue((Int32.Parse(data[0]), data[1]));
                            }
                            // Maintain a stable 60Hz scan rate
                            long nextTick = stopwatch.ElapsedTicks + targetTime;
                            while (stopwatch.ElapsedTicks < nextTick) { Thread.SpinWait(1); }
                        }
                        CurrentF = data[0];
                    }
                    if (data[1] != "X")
                    {
                        _mainwindow.Hash = data[1];
                    }

                    //Debug.WriteLine(Queue.Count);
                }
            }
        }



        string[] Set;
        string? Hash;

        private string[] ReadMemory()
        {
            Set = Scan();
            return (Set);
        }




        string? CurrentMem = null;
        string? Inhash = null;
        bool FirstRun;

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
                        if (CurrentMem == Inhash)
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




        private string[] Scan()
        {
            bool success;
            while (true)
            {
                string frame = BinaryPrimitives.ReverseEndianness(MemoryReader.Instance.Read<UInt32>(
                                        SessionManager.Session.OpenedProcess,
                                        MemoryQueryer.Instance.EmulatorAddressToRealAddress(SessionManager.Session.OpenedProcess, 0x803DCB1C, EmulatorType.Dolphin),
                                        out success)).ToString().PadLeft(8, '0');

                string? hsh = ScanHash();

                return [frame, hsh];
            }
        }
    }
}
