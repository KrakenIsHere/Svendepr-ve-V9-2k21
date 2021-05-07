using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Data;
using System.Reflection;

namespace PyroSquidUniLib.FileSystem
{
    public class AsyncFileSystemHelper
    {
        #region Edit

        public static async void AddContentOnToTextFileAsync(string pathWithFileName, string text)
        {
            File.AppendAllText(pathWithFileName, text);
            await Task.FromResult(true);
        }

        public static async void CreateRewriteTextFileAsync(string path, string fileName, string text)
        {
            switch (Directory.Exists(path))
            {
                case true:
                    {
                        File.WriteAllText(path + fileName, text);
                        break;
                    }
                case false:
                    {
                        Directory.CreateDirectory(path);
                        File.WriteAllText(path + fileName, text);
                        break;
                    }
            }
            await Task.FromResult(true);
        }

        #endregion

        #region Get

        public static async Task<string[]> ReadTextFileToArrayAsync(string pathWithFileName)
        {
            await Task.FromResult(true);
            return File.ReadAllLines(pathWithFileName);
        }

        public static async Task<Bitmap> GetImageFile(string path)
        {
            await Task.FromResult(true);
            return (Bitmap)System.Drawing.Image.FromFile(path);
        }

        public static async Task<string> GetDirectoryParentPathAsync(string path)
        {
            var parent = Directory.GetParent(path);

            await Task.FromResult(true);
            return parent.FullName;
        }

        public static async Task<string[]> GetFilePathsFromFolderAsArrayAsync(string path)
        {
            try
            {
                switch (Directory.Exists(path))
                {
                    case true:
                        {
                            await Task.FromResult(true);
                            return Directory.GetFiles(path);
                        }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            await Task.FromResult(true);
            return null;
        }

        #endregion

        #region Create

        public static async void CreateImageAsync(string pathWithFileName, Bitmap map)
        {
            try
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    using (FileStream fs = new FileStream(pathWithFileName, FileMode.Create, FileAccess.ReadWrite))
                    {
                        map.Save(memory, ImageFormat.Png);
                        var bytes = memory.ToArray();
                        fs.Write(bytes, 0, bytes.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            await Task.FromResult(true);
        }

        public static async void CreateFolderAsync(string pathWithFileName)
        {
            try
            {
                switch (!Directory.Exists(pathWithFileName))
                {
                    case true:
                        {
                            Directory.CreateDirectory(pathWithFileName);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            await Task.FromResult(true);
        }

        #endregion

        #region Delete

        public static async void DeleteFilesAsync(string[] paths)
        {
            try
            {
                foreach (string path in paths)
                {
                    switch (Directory.Exists(path))
                    {
                        case true:
                            {
                                Directory.Delete(path);
                                break;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            await Task.FromResult(true);
        }

        public static async void DeleteSingleFileAsync(string path)
        {
            try
            {
                switch (Directory.Exists(path))
                {
                    case true:
                        {
                            Directory.Delete(path);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            await Task.FromResult(true);
        }

        public static async void DeleteEmptyFolderAsync(string path)
        {
            try
            {
                switch (Directory.Exists(path))
                {
                    case true:
                        {
                            Directory.Delete(path);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            await Task.FromResult(true);
        }

        public static async void DeleteFolderAndContentAsync(string path)
        {
            try
            {
                switch (Directory.Exists(path))
                {
                    case true:
                        {
                            Directory.Delete(path, true);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            await Task.FromResult(true);
        }

        #endregion
    }

    public class FileSystemHelper
    {
        public static FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
        public static OpenFileDialog imageFileDialog = new OpenFileDialog();
        
        #region Edit

        public static void AddContentOnToTextFile(string pathWithFileName, string text)
        {
            File.AppendAllText(pathWithFileName, text);
        }

        #endregion

        #region Get
        public static Bitmap GetImageFile(string path, string fileName)
        {
            switch (Directory.Exists(path))
            {
                case true:
                    {
                        return (Bitmap)System.Drawing.Image.FromFile(path + fileName);
                    }
                default:
                    {
                        return null;
                    }
            }
        }
        public static string GetDirectoryParentPath(string path)
        {
            var parent = Directory.GetParent(path);

            return parent.FullName;
        }

        public static string[] GetFilePathsFromFolderAsArray(string path)
        {
            try
            {
                switch (Directory.Exists(path))
                {
                    case true:
                        {
                            return Directory.GetFiles(path);
                        }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
                
        }

        #endregion

        #region Create

        public static void CreateImage(string pathWithFileName, Bitmap map)
        {
            try
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    using (FileStream fs = new FileStream(pathWithFileName, FileMode.Create, FileAccess.ReadWrite))
                    {
                        map.Save(memory, ImageFormat.Png);
                        var bytes = memory.ToArray();
                        fs.Write(bytes, 0, bytes.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static void CreateFolder(string pathWithFileName)
        {
            try
            {
                switch (!Directory.Exists(pathWithFileName))
                {
                    case true:
                        {
                            Directory.CreateDirectory(pathWithFileName);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        #endregion

        #region Delete

        public static void DeleteFiles(string[] paths)
        {
            try
            {
                foreach (string path in paths)
                {
                    switch (Directory.Exists(path))
                    {
                        case true:
                            {
                                Directory.Delete(path);
                                break;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static void DeleteSingleFile(string path)
        {
            try
            {
                switch (Directory.Exists(path))
                {
                    case true:
                        {
                            Directory.Delete(path);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static void DeleteEmptyFolder(string path)
        {
            try
            {
                switch (Directory.Exists(path))
                {
                    case true:
                        {
                            Directory.Delete(path);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static void DeleteFolderAndContent(string path)
        {
            try
            {
                switch (Directory.Exists(path))
                {
                    case true:
                        {
                            Directory.Delete(path, true);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        #endregion
    }
}
