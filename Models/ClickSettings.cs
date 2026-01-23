using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SeroAutoClicker.Models
{
    // Holds user-configurable click behaviour and notifies the UI of changes
    public class ClickSettings : INotifyPropertyChanged
    {
        private decimal _clickInterval = 1.0m; // decimal allows sub-millisecond precision
        private ClickType _clickType = ClickType.LeftClick;
        private bool _isRepeatEnabled = true;
        private int _repeatCount = 1000;
        private string _activationKey = "F6";

        public decimal ClickInterval
        {
            get => _clickInterval;
            set { _clickInterval = value; OnPropertyChanged(); }
        }

        public ClickType ClickType
        {
            get => _clickType;
            set { _clickType = value; OnPropertyChanged(); }
        }

        public bool IsRepeatEnabled
        {
            get => _isRepeatEnabled;
            set { _isRepeatEnabled = value; OnPropertyChanged(); }
        }

        public int RepeatCount
        {
            get => _repeatCount;
            set { _repeatCount = value; OnPropertyChanged(); }
        }

        public string ActivationKey
        {
            get => _activationKey;
            set { _activationKey = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        // Centralised property change notification for MVVM binding
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Supported mouse click modes
    public enum ClickType
    {
        LeftClick,
        RightClick,
        DoubleClick
    }
}
