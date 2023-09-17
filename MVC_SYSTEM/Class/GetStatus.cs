using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_SYSTEM.MasterModels;

namespace MVC_SYSTEM.Class
{
    public class GetStatus
    {
        MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
        //Check_Balik
        public string GetStatusApprove(int? id)
        {
            string status = "";
            if (id != null)
            {
                status = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "apprvlPkj" && x.fldOptConfValue == id.ToString()).Select(s => s.fldOptConfDesc).FirstOrDefault();
            }
            else
            {
                status = "";
            }

            return status;
        }

        public string GetWorkerStatus (string dbstatus)
        {
            string statusAktif = "";
            if(dbstatus=="0")
            {
                //statuspkj= aktif
                statusAktif = "1";
            }
            else
            {
                //statuspkj= x aktif
                statusAktif = "2";
            }
            return statusAktif;
        }
    }
}