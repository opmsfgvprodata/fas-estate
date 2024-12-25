namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_MaybankRcms
    {
        [Key]
        public Guid fld_ID { get; set; }

        [StringLength(15)]
        public string fld_Nokp { get; set; }

        [StringLength(20)]
        public string fld_Nopkj { get; set; }

        [StringLength(100)]
        public string fld_Nama { get; set; }

        [StringLength(50)]
        public string fld_Kdbank { get; set; }

        [StringLength(50)]
        public string fld_NoAkaun { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_GajiBersih { get; set; }

        public int? fld_Year { get; set; }

        public int? fld_Month { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        [StringLength(5)]
        public string fld_LdgCode { get; set; }

        [StringLength(50)]
        public string fld_LdgName { get; set; }

        [StringLength(2)]
        public string fld_Kdrkyt { get; set; }

        [StringLength(15)]
        public string fld_PaymentMode { get; set; }

        [StringLength(50)]
        public string fld_CostCentre { get; set; }

        [StringLength(50)]
        public string fld_NamaBank { get; set; }

        [StringLength(20)]
        public string fld_RcmsBankCode { get; set; }

        [StringLength(39)]
        public string fld_LdgShortName { get; set; }

        [StringLength(2)]
        public string fld_LdgIndicator { get; set; }

        [StringLength(5)]
        public string fld_LdgBusinessArea { get; set; }
    }
}
