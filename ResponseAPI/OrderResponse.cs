using CRMBytholod.Models;
using CRMBytholod.RequestAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace CRMBytholod.ResponseAPI
{
    public class OrderResponse
    {
        
        public Order Order { get; set; }

        public List<Order> PrevOrders { get; set; }

        public OrderResponse(OrderRequest req)
        {
            // получить заявку и если она повтор до подтянуть список пред заявок
            Order = Order.GetOrder(req.Sessionid, req.ID_ZAKAZ);

            if (Order.Povtor)// если заявка является повтором, то подгружаем еще и список пред заявок
                PrevOrders = Order.GetOldOrdersFromOrder(req.ID_ZAKAZ);
            else
                PrevOrders = new List<Order>();

        }





    }
}
