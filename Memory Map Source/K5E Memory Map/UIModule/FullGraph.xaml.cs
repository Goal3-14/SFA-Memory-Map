using Microsoft.Msagl.GraphViewerGdi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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

namespace K5E_Memory_Map.UIModule
{
    /// <summary>
    /// Interaction logic for FullGraph.xaml
    /// </summary>
    public partial class FullGraph : UserControl, INotifyPropertyChanged
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

        public string Root;
        double RootX;
        double RootY;

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

        public int Depth = 1;
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

        public bool Focus = true;

        public Dictionary<string, TreeNode> NodeHash = new Dictionary<string, TreeNode>();

        private TreeNode NodeBuf;
        private string StrBuf;

        System.Windows.Media.Brush brush;

        double CentreX;
        double CentreY;


        public FullGraph()
        {
            DataContext = this;
            InitializeComponent();


            InitializeGraphViewer();

            SetupTransformations();
            SScrollViewer.MouseDown += scrollViewer_MouseDown;
            SScrollViewer.MouseMove += scrollViewer_MouseMove;
            SScrollViewer.MouseUp += scrollViewer_MouseUp;

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


        public void DisplayGraph(Dictionary<string, TreeNode> nodeHash)
        {

            var graph = new Microsoft.Msagl.Drawing.Graph();

            ClearGraph();


            // Iterate over the nodes and create graph edges
            foreach (var nodeEntry in nodeHash.Values)
            {

                var nodeName = nodeEntry.Mem; // Access the member string of TreeNode

                // Create a node with custom properties
                var node = graph.AddNode(nodeName);
                //node.LabelText = $"\n {nodeName} \n {nodeEntry.TagText}\n {nodeEntry.Text}"; // Add extra text
                node.LabelText = $"{nodeName} \n \n \n";

                //System.Drawing.Color color = System.Drawing.Color.FromName(nodeEntry.Colour);
                //node.Attr.FillColor = new Microsoft.Msagl.Drawing.Color(color.A,color.R,color.G,color.B);
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







            //Set Node Coords and Calculate displayed Text

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



                }










                //SaveStated Grey Box
                if (ShowState == true)
                {
                    if (NodeBuf.Stated == true)
                    {



                        //System.Windows.Media.Brush SBrush = (System.Windows.Media.Brush)typeof(System.Windows.Media.Brushes).GetProperty(NodeBuf.Colour)?.GetValue(null, null);
                        System.Windows.Media.Brush SBrush = System.Windows.Media.Brushes.DarkGray;

                        // Create a button for the node
                        Rectangle SRect = new Rectangle
                        {

                            Width = 170,
                            Height = 70,
                            Tag = NodeBuf.Mem,

                        };
                        if (ShowCol == true)
                        {
                            SRect.Fill = SBrush;
                        }



                        // Set the button's position
                        Canvas.SetLeft(SRect, x + 10);
                        Canvas.SetBottom(SRect, y + 10);

                        // Add the button to the Canvas
                        MyCanvas.Children.Add(SRect);



                    }
                }











                // Create a button for the node

                if (_MainWindow.MenuChoice == "4") //If using Practice
                {
                    if (NodeBuf.PracPath == true)
                    {
                        brush = (System.Windows.Media.Brushes.LightGreen);
                    }
                    else if (NodeBuf.PracPath == false)
                    {
                        brush = (System.Windows.Media.Brushes.Tomato);
                    }
                    else
                    {
                        brush = (System.Windows.Media.Brushes.PaleTurquoise);
                    }
                }
                else
                {
                    brush = (System.Windows.Media.Brush)typeof(System.Windows.Media.Brushes).GetProperty(NodeBuf.Colour)?.GetValue(null, null);
                }
                
                Button nodeButton = new Button
                {
                    Content = StrBuf, // Set the button content to the node label

                    Width = 170,               
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







                //Outline Current Node Blue
                if (CurrentNode != null)
                {
                    if (NodeBuf.Mem == CurrentNode.Mem)
                    {
                        System.Windows.Shapes.Rectangle CurrentBorder = new System.Windows.Shapes.Rectangle
                        {
                            Width = 190,
                            Height = 90,
                            StrokeThickness = 3,
                            Stroke = System.Windows.Media.Brushes.Blue
                        };

                        Canvas.SetLeft(CurrentBorder, x - 10);
                        Canvas.SetBottom(CurrentBorder, y - 10);

                        CentreX = x;
                        CentreY = y;
                        MyCanvas.Children.Add(CurrentBorder);
                    }

                }
                


                //Get Root Coords

                if (NodeBuf.Mem == Root)
                {
                    RootX = x;
                    RootY = y;
                }




                //Edges

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
                    //Debug.WriteLine(MyCanvas.ActualWidth);
                    //Debug.WriteLine(MyCanvas.ActualHeight);
                    //double StretchFactorX = MyCanvas.ActualWidth - 751;
                    double StretchFactorY = MyCanvas.ActualHeight - 451;

                    Line connectionLine = new Line
                    {
                        X1 = (startX) + 85, // Start X (bottom left of the parent button)
                        Y1 = (-startY + StretchFactorY) + 335, // Start Y (bottom left of the parent button)
                        X2 = (endX) + 85,   // End X (bottom left of the child button)
                        Y2 = (-endY + StretchFactorY) + 335,   // End Y (bottom left of the child button)
                        Stroke = System.Windows.Media.Brushes.Black, // Line color
                        StrokeThickness = 3, // Line thickness
                    };





                    // Add the line to the Canvas
                    MyCanvas.Children.Add(connectionLine);
                    Panel.SetZIndex(nodeButton, 1); // Buttons on top with higher Z-Index
                }
            }

            // Assign the graph to the viewer
            _GraphViewer.Graph = graph; // Set the graph to the GViewer
            _GraphViewer.ZoomF = 0.4;

            if (Focus == true)
            {
                CentreTranslation();
            }

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




            float Dist = 350 / (Nodes.Count + 1);
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
                Grid.Margin = new Thickness(Dist * ID, 0, 0, 0);
                //Grid.VerticalAlignment = VerticalAlignment.Center;
                Grid.HorizontalAlignment = HorizontalAlignment.Left;

                // Add the button to the grid
                Grid.Children.Add(button);

                ID++;
            }
        }

        //Think this is old one, but not sure
        private void Display()
        {


            Grid.RowDefinitions.Clear();
            Grid.ColumnDefinitions.Clear();
            Grid.Children.Clear();

            int columns = Breadth;
            int rows = Zoom;

            for (int i = 0; i < rows - 1; i++)
            {
                Grid.RowDefinitions.Add(new RowDefinition());
            }

            MakeRow(Parents, 0);
            MakeRow(new List<TreeNode> { CurrentNode }, 1);

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
                //Debug.WriteLine(CurrentNode.Mem);
                _MainWindow.SelectedNode = SelectedNode;
            }
        }







        private ScaleTransform scaleTransform = new ScaleTransform(1, 1);
        private TranslateTransform translateTransform = new TranslateTransform(0, 0);
        private TransformGroup transformGroup = new TransformGroup(); // Hold both transforms

        private void SetupTransformations()
        {
            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(translateTransform);
            transformGroup.Children.Add(scaleTransform);

            MyCanvas.RenderTransform = transformGroup;
        }






        private bool isDragging = false;
        private System.Windows.Point lastMousePosition;

        private void scrollViewer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                isDragging = true;

                //Debug.WriteLine("");
                //Debug.WriteLine(e.GetPosition(SScrollViewer).X);
                //Debug.WriteLine(e.GetPosition(SScrollViewer).Y);

                //Debug.WriteLine(e.GetPosition(MyCanvas).X);
                //Debug.WriteLine(e.GetPosition(MyCanvas).Y);

                System.Windows.Point canvasPosition = MyCanvas.TranslatePoint(new System.Windows.Point(0, 0), SScrollViewer);
                //Debug.WriteLine(canvasPosition);
                canvasPosition.X += e.GetPosition(MyCanvas).X;
                canvasPosition.Y += e.GetPosition(MyCanvas).Y;
                //Debug.WriteLine(canvasPosition);
                //Debug.WriteLine($"{tx} , {ty}");

                lastMousePosition = e.GetPosition(SScrollViewer);
                Mouse.Capture(SScrollViewer);
            }
        }

        private void scrollViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                System.Windows.Point currentMousePosition = e.GetPosition(SScrollViewer);
                double offsetX = currentMousePosition.X - lastMousePosition.X;
                double offsetY = currentMousePosition.Y - lastMousePosition.Y;

                translateTransform.X += offsetX;
                translateTransform.Y += offsetY;

                lastMousePosition = currentMousePosition;
            }
        }

        private void scrollViewer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isDragging)
            {
                isDragging = false;
                Mouse.Capture(null);
            }
        }



        private double tx;
        private double ty;


        private Double zoomMax = 5;
        private Double zoomMin = 0.5;
        private Double zoomSpeed = 0.001;
        private Double zoom = 1;


        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double oldZoom = zoom;
            zoom += zoomSpeed * e.Delta; // Ajust zooming speed (e.Delta = Mouse spin value )
            if (zoom < zoomMin) { zoom = zoomMin; } // Limit Min Scale
            if (zoom > zoomMax) { zoom = zoomMax; } // Limit Max Scale

            System.Windows.Point mousePos = e.GetPosition(MyCanvas);

            var TPoint = new Node();
            MyCanvas.Children.Add(TPoint);
            Canvas.SetLeft(TPoint, mousePos.X);
            Canvas.SetTop(TPoint, mousePos.Y);

            System.Windows.Point StartPos = MyCanvas.TranslatePoint(new System.Windows.Point(0, 0), SScrollViewer);
            StartPos.X += Canvas.GetLeft(TPoint);
            StartPos.Y += Canvas.GetTop(TPoint);

            scaleTransform.ScaleX = zoom;
            scaleTransform.ScaleY = zoom;

            MyCanvas.UpdateLayout();

            System.Windows.Point EndPos = MyCanvas.TranslatePoint(new System.Windows.Point(0, 0), SScrollViewer);
            //EndPos.X += Canvas.GetLeft(TPoint);
            EndPos.Y += Canvas.GetTop(TPoint);

            double x = Canvas.GetLeft(TPoint);
            double y = Canvas.GetTop(TPoint);

            // Keep the zoom centered around the mouse position
            double offsetX = (EndPos.X - StartPos.X);
            double offsetY = (EndPos.Y - StartPos.Y); // Correctly calculate offset for Y



            // Adjust the translation to keep the zoom centered
            //translateTransform.X += offsetX;
            //translateTransform.Y += offsetY;


            System.Windows.Point EndPoss = MyCanvas.TranslatePoint(new System.Windows.Point(0, 0), SScrollViewer);
            translateTransform.X -= EndPoss.X;
            translateTransform.Y -= EndPoss.Y;

            EndPoss = MyCanvas.TranslatePoint(new System.Windows.Point(0, 0), SScrollViewer);
            translateTransform.X -= EndPoss.X;
            translateTransform.Y -= EndPoss.Y;

            EndPoss = MyCanvas.TranslatePoint(new System.Windows.Point(0, 0), SScrollViewer);
            translateTransform.X -= EndPoss.X;
            translateTransform.Y -= EndPoss.Y;

            EndPoss = MyCanvas.TranslatePoint(new System.Windows.Point(0, 0), SScrollViewer);
            translateTransform.X -= EndPoss.X;
            translateTransform.Y -= EndPoss.Y;

            if (Focus == true)
            {
                CentreTranslation();
            }


            // Mark the event as handled to prevent the default scrolling behavior
            e.Handled = true;

        }
        int del = 0;
        public void CentreTranslation()
        {
            
                translateTransform.X = -CentreX +300;
                translateTransform.Y = CentreY -180;
            
        }

        public void CentreRoot()
        {
            translateTransform.X = -RootX + 300;
            translateTransform.Y = RootY - 180;
        }

        public void ResetZoom()
        {
            scaleTransform.ScaleX = 1;
            scaleTransform.ScaleY = 1;
            zoom = 1;
        }









        public void StartGraph(Dictionary<string, TreeNode> nodeHash)
        {
            
            CurrentNode = nodeHash.First().Value;

            var graph = new Microsoft.Msagl.Drawing.Graph();

            ClearGraph();
            // Iterate over the nodes and create graph edges
            foreach (var nodeEntry in nodeHash.Values)
            {

                var nodeName = nodeEntry.Mem; // Access the member string of TreeNode

                // Create a node with custom properties
                var node = graph.AddNode(nodeName);
                //node.LabelText = $"\n {nodeName} \n {nodeEntry.TagText}\n {nodeEntry.Text}"; // Add extra text
                node.LabelText = $"{nodeName} \n \n \n";

                //System.Drawing.Color color = System.Drawing.Color.FromName(nodeEntry.Colour);
                //node.Attr.FillColor = new Microsoft.Msagl.Drawing.Color(color.A,color.R,color.G,color.B);
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
                    //graph.AddEdge(parent.Mem, nodeName); 
                }

                // Add edges to the children
                foreach (var child in nodeEntry.Children)
                {
                    if (graph.FindNode(child.Mem) == null)
                    {
                        graph.AddNode(child.Mem); // Add child node if it doesn't exist
                    }
                    graph.AddEdge(nodeName, child.Mem); 
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
                        if (NodeBuf.Stated == true)
                        {



                            //System.Windows.Media.Brush SBrush = (System.Windows.Media.Brush)typeof(System.Windows.Media.Brushes).GetProperty(NodeBuf.Colour)?.GetValue(null, null);
                            System.Windows.Media.Brush SBrush = System.Windows.Media.Brushes.DarkGray;

                            // Create a button for the node
                            Rectangle SRect = new Rectangle
                            {

                                Width = 170,               // Set an appropriate width
                                Height = 70,
                                Tag = NodeBuf.Mem,

                            };
                            if (ShowCol == true)
                            {
                                SRect.Fill = SBrush;
                            }



                            // Set the button's position
                            Canvas.SetLeft(SRect, x + 10);
                            Canvas.SetBottom(SRect, y + 10);

                            // Add the button to the Canvas
                            MyCanvas.Children.Add(SRect);



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
                        //Debug.WriteLine(MyCanvas.ActualWidth);
                        //Debug.WriteLine(MyCanvas.ActualHeight);
                        //double StretchFactorX = MyCanvas.ActualWidth - 751;
                        double StretchFactorY = MyCanvas.ActualHeight - 451;

                        Line connectionLine = new Line
                        {
                            X1 = (startX) + 85, // Start X (bottom left of the parent button)
                            Y1 = (-startY + StretchFactorY) + 335, // Start Y (bottom left of the parent button)
                            X2 = (endX) + 85,   // End X (bottom left of the child button)
                            Y2 = (-endY + StretchFactorY) + 335,   // End Y (bottom left of the child button)
                            Stroke = System.Windows.Media.Brushes.Black, // Line color
                            StrokeThickness = 3, // Line thickness
                        };






                        // Add the line to the Canvas
                        MyCanvas.Children.Add(connectionLine);
                        Panel.SetZIndex(nodeButton, 1); // Buttons on top with higher Z-Index
                    }
                }
            }

            // Assign the graph to the viewer
            _GraphViewer.Graph = graph; // Set the graph to the GViewer
            _GraphViewer.ZoomF = 0.4;

            if (Focus == true)
            {
                CentreTranslation();
            }

        }

















    }
}
