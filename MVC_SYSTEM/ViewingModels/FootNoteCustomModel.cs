using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ViewingModels
{
    public class FootNoteCustomModel
    {
        [StringLength(100)]
        public string fld_Desc { get; set; }

        public decimal? fld_Bilangan { get; set; }
    }
}