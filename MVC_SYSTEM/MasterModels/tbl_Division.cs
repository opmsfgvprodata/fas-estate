namespace MVC_SYSTEM.MasterModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Division
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(50)]
        public string fld_DivisionSAPCode { get; set; }

        [StringLength(50)]
        public string fld_DivisionName { get; set; }

        [StringLength(10)]
        public string fld_JnsLot { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}
