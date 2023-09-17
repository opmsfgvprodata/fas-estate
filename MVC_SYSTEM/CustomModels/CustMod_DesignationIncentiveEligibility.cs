using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_SYSTEM.MasterModels;

namespace MVC_SYSTEM.CustomModels
{
    public class CustMod_DesignationIncentiveEligibility
    {
        public tbl_JenisInsentif JenisInsentif { get; set; }
        public List<tblOptionConfigsWeb> Designation { get; set; }
    }
}