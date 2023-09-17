using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MVC_SYSTEM.App_LocalResources;

namespace MVC_SYSTEM.CustomModels
{
    public class CustMod_SAPCredential
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        public string SapID { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(100)]
        [DataType(DataType.Password)]
        public string SapPassword { get; set; }

        public string GLtoGLGuid { get; set; }

        public string GLtoGVendorGuid { get; set; }

        public string GLtoGCustomerGuid { get; set; }
    }
}