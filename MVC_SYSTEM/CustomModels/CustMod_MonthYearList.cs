using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.CustomModels
{
    public partial class CustMod_MonthYearList
    {
        [Key]
        public int ID { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }
    }
}