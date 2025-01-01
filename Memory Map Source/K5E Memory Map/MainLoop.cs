using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Windows.Controls;
using System.IO.MemoryMappedFiles;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Markup;
using System.Diagnostics.Eventing.Reader;
using K5E_Memory_Map.UIModule;
using System.Windows.Threading;
namespace K5E_Memory_Map
{
    



    public class MainLoop
    {
        private readonly MainWindow _MainWindow;
        private CancellationTokenSource _cancellationTokenSource;

        public string MemText;
        public int Frame;

        private int StartFrame;
        private string StartMem;

        private string StartMemS;
        private string StartMemE;

        public TreeNode CurrentNode;
        private TreeNode BufferNode;

        public bool Paused = false;
        public bool Loading = false;
        private int Cycle = 0;

        public Dictionary<string, TreeNode> NodeHash;
        public Dictionary<string, TreeNode>? LoadNodeHash = null;


        string FmapName = "FrameMemory";
        string mapName = "MySharedMemory";


        public MainLoop(MainWindow mainWindow)
        {

            _MainWindow = mainWindow;
            _MainWindow.SSaveMenu._mainLoop = this;
            _cancellationTokenSource = new CancellationTokenSource();
            

        }






        //Get Data From Memory Viewer

        private (string,int) GetValues()
        {
            while (true)
            {
                string? M = GetMem();
                int? F = GetFrame();
                if ((M != null) && (F != null))
                {
                    return (M, (int)F);
                }
                Thread.Sleep(1);
            }

        }

        private string? GetMem() 
        {
            
            try
            {
                using (var mmf = MemoryMappedFile.OpenExisting(mapName))
                {
                    using (var accessor = mmf.CreateViewAccessor(0, 1024, MemoryMappedFileAccess.Read)) // Adjust size as necessary
                    {
                        byte[] buffer = new byte[1024]; // Buffer size must match what you wrote
                        accessor.ReadArray(0, buffer, 0, buffer.Length);
                        string memText = Encoding.UTF8.GetString(buffer).Trim('\0'); // Convert to string and trim null characters
                        if (memText == "")
                        {
                            return null;
                        }
                        return memText;
                    }
                }
            }
            catch (IOException ex)
            {   
                //Console.WriteLine("Could not open memory-mapped file for MemText: " + ex.Message);
                return null;
            }
            
            
        }

        private int? GetFrame()
        {
            
            try
            {
                using (var mmfFrame = MemoryMappedFile.OpenExisting(FmapName))
                {
                    using (var accessor = mmfFrame.CreateViewAccessor(0, 1024, MemoryMappedFileAccess.Read)) // Adjust size as necessary
                    {
                        byte[] buffer = new byte[1024]; // Buffer size must match what you wrote
                        accessor.ReadArray(0, buffer, 0, buffer.Length);
                        string frame = Encoding.UTF8.GetString(buffer).Trim('\0'); // Convert to string and trim null characters
                        try
                        {
                            return Convert.ToInt32(frame);
                        }
                        catch (FormatException)
                        {
                            return null;
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                //Console.WriteLine("Could not open memory-mapped file for frame: " + ex.Message);
                return null;
            }
        }




        public (string, string) SubText(string Mem)
        {
            string Start = StartMem.Substring(0, 4);
            string End = StartMem.Substring(28);

            return (Start, End);
        }




        public (int,string,TreeNode) Reset( Dictionary<string,TreeNode> NodeHash)
        {
            
            string M = "";
            int F = 0;
            TreeNode CNode = null;

            while (true)
            {



                (M, F) = GetValues();

                if (NodeHash.TryGetValue(M, out TreeNode foundObject))
                {
                    CNode = foundObject;
                    break;
                }

                Thread.Sleep(5);
            }

            return (F, M, CNode);


        }







        public void Run(Dictionary<string, TreeNode>? LoadNodeHash)
        {
            Debug.WriteLine("Thread Start");
            
            NodeHash = new Dictionary<string, TreeNode>();
            //TreeNode root = new TreeNode("95C99029EE2F5684DADC658F79FF51BA", NodeHash, null);

            MemoryMappedFile mmfB;

            try
            {
                mmfB = MemoryMappedFile.OpenExisting(mapName);
            }
            catch (IOException)
            {
                mmfB = MemoryMappedFile.CreateNew(mapName, 32);
            }

            try
            {
                mmfB = MemoryMappedFile.OpenExisting(FmapName);
            }
            catch (IOException)
            {
                mmfB = MemoryMappedFile.CreateNew(FmapName, 8);
            }


            _MainWindow.Process = "2";
            (StartFrame, StartMem, CurrentNode) = Reset(NodeHash);
            (StartMemS, StartMemE) = SubText(StartMem);
            Debug.WriteLine("BEGIN");
            Debug.WriteLine(StartFrame);


            // Get the cancellation token
            var token = _cancellationTokenSource.Token;
            _MainWindow.Process = "1";



            Stopwatch stopwatch = Stopwatch.StartNew();
            int iterations = 0;
            double elapsedSeconds = 0;

            if (_MainWindow.Paused == true) {
                _MainWindow.Process = "3";
            }


            while (!token.IsCancellationRequested)
            {
               if (_MainWindow.Process == "4" )
                {

                }
                else if ((_MainWindow.Paused == true) || (Loading == true))
                {
                    
                }
                else
                {
                    

                    (MemText, Frame) = GetValues();
                    _MainWindow.Hash = MemText;
                    _MainWindow.Frame = Frame;




                    if ((Frame - StartFrame < 50) && (Frame - StartFrame >= 0)) //Check continuity, no large jump in frame count
                    {





                        if ((MemText.Substring(0, 4) != StartMemS) && (MemText.Substring(28) != StartMemE)) //Check memory is different and check havent read Address mid read and gotten half new value half old
                        {

                            if (NodeHash.TryGetValue(MemText, out TreeNode foundObject))
                            {
                                //Node exists
                                BufferNode = foundObject;
                                CurrentNode.AddChild(BufferNode);
                                BufferNode.AddParent(CurrentNode);
                                CurrentNode = BufferNode;

                            }
                            else //Node doesnt exist
                            {
                                //Debug.WriteLine("Node Added");

                                CurrentNode = new TreeNode(MemText, NodeHash, CurrentNode);
                            }

                            //CurrentNode = BufferNode;
                            StartFrame = (int)Frame;
                            StartMem = MemText;
                            (StartMemS, StartMemE) = SubText(StartMem);
                        }
                        else
                        {
                            StartFrame = (int)Frame;
                        }




                    }
                    else //Large Change in Frame Count, believes state loaded, Tries to find position in tree
                    {
                        //Debug.WriteLine(StartFrame);
                        //Debug.WriteLine(Frame);
                        Thread.Sleep(300);
                        //Sometimes when loading a state the frame count changes and the memory doesnt update until a read later
                        //so if state loaded it waits briefly before getting memory
                        //to ensure it uses memory after state not before as it will cause loops in the tree

                        (MemText, Frame) = GetValues();
                        _MainWindow.Frame = Frame;


                        if (NodeHash.TryGetValue(MemText, out TreeNode foundObject))
                        {
                            //Node exists
                            CurrentNode = foundObject;
                            StartFrame = (int)Frame;
                            StartMem = MemText;
                            (StartMemS, StartMemE) = SubText(StartMem);

                        }
                        else //Keep trying to find Memory in tree 
                        {
                            _MainWindow.Process = "2";
                            Thread.Sleep(10);
                            (StartFrame, StartMem, CurrentNode) = Reset(NodeHash);
                            (StartMemS, StartMemE) = SubText(StartMem);
                            _MainWindow.Process = "1";
                        }


                    }

                    Cycle++;
                }



                
                if (Cycle == 100)
                    {
                    _MainWindow.Dispatcher.Invoke(() =>
                    {
                        _MainWindow.UpdateLoop(CurrentNode,NodeHash);
                        //Debug.WriteLine(NodeHash.Count);
                    });

                    Cycle = 0;
                    }





                Thread.Sleep(1);




                iterations++;

                if (stopwatch.Elapsed.TotalSeconds >= 1)
                {
                    elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
                    Debug.WriteLine($"Iterations per second: {iterations / elapsedSeconds:F2}");
                    iterations = 0;
                    stopwatch.Restart();
                }
            }
        }

        
        
        





        
        
        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
