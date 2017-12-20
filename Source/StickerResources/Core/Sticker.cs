using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace StickerResources.Core
{
    public sealed class Sticker
    {
        public IEnumerable<string> Keywords { get; private set; }
        public string Name { get; internal set; }

        public StorageFile File { get; internal set; }

        public ImageSource Thumbnail { get; internal set; }

        public StickerExtension Extension { get; set; }

        public static IAsyncOperation<Sticker> CreateAsync(StorageFile file)
        {
            return AsyncInfo.Run(async delegate(CancellationToken token)
            {
                var sticker = new Sticker
                {
                    File = file
                };

                var imageProperties = await file.Properties.GetImagePropertiesAsync();

                sticker.Keywords = new List<string>(imageProperties.Keywords);

                if (token.IsCancellationRequested) return null;

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