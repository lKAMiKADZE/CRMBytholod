using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRMBytholod.Models;

namespace CRMBytholod.ViewModels
{
    public class UsersVM
    {
        public List<User> Users { get; set; }
        public List<TypeUser> TypeUsers { get; set; }

        public UsersVM(long ID_USER)
        {
            Users = User.GetAllUsers(ID_USER);
            TypeUsers = TypeUser.GetAllTypeUsers();
        }


    }
}
