using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRMBytholod.Models;

namespace CRMBytholod.ViewModels
{
    public class UserCreateVM
    {
        public User user { get; set; }
        public List<TypeUser> TypeUsers { get; set; }

        public UserCreateVM()
        {
            user = new User();
            TypeUsers = TypeUser.GetAllTypeUsers();
        }


    }
}
