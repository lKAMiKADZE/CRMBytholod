using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRMBytholod.Models;

namespace CRMBytholod.ViewModels
{
    public class reportZakazMasterVM
    {
        public reportZakazMasterFiltr Filtr { get; set; }
        public List<User> Masters { get; set; }

        public reportZakazMasterVM()
        {
            Filtr = new reportZakazMasterFiltr();
            Masters = User.GetAllMasters();
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
