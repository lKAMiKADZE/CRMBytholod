using CRMBytholod.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRMBytholod.ViewModels
{
    public class OrdersVM
    {
        public List<Order> Orders { get; set; }

        public OrdersVM( string id_user)
        {
            Orders = Order.GetOrdersSite();
        }

    }
}
