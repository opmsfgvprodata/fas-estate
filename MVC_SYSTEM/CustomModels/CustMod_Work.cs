using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.CustomModels
{
    public partial class CustMod_Work
    {
        [Key]
        public int ID { get; set; }

        public string nopkj { get; set; }

        public decimal? kadar { get; set; }

        public byte? gandaankadar { get; set; }

        public decimal? hasil { get; set; }

        public decimal? jumlah { get; set; }

        public decimal? jumlahOA { get; set; }

        public short? kualiti { get; set; }

        public byte? bonus { get; set; }

        public decimal? bonus2 { get; set; }

        public decimal? ot { get; set; }

        public string kdhmnuai { get; set; }

        public short? checkpurpose { get; set; }

        public string incentivecode { get; set; }

        public decimal? incentiveval { get; set; }

        public bool? savethis { get; set; }

        public string sapgl { get; set; }
    }
}