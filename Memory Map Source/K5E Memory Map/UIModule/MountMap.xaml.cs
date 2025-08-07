using System;
using System.Collections.Generic;
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
    /// Interaction logic for MountMap.xaml
    /// </summary>
    public partial class MountMap : UserControl
    {
        public MountMap()
        {
            InitializeComponent();
        }


        public float MountX=0;
        public float MountY =0;
        public float MountZ = 0;

        private Ellipse _trackingMarker;
        private const double MarkerSize = 10;

        public void UpdateMarker(double y, double x)
        {
            if (_trackingMarker == null)
            {
                _trackingMarker = new Ellipse
                {
                    Width = MarkerSize,
                    Height = MarkerSize,
                    Fill = Brushes.Red,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                OverlayCanvas.Children.Add(_trackingMarker);
            }

            // Update position (centered)
            Canvas.SetLeft(_trackingMarker, x - MarkerSize / 2);
            Canvas.SetTop(_trackingMarker, y - MarkerSize / 2);
        }

        public void Update()
        {
            UpdateMarker(0.65*(MountX-3280), -0.50*(MountZ-3890)); //x     z -3400

            //UpdateMarker(250,250);
        }
    }
}
