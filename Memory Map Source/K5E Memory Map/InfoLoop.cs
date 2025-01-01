using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.MemoryMappedFiles;
using K5E_Memory_Map.UIModule;
using System.IO;

namespace K5E_Memory_Map
{
    public class InfoLoop
    {
        
        private CancellationTokenSource _cancellationTokenSource;

        public InfoLoop(MainWindow _MainWindow)
        {

            //_MemDisplay = memDisplay;
            _cancellationTokenSource = new CancellationTokenSource();

            _MainWindow.GetMemDisplay(this);
            
        }

        public MemDisplay _MemDisplay;

        private static string? GetFoxCoords()
        {
            string mapName = "FoxMemory";

            try
            {
                using (var mmf = MemoryMappedFile.OpenExisting(mapName))
                {
                    using (var accessor = mmf.CreateViewAccessor(0, 1024, MemoryMappedFileAccess.Read)) // Adjust size as necessary
                    {
                        byte[] buffer = new byte[1024]; // Buffer size must match what you wrote
                        accessor.ReadArray(0, buffer, 0, buffer.Length);
                        string memText = Encoding.UTF8.GetString(buffer).Trim('\0'); // Convert to string and trim null characters
                        if (memText == "")
                        {
                            return null;
                        }
                        return memText;
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("Could not open memory-mapped file for MemText: " + ex.Message);
                return null;
            }


        }

        private static string? GetMountESWCoords()
        {
            string mapName = "BikeESWMemory";

            try
            {
                using (var mmf = MemoryMappedFile.OpenExisting(mapName))
                {
                    using (var accessor = mmf.CreateViewAccessor(0, 1024, MemoryMappedFileAccess.Read)) // Adjust size as necessary
                    {
                        byte[] buffer = new byte[1024]; // Buffer size must match what you wrote
                        accessor.ReadArray(0, buffer, 0, buffer.Length);
                        string memText = Encoding.UTF8.GetString(buffer).Trim('\0'); // Convert to string and trim null characters
                        if (memText == "")
                        {
                            return null;
                        }
                        return memText;
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("Could not open memory-mapped file for MemText: " + ex.Message);
                return null;
            }


        }

        public void Run()
        {
            var token = _cancellationTokenSource.Token;

            while (!token.IsCancellationRequested)
            {
                string Fox = GetFoxCoords();
                string MountESW = GetMountESWCoords();
                _MemDisplay.FoxCoords = Fox;
                _MemDisplay.MountESWCoords = MountESW;
                

                _MemDisplay.Dispatcher.Invoke(() =>
                {
                    _MemDisplay.UpdateLoop();
                });

                Thread.Sleep(10);

            }

        }

            public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
