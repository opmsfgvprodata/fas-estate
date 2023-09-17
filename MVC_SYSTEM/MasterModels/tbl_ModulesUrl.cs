namespace MVC_SYSTEM.MasterModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_ModulesUrl
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(50)]
        public string fld_Module { get; set; }

        [StringLength(100)]
        public string fld_Url { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        [StringLength(10)]
        public string fld_LevelAccess { get; set; }
    }
}
