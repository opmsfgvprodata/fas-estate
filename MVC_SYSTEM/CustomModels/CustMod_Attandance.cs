using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.CustomModels
{
    public partial class CustMod_Attandance
    {
        [Key]
        public int ID { get; set; }

        public int SelectionCategory { get; set; }

        public string SelectionData { get; set; }

        public DateTime dateseleted { get; set; }

        public string WorkCode { get; set; }

        public int Rainning { get; set; }

        public short atteditstatus { get; set; }

        public int Division { get; set; }

        public string TimeIn { get; set; }

        public string TimeOut { get; set; }
    }

    public class timefilter
    {
        public int id { get; set; }
        public string time { get; set; }
    }
}