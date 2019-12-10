using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRMBytholod.RequestAPI
{
    public class AuthRequest:BaseRequest
    {
        public string Login { get; set; }
        public string Passw { get; set; }


    }
}
