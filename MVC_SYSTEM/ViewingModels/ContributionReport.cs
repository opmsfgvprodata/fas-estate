using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ViewingModels
{
    public class ContributionReport
    {
        [Key]
        public int ID { get; set; }

        public string WorkerName { get; set; }

        public string WorkerNo { get; set; }

        public string WorkerIDNo { get; set; }

        //sarah added
        public string WorkerSocsoNo { get; set; }

        public decimal TotalSalaryForKwsp { get; set; }

        public decimal TotalSalaryForPerkeso { get; set; }

        public decimal KwspContributionEmplyee { get; set; }

        public decimal KwspContributionEmplyer { get; set; }

        public decimal SocsoContributionEmplyee { get; set; }

        public decimal SocsoContributionEmplyer { get; set; }

        public decimal SipContributionEmplyee { get; set; }

        public decimal SipContributionEmplyer { get; set; }

        public decimal SbkpContributionEmplyee { get; set; }

        public decimal SbkpContributionEmplyer { get; set; }
        public decimal PcbContributionEmplyee { get; set; }
        public decimal PcbContributionEmplyer { get; set; }
        public decimal NUPWContributionEmplyee { get; set; }
    }
}