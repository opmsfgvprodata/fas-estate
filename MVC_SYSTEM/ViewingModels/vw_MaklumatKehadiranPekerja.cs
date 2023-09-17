using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ViewingModels
{
    public class vw_MaklumatKehadiranPekerja
    {
        public tbl_Pkjmast Pkjmast { get; set; }
        public List<Int32> HadirHariBiasaByBulan { get; set; }
        public List<Int32> HadirHariMingguByBulan { get; set; }
        public List<Int32> HadirHariCutiUmumByBulan { get; set; }
    }
}