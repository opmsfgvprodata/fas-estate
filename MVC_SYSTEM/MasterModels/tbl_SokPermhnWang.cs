namespace MVC_SYSTEM.MasterModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_SokPermhnWang
    {
        [Key]
        public long fld_ID { get; set; }

        public bool? fld_StsTtpUrsNiaga { get; set; }

        [StringLength(20)]
        public string fld_SkbNo { get; set; }

        public Guid? fld_PostingID { get; set; }

        [StringLength(20)]
        public string fld_NoAcc { get; set; }

        [StringLength(20)]
        public string fld_NoGL { get; set; }

        [StringLength(20)]
        public string fld_NoCIT { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahPermohonan { get; set; }

        //added by faeza 30.09.2021
        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahWorkerNet { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahCash { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahCheque { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahEwallet { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahCdmas { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahLain { get; set; }
        //end added faeza

        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahPDP { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahTT { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahCIT { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahManual { get; set; }

        //added by faeza 30.09.2021
        public int? fld_Verify_By { get; set; }

        public DateTime? fld_Verify_DT { get; set; }

        public int? fld_SemakWil_Status { get; set; }

        public int? fld_SemakWil_By { get; set; }

        public DateTime? fld_SemakWil_DT { get; set; }

        public int? fld_TolakWil_Status { get; set; }

        public int? fld_TolakWil_By { get; set; }

        public DateTime? fld_TolakWil_DT { get; set; }

        public int? fld_SokongWilGM_Status { get; set; }

        public int? fld_SokongWilGM_By { get; set; }

        public DateTime? fld_SokongWilGM_DT { get; set; }

        public int? fld_TolakWilGM_Status { get; set; }

        public int? fld_TolakWilGM_By { get; set; }

        public DateTime? fld_TolakWilGM_DT { get; set; }

        public int? fld_TerimaHQ_Status { get; set; }

        public int? fld_TerimaHQ_By { get; set; }

        public DateTime? fld_TerimaHQ_DT { get; set; }

        public int? fld_TolakHQ_Status { get; set; }

        public int? fld_TolakHQ_By { get; set; }

        public DateTime? fld_TolakHQ_DT { get; set; }

        [StringLength(200)]
        public string fld_Remark { get; set; }

        public int? fld_Year { get; set; }

        public int? fld_Month { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_PenKsrOverall { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_PenKsrPkj { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_PtgKwsp { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_PtgScso { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_PtgLain { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_PenBsh { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_KwspScsoPkj { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_KwspScsoMjkn { get; set; }
    }
}
