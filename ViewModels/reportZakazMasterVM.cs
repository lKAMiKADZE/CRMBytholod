using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRMBytholod.Models;
using CRMBytholod.Models.Report;

namespace CRMBytholod.ViewModels
{
    public class ReportZakazMasterVM
    {
        public reportZakazMasterFiltr Filtr { get; set; }
        public List<User> Masters { get; set; }

        public List<ReportZakazMaster> reportZakazMasters { get; set; }

        //diagram
        public List<PointTime> DiagramLine_ZakazClose { get; set; }
        public List<PointTime> DiagramLine_Diagnostik { get; set; }
        public List<PointTime> DiagramLine_PovtorMoney { get; set; }
        public List<PointTime> DiagramLine_PovtorNotMoney { get; set; }
        public List<PointTime> DiagramLine_Deny { get; set; }


        public ReportZakazMasterVM()
        {
            Filtr = new reportZakazMasterFiltr();
            Masters = User.GetAllMasters();

            DiagramLine_ZakazClose = ReportZakazMaster.GetDiagramLine_ZakazClose(Filtr);
            DiagramLine_Diagnostik = ReportZakazMaster.GetDiagramLine_Diagnostik(Filtr);
            DiagramLine_PovtorMoney = ReportZakazMaster.GetDiagramLine_PovtorMoney(Filtr);
            DiagramLine_PovtorNotMoney = ReportZakazMaster.GetDiagramLine_PovtorNotMoney(Filtr);
            DiagramLine_Deny = ReportZakazMaster.GetDiagramLine_Deny(Filtr);
        }

        public ReportZakazMasterVM(reportZakazMasterFiltr Filtr)
        {
            this.Filtr = Filtr;
            reportZakazMasters = ReportZakazMaster.GetZakazMasters(Filtr);
            Masters = User.GetAllMasters();

            DiagramLine_ZakazClose = ReportZakazMaster.GetDiagramLine_ZakazClose(Filtr);
            DiagramLine_Diagnostik = ReportZakazMaster.GetDiagramLine_Diagnostik(Filtr);
            DiagramLine_PovtorMoney = ReportZakazMaster.GetDiagramLine_PovtorMoney(Filtr);
            DiagramLine_PovtorNotMoney = ReportZakazMaster.GetDiagramLine_PovtorNotMoney(Filtr);
            DiagramLine_Deny = ReportZakazMaster.GetDiagramLine_Deny(Filtr);
        }

    }


    public class reportZakazMasterFiltr
    {
        public DateTime Start { get; set; }
        private DateTime _end;
        public DateTime End { get { return _end.AddSeconds(1); } set { _end = value; } }
        public GroupByDate  GroupDate { get; set; } // DAY MONTH
        public long ID_MASTER { get; set; }

        public reportZakazMasterFiltr()
        {
            Start = DateTime.Now.AddDays(-30);
            End = DateTime.Now;
            GroupDate = GroupByDate.DAY;
        }

    }



}
