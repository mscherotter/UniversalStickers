Turn your Windows UWP app into a host for universal stickers extension apps. Apps that declare the Universal.Sticker.1 App extension will expose stickers & clipart to your app in a sticker control. Your users can browse stickers (images or any other types of file assets) and then place them in your app.

1. Add the App Extension Host declaration to your app for Universal.Stickers.1
2. Place a StickersControl on a Xaml page
3. Handle the StickersControl.StickerSelected event to place the sticker that the users selects into your app.

    xmlns:sr="using:StickerResources"

    <sr:StickersControl StickerSelected="OnStickerSelected"/>