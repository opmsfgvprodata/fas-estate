using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.CustomModels
{
    public class CustMod_DailyIncentive
    {
        [Key]
        public int ID { get; set; }

        public string IncentiveCode { get; set; }

        public string IncentiveDesc { get; set; }
    }
}