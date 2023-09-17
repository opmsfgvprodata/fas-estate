namespace MVC_SYSTEM.ViewingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_MaklumatProduktiviti
    {
        [Key]
        public Guid fld_UniqueID { get; set; }

        [StringLength(20)]
        public string fld_Nopkj { get; set; }

        [StringLength(40)]
        public string fld_Nama { get; set; }

        [StringLength(12)]
        public string fld_Nokp { get; set; }

        public int? fld_KumpulanID { get; set; }

        [StringLength(50)]
        public string fld_KodKumpulan { get; set; }

        [StringLength(50)]
        public string fld_KodKerja { get; set; }

        [StringLength(50)]
        public string fld_Keterangan { get; set; }

        [StringLength(10)]
        public string fld_JenisPelan { get; set; }

        [StringLength(10)]
        public string fld_Unit { get; set; }

        public decimal? fld_Targetharian { get; set; }

        public int? fld_HadirKerja { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        [StringLength(1)]
        public string fld_Kdaktf { get; set; }

        public bool? fld_Deleted { get; set; }

        public int? fld_Year { get; set; }

        public int? fld_Month { get; set; }
    }
}
