using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRMBytholod.Models
{
    public class Order
    {
        public int ID_ZAKAZ { get; set; }
        public string STREET { get; set; }
        public string HOUSE { get; set; }
        public string KORPUS { get; set; }
        public string KVARTIRA { get; set; }
        public string PODEST { get; set; }
        public string ETAG { get; set; }
        public string KOD_DOMOFONA { get; set; }
        public string TELEFON_STACIONAR { get; set; }
        public string TELEFON_SOTOV { get; set; }
        public string HOLODILNIK_DEFECT { get; set; }
        public string MASTER { get; set; }
        public DateTime DATA { get; set; }
        public string VREMJA { get; set; }
        public string DISPETCHER { get; set; }
        public string PRIMECHANIE { get; set; }
        public bool VIPOLNENIE_ZAKAZA { get; set; }
        public string Msisdn1 { get; set; }
        public string Msisdn2 { get; set; }
        public string Msisdn3 { get; set; }
        public int ID_ORGANIZATION { get; set; }
        public int ID_MASTER { get; set; }
        public DateTime DateAdd { get; set; }
        public DateTime DateSendMaster { get; set; }
        public DateTime DateOpenMaster { get; set; }
        public int ID_STATUS { get; set; }
        public int MoneyAll { get; set; }
        public int MoneyFirm { get; set; }
        public int MoneyDetal { get; set; }
        public int MoneyMaster { get; set; }
        public string DescripClose { get; set; }
        public string NameClient { get; set; }
    }
}
