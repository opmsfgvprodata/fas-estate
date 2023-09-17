namespace MVC_SYSTEM.MasterModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblSystemConfig
    {
        [Key]
        public int fldConfigID { get; set; }

        [Display(Name = "Content")] // updated by fitri 17-08-2020
        [StringLength(50)]
        public string fldConfigValue { get; set; }

        [Display(Name = "Description")] // updated by fitri 17-08-2020
        [StringLength(100)]
        public string fldConfigDesc { get; set; }

        [StringLength(50)]
        public string fldFlag1 { get; set; }

        [StringLength(50)]
        public string fldFlag2 { get; set; }

        public bool? fldDeleted { get; set; }
    }
}
