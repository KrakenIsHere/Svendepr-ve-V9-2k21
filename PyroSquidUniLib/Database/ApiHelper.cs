using PyroSquidUniLib.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PyroSquidUniLib.Database
{
    public class ApiHelper
    {
        public static async Task<bool> PutDataAsync(string table, string values)
        {
            try
            {
                var bytes = Encoding.ASCII.GetBytes(values);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format($"https://api.pyrocorestudios.dk/girozillabackend/v1/Edit/{table}"));
                request.Method = "PUT";
                request.ContentType = "application/x-www-form-urlencoded";
                using (var requestStream = request.GetRequestStream())
                {
                    await requestStream.WriteAsync(bytes, 0, bytes.Length);
                }
                
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        public static async Task<bool> JPutDataAsync(string table, string values)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("PUT"), $"https://api.pyrocorestudios.dk/girozillabackend/v1/Edit/{table}"))
                    {
                        request.Headers.TryAddWithoutValidation("accept", "application/json");

                        request.Content = new StringContent(values);
                        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                        var response = await httpClient.SendAsync(request);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        public static async Task<bool> PostDataAsync(string table, string values)
        {
            try
            {
                var bytes = Encoding.ASCII.GetBytes(values);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format($"https://api.pyrocorestudios.dk/girozillabackend/v1/Add/{table}"));
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                using (var requestStream = request.GetRequestStream())
                {
                    await requestStream.WriteAsync(bytes, 0, bytes.Length);
                    Debug.WriteLine(await request.GetResponseAsync());
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        public static async Task<bool> DeleteDataAsync(string table, string values)
        {
            try
            {
                WebRequest request = WebRequest.Create($"https://api.pyrocorestudios.dk/girozillabackend/v1/Remove/{table}?{values}");
                request.Method = "DELETE";
                Debug.WriteLine(await request.GetResponseAsync());
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }
        public static async Task<string> GetDataAsync(string table)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"https://api.pyrocorestudios.dk/girozillabackend/v1/All/{table}");
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            Debug.WriteLine(response.StatusDescription);
                            return await reader.ReadToEndAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return "";
            }
        }
    }
}
