using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppExtensions;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using StickerResources.Core;

namespace StickerResources
{
    /// <summary>
    ///     Sticker selector control
    /// </summary>
    [TemplatePart(Name = "Selector", Type = typeof(GridView))]
    [TemplatePart(Name = "TagsComboBox", Type = typeof(ComboBox))]
    [TemplatePart(Name = "StickerExtensionSource", Type = typeof(CollectionViewSource))]
    [TemplatePart(Name = "ProgressRing", Type = typeof(ProgressRing))]
    [TemplatePart(Name = "NoCollectionsText", Type = typeof(TextBlock))]
    [TemplatePart(Name = "HostingErrorText", Type = typeof(TextBlock))]
    [TemplateVisualState(GroupName = "StatusStates", Name = "Default")]
    [TemplateVisualState(GroupName = "StatusStates", Name = "Busy")]
    [TemplateVisualState(GroupName = "StatusStates", Name = "NoCollections")]
    public sealed class StickersControl : Control, IStickersControl
    {
        private readonly ObservableCollection<StickerExtension> _filteredExtensions =
            new ObservableCollection<StickerExtension>();

        private readonly ObservableCollection<string> _keywords = new ObservableCollection<string>();
        private AppExtensionCatalog _catalog;
        private List<StickerExtension> _extensions = new List<StickerExtension>();
        private Selector _selector;
        private ComboBox _tagsComboBox;
        private CollectionViewSource _viewSource;
        private TextBlock _hostingErrorText;

        /// <summary>
        /// Initializes a new instance of the StickersControl class.
        /// </summary>
        public StickersControl()
        {
            DefaultStyleKey = typeof(StickersControl);

            Loaded += StickersControl_Loaded;
        }

        public static bool IsAvailable => ApiInformation.IsTypePresent(
            "Windows.ApplicationModel.AppExtensions.AppExtensionCatalog");

        public string ContractName
        {
            get => (string) GetValue(ContractNameProperty);
            set => SetValue(ContractNameProperty, value);
        }

        public static DependencyProperty ContractNameProperty { get; } = DependencyProperty.Register(
            "ContractName",
            typeof(string),
            typeof(StickersControl),
            new PropertyMetadata("Universal.Stickers.1", OnContractNameChanged));

        public static DependencyProperty SelectedStickerProperty { get; } =
            DependencyProperty.Register("SelectedSticker", typeof(Sticker), typeof(StickersControl),
                new PropertyMetadata(null, OnSelectedStickerChanged));

        public Style SelectorStyle
        {
            get => (Style) GetValue(SelectorStyleProperty);
            set => SetValue(SelectorStyleProperty, value);
        }

        public static DependencyProperty SelectorStyleProperty { get; } = DependencyProperty.Register("SelectorStyle",
            typeof(Style), typeof(StickersControl), new PropertyMetadata(null));


        public Sticker SelectedSticker
        {
            get => (Sticker) GetValue(SelectedStickerProperty);
            set => SetValue(SelectedStickerProperty, value);
        }

        public event EventHandler<StickerSelectedEventArgs> StickerSelected;

        private async void StickersControl_Loaded(object sender, RoutedEventArgs e)
        {
            await UpdateStickersAsync();
        }

        protected override void OnApplyTemplate()
        {
            if (_selector != null)
                _selector.SelectionChanged -= _selector_SelectionChanged;

            if (_tagsComboBox != null)
                _tagsComboBox.SelectionChanged -= _tagsComboBox_SelectionChanged;

            base.OnApplyTemplate();
            _tagsComboBox = GetTemplateChild("TagsComboBox") as ComboBox;
            _selector = GetTemplateChild("Selector") as Selector;
            _viewSource = GetTemplateChild("StickerExtensionSource") as CollectionViewSource;
            if (_viewSource != null)
                _viewSource.Source = _filteredExtensions;

            if (_selector != null)
                _selector.SelectionChanged += _selector_SelectionChanged;

            if (_tagsComboBox != null)
            {
                _tagsComboBox.ItemsSource = _keywords;
                _tagsComboBox.SelectionChanged += _tagsComboBox_SelectionChanged;
            }

            _hostingErrorText = GetTemplateChild("HostingErrorText") as TextBlock;
        }

        private void _tagsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateFiltered();
        }

        private void _selector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_selector == null) return;

            SelectedSticker = _selector.SelectedItem as Sticker;

            StickerSelected?.Invoke(this, new StickerSelectedEventArgs(SelectedSticker));
        }

        private static async void OnContractNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is StickersControl control)
                await control.UpdateStickersAsync();
        }

        private static void OnSelectedStickerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is StickersControl control && control._selector != null)
                control._selector.SelectedItem = control.SelectedSticker;
        }

        private async Task UpdateStickersAsync()
        {
            if (_catalog != null)
            {
                _catalog.PackageInstalled -= _catalog_PackageInstalled;
                _catalog.PackageStatusChanged -= _catalog_PackageStatusChanged;
                _catalog.PackageUninstalling -= _catalog_PackageUninstalling;
                _catalog.PackageUpdated -= _catalog_PackageUpdated;
            }

            try
            {

                _catalog = AppExtensionCatalog.Open(ContractName);

                _catalog.PackageInstalled += _catalog_PackageInstalled;
                _catalog.PackageStatusChanged += _catalog_PackageStatusChanged;
                _catalog.PackageUninstalling += _catalog_PackageUninstalling;
                _catalog.PackageUpdated += _catalog_PackageUpdated;

                await LoadStickersAsync();
            }
            catch (UnauthorizedAccessException)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"StickerControl HOSTING ERROR:\nModify the application manifest to add an App Extension Host\ndeclaration with the Name '{ContractName}'.",
                    "Error");

                if (_hostingErrorText != null)
                {
                    _hostingErrorText.Visibility = Visibility.Visible;
                }
            }
        }

        private async Task LoadStickersAsync()
        {
            VisualStateManager.GoToState(this, "Busy", true);

            await LoadExtensionsAsync();

            UpdateFiltered();

            UpdateTags();
        }

        private async Task LoadExtensionsAsync()
        {
            var allExtensions = await _catalog.FindAllAsync();

            var extensions = await LoadExtensionsAsync(allExtensions);

            _extensions = extensions;
        }

        private static async Task<List<StickerExtension>> LoadExtensionsAsync(IReadOnlyList<AppExtension> allExtensions)
        {
            var extensions = (from extension in allExtensions
                orderby extension.DisplayName
                select new StickerExtension
                {
                    Name = extension.DisplayName,
                    Description = extension.Description,
                    Extension = extension
                }).ToList();

            foreach (var extension in extensions)
            {
                var properties = await extension.Extension.GetExtensionPropertiesAsync();

                if (properties.TryGetValue("Author", out object author))
                    if (author is PropertySet propertySet)
                        extension.Author = propertySet["#text"].ToString();

                if (properties.TryGetValue("AuthorLink", out object authorLink))
                    if (authorLink is PropertySet propertySet)
                        extension.AuthorLink = new Uri(propertySet["#text"].ToString());

                if (properties.TryGetValue("License", out object license))
                    if (license is PropertySet propertySet)
                    {
                        if (propertySet.TryGetValue("#text", out object licenseText))
                            extension.License = licenseText.ToString();

                        if (propertySet.TryGetValue("@Uri", out object licenseUri))
                            extension.LicenseUri = new Uri(licenseUri.ToString());
                    }

                var folder = await extension.Extension.GetPublicFolderAsync();

                var currentCulture = CultureInfo.CurrentCulture;

                var format = "_keywords.lang-{0}.txt";

                var filenames = new[]
                {
                    string.Format(
                        CultureInfo.InvariantCulture,
                        format,
                        currentCulture.Name),
                    string.Format(
                        CultureInfo.InvariantCulture,
                        format,
                        currentCulture.TwoLetterISOLanguageName),
                    "_keywords.txt"
                };

                IStorageItem keywordsFileItem = null;

                foreach (var filename in filenames)
                {
                    keywordsFileItem = await folder.TryGetItemAsync(filename);

                    if (keywordsFileItem != null)
                    {
                        break;
                    }
                }

                var keywords = new Dictionary<string, string[]>();
                
                if (keywordsFileItem != null && keywordsFileItem.IsOfType(StorageItemTypes.File))
                {
                    var lines = await FileIO.ReadLinesAsync(keywordsFileItem as StorageFile);

                    foreach (var line in lines)
                    {
                        var words = line.Split(',');

                        if (words.Length > 1)
                        {
                            keywords[words[0].Trim()] = words.Skip(1).ToArray();
                        }
                    }
                }

                var files = await folder.GetFilesAsync();

                foreach (var file in files)
                {
                    if (file.Name.StartsWith("_keywords.")) continue;

                    string[] keywordsForSticker;

                    if (keywords.TryGetValue(file.Name, out keywordsForSticker))
                    {
                        var sticker = await Sticker.CreateAsync(file, keywordsForSticker);

                        sticker.Extension = extension;

                        extension.Stickers.Add(sticker);
                    }
                    else
                    {
                        var sticker = await Sticker.CreateAsync(file, null);

                        sticker.Extension = extension;

                        extension.Stickers.Add(sticker);
                    }
                }
            }
            return extensions;
        }

        private void UpdateTags()
        {
            var keywords = from extension in _extensions
                from sticker in extension.Stickers
                from keyword in sticker.Keywords
                select keyword.ToLower();

            var list = keywords.Distinct().ToList();

            if (list.Any())
            {
                if (_tagsComboBox != null)
                {
                    list.Sort();

                    _keywords.Clear();

                    _keywords.Add("All");

                    foreach (var item in list)
                        _keywords.Add(item);

                    _tagsComboBox.SelectedIndex = 0;

                    _tagsComboBox.Visibility = Visibility.Visible;
                }
            }
            else
            {
                if (_tagsComboBox != null)
                    _tagsComboBox.Visibility = Visibility.Collapsed;
            }
        }

        private void UpdateFiltered()
        {
            _filteredExtensions.Clear();

            foreach (var extension in _extensions)
            {
                var filtered = new StickerExtension
                {
                    Name = extension.Name,
                    Description = extension.Description,
                    Extension = extension.Extension,
                    Author = extension.Author,
                    AuthorLink = extension.AuthorLink,
                    License = extension.License,
                    LicenseUri = extension.LicenseUri
                };

                foreach (var sticker in extension.Stickers)
                {
                    if (_tagsComboBox?.SelectedIndex > 0)
                    {
                        var tag = _keywords[_tagsComboBox.SelectedIndex];

                        if (!sticker.Keywords.Contains(tag))
                            continue;
                    }

                    filtered.Stickers.Add(sticker);
                }

                _filteredExtensions.Add(filtered);
            }

            var stateName = "Default";

            if (_filteredExtensions.Sum(extension => extension.Stickers.Count) == 0)
                stateName = "NoStickers";

            VisualStateManager.GoToState(this, stateName, true);
        }

        private async void _catalog_PackageUpdated(AppExtensionCatalog sender, AppExtensionPackageUpdatedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async delegate { await LoadStickersAsync(); });
        }

        private async void _catalog_PackageUninstalling(AppExtensionCatalog sender,
            AppExtensionPackageUninstallingEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, delegate
            {
                _extensions.RemoveAll(item => item.Extension.Package.Id.FamilyName == args.Package.Id.FamilyName);

                UpdateFiltered();

                UpdateTags();
            });
        }

        private async void _catalog_PackageStatusChanged(AppExtensionCatalog sender,
            AppExtensionPackageStatusChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async delegate { await LoadStickersAsync(); });
        }

        private async void _catalog_PackageInstalled(AppExtensionCatalog sender,
            AppExtensionPackageInstalledEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async delegate
            {
                VisualStateManager.GoToState(this, "Busy", true);

                var newExtensions = await LoadExtensionsAsync(args.Extensions);

                _extensions.AddRange(newExtensions);

                UpdateFiltered();

                UpdateTags();
            });
        }
    }
}