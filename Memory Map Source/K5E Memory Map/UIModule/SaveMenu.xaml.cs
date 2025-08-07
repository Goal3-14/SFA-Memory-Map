using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

using System.Linq;
using System.Diagnostics;

using System.IO;
using System.Reflection;
using System.IO.MemoryMappedFiles;
using static System.Net.WebRequestMethods;
using K5E_Memory_Map.HeapVisualizer;
using SFACore.Engine.Common.Logging;
using SFACore.Engine.Common;
using SFACore.Engine.Memory;
using System.Security.Cryptography;
using System.Security.Policy;
using SFACore.Engine.Common.DataStructures;

namespace K5E_Memory_Map.UIModule
{
    /// <summary>
    /// Interaction logic for SaveMenu.xaml
    /// </summary>
    public partial class SaveMenu : System.Windows.Controls.UserControl
    {
        public MainWindow _MainWindow;
        string targetDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..", "Saved Maps"));
        string StateDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..", "SaveStates"));
        string StateTxtDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..", "Save State Folder Path.txt"));
        //string targetDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine( System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, @"..\..\..\..\..", "Saved Maps"));
        public Dictionary<string, TreeNode> NodeHash = new Dictionary<string, TreeNode>();
        public MainLoop _mainLoop;





        static readonly List<UInt32> HeapAddresses = new List<UInt32> { 0x80526020, 0x8112FF80, 0x812EFFA0, 0x8138E1E0, 0x81800000 /* End address */ };
        static readonly Int32 HeapCount = 4;
        static readonly UInt32 HeapTableAddress = 0x80340698;
        static readonly Int32 HeapImageWidth = 4096;
        static readonly Int32 HeapImageHeight = 1;
        static readonly Int32 DPI = 72;

        FullyObservableCollection<HeapVisualizer.HeapViewInfo> HeapViews;
        private MD5 md5 = MD5.Create();






        public SaveMenu()
        {
            InitializeComponent();
        }

        private void LoadFile(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                Title = "Load Graph Data",
                InitialDirectory = targetDirectory
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    _mainLoop.Loading = true;
                    // Load the graph data from the selected file
                    var loadedNodeHash = TreeNodeSerializer.LoadFromFile(openFileDialog.FileName, _mainLoop.NodeHash);
                    //_mainLoop.Stop();

                    //_mainLoop = new MainLoop(_MainWindow);
                    //Task.Run(() => _mainLoop.Run(loadedNodeHash));




                    //Add Stated Property
                    string fileExtension = "*.sav";
                    int States1 = 0;
                    try
                    {
                        // Get all files with the specified extension in the folder
                        string[] files = Directory.GetFiles(StateDirectory, fileExtension);

                        // Loop over each file path and get the file name only
                        foreach (string filePath in files)
                        {
                            // Get the file name (without the full path)
                            string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);

                            if (loadedNodeHash.TryGetValue(fileName, out TreeNode foundObject))
                            {
                               foundObject.Stated = true;
                               States1++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                    }

                    _MainWindow.FFullGraph.NodeHash = loadedNodeHash;
                    _MainWindow.UpdateGraphs();

                    _mainLoop.LoadNodeHash = loadedNodeHash;

                    ///////////////////////////////////////////////
                    _MainWindow.NodeHash = loadedNodeHash;
                    ///////////////////////////////////////////////

                    _mainLoop.Loading = false;

                    // Use loadedNodeHash as needed, e.g., update the UI or internal data structures
                    Debug.WriteLine("Graph data loaded successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    
                    

                    TreeNode Start = loadedNodeHash.First().Value;
                    string RootMem1 = ParentCheck(Start);
                    _MainWindow.FFullGraph.Root = RootMem1;

                    FileName1.Text = "Loaded File: " + System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                    NodeCount1.Text = "Nodes: " + loadedNodeHash.Count;
                    Root1.Text = "Root: " + RootMem1;
                    NumStates1.Text = "Save States: " + States1.ToString();
                    //_MainWindow.FFullGraph.StartGraph(loadedNodeHash);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Errrrrror loading graph data: {ex.Message}", "Errorrrrrrrrrrrrrrr", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }

        private string ParentCheck(TreeNode Node)
        {
            if (Node.Parents.Count == 0)
            {
                return Node.Mem;
            }
            else
            {
                return (ParentCheck(Node.Parents.First()));
            }
        }

        private void SaveFile(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                Title = "Save Graph Data",
                InitialDirectory = targetDirectory,
                FileName = "graph_data.json" // Default file name
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                NodeHash = _MainWindow.NodeHash;
                TreeNodeSerializer.SaveToFile(saveFileDialog.FileName, NodeHash);
            }
        }

        TreeNode? Root;
        string? Mem;

        private void NewRoot(object sender, RoutedEventArgs e)
        {
            Mem = GetMem();
            if (NodeHash.Count == 0)
            {
                if (Mem == null)
                {

                }
                else
                {
                    var NewNode = new TreeNode(Mem, _mainLoop.NodeHash, null);
                    _MainWindow.CurrentNode = NewNode;
                    _MainWindow.NodeHash = _mainLoop.NodeHash;
                    _MainWindow.UpdateCurrent();
                    _MainWindow.UpdateGraphs();
                }
            }
            else
            {
                if (!NodeHash.TryGetValue(Mem, out TreeNode foundObject))
                {
                    if (Mem == null)
                    {

                    }
                    else
                    {
                        var NewNode = new TreeNode(Mem, _mainLoop.NodeHash, null);
                        _MainWindow.CurrentNode = NewNode;
                        _MainWindow.NodeHash = _mainLoop.NodeHash;
                        _MainWindow.UpdateCurrent();
                        _MainWindow.UpdateGraphs();
                    }
                }
            }
            

            
        }



        private void NewTree(object sender, RoutedEventArgs e)
        {
            Mem = GetMem();

            if (Mem == null)
            {

            }
            else
            {
                _mainLoop.NodeHash.Clear();
                var NewNode = new TreeNode(Mem, _mainLoop.NodeHash, null);
                _MainWindow.CurrentNode = NewNode;
                _MainWindow.NodeHash = _mainLoop.NodeHash;
                _MainWindow.FFullGraph.Root = Mem;
                _MainWindow.UpdateCurrent();
                _MainWindow.UpdateGraphs();
            }
        }



        string? hash = null;

        private string? GetMem()
        {
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
                        hash = HeapViews[1].HeapHash?.ToString();
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "Error updating the Heap Visualizer", ex);
            }


            return (hash);
        }

        private void StateFolder(object sender, RoutedEventArgs e)
        {
            string userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile); 
            string DolphinFolder = System.IO.Path.Combine(userFolder, "AppData", "Roaming","Dolphin Emulator","StateSaves");

            if (!Directory.Exists(DolphinFolder))
            {
                DolphinFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }


            Microsoft.Win32.OpenFolderDialog StateFolderDialogue = new Microsoft.Win32.OpenFolderDialog
            {
                Title = "Select StateSaves Folder",
                InitialDirectory = DolphinFolder,
            };

            if (StateFolderDialogue.ShowDialog() == true)
            {
                System.IO.File.WriteAllText(StateTxtDirectory, StateFolderDialogue.FolderName);
            }
        }
    }
}
