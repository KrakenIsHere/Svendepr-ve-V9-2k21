using PyroSquidUniLib.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PyroSquidUniLib.FileSystem
{
    class AsyncFtpHelper
    {
        public static async Task<bool> FileDownload(string localFilepath = "", string filename = "", string ftpfilepath = "")
        {
            try
            {
                string inputfilepath = $@"{localFilepath}{filename}";
                string ftphost = PropertiesExtension.Get<string>("FtpHost");
                string userName = PropertiesExtension.Get<string>("FtpUser");
                string pass = PropertiesExtension.Get<string>("FtpPass");

                string ftpfullpath = "ftp://" + ftphost + ftpfilepath + filename;

                using (WebClient request = new WebClient())
                {
                    request.Credentials = new NetworkCredential(userName, pass);
                    byte[] fileData = request.DownloadData(ftpfullpath);

                    using (FileStream file = File.Create(inputfilepath))
                    {
                        file.Write(fileData, 0, fileData.Length);
                        file.Close();
                    }
                }

                await Task.FromResult(true);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await Task.FromResult(false);
                return false;
            }
        }

        public static async Task<bool> FileUpload(string localFilepath = "", string filename = "", string ftpfilepath = "")
        {
            try
            {
                string outputfilepath = $@"{localFilepath}{filename}";
                string ftphost = PropertiesExtension.Get<string>("FtpHost");
                string userName = PropertiesExtension.Get<string>("FtpUser");
                string pass = PropertiesExtension.Get<string>("FtpPass");

                string ftpfullpath = "ftp://" + ftphost + ftpfilepath + filename;

                using (var client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(userName, pass);
                    client.UploadFile(ftpfullpath, WebRequestMethods.Ftp.UploadFile, outputfilepath);
                }
                await Task.FromResult(true);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await Task.FromResult(false);
                return false;
            }
        }
    }

    class FtpHelper
    {
        public static bool FileDownload(string localFilepath = "", string filename = "", string ftpfilepath = "")
        {
            try
            {
                string inputfilepath = $@"{localFilepath}{filename}";
                string ftphost = PropertiesExtension.Get<string>("FtpHost");
                string userName = PropertiesExtension.Get<string>("FtpUser");
                string pass = PropertiesExtension.Get<string>("FtpPass");

                string ftpfullpath = "ftp://" + ftphost + ftpfilepath + filename;

                using (WebClient request = new WebClient())
                {
                    request.Credentials = new NetworkCredential(userName, pass);
                    byte[] fileData = request.DownloadData(ftpfullpath);

                    using (FileStream file = File.Create(inputfilepath))
                    {
                        file.Write(fileData, 0, fileData.Length);
                        file.Close();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public static bool FileUpload(string localFilepath = "", string filename = "", string ftpfilepath = "")
        {
            try
            {
                string outputfilepath = $@"{localFilepath}{filename}";
                string ftphost = PropertiesExtension.Get<string>("FtpHost");
                string userName = PropertiesExtension.Get<string>("FtpUser");
                string pass = PropertiesExtension.Get<string>("FtpPass");

                string ftpfullpath = "ftp://" + ftphost + ftpfilepath + filename;

                using (var client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(userName, pass);
                    client.UploadFile(ftpfullpath, WebRequestMethods.Ftp.UploadFile, outputfilepath);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
    }
}
