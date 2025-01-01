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
using System.Xml.Serialization;

namespace K5E_Memory_Map.UIModule
{
    /// <summary>
    /// Interaction logic for TagButtons.xaml
    /// </summary>
    public partial class TagButtons : UserControl, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int _graphtype = 0;
        public int GraphType
        {
            get => _graphtype;
            set
            {
                if (_graphtype != value)
                {
                    _graphtype = value;
                    Grid.Children.Clear();
                    Grid.RowDefinitions.Clear();
                    Grid.ColumnDefinitions.Clear();
                    ButtonGrid(4, 4);
                    OnPropertyChanged(nameof(GraphType));
                }
            }
        }

        int[,,] TagLayout = {
            
        { //IM
        {1,3,6,9},
        {2,4,7,10},
        {11,5,8,0}},

        { //Void
        {11,12,13,5},
        {14,15,30,18},
        {16,17,19,0}},

        { //Krystal
        {20,21,22,23},
        {25,26,27,24},
        {28,29,9,0}},

        { //TTH
        {31,32,33,37},
        {34,35,36,38},
        {10,39,40,41}},

        { //Mammoth
        {42,43,44,45},
        {46,47,48,49},
        {50,51,52,53}}

        };
        
        string[,] TagData = {
        {"Special", "Yellow", "Special"}, //0

        //IM
        {"Bike", "LightBlue", "Bike"}, //1
        {"Cell", "LightBlue", "Cell"},  //2
        {"Exit", "LightGray", "Leave Cave"}, //3
        {"Cam", "LightGray", "Cam Stabalise"}, //4
        {"Drop", "LightGray", "Drop from IM"}, //5
        {"Gate", "SandyBrown", "Gate Clip"}, //6
        {"S-Drop", "SandyBrown", "Seam Drop"}, //7
        {"S-Land", "SandyBrown", "Seam Land"}, //8
        {"Arrive", "Aquamarine", "Arrive"},//9
        {"Start", "White", "Load Save/End CS"}, //10

        //Void
        
        {"Swim", "Cyan", "Swim Start"}, //11
        {"Turn", "Cyan", "Change Swim"}, //12
        {"Swimming", "Cyan", "Swimming"}, //13
        {"Enter", "Magenta", "Before Enter"},//14
        {"Reload", "Magenta", "Reload Map"},//15
        {"Pass", "LightGreen", "Success"},//16
        {"Fail", "Red", "Fail"},//17
        {"Height", "White", "Change Swim Height"},//18
        {"Deload", "White", "Deload Map"}, //19

        //Krystal
        {"Galleon", "White", ""},//20
        {"Scales", "White", ""},//21
        {"Wall 1", "White", ""},//22
        {"Pickup", "White", ""},//23
        {"Earthwalker", "White", ""},//24
        {"Ladder", "White", ""},//25
        {"Jellyfish", "White", ""},//26
        {"K1", "White", ""},//27
        {"Submit K1", "White", ""},//28
        {"Flight", "White", ""},//29

        {"Reload", "White", "Reload CC"},//30

        //TTH
        {"Staff", "White", "Staff"},//31
        {"SharpClaw", "Orange", "End Sharpclaw"},//32
        {"Fireblast", "Tomato", "Get Fireblast"},//33
        {"Scarabs", "Gold", "Get Scarabs"},//34
        {"Spores", "Violet", "Get Spore"},//35
        {"Queen", "DarkTurquoise", "Talk Queen"},//36
        {"Shop", "MediumSeaGreen", "Enter Shop"},//37
        {"Candy", "Peru", "Rock Candy"},//38
        {"Magic", "Aqua", "Pickup Magic"},//39
        {"Bomb", "Fuchsia", "Explode Bomb"},//40
        {"WS", "Aquamarine", "Talk Warpstone"},//41

        //Mammoth
        {"", "White", ""},//42
        {"", "White", ""},//43
        {"", "White", ""},//44
        {"", "White", ""},//45
        {"", "White", ""},//46
        {"", "White", ""},//47
        {"", "White", ""},//48
        {"", "White", ""},//49
        {"", "White", ""},//50
        {"", "White", ""},//51
        {"", "White", ""},//52
        {"", "White", ""} //53


        };

        string[,] MenuData = {
        {"IM", "Snow", "Special"}, //0

        //IM
        {"Void", "AliceBlue", "Bike"}, //1
        {"Krystal", "Plum", "Cell"},  //2
        {"TTH", "YellowGreen", "Leave Cave"}, //3
        {"Cam", "PowderBlue", "Cam Stabalise"} }; //4

        public TagButtons()
        {
            InitializeComponent();
            ButtonGrid(4, 4);
        }


        private void ButtonGrid(int rows , int columns)
        {


            


            Grid.RowDefinitions.Clear();
            Grid.ColumnDefinitions.Clear();
            for (int i = 0; i < rows; i++)
            {
                Grid.RowDefinitions.Add(new RowDefinition());
            }
            Grid.RowDefinitions[0].Height = new GridLength(25);
            for (int j = 0; j < columns; j++)
            {
                Grid.ColumnDefinitions.Add(new ColumnDefinition());
            }


            //Tabs
            for (int i = 0; i < 4; i++)
            {


                Button button = new Button();


                Brush brush = (Brush)typeof(Brushes).GetProperty(MenuData[i, 1])?.GetValue(null, null);



                button.Content = $"{MenuData[i, 0]}";
                button.Tag = i; // Store row and column as a tuple
                button.Click += Menu_Click;
                button.Background = brush;
                button.Height = 25;
                button.VerticalAlignment = VerticalAlignment.Top;
                
                // Set the button's position in the grid
                Grid.SetRow(button, 0);
                Grid.SetColumn(button, i);

                // Add the button to the grid
                Grid.Children.Add(button);


            }











            // Add buttons to the grid
            int ID = 0;
            for (int row = 0; row < rows-1; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    // Create a new button
                    Button button = new Button();

                    ID = TagLayout[GraphType,row,col];

                    Brush brush = (Brush)typeof(Brushes).GetProperty(TagData[ID,1])?.GetValue(null, null);



                    button.Content = $"{TagData[ID,0]}";
                    ID++;
                    button.Tag = (row, col); // Store row and column as a tuple
                    button.Click += Button_Click;
                    button.MouseRightButtonDown += Menu_RightClick;
                    button.Background = brush;

                    // Set the button's position in the grid
                    Grid.SetRow(button, row+1);
                    Grid.SetColumn(button, col);

                    // Add the button to the grid
                    Grid.Children.Add(button);
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
                }
            }
        }
        public MainWindow _MainWindow;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentNode != null)
            {
                if (sender is Button button)
                {

                    var (row, col) = ((int, int))button.Tag;
                    //MessageBox.Show($"{TagData[TagLayout[GraphType,row,col],2]}");
                    CurrentNode.AddTag(TagLayout[GraphType, row, col]);
                    _MainWindow.UpdateCurrent();
                    if (_MainWindow.Process == "4")
                    {
                        _MainWindow.Process = "1";
                    }
                }
            }
        }

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                GraphType = (int)button.Tag;
            }
        }



        private void Menu_RightClick(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("jlsgjlsjgl");
            if (sender is Button button)
            {
                if (_MainWindow.SSelectedDetails.Hash != null)
                {
                    var (row, col) = ((int, int))button.Tag;
                    _MainWindow.SSelectedDetails.CurrentNode.AddTag(TagLayout[GraphType, row, col]);
                    _MainWindow.UpdateCurrent();
                    e.Handled = true;
                }
            }
        }
    }
}
