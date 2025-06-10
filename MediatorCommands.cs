using MangaMate.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MangaMate
{
    public static class MediatorCommands
    {
        public static ICommand OpenCatalog { get; } =
            new Command(_ => Mediator.Instance.Notify("OpenCatalogManga"));
    }
}
