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
        public int Service_ID { get; set; }
        public int Customer_ID { get; set; }
        public int Service_NUMBER { get; set; }
        public string Service_DATE { get; set; }
        public string Service_PAYDATE { get; set; }
        public string Service_PAYMENTMETHOD { get; set; }
        public string Service_INVOICE_NUMBER { get; set; }
        public int Service_YEAR { get; set; }
        public Product[] Service_PRODUCT { get; set; }
    }
}
