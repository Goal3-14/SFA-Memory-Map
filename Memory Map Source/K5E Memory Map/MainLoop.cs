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

        public TreeNode CurrentNode;
        private TreeNode BufferNode;

        public bool Paused = false;
        public bool Loading = false;
        private int Cycle = 0;

        public Dictionary<string, TreeNode> NodeHash;
        public Dictionary<string, TreeNode>? LoadNodeHash = null;






        public MainLoop(MainWindow mainWindow)
        {

            _MainWindow = mainWindow;
            _MainWindow.SSaveMenu._mainLoop = this;
            _cancellationTokenSource = new CancellationTokenSource();
            

        }






        //Get Data From Memory Viewer

        private static (string,int) GetValues()
        {
            while (true)
            {
                string? M = GetMem();
                int? F = GetFrame();
                if ((M != null) && (F != null))
                {
                    return (M, (int)F);
                }
                Thread.Sleep(10);
            }

        }

        private static string? GetMem() 
        {
            string mapName = "MySharedMemory";

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
                Console.WriteLine("Could not open memory-mapped file for MemText: " + ex.Message);
                return null;
            }
            
            
        }

        private static int? GetFrame()
        {
            string FmapName = "FrameMemory";
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
                Console.WriteLine("Could not open memory-mapped file for frame: " + ex.Message);
                return null;
            }
        }

        







        public static (int,string,TreeNode) Reset( Dictionary<string,TreeNode> NodeHash)
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

                Thread.Sleep(10);

            }

            return (F, M, CNode);


        }







        public void Run(Dictionary<string, TreeNode>? LoadNodeHash)
        {
            Debug.WriteLine("Thread Start");
            
            NodeHash = new Dictionary<string, TreeNode>();
            //TreeNode root = new TreeNode("95C99029EE2F5684DADC658F79FF51BA", NodeHash, null);
            

            _MainWindow.Process = "2";
            (StartFrame, StartMem, CurrentNode) = Reset(NodeHash);
            Debug.WriteLine("BEGIN");
            Debug.WriteLine(StartFrame);


            // Get the cancellation token
            var token = _cancellationTokenSource.Token;
            _MainWindow.Process = "1";
            

            while (!token.IsCancellationRequested)
            {
               if (_MainWindow.Process == "4")
                {

                }
                else if ((Paused == true) || (Loading == true))
                {
                    _MainWindow.Process = "1";
                }
                else
                {
                    

                    (MemText, Frame) = GetValues();
                    _MainWindow.Hash = MemText;
                    _MainWindow.Frame = Frame;




                    if ((Frame - StartFrame < 50) && (Frame - StartFrame >= 0)) //Check continuity, no large jump in frame count
                    {





                        if ((MemText.Substring(0, 4) != StartMem.Substring(0, 4)) && (MemText.Substring(28) != StartMem.Substring(28))) //Check memory is different and check havent read Address mid read and gotten half new value half old
                        {

                            if (NodeHash.TryGetValue(MemText, out TreeNode foundObject))
                            {
                                //Node exists
                                BufferNode = foundObject;
                                CurrentNode.AddChild(BufferNode);
                                BufferNode.AddParent(CurrentNode);

                            }
                            else //Node doesnt exist
                            {
                                Debug.WriteLine("Node Added");

                                BufferNode = new TreeNode(MemText, NodeHash, CurrentNode);
                            }

                            CurrentNode = BufferNode;
                            StartFrame = (int)Frame;
                            StartMem = MemText;
                        }
                        else
                        {
                            StartFrame = (int)Frame;
                        }




                    }
                    else //Large Change in Frame Count, believes state loaded, Tries to find position in tree
                    {
                        Debug.WriteLine(StartFrame);
                        Debug.WriteLine(Frame);
                        Thread.Sleep(200);
                        //Sometimes when loading a state the frame count changes and the memory doesnt update until a read later
                        //so if state loaded it waits briefly before getting memory
                        //to ensure it uses memory after state not before as it will cause loops in the tree

                        (MemText, Frame) = GetValues();



                        if (NodeHash.TryGetValue(MemText, out TreeNode foundObject))
                        {
                            //Node exists
                            CurrentNode = foundObject;
                            StartFrame = (int)Frame;
                            StartMem = MemText;

                        }
                        else //Keep trying to find Memory in tree 
                        {
                            _MainWindow.Process = "2";
                            Thread.Sleep(500);
                            (StartFrame, StartMem, CurrentNode) = Reset(NodeHash);
                            _MainWindow.Process = "1";
                        }


                    }

                    Cycle++;
                }



                
                if (Cycle == 20)
                    {
                    _MainWindow.Dispatcher.Invoke(() =>
                    {
                        _MainWindow.UpdateLoop(CurrentNode,NodeHash);
                        Debug.WriteLine(NodeHash.Count);
                    });

                    Cycle = 0;
                    }





                Thread.Sleep(10); // Simulate work
            }
        }

        
        
        










        
        
        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
