using MVC_SYSTEM.Class;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MVC_SYSTEM.Controllers
{
    public class OPMSHQController : Controller
    {
        private GetIdentity getidentity = new GetIdentity();
        private GetNSWL GetNSWL = new GetNSWL();
        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
        private EncryptDecrypt Encrypt = new EncryptDecrypt();
        private ChangeTimeZone timezone = new ChangeTimeZone();
        // GET: OPMSHQ
        public ActionResult Index()
        {
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int getuserid = getidentity.ID(User.Identity.Name);
            var user = db.tblUsers.Where(u => u.fldUserID == getuserid).SingleOrDefault();
            var getestateselection = db.tbl_EstateSelection.Where(x => x.fld_UserID == getuserid).FirstOrDefault();
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            var routeurl = db.tbl_EstateSelection.Where(x => x.fld_UserID == getuserid).Select(s => s.fld_HQUrl).FirstOrDefault();
            string passwordencrypt = Encrypt.Encrypt(user.fldUserPassword);
            string usernameencrypt = Encrypt.Encrypt(user.fldUserName);
            int day = timezone.gettimezone().Day;
            int month = timezone.gettimezone().Month;
            int year = timezone.gettimezone().Year;
            string code = day.ToString() + month.ToString() + year.ToString();
            code = Encrypt.Encrypt(code);
            
            Response.Cookies.Clear();
            FormsAuthentication.SetAuthCookie(String.Empty, false);
            FormsAuthentication.SignOut();

            string modules = "no";
            routeurl = routeurl + "/IntegrationLogin?TokenID=" + usernameencrypt + "&PassID=" + passwordencrypt + "&Code=" + code + "&Modules=" + modules;
            return Redirect(routeurl);
        }
    }
}