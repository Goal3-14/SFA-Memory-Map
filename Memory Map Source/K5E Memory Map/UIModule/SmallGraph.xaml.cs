using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//using Microsoft.Msagl.WpfGraphControl;
using Microsoft.Msagl;
using Microsoft.Msagl.Drawing;
using System.Collections.Generic;
using System.Windows.Forms.Integration;
using Microsoft.Msagl.GraphViewerGdi;
using System.Drawing;
using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Layout.MDS;
using System.Windows.Forms.VisualStyles;
using System.Xml.Linq;

namespace K5E_Memory_Map.UIModule
{
    
    public partial class SmallGraph : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindow _MainWindow;

        private List<TreeNode> Parents;
        private List<TreeNode> Childs;
        private TreeNode[,] Layout;

        private TreeNode _currentnode;
        public TreeNode CurrentNode
        {
            get => _currentnode;
            set
            {
                if (true)
                {
                    _currentnode = value;
                    OnPropertyChanged(nameof(CurrentNode));
                    Parents = _currentnode.Parents;
                    Childs = _currentnode.Children;
                    //Debug.WriteLine(_currentnode.Mem);
                    DisplayGraph(NodeHash);
                    //Display();
                }
            }
        }

        private GViewer _GraphViewer;

        public int Depth =1;
        public int Breadth = 0;
        public int Zoom = 4;

        private bool _showmem = false;
        public bool ShowMem
        {
            get => _showmem;
            set
            {
                if (_showmem != value)
                {
                    _showmem = value;
                    DisplayGraph(NodeHash);
                }
            }
        }
        private int _submem = 16;
        public int SubMem
        {
            get => _submem;
            set
            {
                if (_submem != value)
                {
                    _submem = value;
                    DisplayGraph(NodeHash);
                }
            }
        }
        private bool? _showtag;
        public bool? ShowTag
        {
            get => _showtag;
            set
            {
                if (_showtag != value)
                {
                    _showtag = value;
                    DisplayGraph(NodeHash);
                }
            }
        }
        private bool? _showtext;
        public bool? ShowText
        {
            get => _showtext;
            set
            {
                if (_showtext != value)
                {
                    _showtext = value;
                    DisplayGraph(NodeHash);
                }
            }
        }
        private bool? _showstate;
        public bool? ShowState
        {
            get => _showstate;
            set
            {
                if (_showstate != value)
                {
                    _showstate = value;
                    DisplayGraph(NodeHash);
                }
            }
        }
        private bool? _showcol;
        public bool? ShowCol
        {
            get => _showcol;
            set
            {
                if (_showcol != value)
                {
                    _showcol = value;
                    DisplayGraph(NodeHash);
                }
            }
        }

        public Dictionary<string, TreeNode> NodeHash = new Dictionary<string, TreeNode>();

        private TreeNode NodeBuf;
        private string StrBuf;





        public SmallGraph()
        {
            DataContext = this;
            InitializeComponent();


            InitializeGraphViewer();

            

        }





        private void InitializeGraphViewer()
        {
            _GraphViewer = new GViewer();

            WindowsHost.Child = _GraphViewer;
            _GraphViewer.ToolBarIsVisible = true;
        }


        public void ClearGraph()
        {
            _GraphViewer.Graph = new Microsoft.Msagl.Drawing.Graph("Graph"); // Resetting to a new empty graph
            foreach (var child in MyCanvas.Children.OfType<Button>().ToList())
            {
                MyCanvas.Children.Remove(child);
            }
            foreach (var child in MyCanvas.Children.OfType<Line>().ToList())
            {
                MyCanvas.Children.Remove(child);
            }
            foreach (var child in MyCanvas.Children.OfType<System.Windows.Shapes.Rectangle>().ToList())
            {
                MyCanvas.Children.Remove(child);
            }

        }


        public void CreateButton(TreeNode Node, float dx, int i, int y)
        {
            double x = dx*i - 80;   // X coordinate of the node

            

            


                StrBuf = "";

                if (ShowMem == true)
                {
                    StrBuf = $"{StrBuf}{Node.Mem.Substring(0, SubMem)} \n";
                }
                if (ShowTag == true)
                {
                    StrBuf = $"{StrBuf}{Node.TagText} \n";
                }
                if (ShowText == true)
                {
                    StrBuf = $"{StrBuf}{Node.Text} \n";
                }
                if (ShowState == true)
                {
                    //StrBuf = StrBuf + NodeBuf.Mem;
                }


            

            System.Windows.Media.Brush brush = (System.Windows.Media.Brush)typeof(System.Windows.Media.Brushes).GetProperty(Node.Colour)?.GetValue(null, null);

            // Create a button for the node
            Button nodeButton = new Button
            {
                Content = StrBuf, // Set the button content to the node label

                Width = 170,               // Set an appropriate width
                Height = 70,
                Tag = Node.Mem,

            };
            if (ShowCol == true)
            {
                nodeButton.Background = brush;
            }

            nodeButton.Click += NodeClick;

            // Set the button's position
            Canvas.SetLeft(nodeButton, x);
            Canvas.SetBottom(nodeButton, y);

            // Add the button to the Canvas
            MyCanvas.Children.Add(nodeButton);









            double startX = dx*i -81;       // X coordinate for the line start (bottom-left of parent button)
            double startY = y; // Y coordinate for the line start

            double endX = 300;                                     // X coordinate for the line end (bottom-left of child button)
            double endY = 200;                     // Y coordinate for the line end

            // Create a line to connect the parent and child
            Line connectionLine = new Line
            {
                X1 = startX + 85, // Start X (bottom left of the parent button)
                Y1 = -startY +450, // Start Y (bottom left of the parent button)
                X2 = endX + 85,   // End X (bottom left of the child button)
                Y2 = -endY +450,   // End Y (bottom left of the child button)
                Stroke = System.Windows.Media.Brushes.Black, // Line color
                StrokeThickness = 3, // Line thickness
            };

            MyCanvas.Children.Add(connectionLine);
            Panel.SetZIndex(nodeButton, 1); // Buttons on top with higher Z-Index
        }

        public void DisplayGraph(Dictionary<string, TreeNode> nodeHash)
        {
            
            var graph = new Microsoft.Msagl.Drawing.Graph();
            
            ClearGraph();

            if (CurrentNode == null)
            {
                return;
            }


            List<TreeNode> LocalNodes = new List<TreeNode>();
            LocalNodes.Add(CurrentNode);

            foreach (var C in CurrentNode.Children)
            {
                LocalNodes.Add(C);
            }

            int Width = 767;
            int PWidth = CurrentNode.Parents.Count; //667       374
            int CWidth = CurrentNode.Children.Count;
            float Pdx = Width / (PWidth +1);
            float Cdx = Width / (CWidth+1);

            CreateButton(CurrentNode, Width / 2, 1, 200);

            int i = 1;
            foreach (var P in CurrentNode.Parents)
            {
                CreateButton(P, Pdx, i, 350);
                i++;
            }
            i = 1;
            foreach (var C in CurrentNode.Children)
            {
                CreateButton(C, Cdx, i, 50);
                i++;
            }

            return;

            // Iterate over the nodes and create graph edges
            foreach (var nodeEntry in LocalNodes)
            {

                var nodeName = nodeEntry.Mem; // Access the member string of TreeNode

                // Create a node with custom properties
                var node = graph.AddNode(nodeName);
                node.LabelText = $"{nodeName} \n \n \n";
                
                node.Attr.Shape = Microsoft.Msagl.Drawing.Shape.Box; // Set shape (choose from Circle, Square, Ellipse, etc.)
                
                

                if (graph.FindNode(nodeName) == null)
                {
                    graph.AddNode(nodeName); // Add the node if it doesn't exist
                }

                // Add edges to the parents
                foreach (var parent in nodeEntry.Parents)
                {
                    if (graph.FindNode(parent.Mem) == null)
                    {
                        graph.AddNode(parent.Mem); // Add parent node if it doesn't exist
                    }
                    //graph.AddEdge(parent.Mem, nodeName); // Create edge from parent to child
                }

                // Add edges to the children
                foreach (var child in nodeEntry.Children)
                {
                    if (graph.FindNode(child.Mem) == null)
                    {
                        graph.AddNode(child.Mem); // Add child node if it doesn't exist
                    }
                    graph.AddEdge(nodeName, child.Mem); // Create edge from child to parent
                }


            }




            







            Microsoft.Msagl.GraphViewerGdi.GraphRenderer renderer
            = new Microsoft.Msagl.GraphViewerGdi.GraphRenderer
            (graph);
            renderer.CalculateLayout();

            foreach (var node in graph.Nodes)
            {
                // Get node coordinates
                var nodePosition = node.Pos;
                double x = nodePosition.X + 250;   // X coordinate of the node
                double y = nodePosition.Y - 320;  // Y coordinate of the node

                string cleanedInput = new string(node.LabelText.Where(c => !char.IsWhiteSpace(c)).ToArray());

                if (!NodeHash.TryGetValue(cleanedInput, out var NodeBuf))
                {



                }
                else
                {


                    StrBuf = "";

                    if (ShowMem == true)
                    {
                        StrBuf = $"{StrBuf}{NodeBuf.Mem.Substring(0, SubMem)} \n";
                    }
                    if (ShowTag == true)
                    {
                        StrBuf = $"{StrBuf}{NodeBuf.TagText} \n";
                    }
                    if (ShowText == true)
                    {
                        StrBuf = $"{StrBuf}{NodeBuf.Text} \n";
                    }
                    if (ShowState == true)
                    {
                        //StrBuf = StrBuf + NodeBuf.Mem;
                    }


                }

                System.Windows.Media.Brush brush = (System.Windows.Media.Brush)typeof(System.Windows.Media.Brushes).GetProperty(NodeBuf.Colour)?.GetValue(null, null);

                // Create a button for the node
                Button nodeButton = new Button
                {
                    Content = StrBuf, // Set the button content to the node label

                    Width = 170,               // Set an appropriate width
                    Height = 70,
                    Tag = NodeBuf.Mem,

                };
                if (ShowCol == true)
                {
                    nodeButton.Background = brush;
                }

                nodeButton.Click += NodeClick;

                // Set the button's position
                Canvas.SetLeft(nodeButton, x);
                Canvas.SetBottom(nodeButton, y);

                // Add the button to the Canvas
                MyCanvas.Children.Add(nodeButton);









                if (NodeBuf.Mem == CurrentNode.Mem)
                {
                    System.Windows.Shapes.Rectangle CurrentBorder = new System.Windows.Shapes.Rectangle
                    {
                        Width = 190,               
                        Height = 90,
                        StrokeThickness = 3,
                        Stroke = System.Windows.Media.Brushes.Blue
                    };

                    Canvas.SetLeft(CurrentBorder, x -10);
                    Canvas.SetBottom(CurrentBorder, y - 10);

                    MyCanvas.Children.Add(CurrentBorder);
                }









                foreach (var inEdge in node.InEdges)
                {
                    var parentNode = inEdge.SourceNode;

                    // Get the parent node's coordinates
                    var parentPosition = parentNode.GeometryNode.Center;
                    double parentX = parentPosition.X + 250;   // X coordinate of the parent node
                    double parentY = parentPosition.Y - 320;   // Y coordinate of the parent node

                    // Set parent button's position
                    var parentButton = new Button
                    {
                        Width = 170,
                        Height = 70,
                    };
                    Canvas.SetLeft(parentButton, parentX);
                    Canvas.SetBottom(parentButton, parentY);

                    // Calculate the actual positions using Canvas properties
                    double startX = Canvas.GetLeft(parentButton) + 0;       // X coordinate for the line start (bottom-left of parent button)
                    double startY = Canvas.GetBottom(parentButton) - parentButton.Height; // Y coordinate for the line start

                    double endX = x + 0;                                     // X coordinate for the line end (bottom-left of child button)
                    double endY = y - nodeButton.Height;                     // Y coordinate for the line end

                    // Create a line to connect the parent and child
                    Line connectionLine = new Line
                    {
                        X1 = startX +85, // Start X (bottom left of the parent button)
                        Y1 = -startY + 335, // Start Y (bottom left of the parent button)
                        X2 = endX +85,   // End X (bottom left of the child button)
                        Y2 = -endY +335,   // End Y (bottom left of the child button)
                        Stroke = System.Windows.Media.Brushes.Black, // Line color
                        StrokeThickness = 3, // Line thickness
                    };

                    



                    // Add the line to the Canvas
                    MyCanvas.Children.Add(connectionLine);
                    Panel.SetZIndex(nodeButton, 1); // Buttons on top with higher Z-Index
                }
            }

            // Assign the graph to the viewer
            //_GraphViewer.Graph = graph; // Set the graph to the GViewer
            //_GraphViewer.ZoomF= 0.4;
            
        }





        public void TriggerGraph()
        {
            DisplayGraph(NodeHash);
        }
        





        private void MakeRow(List<TreeNode> Nodes, int Row)
        {
            if (Nodes.Count == 0)
            {
                return;
            }


            

            float Dist = 350 / (Nodes.Count+1);
            int ID = 1;
            foreach (TreeNode Node in Nodes)
            {


                Button button = new Button();



                //Brush brush = (Brush)typeof(Brushes).GetProperty(Node.Colour)?.GetValue(null, null);



                button.Content = $"{Node.Mem}";
                //button.Click += NodeClick;
                //button.Background = brush;
                button.Height = 80;
                button.Width = 250;


                // Set the button's position in the grid
                Grid.SetRow(button, Row);
                Grid.Margin = new Thickness(Dist*ID,0,0,0);
                //Grid.VerticalAlignment = VerticalAlignment.Center;
                Grid.HorizontalAlignment = HorizontalAlignment.Left;

                // Add the button to the grid
                Grid.Children.Add(button);

                ID++;
            }
        }

        private void Display()
        {
            

            Grid.RowDefinitions.Clear();
            Grid.ColumnDefinitions.Clear();
            Grid.Children.Clear();

            int columns = Breadth;
            int rows = Zoom;

            for (int i = 0; i < rows-1; i++)
            {
                Grid.RowDefinitions.Add(new RowDefinition());
            }

            MakeRow(Parents,0);
            MakeRow(new List<TreeNode> {CurrentNode},1);

            //MakeRow(Childs, 2);
            
            //for (int row = 0; row < zoom; row++)
            //{
                
            //        // Create a new button
                    
                
            //}





        }




        private void NodeClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                NodeHash.TryGetValue(button.Tag as string, out TreeNode SelectedNode);
                Debug.WriteLine(CurrentNode.Mem);
                _MainWindow.SelectedNode = SelectedNode;
            }
        }








    }
}
