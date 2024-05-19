namespace MVC_SYSTEM.Models
{
    using MVC_SYSTEM.Class;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_TaxPCB2Form
    {
        [Key]
        public Guid fld_ID { get; set; }

        [StringLength(20)]
        public string fld_Nopkj { get; set; }

        [StringLength(20)]
        public string fld_NopkjPermanent { get; set; }

        public int? fld_Year { get; set; }

        public int? fld_Month { get; set; }

        public Guid? fld_GajiID { get; set; }

        public decimal? fld_PCBAmount { get; set; }

        [StringLength(30)]
        public string fld_PCBReceiptNo { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_PCBReceiptDate { get; set; }

        public decimal? fld_CP38Amount { get; set; }

        [StringLength(30)]
        public string fld_CP38ReceiptNo { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_CP38ReceiptDate { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public int? fld_CretaedBy { get; set; }

        public DateTime? fld_CreatedDate { get; set; }

        public int? fld_ModifiedBy { get; set; }

        public DateTime? fld_ModifiedDate { get; set; }
    }
}
