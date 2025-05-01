using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Resources;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace FileManager
{
    enum Ico
    {
        Folder = 0,
        File
    }
    static class Manager
    {
        private static Bitmap[] bitmaps = null;
        public static Bitmap[] Bitmaps
        {
            get
            {
                if (bitmaps is null)
                    bitmaps = new Bitmap[]
                    {
                        Properties.Resources.folder,
                        Properties.Resources.file
                    };
                return bitmaps;
            }
        }

        private static Dictionary<Ico, BitmapImage> images = null;
        public static Dictionary<Ico, BitmapImage> Images
        {
            get
            {
                if (images is null)
                {
                    images = new Dictionary<Ico, BitmapImage>(Bitmaps.Length);
                    for (int i = 0; i < Bitmaps.Length; i++)
                        images.Add((Ico)i, BitmapToBitmapImage(Bitmaps[i], System.Drawing.Imaging.ImageFormat.Png));
                }
                return images;
            }
        }

        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, format); // Save bitmap to stream
                memory.Position = 0; // Reset stream position

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze(); // Make it cross-thread accessible
                return bitmapImage;
            }
        }

        public static string GetParentPath(string path)
        {
            string[] oldNames = path.Split(new char[] { '\\' });
            string[] newNames = new string[oldNames.Length - 1];
            if (oldNames[oldNames.Length - 1] == "") newNames = new string[oldNames.Length - 2];
            for (int i = 0; i < newNames.Length; i++)
                newNames[i] = oldNames[i];
            if (newNames.Length == 1) return newNames[0] + @"\";
            if (newNames.Length == 0) return "Компьютер";
            else return string.Join(@"\", newNames);
        }

        public static DirectoryInfo[] GetDirectories(string path)
        {
            try
            {
                string[] dirsPath = Directory.GetDirectories(path);
                DirectoryInfo[] dirs = new DirectoryInfo[dirsPath.Length];

                for (int i = 0; i < dirs.Length; i++)
                    dirs[i] = new DirectoryInfo(dirsPath[i]);
                return dirs;
            }
            catch (UnauthorizedAccessException)
            {
                return new DirectoryInfo[0];
            }
        }

        public static FileInfo[] GetFiles(string path)
        {
            try
            {
                string[] fileNames = Directory.GetFiles(path);
                FileInfo[] files = new FileInfo[fileNames.Length];

                for (int i = 0; i < files.Length; i++)
                    files[i] = new FileInfo(fileNames[i]);
                return files;
            }
            catch (UnauthorizedAccessException)
            {
                return new FileInfo[0];
            }
        }

        public static string GetFileSizeInfo(double bytes)
        {
            string result = $"{bytes} БТ";

            if (bytes >= 1073741824)
                result = $"{Math.Round(bytes / 1073741824, 1)} ГБ";
            else if (bytes >= 1048576)
                result = $"{Math.Round(bytes / 1048576, 1)} МБ";
            else if (bytes >= 1024)
                result = $"{Math.Round(bytes / 1024, 1)} КБ";

            return result;
        }

        public static string GetDirectorySizeInfo(string directoryPath)
        {
            double size = 0.0;
            void GetFilesSizeOnDirectory(string path)
            {
                DirectoryInfo[] dirs = null;
                FileInfo[] files = null;
                dirs = GetDirectories(path);
                files = GetFiles(path);
                foreach (var file in files)
                    size += file.Length;
                if (dirs.Length == 0) return;

                foreach (var dir in dirs)
                    GetFilesSizeOnDirectory(dir.FullName);
            }
            GetFilesSizeOnDirectory(directoryPath);
            return GetFileSizeInfo(size); //GetFileSizeinfo() fits too
        }

        public static string GetDriveSpaceInfo(double availableSpace, double totalSpace)
        {
            string availbleInfo = $"Доступно: {availableSpace} БТ";
            string totalInfo = $"Всего: {totalSpace} БТ";

            if (availableSpace >= 1073741824)
                availbleInfo = $"Доступно: {Math.Round(availableSpace / 1073741824, 1)} ГБ";
            else if (availableSpace >= 1048576)
                availbleInfo = $"Доступно: {Math.Round(availableSpace / 1048576, 1)} МБ";
            else if (availableSpace >= 1024)
                availbleInfo = $"Доступно: {Math.Round(availableSpace / 1024, 1)} КБ";

            if (totalSpace >= 1073741824)
                totalInfo = $"Всего: {Math.Round(totalSpace / 1073741824, 1)} ГБ";
            else if (totalSpace >= 1048576)
                totalInfo = $"Всего: {Math.Round(totalSpace / 1048576, 1)} МБ";
            else if (totalSpace >= 1024)
                totalInfo = $"{Math.Round(totalSpace / 1024, 1)} КБ";

            return availbleInfo + " \\ " + totalInfo;
        }

        public static string GetPathTextWithArrow(string path)
        {
            string result = null;
            string pathText = @"Компьютер\" + path;
            string[] names = pathText.Split('\\');
            for (int i = 0; i < names.Length; i++)
                result += names[i] + ">";
            return result;
        }
    }
}