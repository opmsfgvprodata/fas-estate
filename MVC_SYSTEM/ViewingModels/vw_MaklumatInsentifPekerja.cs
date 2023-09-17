using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ViewingModels
{
    public class vw_MaklumatInsentifPekerja
    {
        public tbl_Pkjmast Pkjmast { get; set; }
        public List<vw_MaklumatInsentif> Insentif { get; set; }
        public List<vw_MaklumatInsentif> Pendapatan { get; set; }
        public List<vw_MaklumatInsentif> Potongan { get; set; }
    }
}