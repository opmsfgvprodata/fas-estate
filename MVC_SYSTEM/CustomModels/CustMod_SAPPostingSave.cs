using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.CustomModels
{
    public class CustMod_SAPPostingSave
    {
        public Guid PostingID { get; set; }
        [Required]
        public string RefNo { get; set; }
        [Required]
        public DateTime PostingDate { get; set; }
        [Required]
        public DateTime InvoiceDate { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Name2 { get; set; }
        [Required]
        public string VendorNo { get; set; }
        [Required]
        public string DescVendor { get; set; }
    }
}