using MVC_SYSTEM.App_LocalResources;

namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_SupervisorMember
    {
        [Key]
        public int fld_ID { get; set; }
        [StringLength(20)]
        public string fld_Nopkj { get; set; }
        [StringLength(150)]
        public string fld_Nama { get; set; }
        [StringLength(20)]
        public string fld_SupervisorID { get; set; }
        [StringLength(20)]
        public string fld_JobSpecialization { get; set; }

        public int? fld_NegaraID { get; set; }
        public int? fld_SyarikatID { get; set; }
        public int? fld_WilayahID { get; set; }
        public int? fld_LadangID { get; set; }
        public int? fld_DivisionID { get; set; }
        public DateTime? fld_CreatedDT { get; set; }
        public int? fld_CreatedBy { get; set; }
    }
}

