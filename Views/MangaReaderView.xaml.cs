using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MangaMate.Views
{
    /// <summary>
    /// Interaction logic for MangaReaderView.xaml
    /// </summary>
    public partial class MangaReaderView : UserControl
    {
        public MangaReaderView()
        {
            InitializeComponent();

            DataContextChanged += MangaReaderView_DataContextChanged;
        }

        private void MangaReaderView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is INotifyPropertyChanged oldVM)
                oldVM.PropertyChanged -= OnViewModelPropertyChanged;

            if (e.NewValue is INotifyPropertyChanged newVM)
                newVM.PropertyChanged += OnViewModelPropertyChanged;
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PageIndex")
            {
                PageScrollViewer?.ScrollToTop();
            }
        }
    }
}
