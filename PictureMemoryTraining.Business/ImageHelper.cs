using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PictureMemoryTraining.Business.Annotations;

namespace PictureMemoryTraining.Business
{
    public static class ImageHelper
    {
        /// <summary>
        /// 以非独占模式创建一张图片，创建时忽略ColorProfile信息
        /// </summary>
        /// <param name="filename">文件绝对路径</param>
        /// <returns>返回创建后的<see cref="BitmapImage"/></returns>
        public static BitmapImage CreateImageNonReadOnly(string filename)
        {
            using (FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var memoryStream = new MemoryStream();
                fileStream.CopyTo(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
                bitmap.StreamSource = memoryStream;
                bitmap.EndInit();
                bitmap.SetValue(ImageSourceExtension.ImageFilePathProperty, filename);
                return bitmap;
            }
        }
    }
    public static class ImageSourceExtension
    {
        public static readonly DependencyProperty ImageFilePathProperty = DependencyProperty.RegisterAttached("ImageFilePath", typeof(string), typeof(ImageSource));

        public static void SetImageFilePath([NotNull] this ImageSource source, string value)
        {
            source.SetValue(ImageSourceExtension.ImageFilePathProperty, (object)value);
        }

        public static string GetImageFilePath([NotNull] this ImageSource source)
        {
            return (string)source.GetValue(ImageSourceExtension.ImageFilePathProperty);
        }
    }
}
