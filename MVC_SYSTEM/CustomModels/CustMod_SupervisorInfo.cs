using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.CustomModels
{
    public class CustMod_SupervisorInfo
    {

        public int fld_ID { get; set; }
        public string fld_SupervisorID { get; set; }
        public string fld_SupervisorName { get; set; }
        public int? fld_NegaraID { get; set; }
        public int? fld_SyarikatID { get; set; }
        public int? fld_WilayahID { get; set; }
        public int? fld_LadangID { get; set; }
        public int? fld_DivisionID { get; set; }
        public bool? fld_Deleted { get; set; }
        public DateTime? fld_CreatedDT { get; set; }
        public int? fld_CreatedBy { get; set; }

        [StringLength(50)]
        public string fldUserName { get; set; }
        [StringLength(200)]
        public string fldUserFullName { get; set; }
        [StringLength(50)]
        public string fldJawatan { get; set; }
        public int? fldRoleID { get; set; }
        [StringLength(50)]
        public string fld_LdgName { get; set; }

        public int? SupervisorMembersCount { get; set; }
    }
}