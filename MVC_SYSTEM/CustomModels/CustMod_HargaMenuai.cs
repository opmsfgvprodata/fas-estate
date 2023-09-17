using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.CustomModels
{
    public class CustMod_HargaMenuai
    {
        [Key]
        public Guid? ID { get; set; }

        public string JenisPeringkat { get; set; }

        public string KodPeringkat { get; set; }

        public string NamaPeringkat { get; set; }

        public string HargaMenuai { get; set; }
    }
}