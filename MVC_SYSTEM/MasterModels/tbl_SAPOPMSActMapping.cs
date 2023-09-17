namespace MVC_SYSTEM.MasterModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_SAPOPMSActMapping
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(50)]
        public string fld_SAPActCode { get; set; }

        [StringLength(50)]
        public string fld_OPMSActCode { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }
    }
}
