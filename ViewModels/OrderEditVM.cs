using CRMBytholod.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRMBytholod.ViewModels
{
    public class OrderEditVM
    {

        public Order order { get; set; }
        public List<User> MASTERs { get; set; }
        public List<Organization> ORGANIZATIONs { get; set; }

        public OrderEditVM()
        {

        }

        public OrderEditVM(long ID_ZAKAZ)
        {
            order = Order.GetOrderSite(ID_ZAKAZ);
            MASTERs = User.GetAllMasters();
            ORGANIZATIONs = Organization.GetAllOrganization();

        }
    }
}
