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
        public int Page { get; set; }
        public int MaxPage { get; set; }
        public int[] Steps { get; set; }
        public int Step { get; set; }

        public FiltrOrders filtrOrders { get; set; }


        public OrdersVM( string id_user, int _page, int _step, FiltrOrders filtr )
        {
            Steps = new int[] { 5, 10, 20, 50, 100 };
            if (_step == 0) _step = 10;
            if (_page == 0) _page = 1;
                        
            Step = _step;
            Page = _page;

            filtrOrders = filtr;

            Orders = Order.GetOrdersSite(Page, Step, filtrOrders);

        }

    }


    public class FiltrOrders
    {
        public string Adres { get; set; }
        public long ID_STATUS { get; set; }// выпадающий список
        public string Msisdn { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public bool Povtor { get; set; }

        public FiltrOrders()
        {
            DateStart = DateEnd = DateTime.Now;
            DateStart = DateStart.AddMonths(-1);
            Adres = "";
            Msisdn = "";
            ID_STATUS = 0;
            Povtor = false;
        }


    }
}
