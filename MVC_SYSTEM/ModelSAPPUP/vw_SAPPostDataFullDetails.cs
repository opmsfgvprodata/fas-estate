namespace MVC_SYSTEM.ModelSAPPUP
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_SAPPostDataFullDetails
    {
        public int? fld_Month { get; set; }

        public int? fld_Year { get; set; }

        [StringLength(12)]
        public string fld_CompCode { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_DocDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_PostingDate { get; set; }

        [StringLength(10)]
        public string fld_DocType { get; set; }

        [StringLength(50)]
        public string fld_RefNo { get; set; }

        [StringLength(12)]
        public string fld_NoDocSAP { get; set; }

        public short? fld_Purpose { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public int? fld_DivisionID { get; set; }

        public bool? fld_StatusProceed { get; set; }

        public int? fld_ItemNo { get; set; }

        [StringLength(12)]
        public string fld_GL { get; set; }

        [StringLength(12)]
        public string fld_Item { get; set; }

        [StringLength(12)]
        public string fld_SAPActivityCode { get; set; }

        public decimal? fld_Amount { get; set; }

        [StringLength(100)]
        public string fld_Desc { get; set; }

        [StringLength(10)]
        public string fld_Currency { get; set; }

        [Key]
        public Guid fld_ID { get; set; }

        [StringLength(50)]
        public string fld_HeaderText { get; set; }

        public Guid? fld_SAPPostRefID { get; set; }
    }
}
