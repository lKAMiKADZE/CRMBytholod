using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRMBytholod.RequestAPI
{
    public class StatMasterRequest:BaseRequest
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
