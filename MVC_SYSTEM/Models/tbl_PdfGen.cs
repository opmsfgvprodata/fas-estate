namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_PdfGen
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid fld_ID { get; set; }

        [StringLength(50)]
        public string fld_Controller { get; set; }

        [StringLength(50)]
        public string fld_Action { get; set; }

        [StringLength(350)]
        public string fld_Param { get; set; }
        
        public string fld_CookiesVal { get; set; }

        public int? fld_UserID { get; set; }
    }
}
