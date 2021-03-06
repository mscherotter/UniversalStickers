# UniversalStickers
A way for Windows apps to provide collections stickers and clip art to other apps.  Any UWP App with a minimum version set to the Windows Anniversary Update (10.0.14393.0) can be a sticker app or a sticker host.

# Visual Studio Template
This is a Visual Studio 2017 project template that creates an app to provide stickers to other apps.  This template is based on the [Stickers](Source/Stickers) project. This app alone has minimal functionality but when used in conjuction with other apps it can provide collections of stickers to the other apps in a number of ways:
1. You can drag and drop the stickers from this app to other apps that accecpt picture files or bitmaps.
2. You can copy any of the stickers from this app to the clipboard to paste into other apps.
3. You can share a sticker from this app to other UWP apps that accept picture files or bitmaps.
4. You can use the file picker in other UWP apps to select this app as a source for sticker files.
5. UWP Apps that declare that they are Sticker Hosts can directly access sticker collections in this app by declaring in their manifest that they host the *Universal.Sticker.1* app extension.

## Installing the Template
1. Install [Microsoft Visual Studio 2017](https://www.visualstudio.com/downloads/) (Community, Professional, or Enterprise editions)
2. Download [Stickers UWP App.zip](https://github.com/mscherotter/UniversalStickers/blob/master/Stickers%20UWP%20App.zip) file to your system.
3. Move the *Stickers UWP App.zip* file to the %UserProfile%\Documents\Visual Studio 2017\Templates\ProjectTemplates\Visual C# folder on your system.
4. Now in Visual Studio, select File...New...Project... and you should see the *Stickers UWP App* project type in the Visual C# folder.
5. Follow the instructions in the ReadMe.txt file to add your collections and prepare the app for publishing to the Windows Store.

![Sticker UWP App Template](Assets/__PreviewImage.jpg)

# Sticker Host Test Project
[Sticker Host](Source/StickerHost) is a test project that shows the sticker collections on the user's system.  It uses the [StickersControl](Source/StickerResources/StickersControl.cs) in the [StickerResources](Source/StickerResources) project.

![Sticker Host Test App](Assets/StickerHost.png)

## Enabling Sticker Hosting in your app
To make your app a sticker host, in the decalarations section of the app manifest editor, Add an *App Extension Host* declaration and put *Universal.Stickers.1* in the Name field.

# StickerResources Project
[StickerResources](Source/StickerResources) is a Visual Studio 2017 project that hosts a StickersControl custom control that can be used in apps to pick stickers.
## The Journalist app hosting the StickersControl.

![Journalist with the StickersControl](Assets/Journalist.png)
# Sticker Apps And Sticker Hosts
[Find apps](StickerApps.md) that both provide stickers (Extension) and use stickers (Hosts).
