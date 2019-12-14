using CRMBytholod.Models;
using CRMBytholod.RequestAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace CRMBytholod.ResponseAPI
{
    public class OrdersFiltrResponse
    {
        
        public List<Order> Orders { get; set; } 

        public OrdersFiltrResponse(OrdersFiltrRequest req)
        {
            // получение списка заявок
            // как новых так и завершенных

            switch (req.Action)
            {
                case "OLD": Orders = Order.GetOldOrdersFiltr(req.Sessionid,req.Adress,req.DateStart,req.DateEnd); break;
                default : Orders = new List<Order>(); break;

            }
            
        }





    }
}
