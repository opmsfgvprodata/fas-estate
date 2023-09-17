using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.log;
using MVC_SYSTEM.ViewingModels;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.App_LocalResources;
using Itenso.TimePeriod;
using System.Web;
using System.Web.Helpers;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Web.WebPages;
using iTextSharp.text;
using Microsoft.Ajax.Utilities;
using MVC_SYSTEM.CustomModels;
using tbl_Insentif = MVC_SYSTEM.Models.tbl_Insentif;
using tbl_Pkjmast = MVC_SYSTEM.Models.tbl_Pkjmast;
using static MVC_SYSTEM.Class.GlobalFunction;
using System.Transactions;

namespace MVC_SYSTEM.Controllers
{
    //Check_Balik
    [AccessDeniedAuthorizeAttribute(Roles =
        "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public class WorkerInfoController : Controller
    {
        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();

        //private MVC_SYSTEM_HQ_Models db = new MVC_SYSTEM_HQ_Models();
        //MVC_SYSTEM_Viewing dbview = new MVC_SYSTEM_Viewing();
        private ChangeTimeZone timezone = new ChangeTimeZone();

        private GetIdentity getidentity = new GetIdentity();
        private GetNSWL GetNSWL = new GetNSWL();
        errorlog geterror = new errorlog();
        GetConfig GetConfig = new GetConfig();
        GetIdentity GetIdentity = new GetIdentity();
        GetWilayah GetWilayah = new GetWilayah();
        Connection Connection = new Connection();

        // GET: WorkerInfo
        public ActionResult Index()
        {
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.WorkerInfo = "class = active";
            ViewBag.WorkerInfoList = new SelectList(
                db.tblMenuLists.Where(x => x.fld_Flag == "maklumatPkjList" && x.fldDeleted == false &&
                                           x.fld_NegaraID == NegaraID &&
                                           x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fld_ID),
                "fld_Val", "fld_Desc");

            return View();
        }

        [HttpPost]
        public ActionResult Index(string WorkerInfoList)
        {
            return RedirectToAction(WorkerInfoList, "WorkerInfo");
        }

        public ActionResult WorkerGroupInfo(string filter = "", int page = 1, string sort = "fld_Nopkj",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            ViewBag.Message1 = ViewBag.Message;
            ViewBag.status1 = ViewBag.status;

            ViewBag.WorkerInfo = "class = active";
            ViewBag.filter = filter;
            //ViewBag.WorkerSearchFilter = filter;

            return View();
        }

        public PartialViewResult _WorkerSearch(int? RadioGroup, string workerid, string filter, int page = 1, string sort = "fld_KodKumpulan", string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.vw_KumpulanPekerja>();
            int role = GetIdentity.RoleID(getuserid).Value;

            ViewBag.workerid = workerid;
            ViewBag.WorkerSearchFilter = filter;

            IQueryable<ViewingModels.vw_KumpulanPekerja> WorkerList;

            int workerwithoutGroupCount = 0;

            if (RadioGroup == 0 || RadioGroup == null)
            {
                WorkerList = dbview.vw_KumpulanPekerja
                    .Where(x => x.fld_DivisionID == DivisionID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1");

                workerwithoutGroupCount = WorkerList
                    .Count(x => x.fld_KumpulanID == null);
            }

            else
            {
                WorkerList = dbview.vw_KumpulanPekerja
                    .Where(x => x.fld_DivisionID == DivisionID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" &&
                                x.fld_KumpulanID == null);

                workerwithoutGroupCount = WorkerList.Count();
            }

            if (!String.IsNullOrEmpty(filter))
            {
                records.Content = WorkerList
                    .Where(x => x.fld_Nama.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_Nopkj.ToUpper().Contains(filter.ToUpper()))
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = WorkerList
                    .Count(x => x.fld_Nama.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_Nopkj.ToUpper().Contains(filter.ToUpper()));
            }

            else
            {
                records.Content = WorkerList.OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = WorkerList
                    .Count();
            }

            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.RoleID = role;
            ViewBag.pageSize = pageSize;
            ViewBag.WorkerWithoutGroupCount = workerwithoutGroupCount;
            //ViewBag.RadioGroup = RadioGroup;
            if (RadioGroup == null || RadioGroup == 0)
            {
                TempData["RadioGroup"] = 0;
            }
            else
            {
                TempData["RadioGroup"] = 1;
            }
            TempData.Keep();

            return PartialView(records);
        }

        public ActionResult _WorkerGroupSelection(Guid workerid, string WorkerSearchFilter, string filter = "",
            int page = 1,
            string sort = "fld_KodKumpulan",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            TempData.Keep();

            ViewBag.filter = filter;
            ViewBag.workerid = workerid;
            ViewBag.WorkerSearchFilter = WorkerSearchFilter;

            return View();
        }

        public PartialViewResult _WorkerGroupSearch(Guid? workerid, string WorkerSearchFilter, string filter = "",
            int page = 1,
            string sort = "fld_KodKumpulan",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            TempData.Keep();

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.vw_KumpulanKerja>();
            int role = GetIdentity.RoleID(getuserid).Value;
            ViewBag.workerid = workerid;
            ViewBag.WorkerSearchFilter = WorkerSearchFilter;

            var GroupList = dbview.vw_KumpulanKerja
                .Where(x => x.fld_DivisionID == DivisionID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false);

            if (!String.IsNullOrEmpty(filter))
            {
                records.Content = GroupList
                    .Where(x => x.fld_KodKumpulan.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_KodKerja.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_Keterangan.ToUpper().Contains(filter.ToUpper()))
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = GroupList
                    .Count(x => x.fld_KodKumpulan.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_KodKerja.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_Keterangan.ToUpper().Contains(filter.ToUpper()));
            }

            else
            {
                records.Content = GroupList
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = GroupList
                    .Count();

            }

            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.RoleID = role;
            ViewBag.pageSize = pageSize;

            return PartialView(records);
        }

        public ActionResult _WorkerGroupAdd(Guid workerid, int id, string WorkerSearchFilter)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            TempData.Keep();

            Models.vw_KumpulanPekerja vw_KumpulanPekerja = dbr.vw_KumpulanPekerja
                .Single(x => x.fld_UniqueID == workerid && x.fld_LadangID == LadangID &&
                             x.fld_WilayahID == WilayahID && x.fld_NegaraID == NegaraID);

            var getGroupCode = dbr.tbl_KumpulanKerja
                .Where(x => x.fld_KumpulanID == id)
                .Select(s => s.fld_KodKumpulan)
                .OrderByDescending(s => s)
                .FirstOrDefault();

            var getGroupActivity = dbr.tbl_KumpulanKerja
                .Where(x => x.fld_KumpulanID == id)
                .Select(s => s.fld_Keterangan)
                .OrderByDescending(s => s)
                .FirstOrDefault();

            ViewBag.KodKumpulan = getGroupCode;
            ViewBag.Keterangan = getGroupActivity;

            return PartialView(vw_KumpulanPekerja);
        }

        [HttpPost, ActionName("_WorkerGroupAdd")]
        [ValidateAntiForgeryToken]
        public ActionResult WorkerGroupAddConfirm(Guid workerid, int id, string WorkerSearchFilter)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int RadioGroup = 0;

            if (TempData["RadioGroup"] != null)
            {
                RadioGroup = (int) TempData["RadioGroup"];
            }

            try
            {
                var getdata = dbr.tbl_Pkjmast
                    .Single(x => x.fld_UniqueID == workerid && x.fld_LadangID == LadangID &&
                                 x.fld_WilayahID == WilayahID && x.fld_NegaraID == NegaraID);

                getdata.fld_KumpulanID = id;

                dbr.SaveChanges();

                var getid = id;

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
                    msg = GlobalResEstate.msgAdd,
                    status = "success",
                    checkingdata = "0",
                    method = "3",
                    div = "searchResultWorkerGroupInfo",
                    rootUrl = domain,
                    action = "_WorkerSearch",
                    controller = "WorkerInfo",
                    paramName = "RadioGroup",
                    paramValue = RadioGroup,
                    paramName2 = "filter",
                    paramValue2 = WorkerSearchFilter
                });
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
        }

        public ActionResult WorkerGroupManage(string filter = "", int page = 1, string sort = "fld_KodKumpulan",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            ViewBag.filter = filter;
            ViewBag.WorkerInfo = "class = active";

            ViewBag.message = TempData["message"];
            ViewBag.status = TempData["status"];

            return View();
        }

        public PartialViewResult _WorkerGroupManageSearch(string workerid, string filter = "", int page = 1,
            string sort = "fld_KodKumpulan",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.vw_KumpulanKerja>();
            int role = GetIdentity.RoleID(getuserid).Value;
            //ViewBag.filter = filter;

            var GroupList = dbview.vw_KumpulanKerja
                .Where(x => x.fld_DivisionID == DivisionID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false);

            if (!String.IsNullOrEmpty(filter))
            {
                records.Content = GroupList
                    .Where(x => x.fld_KodKumpulan.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_KodKerja.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_Keterangan.ToUpper().Contains(filter.ToUpper()))
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = GroupList
                    .Count(x => x.fld_KodKumpulan.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_KodKerja.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_Keterangan.ToUpper().Contains(filter.ToUpper()));
            }

            else
            {
                records.Content = GroupList
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = GroupList
                    .Count();

            }

            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.RoleID = role;
            ViewBag.pageSize = pageSize;
            return PartialView(records);
        }


        public ActionResult _WorkerGroupManageMember(int id, int page = 1, string sort = "fld_Nopkj",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.tbl_Pkjmast>();

            var GrpMemberList = dbview.tbl_Pkjmast
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_KumpulanID == id);

            records.Content = GrpMemberList
                .OrderBy(sort + " " + sortdir)
                .ToList();

            records.TotalRecords = GrpMemberList
                .Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.pageSize = pageSize;
            // return View(records);

            return PartialView(records);
        }

        public ActionResult WorkerGroupDeleteList(string[] ids)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            TempData["workerID"] = ids;

            return View("_WorkerGroupDelete");
        }

        public ActionResult _WorkerGroupDelete()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            //string[] ids = TempData["workerID"];

            var workerid = (object[]) TempData["workerID"];

            Guid[] id = null;
            if (workerid != null)
            {
                id = new Guid[workerid.Length];
                int j = 0;
                //assign the values in UserIDs array to int[] array by converting
                foreach (string i in workerid)
                {
                    Guid.TryParse(i, out id[j++]);
                }
            }
            //find and delete those selected records
            if (id != null & id.Length > 0)
            {
                //List selectedList = new List();

                using (dbr)
                {
                    var selectedList = dbr.tbl_Pkjmast
                        .Where(a => id.Contains(a.fld_UniqueID) && a.fld_NegaraID == NegaraID &&
                                    a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                    a.fld_LadangID == LadangID)
                        .ToList();

                    foreach (var i in selectedList)
                    {
                        var getRecord = dbr.tbl_Pkjmast.Find(i.fld_UniqueID);

                        getRecord.fld_KumpulanID = null;


                    }
                    dbr.SaveChanges();
                }
            }

            TempData["message"] = GlobalResEstate.msgUpdate;
            TempData["status"] = "success";

            return RedirectToAction("WorkerGroupManage");
        }

        public ActionResult _WorkerGroupDeleteConfirm(string[] ids)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            //int[] id = null;

            return RedirectToAction("Index");
        }

        //driver action to auto calculate worker leave
        public ActionResult LeaveInfo(string filter = "", int page = 1, string sort = "fld_Nopkj",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            ViewBag.Message1 = ViewBag.Message;
            ViewBag.status1 = ViewBag.status;

            ViewBag.WorkerInfo = "class = active";
            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.tbl_Pkjmast>();
            int role = GetIdentity.RoleID(getuserid).Value;
            ViewBag.filter = filter;

            //guna utk test cuti pekerja
            var WorkerList = dbview.tbl_Pkjmast
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID
                            &&
                            x.fld_StatusApproved == 3 || x.fld_StatusApproved == 2);

            records.Content = WorkerList
                .OrderBy(sort + " " + sortdir)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            records.TotalRecords = WorkerList
                .Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;

            ViewBag.RoleID = role;
            ViewBag.pageSize = pageSize;
            return View(records);
        }

        //driver action to auto calculate worker leave //generate cuti tahunan pekerja baru
        public ActionResult ApproveWorker(Guid id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            Models.tbl_Pkjmast tbl_Pkjmast = dbr.tbl_Pkjmast
                .Single(u => u.fld_UniqueID == id && u.fld_WilayahID == WilayahID && u.fld_SyarikatID == SyarikatID &&
                             u.fld_NegaraID == NegaraID);

            return PartialView("ApproveWorker", tbl_Pkjmast);
        }

        //driver action to auto calculate worker leave //generate cuti tahunan pekerja baru
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult ApproveWorker(Guid id, Models.tbl_Pkjmast tbl_Pkjmast)
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
        //        NegaraID.Value);
        //    MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
        //    int year = DateTime.Now.Year;
        //    //DateTime firstDay = new DateTime(year, 1, 1);
        //    DateTime lastDay = new DateTime(year, 12, 31);


        //    try
        //    {
        //        var workerData = dbr.tbl_Pkjmast
        //            .Single(x => x.fld_UniqueID == id && x.fld_LadangID == LadangID &&
        //                         x.fld_WilayahID == WilayahID && x.fld_NegaraID == NegaraID);

        //        DateDiff dateDiff = new DateDiff(Convert.ToDateTime(workerData.fld_Trmlkj).AddDays(-1), lastDay);

        //        //calculate annual leave

        //        var cutiTahunanPkjBaru = db.tbl_CutiMaintenance
        //            .Where(x => x.fld_JenisCuti == "cutiTahunanPkjBaru" && x.fld_Deleted == false
        //                        && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID
        //                        && x.fld_LowerLimit <= dateDiff.Months && x.fld_UpperLimit >= dateDiff.Months)
        //            .Select(s => s.fld_PeruntukkanCuti)
        //            .Single();

        //        var kodCutiTahunan = db.tbl_CutiKategori
        //            .Where(x => x.fld_KeteranganCuti == "CUTI TAHUNAN")
        //            .Select(s => s.fld_KodCuti)
        //            .Single();

        //        Models.tbl_CutiPeruntukan CutiPeruntukanTahunan = new Models.tbl_CutiPeruntukan();

        //        CutiPeruntukanTahunan.fld_NoPkj = workerData.fld_Nopkj;
        //        CutiPeruntukanTahunan.fld_JumlahCuti = cutiTahunanPkjBaru;
        //        CutiPeruntukanTahunan.fld_KodCuti = kodCutiTahunan;
        //        CutiPeruntukanTahunan.fld_JumlahCutiDiambil = 0;
        //        CutiPeruntukanTahunan.fld_Tahun = Convert.ToInt16(year);
        //        dbr.tbl_CutiPeruntukan.Add(CutiPeruntukanTahunan);


        //        //calculate sick leave
        //        var cutiSakitPkjBaru = db.tbl_CutiMaintenance
        //            .Where(x => x.fld_JenisCuti == "cutiSakitPkj" && x.fld_Deleted == false
        //                        && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID
        //                        && x.fld_LowerLimit <= dateDiff.Years && x.fld_UpperLimit >= dateDiff.Years)
        //            .Select(s => s.fld_PeruntukkanCuti)
        //            .Single();

        //        var kodCutiSakit = db.tbl_CutiKategori
        //            .Where(x => x.fld_KeteranganCuti == "CUTI SAKIT")
        //            .Select(s => s.fld_KodCuti)
        //            .Single();

        //        Models.tbl_CutiPeruntukan CutiPeruntukanSakit = new Models.tbl_CutiPeruntukan();

        //        CutiPeruntukanSakit.fld_NoPkj = workerData.fld_Nopkj;
        //        CutiPeruntukanSakit.fld_JumlahCuti = cutiSakitPkjBaru;
        //        CutiPeruntukanSakit.fld_KodCuti = kodCutiSakit;
        //        CutiPeruntukanSakit.fld_JumlahCutiDiambil = 0;
        //        CutiPeruntukanSakit.fld_Tahun = Convert.ToInt16(year);
        //        dbr.tbl_CutiPeruntukan.Add(CutiPeruntukanSakit);

        //        //calculate public holiday
        //        var kodCutiUmum = db.tbl_CutiKategori
        //            .Where(x => x.fld_KeteranganCuti == "CUTI AM")
        //            .Select(s => s.fld_KodCuti)
        //            .Single();

        //        var kodNegeriLadang = db.tbl_Ladang
        //            .Where(x => x.fld_ID == workerData.fld_LadangID)
        //            .Select(s => s.fld_KodNegeri)
        //            .Single();

        //        var CutiUmum = db.tbl_CutiUmum
        //            .Count(x => x.fld_Negeri == kodNegeriLadang && x.fld_Tahun == year &&
        //     
                //var CutiUmum = db.tbl_CutiUmum
                //    .Count(x => x.fld_Negeri == int.Parse(kodNegeriLadang) && x.fld_Tahun == year &&
                //                x.fld_TarikhCuti > workerData.fld_Trmlkj);

        //        Models.tbl_CutiPeruntukan CutiPeruntukanUmum = new Models.tbl_CutiPeruntukan();

        //        CutiPeruntukanUmum.fld_NoPkj = workerData.fld_Nopkj;
        //        CutiPeruntukanUmum.fld_JumlahCuti = CutiUmum;
        //        CutiPeruntukanUmum.fld_KodCuti = kodCutiUmum;
        //        CutiPeruntukanUmum.fld_JumlahCutiDiambil = 0;
        //        CutiPeruntukanUmum.fld_Tahun = Convert.ToInt16(year);
        //        dbr.tbl_CutiPeruntukan.Add(CutiPeruntukanUmum);

        //        workerData.fld_StatusApproved = 2;

        //        dbr.Entry(workerData).State = EntityState.Modified;
        //        dbr.SaveChanges();
        //        var getid = id;
        //        return Json(new
        //        {
        //            success = true,
        //            msg = GlobalResEstate.msgUpdate,
        //            status = "success",
        //            checkingdata = "0",
        //            method = "1",
        //            getid = getid,
        //            data1 = "",
        //            data2 = ""
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
        //        return Json(new
        //        {
        //            success = true,
        //            msg = GlobalResEstate.msgError,
        //            status = "danger",
        //            checkingdata = "1"
        //        });
        //    }
        //}

        //driver action to auto calculate worker leave //generate cuti tahunan pekerja lama
        public ActionResult GenerateWorkerLeave(Guid id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            Models.tbl_Pkjmast tbl_Pkjmast = dbr.tbl_Pkjmast
                .Single(u => u.fld_UniqueID == id && u.fld_WilayahID == WilayahID && u.fld_SyarikatID == SyarikatID &&
                             u.fld_NegaraID == NegaraID);

            return PartialView("GenerateWorkerLeave", tbl_Pkjmast);
        }

        //driver action to auto calculate worker leave //generate cuti tahunan pekerja lama
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult GenerateWorkerLeave(Guid id, Models.tbl_Pkjmast tbl_Pkjmast)
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
        //        NegaraID.Value);
        //    MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
        //    int year = DateTime.Now.Year;
        //    //DateTime firstDay = new DateTime(year, 1, 1);
        //    DateTime lastDay = new DateTime(year, 12, 31);

        //    try
        //    {
        //        var workerData = dbr.tbl_Pkjmast
        //            .Single(x => x.fld_UniqueID == id && x.fld_LadangID == LadangID &&
        //                         x.fld_WilayahID == WilayahID && x.fld_NegaraID == NegaraID);

        //        DateDiff dateDiff = new DateDiff(Convert.ToDateTime(workerData.fld_Trmlkj).AddDays(-1), lastDay);

        //        //calculate annual leave
        //        var cutiTahunanPkjLama = db.tbl_CutiMaintenance
        //            .Where(x => x.fld_JenisCuti == "cutiTahunanPkjLama" && x.fld_Deleted == false
        //                        && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID
        //                        && x.fld_LowerLimit <= dateDiff.Years && x.fld_UpperLimit >= dateDiff.Years)
        //            .Select(s => s.fld_PeruntukkanCuti)
        //            .Single();

        //        var kodCutiTahunan = db.tbl_CutiKategori
        //            .Where(x => x.fld_KeteranganCuti == "C02" && x.fld_NegaraID == NegaraID &&
        //                        x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
        //            .Select(s => s.fld_KodCuti)
        //            .Single();

        //        Models.tbl_CutiPeruntukan CutiPeruntukanTahunan = new Models.tbl_CutiPeruntukan();

        //        CutiPeruntukanTahunan.fld_NoPkj = workerData.fld_Nopkj;
        //        CutiPeruntukanTahunan.fld_JumlahCuti = cutiTahunanPkjLama;
        //        CutiPeruntukanTahunan.fld_KodCuti = kodCutiTahunan;
        //        CutiPeruntukanTahunan.fld_JumlahCutiDiambil = 0;
        //        CutiPeruntukanTahunan.fld_Tahun = Convert.ToInt16(year);
        //        dbr.tbl_CutiPeruntukan.Add(CutiPeruntukanTahunan);

        //        //calculate sick leave
        //        var cutiSakitPkjBaru = db.tbl_CutiMaintenance
        //            .Where(x => x.fld_JenisCuti == "cutiSakitPkj" && x.fld_Deleted == false
        //                        && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID
        //                        && x.fld_LowerLimit <= dateDiff.Years && x.fld_UpperLimit >= dateDiff.Years)
        //            .Select(s => s.fld_PeruntukkanCuti)
        //            .Single();

        //        //int cutiSakit = Convert.ToInt32(peruntukanCutiSakit[0]);

        //        Models.tbl_CutiPeruntukan CutiPeruntukanSakit = new Models.tbl_CutiPeruntukan();

        //        var kodCutiSakit = db.tbl_CutiKategori
        //            .Where(x => x.fld_KeteranganCuti == "C03" && x.fld_NegaraID == NegaraID &&
        //                        x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
        //            .Select(s => s.fld_KodCuti)
        //            .Single();

        //        CutiPeruntukanSakit.fld_NoPkj = workerData.fld_Nopkj;
        //        CutiPeruntukanSakit.fld_JumlahCuti = cutiSakitPkjBaru;
        //        CutiPeruntukanSakit.fld_KodCuti = kodCutiSakit;
        //        CutiPeruntukanSakit.fld_JumlahCutiDiambil = 0;
        //        CutiPeruntukanSakit.fld_Tahun = Convert.ToInt16(year);
        //        dbr.tbl_CutiPeruntukan.Add(CutiPeruntukanSakit);

        //        //calculate public holiday
        //        Models.tbl_CutiPeruntukan CutiPeruntukanUmum = new Models.tbl_CutiPeruntukan();

        //        var kodCutiUmum = db.tbl_CutiKategori
        //            .Where(x => x.fld_KeteranganCuti == "C01" && x.fld_NegaraID == NegaraID &&
        //                        x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
        //            .Select(s => s.fld_KodCuti)
        //            .Single();

        //        var kodNegeriLadang = db.tbl_Ladang
        //            .Where(x => x.fld_ID == workerData.fld_LadangID && x.fld_NegaraID == NegaraID &&
        //                        x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahID && x.fld_Deleted == false)
        //            .Select(s => s.fld_KodNegeri)
        //            .Single();

        //        var CutiUmum = db.tbl_CutiUmum
        //            .Count(x => x.fld_Negeri == kodNegeriLadang && x.fld_Tahun == year && x.fld_NegaraID == NegaraID &&
        //                        x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false);
                //var CutiUmum = db.tbl_CutiUmum
                //    .Count(x => x.fld_Negeri == int.Parse(kodNegeriLadang) && x.fld_Tahun == year && x.fld_NegaraID == NegaraID &&
                //                x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false);

        //        CutiPeruntukanUmum.fld_NoPkj = workerData.fld_Nopkj;
        //        CutiPeruntukanUmum.fld_JumlahCuti = CutiUmum;
        //        CutiPeruntukanUmum.fld_KodCuti = kodCutiUmum;
        //        CutiPeruntukanUmum.fld_JumlahCutiDiambil = 0;
        //        CutiPeruntukanUmum.fld_Tahun = Convert.ToInt16(year);
        //        dbr.tbl_CutiPeruntukan.Add(CutiPeruntukanUmum);

        //        workerData.fld_StatusApproved = 2;

        //        dbr.Entry(workerData).State = EntityState.Modified;
        //        dbr.SaveChanges();
        //        var getid = id;
        //        return Json(new
        //        {
        //            success = true,
        //            msg = GlobalResEstate.msgUpdate,
        //            status = "success",
        //            checkingdata = "0",
        //            method = "1",
        //            getid = getid,
        //            data1 = "",
        //            data2 = ""
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
        //        return Json(new
        //        {
        //            success = true,
        //            msg = GlobalResEstate.msgError,
        //            status = "danger",
        //            checkingdata = "1"
        //        });
        //    }
        //}

        //driver action to determine worker KWSP deduction and socso deduction
        public ActionResult KwspSocsoInfo(string filter = "", int page = 1, string sort = "fld_Nama",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            ViewBag.Message1 = ViewBag.Message;
            ViewBag.status1 = ViewBag.status;

            ViewBag.WorkerInfo = "class = active";
            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.vw_GajiBulananPekerja>();
            int role = GetIdentity.RoleID(getuserid).Value;
            ViewBag.filter = filter;

            //guna utk test cuti pekerja
            var WorkerList = dbview.vw_GajiBulananPekerja
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

            records.Content = WorkerList
                .OrderBy(sort + " " + sortdir)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            records.TotalRecords = WorkerList
                .Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;

            ViewBag.RoleID = role;
            ViewBag.pageSize = pageSize;
            return View(records);
        }

        public ActionResult CalculateKWSP(String nopkj, Guid gajiGuid)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            Models.tbl_Pkjmast tbl_Pkjmast = dbr.tbl_Pkjmast
                .Single(u => u.fld_Nopkj.ToString() == nopkj && u.fld_NegaraID == NegaraID &&
                             u.fld_SyarikatID == SyarikatID &&
                             u.fld_WilayahID == WilayahID && u.fld_LadangID == LadangID);

            return PartialView("CalculateKWSP", tbl_Pkjmast);
        }

        ////driver action to determine worker KWSP deduction
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult CalculateKWSP(String nopkj, Guid gajiGuid, Models.tbl_Pkjmast tbl_Pkjmast)
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
        //        NegaraID.Value);
        //    MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
        //    decimal potonganMajikan, potonganPekerja = 0;

        //    try
        //    {
        //        var workerData = dbr.tbl_Pkjmast
        //            .Single(x => x.fld_Nopkj.ToString() == nopkj && x.fld_LadangID == LadangID &&
        //                         x.fld_WilayahID == WilayahID && x.fld_NegaraID == NegaraID);

        //        var workerSalaryData = dbr.tbl_GajiBulanan
        //            .Single(x => x.fld_ID == gajiGuid);

        //        var carumanKwsp = db.tbl_Kwsp
        //            .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID
        //                        && x.fld_KodCaruman == workerData.fld_KodKWSP &&
        //                        x.fld_KdrLower <= workerSalaryData.fld_GajiKasar
        //                        && x.fld_KdrUpper >= workerSalaryData.fld_GajiKasar)
        //            .Select(s => new {s.fld_Pkj, s.fld_Mjkn})
        //            .FirstOrDefault();

        //        if (carumanKwsp == null)
        //        {
        //            var peratusCaruman = db.tbl_JenisCaruman
        //                .Where(x => x.fld_KodCaruman == workerData.fld_KodKWSP)
        //                .Select(s => new {s.fld_PeratusCarumanPekerja, s.fld_PeratusCarumanMajikanAtas5000})
        //                .Single();

        //            potonganPekerja = Math.Round(Convert.ToDecimal(workerSalaryData.fld_Gaji_Kasar) *
        //                                         Convert.ToDecimal(peratusCaruman.fld_PeratusCarumanPekerja), 2);
        //            potonganMajikan = Math.Round(Convert.ToDecimal(workerSalaryData.fld_Gaji_Kasar) *
        //                                         Convert.ToDecimal(peratusCaruman.fld_PeratusCarumanMajikanAtas5000),
        //                2);

        //            workerSalaryData.fld_Epf_Pkj = potonganPekerja;
        //            workerSalaryData.fld_Epf_Mjk = potonganMajikan;
        //        }

        //        else
        //        {
        //            workerSalaryData.fld_Epf_Pkj = Convert.ToDecimal(carumanKwsp.fld_Pkj);
        //            workerSalaryData.fld_Epf_Mjk = Convert.ToDecimal(carumanKwsp.fld_Mjkn);
        //        }

        //        var carumanSocso = db.tbl_Socso
        //            .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID
        //                        && x.fld_KodCaruman == workerData.fld_KodSocso &&
        //                        x.fld_KdrLower <= workerSalaryData.fld_Gaji_Kasar
        //                        && x.fld_KdrUpper >= workerSalaryData.fld_Gaji_Kasar)
        //            .Select(s => new {s.fld_SocsoPkj, s.fld_SocsoMjkn})
        //            .FirstOrDefault();

        //        if (carumanSocso == null)
        //        {
        //            var carumanSocsoMax = db.tbl_Socso
        //                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID
        //                            && x.fld_KodCaruman == workerData.fld_KodSocso &&
        //                            x.fld_KdrLower <= workerSalaryData.fld_Gaji_Kasar
        //                            && x.fld_KdrUpper == null)
        //                .Select(s => new {s.fld_SocsoPkj, s.fld_SocsoMjkn})
        //                .Single();

        //            workerSalaryData.fld_Socso_Pkj = carumanSocsoMax.fld_SocsoPkj;
        //            workerSalaryData.fld_Socso_Mjk = carumanSocsoMax.fld_SocsoMjkn;
        //        }

        //        else
        //        {
        //            workerSalaryData.fld_Socso_Pkj = carumanSocso.fld_SocsoPkj;
        //            workerSalaryData.fld_Socso_Mjk = carumanSocso.fld_SocsoMjkn;
        //        }

        //        return Json(new
        //        {
        //            success = true,
        //            msg = GlobalResEstate.msgUpdate,
        //            status = "success",
        //            checkingdata = "0",
        //            method = "1",
        //            data1 = "",
        //            data2 = ""
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
        //        return Json(new
        //        {
        //            success = true,
        //            msg = GlobalResEstate.msgError,
        //            status = "danger",
        //            checkingdata = "1"
        //        });

        //    }
        //}

        public ActionResult IncentiveInfo(string filter = "", int page = 1, string sort = "fld_Nama1",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            int month = timezone.gettimezone().Month;
            int year = timezone.gettimezone().Year;
            int rangeyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            var GroupList = new SelectList(
                dbview.tbl_KumpulanKerja
                    .Where(x => x.fld_deleted == false && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                x.fld_LadangID == LadangID)
                    .OrderBy(o => o.fld_KodKumpulan)
                    .Select(s => new SelectListItem
                    {
                        Value = s.fld_KumpulanID.ToString(),
                        Text = s.fld_KodKumpulan + " " + s.fld_Keterangan
                    }),
                "Value", "Text").ToList();
            GroupList.Insert(0, (new SelectListItem {Text = GlobalResEstate.lblAll, Value = "0"}));

            var yearlist = new List<SelectListItem>();
            for (var i = rangeyear; i <= year; i++)
            {
                if (i == timezone.gettimezone().Year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            var MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", month);


            ViewBag.GroupList = GroupList;

            ViewBag.MonthList = MonthList;
            ViewBag.YearList = yearlist;

            ViewBag.WorkerInfo = "class = active";

            ViewBag.filter = filter;

            return View();
        }

        public ViewResult _WorkerIncentiveSearch(string workerid, string filter = "", int YearList = 0, int MonthList = 0, int page = 1,
            string sort = "fld_Nama1",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            List<vw_MaklumatInsentifPekerja> MaklumatInsentifPekerja = new List<vw_MaklumatInsentifPekerja>();

            ViewBag.pageSize = int.Parse(GetConfig.GetData("paging"));

            if (!String.IsNullOrEmpty(filter))
            {
                var workerData = dbview.tbl_Pkjmast
                    .Where(x => x.fld_DivisionID == DivisionID && x.fld_Kdaktf == "1" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                x.fld_Nopkj.ToString().ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_Nama.ToUpper().Contains(filter.ToUpper()))
                    .OrderBy(x => x.fld_Nama);

                foreach (var i in workerData)
                {
                    var insentiveData = dbview.vw_MaklumatInsentif
                        .Where(a => a.fld_Nopkj == i.fld_Nopkj && a.fld_NegaraID == NegaraID &&
                                    a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                    a.fld_LadangID == LadangID && a.fld_Deleted == false && a.fld_Month == MonthList && a.fld_Year == YearList)
                        .OrderBy(x => x.fld_KodInsentif)
                        .ToList();
                    MaklumatInsentifPekerja.Add(new vw_MaklumatInsentifPekerja {Pkjmast = i, Insentif = insentiveData});
                }
            }

            else
            {
                var workerData = dbview.tbl_Pkjmast
                    .Where(x => x.fld_DivisionID == DivisionID && x.fld_Kdaktf == "1" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                    .OrderBy(x => x.fld_Nama);

                foreach (var i in workerData)
                {
                    var insentiveData = dbview.vw_MaklumatInsentif
                        .Where(a => a.fld_Nopkj == i.fld_Nopkj && a.fld_NegaraID == NegaraID &&
                                    a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                    a.fld_LadangID == LadangID && a.fld_Deleted == false && a.fld_Month == MonthList && a.fld_Year == YearList)
                        .OrderBy(x => x.fld_KodInsentif)
                        .ToList();
                    MaklumatInsentifPekerja.Add(new vw_MaklumatInsentifPekerja {Pkjmast = i, Insentif = insentiveData});
                }
            }

            return View(MaklumatInsentifPekerja);
        }

        public ActionResult _WorkerIncentiveAdd(string nopkj)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            int month = timezone.gettimezone().Month;
            int year = timezone.gettimezone().Year;
            int rangeyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            List<SelectListItem> incentiveAdditionTypeList = new List<SelectListItem>();

            incentiveAdditionTypeList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "jenisPenambahanInsentif" && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(
                        s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();
            incentiveAdditionTypeList.Insert(0, new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "" });

            ViewBag.IncentiveAdditionTypeList = incentiveAdditionTypeList;

            List<SelectListItem> incentiveCategoryList = new List<SelectListItem>();

            incentiveCategoryList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "jenisInsentif" && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(
                        s => new SelectListItem {Value = s.fldOptConfValue, Text = s.fldOptConfDesc}),
                "Value", "Text").ToList();
            incentiveCategoryList.Insert(0, new SelectListItem {Text = GlobalResEstate.lblChoose, Value = ""});

            ViewBag.IncentiveCategoryList = incentiveCategoryList;

            List<SelectListItem> incentiveList = new List<SelectListItem>();
            incentiveList.Insert(0, new SelectListItem {Text = GlobalResEstate.lblChoose, Value = ""});

            var yearlist = new List<SelectListItem>();
            for (var i = rangeyear; i <= year; i++)
            {
                if (i == timezone.gettimezone().Year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            var MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", month);


            ViewBag.IncentiveList = incentiveList;

            tbl_InsentifViewModelCreate insentif = new tbl_InsentifViewModelCreate();

            insentif.fld_Nopkj = nopkj;

            ViewBag.MonthList = MonthList;
            ViewBag.YearList = yearlist;

            return PartialView(insentif);
        }

        public JsonResult checkIncentiveRecord(string incentiveCategoryType, string nopkj, int MonthList, int YearList)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);

            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> incentiveAvailableForWorkerList = new List<SelectListItem>();

            if (String.IsNullOrEmpty(incentiveCategoryType))
            {
                return Json(new
                {
                    incentiveAvailableForWorkerList,
                    noData = true
                });
            }

            else
            {
                var workerData = dbview.tbl_Pkjmast
                    .Where(x => x.fld_Nopkj == nopkj && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                x.fld_Kdaktf == "1");

                var workerDesignation = workerData.Select(s => s.fld_Ktgpkj).SingleOrDefault();

                var incentiveEligibilityData = db.tbl_KelayakanInsentifPkjLdg
                    .Where(x => x.fld_KodPkj == workerDesignation &&
                                x.fld_KodInsentif.Contains(incentiveCategoryType)
                                && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                x.fld_LadangID == LadangID && x.fld_Deleted == false)
                    .Select(s => s.fld_KodInsentif)
                    .ToList();

                var incentiveData = db.tbl_JenisInsentif
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false);

                List<String> incentiveDataList = new List<String>();

                //if local worker
                if (GetConfig.GetWebConfigFlag2FromValue(workerData.Select(s => s.fld_Jenispekerja).SingleOrDefault(),
                        "jnsPkj", NegaraID, SyarikatID) == "1")
                {
                    incentiveDataList = incentiveData
                        .Where(x => x.fld_KelayakanKepada == 0 || x.fld_KelayakanKepada == 2)
                        .Select(s => s.fld_KodInsentif)
                        .ToList();
                }

                //foreign worker
                else
                {
                    incentiveDataList = incentiveData
                        .Where(x => x.fld_KelayakanKepada == 1 || x.fld_KelayakanKepada == 2)
                        .Select(s => s.fld_KodInsentif)
                        .ToList();
                }

                var trueIncentiveEligibility = incentiveDataList.Intersect(incentiveEligibilityData);

                var workerIncentiveDataList = dbview.vw_MaklumatInsentif
                    .Where(x => x.fld_Nopkj == nopkj && x.fld_Month == MonthList &&
                                x.fld_Year == YearList && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                x.fld_LadangID == LadangID && x.fld_Deleted == false)
                    .Select(s => s.fld_KodInsentif)
                    .ToList();

                var trueIncentiveEligibilityMinusExistingIncentive =
                    trueIncentiveEligibility.Except(workerIncentiveDataList);

                if (trueIncentiveEligibilityMinusExistingIncentive.ToList().Count == 0)
                {
                    incentiveAvailableForWorkerList.Insert(0,
                        new SelectListItem {Text = GlobalResEstate.lblNoIncentiveEligibility, Value = ""});
                }

                else
                {
                    incentiveAvailableForWorkerList.Insert(0,
                        new SelectListItem {Text = GlobalResEstate.lblChoose, Value = ""});
                }

                var i = 1;

                foreach (var incentive in trueIncentiveEligibilityMinusExistingIncentive)
                {
                    incentiveAvailableForWorkerList.Insert(i,
                        new SelectListItem
                        {

                            Value = incentive,
                            Text = incentiveData.Where(x => x.fld_KodInsentif == incentive)
                                .Select(s => s.fld_Keterangan)
                                .SingleOrDefault()
                        });
                    i++;
                }

                return Json(new
                {
                    incentiveAvailableForWorkerList,
                    noData = false
                });
            }
        }

        public JsonResult getIncentiveLimit(string incentiveCode)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);

            List<SelectListItem> incentiveAvailableForWorkerList = new List<SelectListItem>();

            incentiveAvailableForWorkerList.Insert(0,
                new SelectListItem {Text = GlobalResEstate.lblChoose, Value = ""});

            if (String.IsNullOrEmpty(incentiveCode))
            {
                return Json(new
                {
                    noData = true
                });
            }

            else
            {
                var incentiveData = db.tbl_JenisInsentif
                    .SingleOrDefault(x => x.fld_KodInsentif == incentiveCode && x.fld_NegaraID == NegaraID &&
                                          x.fld_SyarikatID == SyarikatID);

                return Json(new
                {
                    incentiveData,
                    noData = false
                });
            }
        }

        public JsonResult IsIncentiveWithinRange(decimal? fld_NilaiTidakTetap, string fld_KodInsentif)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);

            var incentiveData =
                db.tbl_JenisInsentif.SingleOrDefault(x => x.fld_KodInsentif == fld_KodInsentif &&
                                                          x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID);

            if (incentiveData.fld_MinValue <= fld_NilaiTidakTetap && incentiveData.fld_MaxValue >= fld_NilaiTidakTetap)
            {
                return Json(
                    true,
                    JsonRequestBehavior.AllowGet);
            }

            else
            {
                return Json(
                    false,
                    JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult getDailyIncentiveAmount(string incentiveCode, string nopkj)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            if (String.IsNullOrEmpty(incentiveCode))
            {
                return Json(new
                {
                    noData = true
                });
            }

            else
            {
                var incentiveData = db.tbl_JenisInsentif
                    .SingleOrDefault(x => x.fld_KodInsentif == incentiveCode && x.fld_NegaraID == NegaraID &&
                                          x.fld_SyarikatID == SyarikatID);

                return Json(new
                {
                    msg1 = GlobalResEstate.msgIncentiveMaximumValue,
                    msg2 = GlobalResEstate.lblDay1,
                    ratePerDay = incentiveData.fld_DailyFixedValue,
                    maxIncentive = incentiveData.fld_MaxValue,
                    noData = false
                });
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _WorkerIncentiveAdd(Models.tbl_InsentifViewModelCreate insentifViewModelCreate, int MonthList, int YearList)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            if (ModelState.IsValid)
            {
                try
                {
                    var currentDate = timezone.gettimezone();

                    if (insentifViewModelCreate.fld_JenisPenambahanInsentif == "0")

                    {
                        var ExistingIncentive = dbr.tbl_Insentif.Where(x =>
                            x.fld_Nopkj == insentifViewModelCreate.fld_Nopkj &&
                            x.fld_KodInsentif == insentifViewModelCreate.fld_KodInsentif &&
                            x.fld_Month == MonthList && x.fld_Year == YearList &&
                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID &&
                            x.fld_LadangID == LadangID && x.fld_Deleted == false).Count();

                        if (ExistingIncentive == 0)
                        {
                            tbl_Insentif insentif = new tbl_Insentif();

                            insentif.fld_Nopkj = insentifViewModelCreate.fld_Nopkj;
                            insentif.fld_KodInsentif = insentifViewModelCreate.fld_KodInsentif;
                            insentif.fld_Month = MonthList;
                            insentif.fld_Year = YearList;
                            insentif.fld_NegaraID = NegaraID;
                            insentif.fld_SyarikatID = SyarikatID;
                            insentif.fld_WilayahID = WilayahID;
                            insentif.fld_LadangID = LadangID;
                            insentif.fld_Deleted = false;
                            insentif.fld_CreatedBy = getuserid;
                            insentif.fld_CreatedDT = timezone.gettimezone();

                            if (insentifViewModelCreate.fld_TetapanNilai == 0)
                            {
                                if (GetConfig.GetIncentiveIsValidRange(insentifViewModelCreate.fld_KodInsentif,
                                    (decimal)insentifViewModelCreate.fld_NilaiTidakTetap, NegaraID, SyarikatID))
                                {
                                    insentif.fld_NilaiInsentif = insentifViewModelCreate.fld_NilaiTidakTetap;
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

                            else if (insentifViewModelCreate.fld_TetapanNilai == 1)
                            {
                                insentif.fld_NilaiInsentif = insentifViewModelCreate.fld_NilaiTetap;
                            }

                            else
                            {
                                insentif.fld_NilaiInsentif = insentifViewModelCreate.fld_NilaiHarian;
                            }

                            dbr.tbl_Insentif.Add(insentif);
                        }
                    }

                    else
                    {
                        var noOfMonthLeftInCurrentYear = (12 - timezone.gettimezone().Month) + 1;

                        for (int i = 0; i < noOfMonthLeftInCurrentYear; i++)
                        {
                            var currentMonth = currentDate.Month + i;

                            var ExistingIncentive = dbr.tbl_Insentif.Where(x =>
                                x.fld_Nopkj == insentifViewModelCreate.fld_Nopkj &&
                                x.fld_KodInsentif == insentifViewModelCreate.fld_KodInsentif &&
                                x.fld_Month == MonthList && x.fld_Year == YearList &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID &&
                                x.fld_LadangID == LadangID && x.fld_Deleted == false).Count();

                            if (ExistingIncentive == 0)
                            {
                                tbl_Insentif insentif = new tbl_Insentif();

                                insentif.fld_Nopkj = insentifViewModelCreate.fld_Nopkj;
                                insentif.fld_KodInsentif = insentifViewModelCreate.fld_KodInsentif;
                                insentif.fld_Month = MonthList;
                                insentif.fld_Year = YearList;
                                insentif.fld_NegaraID = NegaraID;
                                insentif.fld_SyarikatID = SyarikatID;
                                insentif.fld_WilayahID = WilayahID;
                                insentif.fld_LadangID = LadangID;
                                insentif.fld_Deleted = false;
                                insentif.fld_CreatedBy = getuserid;
                                insentif.fld_CreatedDT = timezone.gettimezone();

                                if (insentifViewModelCreate.fld_TetapanNilai == 0)
                                {
                                    if (GetConfig.GetIncentiveIsValidRange(insentifViewModelCreate.fld_KodInsentif,
                                        (decimal)insentifViewModelCreate.fld_NilaiTidakTetap, NegaraID, SyarikatID))
                                    {
                                        insentif.fld_NilaiInsentif = insentifViewModelCreate.fld_NilaiTidakTetap;
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

                                else if (insentifViewModelCreate.fld_TetapanNilai == 1)
                                {
                                    insentif.fld_NilaiInsentif = insentifViewModelCreate.fld_NilaiTetap;
                                }

                                else
                                {
                                    insentif.fld_NilaiInsentif = insentifViewModelCreate.fld_NilaiHarian;
                                }

                                dbr.tbl_Insentif.Add(insentif);
                            }
                        }
                    }

                    dbr.SaveChanges();

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
                        msg = GlobalResEstate.msgAdd,
                        status = "success",
                        checkingdata = "0",
                        method = "1",
                        div = "searchResultWorkerIncentiveInfo",
                        rootUrl = domain,
                        action = "_WorkerIncentiveSearch",
                        controller = "WorkerInfo"
                    });
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

        public ActionResult _WorkerIncentiveEdit(Guid insentif)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            tbl_InsentifViewModelEdit insentifViewModelEdit = new tbl_InsentifViewModelEdit();

            insentifViewModelEdit.fld_InsentifID = insentif;
            insentifViewModelEdit.fld_NilaiTidakTetap = dbr.tbl_Insentif
                .SingleOrDefault(u => u.fld_InsentifID == insentif && u.fld_WilayahID == WilayahID &&
                                      u.fld_SyarikatID == SyarikatID && u.fld_NegaraID == NegaraID &&
                                      u.fld_LadangID == LadangID && u.fld_Deleted == false)
                .fld_NilaiInsentif;
            insentifViewModelEdit.fld_KodInsentif = dbr.tbl_Insentif
                .SingleOrDefault(u => u.fld_InsentifID == insentif && u.fld_WilayahID == WilayahID &&
                                      u.fld_SyarikatID == SyarikatID && u.fld_NegaraID == NegaraID &&
                                      u.fld_LadangID == LadangID && u.fld_Deleted == false)
                .fld_KodInsentif;
            insentifViewModelEdit.fld_NegaraID = NegaraID;
            insentifViewModelEdit.fld_SyarikatID = SyarikatID;

            return PartialView(insentifViewModelEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _WorkerIncentiveEdit(tbl_InsentifViewModelEdit insentifViewModelEdit)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            if (ModelState.IsValid)
            {
                try
                {
                    var incentiveData = dbr.tbl_Insentif
                        .SingleOrDefault(x => x.fld_InsentifID == insentifViewModelEdit.fld_InsentifID &&
                                              x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                              x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                              x.fld_Deleted == false);

                    if (GetConfig.GetIncentiveIsValidRange(insentifViewModelEdit.fld_KodInsentif,
                        (decimal) insentifViewModelEdit.fld_NilaiTidakTetap, NegaraID, SyarikatID))
                    {
                        incentiveData.fld_NilaiInsentif = insentifViewModelEdit.fld_NilaiTidakTetap;

                        dbr.SaveChanges();

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
                            div = "searchResultWorkerIncentiveInfo",
                            rootUrl = domain,
                            action = "_WorkerIncentiveSearch",
                            controller = "WorkerInfo"
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

        public ActionResult _WorkerIncentiveDelete(Guid insentif)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            Models.vw_MaklumatInsentif vw_MaklumatInsentif = dbr.vw_MaklumatInsentif
                .SingleOrDefault(x => x.fld_InsentifID == insentif && x.fld_LadangID == LadangID &&
                                      x.fld_WilayahID == WilayahID && x.fld_NegaraID == NegaraID &&
                                      x.fld_Deleted == false);

            return PartialView(vw_MaklumatInsentif);
        }

        [HttpPost, ActionName("_WorkerIncentiveDelete")]
        public ActionResult _WorkerIncentiveDeleteConfirm(Guid insentif)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                var incentiveData = dbr.tbl_Insentif.SingleOrDefault(
                    x => x.fld_InsentifID == insentif && x.fld_LadangID == LadangID &&
                         x.fld_WilayahID == WilayahID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false);

                incentiveData.fld_Deleted = true;

                dbr.SaveChanges();

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
                    msg = GlobalResEstate.msgDelete2,
                    status = "success",
                    checkingdata = "0",
                    method = "1",
                    div = "searchResultWorkerIncentiveInfo",
                    rootUrl = domain,
                    action = "_WorkerIncentiveSearch",
                    controller = "WorkerInfo"
                });
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
        }

        public ActionResult GroupIncentiveInfo(string filter = "", int page = 1, string sort = "fld_KodKumpulan",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            ViewBag.WorkerInfo = "class = active";
            ViewBag.filter = filter;

            return View();
        }

        public ViewResult _GroupIncentiveSearch(string workerid, string filter = "", int page = 1,
            string sort = "fld_KodKumpulan",
            string sortdir = "ASC")
        {

            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.vw_KumpulanKerja>();
            int role = GetIdentity.RoleID(getuserid).Value;
            //ViewBag.filter = filter;

            var GroupList = dbview.vw_KumpulanKerja
                .Where(x => x.fld_DivisionID == DivisionID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false);

            if (!String.IsNullOrEmpty(filter))
            {
                records.Content = GroupList
                    .Where(x => x.fld_KodKumpulan.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_KodKerja.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_Keterangan.ToUpper().Contains(filter.ToUpper()))
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = GroupList
                    .Count(x => x.fld_KodKumpulan.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_KodKerja.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_Keterangan.ToUpper().Contains(filter.ToUpper()));
            }

            else
            {
                records.Content = GroupList
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = GroupList
                    .Count();
            }

            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.RoleID = role;
            ViewBag.pageSize = pageSize;
            //return View(records);
            return View(records);
        }

        public ActionResult _GroupMemberIncentiveInfo(int kumpulan, int page = 1, string sort = "fld_nama1",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            List<vw_MaklumatInsentifPekerja> MaklumatInsentifPekerja = new List<vw_MaklumatInsentifPekerja>();

            ViewBag.pageSize = int.Parse(GetConfig.GetData("paging"));

            var workerData = dbview.tbl_Pkjmast
                .Where(x => x.fld_DivisionID == DivisionID && x.fld_Kdaktf == "1" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_KumpulanID == kumpulan)
                .OrderBy(x => x.fld_Nama);

            foreach (var i in workerData)
            {
                var insentiveData = dbview.vw_MaklumatInsentif
                    .Where(a => a.fld_Nopkj == (i.fld_Nopkj) && a.fld_Deleted == false &&
                                a.fld_Month == DateTime.Now.Month && a.fld_Year == DateTime.Now.Year
                                && a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID &&
                                a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID)
                    .OrderBy(x => x.fld_KodInsentif)
                    .ToList();

                MaklumatInsentifPekerja.Add(new vw_MaklumatInsentifPekerja {Pkjmast = i, Insentif = insentiveData});
            }

            return View(MaklumatInsentifPekerja);
        }

        public ActionResult _GroupIncentiveAdd(int kumpulan)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            //MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            Models.vw_KumpulanKerja vw_KumpulanKerja = dbr.vw_KumpulanKerja
                .Single(u => u.fld_KumpulanID == kumpulan && u.fld_WilayahID == WilayahID &&
                             u.fld_SyarikatID == SyarikatID && u.fld_NegaraID == NegaraID && u.fld_deleted == false);

            tbl_GroupInsentifViewModelCreate GroupInsentifViewModelCreate = new tbl_GroupInsentifViewModelCreate
            {
                fld_KodKumpulan = vw_KumpulanKerja.fld_KodKumpulan,
                fld_KodKerja = vw_KumpulanKerja.fld_KodKerja,
                fld_Keterangan = vw_KumpulanKerja.fld_Keterangan,
                fld_KumpulanID = kumpulan
            };

            List<SelectListItem> incentiveList = new List<SelectListItem>();

            incentiveList = new SelectList(
                db.tbl_JenisInsentif
                    .Where(x => x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
                    .OrderBy(o => o.fld_KodInsentif)
                    .Select(
                        s => new SelectListItem { Value = s.fld_KodInsentif, Text = s.fld_Keterangan }),
                "Value", "Text").ToList();
            incentiveList.Insert(0, new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "" });

            ViewBag.IncentiveList = incentiveList;

            List<SelectListItem> incentiveAdditionTypeList = new List<SelectListItem>();

            incentiveAdditionTypeList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "jenisPenambahanInsentif" && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(
                        s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();
            incentiveAdditionTypeList.Insert(0, new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "" });

            ViewBag.IncentiveAdditionTypeList = incentiveAdditionTypeList;

            return PartialView(GroupInsentifViewModelCreate);
        }

        public JsonResult checkGroupIncentiveEligibility(string kodInsentif, string kodKumpulan)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            List<CustMod_GroupIncentiveEligibility> groupIncentiveEligibilityList =
                new List<CustMod_GroupIncentiveEligibility>();

            bool noData = false;

            var incentiveData = db.tbl_JenisInsentif
                .SingleOrDefault(x => x.fld_KodInsentif == kodInsentif && x.fld_NegaraID == NegaraID &&
                                      x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false);

            if (String.IsNullOrEmpty(kodInsentif))
            {
                noData = true;
            }

            else
            {
                var kumpulanId = dbr.tbl_KumpulanKerja
                    .Where(x => x.fld_KodKumpulan == kodKumpulan && x.fld_NegaraID == NegaraID &&
                                x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_LadangID == LadangID && x.fld_deleted == false)
                    .Select(s => s.fld_KumpulanID)
                    .Single();

                var allWorkerData = dbr.tbl_Pkjmast
                    .Where(x => x.fld_KumpulanID == kumpulanId &&
                                x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                x.fld_Kdaktf == "1");

                foreach (var workerData in allWorkerData)
                {
                    var workerIncentiveData = dbr.tbl_Insentif
                        .SingleOrDefault(x => x.fld_Nopkj == workerData.fld_Nopkj && x.fld_KodInsentif == kodInsentif &&
                                              x.fld_Month == DateTime.Now.Month &&
                                              x.fld_Year == DateTime.Now.Year && x.fld_NegaraID == NegaraID &&
                                              x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                              x.fld_LadangID == LadangID && x.fld_Deleted == false);

                    var incentiveEligilibityData = db.tbl_KelayakanInsentifPkjLdg
                        .SingleOrDefault(x => x.fld_KodPkj == workerData.fld_Ktgpkj &&
                                              x.fld_KodInsentif.Contains(kodInsentif) &&
                                              x.fld_NegaraID == NegaraID &&
                                              x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                              x.fld_LadangID == LadangID && x.fld_Deleted == false);

                    CustMod_GroupIncentiveEligibility groupIncentiveEligibility =
                        new CustMod_GroupIncentiveEligibility();

                    groupIncentiveEligibility.NoPkj = workerData.fld_Nopkj;
                    groupIncentiveEligibility.NamaPkj = workerData.fld_Nama;
                    groupIncentiveEligibility.Designation =
                        GetConfig.GetWebConfigDesc(workerData.fld_Ktgpkj, "designation", NegaraID, SyarikatID);

                    if (incentiveEligilibityData != null)
                    {
                        if (GetConfig.GetWebConfigFlag2FromValue(workerData.fld_Jenispekerja, "jnsPkj", NegaraID,
                                SyarikatID) == "1")
                        {
                            if (incentiveData.fld_KelayakanKepada == 0)
                            {
                                groupIncentiveEligibility.IsEligible = true;
                                groupIncentiveEligibility.IncentiveDesc = workerIncentiveData != null
                                    ? GetConfig.GetIncentiveDescFromCode(kodInsentif, NegaraID, SyarikatID)
                                    : GlobalResEstate.msgNoRecord;
                                groupIncentiveEligibility.Amount = workerIncentiveData != null
                                    ? workerIncentiveData.fld_NilaiInsentif.ToString()
                                    : GlobalResEstate.msgNoRecord;

                            }

                            else if (incentiveData.fld_KelayakanKepada == 1)
                            {
                                groupIncentiveEligibility.IsEligible = false;
                                groupIncentiveEligibility.IncentiveDesc = GlobalResEstate.lblNotQualify;
                                groupIncentiveEligibility.Amount = GlobalResEstate.lblNotQualify;
                            }

                            else
                            {
                                groupIncentiveEligibility.IsEligible = true;
                                groupIncentiveEligibility.IncentiveDesc = workerIncentiveData != null
                                    ? GetConfig.GetIncentiveDescFromCode(kodInsentif, NegaraID, SyarikatID)
                                    : GlobalResEstate.msgNoRecord;
                                groupIncentiveEligibility.Amount = workerIncentiveData != null
                                    ? workerIncentiveData.fld_NilaiInsentif.ToString()
                                    : GlobalResEstate.msgNoRecord;
                            }
                        }

                        else if (GetConfig.GetWebConfigFlag2FromValue(workerData.fld_Jenispekerja, "jnsPkj", NegaraID,
                                     SyarikatID) == "2")
                        {
                            if (incentiveData.fld_KelayakanKepada == 0)
                            {
                                groupIncentiveEligibility.IsEligible = false;
                                groupIncentiveEligibility.IncentiveDesc = GlobalResEstate.lblNotQualify;
                                groupIncentiveEligibility.Amount = GlobalResEstate.lblNotQualify;
                            }

                            else if (incentiveData.fld_KelayakanKepada == 1)
                            {
                                groupIncentiveEligibility.IsEligible = true;
                                groupIncentiveEligibility.IncentiveDesc = workerIncentiveData != null
                                    ? GetConfig.GetIncentiveDescFromCode(kodInsentif, NegaraID, SyarikatID)
                                    : GlobalResEstate.msgNoRecord;
                                groupIncentiveEligibility.Amount = workerIncentiveData != null
                                    ? workerIncentiveData.fld_NilaiInsentif.ToString()
                                    : GlobalResEstate.msgNoRecord;
                            }

                            else
                            {
                                groupIncentiveEligibility.IsEligible = true;
                                groupIncentiveEligibility.IncentiveDesc = workerIncentiveData != null
                                    ? GetConfig.GetIncentiveDescFromCode(kodInsentif, NegaraID, SyarikatID)
                                    : GlobalResEstate.msgNoRecord;
                                groupIncentiveEligibility.Amount = workerIncentiveData != null
                                    ? workerIncentiveData.fld_NilaiInsentif.ToString()
                                    : GlobalResEstate.msgNoRecord;
                            }
                        }
                    }

                    else
                    {
                        groupIncentiveEligibility.IsEligible = false;
                        groupIncentiveEligibility.IncentiveDesc = GlobalResEstate.lblNotQualify;
                        groupIncentiveEligibility.Amount = GlobalResEstate.lblNotQualify;
                    }

                    groupIncentiveEligibilityList.Add(groupIncentiveEligibility);
                }
            }

            return Json(new
            {
                noData,
                groupIncentiveEligibilityList,
                incentiveData,
                incentiveDesc = GetConfig.GetIncentiveDescFromCode(kodInsentif, NegaraID, SyarikatID)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _GroupIncentiveAdd(tbl_GroupInsentifViewModelCreate GroupInsentifViewModelCreate)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                if (ModelState.IsValid)
                {
                    var currentDate = timezone.gettimezone();

                    var incentiveData = db.tbl_JenisInsentif
                        .SingleOrDefault(x => x.fld_KodInsentif == GroupInsentifViewModelCreate.fld_KodInsentif &&
                                              x.fld_NegaraID == NegaraID &&
                                              x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false);

                    var allWorkerData = dbr.tbl_Pkjmast
                        .Where(x => x.fld_KumpulanID == GroupInsentifViewModelCreate.fld_KumpulanID &&
                                    x.fld_NegaraID == NegaraID &&
                                    x.fld_SyarikatID == SyarikatID &&
                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                    x.fld_Kdaktf == "1");

                    foreach (var workerData in allWorkerData)
                    {
                        var workerIncentiveData = dbr.tbl_Insentif
                            .SingleOrDefault(x => x.fld_Nopkj == workerData.fld_Nopkj &&
                                                  x.fld_KodInsentif == GroupInsentifViewModelCreate.fld_KodInsentif &&
                                                  x.fld_Month == DateTime.Now.Month &&
                                                  x.fld_Year == DateTime.Now.Year && x.fld_NegaraID == NegaraID &&
                                                  x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                                  x.fld_LadangID == LadangID && x.fld_Deleted == false);

                        var incentiveEligilibityData = db.tblOptionConfigsWebs
                            .SingleOrDefault(x => x.fldOptConfValue == workerData.fld_Ktgpkj &&
                                                  x.fldOptConfFlag2.Contains(GroupInsentifViewModelCreate
                                                      .fld_KodInsentif) &&
                                                  x.fldOptConfFlag1 == "kelayakanInsentif" &&
                                                  x.fld_NegaraID == NegaraID &&
                                                  x.fld_SyarikatID == SyarikatID && x.fldDeleted == false);

                        if (workerIncentiveData == null)
                        {
                            if (incentiveEligilibityData != null)
                            {
                                if (GetConfig.GetWebConfigFlag2FromValue(workerData.fld_Jenispekerja, "jnsPkj", NegaraID, SyarikatID) == "1")
                                {
                                    if (incentiveData.fld_KelayakanKepada == 0 || incentiveData.fld_KelayakanKepada == 2)
                                    {
                                        if (GroupInsentifViewModelCreate.fld_JenisPenambahanInsentif == "0")
                                        {
                                            var ExistingIncentive = dbr.tbl_Insentif.Where(x =>
                                                x.fld_Nopkj == workerData.fld_Nopkj &&
                                                x.fld_KodInsentif == GroupInsentifViewModelCreate.fld_KodInsentif &&
                                                x.fld_Month == currentDate.Month && x.fld_Year == currentDate.Year &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID &&
                                                x.fld_LadangID == LadangID && x.fld_Deleted == false).Count();

                                            if (ExistingIncentive == 0)
                                            {
                                                tbl_Insentif insentif = new tbl_Insentif();

                                                insentif.fld_Nopkj = workerData.fld_Nopkj;
                                                insentif.fld_Month = DateTime.Now.Month;
                                                insentif.fld_Year = DateTime.Now.Year;
                                                insentif.fld_KodInsentif = GroupInsentifViewModelCreate.fld_KodInsentif;
                                                insentif.fld_NegaraID = NegaraID;
                                                insentif.fld_SyarikatID = SyarikatID;
                                                insentif.fld_WilayahID = WilayahID;
                                                insentif.fld_LadangID = LadangID;
                                                insentif.fld_Deleted = false;
                                                insentif.fld_CreatedBy = getuserid;
                                                insentif.fld_CreatedDT = timezone.gettimezone();

                                                if (incentiveData.fld_TetapanNilai == 0)
                                                {
                                                    insentif.fld_NilaiInsentif =
                                                        GroupInsentifViewModelCreate.fld_NilaiTidakTetap;
                                                }

                                                else if (incentiveData.fld_TetapanNilai == 1)
                                                {
                                                    insentif.fld_NilaiInsentif =
                                                        GroupInsentifViewModelCreate.fld_NilaiTetap;
                                                }

                                                else
                                                {
                                                    var absentData = dbr.tbl_Kerjahdr.Count(
                                                        x => x.fld_Nopkj == workerData.fld_Nopkj &&
                                                             x.fld_Tarikh.Value.Month == DateTime.Now.Month &&
                                                             x.fld_Tarikh.Value.Year == DateTime.Now.Year &&
                                                             x.fld_Kdhdct == "P01");

                                                    insentif.fld_NilaiInsentif =
                                                        GroupInsentifViewModelCreate.fld_NilaiHarian -
                                                        (absentData * incentiveData.fld_DailyFixedValue);
                                                }

                                                dbr.tbl_Insentif.Add(insentif);
                                            }
                                        }

                                        else
                                        {
                                            var noOfMonthLeftInCurrentYear = (12 - timezone.gettimezone().Month) + 1;

                                            for (int i = 0; i < noOfMonthLeftInCurrentYear; i++)
                                            {
                                                var currentMonth = currentDate.Month + i;


                                                var ExistingIncentive = dbr.tbl_Insentif.Where(x =>
                                                    x.fld_Nopkj == workerData.fld_Nopkj &&
                                                    x.fld_KodInsentif == GroupInsentifViewModelCreate.fld_KodInsentif &&
                                                    x.fld_Month == currentMonth && x.fld_Year == currentDate.Year &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID &&
                                                    x.fld_LadangID == LadangID && x.fld_Deleted == false).Count();

                                                if (ExistingIncentive == 0)
                                                {
                                                    tbl_Insentif insentif = new tbl_Insentif();

                                                    insentif.fld_Nopkj = workerData.fld_Nopkj;
                                                    insentif.fld_KodInsentif = GroupInsentifViewModelCreate.fld_KodInsentif;
                                                    insentif.fld_Month = currentMonth;
                                                    insentif.fld_Year = currentDate.Year;
                                                    insentif.fld_NegaraID = NegaraID;
                                                    insentif.fld_SyarikatID = SyarikatID;
                                                    insentif.fld_WilayahID = WilayahID;
                                                    insentif.fld_LadangID = LadangID;
                                                    insentif.fld_Deleted = false;
                                                    insentif.fld_CreatedBy = getuserid;
                                                    insentif.fld_CreatedDT = timezone.gettimezone();

                                                    if (incentiveData.fld_TetapanNilai == 0)
                                                    {
                                                        insentif.fld_NilaiInsentif =
                                                            GroupInsentifViewModelCreate.fld_NilaiTidakTetap;
                                                    }

                                                    else if (incentiveData.fld_TetapanNilai == 1)
                                                    {
                                                        insentif.fld_NilaiInsentif =
                                                            GroupInsentifViewModelCreate.fld_NilaiTetap;
                                                    }

                                                    else
                                                    {
                                                        var absentData = dbr.tbl_Kerjahdr.Count(
                                                            x => x.fld_Nopkj == workerData.fld_Nopkj &&
                                                                 x.fld_Tarikh.Value.Month == DateTime.Now.Month &&
                                                                 x.fld_Tarikh.Value.Year == DateTime.Now.Year &&
                                                                 x.fld_Kdhdct == "P01");

                                                        insentif.fld_NilaiInsentif =
                                                            GroupInsentifViewModelCreate.fld_NilaiHarian -
                                                            (absentData * incentiveData.fld_DailyFixedValue);
                                                    }

                                                    dbr.tbl_Insentif.Add(insentif);
                                                }
                                            }
                                        }
                                    }
                                }

                                else
                                {
                                    if (incentiveData.fld_KelayakanKepada == 1 || incentiveData.fld_KelayakanKepada == 2)
                                    {
                                        if (GroupInsentifViewModelCreate.fld_JenisPenambahanInsentif == "0")
                                        {
                                            var ExistingIncentive = dbr.tbl_Insentif.Where(x =>
                                                x.fld_Nopkj == workerData.fld_Nopkj &&
                                                x.fld_KodInsentif == GroupInsentifViewModelCreate.fld_KodInsentif &&
                                                x.fld_Month == currentDate.Month && x.fld_Year == currentDate.Year &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID &&
                                                x.fld_LadangID == LadangID && x.fld_Deleted == false).Count();

                                            if (ExistingIncentive == 0)
                                            {
                                                tbl_Insentif insentif = new tbl_Insentif();

                                                insentif.fld_Nopkj = workerData.fld_Nopkj;
                                                insentif.fld_Month = DateTime.Now.Month;
                                                insentif.fld_Year = DateTime.Now.Year;
                                                insentif.fld_KodInsentif = GroupInsentifViewModelCreate.fld_KodInsentif;
                                                insentif.fld_NegaraID = NegaraID;
                                                insentif.fld_SyarikatID = SyarikatID;
                                                insentif.fld_WilayahID = WilayahID;
                                                insentif.fld_LadangID = LadangID;
                                                insentif.fld_Deleted = false;
                                                insentif.fld_CreatedBy = getuserid;
                                                insentif.fld_CreatedDT = timezone.gettimezone();

                                                if (incentiveData.fld_TetapanNilai == 0)
                                                {
                                                    insentif.fld_NilaiInsentif =
                                                        GroupInsentifViewModelCreate.fld_NilaiTidakTetap;
                                                }

                                                else if (incentiveData.fld_TetapanNilai == 1)
                                                {
                                                    insentif.fld_NilaiInsentif =
                                                        GroupInsentifViewModelCreate.fld_NilaiTetap;
                                                }

                                                else
                                                {
                                                    var absentData = dbr.tbl_Kerjahdr.Count(
                                                        x => x.fld_Nopkj == workerData.fld_Nopkj &&
                                                             x.fld_Tarikh.Value.Month == DateTime.Now.Month &&
                                                             x.fld_Tarikh.Value.Year == DateTime.Now.Year &&
                                                             x.fld_Kdhdct == "P01");

                                                    insentif.fld_NilaiInsentif =
                                                        GroupInsentifViewModelCreate.fld_NilaiHarian -
                                                        (absentData * incentiveData.fld_DailyFixedValue);
                                                }

                                                dbr.tbl_Insentif.Add(insentif);
                                            }
                                        }

                                        else
                                        {
                                            var noOfMonthLeftInCurrentYear = (12 - timezone.gettimezone().Month) + 1;

                                            for (int i = 0; i < noOfMonthLeftInCurrentYear; i++)
                                            {
                                                var currentMonth = currentDate.Month + i;


                                                var ExistingIncentive = dbr.tbl_Insentif.Where(x =>
                                                    x.fld_Nopkj == workerData.fld_Nopkj &&
                                                    x.fld_KodInsentif == GroupInsentifViewModelCreate.fld_KodInsentif &&
                                                    x.fld_Month == currentMonth && x.fld_Year == currentDate.Year &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID &&
                                                    x.fld_LadangID == LadangID && x.fld_Deleted == false).Count();

                                                if (ExistingIncentive == 0)
                                                {
                                                    tbl_Insentif insentif = new tbl_Insentif();

                                                    insentif.fld_Nopkj = workerData.fld_Nopkj;
                                                    insentif.fld_KodInsentif = GroupInsentifViewModelCreate.fld_KodInsentif;
                                                    insentif.fld_Month = currentMonth;
                                                    insentif.fld_Year = currentDate.Year;
                                                    insentif.fld_NegaraID = NegaraID;
                                                    insentif.fld_SyarikatID = SyarikatID;
                                                    insentif.fld_WilayahID = WilayahID;
                                                    insentif.fld_LadangID = LadangID;
                                                    insentif.fld_Deleted = false;
                                                    insentif.fld_CreatedBy = getuserid;
                                                    insentif.fld_CreatedDT = timezone.gettimezone();

                                                    if (incentiveData.fld_TetapanNilai == 0)
                                                    {
                                                        insentif.fld_NilaiInsentif =
                                                            GroupInsentifViewModelCreate.fld_NilaiTidakTetap;
                                                    }

                                                    else if (incentiveData.fld_TetapanNilai == 1)
                                                    {
                                                        insentif.fld_NilaiInsentif =
                                                            GroupInsentifViewModelCreate.fld_NilaiTetap;
                                                    }

                                                    else
                                                    {
                                                        var absentData = dbr.tbl_Kerjahdr.Count(
                                                            x => x.fld_Nopkj == workerData.fld_Nopkj &&
                                                                 x.fld_Tarikh.Value.Month == DateTime.Now.Month &&
                                                                 x.fld_Tarikh.Value.Year == DateTime.Now.Year &&
                                                                 x.fld_Kdhdct == "P01");

                                                        insentif.fld_NilaiInsentif =
                                                            GroupInsentifViewModelCreate.fld_NilaiHarian -
                                                            (absentData * incentiveData.fld_DailyFixedValue);
                                                    }

                                                    dbr.tbl_Insentif.Add(insentif);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    dbr.SaveChanges();

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
                        msg = GlobalResEstate.msgAdd,
                        status = "success",
                        checkingdata = "0",
                        method = "1",
                        div = "searchResult",
                        rootUrl = domain,
                        action = "_GroupIncentiveSearch",
                        controller = "WorkerInfo"
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
        }

        public ActionResult ProductivityInfo(string filter = "", int page = 1, string sort = "fld_KodKumpulan",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            ViewBag.WorkerInfo = "class = active";
            ViewBag.filter = filter;

            return View();
        }

        public ActionResult PopulateWeekType()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            //string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            MVC_SYSTEM_Viewing dbview2 = new MVC_SYSTEM_Viewing();

            try
            {
                int year = DateTime.Now.Year;

                //get list of month
                var monthList = db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "monthlist" && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                    .Select(s => s.fldOptConfValue)
                    .ToList();

                //get all kod negeri in malaysia
                var kodNegeriList = dbview2.vw_MingguNegeri.ToList();

                //loop every negeri
                foreach (var kodNegeri in kodNegeriList)
                {
                    int kodNegeriAhad = Convert.ToInt32(kodNegeri.fldOptConfValue);

                    if (kodNegeri.fld_JenisMinggu == 1)
                    {
                        //loop every month
                        foreach (var month in monthList)
                        {
                            var cutiUmum = db.tbl_CutiUmum
                                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                            x.fld_Deleted == false && x.fld_Negeri == kodNegeriAhad &&
                                            x.fld_Tahun == year &&
                                            x.fld_TarikhCuti.Value.Month.ToString() == month)
                                .ToList();

                            //First We find out last date of mont
                            //DateTime today = DateTime.Today;
                            DateTime endOfMonth = new DateTime(year, Convert.ToInt32(month),
                                DateTime.DaysInMonth(year, Convert.ToInt32(month)));
                            //get only last day of month
                            int day = endOfMonth.Day;

                            int sunday = 0;

                            for (int i = 0; i < day; ++i)
                            {
                                DateTime d = new DateTime(year, Convert.ToInt32(month), i + 1);


                                //Compare date with sunday
                                if (d.DayOfWeek == DayOfWeek.Sunday)
                                {
                                    sunday = sunday + 1;
                                }
                            }

                            var workingDaysInMonth = (DateTime.DaysInMonth(year, Convert.ToInt32(month))) - sunday -
                                                     cutiUmum.Count;

                            MasterModels.tbl_HariBekerja hariBekerjaMinggu1 = new MasterModels.tbl_HariBekerja();

                            hariBekerjaMinggu1.fld_Month = Convert.ToInt16(month);
                            hariBekerjaMinggu1.fld_Year = Convert.ToInt16(year);
                            hariBekerjaMinggu1.fld_BilanganHariBekerja = Convert.ToInt16(workingDaysInMonth);
                            hariBekerjaMinggu1.fld_JenisMinggu = kodNegeri.fld_JenisMinggu;
                            hariBekerjaMinggu1.fld_NegaraID = NegaraID;
                            hariBekerjaMinggu1.fld_NegeriID = kodNegeriAhad;
                            hariBekerjaMinggu1.fld_SyarikatID = SyarikatID;
                            hariBekerjaMinggu1.fld_Deleted = false;

                            db.tbl_HariBekerja.Add(hariBekerjaMinggu1);
                            db.SaveChanges();
                        }
                    }

                    if (kodNegeri.fld_JenisMinggu == 2)
                    {
                        foreach (var month in monthList)
                        {
                            int kodNegeriJumaat = Convert.ToInt32(kodNegeri.fldOptConfValue);

                            var cutiUmum = db.tbl_CutiUmum
                                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                            x.fld_Deleted == false && x.fld_Negeri == kodNegeriJumaat &&
                                            x.fld_Tahun == year &&
                                            x.fld_TarikhCuti.Value.Month.ToString() == month)
                                .ToList();

                            //First We find out last date of mont
                            //DateTime today = DateTime.Today;
                            DateTime endOfMonth = new DateTime(year, Convert.ToInt32(month),
                                DateTime.DaysInMonth(year, Convert.ToInt32(month)));
                            //get only last day of month
                            int day = endOfMonth.Day;

                            DateTime now = DateTime.Now;
                            int friday = 0;

                            for (int i = 0; i < day; ++i)
                            {
                                DateTime d = new DateTime(year, Convert.ToInt32(month), i + 1);
                                //Compare date with sunday
                                if (d.DayOfWeek == DayOfWeek.Friday)
                                {
                                    friday = friday + 1;
                                }
                            }

                            var workingDaysInMonth = (DateTime.DaysInMonth(year, Convert.ToInt32(month))) - friday - cutiUmum.Count;

                            MasterModels.tbl_HariBekerja hariBekerjaMinggu2 = new MasterModels.tbl_HariBekerja();

                            hariBekerjaMinggu2.fld_Month = Convert.ToInt16(month);
                            hariBekerjaMinggu2.fld_Year = Convert.ToInt16(year);
                            hariBekerjaMinggu2.fld_BilanganHariBekerja = Convert.ToInt16(workingDaysInMonth);
                            hariBekerjaMinggu2.fld_JenisMinggu = kodNegeri.fld_JenisMinggu;
                            hariBekerjaMinggu2.fld_NegaraID = NegaraID;
                            hariBekerjaMinggu2.fld_NegeriID = kodNegeriJumaat;
                            hariBekerjaMinggu2.fld_SyarikatID = SyarikatID;
                            hariBekerjaMinggu2.fld_Deleted = false;

                            db.tbl_HariBekerja.Add(hariBekerjaMinggu2);
                            db.SaveChanges();
                        }
                    }
                }

            }

            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new
                {
                    success = true,
                    msg = GlobalResEstate.msgError,
                    status = "danger",
                    checkingdata = "1"
                });
            }

            //ViewBag.WorkerInfo = "class = active";

            return Json(new
            {
                success = true,
                msg = "Maklumat hari bekerja bulanan berjaya dijana",
                status = "success",
                checkingdata = "1"
            });
        }

        public ActionResult _WorkerProductivitySearch(Guid? workerid, string filter = "", int page = 1,
            string sort = "fld_Nama",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Viewing dbr = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);


            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.vw_MaklumatProduktiviti>();
            int role = GetIdentity.RoleID(getuserid).Value;
            //ViewBag.filter = filter;
            ViewBag.workerid = workerid;

            var workerData = dbr.tbl_Pkjmast
                .Where(x => x.fld_DivisionID == DivisionID && x.fld_NegaraID == NegaraID &&
                            x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_LadangID == LadangID && x.fld_Kdaktf == "1")
                .ToList();

            var kodNegeriLadang = db.tbl_Ladang
                .Where(x => x.fld_ID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WlyhID == WilayahID && x.fld_Deleted == false)
                .Select(s => s.fld_KodNegeri)
                .Single();

            int intkodNegeriLadang = int.Parse(kodNegeriLadang);

            var jenisMinggu = db.tbl_MingguNegeri
                //.Where(x => x.fld_NegeriID == intkodNegeriLadang && x.fld_NegaraID == NegaraID &&
                //.Where(x => x.fld_NegeriID.ToString() == kodNegeriLadang && x.fld_NegaraID == NegaraID &&
                //.Where(x => x.fld_NegeriID == intkodNegeriLadang && x.fld_NegaraID == NegaraID &&
                .Where(x => x.fld_NegeriID == intkodNegeriLadang && x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
                .Select(s => s.fld_JenisMinggu)
                .Single();

            List<SelectListItem> jenisPelanList = new List<SelectListItem>();

            jenisPelanList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "jenisPelan" && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(
                        s => new SelectListItem {Value = s.fldOptConfValue, Text = s.fldOptConfDesc}),
                "Value", "Text").ToList();
            jenisPelanList.Insert(0, new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "" });

            ViewBag.jenisPelanList = jenisPelanList;

            List<SelectListItem> UnitList = new List<SelectListItem>();

            UnitList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "unit" && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(
                        s => new SelectListItem {Value = s.fldOptConfValue, Text = s.fldOptConfDesc}),
                "Value", "Text").ToList();

            UnitList.Insert(0, new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "" });

            ViewBag.UnitList = UnitList;

            var hadirKerja = db.tbl_HariBekerja
                .Where(x => x.fld_Month == DateTime.Now.Month && x.fld_Year == DateTime.Now.Year &&
                            x.fld_JenisMinggu == jenisMinggu && x.fld_NegeriID == intkodNegeriLadang &&
                            x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
                .Select(s => s.fld_BilanganHariBekerja)
                .SingleOrDefault();

            var ProductivityList = dbr.vw_MaklumatProduktiviti
                .Where(x => x.fld_Month == DateTime.Now.Month && x.fld_Year == DateTime.Now.Year &&
                            x.fld_NegaraID == NegaraID && x.fld_WilayahID == WilayahID &&
                            x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID && x.fld_Deleted == false)
                .ToList();

            List<String> workerWithoutProductivity = workerData.Select(s => s.fld_Nopkj)
                .Except(ProductivityList.Select(s => s.fld_Nopkj))
                .ToList();

            foreach (var nopkj in workerWithoutProductivity)
            {
                var worker = dbr.tbl_Pkjmast
                    .Where(x => x.fld_Nopkj == nopkj && x.fld_NegaraID == NegaraID &&
                                x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_LadangID == LadangID && x.fld_Kdaktf == "1");

                ProductivityList.Add(
                    new vw_MaklumatProduktiviti
                    {
                        fld_Nopkj = nopkj,
                        fld_Nama = worker.Select(s => s.fld_Nama).Single(),
                        fld_Nokp = worker.Select(s => s.fld_Nokp).Single(),
                        fld_Targetharian = 0,
                        fld_Unit = "",
                        fld_HadirKerja = 0,
                        fld_JenisPelan = ""
                    });
            }

            foreach (var pkj in ProductivityList)
            {
                pkj.fld_HadirKerja = hadirKerja;
            }

            if (!String.IsNullOrEmpty(filter))
            {
                records.Content = ProductivityList
                    .Where(x => x.fld_Nopkj.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_Nama.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_Nokp.ToUpper().Contains(filter.ToUpper()))
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = ProductivityList
                    .Count(x => x.fld_Nopkj.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_Nama.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_Nokp.ToUpper().Contains(filter.ToUpper()));
            }

            else
            {
                records.Content = ProductivityList
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = ProductivityList
                    .Count();

            }

            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.RoleID = role;
            ViewBag.pageSize = pageSize;
            return PartialView(records);
        }

        public JsonResult addProductivityInfo(string nopkj, string jenisPelan, decimal? targetHarian, int? hadirKerja, string unit)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            if (String.IsNullOrEmpty(jenisPelan) || String.IsNullOrEmpty(targetHarian.ToString()) || String.IsNullOrEmpty(unit))
            {
                return Json(new
                {
                    success = true,
                    msg = GlobalResEstate.msgErrorData,
                    status = "danger",
                    checkingdata = "1"
                });
            }

            else
            {
                try
                {
                    var checkProductivityRecord = dbr.tbl_Produktiviti
                                        .Where(x => x.fld_Nopkj == nopkj && x.fld_Year == DateTime.Now.Year &&
                                                    x.fld_Month == DateTime.Now.Month && x.fld_NegaraID == NegaraID &&
                                                    x.fld_SyarikatID == SyarikatID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();

                    if (checkProductivityRecord.Count == 0)
                    {
                        Models.tbl_Produktiviti Produktiviti = new Models.tbl_Produktiviti();

                        Produktiviti.fld_Nopkj = nopkj.Trim();
                        Produktiviti.fld_JenisPelan = jenisPelan;
                        Produktiviti.fld_Targetharian = targetHarian;
                        Produktiviti.fld_Unit = unit;
                        Produktiviti.fld_HadirKerja = hadirKerja;
                        Produktiviti.fld_Year = timezone.gettimezone().Year;
                        Produktiviti.fld_Month = timezone.gettimezone().Month;
                        Produktiviti.fld_NegaraID = NegaraID;
                        Produktiviti.fld_SyarikatID = SyarikatID;
                        Produktiviti.fld_WilayahID = WilayahID;
                        Produktiviti.fld_LadangID = LadangID;
                        Produktiviti.fld_Deleted = false;
                        Produktiviti.fld_CreatedBy = getuserid;
                        Produktiviti.fld_CreatedDT = timezone.gettimezone();

                        dbr.tbl_Produktiviti.Add(Produktiviti);
                    }

                    else
                    {
                        var materializeProductivityRec = checkProductivityRecord.Single();

                        materializeProductivityRec.fld_JenisPelan = jenisPelan;
                        materializeProductivityRec.fld_Targetharian = targetHarian;
                        materializeProductivityRec.fld_Unit = unit;
                        materializeProductivityRec.fld_HadirKerja = hadirKerja;

                        dbr.Entry(materializeProductivityRec).State = EntityState.Modified;
                        dbr.SaveChanges();
                    }

                    dbr.SaveChanges();

                    return Json(new
                    {
                        success = true,
                        msg = GlobalResEstate.msgAdd,
                        status = "success",
                        checkingdata = "1"
                    });
                }

                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new
                    {
                        success = false,
                        msg = GlobalResEstate.msgErrorData,
                        status = "danger",
                        checkingdata = "0"
                    });
                }
            }
        }

        public JsonResult checkCategoryType(string jenisPelan)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            //MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            List<SelectListItem> UnitList = new List<SelectListItem>();

            if (jenisPelan == "A")
            {
                UnitList = new SelectList(
                    db.tblOptionConfigsWebs
                        .Where(x => x.fldOptConfFlag1 == "unit" && x.fldOptConfFlag2 == "A" &&
                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                        .OrderBy(o => o.fldOptConfDesc)
                        .Select(
                            s => new SelectListItem {Value = s.fldOptConfValue, Text = s.fldOptConfDesc}),
                    "Value", "Text").ToList();
            }

            else if (jenisPelan == "B")
            {
                UnitList = new SelectList(
                    db.tblOptionConfigsWebs
                        .Where(x => x.fldOptConfFlag1 == "unit" && x.fldOptConfFlag2 == "B" &&
                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                    x.fldDeleted == false)
                        .OrderBy(o => o.fldOptConfDesc)
                        .Select(
                            s => new SelectListItem {Value = s.fldOptConfValue, Text = s.fldOptConfDesc}),
                    "Value", "Text").ToList();
                UnitList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "" }));
            }

            else
            {
                UnitList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "" }));
            }

            return Json(new { UnitList = UnitList });
        }


        public ActionResult GroupProductivityInfo(string filter = "", int page = 1, string sort = "fld_KodKumpulan",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            ViewBag.WorkerInfo = "class = active";
            ViewBag.filter = filter;

            return View();
        }

        public ActionResult _GroupProductivitySearch(Guid? workerid, string filter = "", int page = 1,
            string sort = "fld_KodKumpulan",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.vw_KumpulanKerja>();
            int role = GetIdentity.RoleID(getuserid).Value;
            //ViewBag.filter = filter;

            var GroupList = dbview.vw_KumpulanKerja
                .Where(x => x.fld_DivisionID == DivisionID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false);

            if (!String.IsNullOrEmpty(filter))
            {
                records.Content = GroupList
                    .Where(x => x.fld_KodKumpulan.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_KodKerja.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_Keterangan.ToUpper().Contains(filter.ToUpper()))
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = GroupList
                    .Count(x => x.fld_KodKumpulan.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_KodKerja.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_Keterangan.ToUpper().Contains(filter.ToUpper()));
            }

            else
            {
                records.Content = GroupList
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = GroupList
                    .Count();

            }

            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.RoleID = role;
            ViewBag.pageSize = pageSize;
            //return View(records);
            return View(records);
        }

        public ActionResult _GroupProductivityAdd(int kumpulan)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            var groupData = dbview.tbl_KumpulanKerja
                .Single(x => x.fld_KumpulanID == kumpulan);

            //var workerData = dbview.tbl_Pkjmast
            //    .Where(x => x.fld_KumpulanID == kumpulan && x.fld_NegaraID == NegaraID &&
            //                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
            //                x.fld_LadangID == LadangID && x.fld_Kdaktf == "1")
            //    .OrderBy(o => o.fld_Nama);
            
            var kategoriDataList = db.tblOptionConfigsWebs
                .Where(b => b.fldOptConfFlag1 == "jenisPelan" && b.fldDeleted == false && b.fld_NegaraID == NegaraID &&
                            b.fld_SyarikatID == SyarikatID);

            var kodNegeriLadang = db.tbl_Ladang
                .Where(x => x.fld_ID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WlyhID == WilayahID && x.fld_Deleted == false)
                .Select(s => s.fld_KodNegeri)
                .Single();

            int intkodNegeriLadang = int.Parse(kodNegeriLadang);

            var jenisMinggu = db.tbl_MingguNegeri
                .Where(x => x.fld_NegeriID == intkodNegeriLadang && x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
                .Select(s => s.fld_JenisMinggu)
                .Single();

            var hadirKerja = db.tbl_HariBekerja
                .Where(x => x.fld_Month == DateTime.Now.Month && x.fld_Year == DateTime.Now.Year &&
                            x.fld_JenisMinggu == jenisMinggu && x.fld_NegeriID == intkodNegeriLadang &&
                            x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
                .Select(s => s.fld_BilanganHariBekerja)
                .SingleOrDefault();

            var existingProductivityRecord = dbview.vw_MaklumatProduktiviti
                .Where(x => x.fld_KumpulanID == kumpulan && x.fld_Month == DateTime.Now.Month &&
                            x.fld_Year == DateTime.Now.Year && x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                            x.fld_LadangID == LadangID && x.fld_Kdaktf == "1")
                .OrderBy(o => o.fld_Nama);

            List<SelectListItem> jenisKategoriDdl = kategoriDataList
                .OrderBy(o => o.fldOptConfFlag1)
                .Select(s => new SelectListItem
                {
                    Value = s.fldOptConfValue,
                    Text = s.fldOptConfDesc
                }).ToList();

            List<SelectListItem> UnitDdl = new List<SelectListItem>();

            ViewBag.SelectionList = UnitDdl;
            ViewBag.fld_Kategori = jenisKategoriDdl;
            ViewBag.fld_NegaraID = NegaraID;
            ViewBag.fld_SyarikatID = SyarikatID;
            ViewBag.fld_WilayahID = WilayahID;
            ViewBag.fld_LadangID = LadangID;
            ViewBag.fld_kumpulanID = kumpulan;
            ViewBag.fld_KodKumpulan = groupData.fld_KodKumpulan;
            ViewBag.fld_AktivitiKumpulan = groupData.fld_KodKerja;
            ViewBag.fld_Keterangan = groupData.fld_Keterangan;
            ViewBag.fld_HadirKerja = hadirKerja;

            return PartialView(existingProductivityRecord);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _GroupProductivityAdd(string fld_Kategori, decimal fld_TargetHarian, int fld_HadirKerja, int fld_KumpulanID, string SelectionList)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);


            if (String.IsNullOrEmpty(fld_Kategori))
            {
                return Json(new
                {
                    success = true,
                    msg = GlobalResEstate.msgErrorData,
                    status = "danger",
                    checkingdata = "0",
                    method = "1",
                    //getid = getid,
                    data1 = "",
                    data2 = ""
                });
            }

            try
            {

                var workerData = dbr.tbl_Pkjmast
                    .Where(x => x.fld_Kdaktf == "1" && x.fld_KumpulanID == fld_KumpulanID && x.fld_NegaraID == NegaraID &&
                                x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_LadangID == LadangID)
                    .Select(s => s.fld_Nopkj)
                    .ToList();

                var productivityData = dbview.vw_MaklumatProduktiviti
                    .Where(x => x.fld_KumpulanID == fld_KumpulanID &&
                                x.fld_Month == DateTime.Now.Month && x.fld_Year == DateTime.Now.Year &&
                                x.fld_NegaraID == NegaraID && x.fld_WilayahID == WilayahID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID && x.fld_Deleted == false)
                    .Select(s => s.fld_Nopkj)
                    .ToList();

                List<String> workerWithoutProductivity = workerData.Except(productivityData).ToList();

                if (workerWithoutProductivity.Count == 0)
                {
                    return Json(new
                    {
                        success = true,
                        msg = "Maklumat produktiviti telah wujud bagi kesemua pekerja kumpulan ini",
                        status = "danger",
                        checkingdata = "0",
                        method = "1",
                        //getid = getid,
                        data1 = "",
                        data2 = ""
                    });
                }

                foreach (var nopkj in workerWithoutProductivity)
                {
                    Models.tbl_Produktiviti produktiviti = new Models.tbl_Produktiviti();

                    produktiviti.fld_Nopkj = nopkj;
                    produktiviti.fld_JenisPelan = fld_Kategori;
                    produktiviti.fld_Targetharian = fld_TargetHarian;
                    produktiviti.fld_Unit = SelectionList;
                    produktiviti.fld_HadirKerja = fld_HadirKerja;
                    produktiviti.fld_Month = DateTime.Now.Month;
                    produktiviti.fld_Year = DateTime.Now.Year;
                    produktiviti.fld_NegaraID = NegaraID;
                    produktiviti.fld_SyarikatID = SyarikatID;
                    produktiviti.fld_WilayahID = WilayahID;
                    produktiviti.fld_LadangID = LadangID;
                    produktiviti.fld_Deleted = false;
                    produktiviti.fld_CreatedBy = getuserid;
                    produktiviti.fld_CreatedDT = timezone.gettimezone();

                    dbr.tbl_Produktiviti.Add(produktiviti);
                }

                dbr.SaveChanges();

                return Json(new
                {
                    success = true,
                    msg = GlobalResEstate.msgAdd,
                    status = "success",
                    checkingdata = "0",
                    method = "1",
                    //getid = getid,
                    data1 = "",
                    data2 = ""
                });
            }
            //}
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new
                {
                    success = true,
                    msg = GlobalResEstate.msgError,
                    status = "danger",
                    checkingdata = "1"
                });
            }
        }

        public ActionResult _GroupMemberProductivityInfo(int kumpulan, int page = 1, string sort = "fld_Nama",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.vw_MaklumatProduktiviti>();
            int role = GetIdentity.RoleID(getuserid).Value;

            var productivityData = dbview.vw_MaklumatProduktiviti
                .Where(x => x.fld_KumpulanID == kumpulan &&
                            x.fld_Month == DateTime.Now.Month && x.fld_Year == DateTime.Now.Year &&
                            x.fld_NegaraID == NegaraID && x.fld_WilayahID == WilayahID &&
                            x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID && x.fld_Deleted == false);

            records.Content = productivityData
                .OrderBy(sort + " " + sortdir)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            records.TotalRecords = productivityData
                .Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.RoleID = role;
            ViewBag.pageSize = pageSize;
            //return View(records);
            return View(records);
        }

        public ActionResult ProductivityInfoGMN(string filter = "", int page = 1, string sort = "fld_KodKumpulan",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            ViewBag.WorkerInfo = "class = active";
            ViewBag.filter = filter;

            return View();
        }

        public ActionResult _WorkerProductivitySearchGMN(Guid? workerid, string filter = "", int page = 1,
            string sort = "fld_Nama",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Viewing dbr = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.vw_MaklumatProduktiviti>();
            int role = GetIdentity.RoleID(getuserid).Value;
            ViewBag.workerid = workerid;

            var workerData = dbr.tbl_Pkjmast
                .Where(x => x.fld_NegaraID == NegaraID &&
                            x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_LadangID == LadangID && x.fld_Kdaktf == "1")
                .ToList();

            var ProductivityList = dbr.vw_MaklumatProduktiviti
                .Where(x => x.fld_Month == DateTime.Now.Month && x.fld_Year == DateTime.Now.Year &&
                            x.fld_NegaraID == NegaraID && x.fld_WilayahID == WilayahID &&
                            x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID && x.fld_Deleted == false)
                .ToList();

            var kodNegeriLadang = db.tbl_Ladang
                .Where(x => x.fld_ID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WlyhID == WilayahID && x.fld_Deleted == false)
                .Select(s => s.fld_KodNegeri)
                .Single();

            int intkodNegeriLadang = int.Parse(kodNegeriLadang);

            var jenisMinggu = db.tbl_MingguNegeri
                .Where(x => x.fld_NegeriID == intkodNegeriLadang && x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
                .Select(s => s.fld_JenisMinggu)
                .Single();

            List<SelectListItem> jenisPelanList = new List<SelectListItem>();

            jenisPelanList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "jenisPelan" && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(
                        s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();
            jenisPelanList.Insert(0, new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "" });

            ViewBag.jenisPelanList = jenisPelanList;

            List<SelectListItem> UnitList = new List<SelectListItem>();

            UnitList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "unit" && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(
                        s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            UnitList.Insert(0, new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "" });

            ViewBag.UnitList = UnitList;

            var hadirKerja = db.tbl_HariBekerja
                .Where(x => x.fld_Month == DateTime.Now.Month && x.fld_Year == DateTime.Now.Year &&
                            x.fld_JenisMinggu == jenisMinggu && x.fld_NegeriID == intkodNegeriLadang &&
                            x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
                .Select(s => s.fld_BilanganHariBekerja)
                .SingleOrDefault();

            

            List<String> workerWithoutProductivity = workerData.Select(s => s.fld_Nopkj)
                .Except(ProductivityList.Select(s => s.fld_Nopkj))
                .ToList();

            foreach (var nopkj in workerWithoutProductivity)
            {
                var worker = dbr.tbl_Pkjmast
                    .Where(x => x.fld_Nopkj == nopkj && x.fld_NegaraID == NegaraID &&
                                x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_LadangID == LadangID && x.fld_Kdaktf == "1");

                ProductivityList.Add(
                    new vw_MaklumatProduktiviti
                    {
                        fld_Nopkj = nopkj,
                        fld_Nama = worker.Select(s => s.fld_Nama).Single(),
                        fld_Nokp = worker.Select(s => s.fld_Nokp).Single(),
                        fld_Targetharian = 0,
                        fld_Unit = "",
                        fld_HadirKerja = 0,
                        fld_JenisPelan = ""
                    });
            }

            foreach (var pkj in ProductivityList)
            {
                pkj.fld_HadirKerja = hadirKerja;
            }

            if (!String.IsNullOrEmpty(filter))
            {
                records.Content = ProductivityList
                    .Where(x => x.fld_Nopkj.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_Nama.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_Nokp.ToUpper().Contains(filter.ToUpper()))
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = ProductivityList
                    .Count(x => x.fld_Nopkj.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_Nama.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_Nokp.ToUpper().Contains(filter.ToUpper()));
            }

            else
            {
                records.Content = ProductivityList
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = ProductivityList
                    .Count();

            }

            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.RoleID = role;
            ViewBag.pageSize = pageSize;
            return PartialView(records);
        }

        public JsonResult addProductivityInfoGMN(string nopkj, int? hadirKerja)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            if (String.IsNullOrEmpty(hadirKerja.ToString()))
            {
                return Json(new
                {
                    success = true,
                    msg = GlobalResEstate.msgErrorData,
                    status = "danger",
                    checkingdata = "1"
                });
            }

            else
            {
                try
                {
                    var checkProductivityRecord = dbr.tbl_Produktiviti
                                        .Where(x => x.fld_Nopkj == nopkj && x.fld_Year == DateTime.Now.Year &&
                                                    x.fld_Month == DateTime.Now.Month && x.fld_NegaraID == NegaraID &&
                                                    x.fld_SyarikatID == SyarikatID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();

                    if (checkProductivityRecord.Count == 0)
                    {
                        Models.tbl_Produktiviti Produktiviti = new Models.tbl_Produktiviti();

                        Produktiviti.fld_Nopkj = nopkj.Trim();
                        Produktiviti.fld_JenisPelan = "N/A";
                        Produktiviti.fld_Targetharian = 0;
                        Produktiviti.fld_Unit = "N/A";
                        Produktiviti.fld_HadirKerja = hadirKerja;
                        Produktiviti.fld_Year = timezone.gettimezone().Year;
                        Produktiviti.fld_Month = timezone.gettimezone().Month;
                        Produktiviti.fld_NegaraID = NegaraID;
                        Produktiviti.fld_SyarikatID = SyarikatID;
                        Produktiviti.fld_WilayahID = WilayahID;
                        Produktiviti.fld_LadangID = LadangID;
                        Produktiviti.fld_Deleted = false;
                        Produktiviti.fld_CreatedBy = getuserid;
                        Produktiviti.fld_CreatedDT = timezone.gettimezone();

                        dbr.tbl_Produktiviti.Add(Produktiviti);
                    }

                    else
                    {
                        var materializeProductivityRec = checkProductivityRecord.Single();

                        materializeProductivityRec.fld_HadirKerja = hadirKerja;

                        dbr.Entry(materializeProductivityRec).State = EntityState.Modified;
                        dbr.SaveChanges();
                    }

                    dbr.SaveChanges();

                    return Json(new
                    {
                        success = true,
                        msg = GlobalResEstate.msgAdd,
                        status = "success",
                        checkingdata = "1"
                    });
                }

                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new
                    {
                        success = false,
                        msg = GlobalResEstate.msgErrorData,
                        status = "danger",
                        checkingdata = "0"
                    });
                }
            }
        }

        public ActionResult GroupProductivityInfoGMN(string filter = "", int page = 1, string sort = "fld_KodKumpulan",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            ViewBag.WorkerInfo = "class = active";
            ViewBag.filter = filter;

            return View();
        }

        public ActionResult _GroupProductivitySearchGMN(Guid? workerid, string filter = "", int page = 1,
            string sort = "fld_KodKumpulan",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.vw_KumpulanKerja>();
            int role = GetIdentity.RoleID(getuserid).Value;
            //ViewBag.filter = filter;

            var GroupList = dbview.vw_KumpulanKerja
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false);

            if (!String.IsNullOrEmpty(filter))
            {
                records.Content = GroupList
                    .Where(x => x.fld_KodKumpulan.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_KodKerja.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_Keterangan.ToUpper().Contains(filter.ToUpper()))
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = GroupList
                    .Count(x => x.fld_KodKumpulan.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_KodKerja.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_Keterangan.ToUpper().Contains(filter.ToUpper()));
            }

            else
            {
                records.Content = GroupList
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = GroupList
                    .Count();

            }

            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.RoleID = role;
            ViewBag.pageSize = pageSize;
            return View(records);
        }

        public ActionResult _GroupProductivityAddGMN(int kumpulan)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            var groupData = dbview.tbl_KumpulanKerja
                .Single(x => x.fld_KumpulanID == kumpulan);

            var kategoriDataList = db.tblOptionConfigsWebs
                .Where(b => b.fldOptConfFlag1 == "jenisPelan" && b.fldDeleted == false && b.fld_NegaraID == NegaraID &&
                            b.fld_SyarikatID == SyarikatID);

            var kodNegeriLadang = db.tbl_Ladang
                .Where(x => x.fld_ID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WlyhID == WilayahID && x.fld_Deleted == false)
                .Select(s => s.fld_KodNegeri)
                .Single();

            int intkodNegeriLadang = int.Parse(kodNegeriLadang);

            var jenisMinggu = db.tbl_MingguNegeri
                .Where(x => x.fld_NegeriID == intkodNegeriLadang && x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
                .Select(s => s.fld_JenisMinggu)
                .Single();

            var hadirKerja = db.tbl_HariBekerja
                .Where(x => x.fld_Month == DateTime.Now.Month && x.fld_Year == DateTime.Now.Year &&
                            x.fld_JenisMinggu == jenisMinggu && x.fld_NegeriID == intkodNegeriLadang &&
                            x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
                .Select(s => s.fld_BilanganHariBekerja)
                .SingleOrDefault();

            var existingProductivityRecord = dbview.vw_MaklumatProduktiviti
                .Where(x => x.fld_KumpulanID == kumpulan && x.fld_Month == DateTime.Now.Month &&
                            x.fld_Year == DateTime.Now.Year && x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                            x.fld_LadangID == LadangID && x.fld_Kdaktf == "1")
                .OrderBy(o => o.fld_Nama);

            List<SelectListItem> jenisKategoriDdl = kategoriDataList
                .OrderBy(o => o.fldOptConfFlag1)
                .Select(s => new SelectListItem
                {
                    Value = s.fldOptConfValue,
                    Text = s.fldOptConfDesc
                }).ToList();

            List<SelectListItem> UnitDdl = new List<SelectListItem>();

            ViewBag.SelectionList = UnitDdl;
            ViewBag.fld_Kategori = jenisKategoriDdl;
            ViewBag.fld_NegaraID = NegaraID;
            ViewBag.fld_SyarikatID = SyarikatID;
            ViewBag.fld_WilayahID = WilayahID;
            ViewBag.fld_LadangID = LadangID;
            ViewBag.fld_kumpulanID = kumpulan;
            ViewBag.fld_KodKumpulan = groupData.fld_KodKumpulan;
            ViewBag.fld_AktivitiKumpulan = groupData.fld_KodKerja;
            ViewBag.fld_Keterangan = groupData.fld_Keterangan;
            ViewBag.fld_HadirKerja = hadirKerja;

            return PartialView(existingProductivityRecord);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _GroupProductivityAddGMN(int fld_HadirKerja, int fld_KumpulanID)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                var workerData = dbr.tbl_Pkjmast
                    .Where(x => x.fld_Kdaktf == "1" && x.fld_KumpulanID == fld_KumpulanID && x.fld_NegaraID == NegaraID &&
                                x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_LadangID == LadangID)
                    .Select(s => s.fld_Nopkj)
                    .ToList();

                var productivityData = dbview.vw_MaklumatProduktiviti
                    .Where(x => x.fld_KumpulanID == fld_KumpulanID &&
                                x.fld_Month == DateTime.Now.Month && x.fld_Year == DateTime.Now.Year &&
                                x.fld_NegaraID == NegaraID && x.fld_WilayahID == WilayahID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID && x.fld_Deleted == false)
                    .Select(s => s.fld_Nopkj)
                    .ToList();

                List<String> workerWithoutProductivity = workerData.Except(productivityData).ToList();

                if (workerWithoutProductivity.Count == 0)
                {
                    return Json(new
                    {
                        success = true,
                        msg = "Maklumat produktiviti telah wujud bagi kesemua pekerja kumpulan ini",
                        status = "danger",
                        checkingdata = "0",
                        method = "1",
                        //getid = getid,
                        data1 = "",
                        data2 = ""
                    });
                }

                foreach (var nopkj in workerWithoutProductivity)
                {
                    Models.tbl_Produktiviti produktiviti = new Models.tbl_Produktiviti();

                    produktiviti.fld_Nopkj = nopkj;
                    produktiviti.fld_JenisPelan = "N/A";
                    produktiviti.fld_Targetharian = 0;
                    produktiviti.fld_Unit = "N/A";
                    produktiviti.fld_HadirKerja = fld_HadirKerja;
                    produktiviti.fld_Month = timezone.gettimezone().Month;
                    produktiviti.fld_Year = timezone.gettimezone().Year;
                    produktiviti.fld_NegaraID = NegaraID;
                    produktiviti.fld_SyarikatID = SyarikatID;
                    produktiviti.fld_WilayahID = WilayahID;
                    produktiviti.fld_LadangID = LadangID;
                    produktiviti.fld_Deleted = false;
                    produktiviti.fld_CreatedBy = getuserid;
                    produktiviti.fld_CreatedDT = timezone.gettimezone();

                    dbr.tbl_Produktiviti.Add(produktiviti);
                }

                dbr.SaveChanges();

                return Json(new
                {
                    success = true,
                    msg = GlobalResEstate.msgAdd,
                    status = "success",
                    checkingdata = "0",
                    method = "1",
                    //getid = getid,
                    data1 = "",
                    data2 = ""
                });
            }
            //}
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new
                {
                    success = true,
                    msg = GlobalResEstate.msgError,
                    status = "danger",
                    checkingdata = "1"
                });
            }
        }

        public ActionResult _GroupMemberProductivityInfoGMN(int kumpulan, int page = 1, string sort = "fld_Nama",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.vw_MaklumatProduktiviti>();
            int role = GetIdentity.RoleID(getuserid).Value;

            var productivityData = dbview.vw_MaklumatProduktiviti
                .Where(x => x.fld_KumpulanID == kumpulan &&
                            x.fld_Month == DateTime.Now.Month && x.fld_Year == DateTime.Now.Year &&
                            x.fld_NegaraID == NegaraID && x.fld_WilayahID == WilayahID &&
                            x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID && x.fld_Deleted == false);

            records.Content = productivityData
                .OrderBy(sort + " " + sortdir)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            records.TotalRecords = productivityData
                .Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.RoleID = role;
            ViewBag.pageSize = pageSize;
            return View(records);
        }

        public ActionResult YieldInfo(string filter = "", int page = 1, string sort = "fld_KodKumpulan", string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            int drpyear = 0;
            int drprangeyear = 0;
            int month = timezone.gettimezone().Month;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            MVC_SYSTEM_Viewing dbr = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);


            ViewBag.WorkerInfo = "class = active";

            ViewBag.JenisPeringkatList = new SelectList(
                db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnspkt" && x.fldDeleted == false &&
                                                   x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfValue),
                "fldOptConfValue", "fldOptConfDesc");

            ViewBag.MonthList = new SelectList(
                db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false &&
                                                   x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID),
                "fldOptConfValue", "fldOptConfDesc");

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == timezone.gettimezone().Year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }
            ViewBag.MonthList = new SelectList(
                db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false &&
                                                   x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID),
                "fldOptConfValue", "fldOptConfDesc", month);
            ViewBag.YearList = yearlist;

            return View();
        }

        public ActionResult _YieldCreate(int? JenisPeringkatList, int? MonthList, int? YearList)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            //int drpyear = 0;
            //int drprangeyear = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            if (JenisPeringkatList == 1)
            {
                return RedirectToAction("_populatePktList", "WorkerInfo", new { MonthList, YearList });

            }

            else if (JenisPeringkatList == 2)
            {
                return RedirectToAction("_populateSubPktList", "WorkerInfo", new { MonthList, YearList });
            }

            else
            {
                return RedirectToAction("_populateBlockList", "WorkerInfo", new { MonthList, YearList });

            }
        }

        public ActionResult _populateBlockList(int MonthList, int YearList, int page = 1, string sort = "fld_Blok",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<ViewingModels.vw_HasilSawitBlok> vw_HasilSawitBlok = new List<ViewingModels.vw_HasilSawitBlok>();

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.vw_HasilSawitBlok>();
            int role = GetIdentity.RoleID(getuserid).Value;

            var getAllBlok = dbr.tbl_Blok
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false);

            var getHasilSawitBlok = dbview.vw_HasilSawitBlok
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Bulan == MonthList &&
                            x.fld_Tahun == YearList && x.fld_YieldType == "EST");

            if (getHasilSawitBlok.ToList().Count == 0)
            {
                foreach (var i in getAllBlok)
                {
                    vw_HasilSawitBlok.Add(
                        new ViewingModels.vw_HasilSawitBlok
                        {
                            fld_Blok = i.fld_Blok,
                            fld_KodPkt = i.fld_KodPkt,
                            fld_KodPktutama = i.fld_KodPktutama,
                            fld_NamaBlok = i.fld_NamaBlok,
                            fld_HasilTan = 0,
                            fld_LsBlok = i.fld_LsBlok,
                            fld_NegaraID = i.fld_NegaraID,
                            fld_SyarikatID = i.fld_SyarikatID,
                            fld_WilayahID = i.fld_WilayahID,
                            fld_LadangID = i.fld_LadangID
                        });
                }

                var vw_HasilSawitBlokQueryable = vw_HasilSawitBlok.AsQueryable();

                getHasilSawitBlok = vw_HasilSawitBlokQueryable;

            }

            if (getHasilSawitBlok.ToList().Count != getAllBlok.ToList().Count && getHasilSawitBlok.ToList().Count != 0)
            {
                foreach (var i in getAllBlok)
                {
                    decimal? hasil = 0;

                    foreach (var a in getHasilSawitBlok)
                    {
                        if (i.fld_Blok == a.fld_Blok)
                        {
                            hasil = a.fld_HasilTan;
                        }
                    }

                    vw_HasilSawitBlok.Add(
                        new ViewingModels.vw_HasilSawitBlok
                        {
                            fld_Blok = i.fld_Blok,
                            fld_KodPkt = i.fld_KodPkt,
                            fld_KodPktutama = i.fld_KodPktutama,
                            fld_NamaBlok = i.fld_NamaBlok,
                            fld_LsBlok = i.fld_LsBlok,
                            fld_HasilTan = hasil,
                            fld_NegaraID = i.fld_NegaraID,
                            fld_SyarikatID = i.fld_SyarikatID,
                            fld_WilayahID = i.fld_WilayahID,
                            fld_LadangID = i.fld_LadangID
                        });
                }

                var vw_HasilSawitBlokQueryable = vw_HasilSawitBlok.AsQueryable();

                getHasilSawitBlok = vw_HasilSawitBlokQueryable;
            }

            records.Content = getHasilSawitBlok
                .OrderBy(sort + " " + sortdir)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            records.TotalRecords = getHasilSawitBlok
                .Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.RoleID = role;
            ViewBag.pageSize = pageSize;
            return View(records);
        }

        public JsonResult addBlokYieldInfo(decimal? hasilTan, string kodBlok, int month, int year)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            var blokData = dbr.tbl_Blok
                .Where(x => x.fld_Blok == kodBlok && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                            x.fld_Deleted == false);

            if (String.IsNullOrEmpty(kodBlok) || String.IsNullOrEmpty(hasilTan.ToString()))
            {
                return Json(new
                {
                    success = true,
                    msg = GlobalResEstate.msgErrorData,
                    status = "danger",
                    checkingdata = "1"
                });
            }

            else
            {
                var checkYieldRecord = dbr.tbl_HasilSawitBlok
                    .Where(x => x.fld_KodBlok == kodBlok && x.fld_Tahun == year &&
                                x.fld_Bulan == month && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_YieldType == "EST")
                    .ToList();

                if (checkYieldRecord.Count == 0)
                {
                    Models.tbl_HasilSawitBlok HasilSawitBlok = new Models.tbl_HasilSawitBlok();

                    HasilSawitBlok.fld_KodBlok = kodBlok;
                    HasilSawitBlok.fld_HasilTan = hasilTan;
                    HasilSawitBlok.fld_LuasHektar = blokData.Select(s => s.fld_LsBlok).Single();
                    HasilSawitBlok.fld_Tahun = year;
                    HasilSawitBlok.fld_Bulan = month;
                    HasilSawitBlok.fld_NegaraID = NegaraID;
                    HasilSawitBlok.fld_SyarikatID = SyarikatID;
                    HasilSawitBlok.fld_WilayahID = WilayahID;
                    HasilSawitBlok.fld_LadangID = LadangID;
                    HasilSawitBlok.fld_Deleted = false;
                    HasilSawitBlok.fld_YieldType = "EST";
                    HasilSawitBlok.fld_CreatedBy = getuserid;
                    HasilSawitBlok.fld_CreatedDT = timezone.gettimezone();

                    dbr.tbl_HasilSawitBlok.Add(HasilSawitBlok);
                }

                else
                {
                    var materializeBlockYieldRec = checkYieldRecord.Single();

                    materializeBlockYieldRec.fld_HasilTan = hasilTan;

                    dbr.Entry(materializeBlockYieldRec).State = EntityState.Modified;
                }

                dbr.SaveChanges();

                var getSubPeringkatKod = blokData.Select(s => s.fld_KodPkt).Single();

                var getSubPeringkatData =
                    dbr.tbl_SubPkt.Where(x => x.fld_Pkt == getSubPeringkatKod && x.fld_NegaraID == NegaraID &&
                                              x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                              x.fld_LadangID == LadangID &&
                                              x.fld_Deleted == false);

                var countSubPeringkatMonthlyYield = dbr.tbl_HasilSawitBlok
                    .Where(x => x.fld_KodBlok.ToUpper()
                                    .Contains(getSubPeringkatKod.ToUpper()) &&
                                x.fld_Tahun == year &&
                                x.fld_Bulan == month && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                x.fld_LadangID == LadangID && x.fld_Deleted == false)
                    .Select(s => s.fld_HasilTan);

                decimal? totalSubPktYield = 0;

                foreach (var hasil in countSubPeringkatMonthlyYield)
                {
                    totalSubPktYield += hasil;
                }


                var checkSubPeringkatYieldRecord = dbr.tbl_HasilSawitSubPkt
                    .Where(x => x.fld_KodSubPeringkat == getSubPeringkatKod && x.fld_Tahun == year &&
                                x.fld_Bulan == month && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_YieldType == "EST")
                    .ToList();

                if (checkSubPeringkatYieldRecord.Count == 0)
                {

                    Models.tbl_HasilSawitSubPkt HasilSawitSubPkt = new Models.tbl_HasilSawitSubPkt();

                    HasilSawitSubPkt.fld_KodSubPeringkat = blokData.Select(s => s.fld_KodPkt).Single();
                    HasilSawitSubPkt.fld_HasilTan = totalSubPktYield;
                    HasilSawitSubPkt.fld_LuasHektar = getSubPeringkatData.Select(s => s.fld_LsPkt).Single();
                    HasilSawitSubPkt.fld_Tahun = year;
                    HasilSawitSubPkt.fld_Bulan = month;
                    HasilSawitSubPkt.fld_NegaraID = NegaraID;
                    HasilSawitSubPkt.fld_SyarikatID = SyarikatID;
                    HasilSawitSubPkt.fld_WilayahID = WilayahID;
                    HasilSawitSubPkt.fld_LadangID = LadangID;
                    HasilSawitSubPkt.fld_Deleted = false;
                    HasilSawitSubPkt.fld_YieldType = "EST";
                    HasilSawitSubPkt.fld_CreatedBy = getuserid;
                    HasilSawitSubPkt.fld_CreatedDT = timezone.gettimezone();

                    dbr.tbl_HasilSawitSubPkt.Add(HasilSawitSubPkt);
                }

                else
                {
                    var materializeSubPktYieldRec = checkSubPeringkatYieldRecord.Single();

                    materializeSubPktYieldRec.fld_HasilTan = totalSubPktYield;

                    dbr.Entry(materializeSubPktYieldRec).State = EntityState.Modified;
                }

                dbr.SaveChanges();

                var getPeringkatKod = getSubPeringkatData.Select(s => s.fld_KodPktUtama).Single();

                var getPeringkatData =
                    dbr.tbl_PktUtama.Where(x => x.fld_PktUtama == getPeringkatKod);

                var countPeringkatMonthlyYield = dbr.tbl_HasilSawitSubPkt
                    .Where(x => x.fld_KodSubPeringkat.ToUpper()
                                    .Contains(getPeringkatKod.ToUpper()) &&
                                x.fld_Tahun == year &&
                                x.fld_Bulan == month && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                x.fld_LadangID == LadangID && x.fld_Deleted == false)
                    .Select(s => s.fld_HasilTan);

                decimal? totalPktYield = 0;

                foreach (var hasil in countPeringkatMonthlyYield)
                {
                    totalPktYield += hasil;
                }

                var checkPeringkatYieldRecord = dbr.tbl_HasilSawitPkt
                    .Where(x => x.fld_KodPeringkat == getPeringkatKod && x.fld_Tahun == year &&
                                x.fld_Bulan == month && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_YieldType == "EST")
                    .ToList();

                if (checkPeringkatYieldRecord.Count == 0)
                {

                    Models.tbl_HasilSawitPkt HasilSawitPkt = new Models.tbl_HasilSawitPkt();

                    HasilSawitPkt.fld_KodPeringkat = getSubPeringkatData.Select(s => s.fld_KodPktUtama).Single();
                    HasilSawitPkt.fld_HasilTan = totalPktYield;

                    HasilSawitPkt.fld_LuasHektar = getPeringkatData.Select(s => s.fld_LsPktUtama).Single();
                    HasilSawitPkt.fld_Tahun = year;
                    HasilSawitPkt.fld_Bulan = month;
                    HasilSawitPkt.fld_NegaraID = NegaraID;
                    HasilSawitPkt.fld_SyarikatID = SyarikatID;
                    HasilSawitPkt.fld_WilayahID = WilayahID;
                    HasilSawitPkt.fld_LadangID = LadangID;
                    HasilSawitPkt.fld_Deleted = false;
                    HasilSawitPkt.fld_YieldType = "EST";
                    HasilSawitPkt.fld_CreatedBy = getuserid;
                    HasilSawitPkt.fld_CreatedDT = timezone.gettimezone();

                    dbr.tbl_HasilSawitPkt.Add(HasilSawitPkt);
                }

                else
                {
                    var materializePktYieldRec = checkPeringkatYieldRecord.Single();

                    materializePktYieldRec.fld_HasilTan = totalPktYield;

                    dbr.Entry(materializePktYieldRec).State = EntityState.Modified;
                }

                dbr.SaveChanges();

                return Json(new
                {
                    success = true,
                    msg = GlobalResEstate.msgAdd,
                    status = "success",
                    checkingdata = "1"
                });
            }
        }

        public ActionResult _populateSubPktList(int MonthList, int YearList, int page = 1, string sort = "fld_Pkt",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<ViewingModels.vw_HasilSawitSubPkt> vw_HasilSawitSubPkt = new List<ViewingModels.vw_HasilSawitSubPkt>();

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.vw_HasilSawitSubPkt>();
            int role = GetIdentity.RoleID(getuserid).Value;

            var getAllSubPkt = dbr.tbl_SubPkt
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false);

            var getHasilSawitSubPkt = dbview.vw_HasilSawitSubPkt
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Bulan == MonthList &&
                            x.fld_Tahun == YearList && x.fld_YieldType == "EST");

            if (getHasilSawitSubPkt.ToList().Count == 0)
            {
                foreach (var i in getAllSubPkt)
                {
                    vw_HasilSawitSubPkt.Add(
                        new ViewingModels.vw_HasilSawitSubPkt
                        {
                            fld_Pkt = i.fld_Pkt,
                            //fld_KodPkt = i.fld_KodPkt,
                            fld_KodPktUtama = i.fld_KodPktUtama,
                            fld_NamaPkt = i.fld_NamaPkt,
                            fld_LsPkt = i.fld_LsPkt,
                            fld_HasilTan = 0,
                            fld_NegaraID = i.fld_NegaraID,
                            fld_SyarikatID = i.fld_SyarikatID,
                            fld_WilayahID = i.fld_WilayahID,
                            fld_LadangID = i.fld_LadangID
                        });
                }

                var vw_HasilSawitSubPktQueryable = vw_HasilSawitSubPkt.AsQueryable();

                getHasilSawitSubPkt = vw_HasilSawitSubPktQueryable;

            }

            if (getHasilSawitSubPkt.ToList().Count != getAllSubPkt.ToList().Count && getHasilSawitSubPkt.ToList().Count != 0)
            {
                foreach (var i in getAllSubPkt)
                {
                    decimal? hasil = 0;

                    foreach (var a in getHasilSawitSubPkt)
                    {
                        if (i.fld_Pkt == a.fld_Pkt)
                        {
                            hasil = a.fld_HasilTan;
                        }
                    }

                    vw_HasilSawitSubPkt.Add(
                        new ViewingModels.vw_HasilSawitSubPkt
                        {
                            fld_Pkt = i.fld_Pkt,
                            //fld_KodPkt = i.fld_KodPkt,
                            fld_KodPktUtama = i.fld_KodPktUtama,
                            fld_NamaPkt = i.fld_NamaPkt,
                            fld_LsPkt = i.fld_LsPkt,
                            fld_HasilTan = hasil,
                            fld_NegaraID = i.fld_NegaraID,
                            fld_SyarikatID = i.fld_SyarikatID,
                            fld_WilayahID = i.fld_WilayahID,
                            fld_LadangID = i.fld_LadangID
                        });
                }

                var vw_HasilSawitSubPktQueryable = vw_HasilSawitSubPkt.AsQueryable();

                getHasilSawitSubPkt = vw_HasilSawitSubPktQueryable;
            }

            records.Content = getHasilSawitSubPkt
                .OrderBy(sort + " " + sortdir)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            records.TotalRecords = getHasilSawitSubPkt
                .Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.RoleID = role;
            ViewBag.pageSize = pageSize;
            return View(records);
        }

        public JsonResult addSubPktYieldInfo(decimal? hasilTan, string kodSubPkt, int month, int year)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            var subPktData = dbr.tbl_SubPkt
                .Where(x => x.fld_Pkt == kodSubPkt);

            if (String.IsNullOrEmpty(kodSubPkt) || String.IsNullOrEmpty(hasilTan.ToString()))
            {
                return Json(new
                {
                    success = true,
                    msg = GlobalResEstate.msgErrorData,
                    status = "danger",
                    checkingdata = "1"
                });
            }

            else
            {
                var checkSubPktYieldRecord = dbr.tbl_HasilSawitSubPkt
                    .Where(x => x.fld_KodSubPeringkat == kodSubPkt && x.fld_Tahun == year &&
                                x.fld_Bulan == month && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_YieldType == "EST")
                    .ToList();

                if (checkSubPktYieldRecord.Count == 0)
                {
                    Models.tbl_HasilSawitSubPkt HasilSawitSubPkt = new Models.tbl_HasilSawitSubPkt();

                    HasilSawitSubPkt.fld_KodSubPeringkat = kodSubPkt;
                    HasilSawitSubPkt.fld_HasilTan = hasilTan;
                    HasilSawitSubPkt.fld_LuasHektar = subPktData.Select(s => s.fld_LsPkt).Single();
                    HasilSawitSubPkt.fld_Tahun = year;
                    HasilSawitSubPkt.fld_Bulan = month;
                    HasilSawitSubPkt.fld_NegaraID = NegaraID;
                    HasilSawitSubPkt.fld_SyarikatID = SyarikatID;
                    HasilSawitSubPkt.fld_WilayahID = WilayahID;
                    HasilSawitSubPkt.fld_LadangID = LadangID;
                    HasilSawitSubPkt.fld_Deleted = false;
                    HasilSawitSubPkt.fld_YieldType = "EST";
                    HasilSawitSubPkt.fld_CreatedBy = getuserid;
                    HasilSawitSubPkt.fld_CreatedDT = timezone.gettimezone();

                    dbr.tbl_HasilSawitSubPkt.Add(HasilSawitSubPkt);
                }

                else
                {
                    var materializeSubPktYieldRec = checkSubPktYieldRecord.Single();

                    materializeSubPktYieldRec.fld_HasilTan = hasilTan;

                    dbr.Entry(materializeSubPktYieldRec).State = EntityState.Modified;
                }

                dbr.SaveChanges();

                var getPeringkatKod = subPktData.Select(s => s.fld_KodPktUtama).Single();

                var getPeringkatData =
                    dbr.tbl_PktUtama.Where(x => x.fld_PktUtama == getPeringkatKod);

                var countPeringkatMonthlyYield = dbr.tbl_HasilSawitSubPkt
                    .Where(x => x.fld_KodSubPeringkat.ToUpper()
                                    .Contains(getPeringkatKod.ToUpper()) &&
                                x.fld_Tahun == year &&
                                x.fld_Bulan == month && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                x.fld_LadangID == LadangID && x.fld_Deleted == false)
                    .Select(s => s.fld_HasilTan);

                decimal? totalPktYield = 0;

                foreach (var hasil in countPeringkatMonthlyYield)
                {
                    totalPktYield += hasil;
                }


                var checkPeringkatYieldRecord = dbr.tbl_HasilSawitPkt
                    .Where(x => x.fld_KodPeringkat == getPeringkatKod && x.fld_Tahun == year &&
                                x.fld_Bulan == month && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_YieldType == "EST")
                    .ToList();

                if (checkPeringkatYieldRecord.Count == 0)
                {

                    Models.tbl_HasilSawitPkt HasilSawitPkt = new Models.tbl_HasilSawitPkt();

                    HasilSawitPkt.fld_KodPeringkat = subPktData.Select(s => s.fld_KodPktUtama).Single();
                    HasilSawitPkt.fld_HasilTan = totalPktYield;
                    HasilSawitPkt.fld_LuasHektar = getPeringkatData.Select(s => s.fld_LsPktUtama).Single();
                    HasilSawitPkt.fld_Tahun = year;
                    HasilSawitPkt.fld_Bulan = month;
                    HasilSawitPkt.fld_NegaraID = NegaraID;
                    HasilSawitPkt.fld_SyarikatID = SyarikatID;
                    HasilSawitPkt.fld_WilayahID = WilayahID;
                    HasilSawitPkt.fld_LadangID = LadangID;
                    HasilSawitPkt.fld_Deleted = false;
                    HasilSawitPkt.fld_YieldType = "EST";
                    HasilSawitPkt.fld_CreatedBy = getuserid;
                    HasilSawitPkt.fld_CreatedDT = timezone.gettimezone();

                    dbr.tbl_HasilSawitPkt.Add(HasilSawitPkt);
                }

                else
                {
                    var materializeSubPktYieldRec = checkPeringkatYieldRecord.Single();

                    materializeSubPktYieldRec.fld_HasilTan = totalPktYield;

                    dbr.Entry(materializeSubPktYieldRec).State = EntityState.Modified;
                    dbr.SaveChanges();
                }

                dbr.SaveChanges();

                return Json(new
                {
                    success = true,
                    msg = GlobalResEstate.msgAdd,
                    status = "success",
                    checkingdata = "1"
                });
            }
        }

        public ActionResult _populatePktList(int MonthList, int YearList, int page = 1, string sort = "fld_PktUtama",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<ViewingModels.vw_HasilSawitPkt> vw_HasilSawitPkt = new List<ViewingModels.vw_HasilSawitPkt>();
            //List<CustMod_YieldInfoPkt> vw_HasilSawitPkt = new List<ViewingModels.vw_HasilSawitPkt>();

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.vw_HasilSawitPkt>();
            int role = GetIdentity.RoleID(getuserid).Value;

            var getAllPkt = dbr.tbl_PktUtama
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false);

            var getHasilSawitPkt = dbview.vw_HasilSawitPkt
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Bulan == MonthList &&
                            x.fld_Tahun == YearList && x.fld_YieldType == "EST" && x.DeleteHasilSawit == false);

            if (getHasilSawitPkt.ToList().Count == 0)
            {
                foreach (var i in getAllPkt)
                {
                    vw_HasilSawitPkt.Add(
                        new ViewingModels.vw_HasilSawitPkt
                        {
                            fld_PktUtama = i.fld_PktUtama,
                            //fld_KodPkt = i.fld_KodPkt,
                            //fld_KodPktUtama = i.fld_KodPktUtama,
                            fld_NamaPktUtama = i.fld_NamaPktUtama,
                            fld_LsPktUtama = i.fld_LsPktUtama,
                            fld_HasilTan = 0,
                            //fld_TanPHektar = 0,
                            fld_NegaraID = i.fld_NegaraID,
                            fld_SyarikatID = i.fld_SyarikatID,
                            fld_WilayahID = i.fld_WilayahID,
                            fld_LadangID = i.fld_LadangID,
                        });
                }

                var vw_HasilSawitPktQueryable = vw_HasilSawitPkt.AsQueryable();

                getHasilSawitPkt = vw_HasilSawitPktQueryable;

            }

            if (getHasilSawitPkt.ToList().Count != getAllPkt.ToList().Count && getHasilSawitPkt.ToList().Count != 0)
            {
                foreach (var i in getAllPkt)
                {
                    decimal? hasil = 0;
                    //decimal? TanPHektar = 0;

                    foreach (var a in getHasilSawitPkt)
                    {
                        if (i.fld_PktUtama == a.fld_PktUtama)
                        {
                            hasil = a.fld_HasilTan;
                            //TanPHektar = hasil / i.fld_LsPktUtama;
                        }
                    }

                    vw_HasilSawitPkt.Add(
                        new ViewingModels.vw_HasilSawitPkt
                        {
                            fld_PktUtama = i.fld_PktUtama,
                            //fld_KodPkt = i.fld_KodPkt,
                            //fld_KodPktUtama = i.fld_KodPktUtama,
                            fld_NamaPktUtama = i.fld_NamaPktUtama,
                            fld_LsPktUtama = i.fld_LsPktUtama,
                            fld_HasilTan = hasil,
                            //fld_TanPHektar = TanPHektar,
                            fld_NegaraID = i.fld_NegaraID,
                            fld_SyarikatID = i.fld_SyarikatID,
                            fld_WilayahID = i.fld_WilayahID,
                            fld_LadangID = i.fld_LadangID
                        });
                }

                var vw_HasilSawitPktQueryable = vw_HasilSawitPkt.AsQueryable();

                getHasilSawitPkt = vw_HasilSawitPktQueryable;
            }

            records.Content = getHasilSawitPkt
                .OrderBy(sort + " " + sortdir)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            records.TotalRecords = getHasilSawitPkt
                .Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.RoleID = role;
            ViewBag.pageSize = pageSize;
            return View(records);
        }

        public JsonResult addPktYieldInfo(decimal? hasilTan, string kodPkt, int month, int year)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            var pktData = dbr.tbl_PktUtama
                .Where(x => x.fld_PktUtama == kodPkt);

            if (String.IsNullOrEmpty(kodPkt) || String.IsNullOrEmpty(hasilTan.ToString()))
            {
                return Json(new
                {
                    success = true,
                    msg = GlobalResEstate.msgErrorData,
                    status = "danger",
                    checkingdata = "1"
                });
            }

            else
            {
                var checkPktYieldRecord = dbr.tbl_HasilSawitPkt
                    .Where(x => x.fld_KodPeringkat == kodPkt && x.fld_Tahun == year &&
                                x.fld_Bulan == month && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_YieldType == "EST")
                    .ToList();

                if (checkPktYieldRecord.Count == 0)
                {
                    Models.tbl_HasilSawitPkt HasilSawitPkt = new Models.tbl_HasilSawitPkt();

                    HasilSawitPkt.fld_KodPeringkat = kodPkt;
                    HasilSawitPkt.fld_HasilTan = hasilTan;
                    HasilSawitPkt.fld_LuasHektar = pktData.Select(s => s.fld_LsPktUtama).Single();
                    HasilSawitPkt.fld_Tahun = year;
                    HasilSawitPkt.fld_Bulan = month;
                    HasilSawitPkt.fld_NegaraID = NegaraID;
                    HasilSawitPkt.fld_SyarikatID = SyarikatID;
                    HasilSawitPkt.fld_WilayahID = WilayahID;
                    HasilSawitPkt.fld_LadangID = LadangID;
                    HasilSawitPkt.fld_Deleted = false;
                    HasilSawitPkt.fld_YieldType = "EST";
                    HasilSawitPkt.fld_CreatedBy = getuserid;
                    HasilSawitPkt.fld_CreatedDT = timezone.gettimezone();

                    dbr.tbl_HasilSawitPkt.Add(HasilSawitPkt);
                }

                else
                {
                    var materializePktYieldRec = checkPktYieldRecord.Single();

                    materializePktYieldRec.fld_HasilTan = hasilTan;

                    dbr.Entry(materializePktYieldRec).State = EntityState.Modified;
                }

                dbr.SaveChanges();

                return Json(new
                {
                    success = true,
                    msg =GlobalResEstate.msgAdd,
                    status = "success",
                    checkingdata = "1"
                });
            }
        }

        public ActionResult YieldExactInfo(string filter = "", int page = 1, string sort = "fld_KodKumpulan", string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            int drpyear = 0;
            int drprangeyear = 0;
            int month = timezone.gettimezone().Month;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            ViewBag.WorkerInfo = "class = active";

            ViewBag.JenisPeringkatList = new SelectList(
                db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnspkt" && x.fldDeleted == false &&
                                                   x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfValue),
                "fldOptConfValue", "fldOptConfDesc");

            ViewBag.MonthList = new SelectList(
                db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false &&
                                                   x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID),
                "fldOptConfValue", "fldOptConfDesc");

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == timezone.gettimezone().Year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }
            ViewBag.YearList = yearlist;

            ViewBag.MonthList = new SelectList(
                db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false &&
                                                   x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID),
                "fldOptConfValue", "fldOptConfDesc", month - 1);

            return View();
        }

        public ActionResult _YieldExactCreate(int? JenisPeringkatList, int? MonthList, int? YearList)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            //int drpyear = 0;
            //int drprangeyear = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            if (JenisPeringkatList == 1)
            {
                return RedirectToAction("_populateExactPktList", "WorkerInfo", new { MonthList, YearList });

            }

            else if (JenisPeringkatList == 2)
            {
                return RedirectToAction("_populateExactSubPktList", "WorkerInfo", new { MonthList, YearList });
            }

            else
            {
                return RedirectToAction("_populateExactBlockList", "WorkerInfo", new { MonthList, YearList, });

            }
        }

        public JsonResult generateExactYield()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var yieldBracketMonth = Convert.ToInt32(db.tblOptionConfigsWebs.SingleOrDefault(x =>
                x.fldOptConfFlag1 == "yieldBracketMonth" && x.fld_NegaraID == NegaraID &&
                x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).fldOptConfValue);

            var last12Month = Enumerable
                .Range(0, yieldBracketMonth)
                .Select(i => DateTime.Now.AddMonths(i - yieldBracketMonth))
                .Select(monthYear => new { monthYear.Month, monthYear.Year });

            var getAllPkt = dbr.tbl_PktUtama
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false);

            var getAllSubPkt = dbr.tbl_SubPkt
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false);

            var getAllBlok = dbr.tbl_Blok
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false);

            var getHarvestingDisabledFlagIndicator = Convert.ToInt32(db.tblOptionConfigsWebs.SingleOrDefault(x =>
                x.fldOptConfFlag1 == "upahFlag" && x.fldOptConfFlag2 == "true" && x.fld_NegaraID == NegaraID &&
                x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).fldOptConfValue);

            var getHarvestingCode = db.tbl_JenisAktiviti.SingleOrDefault(x =>
                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false &&
                x.fld_DisabledFlag == getHarvestingDisabledFlagIndicator).fld_KodJnsAktvt;

            foreach (var monthYear in last12Month)
            {
                foreach (var blok in getAllBlok)
                {
                    var blokYieldRecord = dbr.tbl_HasilSawitBlok.SingleOrDefault(x =>
                        x.fld_KodBlok == blok.fld_Blok && x.fld_Bulan == monthYear.Month &&
                        x.fld_Tahun == monthYear.Year &&
                        x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                        x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_YieldType == "EXT");

                    var totalBlockYieldRecordInAMonth = dbr.tbl_Kerja.Where(x =>
                            x.fld_Tarikh.Value.Month == monthYear.Month && x.fld_Tarikh.Value.Year == monthYear.Year &&
                            x.fld_KodPkt == blok.fld_Blok &&
                            x.fld_JnisAktvt == getHarvestingCode && x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                            x.fld_LadangID == LadangID)
                        .Select(s => s.fld_JumlahHasil).Sum();

                    if (blokYieldRecord == null)
                    {
                        tbl_HasilSawitBlok tblHasilSawitBlok = new tbl_HasilSawitBlok();

                        tblHasilSawitBlok.fld_KodBlok = blok.fld_Blok;
                        tblHasilSawitBlok.fld_HasilTan = totalBlockYieldRecordInAMonth == null ? 0 : totalBlockYieldRecordInAMonth;
                        tblHasilSawitBlok.fld_LuasHektar = blok.fld_LsBlok;
                        tblHasilSawitBlok.fld_Bulan = monthYear.Month;
                        tblHasilSawitBlok.fld_Tahun = monthYear.Year;
                        tblHasilSawitBlok.fld_NegaraID = NegaraID;
                        tblHasilSawitBlok.fld_SyarikatID = SyarikatID;
                        tblHasilSawitBlok.fld_WilayahID = WilayahID;
                        tblHasilSawitBlok.fld_LadangID = LadangID;
                        tblHasilSawitBlok.fld_Deleted = false;
                        tblHasilSawitBlok.fld_YieldType = "EXT";
                        tblHasilSawitBlok.fld_CreatedBy = getuserid;
                        tblHasilSawitBlok.fld_CreatedDT = timezone.gettimezone();

                        dbr.tbl_HasilSawitBlok.Add(tblHasilSawitBlok);
                    }
                }

                dbr.SaveChanges();

                foreach (var subPkt in getAllSubPkt)
                {
                    var subPktYieldRecord = dbr.tbl_HasilSawitSubPkt.SingleOrDefault(x =>
                        x.fld_KodSubPeringkat == subPkt.fld_Pkt && x.fld_Bulan == monthYear.Month &&
                        x.fld_Tahun == monthYear.Year &&
                        x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                        x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_YieldType == "EXT");

                    var totalSubPktYieldRecordInAMonth = dbr.tbl_Kerja.Where(x =>
                            x.fld_Tarikh.Value.Month == monthYear.Month && x.fld_Tarikh.Value.Year == monthYear.Year &&
                            x.fld_KodPkt.Contains(subPkt.fld_Pkt) &&
                            x.fld_JnisAktvt == getHarvestingCode && x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                            x.fld_LadangID == LadangID)
                        .Select(s => s.fld_JumlahHasil).Sum();

                    if (subPktYieldRecord == null)
                    {
                        tbl_HasilSawitSubPkt tblHasilSawitSubPkt = new tbl_HasilSawitSubPkt();

                        tblHasilSawitSubPkt.fld_KodSubPeringkat = subPkt.fld_Pkt;
                        tblHasilSawitSubPkt.fld_HasilTan = totalSubPktYieldRecordInAMonth == null ? 0 : totalSubPktYieldRecordInAMonth;
                        tblHasilSawitSubPkt.fld_LuasHektar = subPkt.fld_LsPkt;
                        tblHasilSawitSubPkt.fld_Bulan = monthYear.Month;
                        tblHasilSawitSubPkt.fld_Tahun = monthYear.Year;
                        tblHasilSawitSubPkt.fld_NegaraID = NegaraID;
                        tblHasilSawitSubPkt.fld_SyarikatID = SyarikatID;
                        tblHasilSawitSubPkt.fld_WilayahID = WilayahID;
                        tblHasilSawitSubPkt.fld_LadangID = LadangID;
                        tblHasilSawitSubPkt.fld_Deleted = false;
                        tblHasilSawitSubPkt.fld_YieldType = "EXT";
                        tblHasilSawitSubPkt.fld_CreatedBy = getuserid;
                        tblHasilSawitSubPkt.fld_CreatedDT = timezone.gettimezone();

                        dbr.tbl_HasilSawitSubPkt.Add(tblHasilSawitSubPkt);
                    }
                }

                dbr.SaveChanges();

                foreach (var pkt in getAllPkt)
                {

                    var pktYieldRecord = dbr.tbl_HasilSawitPkt.SingleOrDefault(x =>
                        x.fld_KodPeringkat == pkt.fld_PktUtama && x.fld_Bulan == monthYear.Month &&
                        x.fld_Tahun == monthYear.Year &&
                        x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                        x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_YieldType == "EXT");

                    var totalPktYieldRecordInAMonth = dbr.tbl_Kerja.Where(x =>
                            x.fld_KodPkt.Contains(pkt.fld_PktUtama) && x.fld_Tarikh.Value.Month == monthYear.Month &&
                            x.fld_Tarikh.Value.Year == monthYear.Year &&
                            x.fld_JnisAktvt == getHarvestingCode && x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                            x.fld_LadangID == LadangID)
                        .Select(s => s.fld_JumlahHasil).Sum();

                    if (pktYieldRecord == null)
                    {
                        tbl_HasilSawitPkt tblHasilSawitPkt = new tbl_HasilSawitPkt();

                        tblHasilSawitPkt.fld_KodPeringkat = pkt.fld_PktUtama;
                        tblHasilSawitPkt.fld_HasilTan = totalPktYieldRecordInAMonth == null ? 0 : totalPktYieldRecordInAMonth;
                        tblHasilSawitPkt.fld_LuasHektar = pkt.fld_LsPktUtama;
                        tblHasilSawitPkt.fld_Bulan = monthYear.Month;
                        tblHasilSawitPkt.fld_Tahun = monthYear.Year;
                        tblHasilSawitPkt.fld_NegaraID = NegaraID;
                        tblHasilSawitPkt.fld_SyarikatID = SyarikatID;
                        tblHasilSawitPkt.fld_WilayahID = WilayahID;
                        tblHasilSawitPkt.fld_LadangID = LadangID;
                        tblHasilSawitPkt.fld_Deleted = false;
                        tblHasilSawitPkt.fld_YieldType = "EXT";
                        tblHasilSawitPkt.fld_CreatedBy = getuserid;
                        tblHasilSawitPkt.fld_CreatedDT = timezone.gettimezone();

                        dbr.tbl_HasilSawitPkt.Add(tblHasilSawitPkt);
                    }
                }

                dbr.SaveChanges();
            }

            return Json(new
            {
                success = true,
                msg = GlobalResEstate.msgAdd,
                status = "success",
                checkingdata = "1"
            });

        }

        public ActionResult _populateExactBlockList(int MonthList, int YearList, int page = 1, string sort = "fld_Blok", string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            List<ViewingModels.vw_HasilSawitBlok> vw_HasilSawitBlok = new List<ViewingModels.vw_HasilSawitBlok>();

            var status = false;

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.vw_HasilSawitBlok>();
            int role = GetIdentity.RoleID(getuserid).Value;

            GetConfig.GetYieldStatus(MonthList, YearList, out status, NegaraID, SyarikatID, WilayahID, LadangID, false, host, user,
                catalog, pass, "EXT");

            if (status == false)
            {
                ViewBag.Message = @GlobalResEstate.lblYIeldInfoMessage1 + " " + @GlobalResEstate.lblYIeldInfoMessage2;
            }

            else
            {
                ViewBag.Message = @GlobalResEstate.lblYIeldInfoMessage3;
            }

            vw_HasilSawitBlok = dbview.vw_HasilSawitBlok.Where(x =>
                x.fld_Bulan == MonthList && x.fld_Tahun == YearList && x.fld_NegaraID == NegaraID &&
                x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_YieldType == "EXT").ToList();

            records.Content = vw_HasilSawitBlok
                .OrderBy(sort + " " + sortdir)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            records.TotalRecords = vw_HasilSawitBlok
                .Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.RoleID = role;
            ViewBag.pageSize = pageSize;

            return View(records);
        }

        public ActionResult _populateExactSubPktList(int MonthList, int YearList, int page = 1, string sort = "fld_Pkt",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            List<ViewingModels.vw_HasilSawitSubPkt> vw_HasilSawitSubPkt = new List<ViewingModels.vw_HasilSawitSubPkt>();

            var status = false;

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.vw_HasilSawitSubPkt>();
            int role = GetIdentity.RoleID(getuserid).Value;

            GetConfig.GetYieldStatus(MonthList, YearList, out status, NegaraID, SyarikatID, WilayahID, LadangID, false, host, user,
                catalog, pass, "EXT");

            if (status == false)
            {
                ViewBag.Message = @GlobalResEstate.lblYIeldInfoMessage1 + " " + @GlobalResEstate.lblYIeldInfoMessage2;
            }

            else
            {
                ViewBag.Message = @GlobalResEstate.lblYIeldInfoMessage3;
            }

            vw_HasilSawitSubPkt = dbview.vw_HasilSawitSubPkt.Where(x =>
                x.fld_Bulan == MonthList && x.fld_Tahun == YearList && x.fld_NegaraID == NegaraID &&
                x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_YieldType == "EXT").ToList();

            records.Content = vw_HasilSawitSubPkt
                .OrderBy(sort + " " + sortdir)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            records.TotalRecords = vw_HasilSawitSubPkt
                .Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.RoleID = role;
            ViewBag.pageSize = pageSize;
            return View(records);
        }

        public ActionResult _populateExactPktList(int MonthList, int YearList, int page = 1, string sort = "fld_PktUtama",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            List<ViewingModels.vw_HasilSawitPkt> vw_HasilSawitPkt = new List<ViewingModels.vw_HasilSawitPkt>();

            var status = false;

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.vw_HasilSawitPkt>();
            int role = GetIdentity.RoleID(getuserid).Value;

            GetConfig.GetYieldStatus(MonthList, YearList, out status, NegaraID, SyarikatID, WilayahID, LadangID, false, host, user,
                catalog, pass, "EXT");

            if (status == false)
            {
                ViewBag.Message = @GlobalResEstate.lblYIeldInfoMessage1 + " " + @GlobalResEstate.lblYIeldInfoMessage2;
            }

            else
            {
                ViewBag.Message = @GlobalResEstate.lblYIeldInfoMessage3;
            }

            vw_HasilSawitPkt = dbview.vw_HasilSawitPkt.Where(x =>
                x.fld_Bulan == MonthList && x.fld_Tahun == YearList && x.fld_NegaraID == NegaraID &&
                x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_YieldType == "EXT").ToList();

            records.Content = vw_HasilSawitPkt
                .OrderBy(sort + " " + sortdir)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            records.TotalRecords = vw_HasilSawitPkt
                .Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.RoleID = role;
            ViewBag.pageSize = pageSize;
            return View(records);
        }

        public ActionResult WorkerAcc()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            List<SelectListItem> status = new List<SelectListItem>();

            status = new SelectList(db.tblOptionConfigsWebs
                .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                .OrderBy(o => o.fldOptConfDesc)
                .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc })
                .Distinct(), "Value", "Text").ToList();
            status.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.WorkerInfo = "class = active";
            ViewBag.StatusList = status;
            return View();
        }

        public ActionResult _WorkerAcc(string StatusList = "", int page = 1, string sort = "fld_Nama", string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbo = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<Models.tbl_Pkjmast>();
            //int role = GetIdentity.RoleID(getuserid).Value;

            if (StatusList != "0")
            {
                records.Content = dbo.tbl_Pkjmast.Where(x => x.fld_DivisionID == DivisionID && x.fld_StatusAkaun == StatusList && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                       .OrderBy(sort + " " + sortdir)
                       .Skip((page - 1) * pageSize)
                       .Take(pageSize)
                       .ToList();

                records.TotalRecords = dbo.tbl_Pkjmast.Where(x => x.fld_DivisionID == DivisionID && x.fld_StatusAkaun == StatusList && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Count();
                records.CurrentPage = page;
                records.PageSize = pageSize;
                //ViewBag.AktvtList = AktvtList;
                db.Dispose();
                dbo.Dispose();
            }
            else
            {
                records.Content = dbo.tbl_Pkjmast
                .Where(x => x.fld_DivisionID == DivisionID && x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                       .OrderBy(sort + " " + sortdir)
                       .Skip((page - 1) * pageSize)
                       .Take(pageSize)
                       .ToList();

                records.TotalRecords = dbo.tbl_Pkjmast.Where(x => x.fld_DivisionID == DivisionID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Count();
                records.CurrentPage = page;
                records.PageSize = pageSize;
                db.Dispose();
                dbo.Dispose();
            }

            ViewBag.Datacount = records.TotalRecords;
            return View(records);
        }

        public ActionResult BankAcc(string id)
        {
            GetStatus GetStatus = new GetStatus();
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            //if (id == null)
            //{
            //    return RedirectToAction("WorkerAcc");
            //}
            var tbl_Pkjmast = dbr.tbl_Pkjmast.Where(w => w.fld_Nopkj == id && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID && w.fld_WilayahID == WilayahID && w.fld_LadangID == LadangID).FirstOrDefault();

            //if (tbl_Pkjmast == null)
            //{
            //    return RedirectToAction("WorkerAcc");
            //}

            tbl_PkjmastEdit edit = new tbl_PkjmastEdit();

            List<SelectListItem> statusAcc = new List<SelectListItem>();
            statusAcc = new SelectList(db.tblOptionConfigsWebs
                .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                .OrderBy(o => o.fldOptConfValue)
                .Select(s => new SelectListItem
                {
                    Value = s.fldOptConfValue,
                    Text = s.fldOptConfValue + " - " + s.fldOptConfDesc
                })
                .Distinct(), "Value", "Text", tbl_Pkjmast.fld_StatusAkaun).ToList();
            statusAcc.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            PropertyCopy.Copy(edit, tbl_Pkjmast);

            List<SelectListItem> Bank = new List<SelectListItem>();
            Bank = new SelectList(db.tbl_Bank.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).OrderBy(o => o.fld_KodBank).Select(s => new SelectListItem { Value = s.fld_KodBank, Text = s.fld_NamaBank }).Distinct(), "Value", "Text", tbl_Pkjmast.fld_Kdbank).ToList();
            Bank.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            
            ViewBag.fld_StatusAkaun = statusAcc;
            ViewBag.fld_Kdbank = Bank;
            return View(edit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BankAcc(Models.tbl_PkjmastEdit tbl_Pkjmast)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                if (ModelState.IsValid)
                {
                    var getdata = dbr.tbl_Pkjmast.Where(w => w.fld_Nopkj == tbl_Pkjmast.fld_Nopkj &&
                    w.fld_LadangID == LadangID && w.fld_WilayahID == WilayahID &&
                    w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID).FirstOrDefault();

                    getdata.fld_NoAkaun = tbl_Pkjmast.fld_NoAkaun;
                    getdata.fld_Kdbank = tbl_Pkjmast.fld_Kdbank;
                    getdata.fld_StatusAkaun = tbl_Pkjmast.fld_StatusAkaun;

                    dbr.SaveChanges();

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
                        method = "2",
                        div = "BankMaintenanceDetails",
                        rootUrl = domain,
                        action = "_WorkerAcc",
                        controller = "WorkerInfo",
                        paramName = "Statuslist",
                        paramValue = "0"
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

        public ActionResult KwspSocsoAcc(string id)
        {
            GetStatus GetStatus = new GetStatus();
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            //if (id == null)
            //{
            //    /*return RedirectToAction("WorkerAcc")*/
            //    ViewBag.Message = GlobalResEstate.lblChooseAcc;
            //}

            var tbl_Pkjmast = dbr.tbl_Pkjmast.Where(w => w.fld_Nopkj == id && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID && w.fld_WilayahID == WilayahID && w.fld_LadangID == LadangID/*&& w.fld_LadangID = LadangID*/).FirstOrDefault();

            //if (tbl_Pkjmast == null)
            //{
            //    return RedirectToAction("WorkerAcc");
            //}

            tbl_PkjmastEditKwsp edit = new tbl_PkjmastEditKwsp();

            List<SelectListItem> statusKwspSocso = new List<SelectListItem>();

            statusKwspSocso = new SelectList(db.tblOptionConfigsWebs
                .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                .OrderBy(o => o.fldOptConfValue)
                .Select(s => new SelectListItem
                {
                    Value = s.fldOptConfValue,
                    Text = s.fldOptConfValue + " - " + s.fldOptConfDesc
                })
                .Distinct(), "Value", "Text", tbl_Pkjmast.fld_StatusKwspSocso).ToList();
            statusKwspSocso.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            List<SelectListItem> kwspContributionList = new List<SelectListItem>();
            kwspContributionList = new SelectList(
                db.tbl_JenisCaruman
                    .Where(x => x.fld_JenisCaruman == "KWSP" && x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_KodCaruman)
                    .Select(
                        s => new SelectListItem { Value = s.fld_KodCaruman, Text = s.fld_KodCaruman + " - " + s.fld_Keterangan }), "Value", "Text", tbl_Pkjmast.fld_KodKWSP).ToList();
            kwspContributionList.Insert(0, new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "" });
            ViewBag.KwspContributionList = kwspContributionList;

            List<SelectListItem> socsoContributionList = new List<SelectListItem>();
            socsoContributionList = new SelectList(
                db.tbl_JenisCaruman
                    .Where(x => x.fld_JenisCaruman == "SOCSO" && x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_KodCaruman)
                    .Select(
                        s => new SelectListItem { Value = s.fld_KodCaruman, Text = s.fld_KodCaruman + " - " + s.fld_Keterangan }), "Value", "Text", tbl_Pkjmast.fld_KodSocso).ToList();
            socsoContributionList.Insert(0, new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "" });
            ViewBag.SocsoContributionList = socsoContributionList;

            PropertyCopy.Copy(edit, tbl_Pkjmast);

            ViewBag.fld_StatusKwspSocso = statusKwspSocso;

            return View(edit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KwspSocsoAcc(Models.tbl_PkjmastEditKwsp tbl_Pkjmast)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            
            try
            {

                var getdata = dbr.tbl_Pkjmast.Where(w => w.fld_Nopkj == tbl_Pkjmast.fld_Nopkj &&
                w.fld_LadangID == LadangID && w.fld_WilayahID == WilayahID &&
                w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID).FirstOrDefault();
                if (getdata.fld_Kdrkyt == "MA")
                {
                    getdata.fld_Nokwsp = tbl_Pkjmast.fld_Nokwsp;
                    getdata.fld_Noperkeso = tbl_Pkjmast.fld_Noperkeso;
                    getdata.fld_StatusKwspSocso = tbl_Pkjmast.fld_StatusKwspSocso;
                    getdata.fld_KodSocso = tbl_Pkjmast.fld_KodSocso;
                    getdata.fld_KodKWSP = tbl_Pkjmast.fld_KodKWSP;
                }
                else
                {
                    getdata.fld_Noperkeso = tbl_Pkjmast.fld_Noperkeso;
                }
                dbr.SaveChanges();

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
                    method = "2",
                    div = "KwspSocsoMaintenanceDetails",
                    rootUrl = domain,
                    action = "_KwspSocso",
                    controller = "WorkerInfo",
                    paramName = "Statuslist",
                    paramValue = "0"
                });

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

        //public ActionResult PalmPriceInfo()
        //{
        //    ViewBag.WorkerInfo = "class = active";
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

        //    List<SelectListItem> jenisTanaman = db.tblOptionConfigsWebs
        //        .Where(x => x.fldOptConfFlag1 == "jnsTanaman" && x.fld_NegaraID == NegaraID &&
        //                    x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
        //        .OrderBy(o => o.fldOptConfDesc)
        //        .Select(s => new SelectListItem
        //        {
        //            Value = s.fldOptConfValue,
        //            Text = s.fldOptConfValue + " - " + s.fldOptConfDesc
        //        })
        //        .ToList();
        //    jenisTanaman.Insert(0, new SelectListItem { Text = "Sila Pilih", Value = "" });

        //    ViewBag.JenisTanamanList = jenisTanaman;

        //    return View();
        //}

        //public PartialViewResult _PalmPriceSearch(String JenisTanamanList)
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
        //        NegaraID.Value);
        //    MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
        //    MVC_SYSTEM_Viewing dbview2 = new MVC_SYSTEM_Viewing();

        //    if (String.IsNullOrEmpty(JenisTanamanList))
        //    {
        //        ViewBag.Message = "Sila Pilih Jenis Tanaman";
        //        ViewBag.FirstTime = "True";

        //        return PartialView();
        //    }

        //    else
        //    {
        //        var hargaSawitSemasa = dbview2.vw_HargaSemasa
        //            .SingleOrDefault(x => x.fld_JnsTnmn == JenisTanamanList && x.fld_Bulan == DateTime.Now.Month &&
        //                                  x.fld_Tahun == DateTime.Now.Year &&
        //                                  x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false);

        //        ViewBag.JenisTanamanSelectedValue = JenisTanamanList;

        //        if (hargaSawitSemasa != null)
        //        {
        //            return PartialView(hargaSawitSemasa);
        //        }

        //        else
        //        {
        //            ViewBag.Message = "Tiada Rekod Bagi Bulan Ini";
        //            return PartialView(hargaSawitSemasa);
        //        }
        //    }
        //}

        //public ActionResult _PalmPriceAdd(String JenisTanamanList)
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

        //    List<SelectListItem> jenisTanaman = db.tblOptionConfigsWebs
        //        .Where(x => x.fldOptConfFlag1 == "jnsTanaman" && x.fld_NegaraID == NegaraID &&
        //                    x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
        //        .OrderBy(o => o.fldOptConfDesc)
        //        .Select(s => new SelectListItem
        //        {
        //            Value = s.fldOptConfValue,
        //            Text = s.fldOptConfValue + " - " + s.fldOptConfDesc,
        //        })
        //        .ToList();
        //    jenisTanaman.Insert(0, new SelectListItem { Text = "Sila Pilih", Value = "" });

        //    ViewBag.JenisTanamanList = jenisTanaman;
        //    ViewBag.JenisTanamanSelectedValue = JenisTanamanList;

        //    return PartialView();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult _PalmPriceAdd(MasterModels.tbl_HargaSawitSemasa hargaSemasa, string JenisTanamanList)
        //{
        //    //Check_Balik
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

        //    if (String.IsNullOrEmpty(hargaSemasa.fld_HargaSemasa.ToString()) || String.IsNullOrEmpty(JenisTanamanList))
        //    {
        //        ModelState.AddModelError(string.Empty, "Nilai Harga Semasa Tidak Diisi");
        //    }

        //    var getInsentif = db.tbl_HargaSawitRange
        //        .Where(x => x.fld_RangeHargaLower <= hargaSemasa.fld_HargaSemasa &&
        //                    x.fld_RangeHargaUpper >= hargaSemasa.fld_HargaSemasa && x.fld_NegaraID == NegaraID &&
        //                    x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
        //        .Select(s => s.fld_Insentif)
        //        .SingleOrDefault();

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            hargaSemasa.fld_Bulan = DateTime.Now.Month;
        //            hargaSemasa.fld_JnsTnmn = JenisTanamanList;
        //            hargaSemasa.fld_Insentif = getInsentif;
        //            hargaSemasa.fld_Tahun = DateTime.Now.Year;
        //            hargaSemasa.fld_NegaraID = NegaraID;
        //            hargaSemasa.fld_SyarikatID = SyarikatID;
        //            hargaSemasa.fld_Deleted = false;

        //            db.tbl_HargaSawitSemasa.Add(hargaSemasa);
        //            db.SaveChanges();

        //            db.Dispose();

        //            string appname = Request.ApplicationPath;
        //            string domain = Request.Url.GetLeftPart(UriPartial.Authority);
        //            var lang = Request.RequestContext.RouteData.Values["lang"];
        //    TempData["message"] = GlobalResEstate.msgAdd;
        //            if (appname != "/")
        //            {
        //                domain = domain + appname;
        //            }

        //            return Json(new
        //            {
        //                success = true,
        //                msg = "Harga semasa telah berjaya ditambah",
        //                status = "success",
        //                checkingdata = "0",
        //                method = "2",
        //                div = "searchResultPalmPriceInfo",
        //                rootUrl = domain,
        //                action = "_PalmPriceSearch",
        //                controller = "WorkerInfo",
        //                paramName = "JenisTanamanList",
        //                paramValue = JenisTanamanList
        //            });

        //        }
        //        catch (Exception ex)
        //        {
        //            geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());

        //            db.Dispose();

        //            return Json(new
        //            {
        //                success = false,
        //                msg = "Ralat sistem. Sila hubungi IT.",
        //                status = "danger",
        //                checkingdata = "0"
        //            });
        //        }
        //    }
        //    else
        //    {
        //        db.Dispose();

        //        return Json(new
        //        {
        //            success = false,
        //            msg = "Sila semak semula maklumat yang dimasukkan.",
        //            status = "warning",
        //            checkingdata = "0"
        //        });
        //    }
        //}

        //public ActionResult _PalmPriceEdit(String JenisTanamanList)
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
        //    MVC_SYSTEM_Viewing dbview2 = new MVC_SYSTEM_Viewing();

        //    var hargaSawitSemasa = dbview2.vw_HargaSemasa
        //        .SingleOrDefault(x => x.fld_JnsTnmn == JenisTanamanList && x.fld_Bulan == DateTime.Now.Month &&
        //                              x.fld_Tahun == DateTime.Now.Year &&
        //                              x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false);

        //    return PartialView(hargaSawitSemasa);
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult _PalmPriceEdit(ViewingModels.vw_HargaSemasa hargaSemasa)
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    //MVC_SYSTEM_Viewing dbr = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
        //    if (String.IsNullOrEmpty(hargaSemasa.fld_HargaSemasa.ToString()))
        //    {
        //        ModelState.AddModelError(string.Empty, "Nilai Harga Semasa Tidak Diisi");
        //    }

        //    var hargaSawitSemasaData = db.tbl_HargaSawitSemasa
        //        .Where(x => x.fld_JnsTnmn == hargaSemasa.fld_JnsTnmn && x.fld_Bulan == DateTime.Now.Month &&
        //                    x.fld_Tahun == DateTime.Now.Year &&
        //                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false);

        //    var getInsentif = db.tbl_HargaSawitRange
        //        .Where(x => x.fld_RangeHargaLower <= hargaSemasa.fld_HargaSemasa &&
        //                    x.fld_RangeHargaUpper >= hargaSemasa.fld_HargaSemasa && x.fld_NegaraID == NegaraID &&
        //                    x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
        //        .Select(s => s.fld_Insentif)
        //        .SingleOrDefault();

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var materializehargaSawitSemasaData = hargaSawitSemasaData.Single();

        //            materializehargaSawitSemasaData.fld_HargaSemasa = hargaSemasa.fld_HargaSemasa;
        //            materializehargaSawitSemasaData.fld_JnsTnmn = hargaSemasa.fld_JnsTnmn;
        //            materializehargaSawitSemasaData.fld_Insentif = getInsentif;

        //            db.Entry(materializehargaSawitSemasaData).State = EntityState.Modified;

        //            db.SaveChanges();

        //    TempData["message1"] = GlobalResEstate.msgUpdate;
        //            db.Dispose();

        //            string appname = Request.ApplicationPath;
        //            string domain = Request.Url.GetLeftPart(UriPartial.Authority);
        //            var lang = Request.RequestContext.RouteData.Values["lang"];

        //            if (appname != "/")
        //            {
        //                domain = domain + appname;
        //            }

        //            return Json(new
        //            {
        //                success = true,
        //                msg = "Harga semasa telah berjaya dikemaskini",
        //                status = "success",
        //                checkingdata = "0",
        //                method = "2",
        //                div = "searchResultPalmPriceInfo",
        //                rootUrl = domain,
        //                action = "_PalmPriceSearch",
        //                controller = "WorkerInfo",
        //                paramName = "JenisTanamanList",
        //                paramValue = hargaSemasa.fld_JnsTnmn
        //            });

        //        }
        //        catch (Exception ex)
        //        {
        //            geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());

        //            db.Dispose();

        //            return Json(new
        //            {
        //                success = false,
        //                msg = "Ralat sistem. Sila hubungi IT.",
        //                status = "danger",
        //                checkingdata = "0"
        //            });
        //        }
        //    }
        //    else
        //    {
        //        db.Dispose();

        //        return Json(new
        //        {
        //            success = false,
        //            msg = "Sila semak semula maklumat yang dimasukkan.",
        //            status = "warning",
        //            checkingdata = "0"
        //        });
        //    }
        //}

        public ActionResult SkbNo(string MonthList = "", int YearList = 0, int page = 1, string sort = "fld_ID", string sortdir = "ASC")
        {

            //ViewBag.Maintenance = "class = active";
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            //MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            ViewBag.WorkerInfo = "class = active";

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
                records.Content = dbview.tbl_Skb.Where(x => x.fld_Bulan == MonthList && x.fld_Tahun == YearList && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false)
                       .OrderBy(sort + " " + sortdir)
                       .Skip((page - 1) * pageSize)
                       .Take(pageSize)
                       .ToList();

                records.TotalRecords = dbview.tbl_Skb.Where(x => x.fld_Bulan == MonthList && x.fld_Tahun == YearList && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).Count();
                records.CurrentPage = page;
                records.PageSize = pageSize;

                ViewBag.MonthList = monthList;
                ViewBag.YearList = yearlist;
                ViewBag.Datacount = records.TotalRecords;
                return View(records);
            }
        }

        public ActionResult SkbNoInsert()
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            int range = int.Parse(GetConfig.GetData("yeardisplay"));
            int startyear = DateTime.Now.AddYears(-range).Year;
            int currentyear = DateTime.Now.Year;
            DateTime selectdate = DateTime.Now.AddMonths(-1);

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

            ViewBag.fld_Bulan = monthList;
            ViewBag.fld_Tahun = yearlist;
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SkbNoInsert(Models.tbl_Skb tbl_Skb)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var checkdata = dbr.tbl_Skb.Where(x => x.fld_Bulan == tbl_Skb.fld_Bulan && x.fld_Tahun == tbl_Skb.fld_Tahun && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).FirstOrDefault();
            if (checkdata == null)
            {
                tbl_Skb.fld_SyarikatID = SyarikatID;
                tbl_Skb.fld_NegaraID = NegaraID;
                tbl_Skb.fld_WilayahID = WilayahID;
                tbl_Skb.fld_LadangID = LadangID;
                tbl_Skb.fld_Deleted = false;
                dbr.tbl_Skb.Add(tbl_Skb);
                dbr.SaveChanges();
                dbr.Dispose();
                //var getid = db.tbl_Ladang.Where(w => w.fld_ID == tbl_Ladang.fld_ID).FirstOrDefault();
                return Json(new { success = true, msg = GlobalResEstate.msgAdd, status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
            }
            else
            {
                return Json(new { success = true, msg = "Data already exist.", status = "warning", checkingdata = "1" });
            }
        }

        public ActionResult SkbNoUpdate(int id)
        {
            if (id < 1)
            {
                return RedirectToAction("SkbNo");
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
                return RedirectToAction("SkbNo");
            }

            return PartialView("SkbNoUpdate", tbl_Skb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SkbNoUpdate(int id, Models.tbl_Skb tbl_Skb)
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
                return Json(new { success = true, msg =GlobalResEstate.msgErrorData, status = "warning", checkingdata = "1" });
            }
        }

        public ActionResult SkbNoDelete(int id)
        {
            if (id < 1)
            {
                return RedirectToAction("SkbNo");
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
                return RedirectToAction("SkbNo");
            }
            return PartialView("SkbNoDelete", tbl_Skb);
        }

        [HttpPost, ActionName("SkbNoDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult SkbNoDeleteConfirmed(int id)
        {
            try
            {
                int? getuserid = GetIdentity.ID(User.Identity.Name);
                int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
                string host, catalog, user, pass = "";
                GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
                Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
                MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

                Models.tbl_Skb tbl_Skb = dbr.tbl_Skb.Where(w => w.fld_ID == id).FirstOrDefault();
                if (tbl_Skb == null)
                {
                    return Json(new { success = true, msg = "Data already deleted.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
                }
                else
                {
                    dbr.tbl_Skb.Remove(tbl_Skb);
                    dbr.SaveChanges();
                    return Json(new { success = true, msg = GlobalResEstate.msgDelete2, status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
                }

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = true, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }

        }

        public ActionResult MinimumWageInfo(int? YearList, int? MonthList, int page = 1, string sort = "fld_Nama",
            string sortdir = "ASC")
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            ViewBag.WorkerInfo = "class = active";

            int drpyear = 0;
            int drprangeyear = 0;
   
            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == timezone.gettimezone().Year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            ViewBag.YearList = yearlist;

            var month = timezone.gettimezone().Month;

            ViewBag.MonthList = new SelectList(
                db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false &&
                                                   x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID),
                "fldOptConfValue", "fldOptConfDesc", month);

            return View();
        }

        public PartialViewResult _MinimumWageInfoSearch(int? MonthList, int? YearList, int page = 1,
            string sort = "Nama", string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Viewing dbview2 = new MVC_SYSTEM_Viewing();

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<CustMod_MinimumWage>();
            int role = GetIdentity.RoleID(getuserid).Value;

            List<CustMod_MinimumWage> GajiMinimaList = new List<CustMod_MinimumWage>();

            var minimumWageValue = dbview2.tblOptionConfigsWeb
                .Where(x => x.fldOptConfFlag1 == "gajiMinima" && x.fldDeleted == false && x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID)
                .Select(s => s.fldOptConfValue)
                .Single();

            var minimumWageInt = Convert.ToInt32(minimumWageValue);

            var getWageBelow1000 = dbview.vw_GajiMinima
                .Where(x => x.fld_ByrKerja < minimumWageInt && x.fld_Month == MonthList && x.fld_Year == YearList &&
                            x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

            foreach (var wage in getWageBelow1000)
            {
                var hadirHariBiasaByBulan = dbview.tbl_Kerjahdr
                    .Count(x => x.fld_Kdhdct == "H01" && x.fld_Nopkj == wage.fld_Nopkj &&
                                x.fld_Tarikh.Value.Month == MonthList &&
                                x.fld_Tarikh.Value.Year == YearList &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                CustMod_MinimumWage minimumWage = new CustMod_MinimumWage();

                minimumWage.IDPkj = wage.fld_UniqueID;
                minimumWage.NoPkj = wage.fld_Nopkj;
                minimumWage.Nama = wage.fld_Nama;
                minimumWage.Warganegara = wage.fld_Kdrkyt;
                minimumWage.Nokp = wage.fld_Nokp;
                minimumWage.TarikhSahJawatan = wage.fld_Trshjw;
                minimumWage.KategoriKerja = wage.fld_Ktgpkj;
                minimumWage.JumlahHariBekerja = hadirHariBiasaByBulan;
                minimumWage.GajiBulanan = wage.fld_ByrKerja;
                minimumWage.Sebab = wage.fld_Sebab;
                minimumWage.PelanTindakan = wage.fld_Tindakan;
                minimumWage.NegaraID = NegaraID;
                minimumWage.SyarikatID = SyarikatID;
                minimumWage.IDSebab = wage.fld_GajiMinimaID;

                GajiMinimaList.Add(minimumWage);
            }

            records.Content = GajiMinimaList
                .OrderBy(sort + " " + sortdir)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            records.TotalRecords = GajiMinimaList
                .Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.RoleID = role;
            ViewBag.pageSize = pageSize;

            if (MonthList != null && YearList != null)
            {
                ViewBag.Month = MonthList;
                ViewBag.Year = YearList;
            }

            else
            {
                ViewBag.Year = DateTime.Now.Year;
                ViewBag.Month = DateTime.Now.Month;
            }

            List<SelectListItem> ReasonList = new List<SelectListItem>();

            ReasonList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "sebabgajiMinima" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(
                        s => new SelectListItem {Value = s.fldOptConfValue, Text = s.fldOptConfDesc}),
                "Value", "Text").ToList();
            ReasonList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "" }));

            List<SelectListItem> ActionList = new List<SelectListItem>();

            ActionList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "tindakanGajiMinima" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(
                        s => new SelectListItem {Value = s.fldOptConfValue, Text = s.fldOptConfDesc}),
                "Value", "Text").ToList();
            ActionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "" }));

            ViewBag.ReasonList = ReasonList;
            ViewBag.ActionList = ActionList;

            return PartialView(records);
        }

        public JsonResult addReasonInfo(string nopkj, string reason, string action, int month, int year)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            if (String.IsNullOrEmpty(reason) || String.IsNullOrEmpty(action))
            {
                return Json(new
                {
                    success = true,
                    msg = GlobalResEstate.msgErrorData,
                    status = "danger",
                    checkingdata = "1"
                });
            }

            else
            {
                var checkReasonRecord = dbr.tbl_GajiMinima
                    .Where(x => x.fld_Nopkj == nopkj && x.fld_Year == year &&
                                x.fld_Month == month && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_LadangID == LadangID && x.fld_Deleted == false)
                    .ToList();

                if (checkReasonRecord.Count == 0)
                {
                    Models.tbl_GajiMinima GajiMinima = new Models.tbl_GajiMinima();

                    GajiMinima.fld_Nopkj = nopkj;
                    GajiMinima.fld_Year = Convert.ToInt16(year);
                    GajiMinima.fld_Month = Convert.ToInt16(month);
                    GajiMinima.fld_Sebab = reason;
                    GajiMinima.fld_Tindakan = action;
                    GajiMinima.fld_NegaraID = NegaraID;
                    GajiMinima.fld_SyarikatID = SyarikatID;
                    GajiMinima.fld_WilayahID = WilayahID;
                    GajiMinima.fld_LadangID = LadangID;
                    GajiMinima.fld_Deleted = false;

                    dbr.tbl_GajiMinima.Add(GajiMinima);

                    dbr.SaveChanges();

                    return Json(new
                    {
                        success = true,

                        msg =GlobalResEstate.msgAdd,
                        status = "success",
                        checkingdata = "1"
                    });
                }

                else
                {
                    var materializeMinimumWageRec = checkReasonRecord.Single();

                    materializeMinimumWageRec.fld_Sebab = reason;
                    materializeMinimumWageRec.fld_Tindakan = action;

                    dbr.Entry(materializeMinimumWageRec).State = EntityState.Modified;
                    dbr.SaveChanges();

                    return Json(new
                    {
                        success = true,
                        msg = GlobalResEstate.msgUpdate,
                        status = "success",
                        checkingdata = "1"
                    });
                }
            }
        }

        //public ActionResult testCalculateWorkingDay()
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Viewing dbview2 = new MVC_SYSTEM_Viewing();
        //    MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

        //    var year = DateTime.Now.Year;
        //    var month = DateTime.Now.Month;

        //    //mempaga 1
        //    var kodNegeriLadang = db.tbl_Ladang
        //        .Where(x => x.fld_ID == 92)
        //        .Select(s => s.fld_KodNegeri)
        //        .Single();

        //    //get list of month

        //    //get all kod negeri in malaysia
        //    var getMingguNegeri = dbview2.vw_MingguNegeri
        //        .Single(x => x.fld_NegeriID == kodNegeriLadang && x.fld_NegaraID == NegaraID &&
        //get all kod negeri in malaysia
        //var getMingguNegeri = dbview2.vw_MingguNegeri
        //    .Single(x => x.fld_NegeriID.ToString() == kodNegeriLadang && x.fld_NegaraID == NegaraID &&
        //var getMingguNegeri = dbview2.vw_MingguNegeri
        //    .Single(x => x.fld_NegeriID == int.Parse(kodNegeriLadang) && x.fld_NegaraID == NegaraID &&

        //                     x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false);

        //    DateTime testDate = new DateTime(2018, 1, 17);

        //    if (getMingguNegeri.fld_JenisMinggu == 1)
        //    {
        //        var cutiUmum = db.tbl_CutiUmum

        //            .Where(x => x.fld_Negeri == kodNegeriLadang && x.fld_Tahun == year && x.fld_NegaraID == NegaraID &&
        //                        x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false &&
        //                        x.fld_TarikhCuti.Value.Month == month && x.fld_TarikhCuti > DateTime.Now)
        //            .ToList();
        //.Where(x => x.fld_Negeri.ToString() == kodNegeriLadang && x.fld_Tahun == year && x.fld_NegaraID == NegaraID &&
        //            x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false &&
        //            x.fld_TarikhCuti.Value.Month == month && x.fld_TarikhCuti > DateTime.Now)
        //.ToList();
        //.Where(x => x.fld_Negeri == int.Parse(kodNegeriLadang) && x.fld_Tahun == year && x.fld_NegaraID == NegaraID &&
        //            x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false &&
        //            x.fld_TarikhCuti.Value.Month == month && x.fld_TarikhCuti > DateTime.Now)
        //.ToList();

        //        DateTime endOfMonth = new DateTime(year, Convert.ToInt32(month),
        //            DateTime.DaysInMonth(year, Convert.ToInt32(month)));

        //        DateDiff workingDay = new DateDiff(testDate.AddDays(-1), endOfMonth);

        //        //get only last day of month
        //        int day = endOfMonth.Day;

        //        int sunday = 0;

        //        for (int i = workingDay.Days; i < day; ++i)
        //        {
        //            DateTime d = new DateTime(year, Convert.ToInt32(month), i + 1);

        //            //Compare date with sunday
        //            if (d.DayOfWeek == DayOfWeek.Sunday)
        //            {
        //                sunday = sunday + 1;
        //            }
        //        }

        //        var workingDaysInMonth = workingDay.Days - sunday -
        //                                 cutiUmum.Count;

        //        Models.tbl_Produktiviti Produktiviti = new Models.tbl_Produktiviti();
        //        Produktiviti.fld_Nopkj = "ABCDEFG";
        //        Produktiviti.fld_HadirKerja = workingDaysInMonth;
        //        dbr.tbl_Produktiviti.Add(Produktiviti);
        //        dbr.SaveChanges();
        //    }

        //    if (getMingguNegeri.fld_JenisMinggu == 2)
        //    {
        //        var cutiUmum = db.tbl_CutiUmum
        //            .Where(x => x.fld_Negeri == kodNegeriLadang && x.fld_Tahun == year && x.fld_NegaraID == NegaraID &&
        //                        x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false &&
        //                        x.fld_TarikhCuti.Value.Month == month && x.fld_TarikhCuti > DateTime.Now)
        //            .ToList();
        //if (getMingguNegeri.fld_JenisMinggu == 2)
        //{
        //    var cutiUmum = db.tbl_CutiUmum
        //        .Where(x => x.fld_Negeri.ToString() == kodNegeriLadang && x.fld_Tahun == year && x.fld_NegaraID == NegaraID &&
        //                    x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false &&
        //                    x.fld_TarikhCuti.Value.Month == month && x.fld_TarikhCuti > DateTime.Now)
        //        .ToList();
        //if (getMingguNegeri.fld_JenisMinggu == 2)
        //{
        //    var cutiUmum = db.tbl_CutiUmum
        //        .Where(x => x.fld_Negeri == int.Parse(kodNegeriLadang) && x.fld_Tahun == year && x.fld_NegaraID == NegaraID &&
        //                    x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false &&
        //                    x.fld_TarikhCuti.Value.Month == month && x.fld_TarikhCuti > DateTime.Now)
        //        .ToList();

        //        DateTime endOfMonth = new DateTime(year, Convert.ToInt32(month),
        //            DateTime.DaysInMonth(year, Convert.ToInt32(month)));

        //        //get only last day of month
        //        int day = endOfMonth.Day;

        //        int friday = 0;

        //        for (int i = Convert.ToInt32(DateTime.Now.Date); i < day; ++i)
        //        {
        //            DateTime d = new DateTime(year, Convert.ToInt32(month), i + 1);

        //            //Compare date with sunday
        //            if (d.DayOfWeek == DayOfWeek.Friday)
        //            {
        //                friday = friday + 1;
        //            }
        //        }

        //        var workingDaysInMonth = Convert.ToInt32(DateTime.Now) - friday -
        //                                 cutiUmum.Count;

        //        //Models.tbl_Produktiviti Produktiviti = new tbl_Produktiviti();
        //        //Produktiviti.fld_Nopkj = workerData.fld_Nopkj;
        //        //Produktiviti.fld_HadirKerja = workingDaysInMonth;
        //        //dbr.tbl_Produktiviti.Add(Produktiviti);
        //    }

        //    return View();
        //}

        public ActionResult KwspSocso()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            
            List<SelectListItem> status = new List<SelectListItem>();
            
            status = new SelectList(db.tblOptionConfigsWebs
                .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                .OrderBy(o => o.fldOptConfDesc)
                .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc })
                .Distinct(), "Value", "Text").ToList();
            status.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            List<SelectListItem> JnsPkjList = new List<SelectListItem>();
            JnsPkjList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

            ViewBag.WorkerInfo = "class = active";
            ViewBag.StatusList = status;
            ViewBag.JnsPkjList = JnsPkjList;
            return View();
        }

        public ActionResult _KwspSocso(string StatusList = "", string JnsPkjList = "", int page = 1, string sort = "fld_Nama", string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            //MVC_SYSTEM_Models dbo = new MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<Models.tbl_Pkjmast>();
            
            //int role = GetIdentity.RoleID(getuserid).Value;

            var message = "";

            if (String.IsNullOrEmpty(StatusList))
            {
                message = GlobalResEstate.lblChooseAcc;
            }

            else
            {
                message = GlobalResEstate.msgErrorSearch;
            }

            if (JnsPkjList == "TT")
            {
                if (StatusList != "0")
                {
                    records.Content = dbr.tbl_Pkjmast.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Kdrkyt == "MA" && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                           .OrderBy(sort + " " + sortdir)
                           .Skip((page - 1) * pageSize)
                           .Take(pageSize)
                           .ToList();

                    records.TotalRecords = dbr.tbl_Pkjmast.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Kdrkyt == "MA" && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                    //ViewBag.AktvtList = AktvtList;
                    db.Dispose();
                    dbr.Dispose();
                }
                else
                {
                    records.Content = dbr.tbl_Pkjmast
                    .Where(x => x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID &&
                                x.fld_Kdrkyt == "MA" && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                           .OrderBy(sort + " " + sortdir)
                           .Skip((page - 1) * pageSize)
                           .Take(pageSize)
                           .ToList();

                    records.TotalRecords = dbr.tbl_Pkjmast.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Kdrkyt == "MA" && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                    db.Dispose();
                    dbr.Dispose();
                }
            }
            else
            {
                records.Content = dbr.tbl_Pkjmast
                    .Where(x => x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID &&
                                x.fld_Kdrkyt != "MA" && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                           .OrderBy(sort + " " + sortdir)
                           .Skip((page - 1) * pageSize)
                           .Take(pageSize)
                           .ToList();

                records.TotalRecords = dbr.tbl_Pkjmast.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Kdrkyt != "MA" && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Count();
                records.CurrentPage = page;
                records.PageSize = pageSize;
                db.Dispose();
                dbr.Dispose();
            }

            

            ViewBag.Datacount = records.TotalRecords;
            ViewBag.Message = message;
            return View(records);
        }

        public ActionResult PassportExpiryReasonInfo(string filter, int page = 1, string sort = "fld_Nopkj",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            //string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.WorkerInfo = "class = active";

            return View();
        }

        public ActionResult _PassportExpiryReasonInfo(string filter, int page = 1,
            string sort = "fld_Nopkj",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<Models.tbl_TamatPermitPassportViewModelList>();
            int role = GetIdentity.RoleID(getuserid).Value;

            List<tbl_Pkjmast> pkjMastList = new List<tbl_Pkjmast>();
            List<tbl_TamatPermitPassportViewModelList> tamatPermitPassportViewModelList = new List<tbl_TamatPermitPassportViewModelList>();

            var currentDate = timezone.gettimezone();

            var passportExpiryCode = db.tblOptionConfigsWebs.SingleOrDefault(x =>
                x.fldOptConfFlag1 == "passportExpryCode" && x.fld_NegaraID == NegaraID &&
                x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).fldOptConfValue;

            if (!String.IsNullOrEmpty(filter))
            {
                pkjMastList = dbr.tbl_Pkjmast.Where(x =>
                        (x.fld_Nopkj.ToUpper().Contains(filter.ToUpper()) ||
                         x.fld_Nama.ToUpper().Contains(filter.ToUpper())) &&
                        x.fld_Kdaktf == "1" && x.fld_NegaraID == NegaraID &&
                        x.fld_SyarikatID == SyarikatID &&
                        x.fld_WilayahID == WilayahID &&
                        x.fld_LadangID == LadangID && x.fld_Kdrkyt != "MA" &&
                        DbFunctions.TruncateTime(x.fld_T2pspt) < DbFunctions.TruncateTime(currentDate))
                    .OrderBy(o => o.fld_Nama).ToList();
            }

            else
            {
                pkjMastList = dbr.tbl_Pkjmast.Where(x =>
                        x.fld_Kdaktf == "1" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                        x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdrkyt != "MA" &&
                        DbFunctions.TruncateTime(x.fld_T2pspt) < DbFunctions.TruncateTime(currentDate))
                    .OrderBy(o => o.fld_Nama).ToList();
            }

            foreach (var pkjMast in pkjMastList)
            {
                var passportExpiryReasonData = dbr.tbl_TamatPermitPassport.SingleOrDefault(x =>
                    x.fld_Nopkj == pkjMast.fld_Nopkj && x.fld_KategoriSebab == passportExpiryCode &&
                    DbFunctions.TruncateTime(x.fld_TarikhTamat) == DbFunctions.TruncateTime(pkjMast.fld_T2pspt) &&
                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WIlayahID == WilayahID &&
                    x.fld_LadangID == LadangID && x.fld_Deleted == false);

                if (passportExpiryReasonData != null)
                {
                    tamatPermitPassportViewModelList.Add(
                        new Models.tbl_TamatPermitPassportViewModelList
                        {
                            fld_Nopkj = pkjMast.fld_Nopkj,
                            fld_TarikhTamat = pkjMast.fld_T2pspt,
                            fld_SebabDesc = passportExpiryReasonData.fld_SebabDesc,
                            fld_ReasonID = passportExpiryReasonData.fld_ReasonID,
                            fld_NegaraID = pkjMast.fld_NegaraID,
                            fld_SyarikatID = pkjMast.fld_SyarikatID,
                            fld_WIlayahID = pkjMast.fld_WilayahID,
                            fld_LadangID = pkjMast.fld_LadangID,
                            fld_IsExist = true
                        });
                }

                else
                {
                    tamatPermitPassportViewModelList.Add(
                        new Models.tbl_TamatPermitPassportViewModelList
                        {
                            fld_Nopkj = pkjMast.fld_Nopkj,
                            fld_TarikhTamat = pkjMast.fld_T2pspt,
                            fld_NegaraID = pkjMast.fld_NegaraID,
                            fld_SyarikatID = pkjMast.fld_SyarikatID,
                            fld_WIlayahID = pkjMast.fld_WilayahID,
                            fld_LadangID = pkjMast.fld_LadangID,
                            fld_IsExist = false
                        });
                }
            }

            records.Content = tamatPermitPassportViewModelList.OrderBy(sort + " " + sortdir)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            records.TotalRecords = tamatPermitPassportViewModelList
                .Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.RoleID = role;
            ViewBag.pageSize = 1;
            ViewBag.Host = host;
            ViewBag.User = user;
            ViewBag.Catalog = catalog;
            ViewBag.Pass = pass;

            return View(records);
        }

        public ActionResult _PassportExpiryReasonInfoCreate(string id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var workerData = dbr.tbl_Pkjmast.SingleOrDefault(x =>
                x.fld_Nopkj == id && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

            tbl_TamatPermitPassportViewModelCreate tamatPermitPassportViewModelCreate = new tbl_TamatPermitPassportViewModelCreate();

            tamatPermitPassportViewModelCreate.fld_Nopkj = workerData.fld_Nopkj;
            tamatPermitPassportViewModelCreate.fld_TarikhTamat = workerData.fld_T2pspt;
            tamatPermitPassportViewModelCreate.fld_NegaraID = workerData.fld_NegaraID;
            tamatPermitPassportViewModelCreate.fld_SyarikatID = workerData.fld_SyarikatID;
            tamatPermitPassportViewModelCreate.fld_WIlayahID = workerData.fld_WilayahID;
            tamatPermitPassportViewModelCreate.fld_LadangID = workerData.fld_LadangID;

            ViewBag.Host = host;
            ViewBag.User = user;
            ViewBag.Catalog = catalog;
            ViewBag.Pass = pass;

            return PartialView(tamatPermitPassportViewModelCreate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _PassportExpiryReasonInfoCreate(tbl_TamatPermitPassportViewModelCreate tamatPermitPassportViewModelCreate)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                if (ModelState.IsValid)
                {
                    var passportExpiryCode = db.tblOptionConfigsWebs.SingleOrDefault(x =>
                        x.fldOptConfFlag1 == "passportExpryCode" && x.fld_NegaraID == NegaraID &&
                        x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).fldOptConfValue;

                    var passportExpiryData = dbr.tbl_TamatPermitPassport.SingleOrDefault(x =>
                        x.fld_Nopkj == tamatPermitPassportViewModelCreate.fld_Nopkj &&
                        DbFunctions.TruncateTime(x.fld_TarikhTamat) ==
                        DbFunctions.TruncateTime(tamatPermitPassportViewModelCreate.fld_TarikhTamat) &&
                        x.fld_KategoriSebab == passportExpiryCode &&
                        x.fld_NegaraID == NegaraID &&
                        x.fld_SyarikatID == SyarikatID && x.fld_WIlayahID == WilayahID && x.fld_LadangID == LadangID &&
                        x.fld_Deleted == false);

                    if (passportExpiryData != null)
                    {
                        passportExpiryData.fld_Deleted = true;
                    }

                    Models.tbl_TamatPermitPassport tamatPermitPassport = new Models.tbl_TamatPermitPassport();

                    PropertyCopy.Copy(tamatPermitPassport, tamatPermitPassportViewModelCreate);

                    tamatPermitPassport.fld_KategoriSebab = passportExpiryCode;
                    tamatPermitPassport.fld_NegaraID = NegaraID;
                    tamatPermitPassport.fld_SyarikatID = SyarikatID;
                    tamatPermitPassport.fld_WIlayahID = WilayahID;
                    tamatPermitPassport.fld_LadangID = LadangID;
                    tamatPermitPassport.fld_Deleted = false;

                    dbr.tbl_TamatPermitPassport.Add(tamatPermitPassport);
                    dbr.SaveChanges();

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
                        msg = GlobalResEstate.msgAdd,
                        status = "success",
                        checkingdata = "0",
                        method = "1",
                        div = "passportExpiryDetails",
                        rootUrl = domain,
                        action = "_PassportExpiryReasonInfo",
                        controller = "WorkerInfo"
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

        public ActionResult _PassportExpiryReasonInfoEdit(Guid id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var passportExpiryData = dbr.tbl_TamatPermitPassport
                .SingleOrDefault(x => x.fld_ReasonID == id && x.fld_Deleted == false);

            tbl_TamatPermitPassportViewModelEdit tamatPermitPassportViewModelEdit = new tbl_TamatPermitPassportViewModelEdit();

            PropertyCopy.Copy(tamatPermitPassportViewModelEdit, passportExpiryData);

            ViewBag.Host = host;
            ViewBag.User = user;
            ViewBag.Catalog = catalog;
            ViewBag.Pass = pass;

            return PartialView(tamatPermitPassportViewModelEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _PassportExpiryReasonInfoEdit(tbl_TamatPermitPassportViewModelEdit tamatPermitPassportViewModelEdit)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                if (ModelState.IsValid)
                {
                    var passportExpiryData = dbr.tbl_TamatPermitPassport
                        .SingleOrDefault(x => x.fld_ReasonID == tamatPermitPassportViewModelEdit.fld_ReasonID && x.fld_Deleted == false);

                    passportExpiryData.fld_SebabDesc = tamatPermitPassportViewModelEdit.fld_SebabDesc;

                    dbr.SaveChanges();

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
                        div = "passportExpiryDetails",
                        rootUrl = domain,
                        action = "_PassportExpiryReasonInfo",
                        controller = "WorkerInfo"
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

        public ActionResult _PassportExpiryReasonInfoDelete(Guid id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var passportExpiryData = dbr.tbl_TamatPermitPassport
                .SingleOrDefault(x => x.fld_ReasonID == id && x.fld_Deleted == false);

            ViewBag.Host = host;
            ViewBag.User = user;
            ViewBag.Catalog = catalog;
            ViewBag.Pass = pass;

            return PartialView(passportExpiryData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _PassportExpiryReasonInfoDelete(tbl_TamatPermitPassport tamatPermitPassport)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                var passportExpiryData = dbr.tbl_TamatPermitPassport
                    .SingleOrDefault(x => x.fld_ReasonID == tamatPermitPassport.fld_ReasonID && x.fld_Deleted == false);

                passportExpiryData.fld_Deleted = true;

                dbr.SaveChanges();

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
                    msg = GlobalResEstate.msgDelete2,
                    status = "success",
                    checkingdata = "0",
                    method = "1",
                    div = "passportExpiryDetails",
                    rootUrl = domain,
                    action = "_PassportExpiryReasonInfo",
                    controller = "WorkerInfo"
                });

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

        public ActionResult PermitExpiryReasonInfo(string filter, int page = 1, string sort = "fld_Nopkj",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            //string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.WorkerInfo = "class = active";

            return View();
        }

        public ActionResult _PermitExpiryReasonInfo(string filter, int page = 1,
            string sort = "fld_Nopkj",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<Models.tbl_TamatPermitPassportViewModelList>();
            int role = GetIdentity.RoleID(getuserid).Value;

            List<tbl_Pkjmast> pkjMastList = new List<tbl_Pkjmast>();
            List<tbl_TamatPermitPassportViewModelList> tamatPermitPassportViewModelList = new List<tbl_TamatPermitPassportViewModelList>();

            var currentDate = timezone.gettimezone();

            var permitExpiryCode = db.tblOptionConfigsWebs.SingleOrDefault(x =>
                x.fldOptConfFlag1 == "permitExpryCode" && x.fld_NegaraID == NegaraID &&
                x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).fldOptConfValue;

            if (!String.IsNullOrEmpty(filter))
            {
                pkjMastList = dbr.tbl_Pkjmast.Where(x =>
                    (x.fld_Nopkj.ToUpper().Contains(filter.ToUpper()) ||
                     x.fld_Nama.ToUpper().Contains(filter.ToUpper())) &&
                    x.fld_Kdaktf == "1" && x.fld_NegaraID == NegaraID &&
                    x.fld_SyarikatID == SyarikatID &&
                    x.fld_WilayahID == WilayahID &&
                    x.fld_LadangID == LadangID && x.fld_Kdrkyt != "MA" && DbFunctions.TruncateTime(x.fld_T2prmt) <
                    DbFunctions.TruncateTime(currentDate)).OrderBy(o => o.fld_Nama).ToList();
            }

            else
            {
                pkjMastList = dbr.tbl_Pkjmast.Where(x =>
                        x.fld_Kdaktf == "1" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                        x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdrkyt != "MA" &&
                        DbFunctions.TruncateTime(x.fld_T2prmt) < DbFunctions.TruncateTime(currentDate))
                    .OrderBy(o => o.fld_Nama).ToList();
            }

            foreach (var pkjMast in pkjMastList)
            {
                var passportExpiryReasonData = dbr.tbl_TamatPermitPassport.SingleOrDefault(x =>
                    x.fld_Nopkj == pkjMast.fld_Nopkj && x.fld_KategoriSebab == permitExpiryCode &&
                    DbFunctions.TruncateTime(x.fld_TarikhTamat) == DbFunctions.TruncateTime(pkjMast.fld_T2prmt) &&
                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WIlayahID == WilayahID &&
                    x.fld_LadangID == LadangID && x.fld_Deleted == false);

                if (passportExpiryReasonData != null)
                {
                    tamatPermitPassportViewModelList.Add(
                        new Models.tbl_TamatPermitPassportViewModelList
                        {
                            fld_Nopkj = pkjMast.fld_Nopkj,
                            fld_TarikhTamat = pkjMast.fld_T2pspt,
                            fld_SebabDesc = passportExpiryReasonData.fld_SebabDesc,
                            fld_ReasonID = passportExpiryReasonData.fld_ReasonID,
                            fld_NegaraID = pkjMast.fld_NegaraID,
                            fld_SyarikatID = pkjMast.fld_SyarikatID,
                            fld_WIlayahID = pkjMast.fld_WilayahID,
                            fld_LadangID = pkjMast.fld_LadangID,
                            fld_IsExist = true
                        });
                }

                else
                {
                    tamatPermitPassportViewModelList.Add(
                        new Models.tbl_TamatPermitPassportViewModelList
                        {
                            fld_Nopkj = pkjMast.fld_Nopkj,
                            fld_TarikhTamat = pkjMast.fld_T2pspt,
                            fld_NegaraID = pkjMast.fld_NegaraID,
                            fld_SyarikatID = pkjMast.fld_SyarikatID,
                            fld_WIlayahID = pkjMast.fld_WilayahID,
                            fld_LadangID = pkjMast.fld_LadangID,
                            fld_IsExist = false
                        });
                }
            }

            records.Content = tamatPermitPassportViewModelList.OrderBy(sort + " " + sortdir)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            records.TotalRecords = tamatPermitPassportViewModelList
                .Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.RoleID = role;
            ViewBag.pageSize = 1;
            ViewBag.Host = host;
            ViewBag.User = user;
            ViewBag.Catalog = catalog;
            ViewBag.Pass = pass;

            return View(records);
        }

        public ActionResult _PermitExpiryReasonInfoCreate(string id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var workerData = dbr.tbl_Pkjmast.SingleOrDefault(x =>
                x.fld_Nopkj == id && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

            tbl_TamatPermitPassportViewModelCreate tamatPermitPassportViewModelCreate = new tbl_TamatPermitPassportViewModelCreate();

            tamatPermitPassportViewModelCreate.fld_Nopkj = workerData.fld_Nopkj;
            tamatPermitPassportViewModelCreate.fld_TarikhTamat = workerData.fld_T2prmt;
            tamatPermitPassportViewModelCreate.fld_NegaraID = workerData.fld_NegaraID;
            tamatPermitPassportViewModelCreate.fld_SyarikatID = workerData.fld_SyarikatID;
            tamatPermitPassportViewModelCreate.fld_WIlayahID = workerData.fld_WilayahID;
            tamatPermitPassportViewModelCreate.fld_LadangID = workerData.fld_LadangID;

            ViewBag.Host = host;
            ViewBag.User = user;
            ViewBag.Catalog = catalog;
            ViewBag.Pass = pass;

            return PartialView(tamatPermitPassportViewModelCreate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _PermitExpiryReasonInfoCreate(tbl_TamatPermitPassportViewModelCreate tamatPermitPassportViewModelCreate)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                if (ModelState.IsValid)
                {
                    var permitExpiryCode = db.tblOptionConfigsWebs.SingleOrDefault(x =>
                        x.fldOptConfFlag1 == "permitExpryCode" && x.fld_NegaraID == NegaraID &&
                        x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).fldOptConfValue;

                    var permitExpiryData = dbr.tbl_TamatPermitPassport.SingleOrDefault(x =>
                        x.fld_Nopkj == tamatPermitPassportViewModelCreate.fld_Nopkj &&
                        DbFunctions.TruncateTime(x.fld_TarikhTamat) ==
                        DbFunctions.TruncateTime(tamatPermitPassportViewModelCreate.fld_TarikhTamat) &&
                        x.fld_KategoriSebab == permitExpiryCode &&
                        x.fld_NegaraID == NegaraID &&
                        x.fld_SyarikatID == SyarikatID && x.fld_WIlayahID == WilayahID && x.fld_LadangID == LadangID &&
                        x.fld_Deleted == false);

                    if (permitExpiryData != null)
                    {
                        permitExpiryData.fld_Deleted = true;
                    }

                    Models.tbl_TamatPermitPassport tamatPermitPassport = new Models.tbl_TamatPermitPassport();

                    PropertyCopy.Copy(tamatPermitPassport, tamatPermitPassportViewModelCreate);

                    tamatPermitPassport.fld_KategoriSebab = permitExpiryCode;
                    tamatPermitPassport.fld_NegaraID = NegaraID;
                    tamatPermitPassport.fld_SyarikatID = SyarikatID;
                    tamatPermitPassport.fld_WIlayahID = WilayahID;
                    tamatPermitPassport.fld_LadangID = LadangID;
                    tamatPermitPassport.fld_Deleted = false;

                    dbr.tbl_TamatPermitPassport.Add(tamatPermitPassport);
                    dbr.SaveChanges();

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
                        msg = GlobalResEstate.msgAdd,
                        status = "success",
                        checkingdata = "0",
                        method = "1",
                        div = "permitExpiryDetails",
                        rootUrl = domain,
                        action = "_PermitExpiryReasonInfo",
                        controller = "WorkerInfo"
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

        public ActionResult _PermitExpiryReasonInfoEdit(Guid id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var passportExpiryData = dbr.tbl_TamatPermitPassport
                .SingleOrDefault(x => x.fld_ReasonID == id && x.fld_Deleted == false);

            tbl_TamatPermitPassportViewModelEdit tamatPermitPassportViewModelEdit = new tbl_TamatPermitPassportViewModelEdit();

            PropertyCopy.Copy(tamatPermitPassportViewModelEdit, passportExpiryData);

            ViewBag.Host = host;
            ViewBag.User = user;
            ViewBag.Catalog = catalog;
            ViewBag.Pass = pass;

            return PartialView(tamatPermitPassportViewModelEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _PermitExpiryReasonInfoEdit(tbl_TamatPermitPassportViewModelEdit tamatPermitPassportViewModelEdit)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                if (ModelState.IsValid)
                {
                    var passportExpiryData = dbr.tbl_TamatPermitPassport
                        .SingleOrDefault(x => x.fld_ReasonID == tamatPermitPassportViewModelEdit.fld_ReasonID && x.fld_Deleted == false);

                    passportExpiryData.fld_SebabDesc = tamatPermitPassportViewModelEdit.fld_SebabDesc;

                    dbr.SaveChanges();

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
                        div = "permitExpiryDetails",
                        rootUrl = domain,
                        action = "_PermitExpiryReasonInfo",
                        controller = "WorkerInfo"
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

        public ActionResult _PermitExpiryReasonInfoDelete(Guid id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var permitExpiryData = dbr.tbl_TamatPermitPassport
                .SingleOrDefault(x => x.fld_ReasonID == id && x.fld_Deleted == false);

            ViewBag.Host = host;
            ViewBag.User = user;
            ViewBag.Catalog = catalog;
            ViewBag.Pass = pass;

            return PartialView(permitExpiryData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _PermitExpiryReasonInfoDelete(tbl_TamatPermitPassport tamatPermitPassport)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                var permitExpiryData = dbr.tbl_TamatPermitPassport
                    .SingleOrDefault(x => x.fld_ReasonID == tamatPermitPassport.fld_ReasonID && x.fld_Deleted == false);

                permitExpiryData.fld_Deleted = true;

                dbr.SaveChanges();

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
                    msg = GlobalResEstate.msgDelete2,
                    status = "success",
                    checkingdata = "0",
                    method = "1",
                    div = "permitExpiryDetails",
                    rootUrl = domain,
                    action = "_PermitExpiryReasonInfo",
                    controller = "WorkerInfo"
                });

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

        public ActionResult _YieldInfoDelete(Guid id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            var unitData = dbview.vw_HasilSawitPkt.SingleOrDefault(
                x => x.ID2 == id
                            //x.fld_NegaraID == NegaraID &&
                            //x.fld_SyarikatID == SyarikatID
                            );

            ViewingModels.vw_HasilSawitPkt unitViewModel = new ViewingModels.vw_HasilSawitPkt();

            PropertyCopy.Copy(unitViewModel, unitData);

            return View(unitViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _YieldInfoDelete(ViewingModels.vw_HasilSawitPkt optionConfigsWeb)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            MVC_SYSTEM_Models dbo = new MVC_SYSTEM_Models();

            try
            {
                var unitData = dbo.tbl_HasilSawitPkt.SingleOrDefault(
                        x => x.fld_ID == optionConfigsWeb.ID2 //&&
                        //     x.fld_Bulan == optionConfigsWeb.fld_Bulan &&
                        //     x.fld_Tahun == optionConfigsWeb.fld_Tahun

                             );

                //var unit = dbo.tbl_PktUtama.SingleOrDefault(
                //        x => x.fld_PktUtama == unitData.fld_KodPeringkat);

                bool status = true;

                var message = "";
                if (unitData.fld_Deleted == false)
                {
                    status = true;
                    message = GlobalResEstate.msgDelete2;
                }

                else
                {
                    status = false;
                    message = GlobalResEstate.msgUndelete;
                }

                unitData.fld_Deleted = status;

                dbo.SaveChanges();

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
                    method = "3",
                    div = "yieldList",
                    rootUrl = domain,
                    action = "_populatePktList",
                    controller = "WorkerInfo",
                    paramName = "MonthList",
                    paramValue = optionConfigsWeb.fld_Bulan,
                    paramName2 = "YearList",
                    paramValue2 = optionConfigsWeb.fld_Tahun
                });
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

        public ActionResult _YieldInfoEdit(Guid id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            var unitData = dbview.vw_HasilSawitPkt.SingleOrDefault(
                x => x.ID2 == id);

            ViewingModels.vw_HasilSawitPkt unitViewModel = new ViewingModels.vw_HasilSawitPkt();

            PropertyCopy.Copy(unitViewModel, unitData);

            return View(unitViewModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _YieldInfoEdit(ViewingModels.vw_HasilSawitPkt optionConfigsWeb)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Models dbo = new MVC_SYSTEM_Models();

            try
            {
                if (ModelState.IsValid)
                {

                    var pktData = dbo.tbl_PktUtama
                        .Where(x => x.fld_PktUtama == optionConfigsWeb.fld_PktUtama);

                    var checkPktYieldRecord = dbo.tbl_HasilSawitPkt
                        .Where(x => x.fld_KodPeringkat == optionConfigsWeb.fld_PktUtama &&
                                    x.fld_Tahun == optionConfigsWeb.fld_Tahun &&
                                    x.fld_Bulan == optionConfigsWeb.fld_Bulan && 
                                    x.fld_NegaraID == NegaraID &&
                                    x.fld_SyarikatID == SyarikatID && 
                                    x.fld_WilayahID == WilayahID &&
                                    x.fld_LadangID == LadangID && 
                                    x.fld_Deleted == false)
                            .ToList();

                        if (checkPktYieldRecord.Count == 0)
                        {
                            Models.tbl_HasilSawitPkt HasilSawitPkt = new Models.tbl_HasilSawitPkt();

                            HasilSawitPkt.fld_KodPeringkat = optionConfigsWeb.fld_PktUtama;
                            HasilSawitPkt.fld_HasilTan = optionConfigsWeb.fld_HasilTan;
                            HasilSawitPkt.fld_LuasHektar = pktData.Select(s => s.fld_LsPktUtama).Single();
                            HasilSawitPkt.fld_Tahun = optionConfigsWeb.fld_Tahun;
                            HasilSawitPkt.fld_Bulan = optionConfigsWeb.fld_Bulan;
                            HasilSawitPkt.fld_NegaraID = NegaraID;
                            HasilSawitPkt.fld_SyarikatID = SyarikatID;
                            HasilSawitPkt.fld_WilayahID = WilayahID;
                            HasilSawitPkt.fld_LadangID = LadangID;
                            HasilSawitPkt.fld_Deleted = false;
                            HasilSawitPkt.fld_YieldType = "EST";
                            HasilSawitPkt.fld_CreatedBy = getuserid;
                            HasilSawitPkt.fld_CreatedDT = timezone.gettimezone();

                            dbo.tbl_HasilSawitPkt.Add(HasilSawitPkt);
                        }

                        else
                        {
                            var materializePktYieldRec = checkPktYieldRecord.Single();

                            materializePktYieldRec.fld_HasilTan = optionConfigsWeb.fld_HasilTan;

                            dbo.Entry(materializePktYieldRec).State = EntityState.Modified;
                        }

                            dbo.SaveChanges();

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
                        method = "3",
                        div = "yieldList",
                        rootUrl = domain,
                        action = "_populatePktList",
                        controller = "WorkerInfo",
                        paramName = "MonthList",
                        paramValue = optionConfigsWeb.fld_Bulan,
                        paramName2 = "YearList",
                        paramValue2 = optionConfigsWeb.fld_Tahun
                    });
                }
                //            var unitData = dbview.vw_HasilSawitPkt.SingleOrDefault(
                //                x => x.fld_PktUtama == optionConfigsWeb.fld_PktUtama &&
                //                     x.fld_Bulan == optionConfigsWeb.fld_Bulan &&
                //                     x.fld_Tahun == optionConfigsWeb.fld_Tahun);

                //            optionConfigsWeb.fld_HasilTan = optionConfigsWeb.fld_HasilTan;

                //            dbview.SaveChanges();

                //            string appname = Request.ApplicationPath;
                //            string domain = Request.Url.GetLeftPart(UriPartial.Authority);
                //            var lang = Request.RequestContext.RouteData.Values["lang"];

                //            if (appname != "/")
                //            {
                //                domain = domain + appname;
                //            }

                //            return Json(new
                //            {
                //                success = true,
                //                msg = GlobalResEstate.msgUpdate,
                //                status = "success",
                //                checkingdata = "0",
                //                method = "1",
                //                div = "yieldList",
                //                rootUrl = domain,
                //                action = "_populatePktList",
                //                controller = "WorkerInfo"
                //            });
                //        }


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

        public ActionResult _YieldInfoCreate()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int drpyear = 0;
            int drprangeyear = 0;
            int month = timezone.gettimezone().Month;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> jenis = new List<SelectListItem>();
            List<SelectListItem> bulan = new List<SelectListItem>();

            ViewBag.WorkerInfo = "class = active";

            jenis = new SelectList(
                dbr.tbl_PktUtama
                    //.Where(x => x.fldOptConfFlag1 == "jnspkt" && x.fldDeleted == false &&
                    //                               x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fld_ID)
                    .Select(s => new SelectListItem { Value = s.fld_PktUtama, Text = s.fld_PktUtama}), "Value", "Text").ToList();
            jenis.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "" }));

            bulan = new SelectList(
                db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false &&
                                                   x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " - " + s.fldOptConfFlag2 }), "Value", "Text").ToList();
            bulan.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "" }));

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == timezone.gettimezone().Year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }
            ViewBag.MonthList = new SelectList(
                db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false &&
                                                   x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID),
                "fldOptConfValue", "fldOptConfDesc", month);

            ViewBag.YearList = yearlist;
            ViewBag.JenisPeringkatList = jenis;
            ViewBag.MonthList = bulan;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _YieldInfoCreate(ViewingModels.vw_HasilSawitPktCreate optionConfigsWeb)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            var pktData = dbr.tbl_PktUtama
                .Where(x => x.fld_PktUtama == optionConfigsWeb.fld_PktUtama);

            //if (String.IsNullOrEmpty(fld_PktUtama) || String.IsNullOrEmpty(hasilTan.ToString()))
            //{
            //    return Json(new
            //    {
            //        success = true,
            //        msg = GlobalResEstate.msgErrorData,
            //        status = "danger",
            //        checkingdata = "1"
            //    });
            //}

            //else
            //{
            var checkPktYieldRecord = dbr.tbl_HasilSawitPkt
                .Where(x => x.fld_KodPeringkat == optionConfigsWeb.fld_PktUtama &&
                            x.fld_Tahun == optionConfigsWeb.fld_Tahun &&
                            x.fld_Bulan == optionConfigsWeb.fld_Bulan &&
                            x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID &&
                            x.fld_LadangID == LadangID &&
                            x.fld_Deleted == false &&
                            x.fld_YieldType == "EST")
                .ToList();

            if (checkPktYieldRecord.Count == 0)
            {
                Models.tbl_HasilSawitPkt HasilSawitPkt = new Models.tbl_HasilSawitPkt();

                HasilSawitPkt.fld_KodPeringkat = optionConfigsWeb.fld_PktUtama;
                HasilSawitPkt.fld_HasilTan = optionConfigsWeb.fld_HasilTan;
                HasilSawitPkt.fld_LuasHektar = pktData.Select(s => s.fld_LsPktUtama).Single();
                HasilSawitPkt.fld_Tahun = optionConfigsWeb.fld_Tahun;
                HasilSawitPkt.fld_Bulan = optionConfigsWeb.fld_Bulan;
                HasilSawitPkt.fld_NegaraID = NegaraID;
                HasilSawitPkt.fld_SyarikatID = SyarikatID;
                HasilSawitPkt.fld_WilayahID = WilayahID;
                HasilSawitPkt.fld_LadangID = LadangID;
                HasilSawitPkt.fld_Deleted = false;
                HasilSawitPkt.fld_YieldType = "EST";
                HasilSawitPkt.fld_CreatedBy = getuserid;
                HasilSawitPkt.fld_CreatedDT = timezone.gettimezone();

                dbr.tbl_HasilSawitPkt.Add(HasilSawitPkt);
            }

            else
            {
                var materializePktYieldRec = checkPktYieldRecord.Single();

                materializePktYieldRec.fld_HasilTan = optionConfigsWeb.fld_HasilTan;

                dbr.Entry(materializePktYieldRec).State = EntityState.Modified;
            }

            dbr.SaveChanges();

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
                method = "3",
                div = "yieldList",
                rootUrl = domain,
                action = "_populatePktList",
                controller = "WorkerInfo",
                paramName = "MonthList",
                paramValue = optionConfigsWeb.fld_Bulan,
                paramName2 = "YearList",
                paramValue2 = optionConfigsWeb.fld_Tahun
            });
            //}
        }

        public ActionResult _ProductivitiInfoDelete(string id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            var unitData = dbview.tbl_Produktiviti.SingleOrDefault(
                x => x.fld_Nopkj == id && x.fld_Deleted == false
                            //x.fld_NegaraID == NegaraID &&
                            //x.fld_SyarikatID == SyarikatID
                            );

            ViewingModels.tbl_Produktiviti unitViewModel = new ViewingModels.tbl_Produktiviti();

            PropertyCopy.Copy(unitViewModel, unitData);

            ViewBag.host = host;
            ViewBag.catalog = catalog;
            ViewBag.user = user;
            ViewBag.pass = pass;

            return View(unitViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _ProductivitiInfoDelete(ViewingModels.tbl_Produktiviti optionConfigsWeb)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            MVC_SYSTEM_Models dbo = new MVC_SYSTEM_Models();

            try
            {
                var unitData = dbo.tbl_Produktiviti.SingleOrDefault(
                        x => x.fld_Nopkj == optionConfigsWeb.fld_Nopkj && x.fld_Deleted == false
                                                                       //     x.fld_Bulan == optionConfigsWeb.fld_Bulan &&
                                                                       //     x.fld_Tahun == optionConfigsWeb.fld_Tahun

                             );

                //var unit = dbo.tbl_PktUtama.SingleOrDefault(
                //        x => x.fld_PktUtama == unitData.fld_KodPeringkat);

                bool status = true;

                var message = "";
                if (unitData.fld_Deleted == false)
                {
                    status = true;
                    message = GlobalResEstate.msgDelete2;
                }

                else
                {
                    status = false;
                    message = GlobalResEstate.msgUndelete;
                }

                unitData.fld_Deleted = status;

                dbo.SaveChanges();

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
                    div = "searchResultProductivityInfo",
                    rootUrl = domain,
                    action = "_WorkerProductivitySearch",
                    controller = "WorkerInfo"
                    //paramName = "MonthList",
                    //paramValue = optionConfigsWeb.fld_Bulan,
                    //paramName2 = "YearList",
                    //paramValue2 = optionConfigsWeb.fld_Tahun
                });
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

        public ActionResult DependentsInfo(string PkjList = "0", int page = 1, string sort = "fld_Nama", string sortdir = "ASC")
        {
            ViewBag.WorkerInfo = "class = active";
            GetIdentity GetIdentity = new GetIdentity();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> pkj2 = new List<SelectListItem>();

            pkj2 = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o => o.fld_Nama).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + " - " + s.fld_Nama }).Distinct(), "Value", "Text").ToList();
            pkj2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            List<vw_MaklumatKeluarga> MaklumatKeluarga = new List<vw_MaklumatKeluarga>();
            var PkjAll = dbview.tbl_Pkjmast.Where(x => x.fld_StatusApproved==1 && x.fld_Kdaktf=="1" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o => o.fld_Nama).ToList();
            if (PkjList != "0")
            {
                PkjAll = dbview.tbl_Pkjmast.Where(x =>x.fld_Nopkj==PkjList && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o => o.fld_Nama).ToList();
            }
            foreach (var i in PkjAll)
            {
                var Keluarga = dbview.tbl_KeluargaPkj.Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
                MaklumatKeluarga.Add(new vw_MaklumatKeluarga { MklmtPkj = i, MklmtKeluarga = Keluarga });
            }

            ViewBag.pageSize = int.Parse(GetConfig.GetData("paging"));
            ViewBag.PkjList = pkj2;
            return View(MaklumatKeluarga);
        }

        public ActionResult DependentsInfoCreate(string PkjId)
        {
            GetStatus GetStatus = new GetStatus();
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var usednopkj = dbr.tbl_KeluargaPkj.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_Nopkj.Trim());
            var nopkj = new SelectList(dbr.tbl_Pkjmast.Where(x =>x.fld_Nopkj==PkjId && x.fld_Kdaktf == "1" && x.fld_StatusApproved == 1 && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + " - " + s.fld_Nama }), "Value", "Text").ToList();
            //nopkj.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            var hubungan = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "Waris" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            hubungan.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            ViewBag.fld_Nopkj = nopkj;
            ViewBag.fld_Hubungan = hubungan;
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DependentsInfoCreate(Models.tbl_KeluargaPkj KeluargaPkj)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
           
            var checkdata = dbr.tbl_KeluargaPkj.Where(x => x.fld_Nopkj == KeluargaPkj.fld_Nopkj && x.fld_NamaKeluarga==KeluargaPkj.fld_NamaKeluarga && x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).FirstOrDefault();
            if (checkdata == null)
            {
                try
                {
                    KeluargaPkj.fld_NamaKeluarga = KeluargaPkj.fld_NamaKeluarga.ToUpper();
                    KeluargaPkj.fld_Permit = KeluargaPkj.fld_Permit.ToUpper();
                    KeluargaPkj.fld_Pspt = KeluargaPkj.fld_Pspt.ToUpper();
                    KeluargaPkj.fld_NegaraID = NegaraID;
                    KeluargaPkj.fld_SyarikatID = SyarikatID;
                    KeluargaPkj.fld_WilayahID = WilayahID;
                    KeluargaPkj.fld_LadangID = LadangID;
                    KeluargaPkj.fld_Deleted = false;
                    dbr.tbl_KeluargaPkj.Add(KeluargaPkj);
                    dbr.SaveChanges();
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                }

                return Json(new { success = true, msg = GlobalResEstate.msgAdd, status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
            }
            else
            {
                return Json(new { success = true, msg = "Data already exist.", status = "warning", checkingdata = "1" });
            }
        }

        public ActionResult DependentsInfoUpdate(int id)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var keluarga = dbr.tbl_KeluargaPkj.Where(x => x.fld_ID == id && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
            string nopkj = keluarga.fld_Nopkj;
            var pkj = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_Kdaktf == "1" && x.fld_StatusApproved == 1 && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + " - " + s.fld_Nama }), "Value", "Text", nopkj).ToList();
            
            var hubungan = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "Waris" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text",keluarga.fld_Hubungan).ToList();

            var result = dbr.tbl_KeluargaPkj.Where(x => x.fld_ID == id && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();

            ViewBag.fld_Nopkj = pkj;
            ViewBag.fld_Hubungan = hubungan;
            return PartialView(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DependentsInfoUpdate(Models.tbl_KeluargaPkj KeluargaPkj)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int IDkeluarga = Convert.ToInt32(Request.Form["IDkeluarga"]);
            var checkdata = dbr.tbl_KeluargaPkj.Where(x => x.fld_ID == IDkeluarga && x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).FirstOrDefault();
            if (checkdata != null)
            {
                try
                {
                    checkdata.fld_NamaKeluarga = KeluargaPkj.fld_NamaKeluarga.ToUpper();
                    checkdata.fld_Hubungan = KeluargaPkj.fld_Hubungan;
                    checkdata.fld_NoTel = KeluargaPkj.fld_NoTel;
                    checkdata.fld_TLahir = KeluargaPkj.fld_TLahir;
                    checkdata.fld_Permit = KeluargaPkj.fld_Permit.ToUpper();
                    checkdata.fld_PermitExprd = KeluargaPkj.fld_PermitExprd;
                    checkdata.fld_Pspt = KeluargaPkj.fld_Pspt.ToUpper();
                    checkdata.fld_PsptExprd = KeluargaPkj.fld_PsptExprd;
                    checkdata.fld_InsuransExprd = KeluargaPkj.fld_InsuransExprd;
                    checkdata.fld_Deleted = false;
                    dbr.Entry(checkdata).State = EntityState.Modified;
                    dbr.SaveChanges();
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                }

                return Json(new { success = true, msg = GlobalResEstate.msgAdd, status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
            }
            else
            {
                return Json(new { success = true, msg = "Data already exist.", status = "warning", checkingdata = "1" });
            }
        }

        public ActionResult DependentsInfoDelete(int id)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            Models.tbl_KeluargaPkj tbl_KeluargaPkj = dbr.tbl_KeluargaPkj.Where(x => x.fld_ID == id && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
            if (tbl_KeluargaPkj == null)
            {
                return RedirectToAction("DependentsInfo");
            }
            return PartialView("DependentsInfoDelete", tbl_KeluargaPkj);
            //return PartialView();
        }

        [HttpPost, ActionName("DependentsInfoDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult DependentsInfoDeleteConfirmed(int IDkeluarga)
        {
            try
            {
                int? getuserid = GetIdentity.ID(User.Identity.Name);
                int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
                string host, catalog, user, pass = "";
                GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
                Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
                MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

                Models.tbl_KeluargaPkj MaklumatWaris = dbr.tbl_KeluargaPkj.Where(x => x.fld_ID == IDkeluarga && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
                if (MaklumatWaris == null)
                {
                    return Json(new { success = true, msg = "Data already deleted.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
                }
                else
                {
                    dbr.tbl_KeluargaPkj.Remove(MaklumatWaris);
                    dbr.SaveChanges();
                    return Json(new { success = true, msg = GlobalResEstate.msgDelete2, status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
                }

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = true, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
            //return PartialView();
        }

        public JsonResult GetAnk(string nopkj, string isteri, int anak)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            string Nama = dbr.tbl_MklmtKeluargaPkj.Where(x => x.fld_Nopkj == nopkj && x.fld_Flag.Trim().StartsWith("Anak" + anak) && x.fld_Flag.Contains(isteri)).Select(s => s.fld_NamaKeluarga.Trim()).FirstOrDefault();
            string Umur = dbr.tbl_MklmtKeluargaPkj.Where(x => x.fld_Nopkj == nopkj && x.fld_Flag.Trim().StartsWith("Anak" + anak) && x.fld_Flag.Contains(isteri)).Select(s => s.fld_Umur.ToString()).FirstOrDefault();

            return Json(new { nama = Nama, umur = Umur });
        }

        public ActionResult WorkerDebtInfo()
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            ViewBag.WorkerInfo = "class = active";

            return View();
        }

        public ActionResult _WorkerDebtInfo(string filter)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<tbl_HutangPekerjaDetailsList> hutangPekerjaList = new List<tbl_HutangPekerjaDetailsList>();

            ViewBag.pageSize = int.Parse(GetConfig.GetData("paging"));

            if (!String.IsNullOrEmpty(filter))
            {
                var workerData = dbr.tbl_Pkjmast
                    .Where(x => x.fld_Kdaktf == "1" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Where(x =>
                                x.fld_Nopkj.ToString().ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_Nama.ToUpper().Contains(filter.ToUpper()))
                    .OrderBy(x => x.fld_Nama);

                foreach (var worker in workerData)
                {
                    var workerDebtData = dbr.tbl_HutangPekerja
                        .Where(a => a.fld_NoPkj == worker.fld_Nopkj && a.fld_NegaraID == NegaraID &&
                                    a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                    a.fld_LadangID == LadangID && a.fld_Deleted == false)
                        .OrderBy(x => x.fld_KodHutang)
                        .ToList();
                    hutangPekerjaList.Add(new tbl_HutangPekerjaDetailsList { Pkjmast = worker, HutangPekerja = workerDebtData });
                }
            }

            else
            {
                var workerData = dbr.tbl_Pkjmast
                    .Where(x => x.fld_Kdaktf == "1" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                    .OrderBy(x => x.fld_Nama);

                foreach (var worker in workerData)
                {
                    var workerDebtData = dbr.tbl_HutangPekerja
                        .Where(a => a.fld_NoPkj == worker.fld_Nopkj && a.fld_NegaraID == NegaraID &&
                                    a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                    a.fld_LadangID == LadangID && a.fld_Deleted == false)
                        .OrderBy(x => x.fld_KodHutang)
                        .ToList();
                    hutangPekerjaList.Add(new tbl_HutangPekerjaDetailsList { Pkjmast = worker, HutangPekerja = workerDebtData });
                }
            }

            if (hutangPekerjaList.Count == 0)
            {
                ViewBag.Message = GlobalResEstate.msgErrorSearch;
            }

            return View(hutangPekerjaList);
        }

        public ActionResult _WorkerDebtInfoCreate(string id)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var workerDebtData = dbr.tbl_HutangPekerja.Where(x =>
                    x.fld_NoPkj == id && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false)
                .Select(s => s.fld_KodHutang).ToList();

            //var estatePublicHolidayData = db.tbl_CutiUmumLdg
            //    .Where(x => tblCutiUmumLdgViewModelCreateHq.fld_LadangID.ToList()
            //                    .Contains((int)x.fld_LadangID) && tblCutiUmumLdgViewModelCreateHq
            //                    .fld_CutiMasterID.ToList()
            //                    .Contains((int)x.fld_CutiMasterID) &&
            //                x.fld_NegaraID == NegaraID &&
            //                x.fld_SyarikatID == SyarikatID
            //                && x.fld_Year == tblCutiUmumLdgViewModelCreateHq.fld_Year);

            List<SelectListItem> workerDebtList = new List<SelectListItem>();

            workerDebtList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => !workerDebtData.Contains(x.fldOptConfValue) && x.fldOptConfFlag1 == "hutangPekerja" && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(
                        s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();
            workerDebtList.Insert(0, new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "" });

            ViewBag.WorkerDebtList = workerDebtList;

            Models.tbl_HutangPekerjaViewModelCreate hutangPekerjaViewModelCreate = new Models.tbl_HutangPekerjaViewModelCreate();

            hutangPekerjaViewModelCreate.fld_NoPkj = id;
            hutangPekerjaViewModelCreate.fld_NegaraID = NegaraID;
            hutangPekerjaViewModelCreate.fld_SyarikatID = SyarikatID;
            hutangPekerjaViewModelCreate.fld_WilayahID = WilayahID;
            hutangPekerjaViewModelCreate.fld_LadangID = LadangID;

            @ViewBag.Host = host;
            @ViewBag.User = user;
            @ViewBag.Catalog = catalog;
            @ViewBag.Pass = pass;

            return PartialView(hutangPekerjaViewModelCreate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _WorkerDebtInfoCreate(Models.tbl_HutangPekerjaViewModelCreate hutangPekerjaViewModelCreate)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                if (ModelState.IsValid)
                {
                    Models.tbl_HutangPekerja hutangPekerja = new tbl_HutangPekerja();

                    PropertyCopy.Copy(hutangPekerja, hutangPekerjaViewModelCreate);

                    hutangPekerja.fld_CreatedBy = getuserid;
                    hutangPekerja.fld_CreatedDT = timezone.gettimezone();
                    hutangPekerja.fld_Deleted = false;

                    var workerTotalDebtData = dbr.tbl_HutangPekerjaJumlah.SingleOrDefault(x =>
                        x.fld_NoPkj == hutangPekerja.fld_NoPkj && x.fld_NegaraID == NegaraID &&
                        x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                        x.fld_LadangID == LadangID && x.fld_Deleted == false);

                    if (workerTotalDebtData != null)
                    {
                        workerTotalDebtData.fld_JumlahHutang =
                            workerTotalDebtData.fld_JumlahHutang + hutangPekerja.fld_NilaiHutang;
                    }

                    else
                    {
                        Models.tbl_HutangPekerjaJumlah hutangPekerjaJumlah = new Models.tbl_HutangPekerjaJumlah();

                        hutangPekerjaJumlah.fld_NoPkj = hutangPekerja.fld_NoPkj;
                        hutangPekerjaJumlah.fld_JumlahHutang = hutangPekerja.fld_NilaiHutang;
                        hutangPekerjaJumlah.fld_JumlahBayar = 0;
                        hutangPekerjaJumlah.fld_NegaraID = NegaraID;
                        hutangPekerjaJumlah.fld_SyarikatID = SyarikatID;
                        hutangPekerjaJumlah.fld_WilayahID = WilayahID;
                        hutangPekerjaJumlah.fld_LadangID = LadangID;
                        hutangPekerjaJumlah.fld_Deleted = false;
                        hutangPekerjaJumlah.fld_CreatedBy = getuserid;
                        hutangPekerjaJumlah.fld_CreatedDT = timezone.gettimezone();

                        dbr.tbl_HutangPekerjaJumlah.Add(hutangPekerjaJumlah);
                    }

                    hutangPekerja.fld_StatusTambah = true;

                    dbr.tbl_HutangPekerja.Add(hutangPekerja);
                    dbr.SaveChanges();

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
                        msg = GlobalResEstate.msgAdd,
                        status = "success",
                        checkingdata = "0",
                        method = "1",
                        div = "workerDebtInfoDetails",
                        rootUrl = domain,
                        action = "_WorkerDebtInfo",
                        controller = "WorkerInfo"
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

        public ActionResult _WorkerDebtInfoEdit(Guid id)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var workerDebtData = dbr.tbl_HutangPekerja
                .SingleOrDefault(x => x.fld_HutangPkjID == id && x.fld_NegaraID == NegaraID &&
                                      x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                      x.fld_SyarikatID == SyarikatID);

            Models.tbl_HutangPekerjaViewModelEdit hutangPekerjaViewModelEdit = new tbl_HutangPekerjaViewModelEdit();

            PropertyCopy.Copy(hutangPekerjaViewModelEdit, workerDebtData);

            @ViewBag.Host = host;
            @ViewBag.User = user;
            @ViewBag.Catalog = catalog;
            @ViewBag.Pass = pass;

            return PartialView(hutangPekerjaViewModelEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _WorkerDebtInfoEdit(Models.tbl_HutangPekerjaViewModelEdit hutangPekerjaViewModelEdit)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                if (ModelState.IsValid)
                {
                    var workerDebtData = dbr.tbl_HutangPekerja.SingleOrDefault(
                        x => x.fld_HutangPkjID == hutangPekerjaViewModelEdit.fld_HutangPkjID &&
                             x.fld_NegaraID == NegaraID &&
                             x.fld_SyarikatID == SyarikatID);

                    var workerTotalDebtData = dbr.tbl_HutangPekerjaJumlah.SingleOrDefault(x =>
                        x.fld_NoPkj == hutangPekerjaViewModelEdit.fld_NoPkj && x.fld_NegaraID == NegaraID &&
                        x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                        x.fld_LadangID == LadangID && x.fld_Deleted == false);

                    if (workerTotalDebtData != null)
                    {
                        workerTotalDebtData.fld_JumlahHutang =
                            workerTotalDebtData.fld_JumlahHutang - workerDebtData.fld_NilaiHutang;

                        workerTotalDebtData.fld_JumlahHutang =
                            workerTotalDebtData.fld_JumlahHutang + hutangPekerjaViewModelEdit.fld_NilaiHutang;
                    }

                    workerDebtData.fld_NilaiHutang = hutangPekerjaViewModelEdit.fld_NilaiHutang;

                    dbr.SaveChanges();

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
                        div = "workerDebtInfoDetails",
                        rootUrl = domain,
                        action = "_WorkerDebtInfo",
                        controller = "WorkerInfo"
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

        public ActionResult _WorkerDebtInfoDelete(Guid id)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var workerDebtData = dbr.tbl_HutangPekerja
                .SingleOrDefault(x => x.fld_HutangPkjID == id && x.fld_NegaraID == NegaraID &&
                                      x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                      x.fld_SyarikatID == SyarikatID);

            @ViewBag.Host = host;
            @ViewBag.User = user;
            @ViewBag.Catalog = catalog;
            @ViewBag.Pass = pass;

            return PartialView(workerDebtData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _WorkerDebtInfoDelete(Models.tbl_HutangPekerja hutangPekerja)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                var workerDebtData = dbr.tbl_HutangPekerja
                    .SingleOrDefault(x => x.fld_HutangPkjID == hutangPekerja.fld_HutangPkjID && x.fld_NegaraID == NegaraID &&
                                          x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                          x.fld_SyarikatID == SyarikatID);

                workerDebtData.fld_Deleted = true;
                workerDebtData.fld_StatusTambah = false;

                var workerTotalDebtData = dbr.tbl_HutangPekerjaJumlah.SingleOrDefault(x =>
                    x.fld_NoPkj == hutangPekerja.fld_NoPkj && x.fld_NegaraID == NegaraID &&
                    x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                    x.fld_LadangID == LadangID && x.fld_Deleted == false);

                if (workerTotalDebtData != null)
                {
                    workerTotalDebtData.fld_JumlahHutang =
                        workerTotalDebtData.fld_JumlahHutang - workerDebtData.fld_NilaiHutang;
                }

                dbr.SaveChanges();

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
                    msg = GlobalResEstate.msgDelete2,
                    status = "success",
                    checkingdata = "0",
                    method = "1",
                    div = "workerDebtInfoDetails",
                    rootUrl = domain,
                    action = "_WorkerDebtInfo",
                    controller = "WorkerInfo"
                });
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

        public ActionResult AddedContributionManagement()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var GetAddedContributionList = db.tbl_CarumanTambahan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).ToList();

            List<SelectListItem> ContribustionList = new List<SelectListItem>();

            ContribustionList = new SelectList(GetAddedContributionList.OrderBy(o => o.fld_NamaCaruman).Select(s => new SelectListItem { Value = s.fld_KodCaruman, Text = s.fld_KodCaruman + " - " + s.fld_NamaCaruman }).Distinct(), "Value", "Text").ToList();

            ViewBag.AddedContribution = ContribustionList;
            ViewBag.WorkerInfo = "class = active";
            return View();
        }

        public ViewResult _AddedContributionManagement(string AddedContribution, string Filter, int page = 1, string sort = "fld_Nopkj",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            var GetAddConWorker = dbr.vw_PkjCarumanTambahan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID && x.fld_KodCaruman == AddedContribution && (x.fld_Nama.Contains(Filter) || x.fld_Nopkj .Contains(Filter))).OrderBy(o => o.fld_Nopkj).ToList();

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<vw_PkjCarumanTambahan>(); ;

            records.Content = GetAddConWorker
            .OrderBy(sort + " " + sortdir)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

            records.TotalRecords = GetAddConWorker.Count();

            dbr.Dispose();
            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.pageSize = pageSize;
            return View(records);
        }

        public ViewResult _AddedContributionDeactive(Guid ID)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var GetDetailWorkerAddContri = dbr.vw_PkjCarumanTambahan.Find(ID);

            return View(GetDetailWorkerAddContri);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _AddedContributionDeactive(vw_PkjCarumanTambahan vw_PkjCarumanTambahan)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            
            string appname = Request.ApplicationPath;
            string domain = Request.Url.GetLeftPart(UriPartial.Authority);
            var lang = Request.RequestContext.RouteData.Values["lang"];
            bool success = true;
            string msg = "";
            string status = "";
            string AddedContribution = "";

            if (appname != "/")
            {
                domain = domain + appname;
            }

            try
            {
                var GetDetailWorkerAddContri = dbr.tbl_PkjCarumanTambahan.Find(vw_PkjCarumanTambahan.fld_ID);

                GetDetailWorkerAddContri.fld_Deleted = true;

                dbr.Entry(GetDetailWorkerAddContri).State = EntityState.Modified;
                dbr.SaveChanges();

                AddedContribution = GetDetailWorkerAddContri.fld_KodCaruman;
                success = true;
                status = "success";
                msg = "Berjaya Dikemaskini";

            }
            catch (Exception ex)
            {
                success = false;
                status = "warning";
                msg = GlobalResEstate.msgError;
            }

            return Json(new
            {
                success,
                msg,
                status,
                checkingdata = "0",
                method = "2",
                div = "SearchResult",
                rootUrl = domain,
                paramName = "AddedContribution",
                paramValue = AddedContribution,
                action = "_AddedContributionManagement",
                controller = "WorkerInfo"
            });
        }

        public ViewResult _AddedContributionActive(Guid ID)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var GetDetailWorkerAddContri = dbr.vw_PkjCarumanTambahan.Find(ID);

            return View(GetDetailWorkerAddContri);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _AddedContributionActive(vw_PkjCarumanTambahan vw_PkjCarumanTambahan)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            string appname = Request.ApplicationPath;
            string domain = Request.Url.GetLeftPart(UriPartial.Authority);
            var lang = Request.RequestContext.RouteData.Values["lang"];
            bool success = true;
            string msg = "";
            string status = "";
            string AddedContribution = "";

            if (appname != "/")
            {
                domain = domain + appname;
            }

            try
            {
                var GetDetailWorkerAddContri = dbr.tbl_PkjCarumanTambahan.Find(vw_PkjCarumanTambahan.fld_ID);

                GetDetailWorkerAddContri.fld_Deleted = false;

                dbr.Entry(GetDetailWorkerAddContri).State = EntityState.Modified;
                dbr.SaveChanges();

                AddedContribution = GetDetailWorkerAddContri.fld_KodCaruman;
                success = true;
                status = "success";
                msg = "Berjaya Dikemaskini";

            }
            catch (Exception ex)
            {
                success = false;
                status = "warning";
                msg = GlobalResEstate.msgError;
            }

            return Json(new
            {
                success,
                msg,
                status,
                checkingdata = "0",
                method = "2",
                div = "SearchResult",
                rootUrl = domain,
                paramName = "AddedContribution",
                paramValue = AddedContribution,
                action = "_AddedContributionManagement",
                controller = "WorkerInfo"
            });
        }

        public ActionResult WorkerCostCenter()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            List<SelectListItem> status = new List<SelectListItem>();

            
            status.Insert(0, (new SelectListItem { Text = "Not Registered", Value = "0" }));
            status.Insert(0, (new SelectListItem { Text = "Registered", Value = "1" }));

            ViewBag.WorkerInfo = "class = active";
            ViewBag.StatusList = status;
            return View();
        }

        public ActionResult _WorkerCostCenter(string StatusList = "", int page = 1, string sort = "fld_Nama", string sortdir = "ASC", string filter = "")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            //MVC_SYSTEM_Models dbo = new MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<Models.tbl_Pkjmast>();
            //int role = GetIdentity.RoleID(getuserid).Value;

            var message = "";

            if (String.IsNullOrEmpty(StatusList))
            {
                message = GlobalResEstate.lblChooseAcc;
            }

            else
            {
                message = GlobalResEstate.msgErrorSearch;
            }

            if (!string.IsNullOrEmpty(filter))
            {
                records.Content = dbr.tbl_Pkjmast
                    .Where(x => x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && (x.fld_Nopkj.Contains(filter) || x.fld_Nama.Contains(filter)))
                           .OrderBy(sort + " " + sortdir)
                           .Skip((page - 1) * pageSize)
                           .Take(pageSize)
                           .ToList();

                records.TotalRecords = dbr.tbl_Pkjmast.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && (x.fld_Nopkj.Contains(filter) || x.fld_Nama.Contains(filter))).Count();
                records.CurrentPage = page;
                records.PageSize = pageSize;
                db.Dispose();
                dbr.Dispose();
            }
            else
            {
                if (StatusList == "0" && string.IsNullOrEmpty(filter))
                {
                    records.Content = dbr.tbl_Pkjmast.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && string.IsNullOrEmpty(x.fld_KodSAPPekerja))
                           .OrderBy(sort + " " + sortdir)
                           .Skip((page - 1) * pageSize)
                           .Take(pageSize)
                           .ToList();

                    records.TotalRecords = dbr.tbl_Pkjmast.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && string.IsNullOrEmpty(x.fld_KodSAPPekerja)).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                    //ViewBag.AktvtList = AktvtList;
                    db.Dispose();
                    dbr.Dispose();
                }
                else
                {
                    records.Content = dbr.tbl_Pkjmast
                    .Where(x => x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && !string.IsNullOrEmpty(x.fld_KodSAPPekerja))
                           .OrderBy(sort + " " + sortdir)
                           .Skip((page - 1) * pageSize)
                           .Take(pageSize)
                           .ToList();

                    records.TotalRecords = dbr.tbl_Pkjmast.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && !string.IsNullOrEmpty(x.fld_KodSAPPekerja)).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                    db.Dispose();
                    dbr.Dispose();
                }
            }
           

            ViewBag.Datacount = records.TotalRecords;
            ViewBag.Message = message;
            return View(records);
        }

        public ActionResult _CostCenterRegister(string id)
        {
            GetStatus GetStatus = new GetStatus();
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            
            var tbl_Pkjmast = dbr.tbl_Pkjmast.Where(w => w.fld_Nopkj == id && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID && w.fld_WilayahID == WilayahID && w.fld_LadangID == LadangID).FirstOrDefault();
            
            tbl_PkjmastEditKwsp edit = new tbl_PkjmastEditKwsp();

            List<SelectListItem> CostCenter = new List<SelectListItem>();

            CostCenter = new SelectList(db.tbl_SAPCCPUP.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_CostCenter).Select(s => new SelectListItem { Value = s.fld_CostCenter, Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc }), "Value", "Text", tbl_Pkjmast.fld_KodSAPPekerja).ToList();

            ViewBag.CostCenter = CostCenter;
            PropertyCopy.Copy(edit, tbl_Pkjmast);

            return View(edit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _CostCenterRegister(Models.tbl_PkjmastEditKwsp tbl_Pkjmast, string CostCenter, string CostCenterText)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                var getdata = dbr.tbl_Pkjmast.Where(w => w.fld_Nopkj == tbl_Pkjmast.fld_Nopkj &&
                w.fld_LadangID == LadangID && w.fld_WilayahID == WilayahID &&
                w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID).FirstOrDefault();

                if (string.IsNullOrEmpty(CostCenterText))
                {
                    getdata.fld_KodSAPPekerja = CostCenter.PadLeft(10, '0');
                }
                else
                {
                    getdata.fld_KodSAPPekerja = CostCenterText.PadLeft(10, '0');
                }

                dbr.SaveChanges();

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
                    method = "2",
                    div = "Details",
                    rootUrl = domain,
                    action = "_WorkerCostCenter",
                    controller = "WorkerInfo",
                    paramName = "Statuslist",
                    paramValue = "0"
                });

            }

            catch (Exception ex)
            {
                string appname = Request.ApplicationPath;
                string domain = Request.Url.GetLeftPart(UriPartial.Authority);
                var lang = Request.RequestContext.RouteData.Values["lang"];

                if (appname != "/")
                {
                    domain = domain + appname;
                }
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new
                {
                    success = false,
                    msg = GlobalResEstate.msgError,
                    status = "danger",
                    checkingdata = "0",
                    method = "2",
                    div = "Details",
                    rootUrl = domain,
                    action = "_WorkerCostCenter",
                    controller = "WorkerInfo",
                    paramName = "Statuslist",
                    paramValue = "0"
                });
            }

            finally
            {
                db.Dispose();
            }
        }

    }
}
