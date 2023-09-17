namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    public partial class vw_hutangPekerjaLadang
    {
        public string fld_NoPkj { get; set; }
        public string fld_KodHutang { get; set; }
        public decimal? fld_NilaiHutang { get; set; }
        public string fld_Nama { get; set; }
        public string fld_Nokp { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid fld_HutangPkjID { get; set; }
        public string fld_Kdaktf { get; set; }
        public string fld_Jenispekerja { get; set; }
        public int? fld_NegaraID { get; set; }
        public int? fld_SyarikatID { get; set; }
        public int? fld_WilayahID { get; set; }
        public int fld_LadangID { get; set; }
        public bool? fld_Deleted { get; set; }
    }
}