namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_SpecialInsentif
    {
        [Key]
        public Guid fld_SpecialInsentifID { get; set; }

        [StringLength(20)]
        public string fld_Nopkj { get; set; }

        [StringLength(5)]
        public string fld_KodInsentif { get; set; }

        public decimal? fld_NilaiInsentif { get; set; }

        public int? fld_Year { get; set; }

        public int? fld_Month { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_KWSPPkj { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_KWSPMjk { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_SocsoPkj { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_SocsoMjk { get; set; }

        public bool? fld_Deleted { get; set; }

        public int? fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }

        public Guid? fld_InsentifID { get; set; }

        public decimal? fld_P { get; set; }

        public decimal? fld_Y { get; set; }

        public decimal? fld_K { get; set; }

        public decimal? fld_Y1 { get; set; }

        public decimal? fld_K1 { get; set; }

        public decimal? fld_Y2 { get; set; }

        public decimal? fld_K2 { get; set; }

        public decimal? fld_Yt { get; set; }

        public decimal? fld_Kt { get; set; }

        public int? fld_n { get; set; }

        public int? fld_n1 { get; set; }

        public decimal? fld_D { get; set; }

        public decimal? fld_S { get; set; }

        public decimal? fld_Du { get; set; }

        public decimal? fld_Su { get; set; }

        public decimal? fld_Q { get; set; }

        public decimal? fld_C { get; set; }

        public decimal? fld_LP { get; set; }

        public decimal? fld_LP1 { get; set; }

        public decimal? fld_M { get; set; }

        public decimal? fld_R { get; set; }

        public decimal? fld_B { get; set; }

        public decimal? fld_Z { get; set; }

        public decimal? fld_X { get; set; }

        public decimal? fld_CarumanPekerjaYearly { get; set; }

        public decimal? fld_CarumanPekerjaNet { get; set; }

        public int? fld_ChildBelow18Full { get; set; }

        public int? fld_ChildBelow18Half { get; set; }

        public int? fld_ChildAbove18CertFull { get; set; }

        public int? fld_ChildAbove18CertHalf { get; set; }

        public int? fld_ChildAbove18HigherFull { get; set; }

        public int? fld_ChildAbove18HigherHalf { get; set; }

        public int? fld_DisabledChildFull { get; set; }

        public int? fld_DisabledChildHalf { get; set; }

        public int? fld_DisabledChildStudyFull { get; set; }

        public int? fld_DisabledChildStudyHalf { get; set; }

        [StringLength(5)]
        public string fld_TaxMaritalStatus { get; set; }

        [StringLength(5)]
        public string fld_IsIndividuOKU { get; set; }

        [StringLength(5)]
        public string fld_IsSpouseOKU { get; set; }

        public decimal? fld_PCBCarumanPekerja { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_GajiKasar { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_GajiBersih { get; set; }

        public DateTime? fld_ProcessDT { get; set; }
    }
}
