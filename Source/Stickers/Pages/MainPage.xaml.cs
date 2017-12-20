﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
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
        }

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

        #region Methods

        /// <summary>
        ///     Attach the data requested event handler and load the stickers
        /// </summary>
        /// <param name="e">the navigation event arguments</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            DataTransferManager.GetForCurrentView().DataRequested += MainPage_DataRequested;

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

            DataTransferManager.GetForCurrentView().DataRequested -= MainPage_DataRequested;

            base.OnNavigatedFrom(e);
        }

        #endregion

        #region Implementation

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
            dataPackage.SetStorageItems(new[] {sticker.File});

            var stream = await sticker.File.OpenReadAsync();

            var streamRef = RandomAccessStreamReference.CreateFromStream(stream);

            dataPackage.SetBitmap(streamRef);

            dataPackage.Properties.Title = sticker.Name;
            dataPackage.Properties.Description = "Sticker";
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

        #endregion
    }
}