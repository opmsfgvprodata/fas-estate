using System;
//using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Data.Entity;
//using System.Web;
using System.Web.Mvc;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.log;
using MVC_SYSTEM.ViewingModels;
using System.Collections.Generic;
using MVC_SYSTEM.App_LocalResources;
using System.Web;
using System.IO;
using MVC_SYSTEM.Attributes;
using System.Text.RegularExpressions;
using MVC_SYSTEM.CustomModels;
using static MVC_SYSTEM.Class.GlobalFunction;
using System.Globalization;
using MVC_SYSTEM.CustomModels2;

namespace MVC_SYSTEM.Controllers
{
    //Check_Balik
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public class BasicInfoController : Controller
    {
        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
        //private MVC_SYSTEM_HQ_Models db = new MVC_SYSTEM_HQ_Models();
        //MVC_SYSTEM_Viewing dbview = new MVC_SYSTEM_Viewing();
        private GetIdentity getidentity = new GetIdentity();
        private GetNSWL GetNSWL = new GetNSWL();
        errorlog geterror = new errorlog();
        GetConfig GetConfig = new GetConfig();
        GetIdentity GetIdentity = new GetIdentity();
        GetWilayah GetWilayah = new GetWilayah();
        Connection Connection = new Connection();
        GetLadang GetLadang = new GetLadang();
        GlobalFunction globalFunction = new GlobalFunction();

        private ChangeTimeZone timezone = new ChangeTimeZone();

        // GET: BasicInfo
        public ActionResult Index()
        {
            //Check_Balik
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? getroleid = getidentity.getRoleID(getuserid);
            int?[] reportid = new int?[] { };
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            //string host, catalog, user, pass = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.BasicInfo = "class = active";
            ViewBag.BasicInfoList = new SelectList(
                db.tblMenuLists.Where(x => x.fld_Flag == "asasldgList" && x.fldDeleted == false && x.fld_NegaraID == NegaraID &&
                                           x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_ID),
                "fld_Val", "fld_Desc");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string BasicInfoList)
        {
            return RedirectToAction(BasicInfoList, "BasicInfo");
        }

        public JsonResult GetSubList(int ReportList)
        {
            //Check_Balik
            List<SelectListItem> getsublist = new List<SelectListItem>();

            getsublist = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "asaspkjsub" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc").ToList();
            //getsublist = new SelectList(db.tblSubReportLists.Where(x => x.fldMainReportID == ReportList && x.fldDeleted == false).OrderBy(o => o.fldSubReportListName).Select(s => new SelectListItem { Value = s.fldSubReportListID.ToString(), Text = s.fldSubReportListName }), "Value", "Text").ToList();

            return Json(getsublist);
        }

        public ActionResult EstateInfo()
        {
            //Check_Balik
            ViewBag.BasicInfo = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            //string host, catalog, user, pass = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            //Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            //MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            MasterModels.tbl_Ladang tbl_Ladang = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_ID == LadangID && x.fld_WlyhID == WilayahID).FirstOrDefault();
            string kodngri = tbl_Ladang.fld_KodNegeri.ToString();
            string negeri = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "negeri" && x.fldOptConfValue == kodngri && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false && x.fldOptConfValue == kodngri).Select(s => s.fldOptConfDesc).FirstOrDefault();

            ViewBag.Negeri = negeri;

            return View(tbl_Ladang);
            //return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EstateInfo(MasterModels.tbl_Ladang tbl_Ladang)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);


            tbl_Ladang.fld_PengurusSblm = "-";
            tbl_Ladang.fld_AdressSblm = "-";
            tbl_Ladang.fld_TelSblm = "-";
            tbl_Ladang.fld_FaxSblm = "-";
            tbl_Ladang.fld_LdgEmailSblm = "-";

            if (ModelState.IsValid)
            {
                try
                {

                    //var getdata = db.tbl_Ladang.Where(w => w.fld_ID == tbl_Ladang.fld_ID && w.fld_WlyhID == tbl_Ladang.fld_WlyhID).FirstOrDefault();
                    //getdata.fld_Pengurus = tbl_Ladang.fld_Pengurus.ToUpper();
                    //getdata.fld_Adress = tbl_Ladang.fld_Adress.ToUpper();
                    //getdata.fld_Tel = tbl_Ladang.fld_Tel;
                    //getdata.fld_Fax = tbl_Ladang.fld_Fax;
                    //db.Entry(getdata).State = EntityState.Modified;
                    //db.SaveChanges();


                    var getdatahq = db.tbl_Ladang.Where(w => w.fld_NegaraID == NegaraID && w.fld_SyarikatID == SyarikatID && w.fld_ID == tbl_Ladang.fld_ID && w.fld_WlyhID == tbl_Ladang.fld_WlyhID).FirstOrDefault();
                    getdatahq.fld_PengurusSblm = getdatahq.fld_Pengurus;
                    getdatahq.fld_AdressSblm = getdatahq.fld_Adress;
                    getdatahq.fld_TelSblm = getdatahq.fld_Tel;
                    getdatahq.fld_FaxSblm = getdatahq.fld_Fax;
                    getdatahq.fld_LdgEmailSblm = getdatahq.fld_LdgEmail;

                    getdatahq.fld_Pengurus = tbl_Ladang.fld_Pengurus.ToUpper();
                    getdatahq.fld_Adress = tbl_Ladang.fld_Adress.ToUpper();
                    getdatahq.fld_Tel = tbl_Ladang.fld_Tel;
                    getdatahq.fld_Fax = tbl_Ladang.fld_Fax;
                    getdatahq.fld_LdgEmail = tbl_Ladang.fld_LdgEmail;


                    db.Entry(getdatahq).State = EntityState.Modified;
                    db.SaveChanges();

                    var getid = tbl_Ladang.fld_ID;
                    //return RedirectToAction("EstateInfo", "BasicInfo");
                    //string a = "alert('this is alert')";
                    //return JavaScript(a);
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    //return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
                }
            }
            return RedirectToAction("EstateInfo", "BasicInfo");
            //else
            //{
            //    return Json(new { success = true, msg = "Please check field you inserted.", status = "warning", checkingdata = "1" });
            //}

            //return View();
        }

        public ActionResult LevelsInfoOthr()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            ViewBag.BasicInfo = "class = active";


            return View();
        }

        public PartialViewResult _LevelsInfoOthrSearch(string PktSearch = "", int page = 1, string sort = "fld_PktUtama", string sortdir = "DESC")
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            int? DivisionID = 0;
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.tbl_PktUtama>();
            if (PktSearch.Length > 0)
            {
                records.Content = dbview.tbl_PktUtama.Where(x => x.fld_NamaPktUtama.Contains(PktSearch) && x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == false && x.fld_JnsLot != "E" && x.fld_DivisionID == DivisionID)
                  .OrderBy(sort + " " + sortdir)
                  .Skip((page - 1) * pageSize)
                  .Take(pageSize)
                  .ToList();

                records.TotalRecords = dbview.tbl_PktUtama.Where(x => x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == false && x.fld_JnsLot != "E" && x.fld_DivisionID == DivisionID).Count();
                records.CurrentPage = page;
                records.PageSize = pageSize;
            }
            else
            {
                records.Content = dbview.tbl_PktUtama.Where(x => x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == false && x.fld_JnsLot != "E" && x.fld_DivisionID == DivisionID)
                  .OrderBy(sort + " " + sortdir)
                  .Skip((page - 1) * pageSize)
                  .Take(pageSize)
                  .ToList();

                records.TotalRecords = dbview.tbl_PktUtama.Where(x => x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == false && x.fld_JnsLot != "E" && x.fld_DivisionID == DivisionID).Count();
                records.CurrentPage = page;
                records.PageSize = pageSize;
            }

            return PartialView(records);
        }

        public ActionResult PktOthrCreate()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            ViewBag.BasicInfo = "class = active";

            List<SelectListItem> JnsLot = new List<SelectListItem>();
            JnsLot = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "activityLevel" && x.fldOptConfFlag2 == "1" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            JnsLot.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "" }));

            List<SelectListItem> KatAktvt = new List<SelectListItem>();
            //KatAktvt = new SelectList(db.tbl_KategoriAktiviti.Where(x => x.fldOptConfFlag1 == "activityLevel" && x.fldOptConfFlag2 == "1" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            KatAktvt.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "" }));

            List<SelectListItem> CC = new List<SelectListItem>();
            //var GetCCUsed = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && (x.fld_JnsLot != "E" && x.fld_JnsLot != "S" )).Select(s => s.fld_IOcode).ToArray();
            //CC = new SelectList(db.tbl_SAPCCPUP.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && !GetCCUsed.Contains(x.fld_CostCenter)).OrderBy(o => o.fld_CostCenter).Select(s => new SelectListItem { Value = s.fld_CostCenter, Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc }), "Value", "Text").ToList();
            CC = new SelectList(db.tbl_SAPCCPUP.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_CostCenter).Select(s => new SelectListItem { Value = s.fld_CostCenter, Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc }), "Value", "Text").ToList();
            CC.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "" }));

            ViewBag.fld_JnsLot = JnsLot;
            ViewBag.fld_IOref = KatAktvt;
            ViewBag.fld_IOcode = CC;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PktOthrCreate(Models.tbl_PktUtama tbl_PktUtama)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass); 
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            tbl_PktUtamaOthr tbl_PktUtamaOthr = new tbl_PktUtamaOthr();

            if (ModelState.IsValid)
            {
                try
                {
                    tbl_PktUtama.fld_NamaPktUtama = tbl_PktUtama.fld_NamaPktUtama.ToUpper();
                    tbl_PktUtama.fld_LadangID = LadangID;
                    tbl_PktUtama.fld_WilayahID = WilayahID;
                    tbl_PktUtama.fld_SyarikatID = SyarikatID;
                    tbl_PktUtama.fld_NegaraID = NegaraID;
                    tbl_PktUtama.fld_DivisionID = DivisionID;
                    tbl_PktUtama.fld_CreateDate = DateTime.Now;
                    tbl_PktUtama.fld_Deleted = false;
                    dbr.tbl_PktUtama.Add(tbl_PktUtama);
                    dbr.SaveChanges();

                    db.Dispose();
                    dbr.Dispose();

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
                        method = "2",
                        div = "searchResult",
                        rootUrl = domain,
                        action = "_LevelsInfoOthrSearch",
                        controller = "BasicInfo",
                        paramName = "CostCentreSearch",
                        paramValue = "0"
                    });
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());

                    db.Dispose();
                    dbr.Dispose();

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
                db.Dispose();
                dbr.Dispose();

                return Json(new
                {
                    success = false,
                    msg = GlobalResEstate.msgErrorData,
                    status = "warning",
                    checkingdata = "0"
                });
            }
        }

        public ActionResult PktOthrEdit(string id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            ViewBag.BasicInfo = "class = active";

            Models.tbl_PktUtama tbl_PktUtama = dbr.tbl_PktUtama.Where(w => w.fld_PktUtama == id && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID && w.fld_LadangID == LadangID && w.fld_Deleted == false).FirstOrDefault();
            string PrefixKatAkt = tbl_PktUtama.fld_IOref;
            var Kate = db.tbl_KategoriAktiviti.Where(x => x.fld_PrefixPkt == PrefixKatAkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).FirstOrDefault();
            string KateName = Kate.fld_Kategori;
            string JnsLot = db.tblOptionConfigsWebs.Where(x => x.fldOptConfValue == Kate.fld_KodJnsAktvt && x.fldOptConfFlag1 == "activityLevel" && x.fldOptConfFlag2 == "1" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfDesc).FirstOrDefault();

            ViewBag.KateName = KateName;
            ViewBag.JnsLot = JnsLot;

            return View(tbl_PktUtama);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PktOthrEdit(string id, Models.tbl_PktUtama tbl_PktUtama)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            if (tbl_PktUtama.fld_NamaPktUtama.Length > 0)
            {
                try
                {
                    var getdata = dbr.tbl_PktUtama.Where(w => w.fld_PktUtama == id && w.fld_LadangID == LadangID && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID && w.fld_Deleted == false).FirstOrDefault();
                    getdata.fld_NamaPktUtama = tbl_PktUtama.fld_NamaPktUtama.ToUpper();
                    dbr.Entry(getdata).State = EntityState.Modified;
                    dbr.SaveChanges();
                    dbr.Dispose();

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
                        div = "searchResult",
                        rootUrl = domain,
                        action = "_LevelsInfoOthrSearch",
                        controller = "BasicInfo",
                        paramName = "CostCentreSearch",
                        paramValue = "0"
                    });
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());

                    db.Dispose();
                    dbr.Dispose();

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
                db.Dispose();
                dbr.Dispose();

                return Json(new
                {
                    success = false,
                    msg = GlobalResEstate.msgErrorData,
                    status = "warning",
                    checkingdata = "0"
                });
            }
        }

        public ActionResult PktOthrDelete(string id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            ViewBag.BasicInfo = "class = active";

            Models.tbl_PktUtama tbl_PktUtama = dbr.tbl_PktUtama.Where(w => w.fld_PktUtama == id && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID && w.fld_LadangID == LadangID && w.fld_Deleted == false).FirstOrDefault();
            return View(tbl_PktUtama);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PktOthrDelete(string id, Models.tbl_PktUtama tbl_PktUtama)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                var pktUtama = dbr.tbl_PktUtama.Where(w => w.fld_PktUtama == id && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID && w.fld_LadangID == LadangID && w.fld_Deleted == false).FirstOrDefault();
                if (pktUtama != null)
                {
                    pktUtama.fld_Deleted = true;
                    dbr.Entry(pktUtama).State = EntityState.Modified;
                    dbr.SaveChanges();
                }
                dbr.Dispose();

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
                    method = "2",
                    div = "searchResult",
                    rootUrl = domain,
                    action = "_LevelsInfoOthrSearch",
                    controller = "BasicInfo",
                    paramName = "CostCentreSearch",
                    paramValue = "0"
                });
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());

                db.Dispose();
                dbr.Dispose();

                return Json(new
                {
                    success = false,
                    msg = GlobalResEstate.msgError,
                    status = "danger",
                    checkingdata = "0"
                });
            }
        }

        public JsonResult GetKatAktvtCC(string JnsLot)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> KatAktvt = new List<SelectListItem>();
            KatAktvt = new SelectList(db.tbl_KategoriAktiviti.Where(x => x.fld_KodJnsAktvt == JnsLot && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_Kategori).Select(s => new SelectListItem { Value = s.fld_PrefixPkt, Text = s.fld_Kategori }), "Value", "Text").ToList();
            return Json(KatAktvt);
        }

        public JsonResult GetPktUtamaCC(string Prefix)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            string codetnmn;
            int kodpkt = 0;
            int newkodpkt = 0;
            string newpkt = "";
            string tahun = DateTime.Now.ToString("yy");

            if (Prefix != "0")
            {
                codetnmn = Prefix + tahun;
                var getpkt = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_PktUtama.Contains(codetnmn)).Select(s => s.fld_PktUtama).Distinct().OrderByDescending(s => s).FirstOrDefault();

                if (getpkt != null)
                {
                    kodpkt = Convert.ToInt32(getpkt.Substring(getpkt.Length - 2));
                }
                else
                {
                    kodpkt = 0;
                }
                newkodpkt = kodpkt + 1;
                newpkt = codetnmn + newkodpkt.ToString("00");
            }
            else
            {
                newpkt = "";
            }
            return Json(newpkt);
        }
        //public ActionResult GetKodPeringkat(string CostCentre)
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

        //    string GetKodKategori = db.tbl_CostCentre.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_CostCentre == CostCentre).Select(s => s.fld_KodKtgri).FirstOrDefault();
        //    string GetPrefix = db.tbl_KategoriAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodKategori == GetKodKategori).Select(s => s.fld_PrefixPkt).FirstOrDefault();

        //    string newpkt = "";
        //    string ktgriAxtvt = db.tbl_KategoriAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodKategori == GetKodKategori).Select(s => s.fld_Kategori).FirstOrDefault();
        //    int kodpkt = 0;
        //    int newkodpkt = 0;

        //    string tahun = DateTime.Now.ToString("yy");

        //    var getpkt = dbr.tbl_PktUtamaOthr.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_PktCode.Contains(GetPrefix)).OrderByDescending(o => o.fld_PktID).Select(s => s.fld_PktCode).Take(1).FirstOrDefault();
        //    if (getpkt != null)
        //    {
        //        kodpkt = Convert.ToInt32(getpkt.Substring(4, 2));
        //    }
        //    else
        //    {
        //        kodpkt = 0;
        //    }
        //    newkodpkt = kodpkt + 1;
        //    newpkt = GetPrefix + tahun + newkodpkt.ToString("00");

        //    return Json(new { newpkt, ktgriAxtvt });
        //}

        public ActionResult LevelsInfo()
        {
            ViewBag.BasicInfo = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<SelectListItem> JenisPkt = new List<SelectListItem>();

            JenisPkt = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnspkt" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            JenisPkt.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            ViewBag.JnsPkt = JenisPkt;
            return View();
        }

        public ActionResult LevelsInfoPkt(string JnsPkt = "1", int page = 1, string sort = "fld_PktUtama", string sortdir = "ASC")
        {
            //shah 14/01/2021 - repair for paging 
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            var records = new PagedList<CustMod_Pkt>();
            var CustMod_Pkt = new List<CustMod_Pkt>();
            GetEstateDetail GetEstateDetail = new GetEstateDetail();
            int pageSize = int.Parse(GetConfig.GetData("paging"));
            switch (JnsPkt)
            {
                case "1":
                    var pktutama = dbview.tbl_PktUtama.Where(x => x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == false && x.fld_JnsLot == "E" && x.fld_DivisionID == DivisionID)
                   .OrderBy("fld_PktUtama" + " " + sortdir)
                   .Skip((page - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
                    foreach (var item in pktutama)
                    {
                        //add by fitri 16-02-2021
                        var GetCC = db.tbl_SAPPDPUP.FirstOrDefault(a => a.fld_WBSCode == item.fld_IOcode && a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID && a.fld_Deleted == false);
                        CustMod_Pkt.Add(new CustMod_Pkt { id = item.fld_ID, cc = GetCC.fld_CostCenter, kodpkt = item.fld_PktUtama, luaspkt = item.fld_LsPktUtama, namapkt = item.fld_NamaPktUtama, wbs = item.fld_IOcode, deleteaction = "LevelsPktDelete", editaction = "LevelsPktUpdate" });
                        //comment by fitri 16-02-2021
                        //CustMod_Pkt.Add(new CustMod_Pkt { id = item.fld_ID, kodpkt = item.fld_PktUtama, luaspkt = item.fld_LsPktUtama, namapkt = item.fld_NamaPktUtama, wbs = item.fld_IOcode, deleteaction = "LevelsPktDelete", editaction = "LevelsPktUpdate" });
                    }
                    records.Content = CustMod_Pkt;
                    records.TotalRecords = dbview.tbl_PktUtama.Where(x => x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == false && x.fld_JnsLot == "E" && x.fld_DivisionID == DivisionID).Count();
                    break;
                case "2":
                    var subpkt = dbview.tbl_SubPkt.Where(x => x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == false)
                   .OrderBy("fld_Pkt" + " " + sortdir)
                   .Skip((page - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
                    foreach (var item in subpkt)
                    {
                        //comment by fitri 16-02-2021
                        //var wbs = GetEstateDetail.GetWBSCode(item.fld_NegaraID, item.fld_SyarikatID, item.fld_WilayahID, item.fld_LadangID, item.fld_KodPktUtama);
                        //CustMod_Pkt.Add(new CustMod_Pkt { id = item.fld_ID, kodpkt = item.fld_Pkt, luaspkt = item.fld_LsPkt, namapkt = item.fld_NamaPkt, wbs = wbs, deleteaction = "LevelsSubPktDelete", editaction = "LevelsSubPktUpdate" });
                        //add by fitri 16-02-2021
                        var wbs = GetEstateDetail.GetWBSCode(item.fld_NegaraID, item.fld_SyarikatID, item.fld_WilayahID, item.fld_LadangID, item.fld_KodPktUtama);
                        var GetCC = db.tbl_SAPPDPUP.FirstOrDefault(a => a.fld_WBSCode == wbs && a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID && a.fld_Deleted == false);
                        CustMod_Pkt.Add(new CustMod_Pkt { id = item.fld_ID, cc = GetCC.fld_CostCenter, kodpkt = item.fld_Pkt, luaspkt = item.fld_LsPkt, namapkt = item.fld_NamaPkt, wbs = wbs, deleteaction = "LevelsSubPktDelete", editaction = "LevelsSubPktUpdate" });
                    }
                    records.Content = CustMod_Pkt;
                    records.TotalRecords = dbview.tbl_SubPkt.Where(x => x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == false).Count();
                    break;
            }
            records.CurrentPage = page;
            records.PageSize = pageSize;
            return View(records);
        }

        //public ActionResult LevelsInfoSubPkt(string JnsPkt = "2", int page = 1, string sort = "fld_Pkt", string sortdir = "ASC")
        //{
        //    int? getuserid = GetIdentity.ID(User.Identity.Name);
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

        //    int pageSize = int.Parse(GetConfig.GetData("paging"));
        //    var records = new PagedList<ViewingModels.tbl_SubPkt>();
        //    records.Content = dbview.tbl_SubPkt.Where(x => x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == false)
        //           .OrderBy(sort + " " + sortdir)
        //           .Skip((page - 1) * pageSize)
        //           .Take(pageSize)
        //           .ToList();

        //    records.TotalRecords = dbview.tbl_SubPkt.Where(x => x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == false).Count();
        //    records.CurrentPage = page;
        //    records.PageSize = pageSize;
        //    return View(records);
        //}

        public ActionResult LevelsInfoBlok(string JnsPkt = "3", int page = 1, string sort = "fld_Blok", string sortdir = "ASC")
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.tbl_Blok>();
            records.Content = dbview.tbl_Blok.Where(x => x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == false)
                   .OrderBy(sort + " " + sortdir)
                   .Skip((page - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();

            records.TotalRecords = dbview.tbl_Blok.Where(x => x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == false).Count();
            records.CurrentPage = page;
            records.PageSize = pageSize;
            return View(records);
        }

        public ActionResult LevelsPktCreate()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> JnsTnmn = new List<SelectListItem>();
            List<SelectListItem> StatusTnmn = new List<SelectListItem>();
            //List<SelectListItem> TahapKesukaranMenuai = new List<SelectListItem>();
            //List<SelectListItem> TahapKesukaranMembaja = new List<SelectListItem>();


            JnsTnmn = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "jnsTanaman" && x.fldDeleted == false).OrderBy(o => o.fldOptConfID).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }), "Value", "Text").ToList();
            JnsTnmn.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            StatusTnmn = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "statusTanaman" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }), "Value", "Text").ToList();
            StatusTnmn.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            //TahapKesukaranMenuai = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMenuai" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text").ToList();
            //TahapKesukaranMenuai.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            //TahapKesukaranMembaja = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMembaja" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text").ToList();
            //TahapKesukaranMembaja.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            var GetWBSUsed = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_JnsLot == "E").Select(s => s.fld_IOcode).ToArray();
            List<SelectListItem> WBSlist = new List<SelectListItem>();
            WBSlist = new SelectList(db.tbl_SAPCUSTOMPUP.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_IsSelected == false && !GetWBSUsed.Contains(x.fld_WBSCode)), "fld_WBSCode", "fld_WBSCode").ToList();
            WBSlist.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));


            ViewBag.fld_JnsTnmn = JnsTnmn;
            ViewBag.fld_StatusTnmn = StatusTnmn;
            //ViewBag.fld_KesukaranMenuaiPktUtama = TahapKesukaranMenuai;
            //ViewBag.fld_KesukaranMembajaPktUtama = TahapKesukaranMembaja;
            ViewBag.fld_IOcode = WBSlist;
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LevelsPktCreate(Models.tbl_PktUtama tbl_PktUtama)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            if (ModelState.IsValid)
            {
                try
                {
                    if (tbl_PktUtama.fld_IOcode != "0")
                    {
                        var checkdata = dbr.tbl_PktUtama.Where(x => x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && (x.fld_PktUtama == tbl_PktUtama.fld_PktUtama || x.fld_IOcode == tbl_PktUtama.fld_IOcode) && x.fld_Deleted == false).FirstOrDefault();
                        if (checkdata == null)
                        {
                            tbl_PktUtama.fld_NamaPktUtama = tbl_PktUtama.fld_NamaPktUtama.ToUpper();
                            tbl_PktUtama.fld_LadangID = LadangID;
                            tbl_PktUtama.fld_WilayahID = WilayahID;
                            tbl_PktUtama.fld_SyarikatID = SyarikatID;
                            tbl_PktUtama.fld_NegaraID = NegaraID;
                            tbl_PktUtama.fld_CreateDate = DateTime.Now;
                            tbl_PktUtama.fld_Deleted = false;
                            tbl_PktUtama.fld_JnsLot = "E";
                            tbl_PktUtama.fld_DivisionID = DivisionID;
                            dbr.tbl_PktUtama.Add(tbl_PktUtama);
                            dbr.SaveChanges();

                            //var checkIo = dbr.tbl_IO.Where(x => x.fld_IOcode == tbl_PktUtama.fld_IOcode && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
                            //checkIo.fld_Deleted = true;
                            //dbr.Entry(checkIo).State = EntityState.Modified;
                            //dbr.SaveChanges();
                            string RequestForm = Request.Form["listCount"];
                            if (RequestForm != null && RequestForm != "")
                            {
                                int listCount = Convert.ToInt32(Request.Form["listCount"]);
                                if (listCount < 1)
                                {
                                    for (int i = 1; i <= listCount; i++)
                                    {
                                        string idKaw = "ddl" + i;
                                        string idLuas = "textluas" + i;
                                        string JnsKaw = Request.Form[idKaw];
                                        decimal LuasKaw = Convert.ToDecimal(Request.Form[idLuas]);
                                        var checkKwsn = dbr.tbl_KawTidakBerhasil.Where(x => x.fld_JnsKaw == JnsKaw && x.fld_KodPkt == tbl_PktUtama.fld_PktUtama && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
                                        if (checkKwsn == null)
                                        {
                                            Models.tbl_KawTidakBerhasil KawTidakBerhasil = new Models.tbl_KawTidakBerhasil();
                                            KawTidakBerhasil.fld_KodPkt = tbl_PktUtama.fld_PktUtama;
                                            KawTidakBerhasil.fld_LevelPkt = 1;
                                            KawTidakBerhasil.fld_JnsKaw = JnsKaw;
                                            KawTidakBerhasil.fld_LuasKaw = LuasKaw;
                                            KawTidakBerhasil.fld_NegaraID = NegaraID;
                                            KawTidakBerhasil.fld_SyarikatID = SyarikatID;
                                            KawTidakBerhasil.fld_WilayahID = WilayahID;
                                            KawTidakBerhasil.fld_LadangID = LadangID;
                                            KawTidakBerhasil.fld_Deleted = false;
                                            dbr.tbl_KawTidakBerhasil.Add(KawTidakBerhasil);
                                            dbr.SaveChanges();
                                        }
                                    }
                                }
                            }
                            return Json(new { success = true, msg = "Successfully Added", status = "success", checkingdata = "1" });
                        }
                        else
                        {
                            return Json(new { success = true, msg = "Data Already Exist", status = "warning", checkingdata = "1" });
                        }
                    }
                    else
                    {
                        return Json(new { success = true, msg = "Please Select WBS Code", status = "warning", checkingdata = "1" });
                    }

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

        public ActionResult LevelsPktUpdate(string id)
        {
            GetStatus GetStatus = new GetStatus();
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            Models.tbl_PktUtama tbl_PktUtama = dbr.tbl_PktUtama.Where(w => w.fld_PktUtama == id && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID && w.fld_LadangID == LadangID && w.fld_Deleted == false).FirstOrDefault();

            List<SelectListItem> JnsTnmn = new List<SelectListItem>();
            List<SelectListItem> StatusTnmn = new List<SelectListItem>();
            List<SelectListItem> TahapKesukaranMenuai = new List<SelectListItem>();
            List<SelectListItem> TahapKesukaranMembaja = new List<SelectListItem>();

            JnsTnmn = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "jnsTanaman" && x.fldDeleted == false).OrderBy(o => o.fldOptConfID).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }), "Value", "Text").ToList();
            JnsTnmn.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            StatusTnmn = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "statusTanaman" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }), "Value", "Text").ToList();
            StatusTnmn.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            TahapKesukaranMenuai = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMenuai" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text", tbl_PktUtama.fld_KesukaranMenuaiPktUtama).ToList();
            //TahapKesukaranMenuai.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            TahapKesukaranMembaja = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMembaja" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text", tbl_PktUtama.fld_KesukaranMembajaPktUtama).ToList();
            //TahapKesukaranMembaja.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            List<SelectListItem> Kawasanlist = new List<SelectListItem>();
            Kawasanlist = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsKawasan" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            Kawasanlist.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            ViewBag.fld_JnsTnmn = JnsTnmn;
            ViewBag.fld_StatusTnmn = StatusTnmn;
            ViewBag.fld_KesukaranMenuaiPktUtama = TahapKesukaranMenuai;
            ViewBag.fld_KesukaranMembajaPktUtama = TahapKesukaranMembaja;
            ViewBag.fld_JnsKaw1 = Kawasanlist;
            return PartialView(tbl_PktUtama);

            //return PartialView("WorkerUpdate", tbl_Pkjmast);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LevelsPktUpdate(string id, Models.tbl_PktUtama tbl_PktUtama)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                var getdata = dbr.tbl_PktUtama.Where(w => w.fld_PktUtama == id && w.fld_LadangID == LadangID && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID && w.fld_Deleted == false).FirstOrDefault();
                getdata.fld_NamaPktUtama = tbl_PktUtama.fld_NamaPktUtama.ToUpper();
                getdata.fld_KesukaranMenuaiPktUtama = tbl_PktUtama.fld_KesukaranMenuaiPktUtama;
                getdata.fld_KesukaranMembajaPktUtama = tbl_PktUtama.fld_KesukaranMembajaPktUtama;
                getdata.fld_LsPktUtama = tbl_PktUtama.fld_LsPktUtama;
                getdata.fld_LuasKawTnman = tbl_PktUtama.fld_LuasKawTnman;
                getdata.fld_LuasBerhasil = tbl_PktUtama.fld_LuasBerhasil;
                getdata.fld_LuasBlmBerhasil = tbl_PktUtama.fld_LuasBlmBerhasil;
                getdata.fld_BilPokok = tbl_PktUtama.fld_BilPokok;
                getdata.fld_DirianPokok = tbl_PktUtama.fld_DirianPokok;
                getdata.fld_LuasKawTiadaTanaman = tbl_PktUtama.fld_LuasKawTiadaTanaman;
                dbr.Entry(getdata).State = EntityState.Modified;
                dbr.SaveChanges();

                string RequestForm = Request.Form["listCount"];
                if (RequestForm != null && RequestForm != "")
                {
                    var getLuas = dbr.tbl_KawTidakBerhasil.Where(x => x.fld_KodPkt == tbl_PktUtama.fld_PktUtama && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).AsEnumerable();
                    dbr.tbl_KawTidakBerhasil.RemoveRange(getLuas);
                    dbr.SaveChanges();

                    int listCount = Convert.ToInt32(Request.Form["listCount"]);
                    for (int i = 1; i <= listCount; i++)
                    {
                        string idKaw = "ddl" + i;
                        string idLuas = "textluas" + i;
                        string JnsKaw = Request.Form[idKaw];
                        decimal LuasKaw = Convert.ToDecimal(Request.Form[idLuas]);
                        var checkKwsn = dbr.tbl_KawTidakBerhasil.Where(x => x.fld_JnsKaw == JnsKaw && x.fld_KodPkt == tbl_PktUtama.fld_PktUtama && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
                        if (checkKwsn == null)
                        {
                            Models.tbl_KawTidakBerhasil KawTidakBerhasil = new Models.tbl_KawTidakBerhasil();
                            KawTidakBerhasil.fld_KodPkt = tbl_PktUtama.fld_PktUtama;
                            KawTidakBerhasil.fld_LevelPkt = 1;
                            KawTidakBerhasil.fld_JnsKaw = JnsKaw;
                            KawTidakBerhasil.fld_LuasKaw = LuasKaw;
                            KawTidakBerhasil.fld_NegaraID = NegaraID;
                            KawTidakBerhasil.fld_SyarikatID = SyarikatID;
                            KawTidakBerhasil.fld_WilayahID = WilayahID;
                            KawTidakBerhasil.fld_LadangID = LadangID;
                            KawTidakBerhasil.fld_Deleted = false;
                            dbr.tbl_KawTidakBerhasil.Add(KawTidakBerhasil);
                            dbr.SaveChanges();
                        }
                    }
                }
                var getid = id;
                return Json(new { success = true, msg = GlobalResEstate.msgUpdate, status = "success", checkingdata = "0", method = "1", getid = getid, data1 = "", data2 = "" });
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = true, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }

        public ActionResult LevelsPktDelete(string id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int DeleteFlag = 1;
            Models.tbl_PktUtama tbl_PktUtama = dbr.tbl_PktUtama.Where(w => w.fld_PktUtama == id && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID && w.fld_LadangID == LadangID && w.fld_Deleted == false).FirstOrDefault();
            int CountBlok = dbr.tbl_Blok.Where(x => x.fld_KodPktutama == tbl_PktUtama.fld_PktUtama).Select(s => s.fld_Blok).Count();
            int CountSub = dbr.tbl_SubPkt.Where(x => x.fld_KodPktUtama == tbl_PktUtama.fld_PktUtama).Select(s => s.fld_Pkt).Count();
            if (CountBlok > 0)
            {
                DeleteFlag = 3;
            }
            else
            {
                if (CountSub > 0)
                {
                    DeleteFlag = 2;
                }
                else
                {
                    DeleteFlag = 1;
                }
            }

            ViewBag.DeleteFlag = DeleteFlag;
            return PartialView(tbl_PktUtama);
        }

        [HttpPost, ActionName("LevelsPktDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult LevelsPktDeleteConfirmed(string id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                Models.tbl_PktUtama tbl_PktUtama = dbr.tbl_PktUtama.Where(w => w.fld_PktUtama == id && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID && w.fld_LadangID == LadangID && w.fld_Deleted == false).FirstOrDefault();
                if (tbl_PktUtama == null)
                {
                    return Json(new { success = true, msg = GlobalResEstate.msgDelete2, status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
                }
                else
                {
                    tbl_PktUtama.fld_Deleted = true;
                    dbr.Entry(tbl_PktUtama).State = EntityState.Modified;
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

        public ActionResult LevelsSubPktCreate()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> PktUtama = new List<SelectListItem>();
            List<SelectListItem> TahapKesukaranMenuai = new List<SelectListItem>();
            List<SelectListItem> TahapKesukaranMembaja = new List<SelectListItem>();

            PktUtama = new SelectList(dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_JnsLot == "E").OrderBy(o => o.fld_PktUtama).Select(s => new SelectListItem { Value = s.fld_PktUtama, Text = s.fld_PktUtama + " - " + s.fld_NamaPktUtama + " - (" + s.fld_IOcode + ")" }).Distinct(), "Value", "Text").ToList();
            PktUtama.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            TahapKesukaranMenuai = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMenuai" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text").ToList();
            //TahapKesukaranMenuai.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            TahapKesukaranMembaja = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMembaja" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text").ToList();
            //TahapKesukaranMembaja.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            ViewBag.fld_KodPktUtama = PktUtama;
            ViewBag.fld_KesukaranMenuaiPkt = TahapKesukaranMenuai;
            ViewBag.fld_KesukaranMembajaPkt = TahapKesukaranMembaja;
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LevelsSubPktCreate(Models.tbl_SubPkt tbl_SubPkt)
        {
            string kodpkt = tbl_SubPkt.fld_Pkt;
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            //MVC_SYSTEM_Models dbr2 = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            if (ModelState.IsValid)
            {
                try
                {
                    var checkdata = dbr.tbl_SubPkt.Where(x => x.fld_Pkt == tbl_SubPkt.fld_Pkt && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).FirstOrDefault();
                    if (checkdata == null)
                    {
                        tbl_SubPkt.fld_NamaPkt = tbl_SubPkt.fld_NamaPkt.ToUpper();
                        tbl_SubPkt.fld_LadangID = LadangID;
                        tbl_SubPkt.fld_WilayahID = WilayahID;
                        tbl_SubPkt.fld_SyarikatID = SyarikatID;
                        tbl_SubPkt.fld_NegaraID = NegaraID;
                        tbl_SubPkt.fld_CreateDate = DateTime.Now;
                        tbl_SubPkt.fld_Deleted = false;
                        dbr.tbl_SubPkt.Add(tbl_SubPkt);
                        dbr.SaveChanges();

                        string RequestForm = Request.Form["listCount2"];
                        if (RequestForm != null && RequestForm != "")
                        {
                            int listCount = Convert.ToInt32(Request.Form["listCount2"]);
                            for (int i = 1; i <= listCount; i++)
                            {
                                string idKaw = "ddlPkt" + i;
                                string idLuas = "textluasPkt" + i;
                                string JnsKaw = Request.Form[idKaw];
                                decimal LuasKaw = Convert.ToDecimal(Request.Form[idLuas]);
                                var checkKwsn = dbr.tbl_KawTidakBerhasil.Where(x => x.fld_JnsKaw == JnsKaw && x.fld_KodPkt == kodpkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
                                if (checkKwsn == null)
                                {
                                    Models.tbl_KawTidakBerhasil KawTidakBerhasil = new Models.tbl_KawTidakBerhasil();
                                    KawTidakBerhasil.fld_KodPkt = kodpkt;
                                    KawTidakBerhasil.fld_LevelPkt = 2;
                                    KawTidakBerhasil.fld_JnsKaw = JnsKaw;
                                    KawTidakBerhasil.fld_LuasKaw = LuasKaw;
                                    KawTidakBerhasil.fld_NegaraID = NegaraID;
                                    KawTidakBerhasil.fld_SyarikatID = SyarikatID;
                                    KawTidakBerhasil.fld_WilayahID = WilayahID;
                                    KawTidakBerhasil.fld_LadangID = LadangID;
                                    KawTidakBerhasil.fld_Deleted = false;
                                    dbr.tbl_KawTidakBerhasil.Add(KawTidakBerhasil);
                                    dbr.SaveChanges();
                                }
                            }
                        }
                        return Json(new { success = true, msg = GlobalResEstate.msgAdd, status = "success", checkingdata = "0", method = "2", btn = "btnSrch" });
                    }
                    else
                    {
                        return Json(new { success = false, msg = GlobalResEstate.msgDataExist, status = "warning", checkingdata = "1" });
                    }

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

        public ActionResult LevelsSubPktUpdate(string id)
        {
            GetStatus GetStatus = new GetStatus();
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            Models.tbl_SubPkt tbl_SubPkt = dbr.tbl_SubPkt.Where(w => w.fld_Pkt == id && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID && w.fld_LadangID == LadangID && w.fld_Deleted == false).FirstOrDefault();

            List<SelectListItem> PktUtama = new List<SelectListItem>();
            List<SelectListItem> TahapKesukaranMenuai = new List<SelectListItem>();
            List<SelectListItem> TahapKesukaranMembaja = new List<SelectListItem>();

            PktUtama = new SelectList(dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_PktUtama).Select(s => new SelectListItem { Value = s.fld_PktUtama, Text = s.fld_PktUtama + " - " + s.fld_NamaPktUtama }).Distinct(), "Value", "Text").ToList();
            PktUtama.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            TahapKesukaranMenuai = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMenuai" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text").ToList();
            //TahapKesukaranMenuai.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            TahapKesukaranMembaja = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMembaja" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text").ToList();
            //TahapKesukaranMembaja.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            List<SelectListItem> Kawasanlist = new List<SelectListItem>();
            Kawasanlist = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsKawasan" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            Kawasanlist.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            ViewBag.fld_KodPktUtama = PktUtama;
            ViewBag.fld_KesukaranMenuaiPkt = TahapKesukaranMenuai;
            ViewBag.fld_KesukaranMembajaPkt = TahapKesukaranMembaja;
            ViewBag.fld_JnsKaw = Kawasanlist;
            return PartialView(tbl_SubPkt);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LevelsSubPktUpdate(string id, Models.tbl_SubPkt tbl_SubPkt)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                var getdata = dbr.tbl_SubPkt.Where(w => w.fld_Pkt == id && w.fld_LadangID == LadangID && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID && w.fld_Deleted == false).FirstOrDefault();
                getdata.fld_NamaPkt = tbl_SubPkt.fld_NamaPkt.ToUpper();
                getdata.fld_KesukaranMenuaiPkt = tbl_SubPkt.fld_KesukaranMenuaiPkt;
                getdata.fld_KesukaranMembajaPkt = tbl_SubPkt.fld_KesukaranMembajaPkt;
                getdata.fld_LsPkt = tbl_SubPkt.fld_LsPkt;
                getdata.fld_LuasKawTnmanPkt = tbl_SubPkt.fld_LuasKawTnmanPkt;
                getdata.fld_LuasBerhasilPkt = tbl_SubPkt.fld_LuasBerhasilPkt;
                getdata.fld_LuasBlmBerhasilPkt = tbl_SubPkt.fld_LuasBlmBerhasilPkt;
                getdata.fld_BilPokokPkt = tbl_SubPkt.fld_BilPokokPkt;
                getdata.fld_DirianPokokPkt = tbl_SubPkt.fld_DirianPokokPkt;
                getdata.fld_LuasKawTiadaTanamanPkt = tbl_SubPkt.fld_LuasKawTiadaTanamanPkt;
                dbr.Entry(getdata).State = EntityState.Modified;
                dbr.SaveChanges();

                string RequestForm = Request.Form["listCount2"];
                if (RequestForm != null && RequestForm != "")
                {
                    var getLuas = dbr.tbl_KawTidakBerhasil.Where(x => x.fld_KodPkt == tbl_SubPkt.fld_Pkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).AsEnumerable();
                    dbr.tbl_KawTidakBerhasil.RemoveRange(getLuas);
                    dbr.SaveChanges();

                    int listCount = Convert.ToInt32(Request.Form["listCount2"]);
                    for (int i = 1; i <= listCount; i++)
                    {
                        string idKaw = "ddlPkt" + i;
                        string idLuas = "textluasPkt" + i;
                        string JnsKaw = Request.Form[idKaw];
                        decimal LuasKaw = Convert.ToDecimal(Request.Form[idLuas]);
                        var checkKwsn = dbr.tbl_KawTidakBerhasil.Where(x => x.fld_JnsKaw == JnsKaw && x.fld_KodPkt == tbl_SubPkt.fld_Pkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
                        if (checkKwsn == null)
                        {
                            Models.tbl_KawTidakBerhasil KawTidakBerhasil = new Models.tbl_KawTidakBerhasil();
                            KawTidakBerhasil.fld_KodPkt = tbl_SubPkt.fld_Pkt;
                            KawTidakBerhasil.fld_LevelPkt = 2;
                            KawTidakBerhasil.fld_JnsKaw = JnsKaw;
                            KawTidakBerhasil.fld_LuasKaw = LuasKaw;
                            KawTidakBerhasil.fld_NegaraID = NegaraID;
                            KawTidakBerhasil.fld_SyarikatID = SyarikatID;
                            KawTidakBerhasil.fld_WilayahID = WilayahID;
                            KawTidakBerhasil.fld_LadangID = LadangID;
                            KawTidakBerhasil.fld_Deleted = false;
                            dbr.tbl_KawTidakBerhasil.Add(KawTidakBerhasil);
                            dbr.SaveChanges();
                        }
                    }
                }

                var getid = id;
                return Json(new { success = true, msg = GlobalResEstate.msgUpdate, status = "success", checkingdata = "0", method = "2", btn = "btnSrch" });
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = true, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }

        public ActionResult LevelsSubPktDelete(string id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int DeleteFlag = 1;
            Models.tbl_SubPkt tbl_SubPkt = dbr.tbl_SubPkt.Where(w => w.fld_Pkt == id && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID && w.fld_LadangID == LadangID && w.fld_Deleted == false).FirstOrDefault();
            int CountBlok = dbr.tbl_Blok.Where(x => x.fld_KodPkt == tbl_SubPkt.fld_Pkt).Select(s => s.fld_Blok).Count();
            //int CountSub = dbr.tbl_SubPkt.Where(x => x.fld_KodPktUtama == tbl_PktUtama.fld_PktUtama).Select(s => s.fld_Pkt).Count();
            if (CountBlok > 0)
            {
                DeleteFlag = 2;
            }
            else
            {
                DeleteFlag = 1;
            }

            ViewBag.DeleteFlag = DeleteFlag;
            return PartialView(tbl_SubPkt);
        }

        [HttpPost, ActionName("LevelsSubPktDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult LevelsSubPktDeleteConfirmed(string id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                Models.tbl_SubPkt tbl_SubPkt = dbr.tbl_SubPkt.Where(w => w.fld_Pkt == id && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID && w.fld_LadangID == LadangID && w.fld_Deleted == false).FirstOrDefault();
                if (tbl_SubPkt == null)
                {
                    return Json(new { success = true, msg = GlobalResEstate.msgDelete2, status = "success", checkingdata = "0", method = "2", btn = "btnSrch" });
                }
                else
                {
                    tbl_SubPkt.fld_Deleted = true;
                    dbr.Entry(tbl_SubPkt).State = EntityState.Modified;
                    dbr.SaveChanges();
                    return Json(new { success = true, msg = GlobalResEstate.msgDelete2, status = "success", checkingdata = "0", method = "2", btn = "btnSrch" });
                }

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }

        }

        public ActionResult LevelsSubPktUpdateKwsn(string Kodpkt = "", int page = 1, string sort = "fld_JnsKaw", string sortdir = "ASC")
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.tbl_KawTidakBerhasil>();
            records.Content = dbview.tbl_KawTidakBerhasil.Where(x => x.fld_KodPkt == Kodpkt && x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == false)
                   .OrderBy(sort + " " + sortdir)
                   .Skip((page - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();

            records.TotalRecords = dbview.tbl_KawTidakBerhasil.Where(x => x.fld_KodPkt == Kodpkt && x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == false).Count();
            records.CurrentPage = page;
            records.PageSize = pageSize;
            return View(records);
        }

        public ActionResult LevelsBlokCreate()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> PktUtama = new List<SelectListItem>();
            List<SelectListItem> Pkt = new List<SelectListItem>();
            //List<SelectListItem> JnsKesukaran = new List<SelectListItem>();
            List<SelectListItem> KesukaranMenuai = new List<SelectListItem>();
            List<SelectListItem> KesukaranMembaja = new List<SelectListItem>();

            PktUtama = new SelectList(dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_PktUtama).Select(s => new SelectListItem { Value = s.fld_PktUtama, Text = s.fld_PktUtama + " - " + s.fld_NamaPktUtama }).Distinct(), "Value", "Text").ToList();
            PktUtama.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            Pkt = new SelectList(dbr.tbl_SubPkt.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_Pkt).Select(s => new SelectListItem { Value = s.fld_Pkt, Text = s.fld_Pkt + " - " + s.fld_NamaPkt }).Distinct(), "Value", "Text").ToList();
            Pkt.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            KesukaranMenuai = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMenuai" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text").ToList();
            //KesukaranMenuai.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            KesukaranMembaja = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMembaja" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text").ToList();
            //KesukaranMembaja.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            ViewBag.fld_KodPktutama = PktUtama;
            ViewBag.fld_KodPkt = Pkt;
            ViewBag.fld_KesukaranMenuaiBlok = KesukaranMenuai;
            ViewBag.fld_KesukaranMembajaBlok = KesukaranMembaja;
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LevelsBlokCreate(Models.tbl_Blok tbl_Blok)
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
                    var checkdata = dbr.tbl_Blok.Where(x => x.fld_Blok == tbl_Blok.fld_Blok && x.fld_KodPkt == tbl_Blok.fld_KodPkt && x.fld_KodPktutama == tbl_Blok.fld_KodPktutama && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).FirstOrDefault();
                    if (checkdata == null)
                    {
                        //GetLadang GetLadang = new GetLadang();
                        //string ldgcode = GetLadang.GetCodeLadangFromID2(LadangID.Value);
                        tbl_Blok.fld_NamaBlok = tbl_Blok.fld_NamaBlok.ToUpper();
                        tbl_Blok.fld_LadangID = LadangID;
                        tbl_Blok.fld_WilayahID = WilayahID;
                        tbl_Blok.fld_SyarikatID = SyarikatID;
                        tbl_Blok.fld_NegaraID = NegaraID;
                        tbl_Blok.fld_CreateDate = DateTime.Now;
                        tbl_Blok.fld_Deleted = false;
                        dbr.tbl_Blok.Add(tbl_Blok);
                        dbr.SaveChanges();

                        string RequestForm = Request.Form["listCount3"];
                        if (RequestForm != null && RequestForm != "")
                        {
                            int listCount = Convert.ToInt32(Request.Form["listCount3"]);
                            for (int i = 1; i <= listCount; i++)
                            {
                                string idKaw = "ddlBlock" + i;
                                string idLuas = "textluasBlock" + i;
                                string JnsKaw = Request.Form[idKaw];
                                decimal LuasKaw = Convert.ToDecimal(Request.Form[idLuas]);
                                var checkKwsn = dbr.tbl_KawTidakBerhasil.Where(x => x.fld_JnsKaw == JnsKaw && x.fld_KodPkt == tbl_Blok.fld_Blok && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
                                if (checkKwsn == null)
                                {
                                    Models.tbl_KawTidakBerhasil KawTidakBerhasil = new Models.tbl_KawTidakBerhasil();
                                    KawTidakBerhasil.fld_KodPkt = tbl_Blok.fld_Blok;
                                    KawTidakBerhasil.fld_LevelPkt = 1;
                                    KawTidakBerhasil.fld_JnsKaw = JnsKaw;
                                    KawTidakBerhasil.fld_LuasKaw = LuasKaw;
                                    KawTidakBerhasil.fld_NegaraID = NegaraID;
                                    KawTidakBerhasil.fld_SyarikatID = SyarikatID;
                                    KawTidakBerhasil.fld_WilayahID = WilayahID;
                                    KawTidakBerhasil.fld_LadangID = LadangID;
                                    KawTidakBerhasil.fld_Deleted = false;
                                    dbr.tbl_KawTidakBerhasil.Add(KawTidakBerhasil);
                                    dbr.SaveChanges();
                                }
                            }
                        }

                        //dbr.SaveChanges();
                        //var getid = "";
                        //return Json(new { success = true, msg = "Data successfully added.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = "", data2 = "", data3 = "" });
                        return Json(new { success = true, msg = GlobalResEstate.msgAdd, status = "success", checkingdata = "0", method = "2", btn = "btnSrch" });
                    }
                    else
                    {
                        //return Json(new { success = true, msg = "Data already exist.", status = "warning", checkingdata = "1" });
                        return Json(new { success = false, msg = GlobalResEstate.msgDataExist, status = "warning", checkingdata = "1" });
                    }

                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
                }
            }
            else
            {
                return Json(new { success = false, msg = GlobalResEstate.msgErrorData, status = "warning", checkingdata = "1" });
            }
        }

        public ActionResult LevelsBlokUpdate(string id)
        {
            GetStatus GetStatus = new GetStatus();
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            Models.tbl_Blok tbl_Blok = dbr.tbl_Blok.Where(w => w.fld_Blok == id && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID && w.fld_LadangID == LadangID && w.fld_Deleted == false).FirstOrDefault();

            List<SelectListItem> PktUtama = new List<SelectListItem>();
            List<SelectListItem> Pkt = new List<SelectListItem>();
            //List<SelectListItem> JnsKesukaran = new List<SelectListItem>();
            List<SelectListItem> KesukaranMenuai = new List<SelectListItem>();
            List<SelectListItem> KesukaranMembaja = new List<SelectListItem>();

            PktUtama = new SelectList(dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_PktUtama).Select(s => new SelectListItem { Value = s.fld_PktUtama, Text = s.fld_PktUtama + " - " + s.fld_NamaPktUtama }).Distinct(), "Value", "Text").ToList();
            PktUtama.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            Pkt = new SelectList(dbr.tbl_SubPkt.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_Pkt).Select(s => new SelectListItem { Value = s.fld_Pkt, Text = s.fld_Pkt + " - " + s.fld_NamaPkt }).Distinct(), "Value", "Text").ToList();
            Pkt.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            KesukaranMenuai = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMenuai" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text").ToList();
            //KesukaranMenuai.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            KesukaranMembaja = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMembaja" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text").ToList();
            //KesukaranMembaja.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            List<SelectListItem> Kawasanlist = new List<SelectListItem>();
            Kawasanlist = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsKawasan" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            Kawasanlist.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            ViewBag.fld_KodPktutama = PktUtama;
            ViewBag.fld_KodPkt = Pkt;
            ViewBag.fld_KesukaranMenuaiBlok = KesukaranMenuai;
            ViewBag.fld_KesukaranMembajaBlok = KesukaranMembaja;
            ViewBag.fld_JnsKaw3 = Kawasanlist;
            return PartialView(tbl_Blok);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LevelsBlokUpdate(string id, Models.tbl_Blok tbl_Blok)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                var getdata = dbr.tbl_Blok.Where(w => w.fld_Blok == id && w.fld_LadangID == LadangID && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID && w.fld_Deleted == false).FirstOrDefault();
                getdata.fld_NamaBlok = tbl_Blok.fld_NamaBlok.ToUpper();
                getdata.fld_KesukaranMenuaiBlok = tbl_Blok.fld_KesukaranMenuaiBlok;
                getdata.fld_KesukaranMembajaBlok = tbl_Blok.fld_KesukaranMembajaBlok;
                getdata.fld_LsBlok = tbl_Blok.fld_LsBlok;
                getdata.fld_LuasKawTnmanBlok = tbl_Blok.fld_LuasKawTnmanBlok;
                getdata.fld_LuasBerhasilBlok = tbl_Blok.fld_LuasBerhasilBlok;
                getdata.fld_LuasBlmBerhasilBlok = tbl_Blok.fld_LuasBlmBerhasilBlok;
                getdata.fld_BilPokokBlok = tbl_Blok.fld_BilPokokBlok;
                getdata.fld_DirianPokokBlok = tbl_Blok.fld_DirianPokokBlok;
                getdata.fld_LuasKawTiadaTanamanBlok = tbl_Blok.fld_LuasKawTiadaTanamanBlok;
                dbr.Entry(getdata).State = EntityState.Modified;
                dbr.SaveChanges();

                string RequestForm = Request.Form["listCount3"];
                if (RequestForm != null && RequestForm != "")
                {
                    var getLuas = dbr.tbl_KawTidakBerhasil.Where(x => x.fld_KodPkt == tbl_Blok.fld_Blok && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).AsEnumerable();
                    dbr.tbl_KawTidakBerhasil.RemoveRange(getLuas);
                    dbr.SaveChanges();

                    int listCount = Convert.ToInt32(Request.Form["listCount3"]);
                    for (int i = 1; i <= listCount; i++)
                    {
                        string idKaw = "ddlBlock" + i;
                        string idLuas = "textluasBlock" + i;
                        string JnsKaw = Request.Form[idKaw];
                        decimal LuasKaw = Convert.ToDecimal(Request.Form[idLuas]);
                        var checkKwsn = dbr.tbl_KawTidakBerhasil.Where(x => x.fld_JnsKaw == JnsKaw && x.fld_KodPkt == tbl_Blok.fld_Blok && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
                        if (checkKwsn == null)
                        {
                            Models.tbl_KawTidakBerhasil KawTidakBerhasil = new Models.tbl_KawTidakBerhasil();
                            KawTidakBerhasil.fld_KodPkt = tbl_Blok.fld_Blok;
                            KawTidakBerhasil.fld_LevelPkt = 1;
                            KawTidakBerhasil.fld_JnsKaw = JnsKaw;
                            KawTidakBerhasil.fld_LuasKaw = LuasKaw;
                            KawTidakBerhasil.fld_NegaraID = NegaraID;
                            KawTidakBerhasil.fld_SyarikatID = SyarikatID;
                            KawTidakBerhasil.fld_WilayahID = WilayahID;
                            KawTidakBerhasil.fld_LadangID = LadangID;
                            KawTidakBerhasil.fld_Deleted = false;
                            dbr.tbl_KawTidakBerhasil.Add(KawTidakBerhasil);
                            dbr.SaveChanges();
                        }
                    }
                }

                var getid = id;
                return Json(new { success = true, msg = GlobalResEstate.msgUpdate, status = "success", checkingdata = "0", method = "2", btn = "btnSrch" });
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }

        public ActionResult LevelsBlokDelete(string id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            Models.tbl_Blok tbl_Blok = dbr.tbl_Blok.Where(w => w.fld_Blok == id && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID && w.fld_LadangID == LadangID && w.fld_Deleted == false).FirstOrDefault();
            return PartialView(tbl_Blok);
        }

        [HttpPost, ActionName("LevelsBlokDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult LevelsBlokDeleteConfirmed(string id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                Models.tbl_Blok tbl_Blok = dbr.tbl_Blok.Where(w => w.fld_Blok == id && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID && w.fld_LadangID == LadangID && w.fld_Deleted == false).FirstOrDefault();
                if (tbl_Blok == null)
                {
                    return Json(new { success = true, msg = GlobalResEstate.msgDelete2, status = "success", checkingdata = "0", method = "2", btn = "btnSrch" });
                }
                else
                {
                    tbl_Blok.fld_Deleted = true;
                    dbr.Entry(tbl_Blok).State = EntityState.Modified;
                    dbr.SaveChanges();
                    return Json(new { success = true, msg = GlobalResEstate.msgDelete2, status = "success", checkingdata = "0", method = "2", btn = "btnSrch" });
                }

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }

        //public ActionResult LevelsMain()
        //{

        //    //string IOcode = Request.Form["IO_code"];
        //    //string IOlevel = Request.Form["RadioGroup"];
        //    //string IOref = Request.Form["IO_reff"];
        //    //string IO_code = IOcode;
        //    //var activeTab = "tab1primary";
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<SelectListItem> JnsTnmn = new List<SelectListItem>();
        //    List<SelectListItem> StatusTnmn = new List<SelectListItem>();
        //    List<SelectListItem> TahapKesukaranMenuai = new List<SelectListItem>();
        //    List<SelectListItem> TahapKesukaranMembaja = new List<SelectListItem>();

        //    JnsTnmn = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "jnsTanaman" && x.fldDeleted == false).OrderBy(o => o.fldOptConfID).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }), "Value", "Text").ToList();
        //    JnsTnmn.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
        //    StatusTnmn = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "statusTanaman" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }), "Value", "Text").ToList();
        //    StatusTnmn.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

        //    TahapKesukaranMenuai = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMenuai" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text").ToList();
        //    //TahapKesukaranMenuai.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
        //    TahapKesukaranMembaja = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMembaja" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text").ToList();
        //    //TahapKesukaranMembaja.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

        //    ViewBag.fld_JnsTnmn = JnsTnmn;
        //    ViewBag.fld_StatusTnmn = StatusTnmn;
        //    ViewBag.fld_KesukaranMenuaiPktUtama = TahapKesukaranMenuai;
        //    ViewBag.fld_KesukaranMembajaPktUtama = TahapKesukaranMembaja;
        //    return PartialView();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult LevelsMain(Models.tbl_PktUtama tbl_PktUtama)
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var checkdata = dbr.tbl_PktUtama.Where(x => x.fld_PktUtama == tbl_PktUtama.fld_PktUtama && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).FirstOrDefault();
        //            if (checkdata == null)
        //            {
        //                tbl_PktUtama.fld_NamaPktUtama = tbl_PktUtama.fld_NamaPktUtama.ToUpper();
        //                tbl_PktUtama.fld_LadangID = LadangID;
        //                tbl_PktUtama.fld_WilayahID = WilayahID;
        //                tbl_PktUtama.fld_SyarikatID = SyarikatID;
        //                tbl_PktUtama.fld_NegaraID = NegaraID;
        //                tbl_PktUtama.fld_CreateDate = DateTime.Now;
        //                tbl_PktUtama.fld_Deleted = false;
        //                dbr.tbl_PktUtama.Add(tbl_PktUtama);
        //                //dbr.SaveChanges();

        //                var checkIo = dbr.tbl_IO.Where(x => x.fld_IOcode == tbl_PktUtama.fld_IOcode && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
        //                checkIo.fld_Deleted = true;
        //                dbr.Entry(checkIo).State = EntityState.Modified;
        //                //dbr.SaveChanges();

        //                int listCount = Convert.ToInt32(Request.Form["listCount"]);
        //                for (int i = 1; i <= listCount; i++)
        //                {
        //                    string idKaw = "ddl" + i;
        //                    string idLuas = "textluas" + i;
        //                    string JnsKaw = Request.Form[idKaw];
        //                    decimal LuasKaw = Convert.ToDecimal(Request.Form[idLuas]);
        //                    var checkKwsn = dbr.tbl_KawTidakBerhasil.Where(x => x.fld_JnsKaw == JnsKaw && x.fld_KodPkt == tbl_PktUtama.fld_PktUtama && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
        //                    if (checkKwsn==null)
        //                    {
        //                        Models.tbl_KawTidakBerhasil KawTidakBerhasil = new tbl_KawTidakBerhasil();
        //                        KawTidakBerhasil.fld_KodPkt = tbl_PktUtama.fld_PktUtama;
        //                        KawTidakBerhasil.fld_LevelPkt = 1;
        //                        KawTidakBerhasil.fld_JnsKaw = JnsKaw;
        //                        KawTidakBerhasil.fld_LuasKaw = LuasKaw;
        //                        KawTidakBerhasil.fld_NegaraID = NegaraID;
        //                        KawTidakBerhasil.fld_SyarikatID = SyarikatID;
        //                        KawTidakBerhasil.fld_WilayahID = WilayahID;
        //                        KawTidakBerhasil.fld_LadangID = LadangID;
        //                        KawTidakBerhasil.fld_Deleted = false;
        //                        dbr.tbl_KawTidakBerhasil.Add(KawTidakBerhasil);
        //                        dbr.SaveChanges();
        //                    }
        //                }

        //                //var getid = dbr.tbl_PktUtama.Where(x => x.fld_PktUtama == tbl_PktUtama.fld_PktUtama).FirstOrDefault();
        //                //return Json(new { success = true, msg = "Data successfully added.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = "", data2 = "", data3 = "" });


        //                return RedirectToAction("LevelsInfo", "BasicInfo");
        //            }
        //            else
        //            {
        //                //return Json(new { success = true, msg = "Data already exist.", status = "warning", checkingdata = "1" });
        //                return RedirectToAction("LevelsInfo", "BasicInfo");
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
        //            return Json(new { success = true, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
        //        }
        //    }
        //    else
        //    {
        //        return Json(new { success = true, msg = GlobalResEstate.msgErrorData, status = "warning", checkingdata = "1" });
        //    }
        //}

        //public ActionResult LevelsMainUpdate()
        //{
        //    return View();
        //}

        //public ActionResult LevelsSub()
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

        //    List<SelectListItem> PktUtama = new List<SelectListItem>();
        //    List<SelectListItem> TahapKesukaranMenuai = new List<SelectListItem>();
        //    List<SelectListItem> TahapKesukaranMembaja = new List<SelectListItem>();

        //    PktUtama = new SelectList(dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_PktUtama).Select(s => new SelectListItem { Value = s.fld_PktUtama, Text = s.fld_PktUtama + " - " + s.fld_NamaPktUtama }).Distinct(), "Value", "Text").ToList();
        //    PktUtama.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

        //    TahapKesukaranMenuai = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMenuai" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text").ToList();
        //    //TahapKesukaranMenuai.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

        //    TahapKesukaranMembaja = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMembaja" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text").ToList();
        //    //TahapKesukaranMembaja.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

        //    ViewBag.fld_KodPktUtama = PktUtama;
        //    ViewBag.fld_KesukaranMenuaiPkt = TahapKesukaranMenuai;
        //    ViewBag.fld_KesukaranMembajaPkt = TahapKesukaranMembaja;
        //    return PartialView();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult LevelsSub(Models.tbl_SubPkt tbl_SubPkt)
        //{
        //    string kodpkt = tbl_SubPkt.fld_Pkt;
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
        //    //MVC_SYSTEM_Models dbr2 = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var checkdata = dbr.tbl_SubPkt.Where(x => x.fld_Pkt== tbl_SubPkt.fld_Pkt && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).FirstOrDefault();
        //            if (checkdata == null)
        //            {
        //                tbl_SubPkt.fld_NamaPkt = tbl_SubPkt.fld_NamaPkt.ToUpper();
        //                tbl_SubPkt.fld_LadangID = LadangID;
        //                tbl_SubPkt.fld_WilayahID = WilayahID;
        //                tbl_SubPkt.fld_SyarikatID = SyarikatID;
        //                tbl_SubPkt.fld_NegaraID = NegaraID;
        //                tbl_SubPkt.fld_CreateDate = DateTime.Now;
        //                tbl_SubPkt.fld_Deleted = false;
        //                dbr.tbl_SubPkt.Add(tbl_SubPkt);
        //                dbr.SaveChanges();

        //                int listCount = Convert.ToInt32(Request.Form["listCount2"]);
        //                for (int i = 1; i <= listCount; i++)
        //                {
        //                    string idKaw = "ddlPkt" + i;
        //                    string idLuas = "textluasPkt" + i;
        //                    string JnsKaw = Request.Form[idKaw];
        //                    decimal LuasKaw = Convert.ToDecimal(Request.Form[idLuas]);
        //                    var checkKwsn = dbr.tbl_KawTidakBerhasil.Where(x => x.fld_JnsKaw == JnsKaw && x.fld_KodPkt == kodpkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
        //                    if (checkKwsn == null)
        //                    {
        //                        Models.tbl_KawTidakBerhasil KawTidakBerhasil = new tbl_KawTidakBerhasil();
        //                        KawTidakBerhasil.fld_KodPkt = kodpkt;
        //                        KawTidakBerhasil.fld_LevelPkt = 2;
        //                        KawTidakBerhasil.fld_JnsKaw = JnsKaw;
        //                        KawTidakBerhasil.fld_LuasKaw = LuasKaw;
        //                        KawTidakBerhasil.fld_NegaraID = NegaraID;
        //                        KawTidakBerhasil.fld_SyarikatID = SyarikatID;
        //                        KawTidakBerhasil.fld_WilayahID = WilayahID;
        //                        KawTidakBerhasil.fld_LadangID = LadangID;
        //                        KawTidakBerhasil.fld_Deleted = false;
        //                        dbr.tbl_KawTidakBerhasil.Add(KawTidakBerhasil);
        //                        dbr.SaveChanges();
        //                    }
        //                }
        //                return RedirectToAction("LevelsInfo", "BasicInfo");
        //            }
        //            else
        //            {
        //                return RedirectToAction("LevelsInfo", "BasicInfo");
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
        //            return Json(new { success = true, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
        //        }
        //    }
        //    else
        //    {
        //        return Json(new { success = true, msg = GlobalResEstate.msgErrorData, status = "warning", checkingdata = "1" });
        //    }

        //}

        //public ActionResult LevelsBlock()   
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

        //    List<SelectListItem> PktUtama = new List<SelectListItem>();
        //    List<SelectListItem> Pkt = new List<SelectListItem>();
        //    //List<SelectListItem> JnsKesukaran = new List<SelectListItem>();
        //    List<SelectListItem> KesukaranMenuai = new List<SelectListItem>();
        //    List<SelectListItem> KesukaranMembaja = new List<SelectListItem>();

        //    PktUtama = new SelectList(dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_PktUtama).Select(s => new SelectListItem { Value = s.fld_PktUtama, Text = s.fld_PktUtama + " - " + s.fld_NamaPktUtama }).Distinct(), "Value", "Text").ToList();
        //    PktUtama.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
        //    Pkt = new SelectList(dbr.tbl_SubPkt.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_Pkt).Select(s => new SelectListItem { Value = s.fld_Pkt, Text = s.fld_Pkt + " - " + s.fld_NamaPkt }).Distinct(), "Value", "Text").ToList();
        //    Pkt.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
        //    KesukaranMenuai = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMenuai" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text").ToList();
        //    //KesukaranMenuai.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
        //    KesukaranMembaja = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMembaja" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text").ToList();
        //    //KesukaranMembaja.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

        //    ViewBag.fld_KodPktutama = PktUtama;
        //    ViewBag.fld_KodPkt = Pkt;
        //    ViewBag.fld_KesukaranMenuaiBlok = KesukaranMenuai;
        //    ViewBag.fld_KesukaranMembajaBlok = KesukaranMembaja;
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult LevelsBlock(Models.tbl_Blok tbl_Blok)
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var checkdata = dbr.tbl_Blok.Where(x =>x.fld_Blok==tbl_Blok.fld_Blok && x.fld_KodPkt==tbl_Blok.fld_KodPkt && x.fld_KodPktutama == tbl_Blok.fld_KodPktutama && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).FirstOrDefault();
        //            if (checkdata == null)
        //            {
        //                //GetLadang GetLadang = new GetLadang();
        //                //string ldgcode = GetLadang.GetCodeLadangFromID2(LadangID.Value);
        //                tbl_Blok.fld_NamaBlok = tbl_Blok.fld_NamaBlok.ToUpper();
        //                tbl_Blok.fld_LadangID = LadangID;
        //                tbl_Blok.fld_WilayahID = WilayahID;
        //                tbl_Blok.fld_SyarikatID = SyarikatID;
        //                tbl_Blok.fld_NegaraID = NegaraID;
        //                tbl_Blok.fld_CreateDate = DateTime.Now;
        //                tbl_Blok.fld_Deleted = false;
        //                dbr.tbl_Blok.Add(tbl_Blok);
        //                dbr.SaveChanges();

        //                int listCount = Convert.ToInt32(Request.Form["listCount3"]);
        //                for (int i = 1; i <= listCount; i++)
        //                {
        //                    string idKaw = "ddlBlock" + i;
        //                    string idLuas = "textluasBlock" + i;
        //                    string JnsKaw = Request.Form[idKaw];
        //                    decimal LuasKaw = Convert.ToDecimal(Request.Form[idLuas]);
        //                    var checkKwsn = dbr.tbl_KawTidakBerhasil.Where(x => x.fld_JnsKaw == JnsKaw && x.fld_KodPkt == tbl_Blok.fld_Blok && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
        //                    if (checkKwsn == null)
        //                    {
        //                        Models.tbl_KawTidakBerhasil KawTidakBerhasil = new tbl_KawTidakBerhasil();
        //                        KawTidakBerhasil.fld_KodPkt = tbl_Blok.fld_Blok;
        //                        KawTidakBerhasil.fld_LevelPkt = 1;
        //                        KawTidakBerhasil.fld_JnsKaw = JnsKaw;
        //                        KawTidakBerhasil.fld_LuasKaw = LuasKaw;
        //                        KawTidakBerhasil.fld_NegaraID = NegaraID;
        //                        KawTidakBerhasil.fld_SyarikatID = SyarikatID;
        //                        KawTidakBerhasil.fld_WilayahID = WilayahID;
        //                        KawTidakBerhasil.fld_LadangID = LadangID;
        //                        KawTidakBerhasil.fld_Deleted = false;
        //                        dbr.tbl_KawTidakBerhasil.Add(KawTidakBerhasil);
        //                        dbr.SaveChanges();
        //                    }
        //                }
        //                //dbr.SaveChanges();
        //                //var getid = "";
        //                //return Json(new { success = true, msg = "Data successfully added.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = "", data2 = "", data3 = "" });
        //                return RedirectToAction("LevelsInfo", "BasicInfo");
        //            }
        //            else
        //            {
        //                //return Json(new { success = true, msg = "Data already exist.", status = "warning", checkingdata = "1" });
        //                return RedirectToAction("LevelsInfo", "BasicInfo");
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
        //            return Json(new { success = true, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
        //        }
        //    }
        //    else
        //    {
        //        return Json(new { success = true, msg = GlobalResEstate.msgErrorData, status = "warning", checkingdata = "1" });
        //    }
        //}

        //public ActionResult LevelsChange()
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

        //    List<SelectListItem> PktUtama1 = new List<SelectListItem>();
        //    List<SelectListItem> PktUtama2 = new List<SelectListItem>();

        //    PktUtama1 = new SelectList(dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_PktUtama).Select(s => new SelectListItem { Value = s.fld_PktUtama, Text = s.fld_PktUtama + " - " + s.fld_NamaPktUtama }).Distinct(), "Value", "Text").ToList();
        //    PktUtama1.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

        //    PktUtama2 = new SelectList(dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_PktUtama).Select(s => new SelectListItem { Value = s.fld_PktUtama, Text = s.fld_PktUtama + " - " + s.fld_NamaPktUtama }).Distinct(), "Value", "Text").ToList();
        //    PktUtama2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

        //    ViewBag.fld_PktUtama1 = PktUtama1;
        //    ViewBag.fld_PktUtama2 = PktUtama2;
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult LevelsChange(string fld_PktUtama1, string fld_PktUtama2)
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

        //    if (fld_PktUtama1 != "0" && fld_PktUtama2 != "0")
        //    {
        //        //x aktif
        //        try
        //        {
        //            var getdata = dbr.tbl_PktUtama.Where(w => w.fld_PktUtama == fld_PktUtama1 && w.fld_LadangID == LadangID && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID).FirstOrDefault();
        //            getdata.fld_Deleted = true;
        //            dbr.Entry(getdata).State = EntityState.Modified;
        //            dbr.SaveChanges();

        //            var getdatacount = dbr.tbl_PktUtama.Where(w => w.fld_RefKey == fld_PktUtama1 && w.fld_LadangID == LadangID && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID).Count();
        //            var getdata2 = dbr.tbl_PktUtama.Where(w => w.fld_PktUtama == fld_PktUtama2 && w.fld_LadangID == LadangID && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID).FirstOrDefault();
        //            getdata2.fld_RefKey = fld_PktUtama1;
        //            getdata2.fld_Level = getdatacount + 1;
        //            dbr.Entry(getdata2).State = EntityState.Modified;
        //            dbr.SaveChanges();
        //            //return Json(new { success = true, msg = "Data successfully edited.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
        //            return RedirectToAction("LevelsInfo", "BasicInfo");
        //        }
        //        catch (Exception ex)
        //        {
        //            geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
        //            return Json(new { success = true, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
        //        }
        //    }
        //    else
        //    {
        //        return Json(new { success = true, msg = GlobalResEstate.msgErrorSubmit, status = "danger", checkingdata = "1" });
        //    }

        //}

        public ActionResult WorkerInfo(string statusApprove = "0", int page = 1, string sort = "fld_Nama", string sortdir = "ASC", string filter = "")
        {
            GetIdentity GetIdentity = new GetIdentity();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> statusApprove2 = new List<SelectListItem>();

            statusApprove2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "apprvlPkj" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            statusApprove2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            ViewBag.BasicInfo = "class = active";
            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.tbl_Pkjmast>();

            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            int role = GetIdentity.RoleID(getuserid).Value;

            if (statusApprove == "0")
            {
                if (filter != "")
                {
                    records.Content = dbview.tbl_Pkjmast.Where(x => x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && (x.fld_Nama.Contains(filter) || x.fld_Nopkj.Contains(filter) || x.fld_Nokp.Contains(filter)))
                       .OrderBy(sort + " " + sortdir)
                       .Skip((page - 1) * pageSize)
                       .Take(pageSize)
                       .ToList();

                    records.TotalRecords = dbview.tbl_Pkjmast.Where(x => x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && (x.fld_Nama.Contains(filter) || x.fld_Nopkj.Contains(filter) || x.fld_Nokp.Contains(filter))).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
                else
                {
                    records.Content = dbview.tbl_Pkjmast.Where(x => x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID)
                       .OrderBy(sort + " " + sortdir)
                       .Skip((page - 1) * pageSize)
                       .Take(pageSize)
                       .ToList();

                    records.TotalRecords = dbview.tbl_Pkjmast.Where(x => x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
            }
            else
            {
                int status = Int32.Parse(statusApprove);
                records.Content = dbview.tbl_Pkjmast.Where(x => x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_StatusApproved == status)
                       .OrderBy(sort + " " + sortdir)
                       .Skip((page - 1) * pageSize)
                       .Take(pageSize)
                       .ToList();

                records.TotalRecords = dbview.tbl_Pkjmast.Where(x => x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_StatusApproved == status).Count();
                records.CurrentPage = page;
                records.PageSize = pageSize;
            }

            ViewBag.statusApprove = statusApprove2;
            ViewBag.RoleID = role;
            return View(records);
        }

        public ActionResult WorkerRequest()
        {
            //Check_Balik
            //string batch = Request.Form["CreateBatch"].ToString();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> Gender = new List<SelectListItem>();
            List<SelectListItem> statusKahwin = new List<SelectListItem>();
            List<SelectListItem> peneroka = new List<SelectListItem>();
            List<SelectListItem> bangsa = new List<SelectListItem>();
            List<SelectListItem> agama = new List<SelectListItem>();
            List<SelectListItem> krytn = new List<SelectListItem>();
            List<SelectListItem> negeri = new List<SelectListItem>();
            List<SelectListItem> jnsPkj = new List<SelectListItem>();
            List<SelectListItem> ktgrPkj = new List<SelectListItem>();
            List<SelectListItem> pembekal = new List<SelectListItem>();
            List<SelectListItem> division = new List<SelectListItem>();

            //List<SelectListItem> statusAktif = new List<SelectListItem>();
            //List<SelectListItem> banklist = new List<SelectListItem>();
            //List<SelectListItem> jnsKwsp = new List<SelectListItem>();

            Gender = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jantina" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            Gender.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            statusKahwin = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "tarafKahwin" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            statusKahwin.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            peneroka = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusPeneroka" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfID).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            peneroka.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            bangsa = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "bangsa" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            bangsa.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            agama = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "agama" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            agama.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            krytn = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            krytn.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            negeri = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "negeri" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            negeri.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            jnsPkj = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc.ToUpper() }).Distinct(), "Value", "Text").ToList();
            jnsPkj.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            ktgrPkj = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "designation" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            ktgrPkj.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            pembekal = new SelectList(db.tbl_Pembekal.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).OrderBy(o => o.fld_KodPbkl).Select(s => new SelectListItem { Value = s.fld_KodPbkl, Text = s.fld_NamaPbkl }).Distinct(), "Value", "Text").ToList();
            pembekal.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            division = new SelectList(db.tbl_Division.Where(x => x.fld_NegaraID==NegaraID && x.fld_SyarikatID==SyarikatID && x.fld_WilayahID==WilayahID && x.fld_LadangID==LadangID && x.fld_Deleted==false).OrderBy(o => o.fld_DivisionName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_DivisionName }).Distinct(), "Value", "Text").ToList();
            division.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            //statusAktif = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif2").OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            //statusAktif.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));

            //banklist = new SelectList(db.tbl_Bank.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).OrderBy(o => o.fld_KodBank).Select(s => new SelectListItem { Value = s.fld_KodBank, Text = s.fld_KodBank + " - " + s.fld_NamaBank }).Distinct(), "Value", "Text").ToList();
            //banklist.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));

            //jnsKwsp = new SelectList(db.tbl_JenisCaruman.Where(x =>x.fld_JenisCaruman== "KWSP" && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID && x.fld_Deleted == false).OrderBy(o => o.fld_KodCaruman).Select(s => new SelectListItem { Value = s.fld_KodCaruman, Text = s.fld_Keterangan}).Distinct(), "Value", "Text").ToList();
            //jnsKwsp.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));

            string requestCode = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_RequestCode).FirstOrDefault();
            //ViewBag.fld_Batch = fld_Batch;
            ViewBag.fld_Kdjnt = Gender;
            ViewBag.fld_Kdkwn = statusKahwin;
            ViewBag.fld_Kpenrka = peneroka;
            ViewBag.fld_Kdbgsa = bangsa;
            ViewBag.fld_Kdagma = agama;
            ViewBag.fld_Kdrkyt = krytn;
            ViewBag.fld_Neg = negeri;
            ViewBag.fld_Negara = krytn;
            ViewBag.fld_Jenispekerja = jnsPkj;
            ViewBag.fld_Ktgpkj = ktgrPkj;
            ViewBag.fld_Kodbkl = pembekal;
            ViewBag.LdgID = LadangID;
            ViewBag.requestCode = requestCode;
            ViewBag.fld_Negara2 = krytn;
            ViewBag.fld_DivisionID = division;
            //ViewBag.fld_Kdaktf = statusAktif;
            //ViewBag.fld_Kdbank = banklist;
            //ViewBag.fld_KodKWSP = jnsKwsp;
            ViewBag.Flag = 1;
            return View();
        }

        [HttpPost]
        public ActionResult WorkerRequest(Models.tbl_Pkjmast Pkjmast, string jnsPermohonan, int wlyhAsal, string pkjAsal)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string msg = "";
            string statusmsg = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            if (Pkjmast.fld_Jenispekerja == "AS")
            {
                //pekerja haram
                var checkdataAsing = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && (x.fld_Nopkj == Pkjmast.fld_Nopkj)).FirstOrDefault();
                if (checkdataAsing == null && Pkjmast.fld_Nopkj != null)
                {
                    Pkjmast.fld_Nama = Pkjmast.fld_Nama.ToUpper();
                    Pkjmast.fld_Daerah = Pkjmast.fld_Daerah.Trim();
                    Pkjmast.fld_IDpkj = Pkjmast.fld_Nopkj;
                    Pkjmast.fld_Ktgpkj = Pkjmast.fld_Ktgpkj.ToString().Trim();
                    Pkjmast.fld_DateApply = DateTime.Now;
                    Pkjmast.fld_StatusApproved = 2;
                    Pkjmast.fld_AppliedBy = User.Identity.Name;
                    Pkjmast.fld_NegaraID = NegaraID;
                    Pkjmast.fld_SyarikatID = SyarikatID;
                    Pkjmast.fld_WilayahID = WilayahID;
                    Pkjmast.fld_LadangID = LadangID;
                    Pkjmast.fld_Kdldg = GetLadang.GetLadangCode(LadangID.Value);
                    dbr.tbl_Pkjmast.Add(Pkjmast);
                    dbr.SaveChanges();

                    var sbbmasuk = db.tblPkjmastApps.Where(x => x.fldNoPkj == Pkjmast.fld_Nopkj && x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID).FirstOrDefault();
                    sbbmasuk.fldSbbMsk = jnsPermohonan;

                    if (jnsPermohonan == "PL")
                    {
                        string host2, catalog2, user2, pass2 = "";
                        var GetWilayahData = db.tbl_Wilayah.Find(wlyhAsal);
                        Connection.GetConnection(out host2, out catalog2, out user2, out pass2, GetWilayahData.fld_ID, GetWilayahData.fld_SyarikatID, GetWilayahData.fld_NegaraID);
                        MVC_SYSTEM_Models dbr2 = MVC_SYSTEM_Models.ConnectToSqlServer(host2, catalog2, user2, pass2);

                        var GetDataPkjAsal = dbr2.tbl_Pkjmast.Where(x => x.fld_Nopkj == pkjAsal).FirstOrDefault();

                        sbbmasuk.fldLadangAsal = GetDataPkjAsal.fld_LadangID;
                        sbbmasuk.fldWilayahAsal = GetDataPkjAsal.fld_WilayahID;
                        sbbmasuk.fldSyarikatAsal = GetDataPkjAsal.fld_SyarikatID;
                        sbbmasuk.fldNegaraAsal = GetDataPkjAsal.fld_NegaraID;
                        sbbmasuk.fldNoPkjAsal = pkjAsal.ToUpper();

                    }
                    db.SaveChanges();
                    msg = GlobalResEstate.msgAdd;
                    statusmsg = "success";
                }
                else
                {
                    ////*worker already exist
                    msg = GlobalResEstate.msgIcPassExits;
                    statusmsg = "warning";
                }
            }
            else
            {
                //pekerja suci
                var checkdata = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID==WilayahID && x.fld_LadangID==LadangID && (x.fld_Nopkj == Pkjmast.fld_Nopkj || x.fld_Nokp == Pkjmast.fld_Nokp)).FirstOrDefault();
                if (checkdata == null && Pkjmast.fld_Nopkj != null)
                {
                    Pkjmast.fld_Nama = Pkjmast.fld_Nama.ToUpper();
                    Pkjmast.fld_Daerah = Pkjmast.fld_Daerah.Trim();
                    Pkjmast.fld_IDpkj = Pkjmast.fld_Nopkj;
                    Pkjmast.fld_Ktgpkj = Pkjmast.fld_Ktgpkj.ToString().Trim();
                    Pkjmast.fld_DateApply = DateTime.Now;
                    Pkjmast.fld_StatusApproved = 2;
                    Pkjmast.fld_AppliedBy = User.Identity.Name;
                    Pkjmast.fld_NegaraID = NegaraID;
                    Pkjmast.fld_SyarikatID = SyarikatID;
                    Pkjmast.fld_WilayahID = WilayahID;
                    Pkjmast.fld_LadangID = LadangID;
                    Pkjmast.fld_Kdldg = GetLadang.GetLadangCode(LadangID.Value);
                    dbr.tbl_Pkjmast.Add(Pkjmast);
                    dbr.SaveChanges();

                    var sbbmasuk = db.tblPkjmastApps.Where(x => x.fldNoPkj == Pkjmast.fld_Nopkj && x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID).FirstOrDefault();
                    sbbmasuk.fldSbbMsk = jnsPermohonan;

                    if (jnsPermohonan == "PL")
                    {
                        string host2, catalog2, user2, pass2 = "";
                        var GetWilayahData = db.tbl_Wilayah.Find(wlyhAsal);
                        Connection.GetConnection(out host2, out catalog2, out user2, out pass2, GetWilayahData.fld_ID, GetWilayahData.fld_SyarikatID, GetWilayahData.fld_NegaraID);
                        MVC_SYSTEM_Models dbr2 = MVC_SYSTEM_Models.ConnectToSqlServer(host2, catalog2, user2, pass2);

                        var GetDataPkjAsal = dbr2.tbl_Pkjmast.Where(x => x.fld_Nopkj == pkjAsal).FirstOrDefault();

                        sbbmasuk.fldLadangAsal = GetDataPkjAsal.fld_LadangID;
                        sbbmasuk.fldWilayahAsal = GetDataPkjAsal.fld_WilayahID;
                        sbbmasuk.fldSyarikatAsal = GetDataPkjAsal.fld_SyarikatID;
                        sbbmasuk.fldNegaraAsal = GetDataPkjAsal.fld_NegaraID;
                        sbbmasuk.fldNoPkjAsal = pkjAsal.ToUpper();

                    }
                    db.SaveChanges();
                    msg = GlobalResEstate.msgAdd;
                    statusmsg = "success";
                }
                else
                {
                    msg = GlobalResEstate.msgIcPassExits;
                    statusmsg = "warning";
                }
            }


            var result = dbr.tbl_Pkjmast.Where(x => x.fld_Batch == Pkjmast.fld_Batch && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
            string bodyview = RenderRazorViewToString("WorkerRequestList", result);
            return Json(new { bodyview, msg, statusmsg });
            //return RedirectToAction("WorkerRequestWaris", "BasicInfo",new { nopkj=Pkjmast.fld_Nopkj });
        }

        public string RenderRazorViewToString(string viewname, object dataview)
        {
            ViewData.Model = dataview;
            using (var sw = new System.IO.StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewname);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        public ActionResult WorkerTransfer()
        {
            //Check_Balik
            //string batch = Request.Form["CreateBatch"].ToString();
            int[] wlyhid = new int[] { };
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> SyarikatIDList = new List<SelectListItem>();
            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> Gender = new List<SelectListItem>();
            List<SelectListItem> statusKahwin = new List<SelectListItem>();
            List<SelectListItem> peneroka = new List<SelectListItem>();
            List<SelectListItem> bangsa = new List<SelectListItem>();
            List<SelectListItem> agama = new List<SelectListItem>();
            List<SelectListItem> krytn = new List<SelectListItem>();
            List<SelectListItem> negeri = new List<SelectListItem>();
            List<SelectListItem> jnsPkj = new List<SelectListItem>();
            List<SelectListItem> ktgrPkj = new List<SelectListItem>();
            List<SelectListItem> pembekal = new List<SelectListItem>();
            List<SelectListItem> statusAktif = new List<SelectListItem>();
            //List<SelectListItem> banklist = new List<SelectListItem>();

            SyarikatIDList = new SelectList(db.tbl_Syarikat.Where(x => x.fld_NegaraID == NegaraID && x.fld_Deleted == false).OrderBy(o => o.fld_NamaSyarikat), "fld_SyarikatID", "fld_NamaSyarikat").ToList();
            SyarikatIDList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            //wlyhid = GetWilayah.GetWilayahID(SyarikatID);
            //WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
            WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            Gender = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jantina" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            Gender.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            statusKahwin = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "tarafKahwin" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            statusKahwin.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            peneroka = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusPeneroka" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfID).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            peneroka.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            bangsa = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "bangsa" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            bangsa.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            agama = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "agama" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            agama.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            krytn = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            krytn.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            negeri = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "negeri" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            negeri.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            jnsPkj = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc.ToUpper() }).Distinct(), "Value", "Text").ToList();
            jnsPkj.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            ktgrPkj = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "designation" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue.Trim(), Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            ktgrPkj.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            pembekal = new SelectList(db.tbl_Pembekal.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).OrderBy(o => o.fld_KodPbkl).Select(s => new SelectListItem { Value = s.fld_KodPbkl, Text = s.fld_NamaPbkl }).Distinct(), "Value", "Text").ToList();
            pembekal.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            statusAktif = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif2" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            //statusAktif.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));

            //banklist = new SelectList(db.tbl_Bank.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).OrderBy(o => o.fld_KodBank).Select(s => new SelectListItem { Value = s.fld_KodBank, Text = s.fld_KodBank + " - " + s.fld_NamaBank }).Distinct(), "Value", "Text").ToList();
            //banklist.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));

            //ViewBag.Batch = batch;
            ViewBag.fld_SyarikatID = SyarikatIDList;
            ViewBag.fld_WilayahID = WilayahIDList;
            ViewBag.fld_Kdjnt = Gender;
            ViewBag.fld_Kdkwn = statusKahwin;
            ViewBag.fld_Kpenrka = peneroka;
            ViewBag.fld_Kdbgsa = bangsa;
            ViewBag.fld_Kdagma = agama;
            ViewBag.fld_Kdrkyt = krytn;
            ViewBag.fld_Neg = negeri;
            ViewBag.fld_Negara = krytn;
            ViewBag.fld_Jenispekerja = jnsPkj;
            ViewBag.fld_Ktgpkj = ktgrPkj;
            ViewBag.fld_Kodbkl = pembekal;
            ViewBag.fld_Kdaktf = statusAktif;
            //ViewBag.fld_Kdbank = banklist;

            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult WorkerTransfer(Models.tbl_Pkjmast Pkjmast)
        //{
        //    GetLadang GetLadang = new GetLadang();
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
        //    //if (ModelState.IsValid)
        //    //{

        //    try
        //    {
        //        var checkdata = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == Pkjmast.fld_Nopkj).FirstOrDefault();
        //        if (checkdata == null)
        //        {

        //            //tbl_Ladang.fld_Deleted = false;
        //            Pkjmast.fld_Almt1 = Pkjmast.fld_Almt1.Trim();
        //            Pkjmast.fld_IDpkj = Pkjmast.fld_IDpkj;
        //            Pkjmast.fld_DateApply = DateTime.Now;
        //            Pkjmast.fld_StatusApproved =2;
        //            Pkjmast.fld_AppliedBy = Convert.ToString(getuserid);
        //            Pkjmast.fld_NegaraID = NegaraID;
        //            Pkjmast.fld_SyarikatID = SyarikatID;
        //            Pkjmast.fld_WilayahID = WilayahID;
        //            Pkjmast.fld_LadangID = LadangID;
        //            Pkjmast.fld_Kdldg = GetLadang.GetCodeLadangFromID2(LadangID.Value);
        //            dbr.tbl_Pkjmast.Add(Pkjmast);
        //            dbr.SaveChanges();

        //            var notes = db.tblPkjmastApps.Where(x => x.fldNoPkj == Pkjmast.fld_Nopkj && x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID && x.fldWilayahID == WilayahID && x.fldLadangID == LadangID).FirstOrDefault();
        //            notes.fldSbbMsk = "PL";
        //            db.SaveChanges();
        //            //var getid = db.tbl_Ladang.Where(w => w.fld_ID == tbl_Ladang.fld_ID).FirstOrDefault();
        //            return Json(new { success = true, msg = "Data successfully added.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "", data3 = "" });
        //        }
        //        else
        //        {
        //            return Json(new { success = true, msg = "Data already exist.", status = "warning", checkingdata = "1" });
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
        //        return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
        //    }
        //    //}
        //    //else
        //    //{
        //    //    return Json(new { success = true, msg = "Please check fill you inserted.", status = "warning", checkingdata = "1" });
        //    //    //return RedirectToAction("Thankyou");
        //    //}
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult GetWorkerDetail(string PkjTransfer, int? fld_WilayahID)
        //{
        //    Models.tbl_Pkjmast tbl_Pkjmast = db.tbl_Pkjmast.Where(w => w.fld_Nopkj == PkjTransfer && w.fld_WilayahID == fld_WilayahID).FirstOrDefault();

        //    return PartialView("WorkerTransfer", tbl_Pkjmast);
        //}

        public ActionResult WorkerUpdate(string id)
        {
            GetStatus GetStatus = new GetStatus();
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            if (id == null)
            {
                return RedirectToAction("WorkerInfo");
            }
            Models.tbl_Pkjmast tbl_Pkjmast = dbr.tbl_Pkjmast.Where(w => w.fld_Nopkj == id && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID).FirstOrDefault();
            string statuspkj = GetStatus.GetWorkerStatus(tbl_Pkjmast.fld_Kdaktf);
            if (tbl_Pkjmast == null)
            {
                return RedirectToAction("WorkerInfo");
            }

            List<SelectListItem> Gender = new List<SelectListItem>();
            List<SelectListItem> statusKahwin = new List<SelectListItem>();
            List<SelectListItem> peneroka = new List<SelectListItem>();
            List<SelectListItem> bangsa = new List<SelectListItem>();
            List<SelectListItem> agama = new List<SelectListItem>();
            List<SelectListItem> krytn = new List<SelectListItem>();
            List<SelectListItem> negeri = new List<SelectListItem>();
            List<SelectListItem> negara = new List<SelectListItem>();
            List<SelectListItem> jnsPkj = new List<SelectListItem>();
            List<SelectListItem> ktgrPkj = new List<SelectListItem>();
            List<SelectListItem> pembekal = new List<SelectListItem>();
            List<SelectListItem> statusAktif = new List<SelectListItem>();
            List<SelectListItem> banklist = new List<SelectListItem>();
            List<SelectListItem> division = new List<SelectListItem>();

            Gender = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jantina" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text", tbl_Pkjmast.fld_Kdjnt).ToList();
            Gender.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            statusKahwin = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "tarafKahwin" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text", tbl_Pkjmast.fld_Kdkwn).ToList();
            statusKahwin.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            peneroka = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusPeneroka" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfID).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text", tbl_Pkjmast.fld_Kpenrka).ToList();
            peneroka.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            bangsa = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "bangsa" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text", tbl_Pkjmast.fld_Kdbgsa).ToList();
            bangsa.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            agama = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "agama" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text", tbl_Pkjmast.fld_Kdagma).ToList();
            agama.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            krytn = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text", tbl_Pkjmast.fld_Kdrkyt).ToList();
            krytn.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            negeri = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "negeri" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text", tbl_Pkjmast.fld_Neg.Trim()).ToList();
            negeri.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            negara = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text", tbl_Pkjmast.fld_Negara.Trim()).ToList();
            negara.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            jnsPkj = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc.ToUpper() }).Distinct(), "Value", "Text", tbl_Pkjmast.fld_Jenispekerja).ToList();
            jnsPkj.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            ktgrPkj = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "designation" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue.Trim(), Text = s.fldOptConfDesc }).Distinct(), "Value", "Text", tbl_Pkjmast.fld_Ktgpkj.Trim()).ToList();
            ktgrPkj.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            pembekal = new SelectList(db.tbl_Pembekal.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).OrderBy(o => o.fld_KodPbkl).Select(s => new SelectListItem { Value = s.fld_KodPbkl, Text = s.fld_NamaPbkl }).Distinct(), "Value", "Text", tbl_Pkjmast.fld_Kodbkl).ToList();
            pembekal.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            statusAktif = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }).Distinct(), "Value", "Text", statuspkj).ToList();
            statusAktif.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            division = new SelectList(db.tbl_Division.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_DivisionName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_DivisionName }).Distinct(), "Value", "Text", tbl_Pkjmast.fld_DivisionID).ToList();
            division.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            //banklist = new SelectList(db.tbl_Bank.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).OrderBy(o => o.fld_KodBank).Select(s => new SelectListItem { Value = s.fld_KodBank, Text = s.fld_KodBank + " - " + s.fld_NamaBank }).Distinct(), "Value", "Text", tbl_Pkjmast.fld_Kdbank).ToList();
            //banklist.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
            ViewBag.EditDivision = 0;
            var checkAttendance = dbr.tbl_Kerjahdr.Where(x => x.fld_Nopkj == tbl_Pkjmast.fld_Nopkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Any();
            if (checkAttendance==false && tbl_Pkjmast.fld_KumpulanID == null)
            {
                //boleh edit
                ViewBag.EditDivision = 1;
            }
            else
            {
                //xboleh edit
                ViewBag.EditDivision = 2;
            }
            //Comment by fitri 17-09-2020
            //var findImage = dbr.tbl_SupportedDoc.Where(x => x.fld_Nopkj == id && x.fld_Flag == "picPkj" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).Select(s => s.fld_Url).FirstOrDefault();
            //Add by fitri 17-09-2020
            var findImage = dbr.tbl_SupportedDoc.Where(x => x.fld_Nopkj == id && x.fld_Flag == "ProfPic" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).Select(s => s.fld_Url).FirstOrDefault();
            if (findImage == null)
            {
                findImage = "/Asset/Images/default-user.png";
            }
            ViewBag.fld_Kdjnt = Gender;
            ViewBag.fld_Kdkwn = statusKahwin;
            ViewBag.fld_Kpenrka = peneroka;
            ViewBag.fld_Kdbgsa = bangsa;
            ViewBag.fld_Kdagma = agama;
            ViewBag.fld_Kdrkyt = krytn;
            ViewBag.fld_Neg = negeri;
            ViewBag.fld_Negara = negara;
            ViewBag.fld_Jenispekerja = jnsPkj;
            ViewBag.fld_Ktgpkj = ktgrPkj;
            ViewBag.fld_Kodbkl = pembekal;
            ViewBag.fld_Kdaktf = statusAktif;
            ViewBag.ImageSource = findImage;
            ViewBag.fld_Negara2 = negara;
            ViewBag.fld_DivisionID = division;

            return PartialView("WorkerUpdate", tbl_Pkjmast);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult WorkerUpdate(string id, Models.tbl_Pkjmast tbl_Pkjmast)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                var getdata = dbr.tbl_Pkjmast.Where(w => w.fld_Nopkj == id && w.fld_LadangID == LadangID && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID).FirstOrDefault();
                getdata.fld_Trlhr = tbl_Pkjmast.fld_Trlhr;
                getdata.fld_Kdjnt = tbl_Pkjmast.fld_Kdjnt;
                getdata.fld_Kdkwn = tbl_Pkjmast.fld_Kdkwn;
                getdata.fld_Kpenrka = tbl_Pkjmast.fld_Kpenrka;
                getdata.fld_Kdbgsa = tbl_Pkjmast.fld_Kdbgsa;
                getdata.fld_Kdagma = tbl_Pkjmast.fld_Kdagma;
                getdata.fld_Kdrkyt = tbl_Pkjmast.fld_Kdrkyt;
                getdata.fld_Almt1 = tbl_Pkjmast.fld_Almt1;
                getdata.fld_Poskod = tbl_Pkjmast.fld_Poskod;
                getdata.fld_Almt1 = tbl_Pkjmast.fld_Almt1;
                getdata.fld_Neg = tbl_Pkjmast.fld_Neg;
                getdata.fld_Negara = tbl_Pkjmast.fld_Negara;
                getdata.fld_Jenispekerja = tbl_Pkjmast.fld_Jenispekerja;
                getdata.fld_Ktgpkj = tbl_Pkjmast.fld_Ktgpkj;
                getdata.fld_Prmtno = tbl_Pkjmast.fld_Prmtno;
                //getdata.fld_T1prmt = tbl_Pkjmast.fld_T1prmt;
                getdata.fld_T2prmt = tbl_Pkjmast.fld_T2prmt;
                //getdata.fld_T1pspt = tbl_Pkjmast.fld_T1pspt;
                getdata.fld_T2pspt = tbl_Pkjmast.fld_T2pspt;
                getdata.fld_Trmlkj = tbl_Pkjmast.fld_Trmlkj;
                getdata.fld_Trshjw = tbl_Pkjmast.fld_Trshjw;
                getdata.fld_T2visa = tbl_Pkjmast.fld_T2visa;
                getdata.fld_Kodbkl = tbl_Pkjmast.fld_Kodbkl;
                getdata.fld_DivisionID = tbl_Pkjmast.fld_DivisionID;
                dbr.Entry(getdata).State = EntityState.Modified;
                dbr.SaveChanges();
                var getid = id;
                //return Json(new { success = true, msg = GlobalResEstate.msgUpdate, status = "success", checkingdata = "0", method = "1" });
                return Json(new { success = true, msg = GlobalResEstate.msgUpdate, status = "success", checkingdata = "0", method = "2", btn = "Lihat" });
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }

        public ActionResult WorkerStatus(string id)
        {
            //Check_Balik
            GetStatus GetStatus = new GetStatus();
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            if (id == null)
            {
                return RedirectToAction("WorkerInfo");
            }
            Models.tbl_Pkjmast tbl_Pkjmast = dbr.tbl_Pkjmast.Where(w => w.fld_Nopkj == id && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID).FirstOrDefault();
            //string statuspkj = GetStatus.GetWorkerStatus(tbl_Pkjmast.fld_Kdaktf);
            //string statuspkj = GetConfig.GetData2(tbl_Pkjmast.fld_Kdaktf, "statusaktif");
            if (tbl_Pkjmast == null)
            {
                return RedirectToAction("WorkerInfo");
            }

            List<SelectListItem> statusAktif = new List<SelectListItem>();
            List<SelectListItem> sbbTakAktif = new List<SelectListItem>();
            List<SelectListItem> pkjbarulama = new List<SelectListItem>();

            statusAktif = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text", tbl_Pkjmast.fld_Kdaktf).ToList();
            //statusAktif.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
            sbbTakAktif = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "sbbTakAktif" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            sbbTakAktif.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            pkjbarulama = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "pkjbarulama" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            pkjbarulama.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            ViewBag.fld_Kdaktf = statusAktif;
            ViewBag.fld_Sbtakf = sbbTakAktif;
            ViewBag.pkjLamaBaru = pkjbarulama;

            return PartialView("WorkerStatus", tbl_Pkjmast);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult WorkerStatus(string pkjLamaBaru, string nopkjnew, Models.tbl_Pkjmast tbl_Pkjmast, tblStatusPkj tblStatusPkj)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            if (tbl_Pkjmast.fld_Kdaktf == "2")
            {
                //x aktif
                try
                {
                    var getdata = dbr.tbl_Pkjmast.Where(w => w.fld_Nopkj == tbl_Pkjmast.fld_Nopkj && w.fld_LadangID == LadangID && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID).FirstOrDefault();

                    getdata.fld_Kdaktf = tbl_Pkjmast.fld_Kdaktf;
                    getdata.fld_Sbtakf = tbl_Pkjmast.fld_Sbtakf;
                    getdata.fld_Trtakf = tbl_Pkjmast.fld_Trtakf;
                    getdata.fld_Remarks = tbl_Pkjmast.fld_Remarks;
                    dbr.Entry(getdata).State = EntityState.Modified;
                    dbr.SaveChanges();
                    //var getid = id;
                    return Json(new { success = true, msg = GlobalResEstate.msgUpdate, status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new { success = true, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
                }
            }
            else
            {
                //aktif
                try
                {
                    if (pkjLamaBaru == "1")
                    {
                        //nopkj baru
                        //insert statuspkj
                        //tblStatusPkj.fldNoPkjLama = tbl_Pkjmast.fld_Nopkj;
                        //tblStatusPkj.fldNama = tbl_Pkjmast.fld_Nama;
                        //tblStatusPkj.fldNoKP = tbl_Pkjmast.fld_Nokp;
                        //tblStatusPkj.fldStatusAktif = 0;
                        //tblStatusPkj.fldAppliedBy = Convert.ToString(getuserid);
                        //tblStatusPkj.fldAppliedDate = DateTime.Now;
                        //tblStatusPkj.fldNegaraID = NegaraID;
                        //tblStatusPkj.fldSyarikatID = SyarikatID;
                        //tblStatusPkj.fldWilayahID = WilayahID;
                        //tblStatusPkj.fldLadangID = LadangID;
                        //tblStatusPkj.fldNoPkjBaru = nopkjnew;
                        //db.tblStatusPkjs.Add(tblStatusPkj);
                        //db.SaveChanges();

                        //insert new nopkj in pkjmast
                        var pkjmastAsal = dbr.tbl_Pkjmast.Where(w => w.fld_Nopkj == tbl_Pkjmast.fld_Nopkj && w.fld_LadangID == LadangID && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID).FirstOrDefault();
                        pkjmastAsal.fld_Nopkj = nopkjnew;
                        pkjmastAsal.fld_Kdaktf = tbl_Pkjmast.fld_Kdaktf;
                        dbr.tbl_Pkjmast.Add(pkjmastAsal);
                        dbr.SaveChanges();

                        return Json(new { success = true, msg = GlobalResEstate.msgAdd, status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "", data3 = "" });

                    }
                    else if (pkjLamaBaru == "2")
                    {
                        //nopkj lama
                        //tblStatusPkj.fldNoPkjLama = tbl_Pkjmast.fld_Nopkj;
                        //tblStatusPkj.fldNama = tbl_Pkjmast.fld_Nama;
                        //tblStatusPkj.fldNoKP = tbl_Pkjmast.fld_Nokp;
                        //tblStatusPkj.fldStatusAktif = 0;
                        //tblStatusPkj.fldAppliedBy = Convert.ToString(getuserid);
                        //tblStatusPkj.fldAppliedDate = DateTime.Now;
                        //tblStatusPkj.fldNegaraID = NegaraID;
                        //tblStatusPkj.fldSyarikatID = SyarikatID;
                        //tblStatusPkj.fldWilayahID = WilayahID;
                        //tblStatusPkj.fldLadangID = LadangID;
                        //tblStatusPkj.fldNoPkjBaru = tbl_Pkjmast.fld_Nopkj;

                        //db.tblStatusPkjs.Add(tblStatusPkj);
                        //db.SaveChanges();

                        var pkjmastAsal = dbr.tbl_Pkjmast.Where(w => w.fld_Nopkj == tbl_Pkjmast.fld_Nopkj && w.fld_LadangID == LadangID && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID).FirstOrDefault();
                        pkjmastAsal.fld_Kdaktf = tbl_Pkjmast.fld_Kdaktf;
                        dbr.Entry(pkjmastAsal).State = EntityState.Modified;
                        dbr.SaveChanges();

                        return Json(new { success = true, msg = GlobalResEstate.msgAdd, status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "", data3 = "" });
                    }
                    else
                    {
                        return Json(new { success = true, msg = GlobalResEstate.msgErrorData, status = "warning", checkingdata = "1" });
                    }
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new { success = true, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
                }
            }
        }

        public ActionResult WorkerDelete(string id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            if (id == null)
            {
                return RedirectToAction("WorkerInfo");
            }
            Models.tbl_Pkjmast tbl_Pkjmast = dbr.tbl_Pkjmast.Where(w => w.fld_Nopkj == id && w.fld_LadangID == LadangID && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID).FirstOrDefault();
            if (tbl_Pkjmast == null)
            {
                return RedirectToAction("WorkerInfo");
            }
            return PartialView("WorkerDelete", tbl_Pkjmast);
        }

        [HttpPost, ActionName("WorkerDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult WorkerDeleteConfirmed(string id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                Models.tbl_Pkjmast tbl_Pkjmast = dbr.tbl_Pkjmast.Where(w => w.fld_Nopkj == id && w.fld_LadangID == LadangID && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID).FirstOrDefault();
                if (tbl_Pkjmast == null)
                {
                    return Json(new { success = true, msg = GlobalResEstate.msgDelete2, status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
                }
                else
                {
                    dbr.tbl_Pkjmast.Remove(tbl_Pkjmast);
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

        public JsonResult GetStatusTnmn(string JnsTnmn)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            //Check_Balik

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<SelectListItem> statuslist = new List<SelectListItem>();
            statuslist = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "statusTanaman" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }), "Value", "Text").ToList();
            return Json(statuslist);
        }

        //public JsonResult GetKatAktvtCC(string JnsLot)
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<SelectListItem> KatAktvt = new List<SelectListItem>();
        //    KatAktvt = new SelectList(db.tbl_KategoriAktiviti.Where(x =>x.fld_KodJnsAktvt==JnsLot && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_Kategori).Select(s => new SelectListItem { Value = s.fld_PrefixPkt, Text = s.fld_Kategori }), "Value", "Text").ToList();
        //    return Json(KatAktvt);
        //}

        public JsonResult GetSubPkt(string Pktutama)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            List<SelectListItem> SubPkt = new List<SelectListItem>();
            SubPkt = new SelectList(dbr.tbl_SubPkt.Where(x => x.fld_KodPktUtama == Pktutama && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted==false).OrderBy(o => o.fld_Pkt).Select(s => new SelectListItem { Value = s.fld_Pkt, Text = s.fld_Pkt + " - " + s.fld_NamaPkt }).Distinct(), "Value", "Text").ToList();
            return Json(SubPkt);
        }

        public JsonResult GetPktUtama(string JnsTnmn, string StatusTnmn)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            string codetnmn;
            string newpkt = "";
            int kodpkt = 0;
            int newkodpkt = 0;
            string tahun = DateTime.Now.ToString("yy");

            if (JnsTnmn != "0" && StatusTnmn != "0")
            {
                codetnmn = JnsTnmn + StatusTnmn;
                var getpkts = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_PktUtama.Contains(codetnmn)).ToList();
                if (getpkts != null)
                {   
                    var getpkts1 = getpkts.Where(x => x.fld_PktUtama.Length == 7).ToList();

                    if (getpkts1.Count() > 0)
                    {
                        var getpkt = getpkts1.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_PktUtama.Contains(codetnmn)).Select(s => s.fld_PktUtama).Distinct().OrderByDescending(s => s).FirstOrDefault();
                        kodpkt = Convert.ToInt32(getpkt.Substring(getpkt.Length - 3));
                    }
                    else
                    {
                        var getpkt = getpkts.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_PktUtama.Contains(codetnmn)).Select(s => s.fld_PktUtama).Distinct().OrderByDescending(s => s).FirstOrDefault();
                        if (getpkt != null)
                        {
                            kodpkt = Convert.ToInt32(getpkt.Substring(getpkt.Length - 2));
                        }
                        else
                        {
                            kodpkt = 0;
                        }
                    }
                }
                else
                {
                    kodpkt = 0;
                }

                newkodpkt = kodpkt + 1;
                if (newkodpkt.ToString().Length == 3)
                {
                    newpkt = codetnmn + tahun + newkodpkt.ToString("000");
                }
                else
                {
                    newpkt = codetnmn + tahun + newkodpkt.ToString("00");
                }

                //if (getpkt != null)
                //{
                //    if (getpkt.ToString().Length == 7)
                //    {
                //        kodpkt = Convert.ToInt32(getpkt.Substring(getpkt.Length - 3));
                //    }
                //    else
                //    {
                //        kodpkt = Convert.ToInt32(getpkt.Substring(getpkt.Length - 2));
                //    }
                //}
                //else
                //{
                //    kodpkt = 0;
                //}

            }
            else
            {
                newpkt = "";
            }
            return Json(newpkt);
        }

        //public JsonResult GetPktUtamaCC(string Prefix)
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

        //    string codetnmn;
        //    int kodpkt = 0;
        //    int newkodpkt = 0;
        //    string newpkt = "";
        //    string tahun = DateTime.Now.ToString("yy");

        //    if (Prefix != "0")
        //    {
        //        codetnmn = Prefix + tahun;
        //        var getpkt = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_PktUtama.Contains(codetnmn)).Select(s => s.fld_PktUtama).Distinct().OrderByDescending(s => s).FirstOrDefault();

        //        if (getpkt != null)
        //        {
        //            kodpkt = Convert.ToInt32(getpkt.Substring(getpkt.Length - 2));
        //        }
        //        else
        //        {
        //            kodpkt = 0;
        //        }
        //        newkodpkt = kodpkt + 1;
        //        newpkt = codetnmn + newkodpkt.ToString("00");
        //    }
        //    else
        //    {
        //        newpkt = "";
        //    }           
        //    return Json(newpkt);
        //}

        public JsonResult GetPkt(string Pktutama)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            string newpkt = "";
            string kodpkt = "";
            char alphabet = 'A';
            decimal luas = 0;

            if (Pktutama != "0" && Pktutama != "")
            {
                var getluasutama = dbr.tbl_PktUtama.Where(x => x.fld_PktUtama == Pktutama && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_LsPktUtama).FirstOrDefault();
                var getluas = dbr.tbl_SubPkt.Where(x => x.fld_KodPktUtama == Pktutama && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_LsPkt).Sum();
                var getpkt = dbr.tbl_SubPkt.Where(x => x.fld_KodPktUtama == Pktutama && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_Pkt).Distinct().OrderByDescending(s => s).FirstOrDefault();
                luas = getluasutama.Value - getluas.GetValueOrDefault();

                if (getpkt != null)
                {
                    kodpkt = getpkt.Substring(6, 1);
                    alphabet = Convert.ToChar(kodpkt);
                    alphabet = (char)(((int)alphabet) + 1);
                }
                else
                {
                    alphabet = 'A';
                }

                newpkt = Pktutama + alphabet;
            }
            else
            {
                newpkt = "";
            }
            return Json(new { newpkt = newpkt, luas = luas.ToString("0.000") });
        }

        public JsonResult GetBlock(string pktutama, string pkt)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int kodblok = 0;
            int newkodblok = 0;
            string newblok = "";
            decimal luas = 0;

            if (pktutama != "0" && pkt != "0")
            {
                var getluaspkt = dbr.tbl_SubPkt.Where(x => x.fld_KodPktUtama == pktutama && x.fld_Pkt == pkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_LsPkt).FirstOrDefault();
                var getluas = dbr.tbl_Blok.Where(x => x.fld_KodPktutama == pktutama && x.fld_KodPkt == pkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_LsBlok).Sum();
                luas = getluaspkt.Value - getluas.GetValueOrDefault();
                var getblok = dbr.tbl_Blok.Where(x => x.fld_KodPktutama == pktutama && x.fld_KodPkt == pkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_Blok).Distinct().OrderByDescending(s => s).FirstOrDefault();

                if (getblok != null)
                {
                    kodblok = Convert.ToInt32(getblok.Substring(7, 2));
                }
                else
                {
                    kodblok = 0;
                }
                newkodblok = kodblok + 1;
                newblok = pkt + newkodblok.ToString("00");
            }
            else
            {
                newblok = "";
            }
            return Json(new { newblok = newblok, luas = luas.ToString("0.000") });
        }

        public JsonResult checkluas(string kodpktutama, decimal luas)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            decimal? bakiluas = 0;
            int result = 0;
            var koutaluas = dbr.tbl_PktUtama.Where(x => x.fld_PktUtama == kodpktutama && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_LsPktUtama).FirstOrDefault();
            var luaspkt = dbr.tbl_SubPkt.Where(x => x.fld_KodPktUtama == kodpktutama && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_LsPkt).Sum();
            bakiluas = koutaluas - luaspkt.GetValueOrDefault();

            if (luas > bakiluas)
            {
                result = 0;
            }
            else
            {
                result = 1;
            }
            return Json(result);
        }

        public JsonResult checkluas2(string kodpkt, decimal luas)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int result = 0;
            decimal? bakiluas = 0;

            var koutaluas = dbr.tbl_SubPkt.Where(x => x.fld_Pkt == kodpkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_LsPkt).FirstOrDefault();
            var luasblok = dbr.tbl_Blok.Where(x => x.fld_KodPkt == kodpkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_LsBlok).Sum();
            bakiluas = koutaluas - luasblok.GetValueOrDefault();

            if (luas > bakiluas)
            {
                result = 0;
            }
            else
            {
                result = 1;
            }
            return Json(result);
        }

        public JsonResult GetNopkj(string krkytn)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            GetLadang GetLadang = new GetLadang();
            string kuota = "P";
            string warganegara = krkytn;
            string kodldg = GetLadang.GetLadangCode(LadangID.Value);
            string thnKemasukan = DateTime.Now.ToString("yy");
            string nopkjnew = "";
            int kod = 0;

            if (krkytn != "0")
            {
                if (krkytn == "ID")
                {
                    warganegara = krkytn.Substring(1, 1);
                }
                else
                {
                    warganegara = krkytn.Substring(0, 1);
                }
                nopkjnew = kuota + warganegara + kodldg + thnKemasukan;
                var getpkj = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj.Contains(nopkjnew) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_Nopkj.Trim()).Distinct().OrderByDescending(s => s).FirstOrDefault();

                if (getpkj != null)
                {
                    kod = Convert.ToInt32(getpkj.Substring(getpkj.Length - 3));
                }
                else
                {
                    kod = 0;
                }
                kod = kod + 1;
                nopkjnew = nopkjnew + kod.ToString("000");
            }
            else
            {
                nopkjnew = "";
            }
            return Json(nopkjnew);
        }

        public JsonResult GetDaerah(string poskod, string negeri, string negara)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            //Check_Balik
            string daerah = "";
            string ngri = "";
            string ngra = "";

            if (poskod != "0" && negeri != "0" && negara != "0")
            {
                ngri = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "negeri" && x.fldOptConfValue == negeri && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfFlag2).FirstOrDefault();
                ngra = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fldOptConfValue == negara && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfDesc).FirstOrDefault();
                daerah = db.tbl_Poskod.Where(x => x.fld_Postcode == poskod && x.fld_State.Contains(ngri) && x.fld_Region.Contains(ngra) && x.fld_deleted == false).Select(s => s.fld_DistrictArea).FirstOrDefault();
                if (daerah == null)
                {
                    daerah = "";
                }
            }
            else
            {
                daerah = "";
            }
            return Json(daerah);
        }


        public JsonResult GetBatch()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            GetLadang GetLadang = new GetLadang();
            string kodldg = GetLadang.GetLadangCode(LadangID.Value);
            int batchValue = 0;
            string newbatch = "";

            //var batch = db.tblEmailNotiStatus.Where(x =>x.fldEmailNotiFlag.Contains("pkjmast") && x.fldWilayahID == WilayahID && x.fldLadangID == LadangID && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID).Select(s => s.fldEmailNotiFlag).Distinct().OrderByDescending(s => s).FirstOrDefault();
            //var getpkj = db.tbl_Pkjmast.Where(x => x.fld_Nopkj.Contains(nopkjnew) && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_Nopkj).Distinct().OrderByDescending(s => s).FirstOrDefault();
            var batch = dbr.tbl_Pkjmast.Where(x => x.fld_Batch.Contains("pkjmast") && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_Batch).Distinct().OrderByDescending(s => s).FirstOrDefault();
            if (batch != null)
            {
                //batchValue = Convert.ToInt32(batch.Substring(13, 3));
                batchValue = Convert.ToInt32(batch.Substring(batch.Length - 3));
                // batchValue = Convert.ToInt32(batch.Substring(10,3));
                batchValue = Convert.ToInt32(batch.Substring(batch.Length - 3));
            }
            else
            {
                batchValue = 0;
            }
            batchValue = batchValue + 1;
            newbatch = kodldg + "pkjmast" + batchValue.ToString("000");

            return Json(newbatch);
        }

        public JsonResult GetNopkjReplace(string nopkjlama, string pilihanNopkj)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            GetLadang GetLadang = new GetLadang();
            //string kuota = "F";
            string kuotawarganegara = nopkjlama.Substring(0, 2);
            string kodldg = GetLadang.GetLadangCode(LadangID.Value);
            string thnKemasukan = DateTime.Now.ToString("yy");
            string nopkjnew = "";
            int kod = 0;

            if (pilihanNopkj == "1")
            {
                //pkjbaru
                nopkjnew = kuotawarganegara + kodldg + thnKemasukan;
                var getpkj = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj.Contains(nopkjnew) && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_Nopkj).Distinct().OrderByDescending(s => s).FirstOrDefault();

                if (getpkj != null)
                {
                    kod = Convert.ToInt32(getpkj.Substring(7, 3));
                }
                else
                {
                    kod = 0;
                }
                kod = kod + 1;
                nopkjnew = nopkjnew + kod.ToString("000");
            }
            else if (pilihanNopkj == "2")
            {
                //pkjlama
                nopkjnew = nopkjlama;
            }
            else
            {
                nopkjnew = "";
            }
            return Json(nopkjnew);
        }

        public JsonResult GetPkjPindah(int wlyh, string pkj)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, wlyh, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            GetLadang GetLadang = new GetLadang();
            string kuota = "F";
            string warganegara = "";
            string kodldg = "";
            string thnKemasukan = DateTime.Now.ToString("yy");
            string nopkjnew = "";
            int kod = 0;
            Models.tbl_Pkjmast Pkjmast = new Models.tbl_Pkjmast();

            if (wlyh != 0 && pkj != "")
            {
                Pkjmast = dbr.tbl_Pkjmast.Where(w => w.fld_Nopkj == pkj && w.fld_Kdaktf == "2" && w.fld_WilayahID == wlyh && w.fld_NegaraID == NegaraID && w.fld_SyarikatID == SyarikatID).FirstOrDefault();
                //Pkjmast.fld_Trlhr = Pkjmast.fld_Trlhr.ToString();
                //warganegara = Pkjmast.fld_Kdrkyt.Substring(0,1);
                kodldg = GetLadang.GetLadangCode(LadangID.Value);

                if (Pkjmast != null && Pkjmast.fld_Kdrkyt == "ID")
                {
                    warganegara = Pkjmast.fld_Kdrkyt.Substring(1, 1);
                }
                else if (Pkjmast != null && Pkjmast.fld_Kdrkyt != "ID")
                {
                    warganegara = Pkjmast.fld_Kdrkyt.Substring(0, 1);
                }

                nopkjnew = kuota + warganegara + kodldg + thnKemasukan;
                var getpkj = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj.Contains(nopkjnew) && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_Nopkj).Distinct().OrderByDescending(s => s).FirstOrDefault();
                if (getpkj != null)
                {
                    kod = Convert.ToInt32(getpkj.Substring(7, 3));
                }
                else
                {
                    kod = 0;
                }
                kod = kod + 1;
                nopkjnew = nopkjnew + kod.ToString("000");
            }
            return Json(new { Pkjmast = Pkjmast, nopkjnew = nopkjnew });
        }

        public JsonResult GetPermitExpired(DateTime permitDate)
        {
            DateTime exprdDate = permitDate.AddYears(1);
            //exprdDate = exprdDate.ToString("dd/mm/yy");
            string dt = exprdDate.ToString("dd/mm/yyyy");
            return Json(exprdDate);
        }

        //groupinfo action starts here
        public ActionResult GroupInfo(string workerid, string filter = "", int page = 1, string sort = "fld_KodKumpulan",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            ViewBag.filter = filter;
            ViewBag.BasicInfo = "class = active";

            return View();
        }

        public PartialViewResult searchGroup(string workerid, string filter = "", int page = 1, string sort = "fld_KodKumpulan",
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

            return PartialView(records);
        }

        public ActionResult GroupCreate()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> kerjaList = new List<SelectListItem>();

            kerjaList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "kmplnKategoriList" &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem
                    {
                        Value = s.fldOptConfValue,
                        Text = s.fldOptConfDesc
                    }), "Value", "Text").ToList();

            ViewBag.KerjaList = kerjaList;

            List<SelectListItem> divisionList = new List<SelectListItem>();

            divisionList = new SelectList(
                db.tbl_Division
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false)
                    .OrderBy(o => o.fld_DivisionName)
                    .Select(s => new SelectListItem
                    {
                        Value = s.fld_ID.ToString(),
                        Text = s.fld_DivisionName
                    }), "Value", "Text").ToList();

            ViewBag.DivisionList = divisionList;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GroupCreate(Models.tbl_KumpulanKerjaViewModelCreate kumpulanKerjaViewModelCreate)
        {
            //Check_Balik
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
                    String kodKerja = kumpulanKerjaViewModelCreate.fld_KodKerja;

                    tbl_KumpulanKerja kumpulanKerja = new tbl_KumpulanKerja();

                    PropertyCopy.Copy(kumpulanKerja, kumpulanKerjaViewModelCreate);

                    var getCurrentGrpNo = dbr.tbl_KumpulanKerja
                        .Where(x => x.fld_KodKumpulan.Contains(kodKerja) && x.fld_NegaraID == NegaraID &&
                                    x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                    x.fld_LadangID == LadangID)
                        .Select(s => s.fld_KodKumpulan)
                        .OrderByDescending(s => s)
                        .FirstOrDefault();

                    var getGrpActivity = db.tblOptionConfigsWebs
                        .Where(x => x.fldOptConfValue == kumpulanKerjaViewModelCreate.fld_KodKerja && x.fldOptConfFlag1 == "kmplnKategoriList" &&
                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                        .Select(s => s.fldOptConfDesc)
                        .Distinct()
                        .FirstOrDefault();

                    kumpulanKerja.fld_KodKerja = getGrpActivity;
                    kumpulanKerja.fld_NegaraID = NegaraID;
                    kumpulanKerja.fld_SyarikatID = SyarikatID;
                    kumpulanKerja.fld_WilayahID = WilayahID;
                    kumpulanKerja.fld_LadangID = LadangID;
                    kumpulanKerja.fld_deleted = false;

                    if (getCurrentGrpNo == null)
                    {
                        kumpulanKerja.fld_KodKumpulan = kumpulanKerjaViewModelCreate.fld_KodKerja + "001";
                        dbr.tbl_KumpulanKerja.Add(kumpulanKerja);
                        dbr.SaveChanges();

                        db.Dispose();
                        dbr.Dispose();

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
                            action = "searchGroup",
                            controller = "BasicInfo"
                        });
                    }

                    else
                    {
                        int generateNewGrpNo = Convert.ToInt32(getCurrentGrpNo.Substring(1)) + 1;

                        kumpulanKerja.fld_KodKumpulan = kumpulanKerjaViewModelCreate.fld_KodKerja + generateNewGrpNo.ToString("000");
                        dbr.tbl_KumpulanKerja.Add(kumpulanKerja);
                        dbr.SaveChanges();

                        db.Dispose();
                        dbr.Dispose();

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
                            action = "searchGroup",
                            controller = "BasicInfo"
                        });
                    }
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());

                    db.Dispose();
                    dbr.Dispose();

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
                db.Dispose();
                dbr.Dispose();

                return Json(new
                {
                    success = false,
                    msg = GlobalResEstate.msgErrorData,
                    status = "warning",
                    checkingdata = "0"
                });
            }
        }

        public ActionResult GroupEdit(int id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            Models.tbl_KumpulanKerja tbl_KumpulanKerja = dbr.tbl_KumpulanKerja
                .Single(u => u.fld_KumpulanID == id && u.fld_WilayahID == WilayahID && u.fld_SyarikatID == SyarikatID &&
                             u.fld_NegaraID == NegaraID && u.fld_deleted == false);

            ViewBag.fld_LadangName = getidentity.estatename(Convert.ToInt32(getuserid));
            ViewBag.fld_WilayahName = getidentity.getWilayahName(Convert.ToInt32(getuserid));

            return PartialView("GroupEdit", tbl_KumpulanKerja);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GroupEdit(int id, Models.tbl_KumpulanKerja tbl_KumpulanKerja)
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
                    var getdata = dbr.tbl_KumpulanKerja
                        .Single(x => x.fld_KumpulanID == id && x.fld_LadangID == LadangID &&
                                     x.fld_WilayahID == WilayahID && x.fld_NegaraID == NegaraID && x.fld_deleted == false);
                    getdata.fld_Keterangan = tbl_KumpulanKerja.fld_Keterangan.ToUpper();
                    dbr.Entry(getdata).State = EntityState.Modified;
                    dbr.SaveChanges();

                    db.Dispose();
                    dbr.Dispose();

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
                        div = "searchResult",
                        rootUrl = domain,
                        action = "searchGroup",
                        controller = "BasicInfo"
                    });
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());

                    db.Dispose();
                    dbr.Dispose();

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
                db.Dispose();
                dbr.Dispose();

                return Json(new
                {
                    success = false,
                    msg = GlobalResEstate.msgErrorData,
                    status = "warning",
                    checkingdata = "0"
                });
            }
        }

        public ActionResult GroupMemberInfo(int id, int page = 1, string sort = "fld_Nopkj",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.tbl_Pkjmast>();

            var grpMemberList = dbview.tbl_Pkjmast
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_KumpulanID == id && x.fld_Kdaktf == "1");

            records.Content = grpMemberList
                .OrderBy(sort + " " + sortdir)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            records.TotalRecords = grpMemberList
                .Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.pageSize = pageSize;

            return PartialView(records);
        }

        public ActionResult GroupDelete(int id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            Models.tbl_KumpulanKerja kumpulanKerja = dbr.tbl_KumpulanKerja
                .Single(x => x.fld_KumpulanID == id && x.fld_LadangID == LadangID &&
                             x.fld_WilayahID == WilayahID && x.fld_NegaraID == NegaraID && x.fld_deleted == false);

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

            return PartialView("GroupDelete", kumpulanKerja);
        }

        [HttpPost, ActionName("GroupDelete")]
        public ActionResult GroupDeleteConfirm(int id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                var getdata = dbr.tbl_KumpulanKerja
                    .Single(x => x.fld_KumpulanID == id && x.fld_LadangID == LadangID &&
                                 x.fld_WilayahID == WilayahID && x.fld_NegaraID == NegaraID && x.fld_deleted == false);
                getdata.fld_deleted = true;
                dbr.Entry(getdata).State = EntityState.Modified;
                dbr.SaveChanges();

                db.Dispose();
                dbr.Dispose();

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
                    div = "searchResult",
                    rootUrl = domain,
                    action = "searchGroup",
                    controller = "BasicInfo"
                });
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());

                db.Dispose();
                dbr.Dispose();

                return Json(new
                {
                    success = false,
                    msg = GlobalResEstate.msgError,
                    status = "danger",
                    checkingdata = "0"
                });
            }
        }

        public ActionResult BankAccInfo()
        {
            ViewBag.BasicInfo = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> Bank = new List<SelectListItem>();
            Bank = new SelectList(db.tbl_Bank.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_ID).Select(s => new SelectListItem { Value = s.fld_KodBank, Text = s.fld_KodBank + " - " + s.fld_NamaBank }).Distinct(), "Value", "Text").ToList();

            var result = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahID && x.fld_ID == LadangID).FirstOrDefault();
            ViewBag.fld_BankCode = Bank;

            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BankAccInfo(MasterModels.tbl_Ladang tbl_Ladang)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            //string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            //Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            //MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                var getdatahq = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahID && x.fld_ID == LadangID).FirstOrDefault();
                getdatahq.fld_BankCode = tbl_Ladang.fld_BankCode;
                getdatahq.fld_NoAcc = tbl_Ladang.fld_NoAcc;
                getdatahq.fld_OriginatorID = tbl_Ladang.fld_OriginatorID;
                getdatahq.fld_OriginatorName = tbl_Ladang.fld_OriginatorName.ToUpper();
                getdatahq.fld_BranchCode = tbl_Ladang.fld_BranchCode;
                getdatahq.fld_BranchName = tbl_Ladang.fld_BranchName.ToUpper();
                getdatahq.fld_BankCreatedBy = User.Identity.Name;
                getdatahq.fld_BankCreatedDate = DateTime.Now;
                db.Entry(getdatahq).State = EntityState.Modified;
                db.SaveChanges();

                //var getid = tbl_Ladang.fld_ID;
                //return RedirectToAction("EstateInfo", "BasicInfo");
                
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
            }
            List<SelectListItem> Bank = new List<SelectListItem>();
            Bank = new SelectList(db.tbl_Bank.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_ID).Select(s => new SelectListItem { Value = s.fld_KodBank, Text = s.fld_KodBank + " - " + s.fld_NamaBank }).Distinct(), "Value", "Text").ToList();
            var result = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahID && x.fld_ID == LadangID).FirstOrDefault();
            ViewBag.fld_BankCode = Bank;
            return View(result);
        }

        public ActionResult LevelsIO()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> IOcode = new List<SelectListItem>();
            IOcode = new SelectList(dbr.tbl_IO.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_Status == 1), "fld_IOcode", "fld_IOcode").ToList();
            IOcode.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            List<SelectListItem> IOreff = new List<SelectListItem>();
            IOreff = new SelectList(dbr.tbl_IO.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_Status == 2), "fld_IOcode", "fld_IOcode").ToList();
            IOreff.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            ViewBag.IO_code = IOcode;
            ViewBag.IO_reff = IOreff;
            return PartialView();
        }
        //ActionName("LevelsIO")
        [HttpPost, ActionName("LevelsIO")]
        [ValidateAntiForgeryToken]
        public ActionResult LevelsIOsubmit(string IO_code, string IO_reff)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> IOcode = new List<SelectListItem>();
            IOcode = new SelectList(dbr.tbl_IO.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_Status == 1), "fld_IOcode", "fld_IOcode").ToList();
            IOcode.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            List<SelectListItem> IOreff = new List<SelectListItem>();
            IOreff = new SelectList(dbr.tbl_IO.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_Status == 2), "fld_IOcode", "fld_IOcode").ToList();
            IOreff.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            string IO_type = Request.Form["IOType"];

            var checkIo = dbr.tbl_IO.Where(x => x.fld_IOcode == IO_code && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_Status == 1).FirstOrDefault();
            if (checkIo != null)
            {
                //checkIo.fld_IOtype = IO_type;
                //checkIo.fld_IOref = IO_reff;
                checkIo.fld_Status = 2;
                dbr.Entry(checkIo).State = EntityState.Modified;
                dbr.SaveChanges();
            }

            ViewBag.IO_code = IOcode;
            ViewBag.IO_reff = IOreff;
            return PartialView();
        }

        //public ActionResult LevelsInfoTable()
        //{
        //    ViewBag.BasicInfo = "class = active";
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    List<SelectListItem> JenisPkt = new List<SelectListItem>();

        //    JenisPkt = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnspkt" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
        //    JenisPkt.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
        //    ViewBag.JnsPkt = JenisPkt;
        //    return View();
        //}

        //public ActionResult LevelInfo()
        //{
        //    ViewBag.BasicInfo = "class = active";
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    List<SelectListItem> JenisPkt = new List<SelectListItem>();

        //    JenisPkt = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnspkt" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
        //    JenisPkt.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
        //    ViewBag.JnsPkt = JenisPkt;
        //    return View();
        //}

        //public ActionResult LevelInfoPkt(string JnsPkt = "1", int page = 1, string sort = "fld_PktUtama", string sortdir = "ASC")
        //{
        //    int? getuserid = GetIdentity.ID(User.Identity.Name);
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

        //    int pageSize = int.Parse(GetConfig.GetData("paging"));
        //    var records = new PagedList<ViewingModels.tbl_PktUtama>();
        //    records.Content = dbview.tbl_PktUtama.Where(x => x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == false)
        //           .OrderBy(sort + " " + sortdir)
        //           .Skip((page - 1) * pageSize)
        //           .Take(pageSize)
        //           .ToList();

        //    records.TotalRecords = dbview.tbl_PktUtama.Where(x => x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == false).Count();
        //    records.CurrentPage = page;
        //    records.PageSize = pageSize;
        //    return View(records);
        //}
        //public ActionResult LevelInfoSubPkt(string JnsPkt = "2", int page = 1, string sort = "fld_Pkt", string sortdir = "ASC")
        //{
        //    int? getuserid = GetIdentity.ID(User.Identity.Name);
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

        //    int pageSize = int.Parse(GetConfig.GetData("paging"));
        //    var records = new PagedList<ViewingModels.tbl_SubPkt>();
        //    records.Content = dbview.tbl_SubPkt.Where(x => x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == false)
        //           .OrderBy(sort + " " + sortdir)
        //           .Skip((page - 1) * pageSize)
        //           .Take(pageSize)
        //           .ToList();

        //    records.TotalRecords = dbview.tbl_SubPkt.Where(x => x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == false).Count();
        //    records.CurrentPage = page;
        //    records.PageSize = pageSize;
        //    return View(records);
        //}

        //public ActionResult LevelInfoBlok(string JnsPkt = "3", int page = 1, string sort = "fld_Blok", string sortdir = "ASC")
        //{
        //    int? getuserid = GetIdentity.ID(User.Identity.Name);
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

        //    int pageSize = int.Parse(GetConfig.GetData("paging"));
        //    var records = new PagedList<ViewingModels.tbl_Blok>();
        //    records.Content = dbview.tbl_Blok.Where(x => x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == false)
        //           .OrderBy(sort + " " + sortdir)
        //           .Skip((page - 1) * pageSize)
        //           .Take(pageSize)
        //           .ToList();

        //    records.TotalRecords = dbview.tbl_Blok.Where(x => x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == false).Count();
        //    records.CurrentPage = page;
        //    records.PageSize = pageSize;
        //    return View(records);
        //}

        public ActionResult EstateReminder()
        {
            ViewBag.BasicInfo = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            MasterModels.tbl_Ladang tbl_Ladang = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_ID == LadangID && x.fld_WlyhID == WilayahID).FirstOrDefault();
            string kodngri = tbl_Ladang.fld_KodNegeri.ToString();
            string negeri = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "negeri" && x.fldOptConfValue == kodngri && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false && x.fldOptConfValue == kodngri).Select(s => s.fldOptConfDesc).FirstOrDefault();

            ViewBag.Negeri = negeri;
            return View(tbl_Ladang);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EstateReminder(MasterModels.tbl_Ladang tbl_Ladang)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);


            tbl_Ladang.fld_PengurusSblm = "-";
            tbl_Ladang.fld_AdressSblm = "-";
            tbl_Ladang.fld_TelSblm = "-";
            tbl_Ladang.fld_FaxSblm = "-";
            tbl_Ladang.fld_LdgEmailSblm = "-";

            if (ModelState.IsValid)
            {
                try
                {
                    var getdatahq = db.tbl_Ladang.Where(w => w.fld_NegaraID == NegaraID && w.fld_SyarikatID == SyarikatID && w.fld_ID == tbl_Ladang.fld_ID && w.fld_WlyhID == tbl_Ladang.fld_WlyhID && w.fld_Deleted == false).FirstOrDefault();
                    getdatahq.fld_PengurusSblm = getdatahq.fld_Pengurus;
                    getdatahq.fld_AdressSblm = getdatahq.fld_Adress;
                    getdatahq.fld_TelSblm = getdatahq.fld_Tel;
                    getdatahq.fld_FaxSblm = getdatahq.fld_Fax;
                    getdatahq.fld_LdgEmailSblm = getdatahq.fld_LdgEmail;

                    getdatahq.fld_Pengurus = tbl_Ladang.fld_Pengurus.ToUpper();
                    getdatahq.fld_Adress = tbl_Ladang.fld_Adress.ToUpper();
                    getdatahq.fld_Tel = tbl_Ladang.fld_Tel;
                    getdatahq.fld_Fax = tbl_Ladang.fld_Fax;
                    getdatahq.fld_LdgEmail = tbl_Ladang.fld_LdgEmail;

                    db.Entry(getdatahq).State = EntityState.Modified;
                    db.SaveChanges();

                    var getid = tbl_Ladang.fld_ID;
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    //return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
                }
            }
            return RedirectToAction("Index", "Login");
        }

        public JsonResult GetPktChange(string pktUtama)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            Models.tbl_PktUtama tbl_PktUtama = new Models.tbl_PktUtama();

            string jnsTnmn = "";
            string statusTnmn = "";
            decimal luas = 0;

            if (pktUtama != "0")
            {
                tbl_PktUtama = dbr.tbl_PktUtama.Where(x => x.fld_PktUtama == pktUtama && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).FirstOrDefault();
                jnsTnmn = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsTanaman" && x.fldOptConfValue == tbl_PktUtama.fld_JnsTnmn && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfDesc).FirstOrDefault();
                statusTnmn = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusTanaman" && x.fldOptConfValue == tbl_PktUtama.fld_StatusTnmn && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfDesc).FirstOrDefault();
                luas = tbl_PktUtama.fld_LsPktUtama.GetValueOrDefault();
            }
            return Json(new { jnsTnmn = jnsTnmn, statusTnmn = statusTnmn, luas = luas });
        }

        public JsonResult GetKawList()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> Kawasanlist = new List<SelectListItem>();
            Kawasanlist = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsKawasan" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

            return Json(Kawasanlist);
        }

        public JsonResult GetDirianPokok()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            string dirianPokok = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "dirianPokok" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfValue).FirstOrDefault();

            //List<SelectListItem> Dirianlist = new List<SelectListItem>();
            //Dirianlist = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "dirianPokok" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

            return Json(dirianPokok);
        }

        public JsonResult GetKesukaran(string jnsKesukaran)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> TahapKesukaranlist = new List<SelectListItem>();
            TahapKesukaranlist = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "tahapKesukaran" && x.fldOptConfValue.StartsWith(jnsKesukaran) && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }), "Value", "Text").ToList();
            return Json(TahapKesukaranlist);
        }

        public JsonResult GetLuasFromIO(string IO)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            //List<SelectListItem> IOcode = new List<SelectListItem>();
            //IOcode = new SelectList(dbr.tbl_IO.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_Status == 1), "fld_IOcode", "fld_IOcode").ToList();
            var checkLuas = dbr.tbl_IO.Where(x => x.fld_IOcode == IO && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
            decimal? luas = checkLuas.fld_Luas;
            decimal? luasTnmn = checkLuas.fld_LuasKawTnmn;
            decimal? luasTiadaTnmn = checkLuas.fld_LuasKawTiadaTnmn;
            return Json(new { luas = luas, luasTnmn = luasTnmn, luasTiadaTnmn = luasTiadaTnmn });
        }

        public JsonResult EmailValidation(string emel)
        {
            Regex Regex = new Regex(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");
            var result = Regex.IsMatch(emel);
            return Json(result);
        }

        public JsonResult SaveKaw(string pkt, int level, string jnskaw, decimal luas)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            Models.tbl_KawTidakBerhasil tbl_KawTidakBerhasil = new Models.tbl_KawTidakBerhasil();
            int flag = 0;
            string checkExist = dbr.tbl_KawTidakBerhasil.Where(x => x.fld_KodPkt == pkt && x.fld_LevelPkt == level && x.fld_JnsKaw == jnskaw && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_JnsKaw).FirstOrDefault();
            if (checkExist == null)
            {
                //data not exist
                tbl_KawTidakBerhasil.fld_KodPkt = pkt;
                tbl_KawTidakBerhasil.fld_LevelPkt = level;
                tbl_KawTidakBerhasil.fld_JnsKaw = jnskaw;
                tbl_KawTidakBerhasil.fld_LuasKaw = luas;
                tbl_KawTidakBerhasil.fld_NegaraID = NegaraID;
                tbl_KawTidakBerhasil.fld_SyarikatID = SyarikatID;
                tbl_KawTidakBerhasil.fld_WilayahID = WilayahID;
                tbl_KawTidakBerhasil.fld_LadangID = LadangID;
                tbl_KawTidakBerhasil.fld_Deleted = false;
                dbr.tbl_KawTidakBerhasil.Add(tbl_KawTidakBerhasil);
                dbr.SaveChanges();
                flag = 1;
            }
            else
            {
                //data already exist
                flag = 2;
            }
            return Json(flag);
        }

        public JsonResult RemoveKaw(string kodpkt, string jnskaw)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int flag = 0;
            var getkaw = dbr.tbl_KawTidakBerhasil.Where(x => x.fld_KodPkt == kodpkt && x.fld_JnsKaw == jnskaw && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).FirstOrDefault();
            if (getkaw == null)
            {
                flag = 1;
            }
            else
            {
                getkaw.fld_Deleted = true;
                dbr.Entry(getkaw).State = EntityState.Modified;
                dbr.SaveChanges();
                flag = 2;
            }
            return Json(flag);
        }

        [HttpPost]
        public ActionResult UploadFiles()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            Models.tbl_SupportedDoc SupportedDoc = new Models.tbl_SupportedDoc();

            if (Request.Files.Count > 0)
            {
                try
                {
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        string fname = file.FileName;
                        string nopkj = fname.Split('.').FirstOrDefault().Trim();
                        var folder = Server.MapPath("~/Asset/Upload/" + NegaraID + SyarikatID + WilayahID + "/" + LadangID);
                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }
                        string savePath = Path.Combine(folder, fname);
                        file.SaveAs(savePath);
                        //Comment by fitri 17-09-2020
                        //var findRoute = dbr.tbl_SupportedDoc.Where(x => x.fld_Nopkj == nopkj && x.fld_Flag == "picPkj" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
                        //Add by fitri 17-09-2020
                        var findRoute = dbr.tbl_SupportedDoc.Where(x => x.fld_Nopkj == nopkj && x.fld_Flag == "ProfPic" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
                        if (findRoute == null)
                        {
                            //add
                            SupportedDoc.fld_Nopkj = nopkj;
                            SupportedDoc.fld_NamaFile = fname;
                            SupportedDoc.fld_Url = "/Asset/Upload/" + NegaraID + SyarikatID + WilayahID + "/" + LadangID + "/" + fname;
                            SupportedDoc.fld_Flag = "ProfPic";
                            SupportedDoc.fld_NegaraID = NegaraID;
                            SupportedDoc.fld_SyarikatID = SyarikatID;
                            SupportedDoc.fld_WilayahID = WilayahID;
                            SupportedDoc.fld_LadangID = LadangID;
                            SupportedDoc.fld_Deleted = false;
                            dbr.tbl_SupportedDoc.Add(SupportedDoc);
                            dbr.SaveChanges();
                        }
                        else
                        {
                            //update
                            findRoute.fld_NamaFile = fname;
                            findRoute.fld_Url = "/Asset/Upload/" + NegaraID + SyarikatID + WilayahID + "/" + LadangID + "/" + fname;
                            findRoute.fld_Deleted = false;
                            dbr.Entry(findRoute).State = EntityState.Modified;
                            dbr.SaveChanges();
                        }


                    }
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }

        [HttpPost]
        public ActionResult UploadSupportedFiles()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            Models.tbl_SupportedDoc SupportedDoc = new Models.tbl_SupportedDoc();

            if (Request.Files.Count > 0)
            {
                try
                {
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        string fname = file.FileName;
                        string nopkj = fname.Split('_').FirstOrDefault().Trim();
                        var folder = Server.MapPath("~/Asset/Upload/" + NegaraID + SyarikatID + WilayahID + "/" + LadangID);
                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }
                        string savePath = Path.Combine(folder, fname);
                        file.SaveAs(savePath);

                        var findRoute = dbr.tbl_SupportedDoc.Where(x => x.fld_Nopkj == nopkj && x.fld_NamaFile == fname && x.fld_Flag == "supportedDoc" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
                        if (findRoute == null)
                        {
                            //add
                            SupportedDoc.fld_Nopkj = nopkj;
                            SupportedDoc.fld_NamaFile = fname;
                            SupportedDoc.fld_Url = "/Asset/Upload/" + NegaraID + SyarikatID + WilayahID + "/" + LadangID + "/" + fname;
                            SupportedDoc.fld_Flag = "supportedDoc";
                            SupportedDoc.fld_NegaraID = NegaraID;
                            SupportedDoc.fld_SyarikatID = SyarikatID;
                            SupportedDoc.fld_WilayahID = WilayahID;
                            SupportedDoc.fld_LadangID = LadangID;
                            SupportedDoc.fld_Deleted = false;
                            dbr.tbl_SupportedDoc.Add(SupportedDoc);
                            dbr.SaveChanges();
                        }
                        else
                        {
                            //update
                            findRoute.fld_NamaFile = fname;
                            findRoute.fld_Url = "/Asset/Upload/" + NegaraID + SyarikatID + WilayahID + "/" + LadangID + "/" + fname;
                            findRoute.fld_Deleted = false;
                            dbr.Entry(findRoute).State = EntityState.Modified;
                            dbr.SaveChanges();
                        }


                    }
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }

        public JsonResult GetUmurPkj(string sdate)
        {
            int age = 0;
            if (sdate != null)
            {
                //DateTime dateNow = DateTime.Today.ToString("dd/MM/yyyy");
                DateTime dateBirth = DateTime.ParseExact(sdate,"dd/MM/yyyy",CultureInfo.InvariantCulture);
                DateTime dateToday = DateTime.Today;
                age = dateToday.Year - dateBirth.Year;
                if (dateToday < dateBirth.AddYears(age))
                {
                    age--;
                }

            }
            return Json(age);
        }

        [HttpPost]
        public ActionResult WorkerDoc(string nopkj)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            var result = dbr.tbl_SupportedDoc.Where(x => x.fld_Nopkj == nopkj && x.fld_Flag == "supportedDoc" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false);
            return PartialView(result);
        }

        public JsonResult RemoveDoc(int docID)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            string msg = "";
            Boolean statusTrans = false;

            var findDoc = dbr.tbl_SupportedDoc.Where(x => x.fld_ID == docID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
            if (findDoc != null)
            {
                findDoc.fld_Deleted = true;
                dbr.Entry(findDoc).State = EntityState.Modified;
                dbr.SaveChanges();
                statusTrans = true;
                msg = "Data Berjaya Disimpan";
            }
            else
            {
                statusTrans = false;
                msg = "Data Tidak Wujud";
            }
            return Json(new { msg = msg, statusTrans = statusTrans });
        }

        public JsonResult GetWilayahList(int syrktID)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> wilayahlist = new List<SelectListItem>();

            if (syrktID == 0)
            {
                wilayahlist.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            }
            else
            {
                wilayahlist = new SelectList(db.tbl_Wilayah.Where(x => x.fld_SyarikatID == syrktID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).OrderBy(o => o.fld_ID).Select(s => new SelectListItem { Value = s.fld_ID.ToString().Trim(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                wilayahlist.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            }

            return Json(wilayahlist);
        }

        public JsonResult GetPktSAPdetail(string WbsCode)
        {
            
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            string JnsTnmn = "";
            string StatusTanaman = "";
            decimal Luas = 0;
            decimal LuasBerhasil = 0;

            if (WbsCode != "0")
            {
                var getData = db.tbl_SAPCUSTOMPUP.Where(x => x.fld_WBSCode == WbsCode && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_IsSelected == false).FirstOrDefault();
                JnsTnmn = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag3 == getData.fld_JnsTnmn && x.fldOptConfFlag1 == "jnsTanaman" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfValue).FirstOrDefault();
                StatusTanaman = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag3 == getData.fld_StatusTnmn && x.fldOptConfFlag1 == "statusTanaman" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfValue).FirstOrDefault();
                Luas = getData.fld_LsPktUtama.Value;
                LuasBerhasil = getData.fld_LuasBerhasil.Value;
            }
            return Json(new { JnsTnmn = JnsTnmn, StatusTanaman = StatusTanaman, Luas = Luas, LuasBerhasil = LuasBerhasil });
        }

        public ActionResult HarvestingPriceInfo(int? WilayahList, int? LadangList, int page = 1, string sort = "fld_KodInsentif", string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            ViewBag.WorkerInfo = "class = active";

            List<SelectListItem> pktList = new List<SelectListItem>();

            pktList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "jnspkt" && x.fldDeleted == false &&
                                                   x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfID)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            pktList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "" }));

            ViewBag.PktList = pktList;

            return View();
        }

        public ActionResult _HarvestingPriceInfo(string PktList, string filter, int page = 1, string sort = "KodPeringkat", string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<CustomModels.CustMod_HargaMenuai> hargaMenuaiList = new List<CustomModels.CustMod_HargaMenuai>();
            var records = new PagedList<CustomModels.CustMod_HargaMenuai>();
            int role = GetIdentity.RoleID(getuserid).Value;
            int pageSize = int.Parse(GetConfig.GetData("paging"));

            var message = "";

            if (!String.IsNullOrEmpty(PktList))
            {
                message = GlobalResEstate.msgNoRecord;

                if (String.IsNullOrEmpty(filter))
                {
                    if (PktList == "1")
                    {
                        var pktUtamaData = dbr.tbl_PktUtama.Where(x =>
                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                            x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_PktUtama);

                        foreach (var pktUtama in pktUtamaData)
                        {
                            var hargaMenuaiData = dbr.tbl_HargaMenuai
                                .SingleOrDefault(a =>
                                    a.fld_KodPeringkatUtama == pktUtama.fld_PktUtama && a.fld_NegaraID == NegaraID &&
                                    a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                    a.fld_LadangID == LadangID && a.fld_Deleted == false);

                            hargaMenuaiList.Add(new CustMod_HargaMenuai
                            {
                                ID = hargaMenuaiData != null ? hargaMenuaiData.fld_HargaMenuaiID : (Guid?)null,
                                JenisPeringkat = PktList,
                                KodPeringkat = pktUtama.fld_PktUtama,
                                NamaPeringkat = pktUtama.fld_NamaPktUtama,
                                HargaMenuai = hargaMenuaiData != null ? hargaMenuaiData.fld_HargaMenuai.ToString() : ""
                            });
                        }
                    }

                    else if (PktList == "2")
                    {
                        var subPktData = dbr.tbl_SubPkt.Where(x =>
                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                            x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_Pkt);

                        foreach (var subPkt in subPktData)
                        {
                            var hargaMenuaiData = dbr.tbl_HargaMenuai
                                .SingleOrDefault(a =>
                                    a.fld_KodPeringkatUtama == subPkt.fld_Pkt && a.fld_NegaraID == NegaraID &&
                                    a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                    a.fld_LadangID == LadangID && a.fld_Deleted == false);

                            hargaMenuaiList.Add(new CustMod_HargaMenuai
                            {
                                ID = hargaMenuaiData != null ? hargaMenuaiData.fld_HargaMenuaiID : (Guid?)null,
                                JenisPeringkat = PktList,
                                KodPeringkat = subPkt.fld_Pkt,
                                NamaPeringkat = subPkt.fld_NamaPkt,
                                HargaMenuai = hargaMenuaiData != null ? hargaMenuaiData.fld_HargaMenuai.ToString() : ""
                            });
                        }
                    }

                    else
                    {
                        var blokData = dbr.tbl_Blok.Where(x =>
                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                            x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_Blok);

                        foreach (var blok in blokData)
                        {
                            var hargaMenuaiData = dbr.tbl_HargaMenuai
                                .SingleOrDefault(a =>
                                    a.fld_KodPeringkatUtama == blok.fld_Blok && a.fld_NegaraID == NegaraID &&
                                    a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                    a.fld_LadangID == LadangID && a.fld_Deleted == false);

                            hargaMenuaiList.Add(new CustMod_HargaMenuai
                            {
                                ID = hargaMenuaiData != null ? hargaMenuaiData.fld_HargaMenuaiID : (Guid?)null,
                                JenisPeringkat = PktList,
                                KodPeringkat = blok.fld_Blok,
                                NamaPeringkat = blok.fld_NamaBlok,
                                HargaMenuai = hargaMenuaiData != null ? hargaMenuaiData.fld_HargaMenuai.ToString() : ""
                            });
                        }
                    }
                }

                else
                {
                    message = GlobalResEstate.msgNoRecord;

                    if (PktList == "1")
                    {
                        var pktUtamaData = dbr.tbl_PktUtama.Where(x =>
                            x.fld_PktUtama.Contains(filter) || x.fld_NamaPktUtama.Contains(filter) &&
                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID &&
                            x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_PktUtama); ;

                        foreach (var pktUtama in pktUtamaData)
                        {
                            var hargaMenuaiData = dbr.tbl_HargaMenuai
                                .SingleOrDefault(a =>
                                    a.fld_KodPeringkatUtama == pktUtama.fld_PktUtama && a.fld_NegaraID == NegaraID &&
                                    a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                    a.fld_LadangID == LadangID && a.fld_Deleted == false);

                            hargaMenuaiList.Add(new CustMod_HargaMenuai
                            {
                                ID = hargaMenuaiData != null ? hargaMenuaiData.fld_HargaMenuaiID : (Guid?)null,
                                JenisPeringkat = PktList,
                                KodPeringkat = pktUtama.fld_PktUtama,
                                NamaPeringkat = pktUtama.fld_NamaPktUtama,
                                HargaMenuai = hargaMenuaiData != null ? hargaMenuaiData.fld_HargaMenuai.ToString() : ""
                            });
                        }
                    }

                    else if (PktList == "2")
                    {
                        var subPktData = dbr.tbl_SubPkt.Where(x =>
                            x.fld_Pkt.Contains(filter) || x.fld_NamaPkt.Contains(filter) &&
                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID &&
                            x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_Pkt); ;

                        foreach (var subPkt in subPktData)
                        {
                            var hargaMenuaiData = dbr.tbl_HargaMenuai
                                .SingleOrDefault(a =>
                                    a.fld_KodPeringkatUtama == subPkt.fld_Pkt && a.fld_NegaraID == NegaraID &&
                                    a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                    a.fld_LadangID == LadangID && a.fld_Deleted == false);

                            hargaMenuaiList.Add(new CustMod_HargaMenuai
                            {
                                ID = hargaMenuaiData != null ? hargaMenuaiData.fld_HargaMenuaiID : (Guid?)null,
                                JenisPeringkat = PktList,
                                KodPeringkat = subPkt.fld_Pkt,
                                NamaPeringkat = subPkt.fld_NamaPkt,
                                HargaMenuai = hargaMenuaiData != null ? hargaMenuaiData.fld_HargaMenuai.ToString() : ""
                            });
                        }
                    }

                    else
                    {
                        var blokData = dbr.tbl_Blok.Where(x =>
                            x.fld_Blok.Contains(filter) || x.fld_NamaBlok.Contains(filter) &&
                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID &&
                            x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_Blok); ;

                        foreach (var blok in blokData)
                        {
                            var hargaMenuaiData = dbr.tbl_HargaMenuai
                                .SingleOrDefault(a =>
                                    a.fld_KodPeringkatUtama == blok.fld_Blok && a.fld_NegaraID == NegaraID &&
                                    a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                    a.fld_LadangID == LadangID && a.fld_Deleted == false);

                            hargaMenuaiList.Add(new CustMod_HargaMenuai
                            {
                                ID = hargaMenuaiData != null ? hargaMenuaiData.fld_HargaMenuaiID : (Guid?)null,
                                JenisPeringkat = PktList,
                                KodPeringkat = blok.fld_Blok,
                                NamaPeringkat = blok.fld_NamaBlok,
                                HargaMenuai = hargaMenuaiData != null ? hargaMenuaiData.fld_HargaMenuai.ToString() : ""
                            });
                        }
                    }
                }
            }

            else
            {
                message = GlobalResEstate.msgPleaseChooseLevel;
            }

            ViewBag.Message = message;
            ViewBag.pageSize = int.Parse(GetConfig.GetData("paging"));

            records.Content = hargaMenuaiList
                .OrderBy(sort + " " + sortdir)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            records.TotalRecords = hargaMenuaiList
                .Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.RoleID = role;
            ViewBag.PageSize = pageSize;

            return View(records);
        }

        public ActionResult _HarvestingPriceInfoCreate(string kodPeringkat, string jenisPeringkat)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            Models.tbl_HargaMenuaiViewModelCreate hargaMenuaiViewModelCreate = new tbl_HargaMenuaiViewModelCreate();

            if (jenisPeringkat == "1")
            {
                var pktData = dbr.tbl_PktUtama.SingleOrDefault(x =>
                    x.fld_PktUtama == kodPeringkat && x.fld_NegaraID == NegaraID &&
                    x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                    x.fld_Deleted == false);

                hargaMenuaiViewModelCreate.fld_JenisPeringkat = jenisPeringkat;
                hargaMenuaiViewModelCreate.fld_KodPeringkatUtama = kodPeringkat;
                hargaMenuaiViewModelCreate.fld_NamaPeringkat = pktData.fld_NamaPktUtama;
            }

            else if (jenisPeringkat == "2")
            {
                var subPktData = dbr.tbl_SubPkt.SingleOrDefault(x =>
                    x.fld_Pkt == kodPeringkat && x.fld_NegaraID == NegaraID &&
                    x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                    x.fld_Deleted == false);

                hargaMenuaiViewModelCreate.fld_JenisPeringkat = jenisPeringkat;
                hargaMenuaiViewModelCreate.fld_KodPeringkatUtama = kodPeringkat;
                hargaMenuaiViewModelCreate.fld_NamaPeringkat = subPktData.fld_NamaPkt;
            }

            else
            {
                var blokData = dbr.tbl_Blok.SingleOrDefault(x =>
                    x.fld_Blok == kodPeringkat && x.fld_NegaraID == NegaraID &&
                    x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                    x.fld_Deleted == false);

                hargaMenuaiViewModelCreate.fld_JenisPeringkat = jenisPeringkat;
                hargaMenuaiViewModelCreate.fld_KodPeringkatUtama = kodPeringkat;
                hargaMenuaiViewModelCreate.fld_NamaPeringkat = blokData.fld_NamaBlok;
            }

            return PartialView(hargaMenuaiViewModelCreate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _HarvestingPriceInfoCreate(Models.tbl_HargaMenuaiViewModelCreate hargaMenuaiViewModelCreate)
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
                    Models.tbl_HargaMenuai hargaMenuai = new tbl_HargaMenuai();

                    hargaMenuai.fld_KodPeringkatUtama = hargaMenuaiViewModelCreate.fld_KodPeringkatUtama;
                    hargaMenuai.fld_HargaMenuai = hargaMenuaiViewModelCreate.fld_HargaMenuai;
                    hargaMenuai.fld_CreatedBy = getuserid;
                    hargaMenuai.fld_CreatedDT = timezone.gettimezone();
                    hargaMenuai.fld_NegaraID = NegaraID;
                    hargaMenuai.fld_SyarikatID = SyarikatID;
                    hargaMenuai.fld_WilayahID = WilayahID;
                    hargaMenuai.fld_LadangID = LadangID;
                    hargaMenuai.fld_Deleted = false;

                    dbr.tbl_HargaMenuai.Add(hargaMenuai);

                    Models.tbl_HargaMenuaiHistory hargaMenuaiHistory = new tbl_HargaMenuaiHistory();

                    PropertyCopy.Copy(hargaMenuaiHistory, hargaMenuai);

                    dbr.tbl_HargaMenuaiHistory.Add(hargaMenuaiHistory);

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
                        method = "2",
                        div = "harvestingPriceDetails",
                        rootUrl = domain,
                        action = "_HarvestingPriceInfo",
                        controller = "BasicInfo",
                        paramName = "PktList",
                        paramValue = hargaMenuaiViewModelCreate.fld_JenisPeringkat
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

        public ActionResult _HarvestingPriceInfoEdit(Guid? id, string jenisPeringkat)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            Models.tbl_HargaMenuaiViewModelEdit hargaMenuaiViewModelEdit = new tbl_HargaMenuaiViewModelEdit();

            var harvestingPriceData = dbr.tbl_HargaMenuai.SingleOrDefault(x => x.fld_HargaMenuaiID == id);

            if (jenisPeringkat == "1")
            {
                var pktData = dbr.tbl_PktUtama.SingleOrDefault(x =>
                    x.fld_PktUtama == harvestingPriceData.fld_KodPeringkatUtama && x.fld_NegaraID == NegaraID &&
                    x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                    x.fld_Deleted == false);

                hargaMenuaiViewModelEdit.fld_HargaMenuaiID = (Guid)id;
                hargaMenuaiViewModelEdit.fld_JenisPeringkat = jenisPeringkat;
                hargaMenuaiViewModelEdit.fld_KodPeringkatUtama = harvestingPriceData.fld_KodPeringkatUtama;
                hargaMenuaiViewModelEdit.fld_NamaPeringkat = pktData.fld_NamaPktUtama;
            }

            else if (jenisPeringkat == "2")
            {
                var subPktData = dbr.tbl_SubPkt.SingleOrDefault(x =>
                    x.fld_Pkt == harvestingPriceData.fld_KodPeringkatUtama && x.fld_NegaraID == NegaraID &&
                    x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                    x.fld_Deleted == false);

                hargaMenuaiViewModelEdit.fld_HargaMenuaiID = (Guid)id;
                hargaMenuaiViewModelEdit.fld_JenisPeringkat = jenisPeringkat;
                hargaMenuaiViewModelEdit.fld_KodPeringkatUtama = harvestingPriceData.fld_KodPeringkatUtama;
                hargaMenuaiViewModelEdit.fld_NamaPeringkat = subPktData.fld_NamaPkt;
            }

            else
            {
                var blokData = dbr.tbl_Blok.SingleOrDefault(x =>
                    x.fld_Blok == harvestingPriceData.fld_KodPeringkatUtama && x.fld_NegaraID == NegaraID &&
                    x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                    x.fld_Deleted == false);

                hargaMenuaiViewModelEdit.fld_HargaMenuaiID = (Guid)id;
                hargaMenuaiViewModelEdit.fld_JenisPeringkat = jenisPeringkat;
                hargaMenuaiViewModelEdit.fld_KodPeringkatUtama = harvestingPriceData.fld_KodPeringkatUtama;
                hargaMenuaiViewModelEdit.fld_NamaPeringkat = blokData.fld_NamaBlok;
            }

            return PartialView(hargaMenuaiViewModelEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _HarvestingPriceInfoEdit(Models.tbl_HargaMenuaiViewModelEdit hargaMenuaiViewModelEdit)
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
                    var harvestingPriceData = dbr.tbl_HargaMenuai.SingleOrDefault(x =>
                        x.fld_HargaMenuaiID == hargaMenuaiViewModelEdit.fld_HargaMenuaiID);

                    harvestingPriceData.fld_HargaMenuai = hargaMenuaiViewModelEdit.fld_HargaMenuai;
                    harvestingPriceData.fld_CreatedBy = getuserid;
                    harvestingPriceData.fld_CreatedDT = timezone.gettimezone();

                    Models.tbl_HargaMenuaiHistory hargaMenuaiHistory = new tbl_HargaMenuaiHistory();

                    PropertyCopy.Copy(hargaMenuaiHistory, harvestingPriceData);

                    dbr.tbl_HargaMenuaiHistory.Add(hargaMenuaiHistory);

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
                        div = "harvestingPriceDetails",
                        rootUrl = domain,
                        action = "_HarvestingPriceInfo",
                        controller = "BasicInfo",
                        paramName = "PktList",
                        paramValue = hargaMenuaiViewModelEdit.fld_JenisPeringkat
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

        public JsonResult GetBatchNo(string pkjmstbatchno)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            var NSWL = GetNSWL.GetLadangDetail(LadangID.Value);

            int? convertint = 0;
            string genbatchno = "";

            var getbatchno = db.tbl_BatchRunNo.Where(x => x.fld_BatchFlag == pkjmstbatchno && x.fld_NegaraID == NSWL.fld_NegaraID && x.fld_SyarikatID == NSWL.fld_SyarikatID && x.fld_WilayahID == NSWL.fld_WilayahID && x.fld_LadangID == NSWL.fld_LadangID).FirstOrDefault();

            if (getbatchno == null)
            {
                tbl_BatchRunNo tbl_BatchRunNo = new tbl_BatchRunNo();
                tbl_BatchRunNo.fld_BatchRunNo = 2;
                tbl_BatchRunNo.fld_BatchFlag = pkjmstbatchno;
                tbl_BatchRunNo.fld_NegaraID = NegaraID;
                tbl_BatchRunNo.fld_SyarikatID = SyarikatID;
                tbl_BatchRunNo.fld_WilayahID = WilayahID;
                tbl_BatchRunNo.fld_LadangID = LadangID;
                db.tbl_BatchRunNo.Add(tbl_BatchRunNo);
                db.SaveChanges();
                convertint = 1;
                genbatchno = NSWL.fld_LdgCode.ToUpper() + "_WORKER_" + NSWL.fld_LdgCode.ToUpper() + "_" + convertint;
            }
            else
            {
                convertint = getbatchno.fld_BatchRunNo;
                genbatchno = NSWL.fld_LdgCode.ToUpper() + "_WORKER_" + NSWL.fld_LdgCode.ToUpper() + "_" + convertint;
                convertint = convertint + 1;
                getbatchno.fld_BatchRunNo = convertint;
                db.Entry(getbatchno).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(genbatchno);
        }
    }
}