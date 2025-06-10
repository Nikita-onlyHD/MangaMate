using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MangaMate
{
    public sealed class ByteToImageConverter : IValueConverter
    {
        public object Convert(object value, Type t, object p, CultureInfo c)
        {
            if (value is not byte[] bytes || bytes.Length == 0) return null!;
            using var ms = new MemoryStream(bytes);
            var img = new BitmapImage();
            img.BeginInit(); img.StreamSource = ms; img.CacheOption = BitmapCacheOption.OnLoad; img.EndInit(); img.Freeze();
            return img;
        }
        public object ConvertBack(object v, Type t, object p, CultureInfo c) => Binding.DoNothing;
    }
}
