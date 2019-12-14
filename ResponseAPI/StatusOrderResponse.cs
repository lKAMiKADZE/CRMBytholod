using CRMBytholod.Models;
using CRMBytholod.RequestAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace CRMBytholod.ResponseAPI
{
    public class StatusOrderResponse
    {
        
        public bool Result { get; set; }


        public StatusOrderResponse(StatusOrderRequest req)
        {
            Result = false;

            switch (req.ID_STATUS)
            {
                case 3: Order.SetStatus_Denied(req.Sessionid, req.ID_ZAKAZ, req.DescripClose); Result = true; break;
                case 4: Order.SetStatus_InWork(req.Sessionid, req.ID_ZAKAZ); Result = true; break;
                case 5: Order.SetStatus_Succes(req.Sessionid, req.ID_ZAKAZ,req.MoneyAll, req.MoneyDetal, req.MoneyFirm); Result = true; break;
            }
        }





    }
}
