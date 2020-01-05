using CRMBytholod.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRMBytholod.ViewModels
{
    public class OrderCreateVM
    {

        public Order order { get; set; }
        public List<User> MASTERs { get; set; }
        public List<Organization> ORGANIZATIONs { get; set; }

      
        public OrderCreateVM()
        {
            //order = Order.GetOrderSite(ID_ZAKAZ);
            order = new Order();
            MASTERs = User.GetAllMasters();
            ORGANIZATIONs = Organization.GetAllOrganization();

        }
    }
}
