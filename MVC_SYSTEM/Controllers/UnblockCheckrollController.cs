using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.ViewingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using static MVC_SYSTEM.Class.GlobalFunction;

namespace MVC_SYSTEM.Controllers
{
    public class UnblockCheckrollController : Controller
    {
        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
        private GetIdentity GetIdentity = new GetIdentity();
        private GetNSWL GetNSWL = new GetNSWL();
        private Connection Connection = new Connection();
        private GetConfig GetConfig = new GetConfig();
        errorlog geterror = new errorlog();
        private ChangeTimeZone ChangeTimeZone = new ChangeTimeZone();
        // GET: UnblockCheckroll

        public ActionResult Index(string filter, int page = 1, string sort = "fld_BlokStatus",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            
            ViewBag.UnblockCheckroll = "class = active";
            return View();
        }

        public ActionResult _UnblockCheckroll(string filter, int page = 1,
            string sort = "fld_BlokStatus",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<MasterModels.tbl_BlckKmskknDataKerja>();
            int role = GetIdentity.RoleID(getuserid).Value;

            var unitData = db.tbl_BlckKmskknDataKerja
                .Where(x => x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID);

            if (!String.IsNullOrEmpty(filter))
            {
                records.Content = unitData
                    .Where(x => x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID)
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = unitData
                    .Count(x => x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID);


            }

            else
            {
                records.Content = unitData.OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = unitData
                    .Count();
            }

            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.RoleID = role;
            ViewBag.pageSize = 1;

            return View(records);
        }

        public ActionResult _UnblockCheckrollEdit(Guid id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            List<SelectListItem> pilihanYaTidak = new List<SelectListItem>();

            pilihanYaTidak = new SelectList(db.tblOptionConfigsWebs
                .Where(x => x.fldOptConfFlag1 == "pilihanyatidak" && x.fldDeleted == false && x.fld_NegaraID == NegaraID &&
                x.fld_SyarikatID == SyarikatID)
                .OrderBy(o => o.fldOptConfID)
                .Select(s => new SelectListItem { Value = s.fldOptConfFlag2, Text = s.fldOptConfDesc })
                , "Value", "Text").ToList();

            var unitData = db.tbl_BlckKmskknDataKerja.SingleOrDefault(
                x => x.fld_ID == id &&
                            x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID);

            tbl_BlckKmskknDataKerjaUpdate unitViewModel = new tbl_BlckKmskknDataKerjaUpdate();

            PropertyCopy.Copy(unitViewModel, unitData);

            ViewBag.fld_Selection = pilihanYaTidak;
            return View(unitViewModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _UnblockCheckrollEdit(MasterModels.tbl_BlckKmskknDataKerjaUpdate optionConfigsWeb)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            tbl_BlckKmskknDataKerjaHistory BlkHistoryModel = new tbl_BlckKmskknDataKerjaHistory();

            try
            {
                if (ModelState.IsValid)
                {
                    var unitData = db.tbl_BlckKmskknDataKerja.SingleOrDefault(
                        x => x.fld_ID == optionConfigsWeb.fld_ID &&
                             x.fld_NegaraID == NegaraID &&
                             x.fld_SyarikatID == SyarikatID);

                    unitData.fld_ValidDT = ChangeTimeZone.gettimezone();
                    unitData.fld_BlokStatus = optionConfigsWeb.fld_BlokStatus;
                    unitData.fld_Remark = optionConfigsWeb.fld_Remark;
                    unitData.fld_UnBlockAppBy = getuserid;
                    unitData.fld_UnBlockAppDT = ChangeTimeZone.gettimezone();
                    
                    PropertyCopy.Copy(BlkHistoryModel, unitData);

                    db.tbl_BlckKmskknDataKerjaHistory.Add(BlkHistoryModel);
                    db.SaveChanges();
                    
                    string appname = Request.ApplicationPath;
                    string domain = Request.Url.GetLeftPart(UriPartial.Authority);
                    var lang = Request.RequestContext.RouteData.Values["lang"];

                    if (appname != "/")
                    {
                        domain = domain + appname;
                    }

                    return Json(new
                    {
                        success = true,
                        msg = GlobalResEstate.msgUpdate,
                        status = "success",
                        checkingdata = "0",
                        method = "1",
                        div = "UnblockCheckrollDetails",
                        rootUrl = domain,
                        action = "_UnblockCheckroll",
                        controller = "UnblockCheckroll"
                    });
                }
            

                else
                {
                    return Json(new
                    {
                        success = false,
                        msg = GlobalResEstate.msgErrorData,
                        status = "danger",
                        checkingdata = "0"
                    });
                }
            }

            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new
                {
                    success = false,
                    msg = GlobalResEstate.msgError,
                    status = "danger",
                    checkingdata = "0"
                });
            }

            finally
            {
                db.Dispose();
            }
        }

    }
}