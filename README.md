# UniversalStickers
A way for Windows apps to provide collections stickers and clip art to other apps.

# Visual Studio Template
This is a Visual Studio 2017 project template that creates an app to host stickers. This template is based on the [Stickers](Source/Stickers) project. This app alone has minimal functionality but when used in conjuction with other apps it can provide collections of stickers to the other apps in a number of ways:
1. You can drag and drop the stickers from this app to other apps that accecpt picture files or bitmaps
2. You can copy any of the stickers from this app to the clipboard to paste into other apps
3. You can share a sticker from this app to other apps that accept picture files or bitmaps
4. You can use the file picker in other apps to select this app as a source for sticker files.
5. Apps that declare that they are Sticker Hosts can directly access sticker collections in this app by declaring that they support the *Universal.Sticker.1* host name.

## Installing the Template
1. Download the [Stickers UWP App](Stickers UWP App.zip).zip file.
2. Move the *Stickers UWP App.zip* file to the %UserProfile%\Documents\Visual Studio 2017\Templates\ProjectTemplates folder on your system.
3. Now in Visual Studio, select File...New...Project... and you should see the *Stickers UWP App* project type in the Visual C# folder.

# Sticker Host Test Project
[Sticker Host](Source/StickerHost) is a test project that shows the sticker collections on the user's system.  It uses the [StickersControl](Source/StickerResources/StickersControl.cs) in the [StickerResources](Source/StickerResources) project.

# StickerResources Project
This is a Visual Studio project that hosts a StickersControl custom control that can be used in apps to pick stickers.

# Sticker Apps And Sticker Hosts
[Find apps](StickerApps.md) that both provide stickers (Extension) and use stickers (Hosts).
