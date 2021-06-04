using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyroSquidUniLib.Models
{
    public class ServiceProduct
    {
        public int ID { get; set; }
        public string ProduktNavn { get; set; }
        public decimal Pris { get; set; }
        public string Beskrivelse { get; set; }
        public int FejningsID { get; set; }
        public int ProduktID { get; set; }

        public static ServiceProduct[] CreateFromJson(string json)
        {
            return JsonConvert.DeserializeObject<ServiceProduct[]>(json.Replace(" ID", "ID"));
        }
    }
}