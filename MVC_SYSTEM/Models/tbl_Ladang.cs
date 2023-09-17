namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Ladang
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(5)]
        public string fld_LdgCode { get; set; }

        [StringLength(50)]
        public string fld_LdgName { get; set; }

        public int? fld_WlyhID { get; set; }

        [StringLength(100)]
        public string fld_LdgEmail { get; set; }

        public bool? fld_Deleted { get; set; }

        [StringLength(12)]
        public string fld_NoAcc { get; set; }

        [StringLength(10)]
        public string fld_NoGL { get; set; }

        [StringLength(10)]
        public string fld_NoCIT { get; set; }

        [StringLength(50)]
        public string fld_Pengurus { get; set; }

        [StringLength(50)]
        public string fld_Adress { get; set; }

        [StringLength(50)]
        public string fld_Tel { get; set; }

        [StringLength(50)]
        public string fld_Fax { get; set; }

        public int fld_KodNegeri { get; set; }

        [StringLength(50)]
        public string fld_PengurusSblm { get; set; }

        [StringLength(50)]
        public string fld_AdressSblm { get; set; }

        [StringLength(50)]
        public string fld_TelSblm { get; set; }

        [StringLength(50)]
        public string fld_FaxSblm { get; set; }

        [StringLength(100)]
        public string fld_LdgEmailSblm { get; set; }

    }
}
