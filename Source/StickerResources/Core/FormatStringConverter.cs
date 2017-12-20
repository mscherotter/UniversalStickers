using System;
using System.Globalization;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace StickerResources.Core
{
    internal class FormatStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter == null)
            {
                return value;
            }

            var format = parameter.ToString();

            // this will fail if the application has no .resw files
            var resources = ResourceLoader.GetForCurrentView("StickerResources/Resources");

            var resourceValue = resources.GetString(format);

            if (!string.IsNullOrWhiteSpace(resourceValue))
                format = resourceValue;

            return string.Format(CultureInfo.CurrentCulture, format, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}