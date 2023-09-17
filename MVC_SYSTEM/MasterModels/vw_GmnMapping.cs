namespace MVC_SYSTEM.MasterModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_GmnMapping
    {
        public long? bil { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int fld_ID { get; set; }

        [StringLength(5)]
        public string fld_LdgCode { get; set; }

        [StringLength(50)]
        public string fld_LdgName { get; set; }

        [StringLength(15)]
        public string fld_CostCentre { get; set; }

        [StringLength(50)]
        public string fld_Kategori { get; set; }

        [StringLength(2)]
        public string fld_KodKategori { get; set; }

        [StringLength(10)]
        public string fld_KodGL { get; set; }

        [StringLength(10)]
        public string fld_KodAktvt { get; set; }

        [StringLength(150)]
        public string fld_Desc { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}
