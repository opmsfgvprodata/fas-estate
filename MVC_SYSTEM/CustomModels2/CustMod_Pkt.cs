using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.CustomModels2
{
    public class CustMod_Pkt
    {
        public int id { get; set; }
        public string wbs { get; set; }
        public string kodpkt { get; set; }
        public string namapkt { get; set; }
        public decimal? luaspkt { get; set; }
        public string editaction { get; set; }
        public string deleteaction { get; set; }
        public string cc { get; set; } //add by fitri 16-02-2021
    }
}