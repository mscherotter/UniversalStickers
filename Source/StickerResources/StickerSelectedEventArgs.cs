using StickerResources.Core;

namespace StickerResources
{
    /// <summary>
    /// Sticker selected event arguments
    /// </summary>
    public sealed class StickerSelectedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the StickerSelectedEventArgs class.
        /// </summary>
        /// <param name="sticker"></param>
        public StickerSelectedEventArgs(Sticker sticker)
        {
            SelectedSticker = sticker;
        }

        /// <summary>
        /// Gets the selected sticker
        /// </summary>
        public Sticker SelectedSticker { get; }
    }
}