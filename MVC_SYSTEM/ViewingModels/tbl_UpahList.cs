namespace MVC_SYSTEM.ViewingModels
{
    //using AuthModels;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;

    [Table("tbl_Upah")]
    public partial class tbl_Upah
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(50)]
        public string fld_Kawasan { get; set; }

        [StringLength(50)]
        public string fld_JnsKerja { get; set; }

        public string fld_Perincian { get; set; }

        [StringLength(50)]
        public string fld_Unit { get; set; }

        public decimal? fld_Harga { get; set; }

        [StringLength(50)]
        public string fld_Produktiviti { get; set; }

        [StringLength(50)]
        public string fld_KodAktvt { get; set; }

        public string fld_Checkroll { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}