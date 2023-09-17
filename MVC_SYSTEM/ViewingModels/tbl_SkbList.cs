namespace MVC_SYSTEM.ViewingModels
{
    //using AuthModels;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;

    [Table("tbl_Skb")]
    public partial class tbl_Skb
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(2)]
        public string fld_Bulan { get; set; }

        public int? fld_Tahun { get; set; }

        public decimal? fld_GajiBersih { get; set; }

        [StringLength(50)]
        public string fld_NoSkb { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}