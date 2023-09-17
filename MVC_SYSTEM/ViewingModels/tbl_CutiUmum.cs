namespace MVC_SYSTEM.ViewingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_CutiUmum
    {
        [Key]
        public int fld_CutiUmumID { get; set; }

        [Display(Name = "Keterangan")]
        [Required(ErrorMessage = "Sila Isi Keterangan")]
        [StringLength(50)]
        public string fld_KeteranganCuti { get; set; }

        [Display(Name = "Tarikh Cuti")]
        [Required(ErrorMessage = "Sila Isi Tarikh")]
        [Column(TypeName = "date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? fld_TarikhCuti { get; set; }

        [Display(Name = "Negeri")]
        [Required(ErrorMessage = "Sila Pilih Negeri")]
        public int? fld_Negeri { get; set; }

        public short? fld_Tahun { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}
