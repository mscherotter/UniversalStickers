using System;
using System.Collections.Generic;
using System.Globalization;
using Windows.ApplicationModel.AppExtensions;

namespace StickersApp.Core
{
    /// <summary>
    ///     A sticker extension
    /// </summary>
    public class StickerExtension
    {
        /// <summary>
        ///     Gets the list of stickers
        /// </summary>
        public List<Sticker> Stickers { get; set; } = new List<Sticker>();

        public string PublicFolder { get; set; }

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

        public string Id { get; set; }
    }
}