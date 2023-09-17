namespace MVC_SYSTEM.ViewingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_TamatPassport
    {
        [Key]
        public Guid fld_ReasonID { get; set; }

        [StringLength(20)]
        public string fld_Nopkj { get; set; }

        [StringLength(40)]
        public string fld_Nama { get; set; }

        [StringLength(20)]
        public string fld_NoPassporPermit { get; set; }

        [StringLength(20)]
        public string fld_KategoriSebab { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_TarikhTamat { get; set; }

        [StringLength(200)]
        public string fld_SebabDesc { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WIlayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}
