namespace MVC_SYSTEM.MasterModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_AktivitiNNCCMap
    {
        [Key]
        public Guid fld_ID { get; set; }

        [StringLength(5)]
        public string fld_JenisAktiviti { get; set; }

        [StringLength(50)]
        public string fld_NNCC { get; set; }

        [StringLength(50)]
        public string fld_KodAktivitiSAP { get; set; }

        [StringLength(50)]
        public string fld_KodAktivitiOPMS { get; set; }

        [StringLength(5)]
        public string fld_Flag { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}
