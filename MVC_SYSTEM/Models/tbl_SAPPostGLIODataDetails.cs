namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_SAPPostGLIODataDetails
    {
        [Key]
        public Guid fld_ID { get; set; }

        public int? fld_ItemNo { get; set; }

        [StringLength(12)]
        public string fld_GL { get; set; }

        [StringLength(20)]
        public string fld_IO { get; set; }

        public decimal? fld_Amount { get; set; }

        [StringLength(100)]
        public string fld_Desc { get; set; }

        [StringLength(10)]
        public string fld_TaxCode { get; set; }

        [StringLength(10)]
        public string fld_Currency { get; set; }

        public Guid? fld_SAPPostRefID { get; set; }
    }
}
