namespace MVC_SYSTEM.MasterModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_JadualUpahGMN
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int fld_ID { get; set; }

        [StringLength(4)]
        public string fld_KodAktvt { get; set; }

        [StringLength(150)]
        public string fld_Desc { get; set; }

        [StringLength(10)]
        public string fld_Unit { get; set; }

        public decimal? fld_Harga { get; set; }

        [StringLength(2)]
        public string fld_KodJenisAktvt { get; set; }

        [StringLength(50)]
        public string JenisAktvt { get; set; }

        [StringLength(2)]
        public string fld_KategoriAktvt { get; set; }

        [StringLength(50)]
        public string fld_Kategori { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }
    }
}
