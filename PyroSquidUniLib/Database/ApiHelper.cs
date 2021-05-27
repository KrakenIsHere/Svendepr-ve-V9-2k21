using PyroSquidUniLib.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PyroSquidUniLib.Database
{
    public class ApiHelper
    {
        public static async void PostDataTest2()
        {
            var request = WebRequest.Create("link");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write("Values");
            }
        }

        public static async Task<string> PostDataTest3Async(string uri, string data)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.ContentLength = dataBytes.Length;
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";

            using (Stream requestBody = request.GetRequestStream())
            {
                await requestBody.WriteAsync(dataBytes, 0, dataBytes.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public static async Task EditDataAsync(string extention, string values, string method = "PUT")
        {
            var bytes = Encoding.ASCII.GetBytes(values);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format($"http://87.61.88.75:21875/girozillabackend/api/v1/{extention}"));
            request.Method = method;
            request.ContentType = "application/x-www-form-urlencoded";
            using (var requestStream = request.GetRequestStream())
            {
                await requestStream.WriteAsync(bytes, 0, bytes.Length);
            }
            var response = (HttpWebResponse)request.GetResponse();
        }

        public static async Task RemoveDataAsync(string table, string values)
        {
            WebRequest request = WebRequest.Create($"http://87.61.88.75:21875/girozillabackend/api/v1/Remove/{table}?{values}");
            request.Method = "DELETE";
            await request.GetResponseAsync();
        }

        public static async Task<string> GetDataAsync(string table)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"http://87.61.88.75:21875/girozillabackend/api/v1/All/{table}");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
