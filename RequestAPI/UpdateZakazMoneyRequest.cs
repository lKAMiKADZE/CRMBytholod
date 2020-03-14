using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRMBytholod.RequestAPI
{
    public class UpdateZakazMoneyRequest:BaseRequest
    {
        public long ID_ZAKAZ { get; set; }
        public long ID_STATUS { get; set; }
        public int MoneyAll { get; set; }
        public int MoneyDetal { get; set; }
        public int MoneyFirm { get; set; }
        public string DescripClose { get; set; }
        public int MoneyDiagnostik { get; set; }
    }
}
