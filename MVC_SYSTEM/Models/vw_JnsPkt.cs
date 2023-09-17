namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_JnsPkt
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int fld_ID { get; set; }

        [StringLength(50)]
        public string fld_IOcode { get; set; }

        [StringLength(10)]
        public string fld_PktUtama { get; set; }

        [StringLength(50)]
        public string fld_NamaPktUtama { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LsPktUtama { get; set; }

        [StringLength(1)]
        public string fld_JnsTnmn { get; set; }

        [StringLength(1)]
        public string fld_StatusTnmn { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LuasKawTnman { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LuasBerhasil { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LuasBlmBerhasil { get; set; }

        public int? fld_BilPokok { get; set; }

        public int? fld_DirianPokok { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LuasKawTiadaTanaman { get; set; }

        [StringLength(10)]
        public string fld_KesukaranMenuaiPktUtama { get; set; }

        [StringLength(10)]
        public string fld_KesukaranMembajaPktUtama { get; set; }

        [StringLength(10)]
        public string fld_Pkt { get; set; }

        [StringLength(50)]
        public string fld_NamaPkt { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LsPkt { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LuasKawTnmanPkt { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LuasBerhasilPkt { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LuasBlmBerhasilPkt { get; set; }

        public int? fld_BilPokokPkt { get; set; }

        public int? fld_DirianPokokPkt { get; set; }

        [StringLength(10)]
        public string fld_KesukaranMenuaiPkt { get; set; }

        [StringLength(10)]
        public string fld_KesukaranMembajaPkt { get; set; }

        [StringLength(10)]
        public string fld_Blok { get; set; }

        [StringLength(50)]
        public string fld_NamaBlok { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LsBlok { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LuasKawTnmanBlok { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LuasBerhasilBlok { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LuasBlmBerhasilBlok { get; set; }

        public int? fld_BilPokokBlok { get; set; }

        public int? fld_DirianPokokBlok { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LuasKawTiadaTanamanBlok { get; set; }

        [StringLength(10)]
        public string fld_KesukaranMenuaiBlok { get; set; }

        [StringLength(10)]
        public string fld_KesukaranMembajaBlok { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public int? fld_NegaraID { get; set; }
       
    }
}
