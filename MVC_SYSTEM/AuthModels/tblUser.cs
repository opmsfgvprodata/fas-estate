namespace MVC_SYSTEM.AuthModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web.Mvc; // Add by fitri 8-12-2020

    // Fitri update 4-8-2020
    public partial class tblUser
    {
        [Key]
        public int fldUserID { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50)]
        [Display(Name = "Username")]
        [Remote("CheckString", "User", HttpMethod = "POST", ErrorMessage = "Username must start with FAS.")] // Add by fitri 8-12-2020
        public string fldUserName { get; set; }

        [StringLength(200)]
        [Display(Name = "Full Name")]
        public string fldUserFullName { get; set; }

        [StringLength(100)]
        [Display(Name = "Short Name")]
        public string fldUserShortName { get; set; }

        [StringLength(50)]
        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail is not valid")]
        [Required(ErrorMessage = "E-mail is required.")]
        [Display(Name = "Email")]
        public string fldUserEmail { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string fldUserPassword { get; set; }

        [Display(Name = "Role")]
        public int? fldRoleID { get; set; }

        [Display(Name = "Group Company")]
        public int? fld_KmplnSyrktID { get; set; }

        [Display(Name = "Country")]
        public int? fldNegaraID { get; set; }

        [Display(Name = "Company")]
        public int? fldSyarikatID { get; set; }

        [Display(Name = "Region")] // edit by fitri 7-12-2020
        public int? fldWilayahID { get; set; }

        [Display(Name = "Estate")] // edit by fitri 7-12-2020
        public int? fldLadangID { get; set; }

        public int? fldFirstTimeLogin { get; set; }

        [Display(Name = "Client")]
        public int? fldClientID { get; set; }

        [StringLength(10)]
        public string fldUserCategory { get; set; }

        [Display(Name = "Status")]
        public bool? fldDeleted { get; set; }

        public int? fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }

        public int? fld_ModifiedBy { get; set; }

        public DateTime? fld_ModifiedDT { get; set; }

        [StringLength(200)]
        public string fld_TokenLadangID { get; set; }
    }

    //fitri 27-7-2020
    [Table("tblUser")]
    public partial class tblUserCreate
    {
        [Key]
        public int fldUserID { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50)]
        [Display(Name = "Username")]
        public string fldUserName { get; set; }

        [StringLength(200)]
        [Display(Name = "Full Name")]
        public string fldUserFullName { get; set; }

        [StringLength(100)]
        [Display(Name = "Short Name")]
        public string fldUserShortName { get; set; }

        [StringLength(50)]
        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail is not valid")]
        [Required(ErrorMessage = "E-mail is required.")]
        [Display(Name = "Email")]
        public string fldUserEmail { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(100)]
        [Display(Name = "Password")]
        public string fldUserPassword { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(100)]
        [Display(Name = "Confirm Password")]
        [System.ComponentModel.DataAnnotations.Compare("fldUserPassword", ErrorMessage = "Confirm password doesn't match, Type again !")] // Edit by fitri 8-12-2020
        public string fldUserConPassword { get; set; }

        [Display(Name = "Role")]
        public int? fldRoleID { get; set; }

        [Display(Name = "Group Company")]
        public int? fld_KmplnSyrktID { get; set; }

        [Display(Name = "Country")]
        public int? fldNegaraID { get; set; }

        [Display(Name = "Company")]
        public int? fldSyarikatID { get; set; }

        [Display(Name = "Wilayah")]
        public int? fldWilayahID { get; set; }

        [Display(Name = "Ladang")]
        public int? fldLadangID { get; set; }

        public int? fldFirstTimeLogin { get; set; }

        [Display(Name = "Client")]
        public int? fldClientID { get; set; }

        [Display(Name = "Status")]
        public bool? fldDeleted { get; set; }

        public int? fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }

        public int? fld_ModifiedBy { get; set; }

        public DateTime? fld_ModifiedDT { get; set; }

        [StringLength(10)]
        public string fldUserCategory { get; set; }

        [StringLength(200)]
        public string fld_TokenLadangID { get; set; }
    }

}
