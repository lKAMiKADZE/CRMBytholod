using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRMBytholod.ViewModels
{
    public class reportZakazVM
    {
        public reportZakazFiltr Filtr { get; set; }

        public reportZakazVM()
        {
            Filtr = new reportZakazFiltr();
        }

    }


    public class reportZakazFiltr
    {
        public DateTime Start { get; set; }
        private DateTime _end;
        public DateTime End { get { return _end.AddSeconds(1); } set { _end = value; } }
        public GroupByDate  GroupDate { get; set; } // DAY MONTH

        public reportZakazFiltr()
        {
            Start = DateTime.Now.AddDays(-30);
            End = DateTime.Now;
            GroupDate = GroupByDate.DAY;
        }

    }

    public enum GroupByDate { DAY, MONTH };


}
