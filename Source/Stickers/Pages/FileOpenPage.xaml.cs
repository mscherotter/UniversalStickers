using System;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.Storage.Pickers.Provider;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using StickersApp.Core;

namespace StickersApp.Pages
{
    /// <summary>
    ///     File Open page for selecting a sticker in another app
    /// </summary>
    public sealed partial class FileOpenPage
    {
        private readonly FileOpenPickerUI _pickerUi;

        public FileOpenPage(FileOpenPickerUI pickerUi)
        {
            _pickerUi = pickerUi;

            InitializeComponent();

            Loaded += FileOpenPage_Loaded;
        }

        private async void FileOpenPage_Loaded(object sender, RoutedEventArgs e)
        {
            var stickerExtensions = await StickerDataSource.GetStickersAsync();

            if (!_pickerUi.AllowedFileTypes.Contains("*"))
            {
                var filteredList = stickerExtensions.ToList();

                foreach (var extension in filteredList)
                {
                    var filteredStickers = (from sticker in extension.Stickers
                        where _pickerUi.AllowedFileTypes.Contains(sticker.File.FileType.ToLowerInvariant())
                        select sticker).ToList();

                    extension.Stickers = filteredStickers;
                }

                stickerExtensions = filteredList;
            }

            var resources = ResourceLoader.GetForCurrentView();

            _pickerUi.Title = resources.GetString("ExtensionDisplayName");

            FileGridView.SelectionMode = _pickerUi.SelectionMode == FileSelectionMode.Single
                ? ListViewSelectionMode.Single
                : ListViewSelectionMode.Multiple;

            FileGridView.IsMultiSelectCheckBoxEnabled = _pickerUi.SelectionMode == FileSelectionMode.Multiple;

            ExtensionsViewSource.Source = stickerExtensions;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null)
            {
                var addedStickers = e.AddedItems.OfType<Sticker>();

                foreach (var item in addedStickers)
                    if (_pickerUi.CanAddFile(item.File))
                        _pickerUi.AddFile(item.File.Path, item.File);
            }

            if (e.RemovedItems != null)
            {
                var removedItems = e.RemovedItems.OfType<Sticker>();

                foreach (var item in removedItems)
                    if (_pickerUi.ContainsFile(item.File.Path))
                        _pickerUi.RemoveFile(item.File.Path);
            }
        }
    }
}