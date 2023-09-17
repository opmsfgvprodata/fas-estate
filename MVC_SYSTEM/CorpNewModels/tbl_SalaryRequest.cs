namespace MVC_SYSTEM.CorpNewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_SalaryRequest
    {
        [Key]
        public int fld_ID { get; set; }

        public Guid? fld_PostingID { get; set; }

        public short? fld_TotalWorker { get; set; }

        public decimal? fld_TotalAmount { get; set; }

        public int? fld_Month { get; set; }

        public int? fld_Year { get; set; }

        public short? fld_Purpose { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public int? fld_DivisionID { get; set; }

        public int? fld_RequestBy { get; set; }

        public DateTime? fld_RequestDT { get; set; }

        public int? fld_ReviewBy { get; set; }

        public bool? fld_ReviewStatus { get; set; }

        public DateTime? fld_ReviewDT { get; set; }
        
        public bool? fld_ApproveStatus { get; set; }

        public int? fld_ApproveBy { get; set; }

        public DateTime? fld_ApproveDT { get; set; }
    }
}
