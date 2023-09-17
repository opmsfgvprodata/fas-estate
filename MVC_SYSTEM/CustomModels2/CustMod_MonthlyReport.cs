using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.CustomModels2
{
    public class CustMod_MonthlyReport
    {
        public long id { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public string costcenter { get; set; }
        public string level { get; set; }
        public string leveldesc { get; set; }
        public string actcode { get; set; }
        public string actdesc { get; set; }
        public string glcode { get; set; }
        public string grp { get; set; }
        public string workerno { get; set; }
        public string workername { get; set; }
        public string active { get; set; }
        public decimal totalval { get; set; }
    }
}