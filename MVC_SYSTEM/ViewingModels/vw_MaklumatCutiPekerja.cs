using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ViewingModels
{
    public class vw_MaklumatCutiPekerja
    {
        public tbl_Pkjmast Pkjmast { get; set; }
        //public List<vw_MaklumatCuti> Cuti { get; set; }
        public int? CutiTahunan { get; set; }
        public int? CutiAm { get; set; }
        //public int? HadirHariMinggu { get; set; }
        public int? CutiMingguan { get; set; }
        public int? CutiSakit { get; set; }
        public int? Ponteng { get; set; }
        public List<Int32> CutiTahunByBulan { get; set; }
        public List<Int32> CutiAmByBulan { get; set; }
        //public List<Int32> HadirHariMingguByBulan { get; set; }
        public List<Int32> CutiMingguanByBulan { get; set; }
        public List<Int32> CutiSakitByBulan { get; set; }
        public List<Int32> PontengByBulan { get; set; }
    }
}