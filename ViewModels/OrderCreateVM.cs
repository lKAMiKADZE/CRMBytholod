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
            ORGANIZATIONs = Organization.GetAllOrganization();
            order = new Order();
            MASTERs = User.GetAllMasters();
           
        }
        public OrderCreateVM(long ID_ZAKAZ)
        {
            ORGANIZATIONs = Organization.GetAllOrganization();
            order = Order.GetOrderSite(ID_ZAKAZ);
            MASTERs = User.GetAllMasters();

        }
    }
}
