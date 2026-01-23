using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace SeroAutoClicker
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Register hotkey + hook into the window message loop
            var hwnd = new WindowInteropHelper(this).Handle;

            if (DataContext is ViewModels.MainViewModel viewModel)
            {
                viewModel.InitializeHotkey(hwnd);

                var source = HwndSource.FromHwnd(hwnd);
                source?.AddHook(HwndHook);
            }
        }

        private void Window_Closed(object? sender, EventArgs e)
        {
            // Clean shutdown for services/hotkeys/background tasks
            if (DataContext is ViewModels.MainViewModel viewModel)
            {
                viewModel.Dispose();
            }
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;

            // WM_HOTKEY is delivered via the Win32 message pump
            if (msg == WM_HOTKEY)
            {
                if (DataContext is ViewModels.MainViewModel viewModel)
                {
                    viewModel.ProcessHotkey(wParam.ToInt32());
                    handled = true;
                }
            }

            return IntPtr.Zero;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
