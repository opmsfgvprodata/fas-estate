using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.ViewingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using MVC_SYSTEM.Models;
using System.Data.Entity;
using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.log;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public class CheckRollMainController : Controller
    {
        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
        private GetIdentity GetIdentity = new GetIdentity();
        private GetNSWL GetNSWL = new GetNSWL();
        private Connection Connection = new Connection();
        private GetConfig GetConfig = new GetConfig();
        private errorlog geterror = new errorlog();
        // GET: CheckRollMain
        public ActionResult Index()
        {
            ViewBag.CheckRoll = "class = active";
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            ViewBag.CheckRollMenu = new SelectList(db.tblMenuLists.Where(x => x.fld_Flag == "dataEntry" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fld_Val", "fld_Desc");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string CheckRollMenu)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            var GetMenuList = db.tblMenuLists.Where(x => x.fld_Flag == CheckRollMenu && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).FirstOrDefault();
            return RedirectToAction(GetMenuList.fld_Val, GetMenuList.fld_Desc);
        }

        public ActionResult RegisterSkb(string MonthList = "", int YearList = 0, int page = 1, string sort = "fld_ID", string sortdir = "ASC")
        {
            //ViewBag.Maintenance = "class = active";
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            //MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            ViewBag.CheckRoll = "class = active";

            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.tbl_Skb>();

            int range = int.Parse(GetConfig.GetData("yeardisplay"));
            int startyear = DateTime.Now.AddYears(-range).Year;
            int currentyear = DateTime.Now.Year;
            DateTime selectdate = DateTime.Now.AddMonths(-1);

            if (MonthList == "" && YearList == 0)
            {
                var yearlist = new List<SelectListItem>();
                for (var i = startyear; i <= currentyear; i++)
                {
                    if (i == selectdate.Year)
                    {
                        yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                    }
                    else
                    {
                        yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                    }
                }
                var monthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", selectdate.Month);
                records.Content = dbview.tbl_Skb.Where(x => x.fld_Bulan == selectdate.Month.ToString() && x.fld_Tahun == selectdate.Year && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false)
                       .OrderBy(sort + " " + sortdir)
                       .Skip((page - 1) * pageSize)
                       .Take(pageSize)
                       .ToList();

                records.TotalRecords = dbview.tbl_Skb.Where(x => x.fld_Bulan == selectdate.Month.ToString() && x.fld_Tahun == selectdate.Year && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).Count();
                records.CurrentPage = page;
                records.PageSize = pageSize;

                ViewBag.MonthList = monthList;
                ViewBag.YearList = yearlist;
                ViewBag.Datacount = records.TotalRecords;
                return View(records);
            }
            else
            {
                var yearlist = new List<SelectListItem>();
                for (var i = startyear; i <= currentyear; i++)
                {
                    if (i == YearList)
                    {
                        yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                    }
                    else
                    {
                        yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                    }
                }
                var monthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", MonthList);
                GetConfig GetConfig = new GetConfig();
                string bulanString = GetConfig.GetWebConfigDesc(MonthList, "monthlist", NegaraID, SyarikatID);
                records.Content = dbview.tbl_Skb.Where(x => x.fld_Bulan == bulanString && x.fld_Tahun == YearList && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false)
                       .OrderBy(sort + " " + sortdir)
                       .Skip((page - 1) * pageSize)
                       .Take(pageSize)
                       .ToList();

                records.TotalRecords = dbview.tbl_Skb.Where(x => x.fld_Bulan == bulanString && x.fld_Tahun == YearList && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).Count();
                records.CurrentPage = page;
                records.PageSize = pageSize;

                ViewBag.MonthList = monthList;
                ViewBag.YearList = yearlist;
                ViewBag.Datacount = records.TotalRecords;
                return View(records);
            }
        }

        public ActionResult RegisterSkbUpdate(int id)
        {
            if (id < 1)
            {
                return RedirectToAction("RegisterSkb");
            }
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            Models.tbl_Skb tbl_Skb = dbr.tbl_Skb.Where(w => w.fld_ID == id).FirstOrDefault();
            if (tbl_Skb == null)
            {
                return RedirectToAction("RegisterSkb");
            }

            return PartialView(tbl_Skb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterSkbUpdate(int id, Models.tbl_Skb tbl_Skb)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int? getuserid = GetIdentity.ID(User.Identity.Name);
                    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
                    string host, catalog, user, pass = "";
                    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
                    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
                    MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

                    var getdata = dbr.tbl_Skb.Where(w => w.fld_ID == id).FirstOrDefault();

                    getdata.fld_NoSkb = tbl_Skb.fld_NoSkb;

                    dbr.Entry(getdata).State = EntityState.Modified;
                    dbr.SaveChanges();
                    var getid = id;
                    return Json(new { success = true, msg = GlobalResEstate.msgUpdate, status = "success", checkingdata = "0", method = "1", getid = getid, data1 = "", data2 = "" });
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new { success = true, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
                }
            }
            else
            {
                return Json(new { success = true, msg = GlobalResEstate.msgErrorData, status = "warning", checkingdata = "1" });
            }
        }
    }
}