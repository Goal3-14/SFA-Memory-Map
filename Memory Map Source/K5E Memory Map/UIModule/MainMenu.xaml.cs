using System;
using System.Collections.Generic;
using System.Diagnostics;
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

//////////////////////////////////////////////////////////////
using SFACore.Engine.Common;
using SFACore.Engine.Common.DataStructures;
using SFACore.Engine.Common.Logging;
using SFACore.Engine.Memory;
//using SFACore.Source.Docking;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using K5E_Memory_Map.HeapVisualizer;
using System.Data.SqlTypes;
using System.ComponentModel;
using System.Windows.Controls.Primitives;
//////////////////////////////////////////////////////////////


namespace K5E_Memory_Map.UIModule
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : UserControl, INotifyPropertyChanged
    {
        public MainMenu()
        {
            InitializeComponent();
            DataContext = this;   
        }
        public event PropertyChangedEventHandler? PropertyChanged;


        public DisplayMenu _displaymenu;
        public MainWindow _mainwindow;


        private bool _attached;
        public bool Attached
        {
            get => _attached;
            set
            {
                if (_attached != value)
                {
                    _attached = value;
                    OnPropertyChanged(nameof(Attached));
                    if (value == true)
                    {
                        TAttach.Visibility = Visibility.Visible;
                        FAttach.Visibility = Visibility.Hidden;
                        RRegion.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        FAttach.Visibility = Visibility.Visible;
                        TAttach.Visibility = Visibility.Hidden;
                        RRegion.Visibility = Visibility.Hidden;
                    }
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private void ResetCol()
        {
            PPractice.Background = Brushes.Transparent;
            FFull.Background = Brushes.Transparent;
            MMain.Background = Brushes.Transparent;
            SSave.Background = Brushes.Transparent;
            AAnalysis.Background = Brushes.Transparent;
            MMerge.Background = Brushes.Transparent;
            HHelp.Background = Brushes.Transparent;
        }


        private void MenuButton(object sender, RoutedEventArgs e)
        {
            // 0:Main 1:Full 2:save/load 3:Analysis 4:Praqctice 5:Merge
            if (sender is Button button)
            {
                ResetCol();
                button.Background = Brushes.DarkGray;
                _displaymenu.MenuChoice = (string)button.Tag;
                _mainwindow.MenuChoice = (string)button.Tag;

                if ((string)button.Tag == "4" || (string)button.Tag == "3")
                {
                    _mainwindow.Paused = true;
                    _mainwindow.PlayButton.Visibility = Visibility.Visible;
                    _mainwindow.PauseButton.Visibility = Visibility.Collapsed;
                    _mainwindow.Process = "3";
                }
            }
        }




        Process process;
        private void Attach(object sender, RoutedEventArgs e)
        {
            IEnumerable<Process> processes = Process.GetProcesses();
            foreach (Process P in processes)
            {
                if (P.ProcessName == "Dolphin")
                {
                    process = P;
                    _mainwindow.Attatched = true;
                }
            }
            SessionManager.Session.OpenedProcess = process;

            bool success;
            var Rdec = BinaryPrimitives.ReverseEndianness(MemoryReader.Instance.Read<UInt64>(
                                        SessionManager.Session.OpenedProcess,
                                        MemoryQueryer.Instance.EmulatorAddressToRealAddress(SessionManager.Session.OpenedProcess, 0x80000000, EmulatorType.Dolphin),
                                        out success));

            var RBytes = BitConverter.GetBytes(Rdec);
            RBytes = RBytes.Reverse().ToArray();
            var RByte = RBytes.Take(6).ToArray();
            var Region = Encoding.UTF8.GetString(RByte).Trim('\0');
            Debug.WriteLine(Region);

            RRegion.Text = "        "+Region;

        }

    }
}
