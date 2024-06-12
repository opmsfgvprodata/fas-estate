namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class vw_SupervisorMembersInfo
    {

        [Key]
        public string fld_Nopkj { get; set; }
        public string fld_Nama { get; set; }
        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }
        public int? fld_DivisionID { get; set; }
        public string fld_Kdaktf { get; set; }
        public string fld_Ktgpkj { get; set; }
        public string fldOptConfDesc { get; set; }
        public string fldOptConfValue { get; set; }
    }
}
