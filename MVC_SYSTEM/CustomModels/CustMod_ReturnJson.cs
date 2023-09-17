using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MVC_SYSTEM.App_LocalResources;

namespace MVC_SYSTEM.CustomModels
{
    public class CustMod_ReturnJson
    {
        [Key]
        public int ID { get; set; }

        public string TransactionType { get; set; }

        public string Success { get; set; }

        public string Message { get; set; }

        public string Status { get; set; }
    }
}