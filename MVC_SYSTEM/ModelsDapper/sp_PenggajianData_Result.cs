using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsDapper
{
    public class sp_PenggajianData_Result
    {
        public Nullable<System.Guid> fld_ID { get; set; }
        public string fld_NamaWilayah { get; set; }
        public string fld_KodLadang { get; set; }
        public string fld_NamaLadang { get; set; }
        public Nullable<int> fld_Bulan { get; set; }
        public Nullable<int> fld_Tahun { get; set; }
        public Nullable<int> fld_NegaraID { get; set; }
        public Nullable<int> fld_SyarikatID { get; set; }
        public Nullable<int> fld_WilayahID { get; set; }
        public Nullable<int> fld_LadangID { get; set; }
        public Nullable<int> fld_CreatedBy { get; set; }
        public string fld_KodInsentif { get; set; }
        public decimal? fld_NilaiInsentif { get; set; }
        public Nullable<int> fld_headcount { get; set; }
        public string fld_JenisPenggajian { get; set; }
        public string fld_krkytn { get; set; }
        public Nullable<int> ranking { get; set; }
        public string cost_centre { get; set; }

    }
}