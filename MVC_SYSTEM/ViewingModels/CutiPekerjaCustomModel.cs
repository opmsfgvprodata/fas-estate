using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ViewingModels
{
    public class CutiPekerjaCustomModel
    {
        [Key]
        public Guid? fld_ID { get; set; }

        [StringLength(100)]
        public string fld_Desc { get; set; }

        public int? fld_BilanganHari { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_KadarByr { get; set; }

        public decimal? fld_TotalAmount { get; set; }
    }
}