using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq.Dynamic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.ViewingModels;
using MVC_SYSTEM.Security;
//using MVC_SYSTEM.ConfigModels;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.App_LocalResources; // Add by fitri 7-12-2020

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Admin 1")]
    public class UserController : Controller
    {
        private GetConfig getconfig = new GetConfig();
        private GetIdentity getidentity = new GetIdentity();
        private errorlog geterror = new errorlog();
        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
        //private MVC_SYSTEM_Config db2 = new MVC_SYSTEM_Config();
        //private MVC_SYSTEM_Models db3 = new MVC_SYSTEM_Models();
        private EncryptDecrypt crypto = new EncryptDecrypt();
        private GetNSWL GetNSWL = new GetNSWL();
        private GetConfig Getconfig = new GetConfig();
        private GetWilayah GetWilayah = new GetWilayah();
        private ChangeTimeZone timezone = new ChangeTimeZone();

        // GET: User
        // fitri 24072020
        public ActionResult Index(string filter, int page = 1, string sort = "fld_BlokStatus", string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.User = "class = active";
            return View();
        }
        // fitri 24072020
        public ActionResult _User(string filter = "", int fldUserID = 0, int page = 1, string sort = "fldUserName", string sortdir = "DESC")
        {
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            MVC_SYSTEM_Viewing dbview = new MVC_SYSTEM_Viewing();
            ViewBag.User = "class = active";
            ViewBag.Dropdown2 = "dropdown open active";
            int pageSize = int.Parse(getconfig.GetData("paging"));
            var records = new PagedList<ViewingModels.tblUser>();
            ViewBag.filter = filter;

            if (WilayahID == 0 && LadangID == 0)
            {
                if (filter == "")
                {
                    if (getidentity.MySuperAdmin(User.Identity.Name))
                    {
                        records.Content = dbview.tblUsers
                            .Where(x => x.fldUserID != getuserid)
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

                        records.TotalRecords = dbview.tblUsers.Count();
                        records.CurrentPage = page;
                        records.PageSize = pageSize;
                    }
                    else
                    {
                        records.Content = dbview.tblUsers
                            .Where(x => x.fldRoleID > 2 && x.fldRoleID != 9 && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID && x.fldUserID != getuserid)
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

                        records.TotalRecords = dbview.tblUsers.Where(x => x.fldRoleID > 2 && x.fldRoleID != 9 && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID).Count();
                        records.CurrentPage = page;
                        records.PageSize = pageSize;
                    }
                }
                else
                {
                    if (getidentity.MySuperAdmin(User.Identity.Name))
                    {
                        records.Content = dbview.tblUsers.Where(x => x.fldUserFullName.Contains(filter) || x.fldUserName.Contains(filter) && x.fldUserID != getuserid)
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                        records.TotalRecords = dbview.tblUsers.Where(x => x.fldUserFullName.Contains(filter) || x.fldUserName.Contains(filter)).Count();
                        records.CurrentPage = page;
                        records.PageSize = pageSize;
                    }
                    else
                    {
                        records.Content = dbview.tblUsers.Where(x => (x.fldUserFullName.Contains(filter) || x.fldUserName.Contains(filter)) && (x.fldRoleID > 2 && x.fldRoleID != 9 && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID) && x.fldUserID != getuserid)
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                        records.TotalRecords = dbview.tblUsers.Where(x => (x.fldUserFullName.Contains(filter) || x.fldUserName.Contains(filter)) && (x.fldRoleID > 2 && x.fldRoleID != 9 && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID)).Count();
                        records.CurrentPage = page;
                        records.PageSize = pageSize;
                    }
                }

            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                records.Content = dbview.tblUsers.Where(x => (x.fldUserFullName.Contains(filter) || x.fldUserName.Contains(filter)) && (x.fldRoleID > 2 && x.fldRoleID != 9 && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID && x.fldWilayahID == WilayahID))
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                records.TotalRecords = dbview.tblUsers.Where(x => (x.fldUserFullName.Contains(filter) || x.fldUserName.Contains(filter)) && (x.fldRoleID > 2 && x.fldRoleID != 9 && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID && x.fldWilayahID == WilayahID)).Count();
                records.CurrentPage = page;
                records.PageSize = pageSize;
            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                records.Content = dbview.tblUsers.Where(x => (x.fldUserFullName.Contains(filter) || x.fldUserName.Contains(filter)) && (x.fldRoleID > 2 && x.fldRoleID != 9 && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID && x.fldWilayahID == WilayahID && x.fldLadangID == LadangID))
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                records.TotalRecords = dbview.tblUsers.Where(x => (x.fldUserFullName.Contains(filter) || x.fldUserName.Contains(filter)) && (x.fldRoleID > 2 && x.fldRoleID != 9 && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID && x.fldWilayahID == WilayahID && x.fldLadangID == LadangID)).Count();
                records.CurrentPage = page;
                records.PageSize = pageSize;
            }

            return View(records);
        }

        // fitri comment 24072020
        /*public ActionResult Index(string filter = "", int fldUserID = 0, int page = 1, string sort = "fldUserName", string sortdir = "DESC")
        {
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            MVC_SYSTEM_Viewing dbview = new MVC_SYSTEM_Viewing();
            ViewBag.User = "class = active";
            ViewBag.Dropdown2 = "dropdown open active";
            int pageSize = int.Parse(getconfig.GetData("paging"));
            var records = new PagedList<ViewingModels.tblUser>();
            ViewBag.filter = filter;

            if (WilayahID == 0 && LadangID == 0)
            {
                if (filter == "")
                {
                    if (getidentity.MySuperAdmin(User.Identity.Name))
                    {
                        records.Content = dbview.tblUsers
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

                        records.TotalRecords = dbview.tblUsers.Count();
                        records.CurrentPage = page;
                        records.PageSize = pageSize;
                    }
                    else
                    {
                        records.Content = dbview.tblUsers.Where(x => x.fldRoleID > 2 && x.fldRoleID != 9 && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID)
                            .OrderBy(sort + " " + sortdir)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

                        records.TotalRecords = dbview.tblUsers.Where(x => x.fldRoleID > 2 && x.fldRoleID != 9 && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID).Count();
                        records.CurrentPage = page;
                        records.PageSize = pageSize;
                    }
                }
                else
                {
                    if (getidentity.MySuperAdmin(User.Identity.Name))
                    {
                        records.Content = dbview.tblUsers.Where(x => x.fldUserFullName.Contains(filter) || x.fldUserName.Contains(filter))
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                        records.TotalRecords = dbview.tblUsers.Where(x => x.fldUserFullName.Contains(filter) || x.fldUserName.Contains(filter)).Count();
                        records.CurrentPage = page;
                        records.PageSize = pageSize;
                    }
                    else
                    {
                        records.Content = dbview.tblUsers.Where(x => (x.fldUserFullName.Contains(filter) || x.fldUserName.Contains(filter)) && (x.fldRoleID > 2 && x.fldRoleID != 9 && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID))
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                        records.TotalRecords = dbview.tblUsers.Where(x => (x.fldUserFullName.Contains(filter) || x.fldUserName.Contains(filter)) && (x.fldRoleID > 2 && x.fldRoleID != 9 && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID)).Count();
                        records.CurrentPage = page;
                        records.PageSize = pageSize;
                    }
                }

            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                //fitri 27-7-2020
                records.Content = dbview.tblUsers.Where(x => (x.fldUserFullName.Contains(filter) || x.fldUserName.Contains(filter)) && (x.fldRoleID > 2 && x.fldRoleID != 9 && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID && x.fldWilayahID == WilayahID))
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                records.TotalRecords = dbview.tblUsers.Where(x => (x.fldUserFullName.Contains(filter) || x.fldUserName.Contains(filter)) && (x.fldRoleID > 2 && x.fldRoleID != 9 && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID && x.fldWilayahID == WilayahID)).Count();
                records.CurrentPage = page;
                records.PageSize = pageSize;
            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                //fitri 27-7-2020
                records.Content = dbview.tblUsers.Where(x => (x.fldUserFullName.Contains(filter) || x.fldUserName.Contains(filter)) && (x.fldRoleID > 2 && x.fldRoleID != 9 && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID && x.fldWilayahID == WilayahID && x.fldLadangID == LadangID))
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                records.TotalRecords = dbview.tblUsers.Where(x => (x.fldUserFullName.Contains(filter) || x.fldUserName.Contains(filter)) && (x.fldRoleID > 2 && x.fldRoleID != 9 && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID && x.fldWilayahID == WilayahID && x.fldLadangID == LadangID)).Count();
                records.CurrentPage = page;
                records.PageSize = pageSize;
            }
            return View(records);
        }*/

        // GET: User/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            MasterModels.tblUser tblUser = db.tblUsers.Find(id);
            if (tblUser == null)
            {
                return RedirectToAction("Index");
            }
            return PartialView("Details", tblUser);
        }
        public JsonResult CheckString(string fldUserName) // Add by fitri 8-12-2020
        {
            return Json(IsValidStr(fldUserName));
        }
        public bool IsValidStr(string InputUsername) // Add by fitri 8-12-2020
        {
            bool status;
            var strIndicator = "FAS";
            string inputCheckLtr = InputUsername.Substring(0, 3).ToUpper();

            if (inputCheckLtr != strIndicator)
            {
                status = false;
            }
            else
            {
                status = true;
            }
            return status;
        }
        public JsonResult GetLadang(int WilayahID) // Add by fitri 7-12-2020
        {
            List<SelectListItem> ladanglist = new List<SelectListItem>();
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID2 = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID2, out LadangID, getuserid, User.Identity.Name);

            if (GetWilayah.GetAvailableWilayah(SyarikatID))
            {
                if (WilayahID == 0)
                {
                    ladanglist = new SelectList(db.vw_NSWL.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted_L == false).OrderBy(o => o.fld_NamaLadang).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList();
                }
                else
                {
                    ladanglist = new SelectList(db.vw_NSWL.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted_L == false).OrderBy(o => o.fld_NamaLadang).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList();
                }
            }
            return Json(ladanglist);
        }


        // GET: User/Create
        public ActionResult Create()
        {
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int[] wlyhid = new int[] { };
            //string mywlyid = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            if (getidentity.MySuperAdmin(User.Identity.Name))
            {
                ViewBag.fldRoleID = new SelectList(db.tblRoles.Where(x => x.fldDeleted == false), "fldRoleID", "fldDescriptionRole");
            }
            else
            {
                ViewBag.fldRoleID = new SelectList(db.tblRoles.Where(x => x.fldRoleID > 2 && x.fldRoleID != 9 && x.fldDeleted == false), "fldRoleID", "fldDescriptionRole");
            }

            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();
            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = GetWilayah.GetWilayahID(SyarikatID);
                //mywlyid = String.Join("", wlyhid); ;
                WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
                WilayahIDList.Insert(0, (new SelectListItem { Text = "HQ", Value = "0" }));
                // Add by fitri 7-12-2020
                LadangIDList = new SelectList(db.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = GetWilayah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => x.fld_ID == WilayahID), "fld_ID", "fld_WlyhName").ToList();
                // Add by fitri 7-12-2020
                LadangIDList = new SelectList(db.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));
            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = GetWilayah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                // Add by fitri 7-12-2020
                LadangIDList = new SelectList(db.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            }

            ViewBag.fldWilayahID = WilayahIDList;
            ViewBag.fldLadangID = LadangIDList; // Add by fitri 7-12-2020
            return PartialView("Create");
        }

        // POST: User/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MasterModels.tblUser tblUser)
        {
            DateTime getdatetime = timezone.gettimezone();
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            int? getclientid = db.tbl_ServicesList.Where(x => x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID && x.fldWilayahID == tblUser.fldWilayahID).Select(s => s.fld_ClientID).FirstOrDefault();

            int? getkmplnid = db.tbl_Negara.Where(x => x.fld_NegaraID == NegaraID).Select(s => s.fld_KmplnSyrktID).FirstOrDefault();


            //if (ModelState.IsValid)
            //{
            // Comment by fitri 7-12-2020
            /*try 
            {
                var checkdata = db.tblUsers.Where(x => x.fldUserName == tblUser.fldUserName).FirstOrDefault();
                if (checkdata == null)
                {
                    tblUser.fldUserName = tblUser.fldUserName.ToUpper();
                    tblUser.fldUserFullName = tblUser.fldUserFullName.ToUpper();
                    tblUser.fldUserShortName = tblUser.fldUserShortName.ToUpper();
                    tblUser.fldUserPassword = crypto.Encrypt(tblUser.fldUserPassword);
                    tblUser.fldDeleted = false;
                    tblUser.fldClientID = getclientid;
                    tblUser.fld_CreatedBy = getuserid;
                    tblUser.fld_CreatedDT = getdatetime;
                    tblUser.fld_ModifiedBy = getuserid;
                    tblUser.fld_ModifiedDT = getdatetime;
                    tblUser.fld_KmplnSyrktID = getkmplnid;
                    tblUser.fldFirstTimeLogin = 1;
                    tblUser.fldLadangID = 0;
                    tblUser.fldNegaraID = NegaraID;
                    tblUser.fldSyarikatID = SyarikatID;
                    tblUser.fld_KmplnSyrktID = getkmplnid;
                    tblUser.fldUserCategory = "CHECKROLL";
                    db.tblUsers.Add(tblUser);
                    db.SaveChanges();
                    var getid = db.tblUsers.Where(w => w.fldUserName == tblUser.fldUserName).FirstOrDefault();
                    return Json(new { success = true, msg = "Data successfully added.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = tblUser.fldUserName, data2 = tblUser.fldUserName, data3 = "" });
                }
                else
                {
                    return Json(new { success = true, msg = "Username already exist.", status = "warning", checkingdata = "1" });
                }
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
            }*/
            //}
            //else
            //{
            //    return Json(new { success = true, msg = "Please check fill you inserted.", status = "warning", checkingdata = "1" });
            //}

            // Add by fitri 7-12-2020
            try
            {
                if (ModelState.IsValid)
                {
                    var strIndicator = "FAS";
                    string inputUsername = tblUser.fldUserName;
                    string inputCheckLtr = inputUsername.Substring(0, 3).ToUpper();

                    if (inputCheckLtr != strIndicator)
                    {
                        return Json(new { success = false, msg = "Username must start with FAS", status = "danger", checkingdata = "0" });
                    }

                    var checkdata = db.tblUsers.Where(x => x.fldUserName == tblUser.fldUserName).FirstOrDefault();
                    if (checkdata == null)
                    {
                        tblUser.fldUserName = tblUser.fldUserName.ToUpper();
                        tblUser.fldUserFullName = tblUser.fldUserFullName.ToUpper();
                        tblUser.fldUserShortName = tblUser.fldUserShortName.ToUpper();
                        tblUser.fldUserPassword = crypto.Encrypt(tblUser.fldUserPassword);
                        tblUser.fldDeleted = false;
                        tblUser.fldClientID = getclientid;
                        tblUser.fld_CreatedBy = getuserid;
                        tblUser.fld_CreatedDT = getdatetime;
                        tblUser.fld_ModifiedBy = getuserid;
                        tblUser.fld_ModifiedDT = getdatetime;
                        tblUser.fld_KmplnSyrktID = getkmplnid;
                        tblUser.fldFirstTimeLogin = 1;
                        tblUser.fldNegaraID = NegaraID;
                        tblUser.fldSyarikatID = SyarikatID;
                        tblUser.fld_KmplnSyrktID = getkmplnid;
                        tblUser.fldUserCategory = "CHECKROLL";
                        db.tblUsers.Add(tblUser);
                        db.SaveChanges();

                        var getid = db.tblUsers.Where(w => w.fldUserName == tblUser.fldUserName).FirstOrDefault();
                        string appname = Request.ApplicationPath;
                        string domain = Request.Url.GetLeftPart(UriPartial.Authority);
                        var lang = Request.RequestContext.RouteData.Values["lang"];
                        if (appname != "/")
                        { domain = domain + appname; }
                        return Json(new
                        {
                            success = true,
                            msg = GlobalResEstate.msgUpdate,
                            status = "success",
                            checkingdata = "0",
                            method = "1",
                            div = "UserDetails",
                            rootUrl = domain,
                            action = "_User",
                            controller = "User"
                        });
                    }
                    else
                    {
                        return Json(new { success = false, msg = "Username already exist.", status = "warning", checkingdata = "0" });
                    }
                }
                return Json(new { success = false, msg = GlobalResEstate.msgErrorData, status = "danger", checkingdata = "0" });
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "0" });
            }
            finally
            {
                db.Dispose();
            }
        }

        // GET: User/Edit/5
        public ActionResult Edit(int? id)
        {
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            

            if (id == null)
            {
                return RedirectToAction("Index");
            }
            MasterModels.tblUser tblUser = db.tblUsers.Find(id);
            if (tblUser == null)
            {
                return RedirectToAction("Index");
            }

            if (getidentity.MySuperAdmin(User.Identity.Name))
            {
                ViewBag.fldRoleID = new SelectList(db.tblRoles.Where(x => x.fldDeleted == false), "fldRoleID", "fldDescriptionRole", tblUser.fldRoleID);
            }
            else
            {
                ViewBag.fldRoleID = new SelectList(db.tblRoles.Where(x => x.fldRoleID > 2 && x.fldRoleID != 9 && x.fldDeleted == false), "fldRoleID", "fldDescriptionRole", tblUser.fldRoleID);
            }

            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();
            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = GetWilayah.GetWilayahID(SyarikatID);
                //mywlyid = String.Join("", wlyhid); ;
                WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName", tblUser.fldWilayahID).ToList();
                WilayahIDList.Insert(0, (new SelectListItem { Text = "HQ", Value = "0" }));
                // Add by fitri 7-12-2020
                LadangIDList = new SelectList(db.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", tblUser.fldLadangID).ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = GetWilayah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => x.fld_ID == WilayahID), "fld_ID", "fld_WlyhName", tblUser.fldWilayahID).ToList();
                // Add by fitri 7-12-2020
                LadangIDList = new SelectList(db.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", tblUser.fldLadangID).ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));
            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = GetWilayah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", tblUser.fldWilayahID).ToList();
                // Add by fitri 7-12-2020
                LadangIDList = new SelectList(db.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", tblUser.fldLadangID).ToList();
            }

            ViewBag.fldWilayahID = WilayahIDList;
            ViewBag.fldLadangID = LadangIDList; // Add by fitri 7-12-2020
            ViewBag.fldDeleted = new SelectList(db.tblSystemConfigs.Where(x => x.fldFlag1 == "useractivation" && x.fldDeleted == false), "fldConfigValue", "fldConfigDesc", tblUser.fldDeleted);
            tblUser.fldUserPassword = crypto.Decrypt(tblUser.fldUserPassword);
            return PartialView("Edit", tblUser);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, MasterModels.tblUser tblUser)
        {
            DateTime getdatetime = timezone.gettimezone();
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            
            int? getclientid = db.tbl_ServicesList.Where(x => x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID && x.fldWilayahID == tblUser.fldWilayahID).Select(s => s.fld_ClientID).FirstOrDefault();

            int? getkmplnid = db.tbl_Negara.Where(x => x.fld_NegaraID == NegaraID).Select(s => s.fld_KmplnSyrktID).FirstOrDefault();
            
            //if (ModelState.IsValid)
            //{
            try
            {
                var getdata = db.tblUsers.Find(id);
                getdata.fldUserPassword = crypto.Encrypt(tblUser.fldUserPassword);
                getdata.fldUserFullName = tblUser.fldUserFullName.ToUpper();
                getdata.fldUserShortName = tblUser.fldUserShortName.ToUpper();
                getdata.fldUserEmail = tblUser.fldUserEmail;
                getdata.fldClientID = getclientid;
                getdata.fldRoleID = tblUser.fldRoleID;
                getdata.fldDeleted = tblUser.fldDeleted;
                getdata.fld_ModifiedBy = getuserid;
                getdata.fld_ModifiedDT = getdatetime;
                getdata.fld_KmplnSyrktID = getkmplnid;
                getdata.fldUserCategory = "CHECKROLL";
                getdata.fldWilayahID = tblUser.fldWilayahID; // add by Fitri 7-12-2020
                getdata.fldLadangID = tblUser.fldLadangID; // add by Fitri 7-12-2020
                db.Entry(getdata).State = EntityState.Modified;
                db.SaveChanges();
                var getid = id;
                return Json(new { success = true, msg = "Data successfully edited.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = "", data2 = "" });
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
            }
            //}
            //else
            //{
            //    return Json(new { success = true, msg = "Please check fill you inserted.", status = "warning", checkingdata = "1" });
            //}
        }

        // GET: User/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            MasterModels.tblUser tblUser = db.tblUsers.Find(id);
            if (tblUser == null)
            {
                return RedirectToAction("Index");
            }
            return PartialView("Delete", tblUser);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                MasterModels.tblUser tblUser = db.tblUsers.Find(id);
                if (tblUser == null)
                {
                    return Json(new { success = true, msg = "Data already deleted.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
                }
                else
                {
                    db.tblUsers.Remove(tblUser);
                    db.SaveChanges();
                    return Json(new { success = true, msg = "Data successfully deleted.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
                }
                
            }
            catch(Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
            }
            
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
                //db2.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
