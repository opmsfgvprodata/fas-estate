using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.CustomModels2
{
    public class CustMod_OthersPay
    {
        public int id { get; set; }
        public string workerno { get; set; }
        public string workername { get; set; }
        public string cc { get; set; }
        public string gl { get; set; }
        public string actcode { get; set; }
        public string actdecs { get; set; }
        public decimal payment { get; set; }
        public string grp { get; set; }
        public int month { get; set; }
        public int year { get; set; }

    }
}