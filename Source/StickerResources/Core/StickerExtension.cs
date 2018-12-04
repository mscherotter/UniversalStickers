using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Windows.ApplicationModel.AppExtensions;

namespace StickerResources.Core
{
    /// <summary>
    ///     A sticker extension
    /// </summary>
    public sealed class StickerExtension
    {
        /// <summary>
        ///     Gets the list of stickers
        /// </summary>
        public IList<Sticker> Stickers { get; } = new ObservableCollection<Sticker>();

        /// <summary>
        ///     Gets or sets the name of the sticker collection
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the description of the sticker collection
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets the author of the sticker collection
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        ///     Gets or sets a web link to the author of the sticker collection
        /// </summary>
        public Uri AuthorLink { get; set; }

        /// <summary>
        ///     Gets or sets the app extension
        /// </summary>
        public AppExtension Extension { get; internal set; }

        /// <summary>
        ///     Gets the rating Uri for an extension
        /// </summary>
        public Uri RatingUri
        {
            get
            {
                if (Extension == null) return null;

                var uriString = string.Format(
                    CultureInfo.InvariantCulture,
                    "ms-windows-store:Review?PFN={0}",
                    Extension.Package.Id.FamilyName);

                return new Uri(uriString);
            }
        }

        /// <summary>
        /// Gets or sets the license
        /// </summary>
        public string License { get; set; }

        /// <summary>
        /// Gets or sets the license URL
        /// </summary>
        public Uri LicenseUri { get; set; }
    }
}