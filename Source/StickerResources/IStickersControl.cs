using StickerResources.Core;

namespace StickerResources
{
    /// <summary>
    /// Stickers control interface
    /// </summary>
    public interface IStickersControl
    {
        /// <summary>
        /// Gets or sets the width of the control
        /// </summary>
        double Width { get; set; }

        /// <summary>
        /// Gets or sets the selected sticker
        /// </summary>
        Sticker SelectedSticker { get; set; }
    }
}