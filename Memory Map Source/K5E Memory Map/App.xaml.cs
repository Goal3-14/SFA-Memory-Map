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

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _mainWindow = new MainWindow();
            _mainWindow.Show(); // Show the main window
            MainLoop = new MainLoop(_mainWindow);
            Task.Run(() => MainLoop.Run(null));
            Debug.WriteLine("affafg");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            MainLoop?.Stop(); // Stop the main loop on exit
            base.OnExit(e);
        }
    }
}
