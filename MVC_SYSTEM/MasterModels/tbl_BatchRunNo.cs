namespace MVC_SYSTEM.MasterModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_BatchRunNo
    {
        [Key]
        public int fld_BatchID { get; set; }

        public int? fld_BatchRunNo { get; set; }

        public int? fld_BatchRunNo2 { get; set; }

        public int? fld_BatchRunNo3 { get; set; }

        [StringLength(20)]
        public string fld_BatchFlag { get; set; }

        [StringLength(50)]
        public string fld_BatchFlag2 { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }
    }
}
