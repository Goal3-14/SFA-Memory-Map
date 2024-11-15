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

    public partial class ProcessingState : UserControl, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;

        string[] Processes = { "Disconnected", "Running", "Searching", "Paused", "Awaiting Tag" };
        string[] ProcessCol = { "Red", "Green", "Orange", "LightBlue", "Magenta" };

        private string _process = "Disconnected";
        public string Process
        {
            get => _process;
            set
            {
                if (_process != Processes[Convert.ToInt32(value)])
                {
                    int Index = Convert.ToInt32(value);
                    _process = Processes[Index];
                    Brush brush = (Brush)typeof(Brushes).GetProperty(ProcessCol[Index])?.GetValue(null, null);
                    GridPS.Background = brush;
                    OnPropertyChanged(nameof(Process));
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public ProcessingState()
        {
            DataContext = this;
            InitializeComponent();
            //Process = "2";
            
        }
    }
}
