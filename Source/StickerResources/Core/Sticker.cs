using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace StickerResources.Core
{
    /// <summary>
    /// A sticker
    /// </summary>
    public sealed class Sticker
    {
        /// <summary>
        /// Gets the keywords
        /// </summary>
        public IEnumerable<string> Keywords { get; private set; }

        /// <summary>
        /// Gets the name of the sticker
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the file for the sticker
        /// </summary>
        public StorageFile File { get; internal set; }

        /// <summary>
        /// Gets the thumbnail image for the sticker
        /// </summary>
        public ImageSource Thumbnail { get; internal set; }

        /// <summary>
        /// Gets or sets the sticker extensions
        /// </summary>
        public StickerExtension Extension { get; set; }

        /// <summary>
        /// Create a sticker based on a file
        /// </summary>
        /// <param name="file">the image file</param>
        /// <param name="keywords">the keywords</param>
        /// <returns>an async operation with the sticker</returns>
        public static IAsyncOperation<Sticker> CreateAsync(StorageFile file, IReadOnlyList<string> keywords)
        {
            return AsyncInfo.Run(async delegate(CancellationToken token)
            {
                var sticker = new Sticker
                {
                    File = file
                };

                var imageProperties = await file.Properties.GetImagePropertiesAsync();

                if (token.IsCancellationRequested) return null;

                var keywordList = new List<string>(from item in imageProperties.Keywords
                                                   select item.ToLower());

                if (keywords != null)
                {
                    foreach (var keyword in keywords)
                    {
                        if (keywordList.Exists(key => string.Equals(key.Trim(), keyword.Trim(), StringComparison.CurrentCultureIgnoreCase)))
                        {
                            continue;
                        }

                        keywordList.Add(keyword.Trim().ToLower());
                    }
                }

                sticker.Keywords = keywordList;

                sticker.Name = string.IsNullOrWhiteSpace(imageProperties.Title)
                    ? file.DisplayName
                    : imageProperties.Title;

                var thumbnail = await file.GetScaledImageAsThumbnailAsync(ThumbnailMode.SingleItem, 200);

                if (token.IsCancellationRequested) return null;

                var image = new BitmapImage();

                await image.SetSourceAsync(thumbnail);

                if (token.IsCancellationRequested) return null;

                sticker.Thumbnail = image;

                return sticker;
            });
        }
    }
}
