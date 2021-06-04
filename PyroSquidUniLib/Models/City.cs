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
        public int Postnr { get; set; }
        public string By { get; set; }
        public int id { get; set; }
        public static City[] CreateFromJson(string json)
        {
            return JsonConvert.DeserializeObject<City[]>(json);
        }
    }
}
