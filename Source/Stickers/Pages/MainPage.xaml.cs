using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.UI.Animations;
using StickersApp.Common;
using StickersApp.Core;

namespace StickersApp.Pages
{
    /// <summary>
    ///     Main page that shows the stickers in the shared folder and allows users to drag stickers to other apps, supports
    ///     clipboard copy, and share.
    /// </summary>
    public sealed partial class MainPage
    {
        private readonly NavigationHelper _navigationHelper;

        /// <summary>
        ///     Initializes a new instance of the MainPage class.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += _navigationHelper_LoadState;
            _navigationHelper.SaveState += _navigationHelper_SaveState;

            // ReSharper disable once UseNameofExpression
            if (ApiInformation.IsEnumNamedValuePresent("Windows.UI.Xaml.Controls.Symbol", "Share"))
                ShareButton.Icon = new SymbolIcon(Symbol.Share);

            if (ReorderGridAnimation.IsSupported)
                ReorderGridAnimation.SetDuration(StickerGridView, 300);

            if (!DataTransferManager.IsSupported() || AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Xbox")
                ShareButton.Visibility = Visibility.Collapsed;
        }

        #region Methods

        /// <summary>
        ///     Attach the data requested event handler and load the stickers
        /// </summary>
        /// <param name="e">the navigation event arguments</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var stickers = await StickerDataSource.GetStickersAsync();

            FilesSource.Source = stickers;

            _navigationHelper.OnNavigatedTo(e);
        }

        /// <summary>
        ///     Detach the data requested event handler
        /// </summary>
        /// <param name="e">the navigation event arguments</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedFrom(e);

            base.OnNavigatedFrom(e);
        }

        #endregion

        #region Implementation

        private void _navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            e.PageState["SelectedIndex"] = StickerGridView.SelectedIndex;
        }

        private void _navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if (e.PageState != null)
            {
                var index = (int) e.PageState["SelectedIndex"];

                StickerGridView.SelectedIndex = index;

                var selectedItem = StickerGridView.SelectedItem;

                if (selectedItem != null)
                    StickerGridView.ScrollIntoView(selectedItem);
            }
        }

        private async void MainPage_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            if (StickerGridView.SelectedItem is Sticker sticker)
            {
                var deferral = args.Request.GetDeferral();

                await UpdateDataPackageAsync(args.Request.Data, sticker);

                deferral.Complete();
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var enable = StickerGridView.SelectedItem is Sticker;

            CopyButton.IsEnabled = enable;
            ShareButton.IsEnabled = enable;
        }

        private async void OnCopy(object sender, RoutedEventArgs e)
        {
            if (StickerGridView.SelectedItem is Sticker sticker)
            {
                var dataPackage = new DataPackage();

                await UpdateDataPackageAsync(dataPackage, sticker);

                Clipboard.SetContent(dataPackage);
            }
        }

        private async Task UpdateDataPackageAsync(DataPackage dataPackage, Sticker sticker)
        {
            var list = new List<IStorageItem>
            {
                sticker.File
            };

            dataPackage.SetStorageItems(list);

            var stream = await sticker.File.OpenReadAsync();

            var streamRef = RandomAccessStreamReference.CreateFromStream(stream);

            dataPackage.SetBitmap(streamRef);

            dataPackage.Properties.Title = sticker.Name;

            dataPackage.Properties.ApplicationName = Package.Current.DisplayName;

            dataPackage.Properties.PackageFamilyName = Package.Current.Id.FamilyName;

            var resources = ResourceLoader.GetForCurrentView();

            dataPackage.Properties.Description = resources.GetString("Sticker");
        }

        private void OnShare(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }

        private async void OnDragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            var sticker = e.Items.OfType<Sticker>().FirstOrDefault();

            if (sticker != null)
                await UpdateDataPackageAsync(e.Data, sticker);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            DataTransferManager.GetForCurrentView().DataRequested += MainPage_DataRequested;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            DataTransferManager.GetForCurrentView().DataRequested -= MainPage_DataRequested;
        }

        #endregion
    }
}