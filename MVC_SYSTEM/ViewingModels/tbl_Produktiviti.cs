namespace MVC_SYSTEM.ViewingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Produktiviti
    {
        [Key]
        public Guid fld_ProduktivitifID { get; set; }

        [StringLength(20)]
        public string fld_Nopkj { get; set; }

        [StringLength(10)]
        public string fld_JenisPelan { get; set; }

        public decimal? fld_Targetharian { get; set; }

        [StringLength(10)]
        public string fld_Unit { get; set; }

        public int? fld_HadirKerja { get; set; }

        public int? fld_Year { get; set; }

        public int? fld_Month { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }

        public int? fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }
    }
}
