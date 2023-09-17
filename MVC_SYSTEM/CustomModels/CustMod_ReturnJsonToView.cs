using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.CustomModels
{
    public class CustMod_ReturnJsonToView
    {
        [Key]
        public int ID { get; set; }

        public List<CustMod_ReturnJson> ReturnJsonList { get; set; }

        public string Div { get; set; }

        public string RootUrl { get; set; }

        public string Action { get; set; }

        public string Controller { get; set; }

        public string ParamName1 { get; set; }

        public string ParamValue1 { get; set; }

        public string ParamName2 { get; set; }

        public string ParamValue2 { get; set; }
    }
}