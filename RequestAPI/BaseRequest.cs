using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRMBytholod.RequestAPI
{
    public class BaseRequest
    {
        public string Sessionid { get; set; }
        public string Action { get; set; }
    }
}
