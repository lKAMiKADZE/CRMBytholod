using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRMBytholod.Models;

namespace CRMBytholod.ViewModels
{
    public class ReportMasterMoneyVM
    {
        public ReportMasterMoneyFiltr Filtr { get; set; }
        public List<User> Masters { get; set; }
        public ReportMasterMoneyVM()
        {
            Filtr = new ReportMasterMoneyFiltr();
            Masters = User.GetAllMasters();
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
