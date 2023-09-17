namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_RptAIPS
    {
        [Key]
        [Column(Order = 0)]
        public Guid fld_ID { get; set; }

        [StringLength(10)]
        public string fld_Nopkj { get; set; }

        [StringLength(40)]
        public string fld_Nama { get; set; }

        [StringLength(12)]
        public string fld_Nokp { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int fld_KumpulanID { get; set; }

        [StringLength(50)]
        public string fld_KodKumpulan { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(2)]
        public string fld_Ktgpkj { get; set; }

        public int? fld_Year { get; set; }

        public int? fld_Month { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_TargetProd { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_CapaiProd { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_ProdInsentif { get; set; }

        public short? fld_KuaTarget { get; set; }

        public short? fld_KuaCapai { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_KuaInsentif { get; set; }

        public int? fld_HdrTarget { get; set; }

        public int? fld_HdrCapai { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_HdrInsentif { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_AIPS { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        [StringLength(10)]
        public string fld_JenisPelan { get; set; }
        [StringLength(2)]
        public string fld_Jenispekerja { get; set; }
    }
}
