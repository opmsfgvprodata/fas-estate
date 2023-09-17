namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_KerjaSAPData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid fld_ID { get; set; }

        public Guid? fld_KerjaID { get; set; }

        [StringLength(10)]
        public string fld_PaySheetID { get; set; }

        [StringLength(15)]
        public string fld_GLKod { get; set; }

        [StringLength(50)]
        public string fld_IOKod { get; set; }

        [StringLength(50)]
        public string fld_NNCC { get; set; }

        [StringLength(50)]
        public string fld_KodAktivitiSAP { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }
    }
}
