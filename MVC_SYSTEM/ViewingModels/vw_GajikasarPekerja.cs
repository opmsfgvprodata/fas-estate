namespace MVC_SYSTEM.ViewingModels
{

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_GajikasarPekerja
    {

        public tbl_Pkjmast Pkjmast { get; set; }
        [StringLength(20)]
        public string fld_Nopkj { get; set; }

        [StringLength(40)]
        public string fld_Nama { get; set; }
        public string fld_Kdaktf { get; set; }
        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_GajiKasar { get; set; }
        
        [Key]
        public Guid fld_UniqueID { get; set; }
        public string fld_Kodbkl { get; set; }
        public int? fld_Year { get; set; }

        public int? fld_Month { get; set; }


        [Column(TypeName = "date")]
        public DateTime? fld_Trmlkj { get; set; }

        public List<Int32> GajiByBulan { get; set; }

    }
}