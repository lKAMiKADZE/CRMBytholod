using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRMBytholod.Models.Report
{
    public class PointDiagr_OLD
    {
    }

    public class PointTime
    {
        public int Y { get; set; }
        public DateTime X { get; set; }
    }

    public class PointString
    {
        public int Y { get; set; }
        public string X { get; set; }
    }

    public class PointCircle
    {
        public string Xtitle { get; set; }
        public double Y { get; set; }
    }

}
