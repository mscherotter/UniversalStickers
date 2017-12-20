using StickerResources.Core;

namespace StickerResources
{
    public sealed class StickerSelectedEventArgs
    {
        public StickerSelectedEventArgs(Sticker sticker)
        {
            SelectedSticker = sticker;
        }

        public Sticker SelectedSticker { get; }
    }
}