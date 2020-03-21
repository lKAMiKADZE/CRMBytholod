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

            bool OplataNal = !req.OplataBK;

            if (String.IsNullOrEmpty(req.DescripClose))
                req.DescripClose = "";

            switch (req.ID_STATUS)
            {
                case 3: Order.SetStatus_Denied(req.Sessionid, req.ID_ZAKAZ, req.DescripClose); 
                    Result = true; break;
                case 4: Order.SetStatus_InWork(req.Sessionid, req.ID_ZAKAZ); 
                    Result = true; break;
                case 5: Order.SetStatus_Succes(req.Sessionid, req.ID_ZAKAZ, req.MoneyAll, req.MoneyDetal, req.MoneyFirm, req.DescripClose, OplataNal); 
                    Result = true; break;
                case 7: Order.SetStatus_Diagnostik(req.Sessionid, req.ID_ZAKAZ, req.DescripClose, req.MoneyDiagnostik, req.MoneyFirm, OplataNal); 
                    Result = true; break;
            }
        }





    }
}
