using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.CustomModels
{
    public partial class CustMod_Kerjahdrgroup
    {
        [Key]
        public int ID { get; set; }

        [StringLength(10)]
        public string fld_KodKumpulan { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_Tarikh { get; set; }
    }
}