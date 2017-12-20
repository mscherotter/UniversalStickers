using System;
using System.Windows.Input;
using Windows.System;

namespace StickerResources.Core
{
    internal class LaunchUriCommand : ICommand
    {
        private bool _isBusy;
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return !_isBusy && parameter is Uri;
        }

        public async void Execute(object parameter)
        {
            _isBusy = true;
            CanExecuteChanged?.Invoke(this, new EventArgs());

            var uri = parameter as Uri;

            await Launcher.LaunchUriAsync(uri);

            _isBusy = false;
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }
}