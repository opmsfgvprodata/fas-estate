namespace MVC_SYSTEM.MasterModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_SupervisorInfo
    {
        [Key]
        [StringLength(50)]
        public string fldUserName { get; set; }
        [StringLength(200)]
        public string fldUserFullName { get; set; }
        //[StringLength(50)]
        //public string fldJawatan { get; set; }

        //[StringLength(50)]
        //public string fldUserid { get; set; }

        public int? fldSyarikatID { get; set; }
        public int? fldNegaraID { get; set; }
        public int? fldWilayahID { get; set; }
        public int? fldLadangID { get; set; }
        public bool fldDeleted { get; set; }
        public int? fldRoleID { get; set; }
        [StringLength(50)]
        public string fld_LdgName { get; set; }


    }
}
