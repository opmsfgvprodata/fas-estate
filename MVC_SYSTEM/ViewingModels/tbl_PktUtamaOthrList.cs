namespace MVC_SYSTEM.ViewingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tbl_PktUtamaOthr")]
    public partial class tbl_PktUtamaOthrList
    {
        [Key]
        public long fld_PktID { get; set; }

        [StringLength(20)]
        public string fld_CostCentreCode { get; set; }

        [StringLength(20)]
        public string fld_PktCode { get; set; }

        [StringLength(50)]
        public string fld_PktCodeDesc { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Luas { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }

        public int? fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }
    }
}
