using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;


namespace MVC_SYSTEM.ViewingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_TaxWorkerInfo
    {

        public tbl_Pkjmast Pkjmast { get; set; }

        [StringLength(20)]
        public string fld_NopkjPermanent { get; set; }

        [StringLength(20)]
        public string nopkjPermanent { get; set; }

        [StringLength(20)]
        public string fld_Nopkj { get; set; }

        [StringLength(20)]
        public string noPkj { get; set; }

        [StringLength(100)]
        public string fld_Nama { get; set; }

        [StringLength(1)]
        public string fld_Kdaktf { get; set; }

        [DataType(DataType.Text)]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Please enter a valid text or number.")]
        [StringLength(30)]
        public string fld_TaxNo { get; set; }

        public int? fld_Year { get; set; }

        [StringLength(5)]
        public string fld_TaxResidency { get; set; }

        [StringLength(5)]
        public string fld_TaxMaritalStatus { get; set; }

        [StringLength(5)]
        public string fld_IsIndividuOKU { get; set; }

        [StringLength(5)]
        public string fld_IsSpouseOKU { get; set; }

        [DataType(DataType.Text)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Please enter a number only")]
        public int? fld_ChildBelow18Full { get; set; }

        [DataType(DataType.Text)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Please enter a number only")]
        public int? fld_ChildBelow18Half { get; set; }

        [DataType(DataType.Text)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Please enter a number only")]
        public int? fld_ChildAbove18CertFull { get; set; }

        [DataType(DataType.Text)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Please enter a number only")]
        public int? fld_ChildAbove18CertHalf { get; set; }

        [DataType(DataType.Text)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Please enter a number only")]
        public int? fld_ChildAbove18HigherFull { get; set; }

        [DataType(DataType.Text)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Please enter a number only")]
        public int? fld_ChildAbove18HigherHalf { get; set; }

        [DataType(DataType.Text)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Please enter a number only")]
        public int? fld_DisabledChildFull { get; set; }

        [DataType(DataType.Text)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Please enter a number only")]
        public int? fld_DisabledChildHalf { get; set; }

        [DataType(DataType.Text)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Please enter a number only")]
        public int? fld_DisabledChildStudyFull { get; set; }

        [DataType(DataType.Text)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Please enter a number only")]
        public int? fld_DisabledChildStudyHalf { get; set; }

        public DateTime? fld_CreatedDate { get; set; }

        public int? fld_CreatedBy { get; set; }

        public DateTime? fld_ModifiedDate { get; set; }

        public int? fld_ModifiedBy { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public int? fld_DivisionID { get; set; }
    }
}
