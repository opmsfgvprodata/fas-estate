using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsDapper
{
    public class Payslip_Result
    {
        public int fldID { get; set; }
        public string fldNopkj { get; set; }
        public string fldKodPkt { get; set; }
        public string fldKod { get; set; }
        public string fldKeterangan { get; set; }
        public Nullable<decimal> fldKuantiti { get; set; }
        public string fldUnit { get; set; }
        public Nullable<decimal> fldKadar { get; set; }
        public Nullable<int> fldGandaan { get; set; }
        public Nullable<decimal> fldJumlah { get; set; }
        public Nullable<int> fldBulan { get; set; }
        public Nullable<int> fldTahun { get; set; }
        public Nullable<int> fldNegaraID { get; set; }
        public Nullable<int> fldSyarikatID { get; set; }
        public Nullable<int> fldWilayahID { get; set; }
        public Nullable<int> fldLadangID { get; set; }
        public Nullable<int> fldFlag { get; set; }
        public Nullable<int> fldFlagIncome { get; set; }
    }
}