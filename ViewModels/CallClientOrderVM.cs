using CRMBytholod.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRMBytholod.ViewModels
{
    public class CallClientOrderVM
    {
        public Order order { get; set; }

        public CallClientOrderVM(long ID_ZAKAZ)
        {
            order = Order.GetOrderSite(ID_ZAKAZ);
        }
    }
}
