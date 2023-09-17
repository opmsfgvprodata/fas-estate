using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ViewingModels
{
    public class CustMod_IncrementDataList
    {
        public string fld_Nopkj { get; set; }

        public string fld_Nama { get; set; }

        public decimal? fld_IncrmntSalary { get; set; }

        public decimal? fld_DailyInsentif { get; set; }

        public short StatusGetIncrement { get; set; }
    }
}