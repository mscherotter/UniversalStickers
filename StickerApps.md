# Sticker Apps
Open *ms-windows-store://assoc/?Tags=AppExtension-Universal.Stickers.1* in a web browser on Windows 10 to see apps that can provide stickers and clipart to other UWP apps installed on the user's system because they implement the *Universal.Stickers.1* App extension.  You can use the [Sticker UWP App.zip](https://github.com/mscherotter/UniversalStickers/blob/master/Stickers%20UWP%20App.zip) project template to create a Visual Studio 2017 Project to host sticker collections of your own.  See instructions [here](README.md).

# Sticker Hosts
These application host the *Universal.Sticker.1* app extension and will be able to access the content of Sticker Apps.  You can easily host stickers in your creative app by adding the [StickerResources](Source/StickerResources) project to your solution and using the [StickersControl](Source/StickerResources/StickersControl.cs) control to select stickers in your app.
* [Journalist](https://www.microsoft.com/store/apps/9wzdncrdkjj2)
* Your sticker host app here

Please make a pull request to add any Sticker Hosts to the lists.  Any UWP App with a minimum version set to the Windows Anniversary Update (10.0.14393.0) can be a sticker app or a sticker host.

