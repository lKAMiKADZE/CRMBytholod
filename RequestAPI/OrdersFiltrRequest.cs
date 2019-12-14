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



    }
}
