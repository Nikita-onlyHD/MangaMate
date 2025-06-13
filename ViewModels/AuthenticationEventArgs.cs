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
