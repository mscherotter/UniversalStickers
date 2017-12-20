using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace StickersApp.Core
{
    public class ProvideFeedbackCommand : ICommand
    {
        private bool _isBusy;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return !_isBusy && Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.IsSupported();
        }

        public async void Execute(object parameter)
        {
            _isBusy = true;

            CanExecuteChanged?.Invoke(this, new EventArgs());

            var launcher = Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.GetDefault();

            var dictionary = new Dictionary<string, string>
            {
                ["ExtensionId"] = parameter.ToString()
            };

            await launcher.LaunchAsync(dictionary);

            _isBusy = false;

            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }
}