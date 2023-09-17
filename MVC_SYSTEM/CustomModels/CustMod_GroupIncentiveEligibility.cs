using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.CustomModels
{
    public class CustMod_GroupIncentiveEligibility
    {
        public string NoPkj { get; set; }
        public string NamaPkj { get; set; }
        public string Designation { get; set; }
        public bool IsEligible { get; set; }
        public string IncentiveDesc { get; set; }
        public string Amount { get; set; }
    }
}