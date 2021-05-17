using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PyroSquidUniLib.Database
{
    class ApiHelper
    {
        public static async void PostDataTest1()
        {
            var data = new ASCIIEncoding().GetBytes("Values");
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("link");
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/form-urlencoded";
            httpWebRequest.ContentLength = data.Length;
            var stream = httpWebRequest.GetRequestStream();
            stream.Write(data, 0, data.Length);
            stream.Close();
        }

        public static async void PostDataTest2()
        {
            var request = WebRequest.Create("link");
            request.Method = "POST";
            request.ContentType = "application/form-urlencoded";
            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write("Values");
            }
        }
    }
}
