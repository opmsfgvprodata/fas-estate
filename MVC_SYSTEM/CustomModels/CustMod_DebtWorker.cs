using MVC_SYSTEM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.CustomModels
{
    public class CustMod_DebtWorker
    {
        public vw_hutangPekerjaLadang Pkjmast { get; set; }
        public tbl_HutangPekerja HutangPekerja { get; set; }

        public tbl_HutangPekerjaJumlah HutangPekerjaJumlah { get; set; }

        public decimal? JumLoan { get; set; }
        public decimal? JumBayar { get; set; }

        public List<tbl_HutangPekerja> MLoan { get; set; }
        public List<tbl_HutangPekerja> HLoan { get; set; }

       
        public List<tbl_Insentif> LoanDeducList { get; set; }
    }
}