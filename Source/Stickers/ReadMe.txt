Creating a Sticker App
This UWP application template can be used to build and release a UWP app to the
Windows Store that can then provide stickers or clip art to other apps.  
Stickers are just images that are hosted in this app.  

This app alone has minimal functionality but when used in conjuction with other
apps it can provide collections of stickers to the other apps in a number of 
ways:
1. You can drag and drop the stickers from this app to other apps that accecpt 
   picture files or bitmaps
2. You can copy any of the stickers from this app to the clipboard to paste into other apps
3. You can share a sticker from this app to other apps that accept picture files or bitmaps
4. You can use the file picker in other apps to select this app as a source for sticker files.
5. Apps that declare that they are Sticker Hosts can directly access sticker 
   collections in this app by declaring that they support the Universal.Sticker.1 host name.

Steps to Make a Sticker App.
1. For all of your images, there are recommended specifications
	- Stickers can be portrait or lanscape but prefer 300x300 PNG files with transparency or JPG
	- For each sticker, set the Filename or Title image property to something descriptive
	- For each sticker, define relevant tags in the file properties for JPEG files.
2. Once you have picked your files and set the metadata	in them, drag them to 
   the Shared folder in this project.  An app can support multiple collections
   of stickers.  
3. If you want to create additional collections, right-click on the 
   Stickers project and select Add...New folder and give it a descriptive name.
   Once you have created the folder, drag the images from your file explorer to the folder in the project.
4. In the Strings/en-US/Resources.resw change the Value for AppDescription and AppDisplayName to 
   something meaningful.  Change the value for ExtensionDisplayName and ExtensionDescription to something 
   that describes each collection.  If you have added additional collections, add new pairs of display names 
   and descriptions for each one.
5. In the Solution Explorer, right-click on the Package.appxmanifest and select View Code.
6. You will want to insure that there is an <uap:Extension/> element in the manifest file for
   each collection that you want to support.
   - Each AppExtension.Id attribute must be unique
   - Each AppExtension.PublicFolder must be the name of the folder where you put the stickers in the project
   - Each Author and AuthorLink value should be filled in but don't need to be unique.
   - The DisplayName and Description attributes must match the resource file entries

	<uap3:Extension Category="windows.appExtension">
		<uap3:AppExtension Name="Universal.Stickers.1" Id="MyStickers" 
		    DisplayName="ms-resource:ExtensionDisplayName" 
			Description="ms-resource:ExtensionDescription" 
			PublicFolder="Shared">
		<uap3:Properties>
			<Author>Michael S. Scherotter</Author>
			<AuthorLink>http://www.charette.com</AuthorLink>
		</uap3:Properties>
		</uap3:AppExtension>
	</uap3:Extension>
7. Save and slose the manifest file.
8. Re-open the manifest file by double-clicking on it - it should now open in visual editor
9. In the declarations tab, select the File Open Picker on the left and insure that the file types listed 
   correspond to the file types of the stickers you have supplied.
10. In the Visual Assets tab, replace the boilerplate application icon tile, and splash screen images with images 
   appropriate to your sticker collection.

Testing Your Stickers App
