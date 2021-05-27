using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyroSquidUniLib.Models
{
    public class City
    {
        public int value { get; set; }
        public int Postnr { get; set; }
        public string By { get; set; }

        public static City[] CreateFromJson(string json)
        {
            return JsonConvert.DeserializeObject<City[]>(json);
        }
    }
}
