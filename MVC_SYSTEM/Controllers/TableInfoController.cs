using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.ViewingModels;
using MVC_SYSTEM.App_LocalResources;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Data.Entity;
using MVC_SYSTEM.CustomModels;


//Added by Shazana on 9/9
using Rotativa;
using System.Web.Security;
using MVC_SYSTEM.log;
using Itenso.TimePeriod;

namespace MVC_SYSTEM.Controllers
{
    public class TableInfoController : Controller
    {
        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
        private GetIdentity getidentity = new GetIdentity();
        private GetConfig GetConfig = new GetConfig();
        private GetNSWL GetNSWL = new GetNSWL();
        private Connection Connection = new Connection();
        private ChangeTimeZone timezone = new ChangeTimeZone();

        private MVC_SYSTEM_MasterModels db1 = new MVC_SYSTEM_MasterModels();

        private GetIdentity GetIdentity = new GetIdentity(); //Added by Shazana on 9/9

        // GET: TableInfo
        public ActionResult Index()
        {
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? getroleid = getidentity.getRoleID(getuserid);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            int?[] reportid = new int?[] { };

            List<SelectListItem> sublist = new List<SelectListItem>();
            ViewBag.TableInfoSubList = sublist;
            ViewBag.TableInfo = "class = active";

            ViewBag.TableInfoList = new SelectList(db.tblMenuLists.Where(x => x.fld_Flag == "jadualUpah" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_Desc }), "Value", "Text").ToList();

            return View();
        }

        public JsonResult GetSubList(int ListID)
        {
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            var findsub = db.tblMenuLists.Where(x => x.fld_ID == ListID).Select(s => s.fld_Sub).FirstOrDefault();
            List<SelectListItem> sublist = new List<SelectListItem>();
            if (findsub != null)
            {
                sublist = new SelectList(db.tblMenuLists.Where(x => x.fld_Flag == findsub && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_ID).Select(s => new SelectListItem { Value = s.fld_Val, Text = s.fld_Desc }), "Value", "Text").ToList();
            }
            db.Dispose();
            return Json(sublist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string TableInfoList, string TableInfoSubList)
        {
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            if (TableInfoSubList != null)
            {
                return RedirectToAction(TableInfoSubList, "TableInfo");
            }
            else
            {
                int maintenancelist = int.Parse(TableInfoList);
                var action = db.tblMenuLists.Where(x => x.fld_ID == maintenancelist && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_Val).FirstOrDefault();
                db.Dispose();
                return RedirectToAction(action, "TableInfo");
            }
        }

        public ActionResult Payrate()
        {
            ViewBag.TableInfo = "class = active";
            return View();
        }

        public ActionResult PayrateDetail()
        {
            GetIdentity GetIdentity = new GetIdentity();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            var resultreport = db.tbl_UpahMenuai.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted==false).OrderBy(o => o.fld_HasilLower);
            return View(resultreport);
        }

        public ActionResult PayrateFull()
        {
            ViewBag.TableInfo = "class = active";
            return View();
        }

        public ActionResult PayrateFullDetails()
        {
            GetIdentity GetIdentity = new GetIdentity();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            var resultreport = db.vw_JadualUpah.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_KodAktvt);
            return View(resultreport);
        }

        public ActionResult YieldBracket()
        {
            ViewBag.TableInfo = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> JnsPkt = new List<SelectListItem>();
            JnsPkt = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnspkt" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            JnsPkt.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.JnsPktList = JnsPkt;
            return View();
        }

        public ActionResult YieldBracketDetails(string JnsPktList = "0")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            ViewBag.TableInfo = "class = active";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_SP_Models dbsp = MVC_SYSTEM_SP_Models.ConnectToSqlServer(host, catalog, user, pass);

            DateTime blnstrt = DateTime.Now.AddMonths(-12);
            DateTime blnlst = DateTime.Now.AddMonths(-1);

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            var result = dbsp.sp_YieldBracketTable(NegaraID, SyarikatID, WilayahID, LadangID, int.Parse(JnsPktList)).ToList();
            result = result.Where(x => x.fldBulan >= blnstrt.Month && x.fldTahun == blnstrt.Year).Union(result.Where(x => x.fldBulan <= blnlst.Month && x.fldTahun == blnlst.Year)).ToList();
            return PartialView(result);
        }

        //public ActionResult HasilSawit()
        //{
        //    ChangeTimeZone timezone = new ChangeTimeZone();
        //    int month = timezone.gettimezone().Month;
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    ViewBag.TableInfo = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

        //    List<SelectListItem> BulanList = new List<SelectListItem>();
        //    //List<SelectListItem> TahunList = new List<SelectListItem>();
        //    List<SelectListItem> KumpList = new List<SelectListItem>();

        //    BulanList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1== "monthlist" && x.fldDeleted==false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue.ToString(), Text = s.fldOptConfDesc}), "Value", "Text",month).ToList();
        //    BulanList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));

        //    //TahunList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist").OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue.ToString(), Text = s.fldOptConfDesc }), "Value", "Text").ToList();
        //    //TahunList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));

        //    int drpyear = 0;
        //    int drprangeyear = 0;
        //    //ChangeTimeZone timezone = new ChangeTimeZone();
        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {

        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    KumpList = new SelectList(dbr.tbl_HasilSawit.Where(x => x.fld_bulan == 9).OrderBy(o => o.fld_kum).Select(s => new SelectListItem { Value = s.fld_kum, Text = s.fld_kum }), "Value", "Text").Distinct().ToList();
        //    KumpList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));
        //    //int blnselected = BulanList.FirstOrDefault();
        //    var resultreport = dbr.tbl_HasilSawit.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_kdldg == "399" && x.fld_bulan== month && x.fld_tahun== drprangeyear);
        //    //KumpList = new SelectList(dbr.tbl_YieldBracket.Where(x => x.fld_bulan == 9).GroupBy(g => g.fld_kum), "fld_kum", "fld_kum").ToList();
        //    //KumpList.Insert(0, (new SelectListItem { Text = "Semua", Value = "ALL" }));
        //    ViewBag.BulanList = BulanList;
        //    ViewBag.TahunList = yearlist;
        //    ViewBag.KumpulanList = KumpList;
        //    return View("HasilSawit", resultreport);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult HasilSawit(int BulanList, int yearlist, string KumpList)
        //{
        //    ChangeTimeZone timezone = new ChangeTimeZone();
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    int drpyear = 0;
        //    int drprangeyear = 0;

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
        //    List<SelectListItem> BulanList2 = new List<SelectListItem>();
        //    List<SelectListItem> KumpList2 = new List<SelectListItem>();

        //    BulanList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue.ToString(), Text = s.fldOptConfDesc }), "Value", "Text", month).ToList();
        //    BulanList2.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));

        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;
        //    var yearlist2 = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {

        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    KumpList2 = new SelectList(dbr.tbl_HasilSawit.Where(x => x.fld_bulan == 9).OrderBy(o => o.fld_kum).Select(s => new SelectListItem { Value = s.fld_kum, Text = s.fld_kum }), "Value", "Text").Distinct().ToList();
        //    KumpList2.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));
        //    var resultreport = dbr.tbl_HasilSawit.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_kdldg == "399" && x.fld_bulan == BulanList && x.fld_tahun == yearlist && x.fld_kum== KumpList);
        //    ViewBag.BulanList = BulanList2;
        //    ViewBag.TahunList = yearlist2;
        //    ViewBag.KumpulanList = KumpList2;
        //    return View("HasilSawit", resultreport);
        //}

        public ActionResult WorkerAktvt(string Search)
        {
            GetIdentity GetIdentity = new GetIdentity();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            ViewBag.TableInfo = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);


            if (Search != "" && Search != null)
            {
                var resultreport = dbr.tbl_AktvtKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID==WilayahID && (x.fld_Lejar.Contains(Search) || x.fld_Aktvt.Contains(Search)));
                return View("WorkerAktvt", resultreport);
            }
            else
            {
                var resultreport = dbr.tbl_AktvtKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID);
                return View("WorkerAktvt", resultreport);
            }
        }

        public ActionResult OilPrice()
        {
            ViewBag.TableInfo = "class = active";
            ChangeTimeZone timezone = new ChangeTimeZone();
            int drpyear = 0;
            int drprangeyear = 0;
            int month = timezone.gettimezone().Month;
            int year = timezone.gettimezone().Year;
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> BulanList = new List<SelectListItem>();
            BulanList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfID).Select(s => new SelectListItem { Value = s.fldOptConfValue.ToString(), Text = s.fldOptConfDesc }), "Value", "Text", month).ToList();
            BulanList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

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
            ViewBag.Bulan = month;
            ViewBag.Tahun = year;
            ViewBag.BulanList = BulanList;
            ViewBag.TahunList = yearlist;
            return View();
        }

        public ActionResult OilPriceDetail(string Bulan, int Tahun)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            int bulan = int.Parse(Bulan);
            var resultreport = db.tbl_HargaSawitSemasa.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Bulan == bulan && x.fld_Tahun == Tahun && x.fld_Deleted == false).ToList();
            ViewBag.Bulan = bulan;
            ViewBag.Tahun = Tahun;
            return View(resultreport);
        }

        public ActionResult KwspSocso()
        {
            return View();
        }

        public ActionResult Socso()
        {
            ViewBag.TableInfo = "class = active";
            return View();
        }

        public ActionResult SocsoDetail()
        {
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            var resultreport = db.vw_Socso.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_KdrLower);
            return View(resultreport);
        }

        public ActionResult Kwsp()
        {
            ViewBag.TableInfo = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> KwspList2 = new List<SelectListItem>();
            KwspList2 = new SelectList(db.tbl_JenisCaruman.Where(x =>x.fldNegaraID==NegaraID && x.fldSyarikatID==SyarikatID && x.fld_JenisCaruman == "KWSP" && x.fld_Deleted == false ).OrderBy(o => o.fld_KodCaruman).Select(s => new SelectListItem { Value = s.fld_KodCaruman.ToString(), Text = s.fld_Keterangan }), "Value", "Text").ToList();

            ViewBag.KwspList = KwspList2;
            return View();
        }
        public ActionResult KwspDetail(string KwspList = "K01")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            var resultreport = db.tbl_Kwsp.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false && x.fld_KodCaruman == KwspList);
            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.JnsKwsp = db.tbl_JenisCaruman.Where(x => x.fld_KodCaruman == KwspList && x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_Keterangan).FirstOrDefault();
            return View(resultreport);
        }

        public ActionResult Productivity(string Search)
        {
            GetIdentity GetIdentity = new GetIdentity();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            ViewBag.TableInfo = "class = active";


            if (Search != "" && Search != null)
            {
                var resultreport = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false && (x.fld_Desc.Contains(Search) || x.fld_KodAktvt.Contains(Search))).OrderBy(o=>o.fld_Kategori);
                return View("Productivity", resultreport);
            }
            else
            {
                var resultreport = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o=>o.fld_Kategori);
                return View("Productivity", resultreport);
            }
        }

        public ActionResult AllocatedPublicHolidayTable()
        {
            ViewBag.TableInfo = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,NegaraID.Value);

            int drpyear = 0;
            int drprangeyear = 0;
            int month = timezone.gettimezone().Month;

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
            return View();
        }

        public ActionResult _AllocatedPublicHolidayTable(short? YearList, int page = 1, string sort = "fld_TarikhCuti", string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,NegaraID.Value);


            var message = "";
            if (String.IsNullOrEmpty(YearList.ToString()))
            {
                message = GlobalResEstate.msgChooseAllocatedPublicHoliday;
                ViewBag.Message = message;
            }

            else
            {
                message = GlobalResEstate.msgErrorSearch;
                ViewBag.Message = message;
            }
            MVC_SYSTEM_Viewing dbview = new MVC_SYSTEM_Viewing();

            var publiHolidayData = dbview.vw_CutiUmumLdgDetails.Where(x =>
                x.fld_Year == YearList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false &&
                x.fld_CutiUmumDeleted == false); ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            return View(publiHolidayData);
        }

        public ActionResult PayrateGMN()
        {
            ViewBag.TableInfo = "class = active";
            GetIdentity GetIdentity = new GetIdentity();

            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            //MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);


            List<SelectListItem> KategoriList = new List<SelectListItem>();
            KategoriList = new SelectList(db.tbl_KategoriAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID )
                .OrderBy(o => o.fld_ID)
                .Select(s => new SelectListItem { Value = s.fld_KodKategori.ToString(), Text = s.fld_KodKategori + " - " + s.fld_Kategori }),"Value", "Text").ToList();
            KategoriList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "" }));

            ViewBag.SelectionList = KategoriList;
            //ViewBag.Select = "";
            return View();
        }

        public ActionResult PayrateGMNDetails(string Search , string Kategori)
        {
            GetIdentity GetIdentity = new GetIdentity();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            if (Kategori != "" && Kategori != null)
            {
                if (Search != "" && Search != null)
                {
                    var resultreport = db.vw_UpahGMN.Where(x => x.fld_KategoriAktvt == Kategori && (x.fld_JnsAktvt.Contains(Search) && x.fld_Desc.Contains(Search) || x.fld_KodAktvt.Contains(Search)) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_KodAktvt).Distinct();
                    return View(resultreport);
                }
                else
                {
                    var resultreport = db.vw_UpahGMN.Where(x => x.fld_KategoriAktvt == Kategori && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_KodAktvt).Distinct();
                    return View(resultreport);
                }
            }
            else
            {
                if (Search != "" && Search != null)
                {
                    var resultreport = db.vw_UpahGMN.Where(x => (x.fld_JnsAktvt.Contains(Search) && x.fld_Desc.Contains(Search) || x.fld_KodAktvt.Contains(Search)) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_KodAktvt).Distinct();
                    return View(resultreport);
                }
                else
                {
                    var resultreport = db.vw_UpahGMN.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_KodAktvt).Distinct();
                    return View(resultreport);
                }
            }
        }

        public ActionResult AddedContribution()
        {
            ViewBag.TableInfo = "class = active";
            GetIdentity GetIdentity = new GetIdentity();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> ContributionList = new List<SelectListItem>();
            List<SelectListItem> SubContributionList = new List<SelectListItem>();
            ContributionList = new SelectList(db.tbl_CarumanTambahan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_JenisCarumanID).Select(s => new SelectListItem { Value = s.fld_KodCaruman, Text = s.fld_NamaCaruman }), "Value", "Text").ToList();
            ViewBag.ContList = ContributionList;
            ViewBag.SubContList = SubContributionList;
            return View();
        }

        public ActionResult AddedContributionDetail(string subcont)
        {
            GetIdentity GetIdentity = new GetIdentity();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            var resultreport = db.tbl_JadualCarumanTambahan.Where(x => x.fld_KodSubCaruman == subcont && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false);
            ViewBag.NamaCaruman = db.tbl_SubCarumanTambahan.Where(x => x.fld_KodSubCaruman == subcont && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_KeteranganSubCaruman).FirstOrDefault();
            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            return View(resultreport);
        }

        public JsonResult GetSubContribution(string cont)
        {
            GetIdentity GetIdentity = new GetIdentity();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> SubContList = new List<SelectListItem>();
            SubContList = new SelectList(db.tbl_SubCarumanTambahan.Where(x => x.fld_KodCaruman == cont && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_JenisSubCarumanID).Select(s => new SelectListItem { Value = s.fld_KodSubCaruman, Text = s.fld_KeteranganSubCaruman }), "Value", "Text").ToList();

            string subcont = db.tbl_SubCarumanTambahan.Where(x => x.fld_KodCaruman == cont && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_KodSubCaruman).FirstOrDefault();
            return Json(new { SubContList = SubContList, subcont = subcont });
        }

        public ActionResult AllocatedYearlyHolidayTable()
        {
            ViewBag.TableInfo = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            return View();
        }

        public ActionResult _AllocatedYearlyHolidayTable(int page = 1, string sort = "fld_TarikhCuti", string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            var annualLeaveData = db.tbl_CutiMaintenance.Where(x =>
                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false &&
                x.fld_JenisCuti == "C02");

            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NamaSyarikat).FirstOrDefault();

            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();

            return View(annualLeaveData);
        }

        public ActionResult AllocatedSickHolidayTable()
        {
            ViewBag.TableInfo = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            return View();
        }

        public ActionResult _AllocatedSickHolidayTable(int page = 1, string sort = "fld_TarikhCuti", string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            var sickLeaveData = db.tbl_CutiMaintenance.Where(x =>
                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false &&
                x.fld_JenisCuti == "C03");

            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NamaSyarikat).FirstOrDefault();

            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();

            return View(sickLeaveData);
        }

        public ActionResult HarvestingPriceTable()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            ViewBag.TableInfo = "class = active";

            List<SelectListItem> pktList = new List<SelectListItem>();

            pktList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "jnspkt" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfID)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            pktList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.PktList = pktList;

            return View();
        }

        public ActionResult _HarvestingPriceTable(string PktList, string filter)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            List<CustMod_HargaMenuai> hargaMenuaiList = new List<CustMod_HargaMenuai>();

            var message = @GlobalResEstate.msgNoRecord;

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

                else if (PktList == "3")
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

                else
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
                if (PktList == "1")
                {
                    var pktUtamaData = dbr.tbl_PktUtama.Where(x =>
                        x.fld_PktUtama.Contains(filter) || x.fld_NamaPktUtama.Contains(filter) &&
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
                        x.fld_Pkt.Contains(filter) || x.fld_NamaPkt.Contains(filter) &&
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

                else if (PktList == "3")
                {
                    var blokData = dbr.tbl_Blok.Where(x =>
                        x.fld_Blok.Contains(filter) || x.fld_NamaBlok.Contains(filter) &&
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

                else
                {
                    var pktUtamaData = dbr.tbl_PktUtama.Where(x =>
                        x.fld_PktUtama.Contains(filter) || x.fld_NamaPktUtama.Contains(filter) &&
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

                    var subPktData = dbr.tbl_SubPkt.Where(x =>
                        x.fld_Pkt.Contains(filter) || x.fld_NamaPkt.Contains(filter) &&
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

                    var blokData = dbr.tbl_Blok.Where(x =>
                        x.fld_Blok.Contains(filter) || x.fld_NamaBlok.Contains(filter) &&
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

            ViewBag.Message = message;

            return View(hargaMenuaiList);
        }

        public ActionResult PayratePUP()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            ViewBag.TableInfo = "class = active";

            List<SelectListItem> activityLevelList = new List<SelectListItem>();

            activityLevelList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "activityLevel" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfID)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            activityLevelList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.ActivityLevelList = activityLevelList;

            return View();
        }

        public ActionResult _PayratePUP(string ActivityLevelList, string FreeText, string print) //Edited by Shazana on 9/9
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            List<tbl_UpahAktiviti> upahAktivitiData = new List<tbl_UpahAktiviti>(); //Edited by Shazana on 9/9

            var message = @GlobalResEstate.msgNoRecord;

            if (ActivityLevelList == "0" || ActivityLevelList == null) //Edited by Shazana on 9/9
            {
                if (string.IsNullOrEmpty(FreeText))
                {
                    upahAktivitiData = db.tbl_UpahAktiviti.Where(x =>
                      x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&  // Commented by Shazana on 9/9 x.fld_WilayahID == WilayahID &&
                      x.fld_Deleted == false).OrderBy(o => o.fld_ID).ToList(); //Edited by Shazana on 9/9

                    // Commented by Shazana on 9/9  upahAktivitiList.AddRange(upahAktivitiData);
                }

                else
                {

                    ////Commented by Shazana on 9/9
                    // upahAktivitiData = db.tbl_UpahAktiviti.Where(x =>
                    //    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                    //    x.fld_Deleted == false).OrderBy(o => o.fld_ID).Where(x => x.fld_KodAktvt.Contains(filter) || x.fld_Desc.Contains(filter));
                    //upahAktivitiList.AddRange(upahAktivitiData);
                    //Added by Shazana on 9/9
                    upahAktivitiData = db.tbl_UpahAktiviti.Where(x =>
                       x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                       x.fld_Deleted == false && x.fld_KodAktvt.Contains(FreeText) || x.fld_Desc.Contains(FreeText)).OrderBy(o => o.fld_ID).ToList(); //Edited by Shazana on 9/9
                }
            }

            else // if  Commented by Shazana on 9/9 (String.IsNullOrEmpty(ActivityLevelList))
            {
                if (string.IsNullOrEmpty(FreeText))
                {
                    //Commented by Shazana on 9/9
                    // upahAktivitiData = db.tbl_UpahAktiviti.Where(x =>
                    //    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                    //    x.fld_Deleted == false).OrderBy(o => o.fld_ID).ToList();
                    //upahAktivitiList.AddRange(upahAktivitiData);

                    upahAktivitiData = db.tbl_UpahAktiviti.Where(x =>
                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJenisAktvt == ActivityLevelList &&
                    x.fld_Deleted == false).OrderBy(o => o.fld_ID).ToList();

                }

                else
                {
                    ////Commented by Shazana on 9/9
                    // upahAktivitiData = db.tbl_UpahAktiviti.Where(x =>
                    //    x.fld_KodAktvt.Contains(filter) || x.fld_Desc.Contains(filter) &&
                    //    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                    //    x.fld_Deleted == false).OrderBy(o => o.fld_ID); 
                    // upahAktivitiList.AddRange(upahAktivitiData);

                    upahAktivitiData = db.tbl_UpahAktiviti.Where(x =>
                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJenisAktvt == ActivityLevelList &&
                    x.fld_Deleted == false && x.fld_KodAktvt.Contains(FreeText) || x.fld_Desc.Contains(FreeText)).OrderBy(o => o.fld_ID).ToList();

                }
            }
            ViewBag.Print = print; //Added by Shazana on 9/9
            // Commented by Shazana on 9/9
            //else
            //{
            //    if (String.IsNullOrEmpty(filter))
            //    {
            //        var upahAktivitiData = db.tbl_UpahAktiviti.Where(x => x.fld_KodJenisAktvt.Contains(ActivityLevelList) &&
            //            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
            //            x.fld_Deleted == false).OrderBy(o => o.fld_ID);

            //        upahAktivitiList.AddRange(upahAktivitiData);
            //    }

            //    else
            //    {
            //        var upahAktivitiData = db.tbl_UpahAktiviti.Where(x =>
            //            x.fld_KodJenisAktvt.Contains(ActivityLevelList) &&
            //            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
            //            x.fld_Deleted == false).OrderBy(o => o.fld_ID).Where(x => x.fld_KodAktvt.Contains(filter) || x.fld_Desc.Contains(filter)); ;

            //        upahAktivitiList.AddRange(upahAktivitiData);
            //    }
            //}

            ViewBag.Message = message;

            //Commented by Shazana on 9/9return View(upahAktivitiList); 
            return View(upahAktivitiData);
        }


        //Added by Shazana on 9/9
        public void getBackAuth(string getcookiesval)
        {
            string CookiesValue = "";
            try
            {
                CookiesValue = Request.Cookies[FormsAuthentication.FormsCookieName].Value;
                if (CookiesValue != getcookiesval)
                {
                    HttpCookie cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                    cookie.Value = getcookiesval;
                    Response.Cookies.Add(cookie);
                }
            }
            catch
            {
                HttpCookie authoCookies = new HttpCookie(FormsAuthentication.FormsCookieName, getcookiesval);
                Response.SetCookie(authoCookies);
            }
        }

        public bool CheckGenIdentity(int id, string genid, int? userid, string username, out string CookiesValue)
        {
            bool result = false;
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, userid, username);

            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);

            CookiesValue = "";

            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                Guid genidC = Guid.Parse(genid);
                var CheckIdentity = dbr.tbl_PdfGen.Where(x => x.fld_ID == genidC && x.fld_UserID == id).FirstOrDefault();

                if (CheckIdentity == null)
                {
                    result = false;
                }
                else
                {
                    result = true;
                    CookiesValue = CheckIdentity.fld_CookiesVal;
                }
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public ActionResult PrintWorkerPdf(string ActivityLevelList, string FreeText, int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db1.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }
            ViewBag.Message =
            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("_PayratePUP", new { ActivityLevelList, FreeText, print })
                {
                    FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName,
                    Cookies = cookies
                };
            }
            else
            {
                report = new ActionAsPdf("PDFInvalid");
            }

            return report;
        }
    }
}