using CRMBytholod.Models;
using CRMBytholod.RequestAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRMBytholod.ResponseAPI
{
    public class StatMasterResponse
    {

        public User MasterStat { get; set; }

        public StatMasterResponse(StatMasterRequest req)
        {            
            MasterStat = User.GetMasterStat(req.Sessionid, req.Start, req.End);
        }

    }
}
