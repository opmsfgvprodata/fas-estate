using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.CustomModels
{
    public partial class CustMod_WorkerWork
    {
        [Key]
        public Guid fld_ID { get; set; }
        
        public string fld_Nopkj { get; set; }

        public string fld_NamaPkj { get; set; }
        
        public string fld_Kum { get; set; }
        
        public DateTime? fld_Tarikh { get; set; }
        
        public string fld_KodPkt { get; set; }
        
        public string fld_KodAktvt { get; set; }
        
        public decimal? fld_JumlahHasil { get; set; }
        
        public decimal? fld_Amount { get; set; }

        public decimal? fld_AmountOA { get; set; }

        public decimal? fld_DailyIncentive { get; set; }

        public string fld_KodGL { get; set; }
        
        public string fld_Unit { get; set; }

        public decimal? fld_JamOT { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

    }
}
