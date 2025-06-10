using MangaMate.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MangaMate.ViewModels
{
    public class MangaDetailsViewModel : ViewModelBase
    {
        private Book _manga;
        public Book Manga
        {
            get => _manga;
            set
            {
                _manga = value;
                OnPropertyChanged(nameof(Manga));
            }
        }

        public ICommand BackCommand { get; }

        public MangaDetailsViewModel(Book manga)
        {
            _manga = manga;

            BackCommand = new Command(ExecuteBackCommand);

            BackCommand = new Command(_ => Mediator.Instance.Notify("BackToCatalog"));
        }

        private void ExecuteBackCommand(object? obj)
        {
            Mediator.Instance.Notify(nameof(MainViewModel.ShowCatalogMangasViewCommand));
        }
    }
}
