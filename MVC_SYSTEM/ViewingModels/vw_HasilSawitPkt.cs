namespace MVC_SYSTEM.ViewingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_HasilSawitPkt
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int fld_ID { get; set; }

        public Guid ID2 { get; set; }

        [StringLength(10)]
        public string fld_PktUtama { get; set; }

        [StringLength(50)]
        public string fld_NamaPktUtama { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LsPktUtama { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }

        public bool? DeleteHasilSawit { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_HasilTan { get; set; }

        //[Column(TypeName = "numeric")]
        //public decimal? fld_TanPHektar { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Bulan { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Tahun { get; set; }

        [StringLength(10)]
        public string fld_YieldType { get; set; }
    }

    [Table("vw_HasilSawitPkt")]
    public partial class vw_HasilSawitPktCreate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int fld_ID { get; set; }

        public Guid ID2 { get; set; }

        [StringLength(10)]
        public string fld_PktUtama { get; set; }

        [StringLength(50)]
        public string fld_NamaPktUtama { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LsPktUtama { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }

        public bool? DeleteHasilSawit { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_HasilTan { get; set; }

        //[Column(TypeName = "numeric")]
        //public decimal? fld_TanPHektar { get; set; }

        [Required]
        [Column(TypeName = "numeric")]
        public decimal? fld_Bulan { get; set; }

        [Required]
        [Column(TypeName = "numeric")]
        public decimal? fld_Tahun { get; set; }

        [StringLength(10)]
        public string fld_YieldType { get; set; }
    }
}
