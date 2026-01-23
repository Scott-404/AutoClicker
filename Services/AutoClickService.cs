using SeroAutoClicker.Models;
using SeroAutoClicker.Utilities;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SeroAutoClicker.Services
{
    // Runs the click loop and handles start/stop state
    public class AutoClickService
    {
        private CancellationTokenSource? _cts;
        private bool _isRunning;
        private readonly object _lock = new();

        public bool IsRunning
        {
            get { lock (_lock) return _isRunning; }
            private set { lock (_lock) _isRunning = value; }
        }

        public async Task ToggleClickingAsync(ClickSettings settings)
        {
            if (IsRunning)
            {
                StopClicking();
                return;
            }

            IsRunning = true;
            _cts = new CancellationTokenSource();

            await Task.Run(() =>
            {
                try
                {
                    PerformClicks(settings, _cts.Token);
                }
                catch (OperationCanceledException)
                {
                    // Normal shutdown path
                }
                finally
                {
                    IsRunning = false;
                }
            }, _cts.Token);
        }

        private void PerformClicks(ClickSettings settings, CancellationToken token)
        {
            int count = 0;
            Stopwatch stopwatch = new Stopwatch();

            // Loop until cancelled, or until repeat count is reached (if enabled)
            while (!token.IsCancellationRequested &&
                   (settings.IsRepeatEnabled ? count < settings.RepeatCount : true))
            {
                stopwatch.Restart();

                PerformClick(settings.ClickType);
                count++;

                // Interval is stored as decimal ms; convert to microsecond timing for tighter control
                decimal intervalMs = settings.ClickInterval;
                if (intervalMs > 0)
                {
                    double intervalMicroseconds = (double)(intervalMs * 1000);
                    double elapsedMicroseconds = stopwatch.ElapsedTicks * 1000000 / Stopwatch.Frequency;
                    double remainingMicroseconds = intervalMicroseconds - elapsedMicroseconds;

                    if (remainingMicroseconds > 0)
                    {
                        if (remainingMicroseconds > 16000) // coarse sleep
                        {
                            Thread.Sleep((int)(remainingMicroseconds / 1000));
                        }
                        else if (remainingMicroseconds > 1000) // small sleep
                        {
                            Thread.Sleep(1);
                        }
                        else // very small delay: spin to avoid oversleeping
                        {
                            int spinCount = (int)(remainingMicroseconds * 100); // rough calibration
                            if (spinCount > 0)
                            {
                                Thread.SpinWait(spinCount);
                            }
                        }
                    }
                }
            }
        }

        private void PerformClick(ClickType clickType)
        {
            switch (clickType)
            {
                case ClickType.LeftClick:
                    MouseSimulator.LeftClick();
                    break;
                case ClickType.RightClick:
                    MouseSimulator.RightClick();
                    break;
                case ClickType.DoubleClick:
                    MouseSimulator.DoubleClick();
                    break;
            }
        }

        public void StopClicking()
        {
            _cts?.Cancel();
            IsRunning = false;
        }
    }
}
