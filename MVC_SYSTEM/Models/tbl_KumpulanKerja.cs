using MVC_SYSTEM.App_LocalResources;

namespace MVC_SYSTEM.Models
{
    using MVC_SYSTEM.App_LocalResources;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_KumpulanKerja
    {
        [Key]
        public int fld_KumpulanID { get; set; }

        [StringLength(50)]
        public string fld_KodKumpulan { get; set; }

        [StringLength(50)]
        public string fld_KodKerja { get; set; }

        [StringLength(50)]
        public string fld_Keterangan { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public int? fld_DivisionID { get; set; }

        public bool? fld_deleted { get; set; }

        //Added by Shazana 29/9/2023
        [StringLength(20)]
        public string fld_SupervisorID { get; set; }
        [StringLength(150)]
        public string fld_SupervisorName { get; set; }
        //public DateTime? fld_ModifiedDT { get; set; }
        //public int? fld_ModifiedBy { get; set; }
        //public DateTime? fld_CreatedDT { get; set; }
        //public int? fld_CreatedBy { get; set; }

    }

    public partial class tbl_KumpulanKerjaViewModelCreate
    {
        [Key]
        public int fld_KumpulanID { get; set; }

        [StringLength(50)]
        public string fld_KodKumpulan { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(50)]
        public string fld_KodKerja { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(50)]
        public string fld_Keterangan { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public int? fld_DivisionID { get; set; }

        public bool? fld_deleted { get; set; }

        //Added by Shazana 29/9/2023
        [StringLength(20)]
        public string fld_SupervisorID { get; set; }
        [StringLength(150)]
        public string fld_SupervisorName { get; set; }
        public DateTime? fld_ModifiedDT { get; set; }
        public int? fld_ModifiedBy { get; set; }
        public DateTime? fld_CreatedDT { get; set; }
        public int? fld_CreatedBy { get; set; }
    }

    public partial class tbl_KumpulanKerjaViewModelEdit
    {
        [Key]
        public int fld_KumpulanID { get; set; }

        [StringLength(50)]
        public string fld_KodKumpulan { get; set; }

        [StringLength(50)]
        public string fld_KodKerja { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(50)]
        public string fld_Keterangan { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public int? fld_DivisionID { get; set; }

        public bool? fld_deleted { get; set; }

        //Added by Shazana 29/9/2023
        [StringLength(20)]
        public string fld_SupervisorID { get; set; }
        [StringLength(150)]
        public string fld_SupervisorName { get; set; }
        public DateTime? fld_ModifiedDT { get; set; }
        public int? fld_ModifiedBy { get; set; }
        public DateTime? fld_CreatedDT { get; set; }
        public int? fld_CreatedBy { get; set; }
    }

}
