﻿using System;
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

namespace K5E_Memory_Map.UIModule
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : UserControl
    {
        public MainMenu()
        {
            InitializeComponent();
            
        }

        public DisplayMenu _displaymenu;
        public MainWindow _mainwindow;

        private void MenuButton(object sender, RoutedEventArgs e)
        {
            // 0:Main 1:Full 2:save/load 3:Analysis 4:Praqctice 5:Merge
            if (sender is Button button)
            {
                _displaymenu.MenuChoice = (string)button.Tag;
                _mainwindow.MenuChoice = (string)button.Tag;
            }
        }
    }
}