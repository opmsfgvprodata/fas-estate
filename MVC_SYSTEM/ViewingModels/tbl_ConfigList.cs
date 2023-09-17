namespace MVC_SYSTEM.ViewingModels
{
    //using AuthModels;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;

    [Table("tblOptionConfigsWeb")]
    public partial class tblOptionConfigsWeb
    {
        [Key]
        public int fldOptConfID { get; set; }

        [Required]
        [StringLength(50)]
        public string fldOptConfValue { get; set; }

        [Required]
        [StringLength(50)]
        public string fldOptConfDesc { get; set; }

        [Required]
        [StringLength(50)]
        public string fldOptConfFlag1 { get; set; }

        [StringLength(50)]
        public string fldOptConfFlag2 { get; set; }

        public bool? fldDeleted { get; set; }

        [StringLength(50)]
        public string fldOptConfFlag3 { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }
    }
}