using CRMBytholod.Models;
using CRMBytholod.RequestAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace CRMBytholod.ResponseAPI
{
    public class OrdersResponse
    {
        
        public List<Order> Orders { get; set; } 

        public OrdersResponse(OrdersRequest req)
        {
            // получение списка заявок
            // как новых так и завершенных



            switch (req.Action)
            {
                case "OLD": Orders = Order.GetOldOrders(req.Sessionid); break;
                case "NEW": Orders = Order.GetNewOrders(req.Sessionid); break;
                default : Orders = new List<Order>(); break;

            }
            
        }





    }
}
