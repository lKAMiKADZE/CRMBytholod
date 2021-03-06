﻿using CRMBytholod.Models;
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
        public List<User> MASTERs { get; set; }
        public List<Organization> Organizations { get; set; }

        public int CountAllOrders { get; set; }
        public long ID_ZAKAZ_EDIT { get; set; }
        


        public OrdersVM( string id_user, int _page, int _step, FiltrOrders filtr, long ID_ZAKAZ_EDIT)
        {
            Steps = new int[] { 50,100,150,200};
            if (_step == 0) _step = 200;
            if (_page == 0) _page = 1;
                        
            Step = _step;
            Page = _page;

            filtrOrders = filtr;
            this.ID_ZAKAZ_EDIT = ID_ZAKAZ_EDIT;

            Orders = Order.GetOrdersSite(Page, Step, filtrOrders);
            MASTERs = User.GetAllMasters();
            Organizations = Organization.GetAllOrganization();
            CountAllOrders = Order.GetCountOrders(filtrOrders);

        }

    }


    public class FiltrOrders
    {
        private string _adres;
        
        public long ID_STATUS { get; set; }// выпадающий список
        public string Msisdn { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public bool Povtor { get; set; }
        public long ID_Master { get; set; }
        public DateTime? DateOne { get; set; }
        public string City { get; set; }

        public string Adres 
        { 
            get 
            {
                if (String.IsNullOrEmpty(_adres))
                    return _adres;

                return _adres.Trim(); 
            } 
            set { _adres = value; }
        }

        public FiltrOrders()
        {
            DateStart = null;//DateEnd = DateTime.Now;
            DateStart = null ;
            Adres = "";
            Msisdn = "";
            ID_STATUS = 0;
            Povtor = false;
            ID_Master = -1;
            DateOne = null;
            City = "";
        }


    }
}
