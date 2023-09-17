namespace MVC_SYSTEM.ViewingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;

    public class vw_MaklumatTanggungan
    {
        public tbl_MklmtKeluargaPkj MklmtIsteri { get; set; }
        public List<tbl_MklmtKeluargaPkj> MklmtAnak { get; set; }
    }
}
