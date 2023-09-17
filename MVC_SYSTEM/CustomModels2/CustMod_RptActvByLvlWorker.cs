using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.CustomModels2
{
    public class CustMod_RptActvByLvlWorker
    {
        [Key]
        public long id { get; set; }
        public string lvlcode { get; set; }
        public string lvldesc { get; set; }
        public string costcenter { get; set; }
        public string workerno { get; set; }
        public string workername { get; set; }
        public decimal kong { get; set; }
        public decimal ottime { get; set; }
        public decimal otpayment { get; set; }
        public decimal piecereatepay { get; set; }
        public decimal dailyincentivepay { get; set; }
        public decimal iphspay { get; set; }
        public decimal additionalpay { get; set; }
        public decimal otherincome { get; set; }
        public decimal totalincome { get; set; }
        public decimal totaldeduction { get; set; }
        public decimal overallincome { get; set; }
        public short leveltype { get; set; }
        public int fld_NegaraID { get; set; }
        public int fld_SyarikatID { get; set; }
        public int fld_WilayahID { get; set; }
        public int fld_LadangID { get; set; }
    }
}