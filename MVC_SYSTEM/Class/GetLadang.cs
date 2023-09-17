using MVC_SYSTEM.MasterModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.Class
{
    public class GetLadang
    {
        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();

        public string GetLadangName(int ladangid, int wlyhID)
        {
            string LadangName = db.tbl_Ladang.Where(x => x.fld_ID == ladangid && x.fld_WlyhID == wlyhID).Select(s => s.fld_LdgName).FirstOrDefault();
            return LadangName;
        }

        public string GetLadangCode(int ladangid)
        {
            string LadangCode = db.tbl_Ladang.Where(x => x.fld_ID == ladangid).Select(s => s.fld_LdgCode).FirstOrDefault();
            return LadangCode;
        }

        public void GetLadangAcc(out string NoAcc, out string NoGL, out string NoCIT, int? ldgid, int? wlyhid)
        {
            var account = db.tbl_Ladang.Where(x => x.fld_WlyhID == wlyhid && x.fld_ID == ldgid).FirstOrDefault();
            NoAcc = account.fld_NoAcc;
            NoGL = account.fld_NoGL;
            NoCIT = account.fld_NoCIT;
        }

        public string GetLadangNegeriCode(int? ladangid)
        {
            string LadangNegeriCode = db.tbl_Ladang.Where(x => x.fld_ID == ladangid).Select(s => s.fld_KodNegeri).FirstOrDefault();
            return LadangNegeriCode;
        }
    }
}