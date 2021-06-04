using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyroSquidUniLib.Models
{
    public class Customer
    {
        public int ID { get; set; }
        public string Adresse { get; set; }
        public string Fornavn { get; set; }
        public string Efternavn { get; set; }
        public int Postnr { get; set; }
        public string By { get; set; }
        public string Email { get; set; }
        public string Mobil { get; set; }
        public string Hjemme { get; set; }
        public int Fejninger { get; set; }
        public string Maaned { get; set; }
        public string Kommentar { get; set; }

        public static Customer[] CreateFromJson(string json)
        {
            return JsonConvert.DeserializeObject<Customer[]>(json);
        }
    }
}
