using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaMate.ViewModels.Mediator
{
    public interface IEventReceiver
    {
        void HandleEvent();
    }
}
