namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_SAPPostReturn
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid fld_ID { get; set; }

        public int? fld_SortNo { get; set; }

        [StringLength(500)]
        public string fld_Msg1 { get; set; }

        [StringLength(500)]
        public string fld_Msg2 { get; set; }

        [StringLength(500)]
        public string fld_Msg3 { get; set; }

        [StringLength(500)]
        public string fld_Msg4 { get; set; }

        [StringLength(500)]
        public string fld_Msg5 { get; set; }

        public Guid? fld_SAPPostRefID { get; set; }
    }
}
