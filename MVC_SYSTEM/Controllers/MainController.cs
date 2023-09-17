using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using MVC_SYSTEM.Models;

namespace MVC_SYSTEM.Controllers
{

    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public class MainController : Controller
    {
        // GET: Main
        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
        GetIdentity getidentity = new GetIdentity();
        EncryptDecrypt crypto = new EncryptDecrypt();
        GetNSWL GetNSWL = new GetNSWL();
        GetIdentity GetIdentity = new GetIdentity();

        public ActionResult Index()
        {
            ViewBag.Main = "class = active";
            ViewBag.Dropdown = "dropdown";
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            return View();
        }

        public JsonResult ChangePassword(string oldpswd, string newpswd, string confirmpswd)
        {
            if (!string.IsNullOrEmpty(oldpswd))
            {
                int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
                int? getuserid = GetIdentity.ID(User.Identity.Name);
                GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
                var getdata = db.tblUsers.Where(x=>x.fldUserID==getuserid && x.fldNegaraID==NegaraID && x.fldSyarikatID==SyarikatID && x.fldWilayahID==WilayahID && x.fldLadangID==LadangID).FirstOrDefault();
                string userpswd = crypto.Encrypt(oldpswd);
                if (getdata != null && getdata.fldUserPassword==userpswd)
                {
                    if(!string.IsNullOrEmpty(newpswd) && confirmpswd==newpswd && newpswd != oldpswd)
                    {
                        //var pswdpattern = "((?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%]).{6,20})";
                        var pswdpattern = new Regex(@"((?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,20})");
                        if (pswdpattern.IsMatch(newpswd))
                        {
                            getdata.fldUserPassword = crypto.Encrypt(newpswd);
                            db.Entry(getdata).State = EntityState.Modified;
                            db.SaveChanges();
                            return Json(new { success = true, msg = "Password successfully changed.", status = "success" });
                        }
                        else
                        {
                            return Json(new { success = false, msg = "Password tidak sah.", status = "warning" });
                        }
                    }
                    else
                    {
                        return Json(new { success = false, msg = "Error.", status = "warning" });
                    }
                   
                }
                else
                {
                    return Json(new { success = false, msg = "Please contact IT", status = "warning" });
                }
            }
            else
            {
                return Json(new { success = false, msg = "Please enter your password", status = "warning" });
            }

        }

        public ActionResult pwd()
        {
            return View();
        }

        [HttpPost]
        public JsonResult pwdchnge(string pass, int processType)
        {
            string code = "";
            if (!string.IsNullOrEmpty(pass))
            {
                if (processType==1)
                {
                    code = crypto.Encrypt(pass);
                }
                else
                {
                    code = crypto.Decrypt(pass);
                }
            }
            return Json(code);
        }

        public JsonResult DivisionSelection()
        {
            bool CurDivSelection = false;
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> DivisionSelection = new List<SelectListItem>();
            DivisionSelection = new SelectList(db.vw_NSWL_2.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o => o.fld_DivisionName).Select(s => new SelectListItem { Value = s.fld_DivisionID.ToString(), Text = s.fld_DivisionName }), "Value", "Text").ToList();

            var CheckDivisionSelection = db.tbl_EstateDivisionSelection.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_UserID == getuserid).FirstOrDefault();

            if (CheckDivisionSelection != null)
            {
                CurDivSelection = true;
            }
            else
            {
                CurDivSelection = false;
            }

            db.Dispose();
            return Json(new { DivisionSelection, CurDivSelection });
        }
        
        public JsonResult UpdateDivisionSelection(string DivisionSelection)
        {
            string msg = "";
            string statusmsg = "";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            tbl_EstateDivisionSelection tbl_EstateDivisionSelection = new tbl_EstateDivisionSelection();

            var CheckDivisionSelection = db.tbl_EstateDivisionSelection.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_UserID == getuserid).FirstOrDefault();

            if (CheckDivisionSelection != null)
            {
                CheckDivisionSelection.fld_DivisionID = int.Parse(DivisionSelection);
                db.Entry(CheckDivisionSelection).State = EntityState.Modified;
                db.SaveChanges();
                msg = "Selection Success.";
                statusmsg = "success";
            }
            else
            {
                tbl_EstateDivisionSelection.fld_NegaraID = NegaraID;
                tbl_EstateDivisionSelection.fld_SyarikatID = SyarikatID;
                tbl_EstateDivisionSelection.fld_WilayahID = WilayahID;
                tbl_EstateDivisionSelection.fld_LadangID = LadangID;
                tbl_EstateDivisionSelection.fld_DivisionID = int.Parse(DivisionSelection);
                tbl_EstateDivisionSelection.fld_UserID = getuserid;
                db.tbl_EstateDivisionSelection.Add(tbl_EstateDivisionSelection);
                db.SaveChanges();
                msg = "Selection Success.";
                statusmsg = "success";
            }
            
            db.Dispose();
            return Json(new { msg, statusmsg });
        }
    }
}