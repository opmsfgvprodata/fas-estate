using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.CustomModels2
{
    public class CustMod_RptActvByLvlWorkerNActvitList
    {
        public List<CustMod_ActvtList> CustMod_ActvtList { get; set; }
        public List<CustMod_RptMnthSpendByActvt> CustMod_RptMnthSpendByActvt { get; set; }
    }
}