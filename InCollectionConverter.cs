using System.Globalization;
using System.Windows.Data;

namespace MangaMate
{
    public sealed class InCollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is System.Collections.IEnumerable collection)
                return collection.Cast<object>().Contains(value);
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Usually used in multiselect scenarios like CheckBox → ObservableCollection
            return Binding.DoNothing;
        }
    }
}
