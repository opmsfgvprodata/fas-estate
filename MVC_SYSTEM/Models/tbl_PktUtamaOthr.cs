using MVC_SYSTEM.App_LocalResources;

namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_PktUtamaOthr
    {
        [Key]
        public long fld_PktID { get; set; }

        [StringLength(20)]
        public string fld_CostCentreCode { get; set; }

        [StringLength(20)]
        public string fld_PktCode { get; set; }

        [StringLength(50)]
        public string fld_PktCodeDesc { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Luas { get; set; }

        [StringLength(50)]
        public string fld_UnitLuas { get; set; }

        [StringLength(100)]
        public string fld_NamaPenyelia { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }

        public int? fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }
    }

    public partial class tbl_PktUtamaOthrViewModelCreate
    {
        [Key]
        public long fld_PktID { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(20)]
        public string fld_CostCentreCode { get; set; }

        [StringLength(20)]
        public string fld_PktCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(50)]
        public string fld_PktCodeDesc { get; set; }

        [Range(0, 9999999.999, ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgMaxDecimalModelValidation1")]
        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,3})?$", ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_Luas { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(50)]
        public string fld_UnitLuas { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(100)]
        public string fld_NamaPenyelia { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }

        public int? fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }
    }

    public partial class tbl_PktUtamaOthrViewModelEdit
    {
        [Key]
        public long fld_PktID { get; set; }

        [StringLength(20)]
        public string fld_CostCentreCode { get; set; }

        [StringLength(20)]
        public string fld_PktCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(50)]
        public string fld_PktCodeDesc { get; set; }

        [Range(0, 9999999.999, ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgMaxDecimalModelValidation1")]
        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,3})?$", ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_Luas { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(50)]
        public string fld_UnitLuas { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(100)]
        public string fld_NamaPenyelia { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }

        public int? fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }
    }
}
