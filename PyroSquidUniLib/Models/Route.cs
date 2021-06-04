using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyroSquidUniLib.Models
{
    public class Route
    {
        public int ID { get; set; }
        public string Navn { get; set; }
        public string Maaned { get; set; }
        public int Aar { get; set; }
        public string Beskrivelse { get; set; }

        public static Route[] CreateFromJson(string json)
        {
            return JsonConvert.DeserializeObject<Route[]>(json);
        }
    }
}
