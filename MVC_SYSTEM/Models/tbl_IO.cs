namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_IO
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(50)]
        public string fld_IOcode { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Luas { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LuasKawTnmn { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LuasKawTiadaTnmn { get; set; }

        public int? fld_Status { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}
