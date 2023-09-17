using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_SYSTEM.Models;

namespace MVC_SYSTEM.ViewingModels
{
    public class vw_MaklumatInsentifKumpulan
    {
        public tbl_KumpulanKerja KumpulanKerja { get; set; }
        public List<tbl_Pkjmast> Pkjmast { get; set; }
    }
}