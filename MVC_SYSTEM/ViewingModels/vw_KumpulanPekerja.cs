namespace MVC_SYSTEM.ViewingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_KumpulanPekerja
    {
        [StringLength(50)]
        public string fld_KodKumpulan { get; set; }

        public int? fld_KumpulanID { get; set; }

        [Key]
        public Guid fld_UniqueID { get; set; }

        [StringLength(20)]
        public string fld_Nopkj { get; set; }

        [StringLength(40)]
        public string fld_Nama { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public int? fld_DivisionID { get; set; }

        [StringLength(50)]
        public string fld_Keterangan { get; set; }

        [StringLength(40)]
        public string Expr1 { get; set; }

        [StringLength(1)]
        public string fld_Kdaktf { get; set; }
    }
}
