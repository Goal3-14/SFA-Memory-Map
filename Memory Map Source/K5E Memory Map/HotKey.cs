using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows;

namespace K5E_Memory_Map
{
    public partial class HotKey : Window
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        private const int INPUT_KEYBOARD = 1;
        private const ushort KEYEVENTF_KEYDOWN = 0x0000; // Key down flag
        private const ushort KEYEVENTF_KEYUP = 0x0002;   // Key up flag
        private const ushort VK_F10 = 0x79;              // Virtual key code for F10

        [StructLayout(LayoutKind.Sequential)]
        struct INPUT
        {
            public int type;
            public InputUnion u;
        }

        [StructLayout(LayoutKind.Explicit)]
        struct InputUnion
        {
            [FieldOffset(0)]
            public KEYBDINPUT ki;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        public HotKey()
        {
            //InitializeComponent();
        }

        public void SimulateF10Press()
        {
            INPUT[] inputs = new INPUT[2];

            // Press F10
            inputs[0] = new INPUT
            {
                type = INPUT_KEYBOARD,
                u = new InputUnion
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = VK_F10,
                        dwFlags = KEYEVENTF_KEYDOWN
                    }
                }
            };

            // Release F10
            inputs[1] = new INPUT
            {
                type = INPUT_KEYBOARD,
                u = new InputUnion
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = VK_F10,
                        dwFlags = KEYEVENTF_KEYUP
                    }
                }
            };

            // Send the inputs
            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        public void OnActivateF10Click(object sender, RoutedEventArgs e)
        {
            SimulateF10Press();
        }
    }
}
