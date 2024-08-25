namespace MVC_SYSTEM.ViewingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_SpecialInsentive
    {
        [Key]
        [Column(Order = 0)]
        public Guid fld_InsentifID { get; set; }

        [StringLength(20)]
        public string fld_Nopkj { get; set; }

        [StringLength(15)]
        public string fld_Nokp { get; set; }

        [StringLength(5)]
        public string fld_KodInsentif { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_NilaiInsentif { get; set; }

        public int? fld_Year { get; set; }

        public int? fld_Month { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public int? fld_DivisionID { get; set; }

        public int? fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }

        [StringLength(100)]
        public string fld_Nama { get; set; }

        [StringLength(15)]
        public string fld_PaymentMode { get; set; }

        [StringLength(4)]
        public string fld_Last4Pan { get; set; }

        [StringLength(15)]
        public string fld_Notel { get; set; }

        public bool? fld_Deleted { get; set; }

        //public bool? fld_InclSecondPayslip { get; set; }

        //added by faeza 18.04.2023
        [StringLength(50)]
        public string fld_Keterangan { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_GajiKasar { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_GajiBersih { get; set; }

    }
}
