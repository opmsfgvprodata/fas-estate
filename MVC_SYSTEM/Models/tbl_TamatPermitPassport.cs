using MVC_SYSTEM.App_LocalResources;

namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_TamatPermitPassport
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid fld_ReasonID { get; set; }

        [StringLength(20)]
        public string fld_Nopkj { get; set; }

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

    public partial class tbl_TamatPermitPassportViewModelCreate
    {
        [Key]
        public Guid fld_ReasonID { get; set; }

        [StringLength(20)]
        public string fld_Nopkj { get; set; }

        [StringLength(20)]
        public string fld_NoPassporPermit { get; set; }

        [StringLength(20)]
        public string fld_KategoriSebab { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_TarikhTamat { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [MaxLength(200, ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidationMaxWordLEngth")]
        public string fld_SebabDesc { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WIlayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }
    }

    public partial class tbl_TamatPermitPassportViewModelEdit
    {
        [Key]
        public Guid fld_ReasonID { get; set; }

        [StringLength(20)]
        public string fld_Nopkj { get; set; }

        [StringLength(20)]
        public string fld_NoPassporPermit { get; set; }

        [StringLength(20)]
        public string fld_KategoriSebab { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_TarikhTamat { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [MaxLength(200, ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidationMaxWordLEngth")]
        public string fld_SebabDesc { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WIlayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }
    }

    public partial class tbl_TamatPermitPassportViewModelList
    {
        [Key]
        public Guid fld_ReasonID { get; set; }

        [StringLength(20)]
        public string fld_Nopkj { get; set; }

        [StringLength(20)]
        public string fld_NoPassporPermit { get; set; }

        [StringLength(20)]
        public string fld_KategoriSebab { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_TarikhTamat { get; set; }

        public string fld_SebabDesc { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WIlayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }

        public bool? fld_IsExist { get; set; }
    }
}
