using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRMBytholod.Models;

namespace CRMBytholod.ViewModels
{
    public class UserEditVM
    {
        public User user { get; set; }
        public List<TypeUser> TypeUsers { get; set; }

        public UserEditVM()
        {

        }

        public UserEditVM(long ID_USER)
        {
            user = User.GetUser(ID_USER);
            TypeUsers = TypeUser.GetAllTypeUsers();
        }


    }
}
