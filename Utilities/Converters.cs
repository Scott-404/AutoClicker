using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SeroAutoClicker.Utilities
{
    // WPF binding converters (kept simple + UI-focused)
    public class BoolToColorConverter : IValueConverter
    {
        private static readonly Brush ActiveBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x12, 0x3A, 0x52));   // deep cyan-blue
        private static readonly Brush InactiveBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x73, 0x2A, 0x73)); // deep magenta-purple

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool b && b) ? ActiveBrush : InactiveBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // One-way only
            throw new NotImplementedException();
        }
    }

    public class BoolToStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool b && b) ? "ACTIVE" : "INACTIVE";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // One-way only
            throw new NotImplementedException();
        }
    }

    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool b) ? !b : false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // One-way only
            throw new NotImplementedException();
        }
    }

    public class EnumToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Used for enum-backed radio buttons / toggles
            return value?.ToString() == parameter?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Only update source when selected
            return (value is bool b && b) ? parameter : Binding.DoNothing;
        }
    }
}