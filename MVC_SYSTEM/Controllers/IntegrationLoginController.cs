using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace MVC_SYSTEM.Controllers
{
    public class IntegrationLoginController : Controller
    {
        private MVC_SYSTEM_MasterModels db2 = new MVC_SYSTEM_MasterModels();
        private errorlog geterror = new errorlog();
        private ChangeTimeZone timezone = new ChangeTimeZone();
        private GetConfig getConfig = new GetConfig();
        private GetIdentity GetIdentity = new GetIdentity();
        private GetNSWL GetNSWL = new GetNSWL();

        // GET: IntegrationLogin
        public ActionResult Index(string TokenID, string PassID, string Code, string Modules)
        {
            Session["TokenID"] = TokenID;
            Session["PassID"] = PassID;
            Session["Code"] = Code;
            Session["Modules"] = Modules;
            return RedirectToAction("VerifiedUser", "IntegrationLogin");
        }

        public ActionResult VerifiedUser()
        {
            EncryptDecrypt Encrypt = new EncryptDecrypt();
            string Encryptusername = Session["TokenID"].ToString();
            string Encryptpassword = Session["PassID"].ToString();
            string Encryptcode = Session["Code"].ToString();
            string Modules = Session["Modules"].ToString();
            string decryptusername = Encrypt.Decrypt(Encryptusername);
            string decryptpassword = Encrypt.Decrypt(Encryptpassword);

            int day = timezone.gettimezone().Day;
            int month = timezone.gettimezone().Month;
            int year = timezone.gettimezone().Year;
            string code = day.ToString() + month.ToString() + year.ToString();
            code = Encrypt.Encrypt(code);
            Encryptcode = Encrypt.Decrypt(Encryptcode);
            Encryptcode = Encrypt.Encrypt(Encryptcode);
            if (Encryptcode == code)
            {
                var user = db2.tblUsers.Where(u => u.fldUserName == decryptusername.ToUpper() && u.fldUserPassword == decryptpassword).SingleOrDefault();
                if (user != null)
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    string data = js.Serialize(user);
                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, user.fldUserShortName, timezone.gettimezone(), timezone.gettimezone().Add(FormsAuthentication.Timeout), false, data);
                    string encToken = FormsAuthentication.Encrypt(ticket);
                    HttpCookie authoCookies = new HttpCookie(FormsAuthentication.FormsCookieName, encToken);
                    Response.Cookies.Add(authoCookies);

                    getConfig.AddUserAuditTrail(user.fldUserID, "Login to estate from HQ");

                    if (Modules == "yes")
                    {
                        return RedirectToAction("Index", "Modules");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Main");
                    }
                }
                else
                {
                    ModelState.AddModelError("", GlobalResEstate.lblLoginInvalid);
                    return RedirectToAction("Index", "Login");
                }
            }
            else
            {
                ModelState.AddModelError("", GlobalResEstate.lblLoginInvalid);
                return RedirectToAction("Index", "Login");
            }
        }
    }
}