using K5E_Memory_Map.UIModule;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace K5E_Memory_Map
{

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        //public MainLoop? MainLoop { get; private set; }

        private string? _hash; // Backing field for Hash property
        public string? Hash
        {
            get => _hash;
            set
            {
                if (_hash != value)
                {
                    _hash = value;
                    OnPropertyChanged(nameof(Hash)); // Notify the UI of the change
                }
            }
        }

        private TreeNode _currentnode;
        public TreeNode CurrentNode
        {
            get => _currentnode;
            set
            {
                if (_currentnode != value)
                {
                    _currentnode = value;
                    OnPropertyChanged(nameof(CurrentNode));
                    UpdateCurrent();
                }
            }
        }

        public void UpdateCurrent()
        {
            if (_currentnode != null)
            {
                NNodeDetails.CurrentNode = CurrentNode;

                SSmallGraph.NodeHash = NodeHash;
                SSmallGraph.CurrentNode = CurrentNode;

                FFullGraph.NodeHash = NodeHash;
                FFullGraph.CurrentNode = CurrentNode;

                TTagButtons.CurrentNode = CurrentNode;

                SSaveMenu.NodeHash = NodeHash;
            }
            else
            {
                UpdateGraphs();
            }
        }



        private TreeNode _selectednode;
        public TreeNode SelectedNode
        {
            get => _selectednode;
            set
            {
                if (_selectednode != value)
                {
                    _selectednode = value;
                    OnPropertyChanged(nameof(SelectedNode));
                    UpdateSelected();
                }
            }
        }

        public void UpdateSelected()
        {
            SSelectedDetails.CurrentNode = SelectedNode;
            //SSmallGraph.SelectedNode = SelectedNode;
        }

        private TreeNode _rselectednode;
        public TreeNode RSelectedNode
        {
            get => _rselectednode;
            set
            {
                if (_rselectednode != value)
                {
                    _rselectednode = value;
                    OnPropertyChanged(nameof(RSelectedNode));
                }
            }
        }






        public Dictionary<string, TreeNode> NodeHash = new Dictionary<string, TreeNode>();

        private int? _frame;
        public int? Frame
        {
            get => _frame;
            set
            {
                if (_frame != value)
                {
                    _frame = value;
                    OnPropertyChanged(nameof(Frame));
                }
            }
        }


        private string _process = "0";
        public string Process
        {
            get => _process;
            set
            {
                if (_process != value)
                {
                    _process = value;

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ProcessingState.Process = _process;
                    });

                }
            }
        }


        public Boolean Paused = false;
        public int Menu = 1;

        

        private Boolean TextToggle = true;
        private Boolean TagToggle = true;
        private Boolean MemToggle = true;
        private int MemLen = 8;
        private Boolean ColourToggle = false;

        private int Zoom = 1;
        private int Depth = 1;
        private int Breadth = 1;

        private UserControl[] Controls;

        

        private string _menuchoice = "2";
        public string MenuChoice



        {
            get => _menuchoice;
            set
            {
                if (_menuchoice != value)
                {
                    _menuchoice = value; OnPropertyChanged(nameof(MenuChoice));
                    ChangeGraph(_menuchoice);
                }
            }
        }


        public void UpdateLoop(TreeNode Current, Dictionary<string, TreeNode> newHash)
        {
            NodeHash = newHash;
            CurrentNode = Current;
        }


        private void ChangeGraph(string Choice)
        {
            foreach (var control in Controls)
            {
                control.Visibility = Visibility.Collapsed;
            }
            if (Choice == "4")
            {
                Controls[1].Visibility = Visibility.Visible;
            }
            else
            {
                Controls[Convert.ToInt32(MenuChoice)].Visibility = Visibility.Visible;
            }
            UpdateGraphs();

        }


        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }





        public void UpdateGraphs()
        {
            SSmallGraph.TriggerGraph();
            FFullGraph.TriggerGraph();
        }





        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            MMainMenu._displaymenu = DDisplayMenu;
            MMainMenu._mainwindow = this;
            TTagButtons._MainWindow = this;
            DDisplayMenu._SmallGraph = SSmallGraph;
            DDisplayMenu._FullGraph = FFullGraph;
            DDisplayMenu._SelectedDetails = SSelectedDetails;
            SSaveMenu._MainWindow = this;
            NNodeDetails._MainWindow = this;
            SSelectedDetails._MainWindow = this;
            SSmallGraph._MainWindow = this;
            FFullGraph._MainWindow = this;
            
            

            DDisplayMenu.Mem.IsChecked = true;
            DDisplayMenu.Text.IsChecked = true;
            DDisplayMenu.Tag.IsChecked = true;
            DDisplayMenu.Stated.IsChecked = true;
            DDisplayMenu.Colour.IsChecked = true;
            DDisplayMenu.DefSub.IsChecked = true;
            DDisplayMenu.Focus.IsChecked = true;

            Controls = new UserControl[] {
                SSmallGraph,
                FFullGraph,
                SSaveMenu,
                AAnalysis,
                PPractice,
                MMerge
            };

        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Signal the main loop to stop
            var app = (App)Application.Current;
            app.MainLoop?.Stop(); // Ensure you have a property to access MainLoop
        }

        public void GetMemDisplay(InfoLoop _InfoLoop)
        {
            _InfoLoop._MemDisplay = MMemDisplay;
        }

        private void PauseToggle(object sender, RoutedEventArgs e)
        {
            if (MenuChoice != "4")
            {
                if (Paused)
                {
                    Paused = false;
                    Process = "1";
                    PauseButton.Visibility = Visibility.Visible;
                    PlayButton.Visibility = Visibility.Collapsed;
                }
                else
                {
                    Process = "3";
                    Paused = true;
                    PlayButton.Visibility = Visibility.Visible;
                    PauseButton.Visibility = Visibility.Collapsed;
                }
            }
        }

        public void PracClear()
        {
            foreach (TreeNode Node in NodeHash.Values)
            {
                Node.PracPath = null;
            }
            UpdateCurrent();
            UpdateGraphs();
        }
    }
}