namespace MVC_SYSTEM.ViewingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_GajiMinima
    {
        [Key]
        public Guid fld_GajiMinimaID { get; set; }

        [StringLength(20)]
        public string fld_Nopkj { get; set; }

        public short? fld_Year { get; set; }

        public short? fld_Month { get; set; }

        [StringLength(5)]
        public string fld_Sebab { get; set; }

        [StringLength(5)]
        public string fld_Tindakan { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}
