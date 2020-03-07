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
                case "OLD": Orders = Order.GetOldOrdersFiltr(req.Sessionid,req.Adress,req.DateStart,req.DateEnd,
                    req.filtr_SUCCES, req.filtr_DENY, req.filtr_POVTOR, req.filtr_DATE, req.filtr_ADRES, false); break;
                case "NEW": Orders = Order.GetNewOrders(req.Sessionid); break;

                default : Orders = new List<Order>(); break;

            }
            
        }





    }
}
