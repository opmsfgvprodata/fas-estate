using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.CustomModels
{
    public partial class CustMod_LprnBlnnByrnAktvt
    {
        [Key]
        public long fld_ID { get; set; }

        public string fld_CC { get; set; }

        public short fld_LevelType { get; set; }

        public string fld_LevelCode { get; set; }

        public string fld_LevelName { get; set; }

        public string fld_WorkerNo { get; set; }

        public string fld_WorkerName { get; set; }

        public int fld_Month { get; set; }

        public int fld_Year { get; set; }

        public decimal fld_Amount { get; set; }

        public int fld_NegaraID { get; set; }

        public int fld_SyarikatID { get; set; }

        public int fld_WilayahID { get; set; }

        public int fld_LadangID { get; set; }

        public string fld_KumpulanID { get; set; }

        public string fld_KodAktvt { get; set; }

        public string fld_KodGL { get; set; }

        public string levelmaincode { get; set; }

    }
}