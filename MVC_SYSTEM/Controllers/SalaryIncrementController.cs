using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.ViewingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using MVC_SYSTEM.CustomModels;
using System.Data.Entity;
using static MVC_SYSTEM.Class.GlobalFunction;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public class SalaryIncrementController : Controller
    {
        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
        private GetIdentity getidentity = new GetIdentity();
        private GetTriager GetTriager = new GetTriager();
        private GetNSWL GetNSWL = new GetNSWL();
        private ChangeTimeZone timezone = new ChangeTimeZone();
        private errorlog geterror = new errorlog();
        private GetConfig GetConfig = new GetConfig();
        private GetIdentity GetIdentity = new GetIdentity();
        private GetWilayah GetWilayah = new GetWilayah();
        private GetLadang GetLadang = new GetLadang();
        private Connection Connection = new Connection();

        public ActionResult Index(string filter = "")
        {
            ViewBag.BasicInfo = "class = active";
            ViewBag.filter = filter;
            return View();
        }

        public ViewResult _SalaryIncrementSearch(string filter = "", int page = 1,
            string sort = "fld_Nopkj",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var GetWorkerDetails = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1" && x.fld_KumpulanID != null).ToList();

            var GetWorkerGetIncrement = dbr.tbl_PkjIncrmntSalary.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();

            var GetWorkerGetIncremnts = GetWorkerGetIncrement.Join(GetWorkerDetails, j => j.fld_Nopkj, k => k.fld_Nopkj, (j, k) => new { j.fld_Nopkj, k.fld_Nama, j.fld_IncrmntSalary, j.fld_AppStatus, j.fld_Deleted, j.fld_ProcessStage, j.fld_DailyInsentif }).Select(s => new { s.fld_Nopkj, s.fld_Nama, s.fld_IncrmntSalary, s.fld_AppStatus, s.fld_Deleted, s.fld_ProcessStage, s.fld_DailyInsentif }).ToList();

            var GetWorkerDetailsNoPkj = GetWorkerDetails.Select(s => s.fld_Nopkj).ToList();

            var GetWorkerGetIncrementNoPkj = GetWorkerGetIncrement.Select(s => s.fld_Nopkj).ToList();

            List<string> GetWorkerNoIncremntNoPkj = GetWorkerDetailsNoPkj.Except(GetWorkerGetIncrementNoPkj).ToList();

            var GetWorkerNoIncremnts = GetWorkerDetails.Where(x => GetWorkerNoIncremntNoPkj.Contains(x.fld_Nopkj)).Select(s => new { s.fld_Nopkj, s.fld_Nama }).ToList();

            List<CustMod_IncrementDataList> ListIncrmntData = new List<CustMod_IncrementDataList>();

            short StatusIncrmnt = 0;

            if (GetWorkerGetIncrementNoPkj.Count > 0)
            {
                foreach (var GetWorkerGetIncremnt in GetWorkerGetIncremnts)
                {
                    if (GetWorkerGetIncremnt.fld_AppStatus == true)
                    {
                        StatusIncrmnt = 1;
                    }
                    else
                    {
                        if (GetWorkerGetIncremnt.fld_Deleted == true)
                        {
                            StatusIncrmnt = 3;
                        }
                        else
                        {
                            if (GetWorkerGetIncremnt.fld_ProcessStage == 1)
                            {
                                StatusIncrmnt = 4;
                            }
                            else
                            {
                                StatusIncrmnt = 2;
                            }
                        }
                    }
                    ListIncrmntData.Add(new CustMod_IncrementDataList() { fld_Nopkj = GetWorkerGetIncremnt.fld_Nopkj, fld_Nama = GetWorkerGetIncremnt.fld_Nama, fld_IncrmntSalary = GetWorkerGetIncremnt.fld_IncrmntSalary, StatusGetIncrement = StatusIncrmnt, fld_DailyInsentif = GetWorkerGetIncremnt.fld_DailyInsentif == null ? 0 : GetWorkerGetIncremnt.fld_DailyInsentif });
                }
            }

            if (GetWorkerNoIncremnts.Count > 0)
            {
                foreach (var GetWorkerNoIncremnt in GetWorkerNoIncremnts)
                {
                    ListIncrmntData.Add(new CustMod_IncrementDataList() { fld_Nopkj = GetWorkerNoIncremnt.fld_Nopkj, fld_Nama = GetWorkerNoIncremnt.fld_Nama, fld_IncrmntSalary = 0, StatusGetIncrement = 0, fld_DailyInsentif = 0 });
                }
            }

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<CustMod_IncrementDataList>();

            if (!String.IsNullOrEmpty(filter))
            {
                records.Content = ListIncrmntData
                    .Where(x => x.fld_Nopkj.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_Nama.ToUpper().Contains(filter.ToUpper()))
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = ListIncrmntData
                    .Count(x => x.fld_Nopkj.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_Nama.ToUpper().Contains(filter.ToUpper()));
            }

            else
            {
                records.Content = ListIncrmntData
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = ListIncrmntData
                    .Count();
            }

            dbr.Dispose();
            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.pageSize = pageSize;
            return View(records);

        }

        public ViewResult _SalaryIncrementAdd(string NoPkj)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            CustMod_NewRequestIncrement CustMod_NewRequestIncrement = new CustMod_NewRequestIncrement();

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var CheckIncrementDetail = dbr.tbl_PkjIncrmntSalary.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == NoPkj).FirstOrDefault();
            bool StatusExist = false;
            string NamaPkj = "";
            if (CheckIncrementDetail == null)
            {
                var GetWorkerDetails = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1" && x.fld_KumpulanID != null && x.fld_Nopkj == NoPkj).FirstOrDefault();
                StatusExist = false;
                NamaPkj = GetWorkerDetails.fld_Nama;
                CustMod_NewRequestIncrement.NoPkj = NoPkj;
                CustMod_NewRequestIncrement.NamaPkj = NamaPkj;
                CustMod_NewRequestIncrement.IncrmntVal = 0;
                CustMod_NewRequestIncrement.DailyIncentiveVal = 0;
            }
            else
            {
                StatusExist = true;
            }

            ViewBag.StatusExist = StatusExist;

            return View(CustMod_NewRequestIncrement);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _SalaryIncrementAdd(CustMod_NewRequestIncrement CustMod_NewRequestIncrement)
        {
            string appname = Request.ApplicationPath;
            string domain = Request.Url.GetLeftPart(UriPartial.Authority);
            var lang = Request.RequestContext.RouteData.Values["lang"];
            bool success = true;
            string msg = "";
            string status = "";

            if (appname != "/")
            {
                domain = domain + appname;
            }
            try
            {
                DateTime? date = timezone.gettimezone();
                int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
                int? getuserid = getidentity.ID(User.Identity.Name);
                string host, catalog, user, pass = "";

                GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
                Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
                MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

                if (ModelState.IsValid)
                {
                    success = true;
                    status = "success";
                    msg = GlobalResEstate.msgAdd;

                    var tbl_PkjIncrmntSalaryHistory = new tbl_PkjIncrmntSalaryHistory();

                    var tbl_PkjIncrmntSalary = new tbl_PkjIncrmntSalary();

                    tbl_PkjIncrmntSalary.fld_Nopkj = CustMod_NewRequestIncrement.NoPkj;
                    tbl_PkjIncrmntSalary.fld_IncrmntSalary = CustMod_NewRequestIncrement.IncrmntVal;
                    tbl_PkjIncrmntSalary.fld_DailyInsentif = CustMod_NewRequestIncrement.DailyIncentiveVal;
                    tbl_PkjIncrmntSalary.fld_NegaraID = NegaraID;
                    tbl_PkjIncrmntSalary.fld_SyarikatID = SyarikatID;
                    tbl_PkjIncrmntSalary.fld_WilayahID = WilayahID;
                    tbl_PkjIncrmntSalary.fld_LadangID = LadangID;
                    tbl_PkjIncrmntSalary.fld_ReqBy = getuserid;
                    tbl_PkjIncrmntSalary.fld_ReqDT = date;
                    tbl_PkjIncrmntSalary.fld_Year = date.Value.Year;
                    tbl_PkjIncrmntSalary.fld_ProcessStage = 1;
                    tbl_PkjIncrmntSalary.fld_AppStatus = false;
                    tbl_PkjIncrmntSalary.fld_Deleted = false;

                    dbr.tbl_PkjIncrmntSalary.Add(tbl_PkjIncrmntSalary);
                    dbr.SaveChanges();

                    PropertyCopy.Copy(tbl_PkjIncrmntSalaryHistory, tbl_PkjIncrmntSalary);

                    dbr.tbl_PkjIncrmntSalaryHistory.Add(tbl_PkjIncrmntSalaryHistory);
                    dbr.SaveChanges();
                }
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
                method = "1",
                div = "SearchResult",
                rootUrl = domain,
                action = "_SalaryIncrementSearch",
                controller = "SalaryIncrement"
            });
        }

        public ViewResult _SalaryIncrementEdit(string NoPkj)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            CustMod_NewRequestIncrement CustMod_NewRequestIncrement = new CustMod_NewRequestIncrement();

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var CheckIncrementDetail = dbr.tbl_PkjIncrmntSalary.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == NoPkj).FirstOrDefault();
            bool StatusExist = false;
            string NamaPkj = "";
            DateTime? date = timezone.gettimezone();
            if (CheckIncrementDetail != null)
            {
                var GetWorkerDetails = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1" && x.fld_KumpulanID != null && x.fld_Nopkj == CheckIncrementDetail.fld_Nopkj).FirstOrDefault();
                StatusExist = false;
                NamaPkj = GetWorkerDetails.fld_Nama;
                CustMod_NewRequestIncrement.NoPkj = NoPkj;
                CustMod_NewRequestIncrement.NamaPkj = NamaPkj;
                CustMod_NewRequestIncrement.IncrmntVal = CheckIncrementDetail.fld_IncrmntSalary.Value;
                CustMod_NewRequestIncrement.DailyIncentiveVal = CheckIncrementDetail.fld_DailyInsentif == null ? 0 : CheckIncrementDetail.fld_DailyInsentif.Value;
                if (CheckIncrementDetail.fld_Year == date.Value.Year)
                {
                    CustMod_NewRequestIncrement.SubmitAppStatus = CheckIncrementDetail.fld_ProcessStage.Value;
                }
                else
                {
                    CustMod_NewRequestIncrement.SubmitAppStatus = 1;
                }
            }
            else
            {
                StatusExist = true;
            }

            ViewBag.StatusExist = StatusExist;

            return View(CustMod_NewRequestIncrement);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _SalaryIncrementEdit(CustMod_NewRequestIncrement CustMod_NewRequestIncrement)
        {
            string appname = Request.ApplicationPath;
            string domain = Request.Url.GetLeftPart(UriPartial.Authority);
            var lang = Request.RequestContext.RouteData.Values["lang"];
            bool success = true;
            string msg = "";
            string status = "";

            if (appname != "/")
            {
                domain = domain + appname;
            }
            try
            {
                DateTime? date = timezone.gettimezone();
                int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
                int? getuserid = getidentity.ID(User.Identity.Name);
                string host, catalog, user, pass = "";

                GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
                Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
                MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

                if (ModelState.IsValid)
                {
                    success = true;
                    status = "success";
                    msg = GlobalResEstate.msgAdd;

                    var tbl_PkjIncrmntSalaryHistory = new tbl_PkjIncrmntSalaryHistory();

                    var tbl_PkjIncrmntSalary = dbr.tbl_PkjIncrmntSalary.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == CustMod_NewRequestIncrement.NoPkj).FirstOrDefault();

                    tbl_PkjIncrmntSalary.fld_IncrmntSalary = CustMod_NewRequestIncrement.IncrmntVal;
                    tbl_PkjIncrmntSalary.fld_DailyInsentif = CustMod_NewRequestIncrement.DailyIncentiveVal;
                    tbl_PkjIncrmntSalary.fld_ReqBy = getuserid;
                    tbl_PkjIncrmntSalary.fld_ReqDT = date;
                    tbl_PkjIncrmntSalary.fld_Year = date.Value.Year;
                    tbl_PkjIncrmntSalary.fld_ProcessStage = 1;
                    tbl_PkjIncrmntSalary.fld_AppStatus = false;
                    tbl_PkjIncrmntSalary.fld_Deleted = false;

                    dbr.Entry(tbl_PkjIncrmntSalary).State = EntityState.Modified;
                    dbr.SaveChanges();

                    PropertyCopy.Copy(tbl_PkjIncrmntSalaryHistory, tbl_PkjIncrmntSalary);

                    dbr.tbl_PkjIncrmntSalaryHistory.Add(tbl_PkjIncrmntSalaryHistory);
                    dbr.SaveChanges();
                }
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
                method = "1",
                div = "SearchResult",
                rootUrl = domain,
                action = "_SalaryIncrementSearch",
                controller = "SalaryIncrement"
            });
        }

        public ViewResult _ListIncrementRequest()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var GetWorkerDetails = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1" && x.fld_KumpulanID != null).ToList();

            var GetWorkerGetIncrement = dbr.tbl_PkjIncrmntSalary.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_ProcessStage == 1).ToList();

            var GetWorkerGetIncremnts = GetWorkerGetIncrement.Join(GetWorkerDetails, j => j.fld_Nopkj, k => k.fld_Nopkj, (j, k) => new { j.fld_Nopkj, k.fld_Nama, j.fld_IncrmntSalary, j.fld_AppStatus, j.fld_Deleted, j.fld_DailyInsentif }).Select(s => new { s.fld_Nopkj, s.fld_Nama, s.fld_IncrmntSalary, s.fld_AppStatus, s.fld_Deleted, s.fld_DailyInsentif }).ToList();

            var GetWorkerDetailsNoPkj = GetWorkerDetails.Select(s => s.fld_Nopkj).ToList();

            var GetWorkerGetIncrementNoPkj = GetWorkerGetIncrement.Select(s => s.fld_Nopkj).ToList();

            List<string> GetWorkerNoIncremntNoPkj = GetWorkerDetailsNoPkj.Except(GetWorkerGetIncrementNoPkj).ToList();

            var GetWorkerNoIncremnts = GetWorkerDetails.Where(x => GetWorkerNoIncremntNoPkj.Contains(x.fld_Nopkj)).Select(s => new { s.fld_Nopkj, s.fld_Nama }).ToList();

            List<CustMod_IncrementDataList> ListIncrmntData = new List<CustMod_IncrementDataList>();

            short StatusIncrmnt = 0;

            foreach (var GetWorkerGetIncremnt in GetWorkerGetIncremnts)
            {
                if (GetWorkerGetIncremnt.fld_AppStatus == true)
                {
                    StatusIncrmnt = 1;
                }
                else
                {
                    if (GetWorkerGetIncremnt.fld_Deleted == true)
                    {
                        StatusIncrmnt = 3;
                    }
                    else
                    {
                        StatusIncrmnt = 2;
                    }
                }
                ListIncrmntData.Add(new CustMod_IncrementDataList() { fld_Nopkj = GetWorkerGetIncremnt.fld_Nopkj, fld_Nama = GetWorkerGetIncremnt.fld_Nama, fld_IncrmntSalary = GetWorkerGetIncremnt.fld_IncrmntSalary, StatusGetIncrement = StatusIncrmnt, fld_DailyInsentif = GetWorkerGetIncremnt.fld_DailyInsentif });
            }
            return View(ListIncrmntData);
        }

        public JsonResult GetBatchNo()
        {
            DatabaseAction DatabaseAction = new DatabaseAction();
            DateTime DT = timezone.gettimezone();
            int? convertint = 0;
            string genbatchno = "";
            long batchid = 0;
            string host, catalog, user, pass = "";

            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var GetNSWLDetail = GetNSWL.GetLadangDetail(LadangID.Value);
            var getbatchno = db.tbl_BatchRunNo.Where(x => x.fld_BatchFlag == "slryincrmnbatchno" && x.fld_NegaraID == GetNSWLDetail.fld_NegaraID && x.fld_SyarikatID == GetNSWLDetail.fld_SyarikatID && x.fld_WilayahID == GetNSWLDetail.fld_WilayahID && x.fld_LadangID == GetNSWLDetail.fld_LadangID).FirstOrDefault();
            var GetWorkerGetIncrement = dbr.tbl_PkjIncrmntSalary.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_ProcessStage == 1).ToList();
            var GetWorkerGetIncrementHistory = dbr.tbl_PkjIncrmntSalaryHistory.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_ProcessStage == 1).ToList();

            if (getbatchno == null)
            {
                tbl_BatchRunNo tbl_BatchRunNo = new tbl_BatchRunNo();
                tbl_BatchRunNo.fld_BatchRunNo = 2;
                tbl_BatchRunNo.fld_BatchFlag = "slryincrmnbatchno";
                tbl_BatchRunNo.fld_NegaraID = GetNSWLDetail.fld_NegaraID;
                tbl_BatchRunNo.fld_SyarikatID = GetNSWLDetail.fld_SyarikatID;
                tbl_BatchRunNo.fld_WilayahID = GetNSWLDetail.fld_WilayahID;
                tbl_BatchRunNo.fld_LadangID = GetNSWLDetail.fld_LadangID;
                db.tbl_BatchRunNo.Add(tbl_BatchRunNo);
                db.SaveChanges();
                convertint = 1;
                genbatchno = GetNSWLDetail.fld_RequestCode.ToUpper() + "_SALARYINCREMENT_" + GetNSWLDetail.fld_LdgCode.ToUpper() + "_" + convertint;
            }
            else
            {
                convertint = getbatchno.fld_BatchRunNo;
                genbatchno = GetNSWLDetail.fld_RequestCode.ToUpper() + "_SALARYINCREMENT_" + GetNSWLDetail.fld_LdgCode.ToUpper() + "_" + convertint;
                convertint = convertint + 1;
                getbatchno.fld_BatchRunNo = convertint;
                db.Entry(getbatchno).State = EntityState.Modified;
                db.SaveChanges();
            }

            var checkbatchexisting = db.tblASCApprovalFileDetails.Where(x => x.fldFileName == genbatchno && x.fldCodeLadang == GetNSWLDetail.fld_LdgCode && x.fldNegaraID == GetNSWLDetail.fld_NegaraID && x.fldSyarikatID == GetNSWLDetail.fld_SyarikatID && x.fldWilayahID == GetNSWLDetail.fld_WilayahID && x.fldLadangID == GetNSWLDetail.fld_LadangID).FirstOrDefault();

            if (checkbatchexisting == null)
            {
                batchid = DatabaseAction.InsertDataTotblASCApprovalFileDetail(genbatchno, GetNSWLDetail.fld_LdgCode, GetNSWLDetail.fld_NegaraID, GetNSWLDetail.fld_SyarikatID, GetNSWLDetail.fld_WilayahID, GetNSWLDetail.fld_LadangID, 0, 1, 3, DT);
                DatabaseAction.InsertDataTotbl_PkjIncrmntApp(GetWorkerGetIncrement, batchid);
                GetWorkerGetIncrement.ForEach(u => u.fld_FileID = batchid);
                dbr.SaveChanges();

                GetWorkerGetIncrementHistory.ForEach(u => u.fld_FileID = batchid);
                dbr.SaveChanges();
            }

            return Json(genbatchno);
        }

    }
}