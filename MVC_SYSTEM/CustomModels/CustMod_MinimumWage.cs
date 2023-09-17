using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_SYSTEM.ViewingModels;

namespace MVC_SYSTEM.CustomModels
{
    public class CustMod_MinimumWage
    {
        public Guid IDPkj { get; set; }
        public string NoPkj { get; set; }
        public string Nama { get; set; }
        public string Warganegara { get; set; }
        public string Nokp { get; set; }
        public DateTime? TarikhSahJawatan { get; set; }
        public string KategoriKerja { get; set; }
        public int? JumlahHariTawaranKerja { get; set; }
        public int? JumlahHariBekerja { get; set; }
        public decimal? GajiBulanan { get; set; }
        public Guid? IDSebab { get; set; }
        public string Sebab { get; set; }
        public string PelanTindakan { get; set; }
        public int? NegaraID { get; set; }
        public int? SyarikatID { get; set; }
    }
}