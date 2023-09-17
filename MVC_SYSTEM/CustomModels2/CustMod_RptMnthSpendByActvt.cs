using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.CustomModels2
{
    public class CustMod_RptMnthSpendByActvt
    {
        [Key]
        public long id { get; set; }
        public string costcenter { get; set; }
        public string actvtcode { get; set; }
        public string actvtdesc { get; set; }
        public decimal totalspend { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public int fld_NegaraID { get; set; }
        public int fld_SyarikatID { get; set; }
        public int fld_WilayahID { get; set; }
        public int fld_LadangID { get; set; }
    }
}