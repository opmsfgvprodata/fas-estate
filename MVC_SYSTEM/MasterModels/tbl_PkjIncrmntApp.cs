using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.MasterModels
{
    public class tbl_PkjIncrmntApp
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(20)]
        public string fld_Nopkj { get; set; }

        public decimal? fld_IncrmntSalary { get; set; }

        public decimal? fld_DailyInsentif { get; set; }

        public bool? fld_AppStatus { get; set; }

        public int? fld_AppBy { get; set; }

        public DateTime? fld_AppDT { get; set; }

        public int? fld_ReqBy { get; set; }

        public DateTime? fld_ReqDT { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public int? fld_Year { get; set; }

        public int? fld_ProcessStage { get; set; }

        public long? fld_FileID { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}