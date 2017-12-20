using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace StickersApp.Core
{
    /// <summary>
    ///     A sticker
    /// </summary>
    public class Sticker
    {
        private const uint ThumbnailSize = 376;

        /// <summary>
        ///     Gets the name of the sticker
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     Gets the file for the sticker
        /// </summary>
        public StorageFile File { get; private set; }

        /// <summary>
        ///     Gets the thumbnail for the sticker
        /// </summary>
        public ImageSource Thumbnail { get; private set; }

        /// <summary>
        ///     Creates a sticker from an image file
        /// </summary>
        /// <param name="file">the file</param>
        /// <returns>an async operation with a new sticker</returns>
        public static IAsyncOperation<Sticker> CreateAsync(StorageFile file)
        {
            return AsyncInfo.Run(async delegate(CancellationToken token)
            {
                if (file == null) throw new ArgumentNullException(nameof(file), "file cannot be null.");

                var sticker = new Sticker
                {
                    File = file
                };

                var imageProperties = await file.Properties.GetImagePropertiesAsync();

                if (token.IsCancellationRequested) return null;

                sticker.Name = string.IsNullOrWhiteSpace(imageProperties.Title)
                    ? file.DisplayName
                    : imageProperties.Title;

                var thumbnail = await file.GetThumbnailAsync(ThumbnailMode.SingleItem, ThumbnailSize);

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