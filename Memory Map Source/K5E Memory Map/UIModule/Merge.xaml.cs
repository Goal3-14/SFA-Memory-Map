using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.IO;
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
using Microsoft.Msagl.Prototype.LayoutEditing;

namespace K5E_Memory_Map.UIModule
{
    /// <summary>
    /// Interaction logic for Merge.xaml
    /// </summary>
    public partial class Merge : UserControl
    {
        public MainWindow _MainWindow;
        string targetDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..", "Saved Maps"));
        string StateDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..", "SaveStates"));
        //string targetDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine( System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, @"..\..\..\..\..", "Saved Maps"));
        public Dictionary<string, TreeNode> NodeHash = new Dictionary<string, TreeNode>();

        public Dictionary<string, TreeNode>? Loaded1;
        public Dictionary<string, TreeNode>? Loaded2;
        private Dictionary<string, TreeNode>? Merged = null;
        string File1;
        string File2;
        string RootMem1;
        string RootMem2;

        public Merge()
        {
            InitializeComponent();
        }

        private (Dictionary<string, TreeNode>?,string?) LoadFile()
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
                    string FileName = openFileDialog.FileName;
                    // Load the graph data from the selected file
                    var loadedNodeHash = TreeNodeSerializer.LoadFromFile(openFileDialog.FileName, NodeHash);
                    


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



                    // Use loadedNodeHash as needed, e.g., update the UI or internal data structures
                    Debug.WriteLine("Graph data loaded successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    return(loadedNodeHash, System.IO.Path.GetFileNameWithoutExtension(FileName));
                    //_MainWindow.FFullGraph.StartGraph(loadedNodeHash);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Errrrrror loading graph data: {ex.Message}", "Errorrrrrrrrrrrrrrr", MessageBoxButton.OK, MessageBoxImage.Error);
                    
                }
            }
            return (null, null);

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


        private void Load1(object sender, RoutedEventArgs e)
        {
            (Loaded1, File1) = LoadFile();
            if (Loaded1 != null)
            {
                
                int States1 = 0;
                foreach (TreeNode Node in Loaded1.Values)
                {
                    if (Node.Stated == true)
                    {
                        States1++;
                    }
                }

                TreeNode Start = Loaded1.First().Value;

                RootMem1 = ParentCheck(Start);


                FileName1.Text = "Loaded File: " + File1;
                NodeCount1.Text = "Nodes: " + Loaded1.Count;
                Root1.Text = "Root: "+RootMem1;
                NumStates1.Text = "Save States: " + States1.ToString();
            }
        }







        private void Load2(object sender, RoutedEventArgs e)
        {
            (Loaded2, File2) = LoadFile();
            if (Loaded2 != null)
            {

                int States2 = 0;
                foreach (TreeNode Node in Loaded2.Values)
                {
                    if (Node.Stated == true)
                    {
                        States2++;
                    }
                }

                TreeNode Start = Loaded2.First().Value;

                RootMem2 = ParentCheck(Start);


                FileName2.Text = "Loaded File: " + File2;
                NodeCount2.Text = "Nodes: " + Loaded2.Count;
                Root2.Text = "Root: " + RootMem2;
                NumStates2.Text = "Save States: " + States2.ToString();
            }
        }

        private void MergeClick(object sender, RoutedEventArgs e)
        {
            Merged = null;
            if (Loaded1 != null)
            {
                if (Loaded2 != null)
                {
                    int i = 0;
                    TreeNode? Common = null;

                    if (RootMem1 == RootMem2)
                    {

                    }
                    else
                    {
                        if (Loaded1.TryGetValue(RootMem2, out TreeNode foundObject))
                        {
                            Common = foundObject;
                            i++;
                        }
                        if (Loaded2.TryGetValue(RootMem1, out TreeNode foundObject2))
                        {
                            Common = foundObject2;
                            i++;
                        }
                        if (i == 0)
                        {
                            //Multiple Roots (Would have to search all trees for common)
                            EError.Text = "Error: Neither contain others root, result multiple roots";
                        }
                        if (i == 2)
                        {
                            //Both below eachother therefor recursion
                            EError.Text = "Error: Both have other root in them therefor recursion";
                        }
                        if (i == 1)
                        {
                            //Good
                            //Good
                            EError.Text = "";
                        }

                    }
                }
            }
        }



        private void SaveFile(object sender, RoutedEventArgs e)
        {
            if (Merged != null)
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
                    TreeNodeSerializer.SaveToFile(saveFileDialog.FileName, Merged);
                }
            }
        }

    }




}
