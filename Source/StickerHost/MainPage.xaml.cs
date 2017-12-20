using StickerResources;

namespace StickerHost
{
    /// <summary>
    ///     An example page that hosts a <see cref="StickersControl" />
    /// </summary>
    public sealed partial class MainPage
    {
        /// <summary>
        ///     Initializes a new instance of the MainPage class.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Demonstration of an event Handler for the StickerSelected event.
        /// </summary>
        /// <param name="sender">the <see cref="StickersControl" /></param>
        /// <param name="e">the sticker selected event arguments</param>
        private void OnStickerSelected(object sender, StickerSelectedEventArgs e)
        {
            Status.Text = e.SelectedSticker == null ? string.Empty : e.SelectedSticker.Name;
        }
    }
}