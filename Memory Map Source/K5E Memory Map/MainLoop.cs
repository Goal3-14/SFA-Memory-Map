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
using System.Collections.Concurrent;
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


        private readonly ConcurrentQueue<(int,string)> queue;



        public MainLoop(MainWindow mainWindow, ConcurrentQueue<(int, string)> _queue)
        {

            _MainWindow = mainWindow;
            _MainWindow.SSaveMenu._mainLoop = this;
            _cancellationTokenSource = new CancellationTokenSource();

            queue = _queue;
            

        }






        //Get Data From Memory Viewer

        private (string,int) GetValues()
        {
            while (true)
            {
                if (queue.TryDequeue(out var item))
                {
                    int? F = item.Item1;
                    string? M = item.Item2;

                    if ((M != null) && (F != null))
                    {
                        return (M, (int)F);
                    }
                    Thread.Sleep(1);
                }
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
            var token = _cancellationTokenSource.Token;


            






            NodeHash = new Dictionary<string, TreeNode>();
            //TreeNode root = new TreeNode("95C99029EE2F5684DADC658F79FF51BA", NodeHash, null);

            _MainWindow.Process = "2";
            (StartFrame, StartMem, CurrentNode) = Reset(NodeHash);
            Debug.WriteLine("BEGIN");
            Debug.WriteLine(StartFrame);


            
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
                else if (Loading == true)
                {
                    
                }
                else
                {
                    

                    (MemText, Frame) = GetValues();
                    //_MainWindow.Hash = MemText;
                    //_MainWindow.Frame = Frame;




                    if ((Frame - StartFrame < 50) && (Frame - StartFrame >= 0)) //Check continuity, no large jump in frame count
                    {





                        if (true) //Check memory is different and check havent read Address mid read and gotten half new value half old
                        {

                            if (NodeHash.TryGetValue(MemText, out TreeNode foundObject))
                            {
                                //Node exists
                                BufferNode = foundObject;

                                if (!CurrentNode.Parents.Contains(BufferNode))
                                {
                                    CurrentNode.AddChild(BufferNode);
                                    BufferNode.AddParent(CurrentNode);
                                }

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
                        }


                    }
                    else //Large Change in Frame Count, believes state loaded, Tries to find position in tree
                    {
                        //Debug.WriteLine(StartFrame);
                        //Debug.WriteLine(Frame);

                        //Thread.Sleep(300);
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

                        }
                        else //Keep trying to find Memory in tree 
                        {
                            _MainWindow.Process = "2";
                            //Thread.Sleep(10);
                            (StartFrame, StartMem, CurrentNode) = Reset(NodeHash);
                            _MainWindow.Process = "1";
                        }


                    }

                    Cycle++;
                }



                
                if (Cycle == 30)
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
                    //Debug.WriteLine($"Iterations per second: {iterations / elapsedSeconds:F2}");
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
