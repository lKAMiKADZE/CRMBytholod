using CRMBytholod.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRMBytholod.RequestAPI;

namespace CRMBytholod.ResponseAPI
{
    public class NotifiNewOrderResponse
    {

        public List<Order> Orders { get; set; }


        public NotifiNewOrderResponse(NotifiNewOrderRequest req)
        {
            Orders = Order.GetNewNotifi(req.Sessionid);

        }
    }
}
