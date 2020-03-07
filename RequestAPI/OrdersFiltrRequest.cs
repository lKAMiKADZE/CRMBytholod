using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRMBytholod.RequestAPI
{
    public class OrdersFiltrRequest : BaseRequest
    {
        public string Adress { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public bool filtr_SUCCES { get; set; }
        public bool filtr_DENY { get; set; }
        public bool filtr_POVTOR { get; set; }
        public bool filtr_DATE { get; set; }
        public bool filtr_ADRES { get; set; }
        public bool filtr_DIAGNOSTIK { get; set; }
        



    }
}
