namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_AktvtKerja
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(50)]
        public string fld_Lejar { get; set; }

        [StringLength(50)]
        public string fld_Aktvt { get; set; }

        [StringLength(50)]
        public string fld_Peringkat { get; set; }

        [StringLength(50)]
        public string fld_Kump { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Peratus { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Harga { get; set; }

        [StringLength(1)]
        public string fld_Flag { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }
    }
}
