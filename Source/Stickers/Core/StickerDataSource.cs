using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;

namespace StickersApp.Core
{
    public class StickerDataSource
    {
        public static IAsyncOperationWithProgress<IEnumerable<StickerExtension>, double> GetStickersAsync()
        {
            return AsyncInfo.Run(async delegate(CancellationToken token, IProgress<double> progress)
            {
                var manifest = await Package.Current.InstalledLocation.GetFileAsync("AppxManifest.xml");

                if (token.IsCancellationRequested) return null;

                progress.Report(10);

                var uap = "{http://schemas.microsoft.com/appx/manifest/foundation/windows10}";
                var uap3 = "{http://schemas.microsoft.com/appx/manifest/uap/windows10/3}";

                var resources = ResourceLoader.GetForCurrentView();

                using (var stream = await manifest.OpenStreamForReadAsync())
                {
                    if (token.IsCancellationRequested) return null;

                    progress.Report(20);

                    var doc = XDocument.Load(stream);

                    var extensions = (from item in doc.Descendants(uap3 + "AppExtension")
                                      let id = item.Attribute("Id")
                                      let properties = item.Element(uap3 + "Properties")
                                      let author = properties.Element(uap + "Author")
                                      let authorLink = properties.Element(uap + "AuthorLink")
                                      let nameAttribute = item.Attribute("Name")
                                      let displayName = item.Attribute("DisplayName")
                                      let description = item.Attribute("Description")
                                      let publicFolder = item.Attribute("PublicFolder")
                                      where nameAttribute != null
                                      where displayName != null
                                      where description != null
                                      where publicFolder != null
                                      where id != null
                        where nameAttribute.Value == "Universal.Stickers.1"
                        select new StickerExtension
                        {
                            Id = id.Value,
                            PublicFolder = publicFolder.Value,
                            Name = resources.GetString(displayName.Value.Substring("ms-resource:".Length)),
                            Description = resources.GetString(description.Value.Substring("ms-resource:".Length)),
                            Author = author?.Value,
                            AuthorLink = authorLink == null ? null : new Uri(authorLink.Value)
                        }).ToList();

                    if (extensions.Any())
                    {
                        var countPerExtension = 80.0 / Convert.ToDouble(extensions.Count);

                        var index = 0.0;

                        foreach (var extension in extensions)
                        {
                            var action = LoadStickersForExtensionAsync(extension, token);

                            var index1 = index;

                            action.Progress = delegate(IAsyncActionWithProgress<double> info, double progressInfo2)
                            {
                                progress.Report(20.0 +  (countPerExtension * index1) + countPerExtension * progressInfo2 / 100.0);
                            };

                            await action;

                            if (token.IsCancellationRequested) return null;

                            index++;
                        }
                    }

                    return extensions.AsEnumerable();
                }
            });
        }

        private static IAsyncActionWithProgress<double> LoadStickersForExtensionAsync(StickerExtension extension, CancellationToken token)
        {
            return AsyncInfo.Run(async delegate(CancellationToken innerToken, IProgress<double> progress)
            {
                using (var joinedToken = CancellationTokenSource.CreateLinkedTokenSource(token, innerToken))
                {
                    // This will fail if the folder is empty
                    var folder = await Package.Current.InstalledLocation.GetFolderAsync(extension.PublicFolder);

                    if (joinedToken.IsCancellationRequested) return;

                    progress.Report(10);

                    var files = await folder.GetFilesAsync();

                    if (joinedToken.IsCancellationRequested) return;

                    progress.Report(20);

                    if (files.Count > 0)
                    {
                        var countPerFile = 80.0 / Convert.ToDouble(files.Count);
                        var index = 0.0;

                        foreach (var file in files)
                        {
                            var sticker = await Sticker.CreateAsync(file);

                            if (joinedToken.IsCancellationRequested) return;

                            extension.Stickers.Add(sticker);

                            index++;

                            progress.Report(20.0 + countPerFile * index);
                        }
                    }
                }
            });
        }
    }
}