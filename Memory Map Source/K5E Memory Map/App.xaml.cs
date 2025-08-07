using System.Configuration;
using System.Data;
using System.Windows;
using System.Threading.Tasks;
using K5E_Memory_Map.UIModule;
using System.Diagnostics;

namespace K5E_Memory_Map
{
    public partial class App : Application
    {
        public MainLoop? MainLoop { get; private set; } // Property to access MainLoop
        private MainWindow? _mainWindow;

        public InfoLoop? InfoLoop { get; private set; } // Property to access MainLoop

        public ScanLoop? ScanLoop { get; private set; } // Property to access MainLoop

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _mainWindow = new MainWindow();
            _mainWindow.Show(); // Show the main window

            ScanLoop = new ScanLoop();
            Task.Run(() => ScanLoop.Start(_mainWindow));

            MainLoop = new MainLoop(_mainWindow,ScanLoop.Queue);
            Task.Run(() => MainLoop.Run(null));

            
            InfoLoop = new InfoLoop(_mainWindow);
            Task.Run(() => InfoLoop.Run());

           

            Debug.WriteLine("affafg");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            MainLoop?.Stop(); // Stop the main loop on exit
            InfoLoop?.Stop();
            base.OnExit(e);
        }
    }
}
