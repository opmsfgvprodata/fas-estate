using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_SYSTEM.Models;

namespace MVC_SYSTEM.CustomModels
{
    public class CustMod_CustSatisfaction
    {
        public int? UID { get; set; }
        public string UIDNama { get; set; }
        public int? LdgID { get; set; }
        public string LdgNama { get; set; }
        public int? Satis { get; set; }
        public string Note { get; set; }
    }
}