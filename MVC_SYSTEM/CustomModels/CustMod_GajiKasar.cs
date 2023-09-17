using MVC_SYSTEM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.CustomModels
{
    public class CustMod_GajiKasar
    {
        public tbl_Pkjmast Pkjmast { get; set; }
        public List<tbl_GajiBulanan> GajiBulanan { get; set; }

        public List<tblOptionConfigsWeb> Designation { get; set; }
        //public tblOptionConfigsWeb Designation { get; set; }
    }
}