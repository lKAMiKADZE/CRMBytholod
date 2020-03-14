using CRMBytholod.Models;
using CRMBytholod.RequestAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRMBytholod.ResponseAPI
{
    public class UpdateZakazMoneyResponse
    {
        public bool Result { get; set; }


        public UpdateZakazMoneyResponse(UpdateZakazMoneyRequest req)
        {
            Result = false;

            Order.Update_MoneyMaster(req.Sessionid, req.ID_ZAKAZ, req.MoneyAll, req.MoneyDetal, req.MoneyFirm, req.MoneyDiagnostik);

            Result = true;

        }
    }
}
