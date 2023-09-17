using MVC_SYSTEM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.CustomModels
{
    public class CustMod_SAPPostingData
    {
        public tbl_SAPPostRef GetSAPPostRef { get; set; }

        public tbl_SAPPostVendorDataDetails GetSAPPostVendorDataDetails { get; set; }

        public List<tbl_SAPPostGLIODataDetails> SAPPostGLIODataDetails { get; set; }
    }
}