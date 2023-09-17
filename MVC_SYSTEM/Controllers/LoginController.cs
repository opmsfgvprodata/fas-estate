using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
//using MVC_SYSTEM.LoginModels;
using MVC_SYSTEM.Security;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using MVC_SYSTEM.App_LocalResources;
//using MVC_SYSTEM.AuthModels;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.Models;

namespace MVC_SYSTEM.Controllers
{
    public class LoginController : Controller
    {
        //private MVC_SYSTEM_Auth db = new MVC_SYSTEM_Auth();
        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
        //private MVC_SYSTEM_Login db2 = new MVC_SYSTEM_Login();
        //private MVC_SYSTEM_Auth db3 = new MVC_SYSTEM_Auth();
        private errorlog geterror = new errorlog();
        private ChangeTimeZone timezone = new ChangeTimeZone();
        private GetNSWL GetNSWL = new GetNSWL();
        private Connection Connection = new Connection();
        private GetConfig getConfig = new GetConfig();

        // GET: Login
        [Localization("bm")]
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Main");
                //return View();
            }
            else
            {
                return View();
            }
        }

        // POST: Login/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AntiForgeryHandleError]
        public ActionResult Index(tblUser Login, string returnUrl)
        {
            string password;
            try
            {
                if (string.IsNullOrEmpty(Login.fldUserName) == false && string.IsNullOrEmpty(Login.fldUserPassword) == false)
                {
                    //getUser user = null;
                    EncryptDecrypt Encrypt = new EncryptDecrypt();
                    password = Encrypt.Encrypt(Login.fldUserPassword);

                    var user = db.tblUsers.Where(u => u.fldUserName == Login.fldUserName.ToUpper() && u.fldUserPassword == password && u.fldDeleted == false).SingleOrDefault();

                    //var estateselection 
                    
                    if (user != null)
                    {
                        if (user.fldNegaraID == 0 || user.fldSyarikatID == 0 || user.fldWilayahID == 0)
                        {
                            var estateselection = db.tbl_EstateSelection.Where(x => x.fld_UserID == user.fldUserID).FirstOrDefault();
                            if (estateselection != null)
                            {
                                JavaScriptSerializer js = new JavaScriptSerializer();
                                string data = js.Serialize(user);
                                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, user.fldUserShortName, timezone.gettimezone(), timezone.gettimezone().Add(FormsAuthentication.Timeout), false, data);
                                string encToken = FormsAuthentication.Encrypt(ticket);
                                HttpCookie authoCookies = new HttpCookie(FormsAuthentication.FormsCookieName, encToken);
                                Response.Cookies.Add(authoCookies);

                                getConfig.AddUserAuditTrail(user.fldUserID, "Login to estate");

                                if (!string.IsNullOrEmpty(returnUrl))
                                {
                                    return Redirect(returnUrl);
                                }
                                else
                                {
                                    if (user.fldRoleID == 1 || user.fldRoleID == 2)
                                    {
                                        return RedirectToAction("Index", "SuperAdminSelection");
                                    }
                                    else
                                    {
                                        // edited by Zaty
                                        //int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
                                        //string host, catalog, user2, pass = "";
                                        //GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, user.fldUserID, user.fldUserName);
                                        //Connection.GetConnection(out host, out catalog, out user2, out pass, WilayahID, SyarikatID, NegaraID);
                                        //MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user2, pass);
                                        var asasldg = db.tbl_Ladang.Where(x => x.fld_WlyhID == estateselection.fld_WilayahID && x.fld_ID == estateselection.fld_LadangID).FirstOrDefault();
                                        if (string.IsNullOrEmpty(asasldg.fld_Pengurus) | string.IsNullOrEmpty(asasldg.fld_Adress) | string.IsNullOrEmpty(asasldg.fld_Tel) | string.IsNullOrEmpty(asasldg.fld_Fax) | string.IsNullOrEmpty(asasldg.fld_LdgEmail))
                                        {
                                            Response.Cache.SetCacheability(HttpCacheability.NoCache);
                                            Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
                                            Response.Cache.SetAllowResponseInBrowserHistory(false);
                                            Response.Cache.SetNoStore();
                                            //return RedirectToAction("_PartialPassword", "Main");
                                            return RedirectToAction("EstateReminder", "BasicInfo");
                                        }
                                        else
                                        {
                                            return RedirectToAction("Index", "Modules");
                                        }

                                    }
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", GlobalResEstate.lblLoginHQMsg);
                            }
                        }
                        else
                        {
                            JavaScriptSerializer js = new JavaScriptSerializer();
                            string data = js.Serialize(user);
                            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, user.fldUserShortName, timezone.gettimezone(), timezone.gettimezone().Add(FormsAuthentication.Timeout), false, data);
                            string encToken = FormsAuthentication.Encrypt(ticket);
                            HttpCookie authoCookies = new HttpCookie(FormsAuthentication.FormsCookieName, encToken);
                            Response.Cookies.Add(authoCookies);

                            getConfig.AddUserAuditTrail(user.fldUserID, "Login to estate");

                            if (!string.IsNullOrEmpty(returnUrl))
                            {
                                return Redirect(returnUrl);
                            }
                            else
                            {
                                if (user.fldRoleID == 1 || user.fldRoleID == 2)
                                {
                                    return RedirectToAction("Index", "SuperAdminSelection");
                                }
                                else
                                {
                                    // edited by Zaty
                                    //int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
                                    //string host, catalog, user2, pass = "";
                                    //GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, user.fldUserID, user.fldUserName);
                                    //Connection.GetConnection(out host, out catalog, out user2, out pass, WilayahID, SyarikatID, NegaraID);
                                    //MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user2, pass);
                                    var asasldg = db.tbl_Ladang.Where(x => x.fld_WlyhID == user.fldWilayahID && x.fld_ID == user.fldLadangID).FirstOrDefault();
                                    if (string.IsNullOrEmpty(asasldg.fld_Pengurus) | string.IsNullOrEmpty(asasldg.fld_Adress) | string.IsNullOrEmpty(asasldg.fld_Tel) | string.IsNullOrEmpty(asasldg.fld_Fax) | string.IsNullOrEmpty(asasldg.fld_LdgEmail))
                                    {
                                        Response.Cache.SetCacheability(HttpCacheability.NoCache);
                                        Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
                                        Response.Cache.SetAllowResponseInBrowserHistory(false);
                                        Response.Cache.SetNoStore();
                                        return RedirectToAction("EstateReminder", "BasicInfo");
                                    }
                                    else
                                    {
                                        return RedirectToAction("Index", "Modules");
                                    }

                                }
                            }
                        }
                        //if(user.fldWilayahID != 0 && user.fldLadangID !=0)
                        //{
                        //    var routeurl = db3.tbl_Wilayah.Where(x => x.fld_SyarikatID == user.fldSyarikatID && x.fld_ID == user.fldWilayahID).Select(s => s.fld_UrlRoute).FirstOrDefault();
                        //    return Redirect(routeurl + "IntegrationLogin?TokenID=" + user.fld_TokenLadangID);
                        //}
                        //else
                        //{
                        
                        //}
                    }
                    else
                    {
                        ModelState.AddModelError("", GlobalResEstate.lblLoginInvalid);
                    }
                }
                else
                {
                    ModelState.AddModelError("", GlobalResEstate.lblLoginMsg);
                }
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                ModelState.AddModelError("", GlobalResEstate.msgError);
                return View();
            }
            return View(Login);
        }
        public ActionResult Logout()
        {
            //try4:
            Response.Cookies.Clear();
            //try5:
            FormsAuthentication.SetAuthCookie(String.Empty, false);
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Login", null);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
