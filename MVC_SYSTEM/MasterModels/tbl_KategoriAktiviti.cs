namespace MVC_SYSTEM.MasterModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_KategoriAktiviti
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(5)]
        public string fld_KodJnsAktvt { get; set; }

        [StringLength(2)]
        public string fld_KodKategori { get; set; }

        [StringLength(2)]
        public string fld_PrefixPkt { get; set; }

        [StringLength(50)]
        public string fld_Kategori { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}
