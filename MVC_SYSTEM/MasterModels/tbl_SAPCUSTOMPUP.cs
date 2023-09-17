namespace MVC_SYSTEM.MasterModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_SAPCUSTOMPUP
    {
        [Key]
        public Guid fld_ID { get; set; }

        [StringLength(4)]
        public string fld_CompanyCode { get; set; }

        [StringLength(10)]
        public string fld_WilayahCode { get; set; }

        [StringLength(10)]
        public string fld_PktUtama { get; set; }

        [StringLength(10)]
        public string fld_Blok { get; set; }

        [StringLength(24)]
        public string fld_JnsTnmn { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LsPktUtama { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_DirianPokok { get; set; }

        [StringLength(24)]
        public string fld_WBSCode { get; set; }

        public int? fld_TahunTnm { get; set; }

        [StringLength(24)]
        public string fld_StatusTnmn { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LuasBerhasil { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_IsSelected { get; set; }

        public bool? fld_Deleted { get; set; }

        [StringLength(50)]
        public string fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }

        public DateTime? fld_ModifiedDt { get; set; }

        [StringLength(50)]
        public string fld_ModifiedBy { get; set; }
    }
}
