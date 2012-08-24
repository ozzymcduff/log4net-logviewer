using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Data;

namespace LogViewer
{
    public class ImageTypeToImageSourceConverter : MarkupExtension, IValueConverter
    {
        private static Dictionary<LogEntry.ImageType, BitmapSource> _ImageList =
           new Dictionary<LogEntry.ImageType, BitmapSource>()
            {
                {LogEntry.ImageType.Debug, Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Question.Handle, Int32Rect.Empty, null)},
                {LogEntry.ImageType.Error, Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Error.Handle, Int32Rect.Empty, null)},
                {LogEntry.ImageType.Fatal, Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Hand.Handle, Int32Rect.Empty, null)},
                {LogEntry.ImageType.Info, Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Information.Handle, Int32Rect.Empty, null)},
                {LogEntry.ImageType.Warn, Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Warning.Handle, Int32Rect.Empty, null)},
                {LogEntry.ImageType.Custom, Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Asterisk.Handle, Int32Rect.Empty, null)}
            };


        private static ImageSource GetImage(LogEntry.ImageType image)
        {
            return _ImageList[image];
        }
        private static LogEntry.ImageType GetImageType(ImageSource image)
        {
            return _ImageList.First(i => i.Value == image).Key;
        }
        public ImageTypeToImageSourceConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return GetImage((LogEntry.ImageType)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return GetImageType((ImageSource)value);
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
