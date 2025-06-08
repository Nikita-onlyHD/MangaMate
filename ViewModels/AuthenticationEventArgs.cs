using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaMate.ViewModels
{
    internal class AuthenticationEventArgs : EventArgs
    {
        public bool IsSuccessful {  get; init; }

        public AuthenticationEventArgs(bool isSuccessful)
        {
            IsSuccessful = isSuccessful;
        }
    }
}
