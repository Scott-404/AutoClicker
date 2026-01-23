using System;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace SeroAutoClicker.Utilities
{
    // Wraps Win32 global hotkey registration for the app
    public class GlobalHotkeyService : IDisposable
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private readonly IntPtr _handle;
        private readonly int _hotkeyId;
        private bool _disposed;

        public event EventHandler? HotkeyPressed;

        public GlobalHotkeyService(IntPtr windowHandle, int hotkeyId, Key key)
        {
            _handle = windowHandle;
            _hotkeyId = hotkeyId;

            // Translate WPF key to Win32 virtual key
            uint virtualKey = (uint)KeyInterop.VirtualKeyFromKey(key);
            RegisterHotKey(_handle, _hotkeyId, 0, virtualKey);
        }

        public void ProcessHotkey(int id)
        {
            // Only react to the registered hotkey ID
            if (id == _hotkeyId)
            {
                HotkeyPressed?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Dispose()
        {
            // Ensure hotkey is released when shutting down
            if (!_disposed)
            {
                UnregisterHotKey(_handle, _hotkeyId);
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}
