using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace DDrop.Utility.ImageOperations
{
    public static class ImageInterpreter
    {
        public static byte[] FileToByteArray(string fileName)
        {
            byte[] fileData = null;

            using (var fs = File.OpenRead(fileName))
            {
                var binaryReader = new BinaryReader(fs);
                fileData = binaryReader.ReadBytes((int) fs.Length);
            }

            return fileData;
        }

        public static string ByteArrayToFile(byte[] content, string fileName, string directoryName)
        {
            string path = Path.Combine(Environment.CurrentDirectory, directoryName);

            Directory.CreateDirectory(path);

            var fullFilePath = $@"{path}\{fileName}";

            File.WriteAllBytes(fullFilePath, content);

            return fullFilePath;
        }

        public static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;

            var image = new BitmapImage();

            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }

            image.Freeze();

            return image;
        }

        public static BitmapImage ToBitmapImage(this Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        public static byte[] GetRGBValues(this Bitmap bmp)
        {

            using (MemoryStream ms = new MemoryStream())
            {
                bmp.Save(ms, ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        public static void GetImageFilter(OpenFileDialog openFileDialog)
        {
            openFileDialog.Filter = "";

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            var filteredCodecs = codecs.Where(x => x.FormatDescription != "GIF" && x.FormatDescription != "TIFF").ToArray();

            string sep = string.Empty;

            foreach (var c in filteredCodecs)
            {
                string codecName = c.CodecName.Substring(8).Replace("Codec", "Files").Trim();
                openFileDialog.Filter = string.Format("{0}{1}{2} ({3})|{3}", openFileDialog.Filter, sep, codecName,
                    c.FilenameExtension);
                sep = "|";
            }

            openFileDialog.Filter = string.Format("{0}{1}{2} ({3})|{3}", openFileDialog.Filter, sep, "All Files", "*.*");

            openFileDialog.DefaultExt = ".jpg";
        }
    }
}