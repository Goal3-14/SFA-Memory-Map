using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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

using System;
using System.IO;





namespace K5E_Memory_Map.UIModule
{

    public partial class NodeDetails : System.Windows.Controls.UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public MainWindow _MainWindow;


        private TreeNode _currentnode;
        public TreeNode CurrentNode
        {
            get => _currentnode;
            set
            {
                if (true)
                {
                    _currentnode = value;
                    
                    Hash = CurrentNode.Mem;
                    NodeTag = CurrentNode.TagText;
                    NodeText = CurrentNode.Text;
                    if (CurrentNode.Stated)
                    {
                        Stated = "Yes";
                    }
                    else
                    {
                        Stated = "No";
                    }
                    OnPropertyChanged(nameof(CurrentNode));
                }
            }
        }


        private string? _hash;
        public string? Hash
        {
            get => _hash;
            set
            {
                if (_hash != value)
                {
                    _hash = value;
                    OnPropertyChanged(nameof(Hash));
                }
            }
        }

        private string? _nodetag;
        public string? NodeTag
        {
            get => _nodetag;
            set
            {
                if (_nodetag != value)
                {
                    _nodetag = value;
                    OnPropertyChanged(nameof(NodeTag));
                }
            }
        }

        private string? _nodetext;
        public string? NodeText
        {
            get => _nodetext;
            set
            {
                if (_nodetext != value)
                {
                    _nodetext = value;
                    OnPropertyChanged(nameof(NodeText));
                }
            }
        }

        private string? _stated;
        public string? Stated
        {
            get => _stated;
            set
            {
                if (_stated != value)
                {
                    _stated = value;
                    OnPropertyChanged(nameof(Stated));
                }
            }
        }

        public NodeDetails()
        {
            InitializeComponent();
            DataContext = this;
        }




        string DolFolder = "";


        private void Save_State(object sender, RoutedEventArgs e) //bool to note whether it worked or not to know if set Stated var
        {
            string StateDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @".."));
            string fileExtension = "*.txt";
            try
            {
                string[] files = Directory.GetFiles(StateDirectory, fileExtension);
                DolFolder = System.IO.File.ReadAllText(files[0]);
            }
            catch (Exception ex)
            {

            }













            string NewFolder = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..", "SaveStates"));
            string StartFile = "GSAE01.s10";
            string NewFile = Hash+".sav";

            string sourceFilePath = System.IO.Path.Combine(DolFolder, StartFile);
            string destinationFilePath = System.IO.Path.Combine(NewFolder, NewFile);

            try
            {
                // Check if the source file exists
                if (File.Exists(sourceFilePath))
                {
                    // Copy the file from Folder A to Folder B with the new name
                    File.Copy(sourceFilePath, destinationFilePath, overwrite: true);

                    CurrentNode.Stated = true;
                    _MainWindow.UpdateGraphs();

                    //Debug.WriteLine($"File copied from {sourceFilePath} to {destinationFilePath} successfully.");
                }
                
            }
            catch (Exception ex)
            {
                
            }


        }

        private void SaveText(object sender, RoutedEventArgs e)
        {
            if (CurrentNode != null)
            {
                CurrentNode.Text = TextInput.Text;
                TextInput.Text = "";
                _MainWindow.UpdateGraphs();
            }
        }

        private void LoadLast(object sender, RoutedEventArgs e)
        {
            bool Checking = true;
            TreeNode CNode;
            TreeNode PNode = CurrentNode;
            while (Checking == true)
            {
                if (PNode.Parents.Count == 0)
                {
                    Checking = false;
                }
                else
                {
                    CNode = PNode.Parents[0];
                    if (CNode.Stated == true)
                    {
                        LoadState(CNode.Mem);
                        Checking = false;
                    }
                    else
                    {
                        PNode = CNode;
                    }
                }
            }
        }

        private void LoadState(string MemVal)
        {

        }

        private void WaitResult(object sender, RoutedEventArgs e)
        {
            if (_MainWindow.Process == "4")
            {
                _MainWindow.Process = "1";
            }
            else
            {
                _MainWindow.Process = "4";
            } 
        }

        private void AddUnder(object sender, RoutedEventArgs e)
        {
            _MainWindow.SSelectedDetails.AddAbove();
        }
    }
}
