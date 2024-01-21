using MVC_SYSTEM.App_LocalResources; //add by wani 22.9.2020

namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_TaxWorkerInfo
    {
        [Key]
        [StringLength(20)]
        public string fld_NopkjPermanent { get; set; }

        [StringLength(20)]
        public string fld_Nopkj { get; set; }

        //[DataType(DataType.Text)]
        //[RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Please enter a valid text or number.")]
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

        //[DataType(DataType.Text)]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "Please enter a number only")]
        public int? fld_ChildBelow18Full { get; set; }

        //[DataType(DataType.Text)]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "Please enter a number only")]
        public int? fld_ChildBelow18Half { get; set; }

        //[DataType(DataType.Text)]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "Please enter a number only")]
        public int? fld_ChildAbove18CertFull { get; set; }

        public int? fld_ChildAbove18CertHalf { get; set; }

        [DataType(DataType.Text)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Please enter a number only")]
        public int? fld_ChildAbove18HigherFull { get; set; }

        //[DataType(DataType.Text)]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "Please enter a number only")]
        public int? fld_ChildAbove18HigherHalf { get; set; }

        //[DataType(DataType.Text)]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "Please enter a number only")]
        public int? fld_DisabledChildFull { get; set; }

        //[DataType(DataType.Text)]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "Please enter a number only")]
        public int? fld_DisabledChildHalf { get; set; }

        //[DataType(DataType.Text)]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "Please enter a number only")]
        public int? fld_DisabledChildStudyFull { get; set; }

        //[DataType(DataType.Text)]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "Please enter a number only")]
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

    public class tbl_TaxWorkerDetailsList
    {
        public tbl_Pkjmast Pkjmast { get; set; }
        public List<tbl_TaxWorkerInfo> WorkerTax { get; set; }
    }

    public partial class tbl_TaxWorkerInfoViewModelCreate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [StringLength(20)]
        public string fld_NopkjPermanent { get; set; }

        [StringLength(20)]
        public string fld_Nopkj { get; set; }

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

    public partial class tbl_TaxWorkerInfoViewModelEdit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [StringLength(20)]
        public string fld_NopkjPermanent { get; set; }

        [StringLength(20)]
        public string fld_Nopkj { get; set; }

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

        public tbl_Pkjmast Pkjmast { get; set; }

    }
}
