using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyroSquidUniLib.Models
{
    public class CustomerServiceData
    {
        public int id { get; set; }
        public int CustomerID { get; set; }
        public int ServiceNumber { get; set; }
        public int Chimneys { get; set; }
        public int Pipes { get; set; }
        public string KW { get; set; }
        public string Lighting { get; set; }
        public int Height { get; set; }
        public int Diameter { get; set; }
        public string Length { get; set; }
        public string Type { get; set; }

        public static CustomerServiceData[] CreateFromJson(string json)
        {
            return JsonConvert.DeserializeObject<CustomerServiceData[]>(json);
        }
    }
}
