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
    /// Interaction logic for DisplayMenu.xaml
    /// </summary>
    public partial class DisplayMenu : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public SmallGraph _SmallGraph;
        public FullGraph _FullGraph;
        public SelectedDetails _SelectedDetails;

        private string _menuchoice = "0";
        public string MenuChoice

        

        {
            get => _menuchoice;
            set
            {
                if (_menuchoice != value)
                {
                    _menuchoice = value; OnPropertyChanged(nameof(MenuChoice));
                    //Debug.WriteLine(_menuchoice.ToString());
                    if (_menuchoice == "4")
                    {
                        ShowPrac();
                    }
                    else
                    {
                        HidePrac();
                    }
                }
            }
        }

        
        public void DisplayMain()
        {

        }

        public DisplayMenu()
        {
            DataContext = this;
            InitializeComponent();

          
        }

        private void TextCheck(object sender, RoutedEventArgs e)
        {
            _SmallGraph.ShowText = true;
            _FullGraph.ShowText = true;
        }

        private void TextUn(object sender, RoutedEventArgs e)
        {
            _SmallGraph.ShowText = false;
            _FullGraph.ShowText = false;
        }

        private void MemCheck(object sender, RoutedEventArgs e)
        {
            _SmallGraph.ShowMem = true;
            _FullGraph.ShowMem = true;
        }

        private void MemUn(object sender, RoutedEventArgs e)
        {
            _SmallGraph.ShowMem = false;
            _FullGraph.ShowMem = false;
        }

        private void TagCheck(object sender, RoutedEventArgs e)
        {
            _SmallGraph.ShowTag = true;
            _FullGraph.ShowTag = true;
        }

        private void TagUn(object sender, RoutedEventArgs e)
        {
            _SmallGraph.ShowTag = false;
            _FullGraph.ShowTag = false;
        }

        private void ColCheck(object sender, RoutedEventArgs e)
        {
            _SmallGraph.ShowCol = true;
            _FullGraph.ShowCol = true;
        }

        private void ColUn(object sender, RoutedEventArgs e)
        {
            _SmallGraph.ShowCol = false;
            _FullGraph.ShowCol = false;
        }

        private void StatedCheck(object sender, RoutedEventArgs e)
        {
            _SmallGraph.ShowState = true;
            _FullGraph.ShowState = true;
        }

        private void StatedUn(object sender, RoutedEventArgs e)
        {
            _SmallGraph.ShowState = false;
            _FullGraph.ShowState = false;
        }

        private void SubMemCheck(object sender, RoutedEventArgs e)
        {

            if (sender is RadioButton button)
            {
                //Debug.WriteLine(button.Tag);
                _SmallGraph.SubMem = Convert.ToInt32(button.Tag);
                _FullGraph.SubMem = Convert.ToInt32(button.Tag);
            }
        }

        private void NormalGraph(object sender, RoutedEventArgs e)
        {
            _FullGraph.WindowsHost.Visibility = Visibility.Collapsed;
        }

        private void MSAGLGraph(object sender, RoutedEventArgs e)
        {
            _FullGraph.WindowsHost.Visibility = Visibility.Visible;
        }

        private void SetFocus(object sender, RoutedEventArgs e)
        {
            _FullGraph.Focus = true;
            _FullGraph.CentreTranslation();
        }

        private void SetUnFocus(object sender, RoutedEventArgs e)
        {
            _FullGraph.Focus = false;
        }

        private void CentreFull(object sender, RoutedEventArgs e)
        {
            _FullGraph.CentreTranslation();
        }

        private void ResetZoom(object sender, RoutedEventArgs e)
        {
            _FullGraph.ResetZoom();
        }



        private void ShowPrac()
        {
            PracButt.Visibility = Visibility.Visible;
        }

        private void HidePrac()
        {
            PracButt.Visibility= Visibility.Collapsed;
        }


        private void PracGood(object sender, RoutedEventArgs e)
        {
            _SelectedDetails.PracGood();
        }

        private void PracBad(object sender, RoutedEventArgs e)
        {
            _SelectedDetails.PracBad();
        }

        private void PracNeutral(object sender, RoutedEventArgs e)
        {
            _SelectedDetails.PracNeutral();
        }

        private void UpGood(object sender, RoutedEventArgs e)
        {
            _SelectedDetails.UpGood();
        }

        private void DownBad(object sender, RoutedEventArgs e)
        {
            _SelectedDetails.DownBad();
        }

        private void PracClear(object sender, RoutedEventArgs e)
        {
            _SelectedDetails._MainWindow.PracClear();
        }

        private void GoRoot(object sender, RoutedEventArgs e)
        {
            _FullGraph.CentreRoot();
        }

    }
}
