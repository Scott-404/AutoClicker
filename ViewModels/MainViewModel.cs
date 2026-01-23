using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SeroAutoClicker.Models;
using SeroAutoClicker.Services;
using SeroAutoClicker.Utilities;
using System;
using System.Windows;
using System.Windows.Input;
using System.Threading.Tasks;

namespace SeroAutoClicker.ViewModels
{
    // Main UI view model coordinating input, status, and services
    public partial class MainViewModel : ObservableObject, IDisposable
    {
        private readonly AutoClickService _autoClickService;
        private GlobalHotkeyService? _hotkeyService;
        private bool _disposed;
        private System.Threading.CancellationTokenSource? _cursorTrackingCts;

        // User-configurable click settings
        [ObservableProperty]
        private ClickSettings _settings = new();

        // Bound to UI running state
        [ObservableProperty]
        private bool _isRunning;

        // Status text shown in the UI
        [ObservableProperty]
        private string _statusMessage = "Ready - Press F6 to toggle";

        // Live cursor position display
        [ObservableProperty]
        private string _cursorPosition = "X: 0, Y: 0";

        public MainViewModel()
        {
            _autoClickService = new AutoClickService();
            StartCursorTracking();
        }

        [RelayCommand]
        private async Task ToggleClicking()
        {
            try
            {
                await _autoClickService.ToggleClickingAsync(Settings);
                IsRunning = _autoClickService.IsRunning;

                // Status reflects both state and click speed
                if (Settings.ClickInterval == 0)
                    StatusMessage = IsRunning
                        ? $"ACTIVE - MAXIMUM SPEED (Press {Settings.ActivationKey} to stop)"
                        : $"INACTIVE - Press {Settings.ActivationKey} to start";
                else if (Settings.ClickInterval <= 10)
                    StatusMessage = IsRunning
                        ? $"ACTIVE - VERY FAST ({Settings.ClickInterval}ms) (Press {Settings.ActivationKey} to stop)"
                        : $"INACTIVE - Press {Settings.ActivationKey} to start";
                else
                    StatusMessage = IsRunning
                        ? $"ACTIVE - {Settings.ClickInterval}ms interval (Press {Settings.ActivationKey} to stop)"
                        : $"INACTIVE - Press {Settings.ActivationKey} to start";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                IsRunning = false;
                StatusMessage = "Error occurred";
            }
        }

        [RelayCommand]
        private void StopClicking()
        {
            _autoClickService.StopClicking();
            IsRunning = false;
            StatusMessage = $"Stopped - Press {Settings.ActivationKey} to start";
        }

        public void InitializeHotkey(IntPtr windowHandle)
        {
            try
            {
                _hotkeyService?.Dispose();

                // Parse user-selected activation key and register global hotkey
                var key = (Key)Enum.Parse(typeof(Key), Settings.ActivationKey);
                _hotkeyService = new GlobalHotkeyService(windowHandle, 9000, key);
                _hotkeyService.HotkeyPressed += OnHotkeyPressed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to register hotkey: {ex.Message}", "Warning",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void OnHotkeyPressed(object? sender, EventArgs e)
        {
            // Marshal back to UI thread before toggling
            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                await ToggleClicking();
            });
        }

        public void ProcessHotkey(int id)
        {
            _hotkeyService?.ProcessHotkey(id);
        }

        private async void StartCursorTracking()
        {
            _cursorTrackingCts = new System.Threading.CancellationTokenSource();

            // Background cursor polling for live UI display
            await Task.Run(async () =>
            {
                while (!_disposed && !_cursorTrackingCts!.Token.IsCancellationRequested)
                {
                    var pos = MouseSimulator.GetCursorPosition();
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        CursorPosition = $"X: {pos.X}, Y: {pos.Y}";
                    });

                    await Task.Delay(100, _cursorTrackingCts.Token);
                }
            });
        }

        public void Dispose()
        {
            // Ensure hotkeys, background tasks, and clicking are shut down cleanly
            if (!_disposed)
            {
                _hotkeyService?.Dispose();
                _autoClickService.StopClicking();
                _cursorTrackingCts?.Cancel();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}
