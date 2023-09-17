namespace MVC_SYSTEM.ViewingModels
{
    //using AuthModels;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;

    [Table("tbl_Asaspkt")]
    public partial class tbl_Asaspkt
    {
        [Key]
        public Guid fld_UniqueID { get; set; }

        [StringLength(3)]
        public string fld_Ldgcd { get; set; }

        [StringLength(5)]
        public string fld_Pkt { get; set; }

        [StringLength(1)]
        public string fld_Jnstnm { get; set; }

        [StringLength(1)]
        public string fld_Stspkt { get; set; }

        [StringLength(5)]
        public string fld_Pktbgn { get; set; }

        [StringLength(5)]
        public string fld_Pkthsl { get; set; }

        [StringLength(5)]
        public string fld_Pktmtg { get; set; }

        [StringLength(4)]
        public string fld_Prjasl { get; set; }

        [StringLength(6)]
        public string fld_Pktasl { get; set; }

        [StringLength(30)]
        public string fld_Nmpasl { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_Trhbgn { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_Trhhsl { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_Trhmtg { get; set; }

        [StringLength(1)]
        public string fld_Ststs { get; set; }

        [StringLength(1)]
        public string fld_Kdrimv { get; set; }

        [StringLength(8)]
        public string fld_Klon1 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Pklon1 { get; set; }

        [StringLength(8)]
        public string fld_Klon2 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Pklon2 { get; set; }

        [StringLength(8)]
        public string fld_Klon3 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Pklon3 { get; set; }

        [StringLength(8)]
        public string fld_Klon4 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Pklon4 { get; set; }

        [StringLength(8)]
        public string fld_Klon5 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Pklon5 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Lspkt { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Lstnm { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Lshsl { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Lsdsn { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Lskmpg { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Lspaya { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Lsbatu { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Lsltrb { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Lskbr { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Lsklg { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Lslln { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Lstank { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Jlnjkr { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Jlnptn { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Kosbgn { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Bilpok { get; set; }

        [StringLength(1)]
        public string fld_Stssdk { get; set; }

        [StringLength(3)]
        public string fld_Kdklg { get; set; }

        [StringLength(2)]
        public string fld_Ststnh { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Prtluas { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Ktgpok { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Llstsq { get; set; }

        public int? fld_Tahun { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Dec1 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Dec2 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Dec3 { get; set; }

        [StringLength(5)]
        public string fld_String1 { get; set; }

        [StringLength(10)]
        public string fld_String2 { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_Trkdload { get; set; }

        public int? fld_Year { get; set; }

        public int? fld_Month { get; set; }

        [StringLength(50)]
        public string fld_ServicesName { get; set; }

        public int? fld_UploadBy { get; set; }

        public DateTime? fld_UploadDate { get; set; }

        [StringLength(5)]
        public string fld_UploadCdLdg { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public long? fld_ProcessID { get; set; }

        [StringLength(50)]
        public string fld_ASCFileName { get; set; }
    }
}