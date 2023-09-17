namespace MVC_SYSTEM.ViewingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_HasilSawitSubPkt
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int fld_ID { get; set; }

        [StringLength(10)]
        public string fld_Pkt { get; set; }

        [StringLength(10)]
        public string fld_KodPktUtama { get; set; }

        [StringLength(50)]
        public string fld_NamaPkt { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LsPkt { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_HasilTan { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Bulan { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Tahun { get; set; }

        [StringLength(10)]
        public string fld_YieldType { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}
