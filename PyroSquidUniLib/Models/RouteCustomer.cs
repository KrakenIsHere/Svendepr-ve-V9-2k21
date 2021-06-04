using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyroSquidUniLib.Models
{
    public class RouteCustomer
    {
        public int ID { get; set; }
        public string Fornavn { get; set; }
        public string Efternavn { get; set; }
        public string Adresse { get; set; }
        public int Postnr { get; set; }
        public string By { get; set; }
        public int Fejninger { get; set; }
        public string Kommentar { get; set; }
        public string Email { get; set; }
        public string Mobil { get; set; }
        public string Hjemme { get; set; }
        public int KundeID { get; set; }
        public int RuteID { get; set; }

        public static RouteCustomer[] CreateFromJson(string json)
        {
            return JsonConvert.DeserializeObject<RouteCustomer[]>(json.Replace(" ID", "ID"));
        }
    }
}
