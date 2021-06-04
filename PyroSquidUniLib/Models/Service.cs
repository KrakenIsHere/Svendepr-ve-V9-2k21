using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyroSquidUniLib.Models
{
    public enum PaymentMethod { BankTransfer, GiroCard, Invoice }

    public class Service
    {
        public int ID { get; set; }
        public int KundeID { get; set; }
        public string Nummer { get; set; }
        public int AntalGange { get; set; }
        public string Dato { get; set; }
        public string Betaling { get; set; }
        public int aar { get; set; }

        public static Service[] CreateFromJson(string json)
        {
            return JsonConvert.DeserializeObject<Service[]>(json.Replace(" ID", "ID").Replace(" Gange", "Gange"));
        }
    }
}
