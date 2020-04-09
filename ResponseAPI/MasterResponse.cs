using CRMBytholod.Models;
using CRMBytholod.RequestAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace CRMBytholod.ResponseAPI
{
    public class MasterResponse
    {
        
        public User Master { get; set; }


        public MasterResponse(MasterRequest req)
        {
            Master = User.GetMasterInfo(req.Sessionid);       

        }





    }
}
