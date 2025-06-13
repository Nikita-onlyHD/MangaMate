using MangaMate.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace MangaMate.Views
{
    /// <summary>
    /// Interaction logic for MangaChaptersView.xaml
    /// </summary>
    public partial class MangaChaptersView : UserControl
    {
        public MangaChaptersView()
        {
            InitializeComponent();
        }

        private void ChapterItem_DblClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MangaChaptersViewModel vm &&
                (sender as ListViewItem)?.Content is int chapter)
                vm.OpenReaderCommand.Execute(chapter);
        }
    }
}
