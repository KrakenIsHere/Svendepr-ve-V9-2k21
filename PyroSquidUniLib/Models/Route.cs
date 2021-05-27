using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyroSquidUniLib.Models
{
    public class Route
    {
        public int Route_ID { get; set; }
        public string Route_AREA { get; set; }
        public string Route_MONTH { get; set; }
        public string Route_YEAR { get; set; }
        public string Route_DESCRIPTION { get; set; }
        public Customer[] Route_Customers { get; set; }
    }
}
