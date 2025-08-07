using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace K5E_Memory_Map.UIModule
{
    
    public partial class MemDisplay : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        public string? FoxCoords = null;
        public string[] Buffer;

        private string? _foxx;
        public string? FoxX
        {
            get => _foxx;
            set
            {
                if (_foxx != value)
                {
                    _foxx = value;
                    OnPropertyChanged(nameof(FoxX));
                }
            }
        }

        private string? _foxy;
        public string? FoxY
        {
            get => _foxy;
            set
            {
                if (_foxy != value)
                {
                    _foxy = value;
                    OnPropertyChanged(nameof(FoxY));
                }
            }
        }

        private string? _foxz;
        public string? FoxZ
        {
            get => _foxz;
            set
            {
                if (_foxz != value)
                {
                    _foxz = value;
                    OnPropertyChanged(nameof(FoxZ));
                }
            }
        }


        public string? MountESWCoords = null;

        private string? _mountx;
        public string? MountX
        {
            get => _mountx;
            set
            {
                if (_mountx != value)
                {
                    _mountx = value;
                    OnPropertyChanged(nameof(MountX));
                }
            }
        }

        private string? _mounty;
        public string? MountY
        {
            get => _mounty;
            set
            {
                if (_mounty != value)
                {
                    _mounty = value;
                    OnPropertyChanged(nameof(MountY));
                }
            }
        }

        private string? _mountz;
        public string? MountZ
        {
            get => _mountz;
            set
            {
                if (_mountz != value)
                {
                    _mountz = value;
                    OnPropertyChanged(nameof(MountZ));
                }
            }
        }

        private string? _eswx;
        public string? ESWX
        {
            get => _eswx;
            set
            {
                if (_eswx != value)
                {
                    _eswx = value;
                    OnPropertyChanged(nameof(ESWX));
                }
            }
        }

        private string? _eswy;
        public string? ESWY
        {
            get => _eswy;
            set
            {
                if (_eswy != value)
                {
                    _eswy = value;
                    OnPropertyChanged(nameof(ESWY));
                }
            }
        }

        private string? _eswz;
        public string? ESWZ
        {
            get => _eswz;
            set
            {
                if (_eswz != value)
                {
                    _eswz = value;
                    OnPropertyChanged(nameof(ESWZ));
                }
            }
        }

        public MemDisplay()
        {
            InitializeComponent();
            DataContext = this;

        }

        public MainWindow _mainwindow;
        public MountMap _mountmap;

        public void ForcePause()
        {
            _mainwindow.Paused = true;
            _mainwindow.PlayButton.Visibility = Visibility.Visible;
            _mainwindow.PauseButton.Visibility = Visibility.Collapsed;
            _mainwindow.Process = "3";
            _mainwindow.Attatched = false;
        }

        public void UpdateLoop()
        {
            try
            {
                if (FoxCoords != null)
                {
                    Buffer = FoxCoords.Split(' ');
                    FoxX = BitConverter.ToSingle(BitConverter.GetBytes(Convert.ToInt32(Buffer[0])), 0).ToString();
                    FoxY = BitConverter.ToSingle(BitConverter.GetBytes(Convert.ToInt32(Buffer[1])), 0).ToString(); ;
                    FoxZ = BitConverter.ToSingle(BitConverter.GetBytes(Convert.ToInt32(Buffer[2])), 0).ToString(); ;
                }
                if (MountESWCoords != null)
                {
                    Buffer = MountESWCoords.Split(" ");

                    MountX = BitConverter.ToSingle(BitConverter.GetBytes(Convert.ToInt32(Buffer[0])), 0).ToString();
                    MountY = BitConverter.ToSingle(BitConverter.GetBytes(Convert.ToInt32(Buffer[1])), 0).ToString();
                    MountZ = BitConverter.ToSingle(BitConverter.GetBytes(Convert.ToInt32(Buffer[2])), 0).ToString();
                    _mountmap.MountX = float.Parse(MountX, CultureInfo.InvariantCulture.NumberFormat);
                    //_mountmap.MountY = float.Parse(MountY, CultureInfo.InvariantCulture.NumberFormat);
                    _mountmap.MountZ = float.Parse(MountZ, CultureInfo.InvariantCulture.NumberFormat);
                    _mountmap.Update();

                    if (Buffer[3] != "0")
                    {
                        ESWX = BitConverter.ToSingle(BitConverter.GetBytes(Convert.ToInt32(Buffer[3])), 0).ToString();
                        ESWY = BitConverter.ToSingle(BitConverter.GetBytes(Convert.ToInt32(Buffer[4])), 0).ToString();
                        ESWZ = BitConverter.ToSingle(BitConverter.GetBytes(Convert.ToInt32(Buffer[5])), 0).ToString();
                        ESWIndicator.Visibility = Visibility.Visible;
                    }
                    else
                    {

                        ESWX = "---";
                        ESWY = "---";
                        ESWZ = "---";
                        ESWIndicator.Visibility = Visibility.Hidden;
                    }

                }
            }
            catch
            {

            }
        }
    }
}
