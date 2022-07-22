using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDota_UWP.Models
{
    //public class DotaMatchPlayerTotalsModel
    //{
    //    public List<DotaMatchPlayerTotalModel> vTotals { get; set; }
    //}

    public class DotaMatchPlayerTotalModel
    {
        public string field { get; set; }
        public int n { get; set; }
        public float sum { get; set; }
    }

}
