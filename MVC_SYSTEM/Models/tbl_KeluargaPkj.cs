namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_KeluargaPkj
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(20)]
        public string fld_Nopkj { get; set; }

        [StringLength(40)]
        public string fld_NamaKeluarga { get; set; }

        [StringLength(30)]
        public string fld_Hubungan { get; set; }

        [StringLength(20)]
        public string fld_NoTel { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? fld_TLahir { get; set; }

        [StringLength(20)]
        public string fld_Permit { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? fld_PermitExprd { get; set; }

        [StringLength(20)]
        public string fld_Pspt { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? fld_PsptExprd { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? fld_InsuransExprd { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool fld_Deleted { get; set; }
    }
}
