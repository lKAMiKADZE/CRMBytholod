using CRMBytholod.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRMBytholod.ViewModels
{
    public class OrderDetailVM
    {
        public Order order { get; set; }
        public List<Order> PrevOrders { get; set; }
        public List<LogStatusOrder> logStatusOrder { get; set; }
        public List<LogMoney> logMoneyOrder { get; set; }
        


        public OrderDetailVM(long ID_ZAKAZ)
        {
            order = Order.GetOrderSite(ID_ZAKAZ);
            PrevOrders = Order.GetOrdersPrev(ID_ZAKAZ);
            logStatusOrder = LogStatusOrder.GetHistoryStatusOrder(ID_ZAKAZ);
            logMoneyOrder = LogMoney.GetLogMoneys(ID_ZAKAZ);
        }

    }
}
