using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.CustomModels
{
    public class CustMod_HariTerabai
    {
        [Key]
        public int ID { get; set; }

        public int SelectionCategory { get; set; }

        public string SelectionData { get; set; }

        public DateTime dateseleted { get; set; }

        public string WorkCode { get; set; }

        public int Rainning { get; set; }

        public byte? JnisPktHT { get; set; }

        public string PilihanPktHT { get; set; }

        public string PilihanAktvtHT { get; set; }

        public string JenisChargeHT { get; set; }

        public string PilihanMasaHT { get; set; }

        public string ManagerID { get; set; }

        public string ManagerPassword { get; set; }

        public short atteditstatus { get; set; }

        public string NNCCHT { get; set; }

        public string SelectionNNCCHT { get; set; }

        public string SAPPilihanAktvtHT { get; set; }

        public string JnisAktvtHT { get; set; }
    }
}