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
        //string targetDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine( System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, @"..\..\..\..\..", "Saved Maps"));
        public Dictionary<string, TreeNode> NodeHash = new Dictionary<string, TreeNode>();
        public MainLoop _mainLoop;

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
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                    }



                    _mainLoop.LoadNodeHash = loadedNodeHash;
                    //_MainWindow.NodeHash = loadedNodeHash;
                    //_MainWindow.FFullGraph.NodeHash = loadedNodeHash;

                    _mainLoop.Loading = false;

                    // Use loadedNodeHash as needed, e.g., update the UI or internal data structures
                    Debug.WriteLine("Graph data loaded successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    //_MainWindow.FFullGraph.StartGraph(loadedNodeHash);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Errrrrror loading graph data: {ex.Message}", "Errorrrrrrrrrrrrrrr", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
                _MainWindow.UpdateCurrent();
                _MainWindow.UpdateGraphs();
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
    }
}
