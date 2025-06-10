using System.Globalization;
using System.Windows.Data;

namespace MangaMate
{
    public sealed class OneBasedConverter : IValueConverter
    {
        public object Convert(object v, Type t, object p, CultureInfo c) => (int)v + 1;
        public object ConvertBack(object v, Type t, object p, CultureInfo c) => Binding.DoNothing;
    }
}
