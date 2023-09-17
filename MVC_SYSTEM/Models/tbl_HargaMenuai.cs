using MVC_SYSTEM.App_LocalResources;

namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_HargaMenuai
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid fld_HargaMenuaiID { get; set; }

        [StringLength(50)]
        public string fld_KodPeringkatUtama { get; set; }

        public decimal? fld_HargaMenuai { get; set; }

        public int? fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }
    }

    public partial class tbl_HargaMenuaiViewModelCreate
    {
        [Key]
        public Guid fld_HargaMenuaiID { get; set; }

        [StringLength(50)]
        public string fld_KodPeringkatUtama { get; set; }

        public string fld_NamaPeringkat { get; set; }

        public string fld_JenisPeringkat { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 9999999.99, ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgMaxCurrencyModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,2})?$", ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_HargaMenuai { get; set; }

        public int fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }
    }

    public partial class tbl_HargaMenuaiViewModelEdit
    {
        [Key]
        public Guid fld_HargaMenuaiID { get; set; }

        [StringLength(50)]
        public string fld_KodPeringkatUtama { get; set; }

        public string fld_NamaPeringkat { get; set; }

        public string fld_JenisPeringkat { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 9999999.99, ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgMaxCurrencyModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,2})?$", ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_HargaMenuai { get; set; }

        public int fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}
