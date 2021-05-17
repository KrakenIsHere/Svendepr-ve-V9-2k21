using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyroSquidUniLib.Models
{
    public class Customer
    {
        public int Customer_ID { get; set; }
        public string Customer_USERNAME { get; set; }
        public string Customer_FIRSTNAME { get; set; }
        public string Customer_LASTNAME { get; set; }
        public string Customer_ADDRESS { get; set; }
        public int Customer_ZIPCODE { get; set; }
        public string Customer_CITY { get; set; }
        public string Customer_EMAIL { get; set; }
        public string Customer_PHONE_MOBILE { get; set; }
        public string Customer_PHONE_HOME { get; set; }
        public int Customer_SERVICES_NEEDED { get; set; }
        public string Customer_MONTH { get; set; }
        public string Customer_Comment { get; set; }
        public DateTime Customer_CreationDate { get; set; }
    }
}
