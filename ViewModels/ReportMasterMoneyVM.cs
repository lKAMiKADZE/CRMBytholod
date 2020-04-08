using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRMBytholod.Models;
using CRMBytholod.Models.Report;

namespace CRMBytholod.ViewModels
{
    public class ReportMasterMoneyVM
    {
        public ReportMasterMoneyFiltr Filtr { get; set; }
        public List<User> Masters { get; set; }

        public List<ReportMasterMoney> reportMasterMoneys { get; set; }


        //diagram
        public List<PointTime> DiagramLine_UpFirma { get; set; }
        public List<PointTime> DiagramLine_UpSucces { get; set; }
        public List<PointTime> DiagramLine_UpDiagnostik { get; set; }
        public List<PointTime> DiagramLine_UpPovtorMoney { get; set; }
        public List<PointTime> DiagramLine_UpMasterNal { get; set; }
        public List<PointTime> DiagramLine_UpMasterNotNal { get; set; }

        public ReportMasterMoneyVM()
        {
            Filtr = new ReportMasterMoneyFiltr();
            Masters = User.GetAllMasters();

            DiagramLine_UpFirma = ReportMasterMoney.GetDiagramLine_UpFirma(Filtr);
            DiagramLine_UpSucces = ReportMasterMoney.GetDiagramLine_UpSucces(Filtr);
            DiagramLine_UpDiagnostik = ReportMasterMoney.GetDiagramLine_UpDiagnostik(Filtr);
            DiagramLine_UpPovtorMoney = ReportMasterMoney.GetDiagramLine_UpPovtorMoney(Filtr);
            DiagramLine_UpMasterNal = ReportMasterMoney.GetDiagramLine_UpMasterNal(Filtr);
            DiagramLine_UpMasterNotNal = ReportMasterMoney.GetDiagramLine_UpMasterNotNal(Filtr);
        }


        public ReportMasterMoneyVM(ReportMasterMoneyFiltr Filtr)
        {
            this.Filtr = Filtr;
            Masters = User.GetAllMasters();
            reportMasterMoneys = ReportMasterMoney.GetMoneyMasters(Filtr);

            DiagramLine_UpFirma = ReportMasterMoney.GetDiagramLine_UpFirma(Filtr);
            DiagramLine_UpSucces = ReportMasterMoney.GetDiagramLine_UpSucces(Filtr);
            DiagramLine_UpDiagnostik = ReportMasterMoney.GetDiagramLine_UpDiagnostik(Filtr);
            DiagramLine_UpPovtorMoney = ReportMasterMoney.GetDiagramLine_UpPovtorMoney(Filtr);
            DiagramLine_UpMasterNal = ReportMasterMoney.GetDiagramLine_UpMasterNal(Filtr);
            DiagramLine_UpMasterNotNal = ReportMasterMoney.GetDiagramLine_UpMasterNotNal(Filtr);
        }
    }


    public class ReportMasterMoneyFiltr
    {
        public DateTime Start { get; set; }
        private DateTime _end;
        public DateTime End { get { return _end.AddSeconds(1); } set { _end = value; } }
        public GroupByDate GroupDate { get; set; } // DAY MONTH
        public long ID_MASTER { get; set; }

        public ReportMasterMoneyFiltr()
        {
            Start = DateTime.Now.AddDays(-30);
            End = DateTime.Now;
            GroupDate = GroupByDate.DAY;
        }

    }
}
