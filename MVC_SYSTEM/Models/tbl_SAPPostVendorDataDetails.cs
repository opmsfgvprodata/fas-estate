namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_SAPPostVendorDataDetails
    {
        [Key]
        public Guid fld_ID { get; set; }

        public int? fld_ItemNo { get; set; }

        [StringLength(20)]
        public string fld_VendorNo { get; set; }

        [StringLength(100)]
        public string fld_Desc { get; set; }

        public decimal? fld_Amount { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_BaseDate { get; set; }

        [StringLength(10)]
        public string fld_Currency { get; set; }

        public Guid? fld_SAPPostRefID { get; set; }
    }
}
