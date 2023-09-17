namespace MVC_SYSTEM.ViewingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;

    public partial class vw_MaklumatKeluarga
    {
        public tbl_Pkjmast MklmtPkj { get; set; }
        public List<tbl_KeluargaPkj> MklmtKeluarga { get; set; }
        //public tbl_MklmtKeluargaPkj MklmtWaris { get; set; }
        //public List<vw_MaklumatTanggungan> MklmtTanggungan { get; set; }
    }
}
