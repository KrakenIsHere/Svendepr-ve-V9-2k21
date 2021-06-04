using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyroSquidUniLib.Models
{
    public class Product
    {
        public int id { get; set; }
        public string Navn { get; set; }
        public decimal Pris { get; set; }
        public string Beskrivelse { get; set; }

        public static Product[] CreateFromJson(string json)
        {
            return JsonConvert.DeserializeObject<Product[]>(json);
        }
    }
}
