namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_KerjaInfoDetails2
    {
        [Key]
        public Guid fld_ID { get; set; }

        [StringLength(100)]
        public string fld_Nama { get; set; }

        [StringLength(2)]
        public string fld_JnisAktvt { get; set; }

        [StringLength(15)]
        public string fld_GLKod { get; set; }

        [StringLength(4)]
        public string fld_KodAktvt { get; set; }

        [StringLength(10)]
        public string fld_Unit { get; set; }

        public byte? fld_JnsPkt { get; set; }

        [StringLength(10)]
        public string fld_KodPkt { get; set; }

        [StringLength(5)]
        public string fld_KodGL { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_KadarByr { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahHasil { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Amount { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_JamOT { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_KadarOT { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahOT { get; set; }

        public byte? fld_Bonus { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_KadarBonus { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahBonus { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_OverallAmount { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_HrgaKwsnSkar { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_Tarikh { get; set; }

        [StringLength(20)]
        public string fld_Nopkj { get; set; }

        [StringLength(50)]
        public string fld_KodSAPPekerja { get; set; }

        [StringLength(50)]
        public string fld_Kum { get; set; }
    }
}
