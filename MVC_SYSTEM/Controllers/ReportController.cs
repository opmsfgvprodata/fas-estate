using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.ViewingModels;
using MVC_SYSTEM.App_LocalResources;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using MVC_SYSTEM.CustomModels;
using Org.BouncyCastle.Utilities.Collections;
using System.Web.Security;
using System.Web.Script.Serialization;
using MVC_SYSTEM.log;
using Rotativa;
using Itenso.TimePeriod;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.CustomModels2;
using PagedList; // Add by fitri 17-09-2020
using Dapper;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    //Check_Balik
    public class ReportController : Controller
    {
        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
        GetIdentity GetIdentity = new GetIdentity();
        GetNSWL GetNSWL = new GetNSWL();
        Connection Connection = new Connection();
        ChangeTimeZone timezone = new ChangeTimeZone();
        GetConfig GetConfig = new GetConfig();
        errorlog geterror = new errorlog();
        GetWilayah getwilyah = new GetWilayah();
        // GET: Report
        public ActionResult Index()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            List<SelectListItem> sublist = new List<SelectListItem>();
            ViewBag.ReportSubList = sublist;
            ViewBag.Report = "class = active";
            ViewBag.ReportList = new SelectList(db.tblMenuLists.Where(x => x.fld_Flag == "report" || x.fld_Flag == "report2" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                 .Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_Desc }), "Value", "Text").ToList();
            db.Dispose();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string ReportList, string ReportSubList)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            if (ReportSubList != null)
                if (ReportSubList == "LprnBlnnByrnAktvt" || ReportSubList == "LprnThnPrblnjaanAktvtKrja" || ReportSubList == "LprnKrjaTrprnc" || ReportSubList == "LprnKrjaTrprncBulanan" || ReportSubList == "LprnKrjaBlnan" || ReportSubList == "LprnKrjaHarian")
                {
                    return RedirectToAction(ReportSubList, "Report2");
                }
                else
                {
                    return RedirectToAction(ReportSubList, "Report");
                }

            else
            {
                int reportList = int.Parse(ReportList);
                var action = db.tblMenuLists.Where(x => x.fld_ID == reportList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_Val).FirstOrDefault();
                db.Dispose();
                return RedirectToAction(action, "Report");

            }
        }
        public JsonResult GetSubList(int ListID)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            var findsub = db.tblMenuLists.Where(x => x.fld_ID == ListID).Select(s => s.fld_Sub).FirstOrDefault();
            List<SelectListItem> sublist = new List<SelectListItem>();
            if (findsub != null)
            {   //original code
                //sublist = new SelectList(db.tblMenuLists.Where(x => x.fld_Flag == findsub && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                //    .OrderBy(o => o.fld_Desc)
                //    .Select(s => new SelectListItem { Value = s.fld_Val, Text = s.fld_Desc }), "Value", "Text").ToList();
                //modified by faeza 16.10.2020
                sublist = new SelectList(db.tblMenuLists.Where(x => x.fld_Flag == findsub && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fld_ID)
                    .Select(s => new SelectListItem { Value = s.fld_Val, Text = s.fld_Desc }), "Value", "Text").ToList();
            }
            db.Dispose();
            return Json(sublist);
        }

        public ActionResult WorkerReport()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> StatusList = new List<SelectListItem>();
            List<SelectListItem> SelectionList = new List<SelectListItem>();
            List<SelectListItem> JnsPkjList = new List<SelectListItem>();

            StatusList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            StatusList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            SelectionList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nopkj).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }), "Value", "Text").ToList();
            SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            JnsPkjList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            JnsPkjList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
            //SelectionList = new SelectList(dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID==NegaraID && x.fld_SyarikatID==SyarikatID && x.fld_WilayahID==WilayahID && x.fld_LadangID==LadangID && x.fld_deleted==false).OrderBy(o => o.fld_KodKumpulan).Select(s => new SelectListItem { Value = s.fld_KodKumpulan, Text = s.fld_KodKumpulan + "-" + s.fld_Keterangan }), "Value", "Text").ToList();
            //SelectionList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));

            ViewBag.StatusList = StatusList;
            ViewBag.SelectionList = SelectionList;
            ViewBag.JnsPkjList = JnsPkjList;
            ViewBag.getflag = 1;
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult WorkerReport(int RadioGroup, string StatusList, string SelectionList, string print)
        //{
        //    ViewBag.Report = "class = active";
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = GetIdentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

        //    List<SelectListItem> StatusList2 = new List<SelectListItem>();
        //    List<SelectListItem> SelectionList2 = new List<SelectListItem>();
        //    StatusList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text", StatusList).ToList();
        //    StatusList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

        //    ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //    ViewBag.StatusList = StatusList2;
        //    ViewBag.Print = print;

        //    if (RadioGroup == 0)
        //    {
        //        //Individu Semua
        //        if (StatusList == "0")
        //        {
        //            SelectionList2 = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nopkj).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }), "Value", "Text").ToList();
        //            SelectionList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
        //            if (SelectionList == "0")
        //            {
        //                //individu semua pekerja
        //                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);
        //                ViewBag.SelectionList = SelectionList2;
        //                ViewBag.getflag = 2;
        //                return View(result);
        //            }
        //            else
        //            {
        //                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == SelectionList);
        //                ViewBag.SelectionList = SelectionList2;
        //                ViewBag.getflag = 2;
        //                return View(result);
        //            }
        //        }
        //        else
        //        {
        //            SelectionList2 = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == StatusList).OrderBy(o => o.fld_Nopkj).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }), "Value", "Text").ToList();
        //            SelectionList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
        //            if (SelectionList == "0")
        //            {
        //                //individu aktif/xaktif pekerja
        //                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == StatusList);
        //                ViewBag.SelectionList = SelectionList2;
        //                ViewBag.getflag = 2;
        //                return View(result);
        //            }
        //            else
        //            {
        //                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == StatusList && x.fld_Nopkj == SelectionList);
        //                ViewBag.SelectionList = SelectionList2;
        //                ViewBag.getflag = 2;
        //                return View(result);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        //Group
        //        SelectionList2 = new SelectList(dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).OrderBy(o => o.fld_KodKumpulan).Select(s => new SelectListItem { Value = s.fld_KodKumpulan, Text = s.fld_KodKumpulan + "-" + s.fld_Keterangan }), "Value", "Text").ToList();
        //        SelectionList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
        //        if (SelectionList == "0")
        //        {
        //            //semua kump
        //            var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_KumpulanID != null); ViewBag.SelectionList = SelectionList2;
        //            ViewBag.getflag = 2;
        //            return View(result);
        //        }
        //        else
        //        {
        //            //by kump
        //            int getkump = dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false && x.fld_KodKumpulan == SelectionList).Select(s => s.fld_KumpulanID).FirstOrDefault();
        //            var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_KumpulanID == getkump);
        //            ViewBag.SelectionList = SelectionList2;
        //            ViewBag.getflag = 2;
        //            return View(result);
        //        }
        //    }
        //}

        public ActionResult GroupReport()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> GroupList = new List<SelectListItem>();
            List<SelectListItem> JnsPkjList = new List<SelectListItem>();
            GroupList = new SelectList(dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false && x.bilangan_ahli >= 0).OrderBy(o => o.fld_KodKumpulan).Select(s => new SelectListItem { Value = s.fld_KodKumpulan, Text = s.fld_KodKumpulan + "-" + s.fld_Keterangan }), "Value", "Text").ToList();
            GroupList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            JnsPkjList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            JnsPkjList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.GroupList = GroupList;
            ViewBag.JnsPkjList = JnsPkjList;
            //ViewBag.getflag = 1;
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult _GroupReport(string GroupList, string JnsPkjList, string print)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            //List<Models.tbl_Pkjmast> InfoKmpln = new List<Models.tbl_Pkjmast>();
            List<SelectListItem> GroupList2 = new List<SelectListItem>();
            List<SelectListItem> JnsPkjList2 = new List<SelectListItem>();
            GroupList2 = new SelectList(dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false && x.bilangan_ahli >= 0).OrderBy(o => o.fld_KodKumpulan).Select(s => new SelectListItem { Value = s.fld_KodKumpulan, Text = s.fld_KodKumpulan + "-" + s.fld_Keterangan }), "Value", "Text").ToList();
            GroupList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            JnsPkjList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            JnsPkjList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.Print = print;

            if (GroupList == null && JnsPkjList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChooseGroup;
                return View();
            }

            if (GroupList == "0")
            {
                if (JnsPkjList == "0")
                {
                    var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_KumpulanID != null && x.fld_Kdaktf == "1").OrderBy(o => o.fld_KumpulanID);
                    
                    ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                    ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                    if (result.Count() == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }
                    ViewBag.GroupList = GroupList2;
                    //ViewBag.getflag = 2;
                    return View(result);
                }
                else
                {
                    var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_KumpulanID != null && x.fld_Kdaktf == "1" && x.fld_Jenispekerja == JnsPkjList).OrderBy(o => o.fld_KumpulanID);
                   
                    ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                    ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                    if (result.Count() == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }
                    ViewBag.GroupList = GroupList2;
                    //ViewBag.getflag = 2;
                    return View(result);
                }
               
            }
            else 
            {
                if (JnsPkjList == "0")
                {
                    int groupID = dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false && x.fld_KodKumpulan == GroupList).Select(s => s.fld_KumpulanID).FirstOrDefault();
                    var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_KumpulanID == groupID && x.fld_Kdaktf == "1");
                    
                    ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                    ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                    if (result.Count() == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }
                    ViewBag.GroupList = GroupList2;
                    //ViewBag.getflag = 2;
                    return View(result);
                }
                else
                {
                    int groupID = dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false && x.fld_KodKumpulan == GroupList).Select(s => s.fld_KumpulanID).FirstOrDefault();
                    var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_KumpulanID == groupID && x.fld_Kdaktf == "1" && x.fld_Jenispekerja == JnsPkjList);
                    
                    ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                    ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                    if (result.Count() == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }
                    ViewBag.GroupList = GroupList2;
                    //ViewBag.getflag = 2;
                    return View(result);
                }
               
            }
            

        }

        public ActionResult AccountReport()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            ViewBag.Report = "class = active";
            List<SelectListItem> StatusList = new List<SelectListItem>();
            StatusList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            StatusList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            List<SelectListItem> JnsPkjList = new List<SelectListItem>();
            JnsPkjList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            JnsPkjList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.StatusList = StatusList;
            ViewBag.JnsPkjList = JnsPkjList;
            ViewBag.getflag = 1;
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult _AccReport(string StatusList, string JnsPkjList, string print)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<Models.tbl_Pkjmast> AccPekerja = new List<Models.tbl_Pkjmast>();

            List<SelectListItem> StatusList2 = new List<SelectListItem>();
            StatusList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            StatusList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            List<SelectListItem> JnsPkjList2 = new List<SelectListItem>();
            JnsPkjList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            JnsPkjList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.StatusList = StatusList2;
            ViewBag.JnsPkjList = JnsPkjList2;
            ViewBag.getflag = 2;
            ViewBag.Print = print;

            if (StatusList == null && JnsPkjList == null)
            {
                ViewBag.Message = GlobalResEstate.lblChooseAcc;
                return View(AccPekerja);
            }


            if (StatusList == "0")
            {
                if (JnsPkjList == "0")
                {
                    var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1");
                    ViewBag.UserID = getuserid;
                    return View(result);
                }
                else
                {
                    var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_Jenispekerja == JnsPkjList);
                    ViewBag.UserID = getuserid;
                    return View(result);
                }
                    
            }
            else
            {
                if (JnsPkjList == "0")
                {
                    var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_StatusAkaun == StatusList);
                    ViewBag.UserID = getuserid;
                    return View(result);
                }
                else
                {
                    var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_StatusAkaun == StatusList && x.fld_Jenispekerja == JnsPkjList);
                    ViewBag.UserID = getuserid;
                    return View(result);
                }
                
            }
        }

        public ActionResult KwspSocsoReport()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            ViewBag.Report = "class = active";
            List<SelectListItem> StatusList = new List<SelectListItem>();
            StatusList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            StatusList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            List<SelectListItem> JnsPkjList = new List<SelectListItem>();
            JnsPkjList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            JnsPkjList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.StatusList = StatusList;
            //ViewBag.getflag = 1;
            ViewBag.JnsPkjList = JnsPkjList;
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult _KwspSocsoReport(string StatusList , string JnsPkjList, string print)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<Models.tbl_Pkjmast> KwspSocsoPekerja = new List<Models.tbl_Pkjmast>();

            List<SelectListItem> StatusList2 = new List<SelectListItem>();
            StatusList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            StatusList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            List<SelectListItem> JnsPkjList2 = new List<SelectListItem>();
            JnsPkjList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            JnsPkjList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.StatusList = StatusList2;
            //ViewBag.getflag = 2;
            ViewBag.JnsPkjList = JnsPkjList2;
            ViewBag.Print = print;

            if (StatusList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChooseAcc;
                return View(KwspSocsoPekerja);
            }

            else if (StatusList == "0")
            {
                if (JnsPkjList == "0")
                {
                    var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1");
                    return View(result);
                }
                else
                {
                    var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_Jenispekerja == JnsPkjList);
                    return View(result);
                }
                
            }
            else
            {
                if (JnsPkjList == "0")
                {
                    var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_StatusKwspSocso == StatusList);
                    return View(result);
                }
                else
                {
                    var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_StatusKwspSocso == StatusList && x.fld_Jenispekerja == JnsPkjList);
                    return View(result);
                }
                
            }
        }

        public ActionResult WorkReport()
        {
            ViewBag.Report = "class = active";
            int month = timezone.gettimezone().AddMonths(-1).Month;
            int year = timezone.gettimezone().Year;
            int rangeyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;

            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

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
            //string[] nopkj = dbr.tbl_Kerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_Nopkj).ToArray();
            List<SelectListItem> WorkerList = new List<SelectListItem>();
            // Comment by fitri 4-11-2020
            //WorkerList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1").Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + " - " + s.fld_Nama}).Distinct(), "Value", "Text").ToList();
            //WorkerList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nama).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + " - " + s.fld_Nama }).Distinct(), "Value", "Text").ToList();
            //WorkerList = new SelectList(dbr.tbl_Kerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o => o.fld_Nopkj).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj }).Distinct(), "Value", "Text").ToList();
            // Add by fitri 4-11-2020
            WorkerList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nama).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + " - " + s.fld_Nama }), "Value", "Text").ToList();

            WorkerList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            // Add by fitri 15-09-2020
            var PilihanPkt = new List<SelectListItem>();
            // Edit by fitri 05-02-2021
            var peringkatList = new SelectList(dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_PktUtama).Select(s => new SelectListItem { Value = s.fld_PktUtama, Text = s.fld_PktUtama + " - " + s.fld_NamaPktUtama }), "Value", "Text").ToList();
            PilihanPkt.AddRange(peringkatList);
            // Edit by fitri 05-02-2021
            var subPktList = new SelectList(dbr.tbl_SubPkt.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_Pkt).Select(s => new SelectListItem { Value = s.fld_Pkt, Text = s.fld_Pkt + " - " + s.fld_NamaPkt }), "Value", "Text").ToList();
            PilihanPkt.AddRange(subPktList);
            PilihanPkt.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            //List<SelectListItem> JnsPkjList = new List<SelectListItem>();
            //JnsPkjList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            //JnsPkjList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.MonthList = MonthList;
            ViewBag.YearList = yearlist;
            ViewBag.WorkerList = WorkerList;
            ViewBag.PilihanPkt = PilihanPkt; //Add by fitri 15-09-2020
            //ViewBag.JnsPkjList = JnsPkjList;
            ViewBag.getflag = 1;
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult _WorkReport(int? MonthList, int? YearList, string WorkerList, string PilihanPkt, string print) //Add by fitri 15-09-2020
        {
            ViewBag.Report = "class = active";
            int month = timezone.gettimezone().AddMonths(-1).Month;
            int year = timezone.gettimezone().Year;
            int rangeyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;

            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

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

            var MonthList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", month);

            string[] nopkj = dbr.tbl_Kerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_Nopkj).ToArray();
            // Add by fitri 15-09-2020
            string[] pkt = dbr.tbl_Kerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_KodPkt).ToArray();

            List<SelectListItem> WorkerList2 = new List<SelectListItem>();
            WorkerList2 = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
            nopkj.Contains(x.fld_Nopkj) && x.fld_Kdaktf == "1")
            .OrderBy(o => o.fld_Nama)
            .Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + " - " + s.fld_Nama }).Distinct(), "Value", "Text").ToList();
            WorkerList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));


            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.MonthList = MonthList2;
            ViewBag.YearList = yearlist;
            ViewBag.WorkerList = WorkerList2;
            ViewBag.MonthSelection = MonthList;
            ViewBag.YearSelection = YearList;
            ViewBag.getflag = 2;
            ViewBag.Print = print;

            if (MonthList == null && YearList == null && WorkerList == null && PilihanPkt == null) //Add by fitri 15-09-2020
            {
                //ViewBag.Message = GlobalResEstate.msgChooseWork; //Comment by fitri 15-09-2020
                ViewBag.Message = "Sila pilih bulan, tahun, pekerja, dan peringkat"; //Add by fitri 15-09-2020
                return View();
            }
            //Comment by fitri 15-09-2020
            //if (WorkerList == "0" && PilihanPkt == "0") 
            //{
            //    var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && nopkj.Contains(x.fld_Nopkj)).OrderBy(o => o.fld_Tarikh);
            //    if (result.ToList().Count() == 0)
            //    {
            //        ViewBag.Message = "Tiada Maklumat";
            //    }
            //    return View(result);
            //}
            //else
            //{
            //    var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_Nopkj == WorkerList).OrderBy(o => o.fld_Tarikh);
            //    if (result.ToList().Count() == 0)
            //    {
            //        ViewBag.Message = "Tiada Maklumat";
            //    }
            //    return View(result);
            //}
            //Add by fitri 15-09-2020
            if (WorkerList == "0" && PilihanPkt == "0")
            {
                var result = dbr.tbl_Kerja.Where(x => pkt.Contains(x.fld_KodPkt) && x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && nopkj.Contains(x.fld_Nopkj)).OrderBy(o => o.fld_Tarikh);
                if (result.ToList().Count() == 0)
                {
                    ViewBag.Message = "Tiada Maklumat";
                }
                return View(result);
            }
            else if (WorkerList != "0" && PilihanPkt == "0")
            {
                var result = dbr.tbl_Kerja.Where(x => pkt.Contains(x.fld_KodPkt) && x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_Nopkj == WorkerList).OrderBy(o => o.fld_Tarikh);
                if (result.ToList().Count() == 0)
                {
                    ViewBag.Message = "Tiada Maklumat";
                }
                return View(result);
            }
            else
            {
                var result = dbr.tbl_Kerja.Where(x => x.fld_KodPkt == PilihanPkt && x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_Nopkj == WorkerList).OrderBy(o => o.fld_Tarikh);
                if (result.ToList().Count() == 0)
                {
                    ViewBag.Message = "Tiada Maklumat";
                }
                return View(result);
            }
        }

        public ActionResult ExpiredNotiReport()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            ViewBag.Report = "class = active";
            ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "exprdmonthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc");

            List<SelectListItem> JnsPkjList = new List<SelectListItem>();
            JnsPkjList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            JnsPkjList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.JnsPkjList = JnsPkjList;
            ViewBag.getflag = 1;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExpiredNotiReport(string MonthList,string JnsPkjList)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            if (MonthList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChoosePassportExpired;
                return View();
            }

            //var result = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "exprdmonthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID);/*, "fldOptConfValue", "fldOptConfDesc", MonthList);*/
            ViewBag.Report = "class = active";
            ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "exprdmonthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", MonthList);
            ViewBag.SelectionMonth = MonthList;

            List<SelectListItem> JnsPkjList2 = new List<SelectListItem>();
            JnsPkjList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            JnsPkjList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
            ViewBag.JnsPkjList = JnsPkjList2;

            ViewBag.getflag = 2;
            return View();
        }

        public ActionResult ExpiredPermit(string MonthList, string JnsPkjList, string print)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            DateTime todaydate = DateTime.Today;
            DateTime startdate = DateTime.Today.AddMonths(int.Parse(MonthList));
            if (MonthList == "-1")
            {
                if (JnsPkjList == "0")
                {
                    var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_T2prmt.Value.Month <= todaydate.Month && x.fld_T2prmt.Value.Year <= todaydate.Year);
                    ViewBag.DataCount = result.Count();
                    ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                    ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                    ViewBag.Print = print;
                    return View(result);
                }
                else
                {
                    var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_T2prmt.Value.Month <= todaydate.Month && x.fld_T2prmt.Value.Year <= todaydate.Year && x.fld_Jenispekerja == JnsPkjList);
                    ViewBag.DataCount = result.Count();
                    ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                    ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                    ViewBag.Print = print;
                    return View(result);
                }
                
            }
            else
            {
                if (JnsPkjList == "0")
                {
                    var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_T2prmt.Value.Month == startdate.Month && x.fld_T2prmt.Value.Year == startdate.Year);
                    ViewBag.DataCount = result.Count();
                    ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                    ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                    ViewBag.Print = print;
                    return View(result);
                }
                else
                {
                    var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_T2prmt.Value.Month == startdate.Month && x.fld_T2prmt.Value.Year == startdate.Year && x.fld_Jenispekerja == JnsPkjList);
                    ViewBag.DataCount = result.Count();
                    ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                    ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                    ViewBag.Print = print;
                    return View(result);
                }
                    
            }
        }

        public ActionResult ExpiredPassport(string MonthList, string JnsPkjList, string print)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            DateTime todaydate = DateTime.Today;
            DateTime startdate = DateTime.Today.AddMonths(int.Parse(MonthList));
            if (MonthList == "-1")
            {
                if (JnsPkjList == "0")
                {
                    var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_T2pspt.Value.Month <= todaydate.Month && x.fld_T2pspt.Value.Year <= todaydate.Year);
                    ViewBag.DataCount = result.Count();
                    ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                    ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                    //ViewBag.MonthSelection = MonthList;
                    ViewBag.Print = print;
                    return View(result);
                }
                else
                {
                    var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_T2pspt.Value.Month <= todaydate.Month && x.fld_T2pspt.Value.Year <= todaydate.Year && x.fld_Jenispekerja == JnsPkjList);
                    ViewBag.DataCount = result.Count();
                    ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                    ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                    //ViewBag.MonthSelection = MonthList;
                    ViewBag.Print = print;
                    return View(result);
                }
                
            }
            else
            {
                if (JnsPkjList == "0")
                {
                    var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_T2pspt.Value.Month == startdate.Month && x.fld_T2pspt.Value.Year == startdate.Year);
                    ViewBag.DataCount = result.Count();
                    ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                    ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                    //ViewBag.MonthSelection = MonthList;
                    ViewBag.Print = print;
                    return View(result);
                }
                else
                {
                    var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_T2pspt.Value.Month == startdate.Month && x.fld_T2pspt.Value.Year == startdate.Year && x.fld_Jenispekerja == JnsPkjList);
                    ViewBag.DataCount = result.Count();
                    ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                    ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                    //ViewBag.MonthSelection = MonthList;
                    ViewBag.Print = print;
                    return View(result);
                }
                
            }
        }

        public ActionResult HasilReport()
        {
            ViewBag.Report = "class = active";
            int month = timezone.gettimezone().AddMonths(-1).Month;
            int year = timezone.gettimezone().Year;
            int rangeyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;

            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

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
            var tblkerja = dbr.tbl_Kerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);
            string[] nopkj = tblkerja.Select(s => s.fld_Nopkj).ToArray();
            //string[] group = tblkerja.Select(s => s.fld_Kum).Distinct().ToArray();
            //string[] pkt = tblkerja.Select(s => s.fld_KodPkt).Distinct().ToArray();

            List<SelectListItem> WorkerList = new List<SelectListItem>();
            //comment by fitri 26-01-2021
            //WorkerList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && nopkj.Contains(x.fld_Nopkj) && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nama).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + " - " + s.fld_Nama }).Distinct(), "Value", "Text").ToList();
            //add by fitri 26-01-2021
            WorkerList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && nopkj.Contains(x.fld_Nopkj) && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nama).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + " - " + s.fld_Nama }), "Value", "Text").ToList();
            WorkerList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            //List<SelectListItem> GroupList = new List<SelectListItem>();
            //GroupList = new SelectList(dbr.tbl_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted==false && group.Contains(x.fld_KodKumpulan)).Select(s => new SelectListItem { Value = s.fld_KodKumpulan, Text = s.fld_KodKumpulan + " - " + s.fld_Keterangan }).Distinct(), "Value", "Text").ToList();
            //GroupList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));

            //List<SelectListItem> PktList = new List<SelectListItem>();
            //WorkerList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && nopkj.Contains(x.fld_Nopkj)).OrderBy(o => o.fld_Nama).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + " - " + s.fld_Nama }).Distinct(), "Value", "Text").ToList();
            //WorkerList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.MonthList = MonthList;
            ViewBag.YearList = yearlist;
            ViewBag.SelectionList = WorkerList;
            ViewBag.getflag = 1;
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult HasilReport(int RadioGroup, int MonthList, int YearList, string SelectionList)
        //{
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int year = timezone.gettimezone().Year;
        //    int rangeyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;

        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = GetIdentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

        //    var yearlist = new List<SelectListItem>();
        //    for (var i = rangeyear; i <= year; i++)
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

        //    var MonthList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);

        //    string[] nopkj = dbr.tbl_Kerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_Nopkj).ToArray();

        //    List<SelectListItem> WorkerList2 = new List<SelectListItem>();
        //    WorkerList2 = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && nopkj.Contains(x.fld_Nopkj)).OrderBy(o => o.fld_Nama).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + " - " + s.fld_Nama }).Distinct(), "Value", "Text").ToList();
        //    WorkerList2.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));

        //    ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //    ViewBag.MonthList = MonthList2;
        //    ViewBag.YearList = yearlist;
        //    ViewBag.SelectionList = WorkerList2;
        //    ViewBag.MonthSelection = MonthList;
        //    ViewBag.YearSelection = YearList;
        //    ViewBag.getflag = 2;

        //    if (RadioGroup == 0)
        //    {
        //        //Individu
        //        if(SelectionList=="0")
        //        {
        //            var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_KodAktvt == "1001").OrderBy(o => o.fld_Tarikh);
        //            return View(result);
        //        }
        //        else
        //        {
        //            var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_KodAktvt == "1001" && x.fld_Nopkj==SelectionList).OrderBy(o => o.fld_Tarikh);
        //            return View(result);
        //        }

        //    }
        //    else if (RadioGroup == 0)
        //    {
        //        //Kumpulan
        //        if(SelectionList=="0")
        //        {
        //            var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_KodAktvt == "1001").OrderBy(o => o.fld_Tarikh);
        //            return View(result);
        //        }
        //        else
        //        {
        //            var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_KodAktvt == "1001" && x.fld_Kum == SelectionList).OrderBy(o => o.fld_Tarikh);
        //            return View(result);
        //        }

        //    }
        //    else
        //    {
        //        //Peringkat/subperingkat
        //        if(SelectionList=="0")
        //        {
        //            var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_KodAktvt == "1001").OrderBy(o => o.fld_Tarikh);
        //            return View(result);
        //        }
        //        else
        //        {
        //            var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_KodAktvt == "1001" && x.fld_KodPkt==SelectionList).OrderBy(o => o.fld_Tarikh);
        //            return View(result);
        //        }
        //    }
        //}

        //[HttpPost]
        public ActionResult HasilReportDetail(int? RadioGroup, int? MonthList, int? YearList, string SelectionList, string print)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.MonthSelection = MonthList;
            ViewBag.YearSelection = YearList;
            ViewBag.Print = print;
            ViewBag.RadioSelection = RadioGroup; //Add by fitri 2-2-2021

            //Comment by fitri 5-11-2020
            //var GetAtvtForYield = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Kategori == "1").Select(s => s.fld_KodAktvt).ToList();
            //add by fitri 5-11-2020
            //Comment by fitri 2-2-2021
            //var GetAtvtForYield = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_KodAktvt).ToList();
            //Add by fitri 2-2-2021
            var GetAtvtForYield = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Unit == "TAN").Select(s => s.fld_KodAktvt).ToList();

            if (MonthList == null && YearList == null && SelectionList == null)
            {

                ViewBag.Message = GlobalResEstate.msgChooseWork;
                return View();

            }

            if (RadioGroup == 0)
            {
                //Individu
                if (SelectionList == "0")
                {
                    var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && 
                                                          x.fld_Tarikh.Value.Year == YearList &&
                                                          GetAtvtForYield.Contains(x.fld_KodAktvt) &&
                                                          x.fld_NegaraID == NegaraID &&
                                                          x.fld_SyarikatID == SyarikatID &&
                                                          x.fld_WilayahID == WilayahID &&
                                                          x.fld_LadangID == LadangID)
                                              .OrderBy(o => o.fld_Tarikh);
                    ViewBag.DataCount = result.Count();
                    if (result.Count() == 0)
                    {
                        ViewBag.Message = "Tiada Rekod";
                    }
                    return View(result);
                }
                else
                {
                    var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && 
                                                          x.fld_Tarikh.Value.Year == YearList &&
                                                          GetAtvtForYield.Contains(x.fld_KodAktvt) && 
                                                          x.fld_Nopkj == SelectionList &&
                                                          x.fld_NegaraID == NegaraID &&
                                                          x.fld_SyarikatID == SyarikatID &&
                                                          x.fld_WilayahID == WilayahID &&
                                                          x.fld_LadangID == LadangID)
                                              .OrderBy(o => o.fld_Tarikh);
                    ViewBag.DataCount = result.Count();
                    if (result.Count() == 0)
                    {
                        ViewBag.Message = "Tiada Rekod";
                    }
                    return View(result);
                }

            }
            else if (RadioGroup == 1)
            {
                //Kumpulan
                if (SelectionList == "0")
                {
                    var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && 
                                                          x.fld_Tarikh.Value.Year == YearList &&
                                                          GetAtvtForYield.Contains(x.fld_KodAktvt) &&
                                                          x.fld_NegaraID == NegaraID &&
                                                          x.fld_SyarikatID == SyarikatID &&
                                                          x.fld_WilayahID == WilayahID &&
                                                          x.fld_LadangID == LadangID)
                                              .OrderBy(o => o.fld_Tarikh);
                    ViewBag.DataCount = result.Count();
                    if (result.Count() == 0)
                    {
                        ViewBag.Message = "Tiada Rekod";
                    }
                    return View(result);
                }
                else
                {
                    var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && 
                                                          x.fld_Tarikh.Value.Year == YearList &&
                                                          GetAtvtForYield.Contains(x.fld_KodAktvt) && 
                                                          x.fld_Kum == SelectionList &&
                                                          x.fld_NegaraID == NegaraID &&
                                                          x.fld_SyarikatID == SyarikatID &&
                                                          x.fld_WilayahID == WilayahID &&
                                                          x.fld_LadangID == LadangID)
                                              .OrderBy(o => o.fld_Tarikh);
                    ViewBag.DataCount = result.Count();
                    if (result.Count() == 0)
                    {
                        ViewBag.Message = "Tiada Rekod";
                    }
                    return View(result);
                }

            }
            else
            {
                //Peringkat/subperingkat
                if (SelectionList == "0")
                {
                    var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && 
                                                          x.fld_Tarikh.Value.Year == YearList &&
                                                          GetAtvtForYield.Contains(x.fld_KodAktvt) &&
                                                          x.fld_NegaraID == NegaraID &&
                                                          x.fld_SyarikatID == SyarikatID &&
                                                          x.fld_WilayahID == WilayahID &&
                                                          x.fld_LadangID == LadangID)
                                              .OrderBy(o => o.fld_Tarikh);
                    ViewBag.DataCount = result.Count();
                    if (result.Count() == 0)
                    {
                        ViewBag.Message = "Tiada Rekod";
                    }
                    return View(result);
                }
                else
                {
                    var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && 
                                                          x.fld_Tarikh.Value.Year == YearList &&
                                                          GetAtvtForYield.Contains(x.fld_KodAktvt) && 
                                                          x.fld_KodPkt == SelectionList &&
                                                          x.fld_NegaraID == NegaraID &&
                                                          x.fld_SyarikatID == SyarikatID &&
                                                          x.fld_WilayahID == WilayahID &&
                                                          x.fld_LadangID == LadangID)
                                              .OrderBy(o => o.fld_Tarikh);
                    ViewBag.DataCount = result.Count();
                    if (result.Count() == 0)
                    {
                        ViewBag.Message = "Tiada Rekod";
                    }
                    return View(result);
                }
            }
        }

        public ActionResult HasilSearch()
        {
            ViewBag.Report = "class = active";
            return View();
        }

        public ActionResult BankAccReport()
        {
            ViewBag.Report = "class = active";
            //int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            //int? getuserid = GetIdentity.ID(User.Identity.Name);
            ////string host, catalog, user, pass = "";
            //GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            //Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            //MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            //var result = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahID && x.fld_ID == LadangID);

            //ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            //ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            //ViewBag.Print = print;
            return View();
        }

        public ActionResult _BankAccReport(string print)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            //string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            //Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            //MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var result = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahID && x.fld_ID == LadangID);

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.Print = print;
            return View(result);
        }

        public ActionResult AIPSReport()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

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

            List<SelectListItem> GroupList = new List<SelectListItem>();
            GroupList = new SelectList(dbr.tbl_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).OrderBy(o => o.fld_KodKumpulan).Select(s => new SelectListItem { Value = s.fld_KumpulanID.ToString(), Text = s.fld_KodKumpulan }).Distinct(), "Value", "Text").ToList();
            GroupList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            List<SelectListItem> WorkerList = new List<SelectListItem>();
            WorkerList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf=="1").OrderBy(o => o.fld_Nama).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + " - " + s.fld_Nama }).Distinct(), "Value", "Text").ToList();
            WorkerList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            List<SelectListItem> JnsPkjList = new List<SelectListItem>();
            JnsPkjList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            JnsPkjList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));


            ViewBag.YearList = yearlist;
            ViewBag.GroupList = GroupList;
            ViewBag.WorkerList = WorkerList;
            ViewBag.JnsPkjList = JnsPkjList;
            return View();
        }

        public ActionResult AIPSReportDetail(int? YearList, string GroupList, string WorkerList, string JnsPkjList, string print)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.YearSelection = YearList;
            ViewBag.Print = print;

            if (GroupList == null && YearList == null && WorkerList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChooseAips;
                return View();
            }

            if (GroupList=="0")
            {
                if (JnsPkjList == "0")
                {
                    var result = dbr.vw_RptAIPS.Where(x => x.fld_Year == YearList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);
                    if (result.Count() == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }
                    ViewBag.DataCount = result.Count();
                    return PartialView(result);
                }
                else
                {
                    var result = dbr.vw_RptAIPS.Where(x => x.fld_Year == YearList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Jenispekerja == JnsPkjList);
                    if (result.Count() == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }
                    ViewBag.DataCount = result.Count();
                    return PartialView(result);
                }
                
            }
            else
            {
                int groupID = int.Parse(GroupList);
                if (WorkerList == "0")
                {
                    var result = dbr.vw_RptAIPS.Where(x =>x.fld_KumpulanID==groupID && x.fld_Year == YearList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);
                    if (result.Count() == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }
                    ViewBag.DataCount = result.Count();
                    return PartialView(result);
                }
                else
                {
                    var result = dbr.vw_RptAIPS.Where(x =>x.fld_KumpulanID==groupID && x.fld_Nopkj == WorkerList && x.fld_Year == YearList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Jenispekerja == JnsPkjList);
                    if (result.Count() == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }
                    ViewBag.DataCount = result.Count();
                    return PartialView(result);
                }
            }
           
        }        

        public JsonResult GetList(int RadioGroup, string StatusList)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            List<SelectListItem> SelectionList = new List<SelectListItem>();
            string SelectionLabel = "";

            if (RadioGroup == 0)
            {
                if (String.IsNullOrEmpty(StatusList))
                {
                    //Individu Semua
                    SelectionLabel = "Pekerja";

                    SelectionList = new SelectList(
                        dbr.tbl_Pkjmast
                            .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                        x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_DivisionID == DivisionID)
                            .OrderBy(o => o.fld_Nopkj)
                            .Select(
                                s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }),
                        "Value", "Text").ToList();
                    //SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
                }

                else
                {
                    //Individu Semua
                    SelectionLabel = "Pekerja";
                    if (StatusList == "0")
                    {
                        SelectionList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf != null && x.fld_DivisionID == DivisionID).OrderBy(o => o.fld_Nama).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }), "Value", "Text").ToList();
                        //SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
                    }
                    else
                    {
                        SelectionList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == StatusList && x.fld_DivisionID == DivisionID).OrderBy(o => o.fld_Nama).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }), "Value", "Text").ToList();
                        //SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
                    }
                }

            }
            else
            {
                //Group
                SelectionLabel = "Kumpulan";
                SelectionList = new SelectList(dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false && x.fld_DivisionID == DivisionID).OrderBy(o => o.fld_KodKumpulan).Select(s => new SelectListItem { Value = s.fld_KodKumpulan, Text = s.fld_KodKumpulan + "-" + s.fld_Keterangan }), "Value", "Text").ToList();
                //SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
            }
            return Json(new { SelectionList = SelectionList, SelectionLabel = SelectionLabel });
        }

        public JsonResult GetList2(int RadioGroup)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            List<SelectListItem> SelectionList = new List<SelectListItem>();
            string SelectionLabel = "";

            var tblkerja = dbr.tbl_Kerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);
            string[] nopkj = tblkerja.Select(s => s.fld_Nopkj).ToArray();
            string[] group = tblkerja.Select(s => s.fld_Kum).Distinct().ToArray();
            string[] pkt = tblkerja.Select(s => s.fld_KodPkt).Distinct().ToArray();

            if (RadioGroup == 0)
            {
                //Individu Semua
                SelectionLabel = "Pekerja";
                SelectionList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && nopkj.Contains(x.fld_Nopkj)).OrderBy(o => o.fld_Nama).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + " - " + s.fld_Nama }).Distinct(), "Value", "Text").ToList();
                SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
            }
            else if (RadioGroup == 1)
            {
                //Group
                SelectionLabel = "Kumpulan";
                SelectionList = new SelectList(dbr.tbl_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false && group.Contains(x.fld_KodKumpulan)).Select(s => new SelectListItem { Value = s.fld_KodKumpulan, Text = s.fld_KodKumpulan + " - " + s.fld_Keterangan }).Distinct(), "Value", "Text").ToList();
                SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
            }
            else
            {
                //Pkt
                SelectionLabel = "Peringkat";
                SelectionList = new SelectList(dbr.tbl_Kerja
                                                  .Where(x => x.fld_NegaraID == NegaraID && 
                                                              x.fld_SyarikatID == SyarikatID && 
                                                              x.fld_WilayahID == WilayahID && 
                                                              x.fld_LadangID == LadangID)
                                                  .Select(s => new SelectListItem { Value = s.fld_KodPkt, Text = s.fld_KodPkt })
                                                  .Distinct(), "Value", "Text").ToList();
                SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
            }
            return Json(new { SelectionList = SelectionList, SelectionLabel = SelectionLabel });
        }

        public JsonResult GetWorkerList(string groupid)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            List<SelectListItem> WorkerList = new List<SelectListItem>();
            int idgroup = int.Parse(groupid);
            if (idgroup == 0)
            {
                WorkerList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_KumpulanID!=null).OrderBy(o => o.fld_Nama).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + " - " + s.fld_Nama }).Distinct(), "Value", "Text").ToList();
                //WorkerList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));
            }
            else
            {
                WorkerList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_KumpulanID== idgroup).OrderBy(o => o.fld_Nama).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + " - " + s.fld_Nama }).Distinct(), "Value", "Text").ToList();
                //WorkerList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));
            }
            return Json(WorkerList);
        }
        [HttpPost]
        public ActionResult ConvertPDF(string myHtml, string filename, string reportname)
        {
            bool success = false;
            string msg = "";
            string status = "";
            MasterModels.tblHtmlReport tblHtmlReport = new MasterModels.tblHtmlReport();

            tblHtmlReport.fldHtlmCode = myHtml;
            tblHtmlReport.fldFileName = filename;
            tblHtmlReport.fldReportName = reportname;

            db.tblHtmlReport.Add(tblHtmlReport);
            db.SaveChanges();

            success = true;
            status = "success";

            return Json(new { success = success, id = tblHtmlReport.fldID, msg = msg, status = status, link = Url.Action("GetPDF", "Report", null, "http") + "/" + tblHtmlReport.fldID });
        }

        //public ActionResult GetPDF(int id)
        //{
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int? getuserid = GetIdentity.ID(User.Identity.Name);
        //    string width = "", height = "";
        //    string imagepath = Server.MapPath("~/Asset/Images/");

        //    var gethtml = db.tblHtmlReport.Find(id);
        //    var getsize = db.tblReportLists.Where(x => x.fldReportListAction == gethtml.fldReportName.ToString()).FirstOrDefault();
        //    if (getsize != null)
        //    {
        //        width = getsize.fldWidthReport.ToString();
        //        height = getsize.fldHeightReport.ToString();
        //    }
        //    else
        //    {
        //        var getsizesubreport = db.tblSubReportLists.Where(x => x.fldSubReportListAction == gethtml.fldReportName.ToString()).FirstOrDefault();
        //        width = getsizesubreport.fldSubWidthReport.ToString();
        //        height = getsizesubreport.fldSubHeightReport.ToString();
        //    }
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    var logosyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_LogoName).FirstOrDefault();

        //    //Export HTML String as PDF.
        //    //Image logo = Image.GetInstance(imagepath + logosyarikat);
        //    //Image alignment
        //    //logo.ScaleToFit(50f, 50f);
        //    //logo.Alignment = Image.TEXTWRAP | Image.ALIGN_CENTER;
        //    //StringReader sr = new StringReader(gethtml.fldHtlmCode);
        //    Document pdfDoc = new Document(new Rectangle(int.Parse("1190"), int.Parse("1684")), 50f, 50f, 50f, 50f);
        //    //HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
        //    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
        //    pdfDoc.Open();
        //    //pdfDoc.Add(logo);
        //    using (TextReader sr = new StringReader(gethtml.fldHtlmCode))
        //    {
        //        using (var htmlWorker = new HTMLWorkerExtended(pdfDoc, imagepath + logosyarikat))
        //        {
        //            htmlWorker.Open();
        //            htmlWorker.Parse(sr);
        //        }
        //    }
        //    pdfDoc.Close();
        //    Response.ContentType = "application/pdf";
        //    Response.AddHeader("content-disposition", "attachment;filename=" + gethtml.fldFileName + ".pdf");
        //    Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //    Response.Write(pdfDoc);
        //    Response.End();

        //    db.Entry(gethtml).State = EntityState.Deleted;
        //    db.SaveChanges();
        //    return View();
        //}

        //public ActionResult testPhoto(HttpPostedFileBase file)
        //{
        //    if (file != null)
        //    {
        //        string pic = System.IO.Path.GetFileName(file.FileName);
        //        string path = System.IO.Path.Combine(
        //                               Server.MapPath("~/Asset/Images"), pic);
        //        // file is uploaded
        //        file.SaveAs(path);

        //        // save the image path path to the database or you can send image 
        //        // directly to database
        //        // in-case if you want to store byte[] ie. for DB
        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            file.InputStream.CopyTo(ms);
        //            byte[] array = ms.GetBuffer();
        //            byte[] array2 = new byte[file.ContentLength];

        //            GetIdentity getidentity = new GetIdentity();
        //            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //            int? getuserid = getidentity.ID(User.Identity.Name);
        //            string host, catalog, user, pass = "";
        //            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
        //            var filesaving = dbr.tbl_Photo.Where(x => x.fld_Photo == array2).FirstOrDefault();
        //            if(filesaving==null)
        //            {
        //                filesaving.fld_Photo = array2;
        //                filesaving.fld_Nopkj = "1112345";
        //                dbr.tbl_Photo.Add(filesaving);
        //                dbr.SaveChanges();
        //            }
        //        }



        //    }
        //    // after successfully uploading redirect the user
        //    //return RedirectToAction("actionname", "controller name");
        //    return View();

        //}

        public ActionResult IncentiveReport()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int drpyear = 0;
            int drprangeyear = 0;
            int month = timezone.gettimezone().Month;

            List<SelectListItem> SelectionList = new List<SelectListItem>();
            SelectionList = new SelectList(
                dbr.tbl_Pkjmast
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1")
                    .OrderBy(o => o.fld_Nopkj)
                    .Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }),
                "Value", "Text").ToList();
            SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.SelectionList = SelectionList;

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
                "fldOptConfValue", "fldOptConfDesc");

            ViewBag.YearList = yearlist;

            List<SelectListItem> JnsPkjList = new List<SelectListItem>();
            JnsPkjList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            JnsPkjList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
            ViewBag.JnsPkjList = JnsPkjList;

            return View();
        }

        public ViewResult _WorkerIncentiveRptSearch(int? RadioGroup, int? MonthList, int? YearList,
            string SelectionList, string StatusList, string WorkCategoryList, string print, string JnsPkjList)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            List<vw_MaklumatInsentifPekerja> MaklumatInsentifPekerja = new List<vw_MaklumatInsentifPekerja>();

            ViewBag.MonthList = MonthList;
            ViewBag.YearList = YearList;
            ViewBag.WorkerList = SelectionList;
            ViewBag.Print = print;
            ViewBag.JnsPkjList = JnsPkjList;

            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();


            if (MonthList == null && YearList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChooseWork;
                return View(MaklumatInsentifPekerja);
            }

            else
            {
                if (RadioGroup == 0)

                {
                    if (SelectionList == "0")
                    {
                       if (JnsPkjList == "0")
                        {
                            IOrderedQueryable<ViewingModels.tbl_Pkjmast> workerData;

                            if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                            {
                                workerData = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList &&
                                                x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            else
                            {
                                workerData = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            foreach (var i in workerData)
                            {
                                var pendapatan = dbview.vw_MaklumatInsentif
                                    .Where(a => a.fld_Nopkj == (i.fld_Nopkj) && a.fld_Month == MonthList &&
                                                a.fld_Year == YearList && a.fld_KodInsentif.Contains("P") &&
                                                a.fld_NegaraID == NegaraID &&
                                                a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                                a.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_KodInsentif)
                                    .ToList();

                                var potongan = dbview.vw_MaklumatInsentif
                                    .Where(a => a.fld_Nopkj == (i.fld_Nopkj) && a.fld_Month == MonthList &&
                                                a.fld_Year == YearList && a.fld_KodInsentif.Contains("T") &&
                                                a.fld_NegaraID == NegaraID &&
                                                a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                                a.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_KodInsentif)
                                    .ToList();

                                MaklumatInsentifPekerja.Add(
                                    new vw_MaklumatInsentifPekerja
                                    {
                                        Pkjmast = i,
                                        Pendapatan = pendapatan,
                                        Potongan = potongan
                                    });
                            }

                            if (MaklumatInsentifPekerja.Count == 0)
                            {
                                ViewBag.Message = GlobalResEstate.msgNoRecord;
                            }

                            return View(MaklumatInsentifPekerja);
                        }
                        else
                        {
                            IOrderedQueryable<ViewingModels.tbl_Pkjmast> workerData;

                            if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                            {
                                workerData = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList && x.fld_Jenispekerja == JnsPkjList &&
                                                x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            else
                            {
                                workerData = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_Jenispekerja == JnsPkjList &&
                                                x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            foreach (var i in workerData)
                            {
                                var pendapatan = dbview.vw_MaklumatInsentif
                                    .Where(a => a.fld_Nopkj == (i.fld_Nopkj) && a.fld_Month == MonthList &&
                                                a.fld_Year == YearList && a.fld_KodInsentif.Contains("P") &&
                                                a.fld_NegaraID == NegaraID &&
                                                a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                                a.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_KodInsentif)
                                    .ToList();

                                var potongan = dbview.vw_MaklumatInsentif
                                    .Where(a => a.fld_Nopkj == (i.fld_Nopkj) && a.fld_Month == MonthList &&
                                                a.fld_Year == YearList && a.fld_KodInsentif.Contains("T") &&
                                                a.fld_NegaraID == NegaraID &&
                                                a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                                a.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_KodInsentif)
                                    .ToList();

                                MaklumatInsentifPekerja.Add(
                                    new vw_MaklumatInsentifPekerja
                                    {
                                        Pkjmast = i,
                                        Pendapatan = pendapatan,
                                        Potongan = potongan
                                    });
                            }

                            if (MaklumatInsentifPekerja.Count == 0)
                            {
                                ViewBag.Message = GlobalResEstate.msgNoRecord;
                            }

                            return View(MaklumatInsentifPekerja);
                        }

                    }

                    else
                    {
                        if (JnsPkjList == "0")
                        {
                            var workerDataSingle = new ViewingModels.tbl_Pkjmast();

                            if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                            {
                                workerDataSingle = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_Nopkj == SelectionList && x.fld_Kdaktf == StatusList &&
                                                x.fld_Ktgpkj == WorkCategoryList && x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama)
                                    .SingleOrDefault();
                            }

                            else
                            {
                                workerDataSingle = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_Nopkj == SelectionList && x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama)
                                    .SingleOrDefault();
                            }

                            if (workerDataSingle != null)
                            {
                                var pendapatan = dbview.vw_MaklumatInsentif
                                    .Where(a => a.fld_Nopkj == SelectionList && a.fld_Month == MonthList &&
                                                a.fld_Year == YearList && a.fld_KodInsentif.Contains("P") &&
                                                a.fld_NegaraID == NegaraID &&
                                                a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                                a.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_KodInsentif)
                                    .ToList();

                                var potongan = dbview.vw_MaklumatInsentif
                                    .Where(a => a.fld_Nopkj == SelectionList && a.fld_Month == MonthList &&
                                                a.fld_Year == YearList && a.fld_KodInsentif.Contains("T") &&
                                                a.fld_NegaraID == NegaraID &&
                                                a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                                a.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_KodInsentif)
                                    .ToList();

                                MaklumatInsentifPekerja.Add(
                                    new vw_MaklumatInsentifPekerja
                                    {
                                        Pkjmast = workerDataSingle,
                                        Pendapatan = pendapatan,
                                        Potongan = potongan
                                    });
                            }
                        }
                        else
                        {
                            var workerDataSingle = new ViewingModels.tbl_Pkjmast();

                            if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                            {
                                workerDataSingle = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_Nopkj == SelectionList && x.fld_Kdaktf == StatusList && x.fld_Jenispekerja == JnsPkjList &&
                                                x.fld_Ktgpkj == WorkCategoryList && x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama)
                                    .SingleOrDefault();
                            }

                            else
                            {
                                workerDataSingle = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_Nopkj == SelectionList && x.fld_NegaraID == NegaraID && x.fld_Jenispekerja == JnsPkjList &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama)
                                    .SingleOrDefault();
                            }

                            if (workerDataSingle != null)
                            {
                                var pendapatan = dbview.vw_MaklumatInsentif
                                    .Where(a => a.fld_Nopkj == SelectionList && a.fld_Month == MonthList &&
                                                a.fld_Year == YearList && a.fld_KodInsentif.Contains("P") &&
                                                a.fld_NegaraID == NegaraID &&
                                                a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                                a.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_KodInsentif)
                                    .ToList();

                                var potongan = dbview.vw_MaklumatInsentif
                                    .Where(a => a.fld_Nopkj == SelectionList && a.fld_Month == MonthList &&
                                                a.fld_Year == YearList && a.fld_KodInsentif.Contains("T") &&
                                                a.fld_NegaraID == NegaraID &&
                                                a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                                a.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_KodInsentif)
                                    .ToList();

                                MaklumatInsentifPekerja.Add(
                                    new vw_MaklumatInsentifPekerja
                                    {
                                        Pkjmast = workerDataSingle,
                                        Pendapatan = pendapatan,
                                        Potongan = potongan 
                                    });
                            }
                        }
                    }

                    if (MaklumatInsentifPekerja.Count == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }

                    return View(MaklumatInsentifPekerja);
                }

                else
                {
                    if (SelectionList == "0")
                    {
                        if (JnsPkjList == "0")
                        {
                            IOrderedQueryable<ViewingModels.tbl_Pkjmast> workerData;

                            if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                            {
                                workerData = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList &&
                                                x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            else
                            {
                                workerData = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            foreach (var i in workerData)
                            {
                                var pendapatan = dbview.vw_MaklumatInsentif
                                    .Where(a => a.fld_Nopkj == (i.fld_Nopkj) && a.fld_Month == MonthList &&
                                                a.fld_Year == YearList && a.fld_KodInsentif.Contains("P") &&
                                                a.fld_NegaraID == NegaraID &&
                                                a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                                a.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_KodInsentif)
                                    .ToList();

                                var potongan = dbview.vw_MaklumatInsentif
                                    .Where(a => a.fld_Nopkj == (i.fld_Nopkj) && a.fld_Month == MonthList &&
                                                a.fld_Year == YearList && a.fld_KodInsentif.Contains("T") &&
                                                a.fld_NegaraID == NegaraID &&
                                                a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                                a.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_KodInsentif)
                                    .ToList();

                                MaklumatInsentifPekerja.Add(
                                    new vw_MaklumatInsentifPekerja
                                    {
                                        Pkjmast = i,
                                        Pendapatan = pendapatan,
                                        Potongan = potongan
                                    });
                            }

                            if (MaklumatInsentifPekerja.Count == 0)
                            {
                                ViewBag.Message = GlobalResEstate.msgNoRecord;
                            }

                            return View(MaklumatInsentifPekerja);
                        }
                        else
                        {
                            IOrderedQueryable<ViewingModels.tbl_Pkjmast> workerData;

                            if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                            {
                                workerData = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList && x.fld_Jenispekerja == JnsPkjList &&
                                                x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            else
                            {
                                workerData = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_Jenispekerja == JnsPkjList &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            foreach (var i in workerData)
                            {
                                var pendapatan = dbview.vw_MaklumatInsentif
                                    .Where(a => a.fld_Nopkj == (i.fld_Nopkj) && a.fld_Month == MonthList &&
                                                a.fld_Year == YearList && a.fld_KodInsentif.Contains("P") &&
                                                a.fld_NegaraID == NegaraID &&
                                                a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                                a.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_KodInsentif)
                                    .ToList();

                                var potongan = dbview.vw_MaklumatInsentif
                                    .Where(a => a.fld_Nopkj == (i.fld_Nopkj) && a.fld_Month == MonthList &&
                                                a.fld_Year == YearList && a.fld_KodInsentif.Contains("T") &&
                                                a.fld_NegaraID == NegaraID &&
                                                a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                                a.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_KodInsentif)
                                    .ToList();

                                MaklumatInsentifPekerja.Add(
                                    new vw_MaklumatInsentifPekerja
                                    {
                                        Pkjmast = i,
                                        Pendapatan = pendapatan,
                                        Potongan = potongan
                                    });
                            }

                            if (MaklumatInsentifPekerja.Count == 0)
                            {
                                ViewBag.Message = GlobalResEstate.msgNoRecord;
                            }

                            return View(MaklumatInsentifPekerja);
                        }
                    }

                    else
                    {
                        if (JnsPkjList == "0")
                        {
                            var groupData = dbview.tbl_KumpulanKerja
                            .Where(x => x.fld_KodKumpulan == SelectionList && x.fld_NegaraID == NegaraID &&
                                        x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                        x.fld_LadangID == LadangID)
                            .Select(s => s.fld_KumpulanID)
                            .SingleOrDefault();

                            IOrderedQueryable<ViewingModels.tbl_Pkjmast> workerData;

                            if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                            {
                                workerData = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_KumpulanID == groupData && x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList &&
                                                x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            else
                            {
                                workerData = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_KumpulanID == groupData && x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            foreach (var i in workerData)
                            {
                                var pendapatan = dbview.vw_MaklumatInsentif
                                    .Where(a => a.fld_Nopkj == (i.fld_Nopkj) && a.fld_Month == MonthList &&
                                                a.fld_Year == YearList && a.fld_KodInsentif.Contains("P") &&
                                                a.fld_NegaraID == NegaraID &&
                                                a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                                a.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_KodInsentif)
                                    .ToList();

                                var potongan = dbview.vw_MaklumatInsentif
                                    .Where(a => a.fld_Nopkj == (i.fld_Nopkj) && a.fld_Month == MonthList &&
                                                a.fld_Year == YearList && a.fld_KodInsentif.Contains("T") &&
                                                a.fld_NegaraID == NegaraID &&
                                                a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                                a.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_KodInsentif)
                                    .ToList();

                                MaklumatInsentifPekerja.Add(
                                    new vw_MaklumatInsentifPekerja
                                    {
                                        Pkjmast = i,
                                        Pendapatan = pendapatan,
                                        Potongan = potongan
                                    });
                            }
                        }
                        else
                        {
                            var groupData = dbview.tbl_KumpulanKerja
                            .Where(x => x.fld_KodKumpulan == SelectionList && x.fld_NegaraID == NegaraID &&
                                        x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                        x.fld_LadangID == LadangID)
                            .Select(s => s.fld_KumpulanID)
                            .SingleOrDefault();

                            IOrderedQueryable<ViewingModels.tbl_Pkjmast> workerData;

                            if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                            {
                                workerData = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_KumpulanID == groupData && x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList && x.fld_Jenispekerja == JnsPkjList &&
                                                x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            else
                            {
                                workerData = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_KumpulanID == groupData && x.fld_NegaraID == NegaraID && x.fld_Jenispekerja == JnsPkjList &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            foreach (var i in workerData)
                            {
                                var pendapatan = dbview.vw_MaklumatInsentif
                                    .Where(a => a.fld_Nopkj == (i.fld_Nopkj) && a.fld_Month == MonthList &&
                                                a.fld_Year == YearList && a.fld_KodInsentif.Contains("P") &&
                                                a.fld_NegaraID == NegaraID &&
                                                a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                                a.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_KodInsentif)
                                    .ToList();

                                var potongan = dbview.vw_MaklumatInsentif
                                    .Where(a => a.fld_Nopkj == (i.fld_Nopkj) && a.fld_Month == MonthList &&
                                                a.fld_Year == YearList && a.fld_KodInsentif.Contains("T") &&
                                                a.fld_NegaraID == NegaraID &&
                                                a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                                a.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_KodInsentif)
                                    .ToList();

                                MaklumatInsentifPekerja.Add(
                                    new vw_MaklumatInsentifPekerja
                                    {
                                        Pkjmast = i,
                                        Pendapatan = pendapatan,
                                        Potongan = potongan
                                    });
                            }
                        }
                    }

                    if (MaklumatInsentifPekerja.Count == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }

                    return View(MaklumatInsentifPekerja);
                }
            }
        }

        public ActionResult _WorkerIncentiveRptAdvanceSearch()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var statusList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            ViewBag.StatusList = statusList;

            var workCategoryList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "designation" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            ViewBag.WorkCategoryList = workCategoryList;

           
            return View();
        }

        public ActionResult LeaveReport()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int drpyear = 0;
            int drprangeyear = 0;
            int month = timezone.gettimezone().Month;

            List<SelectListItem> SelectionList = new List<SelectListItem>();
            SelectionList = new SelectList(
                dbr.tbl_Pkjmast
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1")
                    .OrderBy(o => o.fld_Nopkj)
                    .Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }),
                "Value", "Text").ToList();
            SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.SelectionList = SelectionList;

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

            var statusList = new List<SelectListItem>();
            statusList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            ViewBag.StatusList = statusList;

            List<SelectListItem> JnsPkjList = new List<SelectListItem>();
            JnsPkjList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            JnsPkjList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
            ViewBag.JnsPkjList = JnsPkjList;

            return View();
        }

        public ViewResult _WorkerLeaveRptSearch(int? RadioGroup, int? YearList,
            string SelectionList, string StatusList, string WorkCategoryList, string JnsPkjList, string print)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            List<vw_MaklumatCutiPekerja> MaklumatCutiPekerja = new List<vw_MaklumatCutiPekerja>();

            ViewBag.YearList = YearList;
            ViewBag.WorkerList = SelectionList;
            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();
            ViewBag.Print = print;

            if (YearList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChooseWork;
                return View(MaklumatCutiPekerja);
            }

            else
            {
                if (RadioGroup == 0)
                {
                    if (SelectionList == "0")
                    {
                        if (JnsPkjList == "0")
                        {
                            IOrderedQueryable<ViewingModels.tbl_Pkjmast> workerData;

                            if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                            {
                                workerData = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList &&
                                                x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            else
                            {
                                workerData = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            foreach (var i in workerData)
                            {
                                List<Int32> CutiTahunByBulanList = new List<Int32>();
                                List<Int32> CutiAmByBulanList = new List<Int32>();
                                List<Int32> CutiSakitByBulanList = new List<Int32>();
                                List<Int32> CutiMingguanList = new List<Int32>();
                                List<Int32> PontengList = new List<Int32>();

                                var leaveAllocation = dbview.tbl_CutiPeruntukan
                                    .Where(x => x.fld_NoPkj == i.fld_Nopkj && x.fld_Tahun == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID );

                                var cutiTahunan = leaveAllocation
                                    .Where(x => x.fld_KodCuti == "C02")
                                    .Select(s => s.fld_JumlahCuti)
                                    .SingleOrDefault();

                                if (!cutiTahunan.HasValue)
                                {
                                    cutiTahunan = 0;
                                }

                                var cutiAm = leaveAllocation
                                    .Where(x => x.fld_KodCuti == "C01")
                                    .Select(s => s.fld_JumlahCuti)
                                    .SingleOrDefault();

                                if (!cutiAm.HasValue)
                                {
                                    cutiAm = 0;
                                }

                                var cutiSakit = leaveAllocation
                                    .Where(x => x.fld_KodCuti == "C03")
                                    .Select(s => s.fld_JumlahCuti)
                                    .SingleOrDefault();

                                if (!cutiSakit.HasValue)
                                {
                                    cutiSakit = 0;
                                }

                                for (var month = 1; month <= 12; month++)
                                {
                                    var cutiTahunByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "C02" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID
                                                    );


                                    CutiTahunByBulanList.Add(cutiTahunByBulan);

                                    var cutiAmByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "C01" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID
                                                    );

                                    CutiAmByBulanList.Add(cutiAmByBulan);

                                    var cutiSakitByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "C03" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID
                                                    );

                                    CutiSakitByBulanList.Add(cutiSakitByBulan);

                                    var cutiMingguanByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "C07" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    CutiMingguanList.Add(cutiMingguanByBulan);

                                    var pontengByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "P01" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    PontengList.Add(pontengByBulan);

                                }

                                MaklumatCutiPekerja.Add(
                                    new vw_MaklumatCutiPekerja
                                    {
                                        Pkjmast = i,
                                        CutiTahunan = cutiTahunan,
                                        CutiAm = cutiAm,
                                        CutiSakit = cutiSakit,
                                        CutiTahunByBulan = CutiTahunByBulanList,
                                        CutiAmByBulan = CutiAmByBulanList,
                                        CutiSakitByBulan = CutiSakitByBulanList,
                                        CutiMingguanByBulan = CutiMingguanList,
                                        PontengByBulan = PontengList
                                    });
                            }

                            if (MaklumatCutiPekerja.Count == 0)
                            {
                                ViewBag.Message = GlobalResEstate.msgNoRecord;
                            }

                            return View(MaklumatCutiPekerja);
                        }
                        else
                        {
                            IOrderedQueryable<ViewingModels.tbl_Pkjmast> workerData;

                            if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                            {
                                workerData = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList && x.fld_Jenispekerja == JnsPkjList &&
                                                x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            else
                            {
                                workerData = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_Jenispekerja == JnsPkjList &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            foreach (var i in workerData)
                            {
                                List<Int32> CutiTahunByBulanList = new List<Int32>();
                                List<Int32> CutiAmByBulanList = new List<Int32>();
                                List<Int32> CutiSakitByBulanList = new List<Int32>();
                                List<Int32> CutiMingguanList = new List<Int32>();
                                List<Int32> PontengList = new List<Int32>();

                                var leaveAllocation = dbview.tbl_CutiPeruntukan
                                    .Where(x => x.fld_NoPkj == i.fld_Nopkj && x.fld_Tahun == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                var cutiTahunan = leaveAllocation
                                    .Where(x => x.fld_KodCuti == "C02")
                                    .Select(s => s.fld_JumlahCuti)
                                    .SingleOrDefault();

                                if (!cutiTahunan.HasValue)
                                {
                                    cutiTahunan = 0;
                                }

                                var cutiAm = leaveAllocation
                                    .Where(x => x.fld_KodCuti == "C01")
                                    .Select(s => s.fld_JumlahCuti)
                                    .SingleOrDefault();

                                if (!cutiAm.HasValue)
                                {
                                    cutiAm = 0;
                                }

                                var cutiSakit = leaveAllocation
                                    .Where(x => x.fld_KodCuti == "C03")
                                    .Select(s => s.fld_JumlahCuti)
                                    .SingleOrDefault();

                                if (!cutiSakit.HasValue)
                                {
                                    cutiSakit = 0;
                                }

                                for (var month = 1; month <= 12; month++)
                                {
                                    var cutiTahunByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "C02" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID
                                                    );


                                    CutiTahunByBulanList.Add(cutiTahunByBulan);

                                    var cutiAmByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "C01" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID
                                                    );


                                    CutiAmByBulanList.Add(cutiAmByBulan);

                                    var cutiSakitByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "C03" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID
                                                    );


                                    CutiSakitByBulanList.Add(cutiSakitByBulan);

                                    var cutiMingguanByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "C07" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    CutiMingguanList.Add(cutiMingguanByBulan);

                                    var pontengByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "P01" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    PontengList.Add(pontengByBulan);

                                }

                                MaklumatCutiPekerja.Add(
                                    new vw_MaklumatCutiPekerja
                                    {
                                        Pkjmast = i,
                                        CutiTahunan = cutiTahunan,
                                        CutiAm = cutiAm,
                                        CutiSakit = cutiSakit,
                                        CutiTahunByBulan = CutiTahunByBulanList,
                                        CutiAmByBulan = CutiAmByBulanList,
                                        CutiSakitByBulan = CutiSakitByBulanList,
                                        CutiMingguanByBulan = CutiMingguanList,
                                        PontengByBulan = PontengList
                                    });
                            }

                            if (MaklumatCutiPekerja.Count == 0)
                            {
                                ViewBag.Message = GlobalResEstate.msgNoRecord;
                            }

                            return View(MaklumatCutiPekerja);
                        }
                    }

                    else
                    {
                        if (JnsPkjList == "0")
                        {
                            var workerDataSingle = new ViewingModels.tbl_Pkjmast();

                            if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                            {
                                workerDataSingle = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_Nopkj == SelectionList && x.fld_Kdaktf == StatusList &&
                                                x.fld_Ktgpkj == WorkCategoryList && x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama)
                                    .SingleOrDefault();
                            }

                            else
                            {
                                workerDataSingle = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_Nopkj == SelectionList && x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama)
                                    .SingleOrDefault();
                            }

                            if (workerDataSingle != null)
                            {
                                List<Int32> CutiTahunByBulanList = new List<Int32>();
                                List<Int32> CutiAmByBulanList = new List<Int32>();
                                //List<Int32> HadirHariMingguList = new List<Int32>();
                                List<Int32> CutiSakitByBulanList = new List<Int32>();
                                List<Int32> CutiMingguanList = new List<Int32>();
                                List<Int32> PontengList = new List<Int32>();

                                var leaveAllocation = dbview.tbl_CutiPeruntukan
                                    .Where(x => x.fld_NoPkj == SelectionList && x.fld_Tahun == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                var cutiTahunan = leaveAllocation
                                    .Where(x => x.fld_KodCuti == "C02")
                                    .Select(s => s.fld_JumlahCuti)
                                    .SingleOrDefault();

                                if (!cutiTahunan.HasValue)
                                {
                                    cutiTahunan = 0;
                                }

                                var cutiAm = leaveAllocation
                                    .Where(x => x.fld_KodCuti == "C01")
                                    .Select(s => s.fld_JumlahCuti)
                                    .SingleOrDefault();

                                if (!cutiAm.HasValue)
                                {
                                    cutiAm = 0;
                                }

                                var cutiSakit = leaveAllocation
                                    .Where(x => x.fld_KodCuti == "C03")
                                    .Select(s => s.fld_JumlahCuti)
                                    .SingleOrDefault();

                                if (!cutiSakit.HasValue)
                                {
                                    cutiSakit = 0;
                                }

                                for (var month = 1; month <= 12; month++)
                                {
                                    var cutiTahunByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "C02" && x.fld_Nopkj == SelectionList &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID
                                                    );


                                    CutiTahunByBulanList.Add(cutiTahunByBulan);

                                    var cutiAmByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "C01" && x.fld_Nopkj == SelectionList &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID
                                                    );

                                    CutiAmByBulanList.Add(cutiAmByBulan);

                                    var cutiSakitByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "C03" && x.fld_Nopkj == SelectionList &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID
                                                    );

                                    CutiSakitByBulanList.Add(cutiSakitByBulan);

                                    var cutiMingguanByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "C07" && x.fld_Nopkj == SelectionList &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    CutiMingguanList.Add(cutiMingguanByBulan);

                                    var pontengByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "P01" && x.fld_Nopkj == SelectionList &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    PontengList.Add(pontengByBulan);
                                }

                                MaklumatCutiPekerja.Add(
                                    new vw_MaklumatCutiPekerja
                                    {
                                        Pkjmast = workerDataSingle,
                                        CutiTahunan = cutiTahunan,
                                        CutiAm = cutiAm,
                                        CutiSakit = cutiSakit,
                                        CutiTahunByBulan = CutiTahunByBulanList,
                                        CutiAmByBulan = CutiAmByBulanList,
                                        CutiSakitByBulan = CutiSakitByBulanList,
                                        CutiMingguanByBulan = CutiMingguanList,
                                        PontengByBulan = PontengList
                                    });
                            }
                        }
                        else
                        {
                            var workerDataSingle = new ViewingModels.tbl_Pkjmast();

                            if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                            {
                                workerDataSingle = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_Nopkj == SelectionList && x.fld_Kdaktf == StatusList && x.fld_Jenispekerja == JnsPkjList &&
                                                x.fld_Ktgpkj == WorkCategoryList && x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama)
                                    .SingleOrDefault();
                            }

                            else
                            {
                                workerDataSingle = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_Nopkj == SelectionList && x.fld_Jenispekerja == JnsPkjList && x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama)
                                    .SingleOrDefault();
                            }

                            if (workerDataSingle != null)
                            {
                                List<Int32> CutiTahunByBulanList = new List<Int32>();
                                List<Int32> CutiAmByBulanList = new List<Int32>();
                                //List<Int32> HadirHariMingguList = new List<Int32>();
                                List<Int32> CutiSakitByBulanList = new List<Int32>();
                                List<Int32> CutiMingguanList = new List<Int32>();
                                List<Int32> PontengList = new List<Int32>();

                                var leaveAllocation = dbview.tbl_CutiPeruntukan
                                    .Where(x => x.fld_NoPkj == SelectionList && x.fld_Tahun == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                var cutiTahunan = leaveAllocation
                                    .Where(x => x.fld_KodCuti == "C02")
                                    .Select(s => s.fld_JumlahCuti)
                                    .SingleOrDefault();

                                if (!cutiTahunan.HasValue)
                                {
                                    cutiTahunan = 0;
                                }

                                var cutiAm = leaveAllocation
                                    .Where(x => x.fld_KodCuti == "C01")
                                    .Select(s => s.fld_JumlahCuti)
                                    .SingleOrDefault();

                                if (!cutiAm.HasValue)
                                {
                                    cutiAm = 0;
                                }

                                var cutiSakit = leaveAllocation
                                    .Where(x => x.fld_KodCuti == "C03")
                                    .Select(s => s.fld_JumlahCuti)
                                    .SingleOrDefault();

                                if (!cutiSakit.HasValue)
                                {
                                    cutiSakit = 0;
                                }

                                for (var month = 1; month <= 12; month++)
                                {
                                    var cutiTahunByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "C02" && x.fld_Nopkj == SelectionList &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID
                                                    );

                                    CutiTahunByBulanList.Add(cutiTahunByBulan);

                                    var cutiAmByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "C01" && x.fld_Nopkj == SelectionList &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID
                                                    );
                                    CutiAmByBulanList.Add(cutiAmByBulan);

                                    var cutiSakitByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "C03" && x.fld_Nopkj == SelectionList &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID
                                                    );

                                    CutiSakitByBulanList.Add(cutiSakitByBulan);

                                    var cutiMingguanByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "C07" && x.fld_Nopkj == SelectionList &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    CutiMingguanList.Add(cutiMingguanByBulan);

                                    var pontengByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "P01" && x.fld_Nopkj == SelectionList &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    PontengList.Add(pontengByBulan);
                                }

                                MaklumatCutiPekerja.Add(
                                    new vw_MaklumatCutiPekerja
                                    {
                                        Pkjmast = workerDataSingle,
                                        CutiTahunan = cutiTahunan,
                                        CutiAm = cutiAm,
                                        CutiSakit = cutiSakit,
                                        CutiTahunByBulan = CutiTahunByBulanList,
                                        CutiAmByBulan = CutiAmByBulanList,
                                        CutiSakitByBulan = CutiSakitByBulanList,
                                        CutiMingguanByBulan = CutiMingguanList,
                                        PontengByBulan = PontengList
                                    });
                            }
                        }
                    }

                    if (MaklumatCutiPekerja.Count == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }

                    return View(MaklumatCutiPekerja);
                }

                else
                {
                    if (SelectionList == "0")
                    {
                        if (JnsPkjList == "0")
                        {
                            IOrderedQueryable<ViewingModels.tbl_Pkjmast> workerData;

                            if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                            {
                                workerData = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList &&
                                                x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            else
                            {
                                workerData = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            foreach (var i in workerData)
                            {
                                List<Int32> CutiTahunByBulanList = new List<Int32>();
                                List<Int32> CutiAmByBulanList = new List<Int32>();
                                List<Int32> CutiSakitByBulanList = new List<Int32>();
                                List<Int32> CutiMingguanList = new List<Int32>();
                                List<Int32> PontengList = new List<Int32>();

                                var leaveAllocation = dbview.tbl_CutiPeruntukan
                                    .Where(x => x.fld_NoPkj == i.fld_Nopkj && x.fld_Tahun == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                var cutiTahunan = leaveAllocation
                                    .Where(x => x.fld_KodCuti == "C02")
                                    .Select(s => s.fld_JumlahCuti)
                                    .SingleOrDefault();

                                if (!cutiTahunan.HasValue)
                                {
                                    cutiTahunan = 0;
                                }

                                var cutiAm = leaveAllocation
                                    .Where(x => x.fld_KodCuti == "C01")
                                    .Select(s => s.fld_JumlahCuti)
                                    .SingleOrDefault();

                                if (!cutiAm.HasValue)
                                {
                                    cutiAm = 0;
                                }

                                var cutiSakit = leaveAllocation
                                    .Where(x => x.fld_KodCuti == "C03")
                                    .Select(s => s.fld_JumlahCuti)
                                    .SingleOrDefault();

                                if (!cutiSakit.HasValue)
                                {
                                    cutiSakit = 0;
                                }

                                for (var month = 1; month <= 12; month++)
                                {
                                    var cutiTahunByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "C02" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID
                                                    );

                                    CutiTahunByBulanList.Add(cutiTahunByBulan);

                                    var cutiAmByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "C01" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID
                                                    );

                                    CutiAmByBulanList.Add(cutiAmByBulan);

                                    var cutiSakitByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "C03" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID
                                                    );

                                    CutiSakitByBulanList.Add(cutiSakitByBulan);

                                    var cutiMingguanByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "C07" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    CutiMingguanList.Add(cutiMingguanByBulan);

                                    var pontengByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "P01" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    PontengList.Add(pontengByBulan);

                                }

                                MaklumatCutiPekerja.Add(
                                    new vw_MaklumatCutiPekerja
                                    {
                                        Pkjmast = i,
                                        CutiTahunan = cutiTahunan,
                                        CutiAm = cutiAm,
                                        CutiSakit = cutiSakit,
                                        CutiTahunByBulan = CutiTahunByBulanList,
                                        CutiAmByBulan = CutiAmByBulanList,
                                        CutiSakitByBulan = CutiSakitByBulanList,
                                        CutiMingguanByBulan = CutiMingguanList,
                                        PontengByBulan = PontengList
                                    });
                            }

                            if (MaklumatCutiPekerja.Count == 0)
                            {
                                ViewBag.Message = GlobalResEstate.msgNoRecord;
                            }

                            return View(MaklumatCutiPekerja);
                        } else
                        {
                            IOrderedQueryable<ViewingModels.tbl_Pkjmast> workerData;

                            if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                            {
                                workerData = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList && x.fld_Jenispekerja == JnsPkjList &&
                                                x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            else
                            {
                                workerData = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_Jenispekerja == JnsPkjList &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            foreach (var i in workerData)
                            {
                                List<Int32> CutiTahunByBulanList = new List<Int32>();
                                List<Int32> CutiAmByBulanList = new List<Int32>();
                                List<Int32> CutiSakitByBulanList = new List<Int32>();
                                List<Int32> CutiMingguanList = new List<Int32>();
                                List<Int32> PontengList = new List<Int32>();

                                var leaveAllocation = dbview.tbl_CutiPeruntukan
                                    .Where(x => x.fld_NoPkj == i.fld_Nopkj && x.fld_Tahun == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                var cutiTahunan = leaveAllocation
                                    .Where(x => x.fld_KodCuti == "C02")
                                    .Select(s => s.fld_JumlahCuti)
                                    .SingleOrDefault();

                                if (!cutiTahunan.HasValue)
                                {
                                    cutiTahunan = 0;
                                }

                                var cutiAm = leaveAllocation
                                    .Where(x => x.fld_KodCuti == "C01")
                                    .Select(s => s.fld_JumlahCuti)
                                    .SingleOrDefault();

                                if (!cutiAm.HasValue)
                                {
                                    cutiAm = 0;
                                }

                                var cutiSakit = leaveAllocation
                                    .Where(x => x.fld_KodCuti == "C03")
                                    .Select(s => s.fld_JumlahCuti)
                                    .SingleOrDefault();

                                if (!cutiSakit.HasValue)
                                {
                                    cutiSakit = 0;
                                }

                                for (var month = 1; month <= 12; month++)
                                {
                                    var cutiTahunByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "C02" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID
                                                    );

                                    CutiTahunByBulanList.Add(cutiTahunByBulan);

                                    var cutiAmByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "C01" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID
                                                    );

                                    CutiAmByBulanList.Add(cutiAmByBulan);

                                    var cutiSakitByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "C03" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID
                                                    );

                                    CutiSakitByBulanList.Add(cutiSakitByBulan);

                                    var cutiMingguanByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "C07" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    CutiMingguanList.Add(cutiMingguanByBulan);

                                    var pontengByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "P01" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    PontengList.Add(pontengByBulan);

                                }

                                MaklumatCutiPekerja.Add(
                                    new vw_MaklumatCutiPekerja
                                    {
                                        Pkjmast = i,
                                        CutiTahunan = cutiTahunan,
                                        CutiAm = cutiAm,
                                        CutiSakit = cutiSakit,
                                        CutiTahunByBulan = CutiTahunByBulanList,
                                        CutiAmByBulan = CutiAmByBulanList,
                                        CutiSakitByBulan = CutiSakitByBulanList,
                                        CutiMingguanByBulan = CutiMingguanList,
                                        PontengByBulan = PontengList
                                    });
                            }

                            if (MaklumatCutiPekerja.Count == 0)
                            {
                                ViewBag.Message = GlobalResEstate.msgNoRecord;
                            }

                            return View(MaklumatCutiPekerja);
                        }
                    }

                    else
                    {
                        if (JnsPkjList == "0")
                        {
                            var groupData = dbview.tbl_KumpulanKerja
                            .Where(x => x.fld_KodKumpulan == SelectionList && x.fld_NegaraID == NegaraID &&
                                        x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                        x.fld_LadangID == LadangID)
                            .Select(s => s.fld_KumpulanID)
                            .SingleOrDefault();

                            IOrderedQueryable<ViewingModels.tbl_Pkjmast> workerData;

                            if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                            {
                                workerData = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_KumpulanID == groupData && x.fld_Kdaktf == StatusList &&
                                                x.fld_Ktgpkj == WorkCategoryList &&
                                                x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            else
                            {
                                workerData = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_KumpulanID == groupData && x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            foreach (var i in workerData)
                            {
                                List<Int32> CutiTahunByBulanList = new List<Int32>();
                                List<Int32> CutiAmByBulanList = new List<Int32>();
                                List<Int32> CutiSakitByBulanList = new List<Int32>();
                                List<Int32> CutiMingguanList = new List<Int32>();
                                List<Int32> PontengList = new List<Int32>();

                                var leaveAllocation = dbview.tbl_CutiPeruntukan
                                    .Where(x => x.fld_NoPkj == i.fld_Nopkj && x.fld_Tahun == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                var cutiTahunan = leaveAllocation
                                    .Where(x => x.fld_KodCuti == "C02")
                                    .Select(s => s.fld_JumlahCuti)
                                    .SingleOrDefault();

                                if (!cutiTahunan.HasValue)
                                {
                                    cutiTahunan = 0;
                                }

                                var cutiAm = leaveAllocation
                                    .Where(x => x.fld_KodCuti == "C01")
                                    .Select(s => s.fld_JumlahCuti)
                                    .SingleOrDefault();

                                if (!cutiAm.HasValue)
                                {
                                    cutiAm = 0;
                                }

                                var cutiSakit = leaveAllocation
                                    .Where(x => x.fld_KodCuti == "C03")
                                    .Select(s => s.fld_JumlahCuti)
                                    .SingleOrDefault();

                                if (!cutiSakit.HasValue)
                                {
                                    cutiSakit = 0;
                                }

                                for (var month = 1; month <= 12; month++)
                                {
                                    var cutiTahunByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "C02" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID
                                                    );

                                    CutiTahunByBulanList.Add(cutiTahunByBulan);

                                    var cutiAmByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "C01" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID
                                                    );

                                    CutiAmByBulanList.Add(cutiAmByBulan);

                                    var cutiSakitByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "C03" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID
                                                    );

                                    CutiSakitByBulanList.Add(cutiSakitByBulan);

                                    var cutiMingguanByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "C07" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    CutiMingguanList.Add(cutiMingguanByBulan);

                                    var pontengByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "P01" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    PontengList.Add(pontengByBulan);

                                }

                                MaklumatCutiPekerja.Add(
                                    new vw_MaklumatCutiPekerja
                                    {
                                        Pkjmast = i,
                                        CutiTahunan = cutiTahunan,
                                        CutiAm = cutiAm,
                                        CutiSakit = cutiSakit,
                                        CutiTahunByBulan = CutiTahunByBulanList,
                                        CutiAmByBulan = CutiAmByBulanList,
                                        CutiSakitByBulan = CutiSakitByBulanList,
                                        CutiMingguanByBulan = CutiMingguanList,
                                        PontengByBulan = PontengList
                                    });
                            }
                        }
                        else
                        {
                            var groupData = dbview.tbl_KumpulanKerja
                            .Where(x => x.fld_KodKumpulan == SelectionList && x.fld_NegaraID == NegaraID &&
                                        x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                        x.fld_LadangID == LadangID)
                            .Select(s => s.fld_KumpulanID)
                            .SingleOrDefault();

                            IOrderedQueryable<ViewingModels.tbl_Pkjmast> workerData;

                            if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                            {
                                workerData = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_KumpulanID == groupData && x.fld_Kdaktf == StatusList && x.fld_Jenispekerja == JnsPkjList &&
                                                x.fld_Ktgpkj == WorkCategoryList &&
                                                x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            else
                            {
                                workerData = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_KumpulanID == groupData && x.fld_NegaraID == NegaraID && x.fld_Jenispekerja == JnsPkjList &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            foreach (var i in workerData)
                            {
                                List<Int32> CutiTahunByBulanList = new List<Int32>();
                                List<Int32> CutiAmByBulanList = new List<Int32>();
                                List<Int32> CutiSakitByBulanList = new List<Int32>();
                                List<Int32> CutiMingguanList = new List<Int32>();
                                List<Int32> PontengList = new List<Int32>();

                                var leaveAllocation = dbview.tbl_CutiPeruntukan
                                    .Where(x => x.fld_NoPkj == i.fld_Nopkj && x.fld_Tahun == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                var cutiTahunan = leaveAllocation
                                    .Where(x => x.fld_KodCuti == "C02")
                                    .Select(s => s.fld_JumlahCuti)
                                    .SingleOrDefault();

                                if (!cutiTahunan.HasValue)
                                {
                                    cutiTahunan = 0;
                                }

                                var cutiAm = leaveAllocation
                                    .Where(x => x.fld_KodCuti == "C01")
                                    .Select(s => s.fld_JumlahCuti)
                                    .SingleOrDefault();

                                if (!cutiAm.HasValue)
                                {
                                    cutiAm = 0;
                                }

                                var cutiSakit = leaveAllocation
                                    .Where(x => x.fld_KodCuti == "C03")
                                    .Select(s => s.fld_JumlahCuti)
                                    .SingleOrDefault();

                                if (!cutiSakit.HasValue)
                                {
                                    cutiSakit = 0;
                                }

                                for (var month = 1; month <= 12; month++)
                                {
                                    var cutiTahunByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "C02" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID
                                                    );

                                    CutiTahunByBulanList.Add(cutiTahunByBulan);

                                    var cutiAmByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "C01" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID
                                                    );

                                    CutiAmByBulanList.Add(cutiAmByBulan);

                                    var cutiSakitByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "C03" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID
                                                    );

                                    CutiSakitByBulanList.Add(cutiSakitByBulan);

                                    var cutiMingguanByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "C07" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    CutiMingguanList.Add(cutiMingguanByBulan);

                                    var pontengByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "P01" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    PontengList.Add(pontengByBulan);

                                }

                                MaklumatCutiPekerja.Add(
                                    new vw_MaklumatCutiPekerja
                                    {
                                        Pkjmast = i,
                                        CutiTahunan = cutiTahunan,
                                        CutiAm = cutiAm,
                                        CutiSakit = cutiSakit,
                                        CutiTahunByBulan = CutiTahunByBulanList,
                                        CutiAmByBulan = CutiAmByBulanList,
                                        CutiSakitByBulan = CutiSakitByBulanList,
                                        CutiMingguanByBulan = CutiMingguanList,
                                        PontengByBulan = PontengList
                                    });
                            }
                        }
                    }

                    if (MaklumatCutiPekerja.Count == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }

                    return View(MaklumatCutiPekerja);
                }
            }
        }

        public ActionResult _WorkerLeaveRptAdvanceSearch()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var statusList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            ViewBag.StatusList = statusList;

            var workCategoryList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "designation" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            ViewBag.WorkCategoryList = workCategoryList;

            return View();
        }

        public ActionResult _WorkerAnnualLeaveByMonth(string nopkj, int? month, int? year)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var getAnnualLeave = dbview.tbl_Kerjahdr
                .Where(x => x.fld_Kdhdct == "C02" && x.fld_Nopkj == nopkj && x.fld_Tarikh.Value.Month == month &&
                           x.fld_Tarikh.Value.Year == year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                .OrderBy(o => o.fld_Tarikh);

            return View(getAnnualLeave);
        }

        public ActionResult _WorkerPublicHolidayByMonth(string nopkj, int? month, int? year)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var getPublicHoliday = dbview.tbl_Kerjahdr
                .Where(x => x.fld_Kdhdct == "C01" && x.fld_Nopkj == nopkj && x.fld_Tarikh.Value.Month == month &&
                           x.fld_Tarikh.Value.Year == year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                .OrderBy(o => o.fld_Tarikh);


            return View(getPublicHoliday);
        }
        public ActionResult _WorkerWeeklyLeaveByMonth(string nopkj, int? month, int? year)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var cutiMingguanByBulan = dbview.tbl_Kerjahdr
                .Where(x => x.fld_Kdhdct == "C07" && x.fld_Nopkj == nopkj &&
                            x.fld_Tarikh.Value.Month == month && x.fld_Tarikh.Value.Year == year &&
                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                .OrderBy(o => o.fld_Tarikh);

            return View(cutiMingguanByBulan);
        }

        public ActionResult _WorkerSkipByMonth(string nopkj, int? month, int? year)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var pontengByBulan = dbview.tbl_Kerjahdr
                .Where(x => x.fld_Kdhdct == "P01" && x.fld_Nopkj == nopkj &&
                            x.fld_Tarikh.Value.Month == month && x.fld_Tarikh.Value.Year == year &&
                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                .OrderBy(o => o.fld_Tarikh);

            return View(pontengByBulan);
        }

        public ActionResult AttendanceReport()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int drpyear = 0;
            int drprangeyear = 0;
            int month = timezone.gettimezone().Month;

            List<SelectListItem> SelectionList = new List<SelectListItem>();
            SelectionList = new SelectList(
                dbr.tbl_Pkjmast
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1")
                    .OrderBy(o => o.fld_Nopkj)
                    .Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }),
                "Value", "Text").ToList();
            SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.SelectionList = SelectionList;
            

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

            var statusList = new List<SelectListItem>();
            statusList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            ViewBag.StatusList = statusList;

            List<SelectListItem> JnsPkjList = new List<SelectListItem>();
            JnsPkjList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            JnsPkjList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
            ViewBag.JnsPkjList = JnsPkjList;

            return View();
        }

        public ViewResult _WorkerAttendanceRptSearch(int? RadioGroup, int? YearList,
            string SelectionList, string StatusList, string WorkCategoryList, string JnsPkjList, string print)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            List<vw_MaklumatKehadiranPekerja> MaklumatKehadiranPekerja = new List<vw_MaklumatKehadiranPekerja>();

            ViewBag.YearList = YearList;
            ViewBag.WorkerList = SelectionList;
            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();
            ViewBag.Print = print;

            if (YearList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChooseWork;
                return View(MaklumatKehadiranPekerja);
            }

            else
            {
                if (RadioGroup == 0)
                {
                    if (SelectionList == "0")
                    {
                        if (JnsPkjList == "0")
                        {
                            IOrderedQueryable<ViewingModels.tbl_Pkjmast> workerData;

                            if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                            {
                                workerData = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList &&
                                                x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            else
                            {
                                workerData = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            foreach (var i in workerData)
                            {
                                List<Int32> hadirHariBiasaList = new List<Int32>();
                                List<Int32> hadirHariMingguList = new List<Int32>();
                                List<Int32> hadirHariCutiUmumList = new List<Int32>();

                                for (var month = 1; month <= 12; month++)
                                {
                                    var hadirHariBiasaByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "H01" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    hadirHariBiasaList.Add(hadirHariBiasaByBulan);

                                    var hadirHariMingguByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "H02" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    hadirHariMingguList.Add(hadirHariMingguByBulan);

                                    var hadirHariCutiUmumByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "H03" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    hadirHariCutiUmumList.Add(hadirHariCutiUmumByBulan);
                                }

                                MaklumatKehadiranPekerja.Add(
                                    new vw_MaklumatKehadiranPekerja
                                    {
                                        Pkjmast = i,
                                        HadirHariBiasaByBulan = hadirHariBiasaList,
                                        HadirHariMingguByBulan = hadirHariMingguList,
                                        HadirHariCutiUmumByBulan = hadirHariCutiUmumList

                                    });
                            }

                            if (MaklumatKehadiranPekerja.Count == 0)
                            {
                                ViewBag.Message = GlobalResEstate.msgNoRecord;
                            }

                            return View(MaklumatKehadiranPekerja);
                        }
                        else
                        {
                            IOrderedQueryable<ViewingModels.tbl_Pkjmast> workerData;

                            if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                            {
                                workerData = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList && x.fld_Jenispekerja == JnsPkjList &&
                                                x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            else
                            {
                                workerData = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_Jenispekerja == JnsPkjList &&
                                                x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            foreach (var i in workerData)
                            {
                                List<Int32> hadirHariBiasaList = new List<Int32>();
                                List<Int32> hadirHariMingguList = new List<Int32>();
                                List<Int32> hadirHariCutiUmumList = new List<Int32>();

                                for (var month = 1; month <= 12; month++)
                                {
                                    var hadirHariBiasaByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "H01" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    hadirHariBiasaList.Add(hadirHariBiasaByBulan);

                                    var hadirHariMingguByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "H02" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    hadirHariMingguList.Add(hadirHariMingguByBulan);

                                    var hadirHariCutiUmumByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "H03" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    hadirHariCutiUmumList.Add(hadirHariCutiUmumByBulan);
                                }

                                MaklumatKehadiranPekerja.Add(
                                    new vw_MaklumatKehadiranPekerja
                                    {
                                        Pkjmast = i,
                                        HadirHariBiasaByBulan = hadirHariBiasaList,
                                        HadirHariMingguByBulan = hadirHariMingguList,
                                        HadirHariCutiUmumByBulan = hadirHariCutiUmumList

                                    });
                            }

                            if (MaklumatKehadiranPekerja.Count == 0)
                            {
                                ViewBag.Message = GlobalResEstate.msgNoRecord;
                            }

                            return View(MaklumatKehadiranPekerja);
                        }
                        
                    }

                    else
                    {
                        if (JnsPkjList == "0")
                        {
                            var workerDataSingle = new ViewingModels.tbl_Pkjmast();

                            if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                            {
                                workerDataSingle = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_Nopkj == SelectionList && x.fld_Kdaktf == StatusList &&
                                                x.fld_Ktgpkj == WorkCategoryList && x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama)
                                    .SingleOrDefault();
                            }

                            else
                            {
                                workerDataSingle = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_Nopkj == SelectionList && x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama)
                                    .SingleOrDefault();
                            }

                            if (workerDataSingle != null)
                            {
                                List<Int32> hadirHariBiasaList = new List<Int32>();
                                List<Int32> hadirHariMingguList = new List<Int32>();
                                List<Int32> hadirHariCutiUmumList = new List<Int32>();

                                for (var month = 1; month <= 12; month++)
                                {
                                    var hadirHariBiasaByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "H01" && x.fld_Nopkj == SelectionList &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    hadirHariBiasaList.Add(hadirHariBiasaByBulan);

                                    var hadirHariMingguByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "H02" && x.fld_Nopkj == SelectionList &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    hadirHariMingguList.Add(hadirHariMingguByBulan);

                                    var hadirHariCutiUmumByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "H03" && x.fld_Nopkj == SelectionList &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    hadirHariCutiUmumList.Add(hadirHariCutiUmumByBulan);
                                }

                                MaklumatKehadiranPekerja.Add(
                                    new vw_MaklumatKehadiranPekerja
                                    {
                                        Pkjmast = workerDataSingle,
                                        HadirHariBiasaByBulan = hadirHariBiasaList,
                                        HadirHariMingguByBulan = hadirHariMingguList,
                                        HadirHariCutiUmumByBulan = hadirHariCutiUmumList

                                    });
                            }
                        }
                        else
                        {
                            var workerDataSingle = new ViewingModels.tbl_Pkjmast();

                            if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                            {
                                workerDataSingle = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_Nopkj == SelectionList && x.fld_Kdaktf == StatusList && x.fld_Jenispekerja == JnsPkjList &&
                                                x.fld_Ktgpkj == WorkCategoryList && x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama)
                                    .SingleOrDefault();
                            }

                            else
                            {
                                workerDataSingle = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_Nopkj == SelectionList && x.fld_Jenispekerja == JnsPkjList && x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama)
                                    .SingleOrDefault();
                            }

                            if (workerDataSingle != null)
                            {
                                List<Int32> hadirHariBiasaList = new List<Int32>();
                                List<Int32> hadirHariMingguList = new List<Int32>();
                                List<Int32> hadirHariCutiUmumList = new List<Int32>();

                                for (var month = 1; month <= 12; month++)
                                {
                                    var hadirHariBiasaByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "H01" && x.fld_Nopkj == SelectionList &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    hadirHariBiasaList.Add(hadirHariBiasaByBulan);

                                    var hadirHariMingguByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "H02" && x.fld_Nopkj == SelectionList &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    hadirHariMingguList.Add(hadirHariMingguByBulan);

                                    var hadirHariCutiUmumByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "H03" && x.fld_Nopkj == SelectionList &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    hadirHariCutiUmumList.Add(hadirHariCutiUmumByBulan);
                                }

                                MaklumatKehadiranPekerja.Add(
                                    new vw_MaklumatKehadiranPekerja
                                    {
                                        Pkjmast = workerDataSingle,
                                        HadirHariBiasaByBulan = hadirHariBiasaList,
                                        HadirHariMingguByBulan = hadirHariMingguList,
                                        HadirHariCutiUmumByBulan = hadirHariCutiUmumList

                                    });
                            }
                        }
                        

                    }

                    if (MaklumatKehadiranPekerja.Count == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }

                    return View(MaklumatKehadiranPekerja);
                }

                else
                {
                    if (SelectionList == "0")
                    {
                        if (JnsPkjList == "0")
                        {
                            IOrderedQueryable<ViewingModels.tbl_Pkjmast> workerData;

                            if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                            {
                                workerData = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList &&
                                                x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            else
                            {
                                workerData = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            foreach (var i in workerData)
                            {
                                List<Int32> hadirHariBiasaList = new List<Int32>();
                                List<Int32> hadirHariMingguList = new List<Int32>();
                                List<Int32> hadirHariCutiUmumList = new List<Int32>();

                                for (var month = 1; month <= 12; month++)
                                {
                                    var hadirHariBiasaByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "H01" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    hadirHariBiasaList.Add(hadirHariBiasaByBulan);

                                    var hadirHariMingguByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "H02" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    hadirHariMingguList.Add(hadirHariMingguByBulan);

                                    var hadirHariCutiUmumByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "H03" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    hadirHariCutiUmumList.Add(hadirHariCutiUmumByBulan);
                                }

                                MaklumatKehadiranPekerja.Add(
                                    new vw_MaklumatKehadiranPekerja
                                    {
                                        Pkjmast = i,
                                        HadirHariBiasaByBulan = hadirHariBiasaList,
                                        HadirHariMingguByBulan = hadirHariMingguList,
                                        HadirHariCutiUmumByBulan = hadirHariCutiUmumList

                                    });
                            }

                            if (MaklumatKehadiranPekerja.Count == 0)
                            {
                                ViewBag.Message = GlobalResEstate.msgNoRecord;
                            }

                            return View(MaklumatKehadiranPekerja);
                        }
                        else
                        {
                            IOrderedQueryable<ViewingModels.tbl_Pkjmast> workerData;

                            if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                            {
                                workerData = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList && x.fld_Jenispekerja == JnsPkjList &&
                                                x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            else
                            {
                                workerData = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_Jenispekerja == JnsPkjList &&
                                                x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            foreach (var i in workerData)
                            {
                                List<Int32> hadirHariBiasaList = new List<Int32>();
                                List<Int32> hadirHariMingguList = new List<Int32>();
                                List<Int32> hadirHariCutiUmumList = new List<Int32>();

                                for (var month = 1; month <= 12; month++)
                                {
                                    var hadirHariBiasaByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "H01" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    hadirHariBiasaList.Add(hadirHariBiasaByBulan);

                                    var hadirHariMingguByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "H02" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    hadirHariMingguList.Add(hadirHariMingguByBulan);

                                    var hadirHariCutiUmumByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "H03" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    hadirHariCutiUmumList.Add(hadirHariCutiUmumByBulan);
                                }

                                MaklumatKehadiranPekerja.Add(
                                    new vw_MaklumatKehadiranPekerja
                                    {
                                        Pkjmast = i,
                                        HadirHariBiasaByBulan = hadirHariBiasaList,
                                        HadirHariMingguByBulan = hadirHariMingguList,
                                        HadirHariCutiUmumByBulan = hadirHariCutiUmumList

                                    });
                            }

                            if (MaklumatKehadiranPekerja.Count == 0)
                            {
                                ViewBag.Message = GlobalResEstate.msgNoRecord;
                            }

                            return View(MaklumatKehadiranPekerja);
                        }
                    }

                    else
                    {
                       if (JnsPkjList == "0")
                        {
                            var groupData = dbview.tbl_KumpulanKerja
                            .Where(x => x.fld_KodKumpulan == SelectionList && x.fld_NegaraID == NegaraID &&
                                        x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                        x.fld_LadangID == LadangID)
                            .Select(s => s.fld_KumpulanID)
                            .SingleOrDefault();

                            IOrderedQueryable<ViewingModels.tbl_Pkjmast> workerData;

                            if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                            {
                                workerData = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_KumpulanID == groupData && x.fld_Kdaktf == StatusList &&
                                                x.fld_Ktgpkj == WorkCategoryList &&
                                                x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            else
                            {
                                workerData = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_KumpulanID == groupData && x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            foreach (var i in workerData)
                            {
                                List<Int32> hadirHariBiasaList = new List<Int32>();
                                List<Int32> hadirHariMingguList = new List<Int32>();
                                List<Int32> hadirHariCutiUmumList = new List<Int32>();

                                for (var month = 1; month <= 12; month++)
                                {
                                    var hadirHariBiasaByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "H01" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    hadirHariBiasaList.Add(hadirHariBiasaByBulan);

                                    var hadirHariMingguByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "H02" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    hadirHariMingguList.Add(hadirHariMingguByBulan);

                                    var hadirHariCutiUmumByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "H03" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    hadirHariCutiUmumList.Add(hadirHariCutiUmumByBulan);
                                }

                                MaklumatKehadiranPekerja.Add(
                                    new vw_MaklumatKehadiranPekerja
                                    {
                                        Pkjmast = i,
                                        HadirHariBiasaByBulan = hadirHariBiasaList,
                                        HadirHariMingguByBulan = hadirHariMingguList,
                                        HadirHariCutiUmumByBulan = hadirHariCutiUmumList

                                    });
                            }

                            if (MaklumatKehadiranPekerja.Count == 0)
                            {
                                ViewBag.Message = GlobalResEstate.msgNoRecord;
                            }

                            return View(MaklumatKehadiranPekerja);
                        }
                        else
                        {
                            var groupData = dbview.tbl_KumpulanKerja
                            .Where(x => x.fld_KodKumpulan == SelectionList && x.fld_NegaraID == NegaraID &&
                                        x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                        x.fld_LadangID == LadangID)
                            .Select(s => s.fld_KumpulanID)
                            .SingleOrDefault();

                            IOrderedQueryable<ViewingModels.tbl_Pkjmast> workerData;

                            if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                            {
                                workerData = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_KumpulanID == groupData && x.fld_Kdaktf == StatusList && x.fld_Jenispekerja == JnsPkjList &&
                                                x.fld_Ktgpkj == WorkCategoryList &&
                                                x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            else
                            {
                                workerData = dbview.tbl_Pkjmast
                                    .Where(x => x.fld_KumpulanID == groupData && x.fld_Jenispekerja == JnsPkjList && x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            foreach (var i in workerData)
                            {
                                List<Int32> hadirHariBiasaList = new List<Int32>();
                                List<Int32> hadirHariMingguList = new List<Int32>();
                                List<Int32> hadirHariCutiUmumList = new List<Int32>();

                                for (var month = 1; month <= 12; month++)
                                {
                                    var hadirHariBiasaByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "H01" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    hadirHariBiasaList.Add(hadirHariBiasaByBulan);

                                    var hadirHariMingguByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "H02" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    hadirHariMingguList.Add(hadirHariMingguByBulan);

                                    var hadirHariCutiUmumByBulan = dbview.tbl_Kerjahdr
                                        .Count(x => x.fld_Kdhdct == "H03" && x.fld_Nopkj == i.fld_Nopkj &&
                                                    x.fld_Tarikh.Value.Month == month &&
                                                    x.fld_Tarikh.Value.Year == YearList &&
                                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    hadirHariCutiUmumList.Add(hadirHariCutiUmumByBulan);
                                }

                                MaklumatKehadiranPekerja.Add(
                                    new vw_MaklumatKehadiranPekerja
                                    {
                                        Pkjmast = i,
                                        HadirHariBiasaByBulan = hadirHariBiasaList,
                                        HadirHariMingguByBulan = hadirHariMingguList,
                                        HadirHariCutiUmumByBulan = hadirHariCutiUmumList

                                    });
                            }

                            if (MaklumatKehadiranPekerja.Count == 0)
                            {
                                ViewBag.Message = GlobalResEstate.msgNoRecord;
                            }

                            return View(MaklumatKehadiranPekerja);
                        }
                    }
                }
            }
        }

        public ActionResult _WorkerRegularDayAttendanceByMonth(string nopkj, int? month, int? year)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            var getRegularDayAttendance = dbview.tbl_Kerjahdr
                .Where(x => x.fld_Kdhdct == "H01" && x.fld_Nopkj == nopkj &&
                            x.fld_Tarikh.Value.Month == month &&
                            x.fld_Tarikh.Value.Year == year &&
                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                .OrderBy(o => o.fld_Tarikh);

            return View(getRegularDayAttendance);
        }

        public ActionResult _WorkerWeekendAttendanceByMonth(string nopkj, int? month, int? year)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            var getWeekendAttendance = dbview.tbl_Kerjahdr
                .Where(x => x.fld_Kdhdct == "H02" && x.fld_Nopkj == nopkj &&
                            x.fld_Tarikh.Value.Month == month && x.fld_Tarikh.Value.Year == year &&
                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                .OrderBy(o => o.fld_Tarikh);

            return View(getWeekendAttendance);
        }

        public ActionResult _WorkerPublicHolidayAttendanceByMonth(string nopkj, int? month, int? year)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            var hadirHariCutiUmumByBulan = dbview.tbl_Kerjahdr
                .Where(x => x.fld_Kdhdct == "H03" && x.fld_Nopkj == nopkj &&
                            x.fld_Tarikh.Value.Month == month &&
                            x.fld_Tarikh.Value.Year == year &&
                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                .OrderBy(o => o.fld_Tarikh);

            return View(hadirHariCutiUmumByBulan);
        }

        public ActionResult _WorkerAttendanceRptAdvanceSearch()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var statusList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            ViewBag.StatusList = statusList;

            var workCategoryList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "designation" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            ViewBag.WorkCategoryList = workCategoryList;

            return View();
        }

        public ActionResult TransactionListingReport()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int drpyear = 0;
            int drprangeyear = 0;
            int month = timezone.gettimezone().Month;

            List<SelectListItem> SelectionList = new List<SelectListItem>();
            SelectionList = new SelectList(
                dbr.tbl_Pkjmast
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                    .OrderBy(o => o.fld_Nopkj)
                    .Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }),
                "Value", "Text").ToList();
            SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.SelectionList = SelectionList;

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
                "fldOptConfValue", "fldOptConfDesc");

            ViewBag.YearList = yearlist;
            ViewBag.UserID = getuserid;

            return View();
        }

        public ViewResult _TransactionListingRptSearch(int? MonthList, int? YearList, string print)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Viewing dbview2 = new MVC_SYSTEM_Viewing();


            ViewBag.MonthList = MonthList;
            ViewBag.YearList = YearList;

            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.NamaPengurus = dbview2.tbl_Ladang
                .Where(x => x.fld_ID == LadangID)
                .Select(s => s.fld_Pengurus).Single();
            ViewBag.NamaPenyelia = dbview2.tblUsers
                .Where(x => x.fldUserID == getuserid)
                .Select(s => s.fldUserFullName).Single();
            ViewBag.IDPenyelia = getuserid;
            ViewBag.Print = print;


            if (MonthList == null && YearList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChooseMonthYear;
                return View();
            }

            else
            {
                var GetCotribution = db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag3 == "Employee" && x.fldDeleted == false).Select(s => s.fldOptConfValue).ToList();

                var TransactionListingList = dbview.vw_RptSctran
                    .Where(x => !GetCotribution.Contains(x.fld_KodAktvt) && x.fld_Month == MonthList &&
                                x.fld_Year == YearList && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID)
                    .OrderBy(o => new { o.fld_Kategori, o.fld_Amt });

                if (!TransactionListingList.Any())
                {
                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                    return View();

                }

                ViewBag.UserID = getuserid;
                return View(TransactionListingList);
            }
        }

        public ActionResult PaySlipReport()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int drpyear = 0;
            int drprangeyear = 0;
            int month = timezone.gettimezone().Month;

            List<SelectListItem> SelectionList = new List<SelectListItem>();
            SelectionList = new SelectList(
                dbr.tbl_Pkjmast
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1")
                    .OrderBy(o => o.fld_Nopkj)
                    .Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }),
                "Value", "Text").ToList();
            SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.SelectionList = SelectionList;

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

            var statusList = new List<SelectListItem>();
            statusList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            var monthList = new SelectList(
                db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false &&
                                                   x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID),
                "fldOptConfValue", "fldOptConfDesc", month);

            ViewBag.MonthList = monthList;
            ViewBag.StatusList = statusList;

            return View();
        }

        public ActionResult _WorkerPaySlipRptAdvanceSearch()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var statusList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            ViewBag.StatusList = statusList;

            var workCategoryList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "designation" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            ViewBag.WorkCategoryList = workCategoryList;

            return View();
        }

        //public ActionResult PrintWorkerPaySlipPdf(int? RadioGroup, int? MonthList, int? YearList,
        //    string SelectionList, string StatusList, string WorkCategoryList, int id, string genid)
        //{
        //    int? getuserid = 0;
        //    string getusername = "";
        //    string getcookiesval = "";
        //    bool checkidentity = false;
        //    //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
        //    var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
        //    if (getuser != null)
        //    {
        //        getuserid = GetIdentity.ID(getuser.fldUserName);
        //        getusername = getuser.fldUserName;
        //    }

        //    checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

        //    ActionAsPdf report = new ActionAsPdf("");

        //    if (checkidentity)
        //    {
        //        getBackAuth(getcookiesval);
        //        var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
        //        //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
        //        string print = "Yes";
        //        report = new ActionAsPdf("_WorkerPaySlipRptSearch", new { RadioGroup, MonthList, YearList, SelectionList, StatusList, WorkCategoryList, print })
        //        {
        //            FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName,
        //            Cookies = cookies
        //        };
        //    }
        //    else
        //    {
        //        report = new ActionAsPdf("PDFInvalidGen");
        //    }
        //}

        //    ActionAsPdf report = new ActionAsPdf("");

        //    if (checkidentity)
        //    {
        //        getBackAuth(getcookiesval);
        //        string print = "Yes";
        //        report = new ActionAsPdf("_WorkerPaySlipRptSearch", new { RadioGroup, MonthList, YearList, SelectionList, StatusList, WorkCategoryList, print });
        //    }
        //    else
        //    {
        //        report = new ActionAsPdf("PDFInvalidGen");
        //    }

        //    return report;
        //}

        public string PDFInvalidGen()
        {
            return GlobalResEstate.msgInvalidPDFConvert;
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
            catch(Exception ex)
            {
                result = false;
            }
            return result;
        }

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
                    //geterror.testlog("Try if : " + User.Identity.Name, "Try if : " + CookiesValue, "Try if : " + getcookiesval);
                }
                //geterror.testlog("Try no if : " + User.Identity.Name, "Try no if : " + CookiesValue, "Try no if : " + getcookiesval);
            }
            catch
            {
                HttpCookie authoCookies = new HttpCookie(FormsAuthentication.FormsCookieName, getcookiesval);
                Response.SetCookie(authoCookies);
                //geterror.testlog("Catch : " + User.Identity.Name, "Catch : " + CookiesValue, "Catch : " + getcookiesval);
            }
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
        }
        
        public ViewResult _WorkerPaySlipRptSearch(int? RadioGroup, int? MonthList, int? YearList,
            string SelectionList, string StatusList, string WorkCategoryList, string print)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            //if (print == "Yes" && User.Identity.Name == "")
            //{
            //    geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "Print Mode : " + print);
            //}
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<vw_PaySlipPekerja> PaySlipPekerja = new List<vw_PaySlipPekerja>();

            ViewBag.MonthList = MonthList;
            ViewBag.YearList = YearList;
            ViewBag.WorkerList = SelectionList;
            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.Print = print;

            if (MonthList == null && YearList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChooseWork;
                return View(PaySlipPekerja);
            }

            else
            {
                if (RadioGroup == 0)
                {
                    if (SelectionList == "0")
                    {
                        IOrderedQueryable<ViewingModels.vw_GajiPekerja> workerData;

                        if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                        {
                            workerData = dbview.vw_GajiPekerja
                                .Where(x => x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList &&
                                            x.fld_Year == YearList && x.fld_Month == MonthList &&
                                            x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        else
                        {
                            workerData = dbview.vw_GajiPekerja
                                .Where(x => x.fld_NegaraID == NegaraID &&
                                            x.fld_Year == YearList && x.fld_Month == MonthList &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        foreach (var i in workerData)
                        {
                            List<ViewingModels.vw_MaklumatInsentif> workerIncentiveRecordList = new List<ViewingModels.vw_MaklumatInsentif>();

                            List<FootNoteCustomModel> footNoteCustomModelList = new List<FootNoteCustomModel>();

                            var workerMonthlySalary = dbview.tbl_GajiBulanan
                                .SingleOrDefault(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Month == MonthList &&
                                                      x.fld_Year == YearList && x.fld_NegaraID == NegaraID &&
                                                      x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                                      x.fld_LadangID == LadangID);

                            List<CarumanTambahanCustomModel> carumanTambahanCustomModelList = new List<CarumanTambahanCustomModel>();

                            var workerAdditionalContribution = dbr.tbl_ByrCarumanTambahan
                                .Where(x => x.fld_GajiID == i.fld_ID && x.fld_Month == MonthList &&
                                            x.fld_Year == YearList && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                            x.fld_LadangID == LadangID);

                            foreach (var caruman in workerAdditionalContribution)
                            {
                                CarumanTambahanCustomModel carumanTambahanCustomModel = new CarumanTambahanCustomModel();

                                carumanTambahanCustomModel.fld_ID = caruman.fld_ID;
                                carumanTambahanCustomModel.fld_KodCarumanTambahan = caruman.fld_KodSubCaruman;
                                carumanTambahanCustomModel.fld_CarumanMajikan = caruman.fld_CarumanMajikan;
                                carumanTambahanCustomModel.fld_CarumanPekerja = caruman.fld_CarumanPekerja;

                                carumanTambahanCustomModelList.Add(carumanTambahanCustomModel);
                            }

                            var workerIncentiveRecord = dbview.vw_MaklumatInsentif
                                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Month == MonthList &&
                                            x.fld_Year == YearList && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                            x.fld_LadangID == LadangID && x.fld_Deleted == false);

                            foreach (var insentifRecord in workerIncentiveRecord)
                            {
                                workerIncentiveRecordList.Add(insentifRecord);
                            }

                            List<KerjaPekerjaCustomModel> kerjaPekerjaCustomModelList = new List<KerjaPekerjaCustomModel>();

                            var workerWorkRecordGroupBy = dbview.vw_KerjaPekerja
                                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => new { x.fld_KodAktvt, x.fld_KodPkt, x.fld_Kdhdct })
                                .OrderBy(o => o.Key.fld_KodAktvt)
                                .ThenBy(t => t.Key.fld_KodPkt)
                                .ThenBy(t2 => t2.Key.fld_Kdhdct)
                                .Select(lg =>
                                    new
                                    {
                                        fld_ID = lg.FirstOrDefault().fld_ID,
                                        fld_Desc = lg.FirstOrDefault().fld_Desc,
                                        fld_KodPkt = lg.FirstOrDefault().fld_KodPkt,
                                        fld_JumlahHasil = lg.Sum(w => w.fld_JumlahHasil),
                                        fld_Unit = lg.FirstOrDefault().fld_Unit,
                                        fld_KadarByr = lg.FirstOrDefault().fld_KadarByr,
                                        fld_Gandaan = lg.FirstOrDefault().fldOptConfFlag3,
                                        fld_TotalAmount = lg.Sum(w => w.fld_Amount)
                                    });

                            foreach (var work in workerWorkRecordGroupBy)
                            {
                                KerjaPekerjaCustomModel kerjaPekerjaCustomModel = new KerjaPekerjaCustomModel();

                                kerjaPekerjaCustomModel.fld_ID = work.fld_ID;
                                kerjaPekerjaCustomModel.fld_Desc = work.fld_Desc;
                                kerjaPekerjaCustomModel.fld_KodPkt = work.fld_KodPkt;
                                kerjaPekerjaCustomModel.fld_JumlahHasil = work.fld_JumlahHasil;
                                kerjaPekerjaCustomModel.fld_Unit = work.fld_Unit;
                                kerjaPekerjaCustomModel.fld_KadarByr = work.fld_KadarByr;
                                kerjaPekerjaCustomModel.fld_Gandaan = work.fld_Gandaan;
                                kerjaPekerjaCustomModel.fld_TotalAmount = work.fld_TotalAmount;

                                kerjaPekerjaCustomModelList.Add(kerjaPekerjaCustomModel);
                            }

                            List<OTPekerjaCustomModel> otPekerjaCustomModelList = new List<OTPekerjaCustomModel>();

                            var workerOTRecordGroupBy = dbview.vw_OTPekerja
                                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => x.fld_Kdhdct)
                                .OrderBy(o => o.Key)
                                .Select(lg =>
                                    new
                                    {
                                        fld_ID = lg.FirstOrDefault().fld_ID,
                                        fld_JumlahJamOT = lg.Sum(w => w.fld_JamOT),
                                        fld_Desc = lg.FirstOrDefault().fldDesc,
                                        fld_KadarByr = lg.FirstOrDefault().fld_Kadar,
                                        fld_Gandaan = lg.FirstOrDefault().fldRate,
                                        fld_TotalAmount = lg.Sum(w => w.fld_Jumlah)
                                    });

                            foreach (var ot in workerOTRecordGroupBy)
                            {
                                OTPekerjaCustomModel otPekerjaCustomModel = new OTPekerjaCustomModel();

                                otPekerjaCustomModel.fld_ID = ot.fld_ID;
                                otPekerjaCustomModel.fld_Desc = GlobalResEstate.lblOvertime + ot.fld_Desc;
                                otPekerjaCustomModel.fld_JumlahJamOT = ot.fld_JumlahJamOT;
                                otPekerjaCustomModel.fld_Unit = GlobalResEstate.lblHour;
                                otPekerjaCustomModel.fld_KadarByr = ot.fld_KadarByr;
                                otPekerjaCustomModel.fld_Gandaan = ot.fld_Gandaan;
                                otPekerjaCustomModel.fld_TotalAmount = ot.fld_TotalAmount;

                                otPekerjaCustomModelList.Add(otPekerjaCustomModel);

                                FootNoteCustomModel otFootNoteCustomModel = new FootNoteCustomModel();

                                otFootNoteCustomModel.fld_Desc = GlobalResEstate.lblOvertime + ot.fld_Desc;
                                otFootNoteCustomModel.fld_Bilangan = ot.fld_JumlahJamOT;

                                footNoteCustomModelList.Add(otFootNoteCustomModel);
                            }

                            List<BonusPekerjaCustomModel> bonusPekerjaCustomModelList = new List<BonusPekerjaCustomModel>();

                            var workerBonusRecordGroupBy = dbview.vw_BonusPekerja
                                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => new { x.fld_KodPkt, x.fld_Bonus, x.fld_KodAktvt })
                                .OrderBy(o => o.Key.fld_KodPkt)
                                .ThenBy(t => t.Key.fld_Bonus)
                                .Select(lg =>
                                    new
                                    {
                                        fld_ID = lg.FirstOrDefault().fld_ID,
                                        fld_Desc = lg.FirstOrDefault().fld_Desc,
                                        fld_KodPkt = lg.FirstOrDefault().fld_KodPkt,
                                        fld_BilanganHari = lg.Count(),
                                        fld_Bonus = lg.FirstOrDefault().fld_Bonus,
                                        fld_KadarByr = lg.FirstOrDefault().fld_Kadar,
                                        fld_TotalAmount = lg.Sum(w => w.fld_Jumlah)
                                    });

                            foreach (var ot in workerBonusRecordGroupBy)
                            {
                                BonusPekerjaCustomModel bonusPekerjaCustomModel = new BonusPekerjaCustomModel();

                                bonusPekerjaCustomModel.fld_ID = ot.fld_ID;
                                bonusPekerjaCustomModel.fld_Desc = ot.fld_Desc;
                                bonusPekerjaCustomModel.fld_BilanganHari = ot.fld_BilanganHari;
                                bonusPekerjaCustomModel.fld_KodPkt = ot.fld_KodPkt;
                                bonusPekerjaCustomModel.fld_Bonus = ot.fld_Bonus;
                                bonusPekerjaCustomModel.fld_KadarByr = ot.fld_KadarByr;
                                bonusPekerjaCustomModel.fld_TotalAmount = ot.fld_TotalAmount;

                                bonusPekerjaCustomModelList.Add(bonusPekerjaCustomModel);
                            }

                            List<CutiPekerjaCustomModel> cutiPekerjaCustomModelList = new List<CutiPekerjaCustomModel>();

                            var workerLeaveRecordGroupBy = dbview.vw_CutiPekerja
                                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => new { x.fld_Kdhdct })
                                .OrderBy(o => o.Key.fld_Kdhdct)
                                .Select(lg =>
                                    new
                                    {
                                        fld_ID = lg.FirstOrDefault().fld_ID,
                                        fld_Desc = lg.FirstOrDefault().fldOptConfDesc,
                                        fld_BilanganHari = lg.Count(),
                                        fld_KadarByr = lg.FirstOrDefault().fld_Kadar,
                                        fld_TotalAmount = lg.Sum(w => w.fld_Jumlah)
                                    });

                            foreach (var ot in workerLeaveRecordGroupBy)
                            {
                                CutiPekerjaCustomModel cutiPekerjaCustomModel = new CutiPekerjaCustomModel();

                                cutiPekerjaCustomModel.fld_ID = ot.fld_ID;
                                cutiPekerjaCustomModel.fld_Desc = ot.fld_Desc;
                                cutiPekerjaCustomModel.fld_BilanganHari = ot.fld_BilanganHari;
                                cutiPekerjaCustomModel.fld_KadarByr = ot.fld_KadarByr;
                                cutiPekerjaCustomModel.fld_TotalAmount = ot.fld_TotalAmount;

                                cutiPekerjaCustomModelList.Add(cutiPekerjaCustomModel);
                            }

                            var workerWorkingDay = dbview.vw_KehadiranPekerja
                                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => new { x.fld_Kdhdct })
                                .OrderBy(o => o.Key.fld_Kdhdct)
                                .Select(lg =>
                                    new
                                    {
                                        fld_Desc = lg.FirstOrDefault().fldOptConfDesc,
                                        fld_Bilangan = lg.Count(),
                                    });

                            foreach (var workingDay in workerWorkingDay)
                            {
                                FootNoteCustomModel footNoteCustomModel = new FootNoteCustomModel();

                                footNoteCustomModel.fld_Desc = workingDay.fld_Desc;
                                footNoteCustomModel.fld_Bilangan = workingDay.fld_Bilangan;

                                footNoteCustomModelList.Add(footNoteCustomModel);
                            }

                            var workerRainDay = dbview.vw_KehadiranPekerja
                                .Count(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList && x.fld_Hujan == 1);

                            if (workerRainDay != 0)
                            {
                                FootNoteCustomModel footNoteHariHujanCustomModel = new FootNoteCustomModel();

                                footNoteHariHujanCustomModel.fld_Desc = GlobalResEstate.lblTotalRainDay;
                                footNoteHariHujanCustomModel.fld_Bilangan = workerRainDay;

                                footNoteCustomModelList.Add(footNoteHariHujanCustomModel);
                            }

                            PaySlipPekerja.Add(
                                new vw_PaySlipPekerja()
                                {
                                    Pkjmast = i,
                                    GajiBulanan = workerMonthlySalary,
                                    InsentifPekerja = workerIncentiveRecordList,
                                    KerjaPekerja = kerjaPekerjaCustomModelList,
                                    OTPekerja = otPekerjaCustomModelList,
                                    BonusPekerja = bonusPekerjaCustomModelList,
                                    CutiPekerja = cutiPekerjaCustomModelList,
                                    FootNote = footNoteCustomModelList,
                                    CarumanTambahan = carumanTambahanCustomModelList
                                });
                        }

                        if (PaySlipPekerja.Count == 0)
                        {
                            ViewBag.Message = GlobalResEstate.msgNoRecord;
                        }

                        return View(PaySlipPekerja);
                    }

                    else
                    {
                        var workerDataSingle = new ViewingModels.vw_GajiPekerja();

                        if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                        {
                            workerDataSingle = dbview.vw_GajiPekerja
                                .Where(x => x.fld_Nopkj == SelectionList && x.fld_Kdaktf == StatusList &&
                                            x.fld_Year == YearList && x.fld_Month == MonthList &&
                                            x.fld_Ktgpkj == WorkCategoryList && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama)
                                .SingleOrDefault();
                        }

                        else
                        {
                            workerDataSingle = dbview.vw_GajiPekerja
                                .Where(x => x.fld_Nopkj == SelectionList && x.fld_NegaraID == NegaraID &&
                                            x.fld_Year == YearList && x.fld_Month == MonthList &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama)
                                .SingleOrDefault();
                        }

                        if (workerDataSingle != null)
                        {
                            List<ViewingModels.vw_MaklumatInsentif> workerIncentiveRecordList = new List<ViewingModels.vw_MaklumatInsentif>();

                            List<FootNoteCustomModel> footNoteCustomModelList = new List<FootNoteCustomModel>();

                            var workerMonthlySalary = dbview.tbl_GajiBulanan
                                .SingleOrDefault(x => x.fld_Nopkj == SelectionList && x.fld_Month == MonthList &&
                                                      x.fld_Year == YearList && x.fld_NegaraID == NegaraID &&
                                                      x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                                      x.fld_LadangID == LadangID);

                            List<CarumanTambahanCustomModel> carumanTambahanCustomModelList = new List<CarumanTambahanCustomModel>();

                            var workerAdditionalContribution = dbr.tbl_ByrCarumanTambahan
                                .Where(x => x.fld_GajiID == workerDataSingle.fld_ID && x.fld_Month == MonthList &&
                                            x.fld_Year == YearList && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                            x.fld_LadangID == LadangID);

                            foreach (var caruman in workerAdditionalContribution)
                            {
                                CarumanTambahanCustomModel carumanTambahanCustomModel = new CarumanTambahanCustomModel();

                                carumanTambahanCustomModel.fld_ID = caruman.fld_ID;
                                carumanTambahanCustomModel.fld_KodCarumanTambahan = caruman.fld_KodSubCaruman;
                                carumanTambahanCustomModel.fld_CarumanMajikan = caruman.fld_CarumanMajikan;
                                carumanTambahanCustomModel.fld_CarumanPekerja = caruman.fld_CarumanPekerja;

                                carumanTambahanCustomModelList.Add(carumanTambahanCustomModel);
                            }

                            var workerIncentiveRecord = dbview.vw_MaklumatInsentif
                                .Where(x => x.fld_Nopkj == SelectionList && x.fld_Month == MonthList &&
                                            x.fld_Year == YearList && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                            x.fld_LadangID == LadangID && x.fld_Deleted == false);

                            foreach (var insentifRecord in workerIncentiveRecord)
                            {
                                workerIncentiveRecordList.Add(insentifRecord);
                            }

                            List<KerjaPekerjaCustomModel> kerjaPekerjaCustomModelList = new List<KerjaPekerjaCustomModel>();

                            var workerWorkRecordGroupBy = dbview.vw_KerjaPekerja
                                .Where(x => x.fld_Nopkj == SelectionList && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => new { x.fld_KodAktvt, x.fld_KodPkt, x.fld_Kdhdct })
                                .OrderBy(o => o.Key.fld_KodAktvt)
                                .ThenBy(t => t.Key.fld_KodPkt)
                                .ThenBy(t2 => t2.Key.fld_Kdhdct)
                                .Select(lg =>
                                    new
                                    {
                                        fld_ID = lg.FirstOrDefault().fld_ID,
                                        fld_Desc = lg.FirstOrDefault().fld_Desc,
                                        fld_KodPkt = lg.FirstOrDefault().fld_KodPkt,
                                        fld_JumlahHasil = lg.Sum(w => w.fld_JumlahHasil),
                                        fld_Unit = lg.FirstOrDefault().fld_Unit,
                                        fld_KadarByr = lg.FirstOrDefault().fld_KadarByr,
                                        fld_Gandaan = lg.FirstOrDefault().fldOptConfFlag3,
                                        fld_TotalAmount = lg.Sum(w => w.fld_Amount)
                                    });

                            foreach (var work in workerWorkRecordGroupBy)
                            {
                                KerjaPekerjaCustomModel kerjaPekerjaCustomModel = new KerjaPekerjaCustomModel();

                                kerjaPekerjaCustomModel.fld_ID = work.fld_ID;
                                kerjaPekerjaCustomModel.fld_Desc = work.fld_Desc;
                                kerjaPekerjaCustomModel.fld_KodPkt = work.fld_KodPkt;
                                kerjaPekerjaCustomModel.fld_JumlahHasil = work.fld_JumlahHasil;
                                kerjaPekerjaCustomModel.fld_Unit = work.fld_Unit;
                                kerjaPekerjaCustomModel.fld_KadarByr = work.fld_KadarByr;
                                kerjaPekerjaCustomModel.fld_Gandaan = work.fld_Gandaan;
                                kerjaPekerjaCustomModel.fld_TotalAmount = work.fld_TotalAmount;

                                kerjaPekerjaCustomModelList.Add(kerjaPekerjaCustomModel);
                            }

                            List<OTPekerjaCustomModel> otPekerjaCustomModelList = new List<OTPekerjaCustomModel>();

                            var workerOTRecordGroupBy = dbview.vw_OTPekerja
                                .Where(x => x.fld_Nopkj == SelectionList && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => x.fld_Kdhdct)
                                .OrderBy(o => o.Key)
                                .Select(lg =>
                                    new
                                    {
                                        fld_ID = lg.FirstOrDefault().fld_ID,
                                        fld_JumlahJamOT = lg.Sum(w => w.fld_JamOT),
                                        fld_Desc = lg.FirstOrDefault().fldDesc,
                                        fld_KadarByr = lg.FirstOrDefault().fld_Kadar,
                                        fld_Gandaan = lg.FirstOrDefault().fldRate,
                                        fld_TotalAmount = lg.Sum(w => w.fld_Jumlah)
                                    });

                            foreach (var ot in workerOTRecordGroupBy)
                            {
                                OTPekerjaCustomModel otPekerjaCustomModel = new OTPekerjaCustomModel();

                                otPekerjaCustomModel.fld_ID = ot.fld_ID;
                                otPekerjaCustomModel.fld_Desc = GlobalResEstate.lblOvertime + ot.fld_Desc;
                                otPekerjaCustomModel.fld_JumlahJamOT = ot.fld_JumlahJamOT;
                                otPekerjaCustomModel.fld_Unit = GlobalResEstate.lblHour;
                                otPekerjaCustomModel.fld_KadarByr = ot.fld_KadarByr;
                                otPekerjaCustomModel.fld_Gandaan = ot.fld_Gandaan;
                                otPekerjaCustomModel.fld_TotalAmount = ot.fld_TotalAmount;

                                otPekerjaCustomModelList.Add(otPekerjaCustomModel);

                                FootNoteCustomModel otFootNoteCustomModel = new FootNoteCustomModel();

                                otFootNoteCustomModel.fld_Desc = GlobalResEstate.lblOvertime + " " + ot.fld_Desc;
                                otFootNoteCustomModel.fld_Bilangan = ot.fld_JumlahJamOT;

                                footNoteCustomModelList.Add(otFootNoteCustomModel);
                            }

                            List<BonusPekerjaCustomModel> bonusPekerjaCustomModelList = new List<BonusPekerjaCustomModel>();

                            var workerBonusRecordGroupBy = dbview.vw_BonusPekerja
                                .Where(x => x.fld_Nopkj == SelectionList && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => new { x.fld_KodPkt, x.fld_Bonus, x.fld_KodAktvt })
                                .OrderBy(o => o.Key.fld_KodPkt)
                                .ThenBy(t => t.Key.fld_Bonus)
                                .Select(lg =>
                                    new
                                    {
                                        fld_ID = lg.FirstOrDefault().fld_ID,
                                        fld_Desc = lg.FirstOrDefault().fld_Desc,
                                        fld_KodPkt = lg.FirstOrDefault().fld_KodPkt,
                                        fld_BilanganHari = lg.Count(),
                                        fld_Bonus = lg.FirstOrDefault().fld_Bonus,
                                        fld_KadarByr = lg.FirstOrDefault().fld_Kadar,
                                        fld_TotalAmount = lg.Sum(w => w.fld_Jumlah)
                                    });

                            foreach (var ot in workerBonusRecordGroupBy)
                            {
                                BonusPekerjaCustomModel bonusPekerjaCustomModel = new BonusPekerjaCustomModel();

                                bonusPekerjaCustomModel.fld_ID = ot.fld_ID;
                                bonusPekerjaCustomModel.fld_Desc = ot.fld_Desc;
                                bonusPekerjaCustomModel.fld_BilanganHari = ot.fld_BilanganHari;
                                bonusPekerjaCustomModel.fld_KodPkt = ot.fld_KodPkt;
                                bonusPekerjaCustomModel.fld_Bonus = ot.fld_Bonus;
                                bonusPekerjaCustomModel.fld_KadarByr = ot.fld_KadarByr;
                                bonusPekerjaCustomModel.fld_TotalAmount = ot.fld_TotalAmount;

                                bonusPekerjaCustomModelList.Add(bonusPekerjaCustomModel);
                            }

                            List<CutiPekerjaCustomModel> cutiPekerjaCustomModelList = new List<CutiPekerjaCustomModel>();

                            var workerLeaveRecordGroupBy = dbview.vw_CutiPekerja
                                .Where(x => x.fld_Nopkj == SelectionList && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => new { x.fld_Kdhdct })
                                .OrderBy(o => o.Key.fld_Kdhdct)
                                .Select(lg =>
                                    new
                                    {
                                        fld_ID = lg.FirstOrDefault().fld_ID,
                                        fld_Desc = lg.FirstOrDefault().fldOptConfDesc,
                                        fld_BilanganHari = lg.Count(),
                                        fld_KadarByr = lg.FirstOrDefault().fld_Kadar,
                                        fld_TotalAmount = lg.Sum(w => w.fld_Jumlah)
                                    });

                            foreach (var ot in workerLeaveRecordGroupBy)
                            {
                                CutiPekerjaCustomModel cutiPekerjaCustomModel = new CutiPekerjaCustomModel();

                                cutiPekerjaCustomModel.fld_ID = ot.fld_ID;
                                cutiPekerjaCustomModel.fld_Desc = ot.fld_Desc;
                                cutiPekerjaCustomModel.fld_BilanganHari = ot.fld_BilanganHari;
                                cutiPekerjaCustomModel.fld_KadarByr = ot.fld_KadarByr;
                                cutiPekerjaCustomModel.fld_TotalAmount = ot.fld_TotalAmount;

                                cutiPekerjaCustomModelList.Add(cutiPekerjaCustomModel);
                            }


                            var workerWorkingDay = dbview.vw_KehadiranPekerja
                                .Where(x => x.fld_Nopkj == SelectionList && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => new { x.fld_Kdhdct })
                                .OrderBy(o => o.Key.fld_Kdhdct)
                                .Select(lg =>
                                    new
                                    {
                                        fld_Desc = lg.FirstOrDefault().fldOptConfDesc,
                                        fld_Bilangan = lg.Count(),
                                    });

                            foreach (var workingDay in workerWorkingDay)
                            {
                                FootNoteCustomModel footNoteCustomModel = new FootNoteCustomModel();

                                footNoteCustomModel.fld_Desc = workingDay.fld_Desc;
                                footNoteCustomModel.fld_Bilangan = workingDay.fld_Bilangan;

                                footNoteCustomModelList.Add(footNoteCustomModel);
                            }

                            var workerRainDay = dbview.vw_KehadiranPekerja
                                .Count(x => x.fld_Nopkj == SelectionList && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList && x.fld_Hujan == 1);

                            if (workerRainDay != 0)
                            {
                                FootNoteCustomModel footNoteHariHujanCustomModel = new FootNoteCustomModel();

                                footNoteHariHujanCustomModel.fld_Desc = GlobalResEstate.lblTotalRainDay;
                                footNoteHariHujanCustomModel.fld_Bilangan = workerRainDay;

                                footNoteCustomModelList.Add(footNoteHariHujanCustomModel);
                            }

                            PaySlipPekerja.Add(
                                new vw_PaySlipPekerja()
                                {
                                    Pkjmast = workerDataSingle,
                                    GajiBulanan = workerMonthlySalary,
                                    InsentifPekerja = workerIncentiveRecordList,
                                    KerjaPekerja = kerjaPekerjaCustomModelList,
                                    OTPekerja = otPekerjaCustomModelList,
                                    BonusPekerja = bonusPekerjaCustomModelList,
                                    CutiPekerja = cutiPekerjaCustomModelList,
                                    FootNote = footNoteCustomModelList,
                                    CarumanTambahan = carumanTambahanCustomModelList
                                });
                        }
                    }

                    if (PaySlipPekerja.Count == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }

                    return View(PaySlipPekerja);
                }

                else
                {
                    if (SelectionList == "0")
                    {
                        IOrderedQueryable<ViewingModels.vw_GajiPekerja> workerData;

                        if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                        {
                            workerData = dbview.vw_GajiPekerja
                                .Where(x => x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList &&
                                            x.fld_Year == YearList && x.fld_Month == MonthList &&
                                            x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        else
                        {
                            workerData = dbview.vw_GajiPekerja
                                .Where(x => x.fld_NegaraID == NegaraID &&
                                            x.fld_Year == YearList && x.fld_Month == MonthList &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        foreach (var i in workerData)
                        {
                            List<ViewingModels.vw_MaklumatInsentif> workerIncentiveRecordList = new List<ViewingModels.vw_MaklumatInsentif>();

                            List<FootNoteCustomModel> footNoteCustomModelList = new List<FootNoteCustomModel>();

                            var workerMonthlySalary = dbview.tbl_GajiBulanan
                                .SingleOrDefault(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Month == MonthList &&
                                                      x.fld_Year == YearList && x.fld_NegaraID == NegaraID &&
                                                      x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                                      x.fld_LadangID == LadangID);

                            List<CarumanTambahanCustomModel> carumanTambahanCustomModelList = new List<CarumanTambahanCustomModel>();

                            var workerAdditionalContribution = dbr.tbl_ByrCarumanTambahan
                                .Where(x => x.fld_GajiID == i.fld_ID && x.fld_Month == MonthList &&
                                            x.fld_Year == YearList && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                            x.fld_LadangID == LadangID);

                            foreach (var caruman in workerAdditionalContribution)
                            {
                                CarumanTambahanCustomModel carumanTambahanCustomModel = new CarumanTambahanCustomModel();

                                carumanTambahanCustomModel.fld_ID = caruman.fld_ID;
                                carumanTambahanCustomModel.fld_KodCarumanTambahan = caruman.fld_KodSubCaruman;
                                carumanTambahanCustomModel.fld_CarumanMajikan = caruman.fld_CarumanMajikan;
                                carumanTambahanCustomModel.fld_CarumanPekerja = caruman.fld_CarumanPekerja;

                                carumanTambahanCustomModelList.Add(carumanTambahanCustomModel);
                            }

                            var workerIncentiveRecord = dbview.vw_MaklumatInsentif
                                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Month == MonthList &&
                                            x.fld_Year == YearList && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                            x.fld_LadangID == LadangID && x.fld_Deleted == false);

                            foreach (var insentifRecord in workerIncentiveRecord)
                            {
                                workerIncentiveRecordList.Add(insentifRecord);
                            }

                            List<KerjaPekerjaCustomModel> kerjaPekerjaCustomModelList = new List<KerjaPekerjaCustomModel>();

                            var workerWorkRecordGroupBy = dbview.vw_KerjaPekerja
                                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => new { x.fld_KodAktvt, x.fld_KodPkt, x.fld_Kdhdct })
                                .OrderBy(o => o.Key.fld_KodAktvt)
                                .ThenBy(t => t.Key.fld_KodPkt)
                                .ThenBy(t2 => t2.Key.fld_Kdhdct)
                                .Select(lg =>
                                    new
                                    {
                                        fld_ID = lg.FirstOrDefault().fld_ID,
                                        fld_Desc = lg.FirstOrDefault().fld_Desc,
                                        fld_KodPkt = lg.FirstOrDefault().fld_KodPkt,
                                        fld_JumlahHasil = lg.Sum(w => w.fld_JumlahHasil),
                                        fld_Unit = lg.FirstOrDefault().fld_Unit,
                                        fld_KadarByr = lg.FirstOrDefault().fld_KadarByr,
                                        fld_Gandaan = lg.FirstOrDefault().fldOptConfFlag3,
                                        fld_TotalAmount = lg.Sum(w => w.fld_Amount)
                                    });

                            foreach (var work in workerWorkRecordGroupBy)
                            {
                                KerjaPekerjaCustomModel kerjaPekerjaCustomModel = new KerjaPekerjaCustomModel();

                                kerjaPekerjaCustomModel.fld_ID = work.fld_ID;
                                kerjaPekerjaCustomModel.fld_Desc = work.fld_Desc;
                                kerjaPekerjaCustomModel.fld_KodPkt = work.fld_KodPkt;
                                kerjaPekerjaCustomModel.fld_JumlahHasil = work.fld_JumlahHasil;
                                kerjaPekerjaCustomModel.fld_Unit = work.fld_Unit;
                                kerjaPekerjaCustomModel.fld_KadarByr = work.fld_KadarByr;
                                kerjaPekerjaCustomModel.fld_Gandaan = work.fld_Gandaan;
                                kerjaPekerjaCustomModel.fld_TotalAmount = work.fld_TotalAmount;

                                kerjaPekerjaCustomModelList.Add(kerjaPekerjaCustomModel);
                            }

                            List<OTPekerjaCustomModel> otPekerjaCustomModelList = new List<OTPekerjaCustomModel>();

                            var workerOTRecordGroupBy = dbview.vw_OTPekerja
                                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => x.fld_Kdhdct)
                                .OrderBy(o => o.Key)
                                .Select(lg =>
                                    new
                                    {
                                        fld_ID = lg.FirstOrDefault().fld_ID,
                                        fld_JumlahJamOT = lg.Sum(w => w.fld_JamOT),
                                        fld_Desc = lg.FirstOrDefault().fldDesc,
                                        fld_KadarByr = lg.FirstOrDefault().fld_Kadar,
                                        fld_Gandaan = lg.FirstOrDefault().fldRate,
                                        fld_TotalAmount = lg.Sum(w => w.fld_Jumlah)
                                    });

                            foreach (var ot in workerOTRecordGroupBy)
                            {
                                OTPekerjaCustomModel otPekerjaCustomModel = new OTPekerjaCustomModel();

                                otPekerjaCustomModel.fld_ID = ot.fld_ID;
                                otPekerjaCustomModel.fld_Desc = GlobalResEstate.lblOvertime + ot.fld_Desc;
                                otPekerjaCustomModel.fld_JumlahJamOT = ot.fld_JumlahJamOT;
                                otPekerjaCustomModel.fld_Unit = GlobalResEstate.lblHour;
                                otPekerjaCustomModel.fld_KadarByr = ot.fld_KadarByr;
                                otPekerjaCustomModel.fld_Gandaan = ot.fld_Gandaan;
                                otPekerjaCustomModel.fld_TotalAmount = ot.fld_TotalAmount;

                                otPekerjaCustomModelList.Add(otPekerjaCustomModel);

                                FootNoteCustomModel otFootNoteCustomModel = new FootNoteCustomModel();

                                otFootNoteCustomModel.fld_Desc = GlobalResEstate.lblOvertime + ot.fld_Desc;
                                otFootNoteCustomModel.fld_Bilangan = ot.fld_JumlahJamOT;

                                footNoteCustomModelList.Add(otFootNoteCustomModel);
                            }

                            List<BonusPekerjaCustomModel> bonusPekerjaCustomModelList = new List<BonusPekerjaCustomModel>();

                            var workerBonusRecordGroupBy = dbview.vw_BonusPekerja
                                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => new { x.fld_KodPkt, x.fld_Bonus, x.fld_KodAktvt })
                                .OrderBy(o => o.Key.fld_KodPkt)
                                .ThenBy(t => t.Key.fld_Bonus)
                                .Select(lg =>
                                    new
                                    {
                                        fld_ID = lg.FirstOrDefault().fld_ID,
                                        fld_Desc = lg.FirstOrDefault().fld_Desc,
                                        fld_KodPkt = lg.FirstOrDefault().fld_KodPkt,
                                        fld_BilanganHari = lg.Count(),
                                        fld_Bonus = lg.FirstOrDefault().fld_Bonus,
                                        fld_KadarByr = lg.FirstOrDefault().fld_Kadar,
                                        fld_TotalAmount = lg.Sum(w => w.fld_Jumlah)
                                    });

                            foreach (var ot in workerBonusRecordGroupBy)
                            {
                                BonusPekerjaCustomModel bonusPekerjaCustomModel = new BonusPekerjaCustomModel();

                                bonusPekerjaCustomModel.fld_ID = ot.fld_ID;
                                bonusPekerjaCustomModel.fld_Desc = ot.fld_Desc;
                                bonusPekerjaCustomModel.fld_BilanganHari = ot.fld_BilanganHari;
                                bonusPekerjaCustomModel.fld_KodPkt = ot.fld_KodPkt;
                                bonusPekerjaCustomModel.fld_Bonus = ot.fld_Bonus;
                                bonusPekerjaCustomModel.fld_KadarByr = ot.fld_KadarByr;
                                bonusPekerjaCustomModel.fld_TotalAmount = ot.fld_TotalAmount;

                                bonusPekerjaCustomModelList.Add(bonusPekerjaCustomModel);
                            }

                            List<CutiPekerjaCustomModel> cutiPekerjaCustomModelList = new List<CutiPekerjaCustomModel>();

                            var workerLeaveRecordGroupBy = dbview.vw_CutiPekerja
                                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => new { x.fld_Kdhdct })
                                .OrderBy(o => o.Key.fld_Kdhdct)
                                .Select(lg =>
                                    new
                                    {
                                        fld_ID = lg.FirstOrDefault().fld_ID,
                                        fld_Desc = lg.FirstOrDefault().fldOptConfDesc,
                                        fld_BilanganHari = lg.Count(),
                                        fld_KadarByr = lg.FirstOrDefault().fld_Kadar,
                                        fld_TotalAmount = lg.Sum(w => w.fld_Jumlah)
                                    });

                            foreach (var ot in workerLeaveRecordGroupBy)
                            {
                                CutiPekerjaCustomModel cutiPekerjaCustomModel = new CutiPekerjaCustomModel();

                                cutiPekerjaCustomModel.fld_ID = ot.fld_ID;
                                cutiPekerjaCustomModel.fld_Desc = ot.fld_Desc;
                                cutiPekerjaCustomModel.fld_BilanganHari = ot.fld_BilanganHari;
                                cutiPekerjaCustomModel.fld_KadarByr = ot.fld_KadarByr;
                                cutiPekerjaCustomModel.fld_TotalAmount = ot.fld_TotalAmount;

                                cutiPekerjaCustomModelList.Add(cutiPekerjaCustomModel);
                            }

                            var workerWorkingDay = dbview.vw_KehadiranPekerja
                                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => new { x.fld_Kdhdct })
                                .OrderBy(o => o.Key.fld_Kdhdct)
                                .Select(lg =>
                                    new
                                    {
                                        fld_Desc = lg.FirstOrDefault().fldOptConfDesc,
                                        fld_Bilangan = lg.Count(),
                                    });

                            foreach (var workingDay in workerWorkingDay)
                            {
                                FootNoteCustomModel footNoteCustomModel = new FootNoteCustomModel();

                                footNoteCustomModel.fld_Desc = workingDay.fld_Desc;
                                footNoteCustomModel.fld_Bilangan = workingDay.fld_Bilangan;

                                footNoteCustomModelList.Add(footNoteCustomModel);
                            }

                            var workerRainDay = dbview.vw_KehadiranPekerja
                                .Count(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList && x.fld_Hujan == 1);

                            if (workerRainDay != 0)
                            {
                                FootNoteCustomModel footNoteHariHujanCustomModel = new FootNoteCustomModel();

                                footNoteHariHujanCustomModel.fld_Desc = GlobalResEstate.lblTotalRainDay;
                                footNoteHariHujanCustomModel.fld_Bilangan = workerRainDay;

                                footNoteCustomModelList.Add(footNoteHariHujanCustomModel);
                            }

                            PaySlipPekerja.Add(
                                new vw_PaySlipPekerja()
                                {
                                    Pkjmast = i,
                                    GajiBulanan = workerMonthlySalary,
                                    InsentifPekerja = workerIncentiveRecordList,
                                    KerjaPekerja = kerjaPekerjaCustomModelList,
                                    OTPekerja = otPekerjaCustomModelList,
                                    BonusPekerja = bonusPekerjaCustomModelList,
                                    CutiPekerja = cutiPekerjaCustomModelList,
                                    FootNote = footNoteCustomModelList,
                                    CarumanTambahan = carumanTambahanCustomModelList
                                });
                        }

                        if (PaySlipPekerja.Count == 0)
                        {
                            ViewBag.Message = GlobalResEstate.msgNoRecord;
                        }

                        return View(PaySlipPekerja);
                    }

                    else
                    {
                        var groupData = dbview.tbl_KumpulanKerja
                            .Where(x => x.fld_KodKumpulan == SelectionList && x.fld_NegaraID == NegaraID &&
                                        x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                        x.fld_LadangID == LadangID)
                            .Select(s => s.fld_KumpulanID)
                            .SingleOrDefault();

                        IOrderedQueryable<ViewingModels.vw_GajiPekerja> workerData;

                        if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                        {
                            workerData = dbview.vw_GajiPekerja
                                .Where(x => x.fld_KumpulanID == groupData && x.fld_Kdaktf == StatusList &&
                                            x.fld_Year == YearList && x.fld_Month == MonthList &&
                                            x.fld_Ktgpkj == WorkCategoryList &&
                                            x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        else
                        {
                            workerData = dbview.vw_GajiPekerja
                                .Where(x => x.fld_KumpulanID == groupData && x.fld_NegaraID == NegaraID &&
                                            x.fld_Year == YearList && x.fld_Month == MonthList &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        foreach (var i in workerData)
                        {
                            List<ViewingModels.vw_MaklumatInsentif> workerIncentiveRecordList = new List<ViewingModels.vw_MaklumatInsentif>();

                            List<FootNoteCustomModel> footNoteCustomModelList = new List<FootNoteCustomModel>();

                            var workerMonthlySalary = dbview.tbl_GajiBulanan
                                .SingleOrDefault(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Month == MonthList &&
                                                      x.fld_Year == YearList && x.fld_NegaraID == NegaraID &&
                                                      x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                                      x.fld_LadangID == LadangID);

                            List<CarumanTambahanCustomModel> carumanTambahanCustomModelList = new List<CarumanTambahanCustomModel>();

                            var workerAdditionalContribution = dbr.tbl_ByrCarumanTambahan
                                .Where(x => x.fld_GajiID == i.fld_ID && x.fld_Month == MonthList &&
                                            x.fld_Year == YearList && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                            x.fld_LadangID == LadangID);

                            foreach (var caruman in workerAdditionalContribution)
                            {
                                CarumanTambahanCustomModel carumanTambahanCustomModel = new CarumanTambahanCustomModel();

                                carumanTambahanCustomModel.fld_ID = caruman.fld_ID;
                                carumanTambahanCustomModel.fld_KodCarumanTambahan = caruman.fld_KodSubCaruman;
                                carumanTambahanCustomModel.fld_CarumanMajikan = caruman.fld_CarumanMajikan;
                                carumanTambahanCustomModel.fld_CarumanPekerja = caruman.fld_CarumanPekerja;

                                carumanTambahanCustomModelList.Add(carumanTambahanCustomModel);
                            }

                            var workerIncentiveRecord = dbview.vw_MaklumatInsentif
                                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Month == MonthList &&
                                            x.fld_Year == YearList && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                            x.fld_LadangID == LadangID && x.fld_Deleted == false);

                            foreach (var insentifRecord in workerIncentiveRecord)
                            {
                                workerIncentiveRecordList.Add(insentifRecord);
                            }

                            List<KerjaPekerjaCustomModel> kerjaPekerjaCustomModelList = new List<KerjaPekerjaCustomModel>();

                            var workerWorkRecordGroupBy = dbview.vw_KerjaPekerja
                                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => new { x.fld_KodAktvt, x.fld_KodPkt, x.fld_Kdhdct })
                                .OrderBy(o => o.Key.fld_KodAktvt)
                                .ThenBy(t => t.Key.fld_KodPkt)
                                .ThenBy(t2 => t2.Key.fld_Kdhdct)
                                .Select(lg =>
                                    new
                                    {
                                        fld_ID = lg.FirstOrDefault().fld_ID,
                                        fld_Desc = lg.FirstOrDefault().fld_Desc,
                                        fld_KodPkt = lg.FirstOrDefault().fld_KodPkt,
                                        fld_JumlahHasil = lg.Sum(w => w.fld_JumlahHasil),
                                        fld_Unit = lg.FirstOrDefault().fld_Unit,
                                        fld_KadarByr = lg.FirstOrDefault().fld_KadarByr,
                                        fld_Gandaan = lg.FirstOrDefault().fldOptConfFlag3,
                                        fld_TotalAmount = lg.Sum(w => w.fld_Amount)
                                    });

                            foreach (var work in workerWorkRecordGroupBy)
                            {
                                KerjaPekerjaCustomModel kerjaPekerjaCustomModel = new KerjaPekerjaCustomModel();

                                kerjaPekerjaCustomModel.fld_ID = work.fld_ID;
                                kerjaPekerjaCustomModel.fld_Desc = work.fld_Desc;
                                kerjaPekerjaCustomModel.fld_KodPkt = work.fld_KodPkt;
                                kerjaPekerjaCustomModel.fld_JumlahHasil = work.fld_JumlahHasil;
                                kerjaPekerjaCustomModel.fld_Unit = work.fld_Unit;
                                kerjaPekerjaCustomModel.fld_KadarByr = work.fld_KadarByr;
                                kerjaPekerjaCustomModel.fld_Gandaan = work.fld_Gandaan;
                                kerjaPekerjaCustomModel.fld_TotalAmount = work.fld_TotalAmount;

                                kerjaPekerjaCustomModelList.Add(kerjaPekerjaCustomModel);
                            }

                            List<OTPekerjaCustomModel> otPekerjaCustomModelList = new List<OTPekerjaCustomModel>();

                            var workerOTRecordGroupBy = dbview.vw_OTPekerja
                                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => x.fld_Kdhdct)
                                .OrderBy(o => o.Key)
                                .Select(lg =>
                                    new
                                    {
                                        fld_ID = lg.FirstOrDefault().fld_ID,
                                        fld_JumlahJamOT = lg.Sum(w => w.fld_JamOT),
                                        fld_Desc = lg.FirstOrDefault().fldDesc,
                                        fld_KadarByr = lg.FirstOrDefault().fld_Kadar,
                                        fld_Gandaan = lg.FirstOrDefault().fldRate,
                                        fld_TotalAmount = lg.Sum(w => w.fld_Jumlah)
                                    });

                            foreach (var ot in workerOTRecordGroupBy)
                            {
                                OTPekerjaCustomModel otPekerjaCustomModel = new OTPekerjaCustomModel();

                                otPekerjaCustomModel.fld_ID = ot.fld_ID;
                                otPekerjaCustomModel.fld_Desc = GlobalResEstate.lblOvertime + ot.fld_Desc;
                                otPekerjaCustomModel.fld_JumlahJamOT = ot.fld_JumlahJamOT;
                                otPekerjaCustomModel.fld_Unit = GlobalResEstate.lblHour;
                                otPekerjaCustomModel.fld_KadarByr = ot.fld_KadarByr;
                                otPekerjaCustomModel.fld_Gandaan = ot.fld_Gandaan;
                                otPekerjaCustomModel.fld_TotalAmount = ot.fld_TotalAmount;

                                otPekerjaCustomModelList.Add(otPekerjaCustomModel);

                                FootNoteCustomModel otFootNoteCustomModel = new FootNoteCustomModel();

                                otFootNoteCustomModel.fld_Desc = GlobalResEstate.lblOvertime + " " + ot.fld_Desc;
                                otFootNoteCustomModel.fld_Bilangan = ot.fld_JumlahJamOT;

                                footNoteCustomModelList.Add(otFootNoteCustomModel);
                            }

                            List<BonusPekerjaCustomModel> bonusPekerjaCustomModelList = new List<BonusPekerjaCustomModel>();

                            var workerBonusRecordGroupBy = dbview.vw_BonusPekerja
                                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => new { x.fld_KodPkt, x.fld_Bonus, x.fld_KodAktvt })
                                .OrderBy(o => o.Key.fld_KodPkt)
                                .ThenBy(t => t.Key.fld_Bonus)
                                .Select(lg =>
                                    new
                                    {
                                        fld_ID = lg.FirstOrDefault().fld_ID,
                                        fld_Desc = lg.FirstOrDefault().fld_Desc,
                                        fld_KodPkt = lg.FirstOrDefault().fld_KodPkt,
                                        fld_BilanganHari = lg.Count(),
                                        fld_Bonus = lg.FirstOrDefault().fld_Bonus,
                                        fld_KadarByr = lg.FirstOrDefault().fld_Kadar,
                                        fld_TotalAmount = lg.Sum(w => w.fld_Jumlah)
                                    });

                            foreach (var ot in workerBonusRecordGroupBy)
                            {
                                BonusPekerjaCustomModel bonusPekerjaCustomModel = new BonusPekerjaCustomModel();

                                bonusPekerjaCustomModel.fld_ID = ot.fld_ID;
                                bonusPekerjaCustomModel.fld_Desc = ot.fld_Desc;
                                bonusPekerjaCustomModel.fld_BilanganHari = ot.fld_BilanganHari;
                                bonusPekerjaCustomModel.fld_KodPkt = ot.fld_KodPkt;
                                bonusPekerjaCustomModel.fld_Bonus = ot.fld_Bonus;
                                bonusPekerjaCustomModel.fld_KadarByr = ot.fld_KadarByr;
                                bonusPekerjaCustomModel.fld_TotalAmount = ot.fld_TotalAmount;

                                bonusPekerjaCustomModelList.Add(bonusPekerjaCustomModel);
                            }

                            List<CutiPekerjaCustomModel> cutiPekerjaCustomModelList = new List<CutiPekerjaCustomModel>();

                            var workerLeaveRecordGroupBy = dbview.vw_CutiPekerja
                                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => new { x.fld_Kdhdct })
                                .OrderBy(o => o.Key.fld_Kdhdct)
                                .Select(lg =>
                                    new
                                    {
                                        fld_ID = lg.FirstOrDefault().fld_ID,
                                        fld_Desc = lg.FirstOrDefault().fldOptConfDesc,
                                        fld_BilanganHari = lg.Count(),
                                        fld_KadarByr = lg.FirstOrDefault().fld_Kadar,
                                        fld_TotalAmount = lg.Sum(w => w.fld_Jumlah)
                                    });

                            foreach (var ot in workerLeaveRecordGroupBy)
                            {
                                CutiPekerjaCustomModel cutiPekerjaCustomModel = new CutiPekerjaCustomModel();

                                cutiPekerjaCustomModel.fld_ID = ot.fld_ID;
                                cutiPekerjaCustomModel.fld_Desc = ot.fld_Desc;
                                cutiPekerjaCustomModel.fld_BilanganHari = ot.fld_BilanganHari;
                                cutiPekerjaCustomModel.fld_KadarByr = ot.fld_KadarByr;
                                cutiPekerjaCustomModel.fld_TotalAmount = ot.fld_TotalAmount;

                                cutiPekerjaCustomModelList.Add(cutiPekerjaCustomModel);
                            }

                            var workerWorkingDay = dbview.vw_KehadiranPekerja
                                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => new { x.fld_Kdhdct })
                                .OrderBy(o => o.Key.fld_Kdhdct)
                                .Select(lg =>
                                    new
                                    {
                                        fld_Desc = lg.FirstOrDefault().fldOptConfDesc,
                                        fld_Bilangan = lg.Count(),
                                    });

                            foreach (var workingDay in workerWorkingDay)
                            {
                                FootNoteCustomModel footNoteCustomModel = new FootNoteCustomModel();

                                footNoteCustomModel.fld_Desc = workingDay.fld_Desc;
                                footNoteCustomModel.fld_Bilangan = workingDay.fld_Bilangan;

                                footNoteCustomModelList.Add(footNoteCustomModel);
                            }

                            var workerRainDay = dbview.vw_KehadiranPekerja
                                .Count(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList && x.fld_Hujan == 1);

                            if (workerRainDay != 0)
                            {
                                FootNoteCustomModel footNoteHariHujanCustomModel = new FootNoteCustomModel();

                                footNoteHariHujanCustomModel.fld_Desc = GlobalResEstate.lblTotalRainDay;
                                footNoteHariHujanCustomModel.fld_Bilangan = workerRainDay;

                                footNoteCustomModelList.Add(footNoteHariHujanCustomModel);
                            }

                            PaySlipPekerja.Add(
                                new vw_PaySlipPekerja()
                                {
                                    Pkjmast = i,
                                    GajiBulanan = workerMonthlySalary,
                                    InsentifPekerja = workerIncentiveRecordList,
                                    KerjaPekerja = kerjaPekerjaCustomModelList,
                                    OTPekerja = otPekerjaCustomModelList,
                                    BonusPekerja = bonusPekerjaCustomModelList,
                                    CutiPekerja = cutiPekerjaCustomModelList,
                                    FootNote = footNoteCustomModelList,
                                    CarumanTambahan = carumanTambahanCustomModelList
                                });
                        }
                    }

                    if (PaySlipPekerja.Count == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }

                    return View(PaySlipPekerja);
                }
            }
        }

        public ActionResult PaySheetReport()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int drpyear = 0;
            int drprangeyear = 0;
            int month = timezone.gettimezone().Month;

            List<SelectListItem> SelectionList = new List<SelectListItem>();
            SelectionList = new SelectList(
                dbr.tbl_Pkjmast
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1")
                    .OrderBy(o => o.fld_Nopkj)
                    .Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }),
                "Value", "Text").ToList();
            SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.SelectionList = SelectionList;

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

            var statusList = new List<SelectListItem>();
            statusList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            var monthList = new SelectList(
                db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false &&
                                                   x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID),
                "fldOptConfValue", "fldOptConfDesc", month);


            List<SelectListItem> JnsPkjList = new List<SelectListItem>();

            JnsPkjList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" && 
            x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue)
            .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            JnsPkjList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            //added by faeza 31.05.2021
            List<SelectListItem> PaymentModeList = new List<SelectListItem>();
            PaymentModeList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "paymentmode" &&
            x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue)
            .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            PaymentModeList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));


            ViewBag.MonthList = monthList;
            ViewBag.StatusList = statusList;
            ViewBag.JnsPkjList = JnsPkjList;
            ViewBag.PaymentModeList = PaymentModeList;//added by faeza 31.05.2021

            return View();
        }

        public ActionResult _WorkerPaySheetRptAdvanceSearch()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var statusList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            ViewBag.StatusList = statusList;

            var workCategoryList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "designation" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            ViewBag.WorkCategoryList = workCategoryList;

            return View();
        }

        public ViewResult _WorkerPaySheetRptSearch(int? RadioGroup, int? MonthList, int? YearList,
            string SelectionList, string StatusList, string WorkCategoryList, string JnsPkjList, string PaymentModeList, string print)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            MVC_SYSTEM_Viewing dbview2 = new MVC_SYSTEM_Viewing();
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            List<vw_PaySheetPekerjaCustomModel> PaySheetPekerjaList = new List<vw_PaySheetPekerjaCustomModel>();
            ViewBag.MonthList = MonthList;
            ViewBag.YearList = YearList;
            ViewBag.WorkerList = SelectionList;
            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();
            ViewBag.Ladang = db.tbl_Ladang
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_ID == LadangID)
                .Select(s => s.fld_LdgName)
                .FirstOrDefault();
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.NamaPengurus = dbview2.tbl_Ladang
                .Where(x => x.fld_ID == LadangID)
                .Select(s => s.fld_Pengurus).Single();
            ViewBag.NamaPenyelia = dbview2.tblUsers
                .Where(x => x.fldUserID == getuserid)
                .Select(s => s.fldUserFullName).Single();
            ViewBag.IDPenyelia = getuserid;
            List<SelectListItem> JnsPkjList2 = new List<SelectListItem>();
            JnsPkjList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text", JnsPkjList).ToList();
            JnsPkjList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
            ViewBag.JnsPkjList = JnsPkjList2;

            //added by faeza 31.05.2021
            List<SelectListItem> PaymentModeList2 = new List<SelectListItem>();
            PaymentModeList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "paymentmode" &&
            x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue)
            .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            PaymentModeList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
            ViewBag.PaymentModeSelection = PaymentModeList;

            ViewBag.Print = print;
            if (MonthList == null && YearList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChooseWork;
                return View(PaySheetPekerjaList);
            }
            else
            {
                if (RadioGroup == 0)
                {
                    //individu
                    if (SelectionList == "0")
                    {
                        if (JnsPkjList == "0")
                        {
                            if (PaymentModeList == "0")
                            {
                                IOrderedQueryable<ViewingModels.vw_PaySheetPekerja> salaryData;
                                if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                                {
                                    salaryData = dbview.vw_PaySheetPekerja
                                        .Where(x => x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList &&
                                                    x.fld_Year == YearList && x.fld_Month == MonthList &&
                                                    x.fld_NegaraID == NegaraID &&
                                                    x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID)
                                        .OrderBy(x => x.fld_Nama);
                                }
                                else
                                {
                                    salaryData = dbview.vw_PaySheetPekerja
                                        .Where(x => x.fld_NegaraID == NegaraID &&
                                                    x.fld_Year == YearList && x.fld_Month == MonthList &&
                                                    x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID)
                                        .OrderBy(x => x.fld_Nama);
                                }
                                foreach (var salary in salaryData)
                                {
                                    var workerAdditionalContribution = dbr.tbl_ByrCarumanTambahan
                                        .Where(x => x.fld_GajiID == salary.fld_ID && x.fld_NegaraID == NegaraID &&
                                                    x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                                    x.fld_LadangID == LadangID);
                                    List<CarumanTambahanCustomModel> carumanTambahanCustomModelList = new List<CarumanTambahanCustomModel>();
                                    foreach (var caruman in workerAdditionalContribution)
                                    {
                                        CarumanTambahanCustomModel carumanTambahanCustomModel = new CarumanTambahanCustomModel();
                                        carumanTambahanCustomModel.fld_ID = caruman.fld_ID;
                                        carumanTambahanCustomModel.fld_KodCarumanTambahan = caruman.fld_KodSubCaruman;
                                        carumanTambahanCustomModel.fld_CarumanMajikan = caruman.fld_CarumanMajikan;
                                        carumanTambahanCustomModel.fld_CarumanPekerja = caruman.fld_CarumanPekerja;
                                        carumanTambahanCustomModelList.Add(carumanTambahanCustomModel);
                                    }
                                    PaySheetPekerjaList.Add(
                                        new vw_PaySheetPekerjaCustomModel()
                                        {
                                            PaySheetPekerja = salary,
                                            CarumanTambahan = carumanTambahanCustomModelList
                                        });
                                }
                                if (PaySheetPekerjaList.Count == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }
                                return View(PaySheetPekerjaList);
                            }
                            else
                            {
                                IOrderedQueryable<ViewingModels.vw_PaySheetPekerja> salaryData;
                                if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                                {
                                    salaryData = dbview.vw_PaySheetPekerja
                                        .Where(x => x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList && x.fld_PaymentMode == PaymentModeList &&
                                                    x.fld_Year == YearList && x.fld_Month == MonthList &&
                                                    x.fld_NegaraID == NegaraID &&
                                                    x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID)
                                        .OrderBy(x => x.fld_Nama);
                                }
                                else
                                {
                                    salaryData = dbview.vw_PaySheetPekerja
                                        .Where(x => x.fld_NegaraID == NegaraID &&
                                                    x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_PaymentMode == PaymentModeList &&
                                                    x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID)
                                        .OrderBy(x => x.fld_Nama);
                                }
                                foreach (var salary in salaryData)
                                {
                                    var workerAdditionalContribution = dbr.tbl_ByrCarumanTambahan
                                        .Where(x => x.fld_GajiID == salary.fld_ID && x.fld_NegaraID == NegaraID &&
                                                    x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                                    x.fld_LadangID == LadangID);
                                    List<CarumanTambahanCustomModel> carumanTambahanCustomModelList = new List<CarumanTambahanCustomModel>();
                                    foreach (var caruman in workerAdditionalContribution)
                                    {
                                        CarumanTambahanCustomModel carumanTambahanCustomModel = new CarumanTambahanCustomModel();
                                        carumanTambahanCustomModel.fld_ID = caruman.fld_ID;
                                        carumanTambahanCustomModel.fld_KodCarumanTambahan = caruman.fld_KodSubCaruman;
                                        carumanTambahanCustomModel.fld_CarumanMajikan = caruman.fld_CarumanMajikan;
                                        carumanTambahanCustomModel.fld_CarumanPekerja = caruman.fld_CarumanPekerja;
                                        carumanTambahanCustomModelList.Add(carumanTambahanCustomModel);
                                    }
                                    PaySheetPekerjaList.Add(
                                        new vw_PaySheetPekerjaCustomModel()
                                        {
                                            PaySheetPekerja = salary,
                                            CarumanTambahan = carumanTambahanCustomModelList
                                        });
                                }
                                if (PaySheetPekerjaList.Count == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }
                                return View(PaySheetPekerjaList);
                            }
                        }
                        else
                        {
                            if (PaymentModeList == "0")
                            {
                                IOrderedQueryable<ViewingModels.vw_PaySheetPekerja> salaryData;
                                if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                                {
                                    salaryData = dbview.vw_PaySheetPekerja
                                        .Where(x => x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList && x.fld_Jenispekerja == JnsPkjList &&
                                                    x.fld_Year == YearList && x.fld_Month == MonthList &&
                                                    x.fld_NegaraID == NegaraID &&
                                                    x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID)
                                        .OrderBy(x => x.fld_Nama);
                                }
                                else
                                {
                                    salaryData = dbview.vw_PaySheetPekerja
                                        .Where(x => x.fld_NegaraID == NegaraID &&
                                                    x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_Jenispekerja == JnsPkjList &&
                                                    x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID)
                                        .OrderBy(x => x.fld_Nama);
                                }
                                foreach (var salary in salaryData)
                                {
                                    var workerAdditionalContribution = dbr.tbl_ByrCarumanTambahan
                                        .Where(x => x.fld_GajiID == salary.fld_ID && x.fld_NegaraID == NegaraID &&
                                                    x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                                    x.fld_LadangID == LadangID);
                                    List<CarumanTambahanCustomModel> carumanTambahanCustomModelList = new List<CarumanTambahanCustomModel>();
                                    foreach (var caruman in workerAdditionalContribution)
                                    {
                                        CarumanTambahanCustomModel carumanTambahanCustomModel = new CarumanTambahanCustomModel();
                                        carumanTambahanCustomModel.fld_ID = caruman.fld_ID;
                                        carumanTambahanCustomModel.fld_KodCarumanTambahan = caruman.fld_KodSubCaruman;
                                        carumanTambahanCustomModel.fld_CarumanMajikan = caruman.fld_CarumanMajikan;
                                        carumanTambahanCustomModel.fld_CarumanPekerja = caruman.fld_CarumanPekerja;
                                        carumanTambahanCustomModelList.Add(carumanTambahanCustomModel);
                                    }
                                    PaySheetPekerjaList.Add(
                                        new vw_PaySheetPekerjaCustomModel()
                                        {
                                            PaySheetPekerja = salary,
                                            CarumanTambahan = carumanTambahanCustomModelList
                                        });
                                }
                                if (PaySheetPekerjaList.Count == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }
                                return View(PaySheetPekerjaList);
                            }
                            else
                            {
                                IOrderedQueryable<ViewingModels.vw_PaySheetPekerja> salaryData;
                                if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                                {
                                    salaryData = dbview.vw_PaySheetPekerja
                                        .Where(x => x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList && x.fld_Jenispekerja == JnsPkjList && x.fld_PaymentMode == PaymentModeList &&
                                                    x.fld_Year == YearList && x.fld_Month == MonthList &&
                                                    x.fld_NegaraID == NegaraID &&
                                                    x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID)
                                        .OrderBy(x => x.fld_Nama);
                                }
                                else
                                {
                                    salaryData = dbview.vw_PaySheetPekerja
                                        .Where(x => x.fld_NegaraID == NegaraID &&
                                                    x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_Jenispekerja == JnsPkjList && x.fld_PaymentMode == PaymentModeList &&
                                                    x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID)
                                        .OrderBy(x => x.fld_Nama);
                                }
                                foreach (var salary in salaryData)
                                {
                                    var workerAdditionalContribution = dbr.tbl_ByrCarumanTambahan
                                        .Where(x => x.fld_GajiID == salary.fld_ID && x.fld_NegaraID == NegaraID &&
                                                    x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                                    x.fld_LadangID == LadangID);
                                    List<CarumanTambahanCustomModel> carumanTambahanCustomModelList = new List<CarumanTambahanCustomModel>();
                                    foreach (var caruman in workerAdditionalContribution)
                                    {
                                        CarumanTambahanCustomModel carumanTambahanCustomModel = new CarumanTambahanCustomModel();
                                        carumanTambahanCustomModel.fld_ID = caruman.fld_ID;
                                        carumanTambahanCustomModel.fld_KodCarumanTambahan = caruman.fld_KodSubCaruman;
                                        carumanTambahanCustomModel.fld_CarumanMajikan = caruman.fld_CarumanMajikan;
                                        carumanTambahanCustomModel.fld_CarumanPekerja = caruman.fld_CarumanPekerja;
                                        carumanTambahanCustomModelList.Add(carumanTambahanCustomModel);
                                    }
                                    PaySheetPekerjaList.Add(
                                        new vw_PaySheetPekerjaCustomModel()
                                        {
                                            PaySheetPekerja = salary,
                                            CarumanTambahan = carumanTambahanCustomModelList
                                        });
                                }
                                if (PaySheetPekerjaList.Count == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }
                                return View(PaySheetPekerjaList);
                            }
                        }

                    }
                    else
                    {
                        if (JnsPkjList == "0")
                        {
                            if (PaymentModeList == "0")
                            {
                                var salaryDataSingle = new ViewingModels.vw_PaySheetPekerja();
                                if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                                {
                                    salaryDataSingle = dbview.vw_PaySheetPekerja
                                        .Where(x => x.fld_Nopkj == SelectionList && x.fld_Kdaktf == StatusList &&
                                                    x.fld_Year == YearList && x.fld_Month == MonthList &&
                                                    x.fld_Ktgpkj == WorkCategoryList && x.fld_NegaraID == NegaraID &&
                                                    x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID)
                                        .OrderBy(x => x.fld_Nama)
                                        .SingleOrDefault();
                                }
                                else
                                {
                                    salaryDataSingle = dbview.vw_PaySheetPekerja
                                        .Where(x => x.fld_Nopkj == SelectionList && x.fld_NegaraID == NegaraID &&
                                                    x.fld_Year == YearList && x.fld_Month == MonthList &&
                                                    x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID)
                                        .OrderBy(x => x.fld_Nama)
                                        .SingleOrDefault();
                                }
                                if (salaryDataSingle != null)
                                {
                                    var workerAdditionalContribution = dbr.tbl_ByrCarumanTambahan
                                        .Where(x => x.fld_GajiID == salaryDataSingle.fld_ID && x.fld_NegaraID == NegaraID &&
                                                    x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                                    x.fld_LadangID == LadangID);
                                    List<CarumanTambahanCustomModel> carumanTambahanCustomModelList = new List<CarumanTambahanCustomModel>();
                                    foreach (var caruman in workerAdditionalContribution)
                                    {
                                        CarumanTambahanCustomModel carumanTambahanCustomModel = new CarumanTambahanCustomModel();
                                        carumanTambahanCustomModel.fld_ID = caruman.fld_ID;
                                        carumanTambahanCustomModel.fld_KodCarumanTambahan = caruman.fld_KodSubCaruman;
                                        carumanTambahanCustomModel.fld_CarumanMajikan = caruman.fld_CarumanMajikan;
                                        carumanTambahanCustomModel.fld_CarumanPekerja = caruman.fld_CarumanPekerja;
                                        carumanTambahanCustomModelList.Add(carumanTambahanCustomModel);
                                    }
                                    PaySheetPekerjaList.Add(
                                        new vw_PaySheetPekerjaCustomModel()
                                        {
                                            PaySheetPekerja = salaryDataSingle,
                                            CarumanTambahan = carumanTambahanCustomModelList
                                        });
                                }
                                if (PaySheetPekerjaList.Count == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }
                                return View(PaySheetPekerjaList);
                            }
                            else
                            {
                                var salaryDataSingle = new ViewingModels.vw_PaySheetPekerja();
                                if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                                {
                                    salaryDataSingle = dbview.vw_PaySheetPekerja
                                        .Where(x => x.fld_Nopkj == SelectionList && x.fld_Kdaktf == StatusList && x.fld_PaymentMode == PaymentModeList &&
                                                    x.fld_Year == YearList && x.fld_Month == MonthList &&
                                                    x.fld_Ktgpkj == WorkCategoryList && x.fld_NegaraID == NegaraID &&
                                                    x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID)
                                        .OrderBy(x => x.fld_Nama)
                                        .SingleOrDefault();
                                }
                                else
                                {
                                    salaryDataSingle = dbview.vw_PaySheetPekerja
                                        .Where(x => x.fld_Nopkj == SelectionList && x.fld_NegaraID == NegaraID && x.fld_PaymentMode == PaymentModeList &&
                                                    x.fld_Year == YearList && x.fld_Month == MonthList &&
                                                    x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID)
                                        .OrderBy(x => x.fld_Nama)
                                        .SingleOrDefault();
                                }
                                if (salaryDataSingle != null)
                                {
                                    var workerAdditionalContribution = dbr.tbl_ByrCarumanTambahan
                                        .Where(x => x.fld_GajiID == salaryDataSingle.fld_ID && x.fld_NegaraID == NegaraID &&
                                                    x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                                    x.fld_LadangID == LadangID);
                                    List<CarumanTambahanCustomModel> carumanTambahanCustomModelList = new List<CarumanTambahanCustomModel>();
                                    foreach (var caruman in workerAdditionalContribution)
                                    {
                                        CarumanTambahanCustomModel carumanTambahanCustomModel = new CarumanTambahanCustomModel();
                                        carumanTambahanCustomModel.fld_ID = caruman.fld_ID;
                                        carumanTambahanCustomModel.fld_KodCarumanTambahan = caruman.fld_KodSubCaruman;
                                        carumanTambahanCustomModel.fld_CarumanMajikan = caruman.fld_CarumanMajikan;
                                        carumanTambahanCustomModel.fld_CarumanPekerja = caruman.fld_CarumanPekerja;
                                        carumanTambahanCustomModelList.Add(carumanTambahanCustomModel);
                                    }
                                    PaySheetPekerjaList.Add(
                                        new vw_PaySheetPekerjaCustomModel()
                                        {
                                            PaySheetPekerja = salaryDataSingle,
                                            CarumanTambahan = carumanTambahanCustomModelList
                                        });
                                }
                                if (PaySheetPekerjaList.Count == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }
                                return View(PaySheetPekerjaList);
                            }
                        }
                        else
                        {
                            if (PaymentModeList == "0")
                            {
                                var salaryDataSingle = new ViewingModels.vw_PaySheetPekerja();
                                if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                                {
                                    salaryDataSingle = dbview.vw_PaySheetPekerja
                                        .Where(x => x.fld_Nopkj == SelectionList && x.fld_Kdaktf == StatusList && x.fld_Jenispekerja == JnsPkjList &&
                                                    x.fld_Year == YearList && x.fld_Month == MonthList &&
                                                    x.fld_Ktgpkj == WorkCategoryList && x.fld_NegaraID == NegaraID &&
                                                    x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID)
                                        .OrderBy(x => x.fld_Nama)
                                        .SingleOrDefault();
                                }
                                else
                                {
                                    salaryDataSingle = dbview.vw_PaySheetPekerja
                                        .Where(x => x.fld_Nopkj == SelectionList && x.fld_NegaraID == NegaraID && x.fld_Jenispekerja == JnsPkjList &&
                                                    x.fld_Year == YearList && x.fld_Month == MonthList &&
                                                    x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID)
                                        .OrderBy(x => x.fld_Nama)
                                        .SingleOrDefault();
                                }
                                if (salaryDataSingle != null)
                                {
                                    var workerAdditionalContribution = dbr.tbl_ByrCarumanTambahan
                                        .Where(x => x.fld_GajiID == salaryDataSingle.fld_ID && x.fld_NegaraID == NegaraID &&
                                                    x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                                    x.fld_LadangID == LadangID);
                                    List<CarumanTambahanCustomModel> carumanTambahanCustomModelList = new List<CarumanTambahanCustomModel>();
                                    foreach (var caruman in workerAdditionalContribution)
                                    {
                                        CarumanTambahanCustomModel carumanTambahanCustomModel = new CarumanTambahanCustomModel();
                                        carumanTambahanCustomModel.fld_ID = caruman.fld_ID;
                                        carumanTambahanCustomModel.fld_KodCarumanTambahan = caruman.fld_KodSubCaruman;
                                        carumanTambahanCustomModel.fld_CarumanMajikan = caruman.fld_CarumanMajikan;
                                        carumanTambahanCustomModel.fld_CarumanPekerja = caruman.fld_CarumanPekerja;
                                        carumanTambahanCustomModelList.Add(carumanTambahanCustomModel);
                                    }
                                    PaySheetPekerjaList.Add(
                                        new vw_PaySheetPekerjaCustomModel()
                                        {
                                            PaySheetPekerja = salaryDataSingle,
                                            CarumanTambahan = carumanTambahanCustomModelList
                                        });
                                }
                                if (PaySheetPekerjaList.Count == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }
                                return View(PaySheetPekerjaList);
                            }
                            else
                            {
                                var salaryDataSingle = new ViewingModels.vw_PaySheetPekerja();
                                if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                                {
                                    salaryDataSingle = dbview.vw_PaySheetPekerja
                                        .Where(x => x.fld_Nopkj == SelectionList && x.fld_Kdaktf == StatusList && x.fld_Jenispekerja == JnsPkjList && x.fld_PaymentMode == PaymentModeList &&
                                                    x.fld_Year == YearList && x.fld_Month == MonthList &&
                                                    x.fld_Ktgpkj == WorkCategoryList && x.fld_NegaraID == NegaraID &&
                                                    x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID)
                                        .OrderBy(x => x.fld_Nama)
                                        .SingleOrDefault();
                                }
                                else
                                {
                                    salaryDataSingle = dbview.vw_PaySheetPekerja
                                        .Where(x => x.fld_Nopkj == SelectionList && x.fld_NegaraID == NegaraID && x.fld_Jenispekerja == JnsPkjList && x.fld_PaymentMode == PaymentModeList &&
                                                    x.fld_Year == YearList && x.fld_Month == MonthList &&
                                                    x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID)
                                        .OrderBy(x => x.fld_Nama)
                                        .SingleOrDefault();
                                }
                                if (salaryDataSingle != null)
                                {
                                    var workerAdditionalContribution = dbr.tbl_ByrCarumanTambahan
                                        .Where(x => x.fld_GajiID == salaryDataSingle.fld_ID && x.fld_NegaraID == NegaraID &&
                                                    x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                                    x.fld_LadangID == LadangID);
                                    List<CarumanTambahanCustomModel> carumanTambahanCustomModelList = new List<CarumanTambahanCustomModel>();
                                    foreach (var caruman in workerAdditionalContribution)
                                    {
                                        CarumanTambahanCustomModel carumanTambahanCustomModel = new CarumanTambahanCustomModel();
                                        carumanTambahanCustomModel.fld_ID = caruman.fld_ID;
                                        carumanTambahanCustomModel.fld_KodCarumanTambahan = caruman.fld_KodSubCaruman;
                                        carumanTambahanCustomModel.fld_CarumanMajikan = caruman.fld_CarumanMajikan;
                                        carumanTambahanCustomModel.fld_CarumanPekerja = caruman.fld_CarumanPekerja;
                                        carumanTambahanCustomModelList.Add(carumanTambahanCustomModel);
                                    }
                                    PaySheetPekerjaList.Add(
                                        new vw_PaySheetPekerjaCustomModel()
                                        {
                                            PaySheetPekerja = salaryDataSingle,
                                            CarumanTambahan = carumanTambahanCustomModelList
                                        });
                                }
                                if (PaySheetPekerjaList.Count == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }
                                return View(PaySheetPekerjaList);
                            }

                        }
                    }
                }
                else
                {
                    //group
                    if (SelectionList == "0")
                    {
                        //group semua
                        if (JnsPkjList == "0")
                        {
                            if (PaymentModeList == "0")
                            {
                                IOrderedQueryable<ViewingModels.vw_PaySheetPekerja> salaryData;
                                if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                                {
                                    salaryData = dbview.vw_PaySheetPekerja
                                        .Where(x => x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList &&
                                                    x.fld_Year == YearList && x.fld_Month == MonthList &&
                                                    x.fld_NegaraID == NegaraID &&
                                                    x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID)
                                        .OrderBy(x => x.fld_Nama);
                                }
                                else
                                {
                                    salaryData = dbview.vw_PaySheetPekerja
                                        .Where(x => x.fld_NegaraID == NegaraID &&
                                                    x.fld_Year == YearList && x.fld_Month == MonthList &&
                                                    x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID)
                                        .OrderBy(x => x.fld_Nama);
                                }
                                foreach (var salary in salaryData)
                                {
                                    var workerAdditionalContribution = dbr.tbl_ByrCarumanTambahan
                                        .Where(x => x.fld_GajiID == salary.fld_ID && x.fld_NegaraID == NegaraID &&
                                                    x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                                    x.fld_LadangID == LadangID);
                                    List<CarumanTambahanCustomModel> carumanTambahanCustomModelList = new List<CarumanTambahanCustomModel>();
                                    foreach (var caruman in workerAdditionalContribution)
                                    {
                                        CarumanTambahanCustomModel carumanTambahanCustomModel = new CarumanTambahanCustomModel();
                                        carumanTambahanCustomModel.fld_ID = caruman.fld_ID;
                                        carumanTambahanCustomModel.fld_KodCarumanTambahan = caruman.fld_KodSubCaruman;
                                        carumanTambahanCustomModel.fld_CarumanMajikan = caruman.fld_CarumanMajikan;
                                        carumanTambahanCustomModel.fld_CarumanPekerja = caruman.fld_CarumanPekerja;
                                        carumanTambahanCustomModelList.Add(carumanTambahanCustomModel);
                                    }
                                    PaySheetPekerjaList.Add(
                                       new vw_PaySheetPekerjaCustomModel()
                                       {
                                           PaySheetPekerja = salary,
                                           CarumanTambahan = carumanTambahanCustomModelList
                                       });
                                }
                                if (PaySheetPekerjaList.Count == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }
                                return View(PaySheetPekerjaList);
                            }
                            else
                            {
                                IOrderedQueryable<ViewingModels.vw_PaySheetPekerja> salaryData;
                                if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                                {
                                    salaryData = dbview.vw_PaySheetPekerja
                                        .Where(x => x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList && x.fld_PaymentMode == PaymentModeList &&
                                                    x.fld_Year == YearList && x.fld_Month == MonthList &&
                                                    x.fld_NegaraID == NegaraID &&
                                                    x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID)
                                        .OrderBy(x => x.fld_Nama);
                                }
                                else
                                {
                                    salaryData = dbview.vw_PaySheetPekerja
                                        .Where(x => x.fld_NegaraID == NegaraID &&
                                                    x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_PaymentMode == PaymentModeList &&
                                                    x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID)
                                        .OrderBy(x => x.fld_Nama);
                                }
                                foreach (var salary in salaryData)
                                {
                                    var workerAdditionalContribution = dbr.tbl_ByrCarumanTambahan
                                        .Where(x => x.fld_GajiID == salary.fld_ID && x.fld_NegaraID == NegaraID &&
                                                    x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                                    x.fld_LadangID == LadangID);
                                    List<CarumanTambahanCustomModel> carumanTambahanCustomModelList = new List<CarumanTambahanCustomModel>();
                                    foreach (var caruman in workerAdditionalContribution)
                                    {
                                        CarumanTambahanCustomModel carumanTambahanCustomModel = new CarumanTambahanCustomModel();
                                        carumanTambahanCustomModel.fld_ID = caruman.fld_ID;
                                        carumanTambahanCustomModel.fld_KodCarumanTambahan = caruman.fld_KodSubCaruman;
                                        carumanTambahanCustomModel.fld_CarumanMajikan = caruman.fld_CarumanMajikan;
                                        carumanTambahanCustomModel.fld_CarumanPekerja = caruman.fld_CarumanPekerja;
                                        carumanTambahanCustomModelList.Add(carumanTambahanCustomModel);
                                    }
                                    PaySheetPekerjaList.Add(
                                       new vw_PaySheetPekerjaCustomModel()
                                       {
                                           PaySheetPekerja = salary,
                                           CarumanTambahan = carumanTambahanCustomModelList
                                       });
                                }
                                if (PaySheetPekerjaList.Count == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }
                                return View(PaySheetPekerjaList);
                            }

                        }
                        else
                        {
                            if (PaymentModeList == "0")
                            {
                                IOrderedQueryable<ViewingModels.vw_PaySheetPekerja> salaryData;
                                if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                                {
                                    salaryData = dbview.vw_PaySheetPekerja
                                        .Where(x => x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList && x.fld_Jenispekerja == JnsPkjList &&
                                                    x.fld_Year == YearList && x.fld_Month == MonthList &&
                                                    x.fld_NegaraID == NegaraID &&
                                                    x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID)
                                        .OrderBy(x => x.fld_Nama);
                                }
                                else
                                {
                                    salaryData = dbview.vw_PaySheetPekerja
                                        .Where(x => x.fld_NegaraID == NegaraID && x.fld_Jenispekerja == JnsPkjList &&
                                                    x.fld_Year == YearList && x.fld_Month == MonthList &&
                                                    x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID)
                                        .OrderBy(x => x.fld_Nama);
                                }
                                foreach (var salary in salaryData)
                                {
                                    var workerAdditionalContribution = dbr.tbl_ByrCarumanTambahan
                                        .Where(x => x.fld_GajiID == salary.fld_ID && x.fld_NegaraID == NegaraID &&
                                                    x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                                    x.fld_LadangID == LadangID);
                                    List<CarumanTambahanCustomModel> carumanTambahanCustomModelList = new List<CarumanTambahanCustomModel>();
                                    foreach (var caruman in workerAdditionalContribution)
                                    {
                                        CarumanTambahanCustomModel carumanTambahanCustomModel = new CarumanTambahanCustomModel();
                                        carumanTambahanCustomModel.fld_ID = caruman.fld_ID;
                                        carumanTambahanCustomModel.fld_KodCarumanTambahan = caruman.fld_KodSubCaruman;
                                        carumanTambahanCustomModel.fld_CarumanMajikan = caruman.fld_CarumanMajikan;
                                        carumanTambahanCustomModel.fld_CarumanPekerja = caruman.fld_CarumanPekerja;
                                        carumanTambahanCustomModelList.Add(carumanTambahanCustomModel);
                                    }
                                    PaySheetPekerjaList.Add(
                                       new vw_PaySheetPekerjaCustomModel()
                                       {
                                           PaySheetPekerja = salary,
                                           CarumanTambahan = carumanTambahanCustomModelList
                                       });
                                }
                                if (PaySheetPekerjaList.Count == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }
                                return View(PaySheetPekerjaList);
                            }
                            else
                            {
                                IOrderedQueryable<ViewingModels.vw_PaySheetPekerja> salaryData;
                                if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                                {
                                    salaryData = dbview.vw_PaySheetPekerja
                                        .Where(x => x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList && x.fld_Jenispekerja == JnsPkjList && x.fld_PaymentMode == PaymentModeList &&
                                                    x.fld_Year == YearList && x.fld_Month == MonthList &&
                                                    x.fld_NegaraID == NegaraID &&
                                                    x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID)
                                        .OrderBy(x => x.fld_Nama);
                                }
                                else
                                {
                                    salaryData = dbview.vw_PaySheetPekerja
                                        .Where(x => x.fld_NegaraID == NegaraID && x.fld_Jenispekerja == JnsPkjList && x.fld_PaymentMode == PaymentModeList &&
                                                    x.fld_Year == YearList && x.fld_Month == MonthList &&
                                                    x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID)
                                        .OrderBy(x => x.fld_Nama);
                                }
                                foreach (var salary in salaryData)
                                {
                                    var workerAdditionalContribution = dbr.tbl_ByrCarumanTambahan
                                        .Where(x => x.fld_GajiID == salary.fld_ID && x.fld_NegaraID == NegaraID &&
                                                    x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                                    x.fld_LadangID == LadangID);
                                    List<CarumanTambahanCustomModel> carumanTambahanCustomModelList = new List<CarumanTambahanCustomModel>();
                                    foreach (var caruman in workerAdditionalContribution)
                                    {
                                        CarumanTambahanCustomModel carumanTambahanCustomModel = new CarumanTambahanCustomModel();
                                        carumanTambahanCustomModel.fld_ID = caruman.fld_ID;
                                        carumanTambahanCustomModel.fld_KodCarumanTambahan = caruman.fld_KodSubCaruman;
                                        carumanTambahanCustomModel.fld_CarumanMajikan = caruman.fld_CarumanMajikan;
                                        carumanTambahanCustomModel.fld_CarumanPekerja = caruman.fld_CarumanPekerja;
                                        carumanTambahanCustomModelList.Add(carumanTambahanCustomModel);
                                    }
                                    PaySheetPekerjaList.Add(
                                       new vw_PaySheetPekerjaCustomModel()
                                       {
                                           PaySheetPekerja = salary,
                                           CarumanTambahan = carumanTambahanCustomModelList
                                       });
                                }
                                if (PaySheetPekerjaList.Count == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }
                                return View(PaySheetPekerjaList);
                            }

                        }
                    }
                    else
                    {
                        if (JnsPkjList == "0")
                        {
                            if (PaymentModeList == "0")
                            {
                                var groupData = dbview.tbl_KumpulanKerja
                                .Where(x => x.fld_KodKumpulan == SelectionList && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                            x.fld_LadangID == LadangID)
                                .Select(s => s.fld_KumpulanID)
                                .SingleOrDefault();
                                IOrderedQueryable<ViewingModels.vw_PaySheetPekerja> salaryData;
                                if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                                {
                                    salaryData = dbview.vw_PaySheetPekerja
                                        .Where(x => x.fld_KumpulanID == groupData && x.fld_Kdaktf == StatusList &&
                                                    x.fld_Year == YearList && x.fld_Month == MonthList &&
                                                    x.fld_Ktgpkj == WorkCategoryList &&
                                                    x.fld_NegaraID == NegaraID &&
                                                    x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID)
                                        .OrderBy(x => x.fld_Nama);
                                }
                                else
                                {
                                    salaryData = dbview.vw_PaySheetPekerja
                                        .Where(x => x.fld_KumpulanID == groupData && x.fld_NegaraID == NegaraID &&
                                                    x.fld_Year == YearList && x.fld_Month == MonthList &&
                                                    x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID)
                                        .OrderBy(x => x.fld_Nama);
                                }
                                foreach (var salary in salaryData)
                                {
                                    var workerAdditionalContribution = dbr.tbl_ByrCarumanTambahan
                                        .Where(x => x.fld_GajiID == salary.fld_ID && x.fld_NegaraID == NegaraID &&
                                                    x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                                    x.fld_LadangID == LadangID);
                                    List<CarumanTambahanCustomModel> carumanTambahanCustomModelList = new List<CarumanTambahanCustomModel>();
                                    foreach (var caruman in workerAdditionalContribution)
                                    {
                                        CarumanTambahanCustomModel carumanTambahanCustomModel = new CarumanTambahanCustomModel();
                                        carumanTambahanCustomModel.fld_ID = caruman.fld_ID;
                                        carumanTambahanCustomModel.fld_KodCarumanTambahan = caruman.fld_KodSubCaruman;
                                        carumanTambahanCustomModel.fld_CarumanMajikan = caruman.fld_CarumanMajikan;
                                        carumanTambahanCustomModel.fld_CarumanPekerja = caruman.fld_CarumanPekerja;
                                        carumanTambahanCustomModelList.Add(carumanTambahanCustomModel);
                                    }
                                    PaySheetPekerjaList.Add(
                                       new vw_PaySheetPekerjaCustomModel()
                                       {
                                           PaySheetPekerja = salary,
                                           CarumanTambahan = carumanTambahanCustomModelList
                                       });
                                }
                                if (PaySheetPekerjaList.Count == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }
                                return View(PaySheetPekerjaList);
                            }
                            else
                            {
                                var groupData = dbview.tbl_KumpulanKerja
                                .Where(x => x.fld_KodKumpulan == SelectionList && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                            x.fld_LadangID == LadangID)
                                .Select(s => s.fld_KumpulanID)
                                .SingleOrDefault();
                                IOrderedQueryable<ViewingModels.vw_PaySheetPekerja> salaryData;
                                if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                                {
                                    salaryData = dbview.vw_PaySheetPekerja
                                        .Where(x => x.fld_KumpulanID == groupData && x.fld_Kdaktf == StatusList && x.fld_PaymentMode == PaymentModeList &&
                                                    x.fld_Year == YearList && x.fld_Month == MonthList &&
                                                    x.fld_Ktgpkj == WorkCategoryList &&
                                                    x.fld_NegaraID == NegaraID &&
                                                    x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID)
                                        .OrderBy(x => x.fld_Nama);
                                }
                                else
                                {
                                    salaryData = dbview.vw_PaySheetPekerja
                                        .Where(x => x.fld_KumpulanID == groupData && x.fld_NegaraID == NegaraID && x.fld_PaymentMode == PaymentModeList &&
                                                    x.fld_Year == YearList && x.fld_Month == MonthList &&
                                                    x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID)
                                        .OrderBy(x => x.fld_Nama);
                                }
                                foreach (var salary in salaryData)
                                {
                                    var workerAdditionalContribution = dbr.tbl_ByrCarumanTambahan
                                        .Where(x => x.fld_GajiID == salary.fld_ID && x.fld_NegaraID == NegaraID &&
                                                    x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                                    x.fld_LadangID == LadangID);
                                    List<CarumanTambahanCustomModel> carumanTambahanCustomModelList = new List<CarumanTambahanCustomModel>();
                                    foreach (var caruman in workerAdditionalContribution)
                                    {
                                        CarumanTambahanCustomModel carumanTambahanCustomModel = new CarumanTambahanCustomModel();
                                        carumanTambahanCustomModel.fld_ID = caruman.fld_ID;
                                        carumanTambahanCustomModel.fld_KodCarumanTambahan = caruman.fld_KodSubCaruman;
                                        carumanTambahanCustomModel.fld_CarumanMajikan = caruman.fld_CarumanMajikan;
                                        carumanTambahanCustomModel.fld_CarumanPekerja = caruman.fld_CarumanPekerja;
                                        carumanTambahanCustomModelList.Add(carumanTambahanCustomModel);
                                    }
                                    PaySheetPekerjaList.Add(
                                       new vw_PaySheetPekerjaCustomModel()
                                       {
                                           PaySheetPekerja = salary,
                                           CarumanTambahan = carumanTambahanCustomModelList
                                       });
                                }
                                if (PaySheetPekerjaList.Count == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }
                                return View(PaySheetPekerjaList);
                            }

                        }
                        else
                        {
                            if (PaymentModeList == "0")
                            {
                                var groupData = dbview.tbl_KumpulanKerja
                                .Where(x => x.fld_KodKumpulan == SelectionList && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                            x.fld_LadangID == LadangID)
                                .Select(s => s.fld_KumpulanID)
                                .SingleOrDefault();
                                IOrderedQueryable<ViewingModels.vw_PaySheetPekerja> salaryData;
                                if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                                {
                                    salaryData = dbview.vw_PaySheetPekerja
                                        .Where(x => x.fld_KumpulanID == groupData && x.fld_Kdaktf == StatusList && x.fld_Jenispekerja == JnsPkjList &&
                                                    x.fld_Year == YearList && x.fld_Month == MonthList &&
                                                    x.fld_Ktgpkj == WorkCategoryList &&
                                                    x.fld_NegaraID == NegaraID &&
                                                    x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID)
                                        .OrderBy(x => x.fld_Nama);
                                }
                                else
                                {
                                    salaryData = dbview.vw_PaySheetPekerja
                                        .Where(x => x.fld_KumpulanID == groupData && x.fld_NegaraID == NegaraID &&
                                                    x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_Jenispekerja == JnsPkjList &&
                                                    x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID)
                                        .OrderBy(x => x.fld_Nama);
                                }
                                foreach (var salary in salaryData)
                                {
                                    var workerAdditionalContribution = dbr.tbl_ByrCarumanTambahan
                                        .Where(x => x.fld_GajiID == salary.fld_ID && x.fld_NegaraID == NegaraID &&
                                                    x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                                    x.fld_LadangID == LadangID);
                                    List<CarumanTambahanCustomModel> carumanTambahanCustomModelList = new List<CarumanTambahanCustomModel>();
                                    foreach (var caruman in workerAdditionalContribution)
                                    {
                                        CarumanTambahanCustomModel carumanTambahanCustomModel = new CarumanTambahanCustomModel();
                                        carumanTambahanCustomModel.fld_ID = caruman.fld_ID;
                                        carumanTambahanCustomModel.fld_KodCarumanTambahan = caruman.fld_KodSubCaruman;
                                        carumanTambahanCustomModel.fld_CarumanMajikan = caruman.fld_CarumanMajikan;
                                        carumanTambahanCustomModel.fld_CarumanPekerja = caruman.fld_CarumanPekerja;
                                        carumanTambahanCustomModelList.Add(carumanTambahanCustomModel);
                                    }
                                    PaySheetPekerjaList.Add(
                                       new vw_PaySheetPekerjaCustomModel()
                                       {
                                           PaySheetPekerja = salary,
                                           CarumanTambahan = carumanTambahanCustomModelList
                                       });
                                }
                                if (PaySheetPekerjaList.Count == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }
                                return View(PaySheetPekerjaList);
                            }
                            else
                            {
                                var groupData = dbview.tbl_KumpulanKerja
                                .Where(x => x.fld_KodKumpulan == SelectionList && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                            x.fld_LadangID == LadangID)
                                .Select(s => s.fld_KumpulanID)
                                .SingleOrDefault();
                                IOrderedQueryable<ViewingModels.vw_PaySheetPekerja> salaryData;
                                if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                                {
                                    salaryData = dbview.vw_PaySheetPekerja
                                        .Where(x => x.fld_KumpulanID == groupData && x.fld_Kdaktf == StatusList && x.fld_Jenispekerja == JnsPkjList && x.fld_PaymentMode == PaymentModeList &&
                                                    x.fld_Year == YearList && x.fld_Month == MonthList &&
                                                    x.fld_Ktgpkj == WorkCategoryList &&
                                                    x.fld_NegaraID == NegaraID &&
                                                    x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID)
                                        .OrderBy(x => x.fld_Nama);
                                }
                                else
                                {
                                    salaryData = dbview.vw_PaySheetPekerja
                                        .Where(x => x.fld_KumpulanID == groupData && x.fld_NegaraID == NegaraID && x.fld_PaymentMode == PaymentModeList &&
                                                    x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_Jenispekerja == JnsPkjList &&
                                                    x.fld_SyarikatID == SyarikatID &&
                                                    x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_DivisionID == DivisionID)
                                        .OrderBy(x => x.fld_Nama);
                                }
                                foreach (var salary in salaryData)
                                {
                                    var workerAdditionalContribution = dbr.tbl_ByrCarumanTambahan
                                        .Where(x => x.fld_GajiID == salary.fld_ID && x.fld_NegaraID == NegaraID &&
                                                    x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                                    x.fld_LadangID == LadangID);
                                    List<CarumanTambahanCustomModel> carumanTambahanCustomModelList = new List<CarumanTambahanCustomModel>();
                                    foreach (var caruman in workerAdditionalContribution)
                                    {
                                        CarumanTambahanCustomModel carumanTambahanCustomModel = new CarumanTambahanCustomModel();
                                        carumanTambahanCustomModel.fld_ID = caruman.fld_ID;
                                        carumanTambahanCustomModel.fld_KodCarumanTambahan = caruman.fld_KodSubCaruman;
                                        carumanTambahanCustomModel.fld_CarumanMajikan = caruman.fld_CarumanMajikan;
                                        carumanTambahanCustomModel.fld_CarumanPekerja = caruman.fld_CarumanPekerja;
                                        carumanTambahanCustomModelList.Add(carumanTambahanCustomModel);
                                    }
                                    PaySheetPekerjaList.Add(
                                       new vw_PaySheetPekerjaCustomModel()
                                       {
                                           PaySheetPekerja = salary,
                                           CarumanTambahan = carumanTambahanCustomModelList
                                       });
                                }
                                if (PaySheetPekerjaList.Count == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }
                                return View(PaySheetPekerjaList);
                            }

                        }
                    }
                }
            }
        }

        public ActionResult AverageMonthlySalaryReport()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int range = int.Parse(GetConfig.GetData("yeardisplay"));
            int startyear = DateTime.Now.AddYears(-range).Year;
            int currentyear = DateTime.Now.Year;
            DateTime selectdate = DateTime.Now;

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

            List<SelectListItem> GroupList = new List<SelectListItem>();
            GroupList = new SelectList(dbr.tbl_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).OrderBy(o => o.fld_KodKumpulan).Select(s => new SelectListItem { Value = s.fld_KumpulanID.ToString(), Text = s.fld_KodKumpulan }).Distinct(), "Value", "Text").ToList();
            GroupList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            List<SelectListItem> WorkerList = new List<SelectListItem>();
            WorkerList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nama).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + " - " + s.fld_Nama }).Distinct(), "Value", "Text").ToList();
            WorkerList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.YearList = yearlist;
            ViewBag.GroupList = GroupList;
            ViewBag.WorkerList = WorkerList;
            return View();
        }
        
        public ViewResult AverageMonthlySalaryReportDetail(int? YearList, string GroupList, string WorkerList)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            //MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_SP_Models dbsp = MVC_SYSTEM_SP_Models.ConnectToSqlServer(host, catalog, user, pass);

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.YearSelection = YearList;
            if (GroupList == "0" && WorkerList == "0") // edit by fitri 08-10-2020
            {
                var result = dbsp.sp_RptPurataGajiBulanan(NegaraID, SyarikatID, WilayahID, LadangID, YearList).ToList();
                ViewBag.DataCount = result.Count();
                return View(result);
            }
            else if (GroupList == "0" && WorkerList != "0") // add by fitri 08-10-2020
            {
                var result = dbsp.sp_RptPurataGajiBulanan(NegaraID, SyarikatID, WilayahID, LadangID, YearList).Where(x => x.fld_Nopkj == WorkerList).ToList();
                ViewBag.DataCount = result.Count();
                return View(result);
            }
            else
            {
                GroupList = GroupList == null ? "0" : GroupList;
                int groupID = int.Parse(GroupList);
                if (WorkerList == "0")
                {
                    var result = dbsp.sp_RptPurataGajiBulanan(NegaraID, SyarikatID, WilayahID, LadangID,YearList).Where(x=>x.fld_GroupID==groupID).ToList();
                    ViewBag.DataCount = result.Count();
                    return View(result);
                }
                else
                {
                    var result = dbsp.sp_RptPurataGajiBulanan(NegaraID, SyarikatID, WilayahID, LadangID, YearList).Where(x=>x.fld_GroupID==groupID && x.fld_Nopkj==WorkerList).ToList();
                    ViewBag.DataCount = result.Count();
                    return View(result);
                }
            }
        }

        public ActionResult MinimumWageReport()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int drpyear = 0;
            int drprangeyear = 0;
            int month = timezone.gettimezone().Month;

            List<SelectListItem> SelectionList = new List<SelectListItem>();
            SelectionList = new SelectList(
                dbr.tbl_Pkjmast
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1")
                    .OrderBy(o => o.fld_Nopkj)
                    .Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }),
                "Value", "Text").ToList();
            SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.SelectionList = SelectionList;

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
                "fldOptConfValue", "fldOptConfDesc");

            ViewBag.YearList = yearlist;

            var statusList = new List<SelectListItem>();
            statusList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            ViewBag.StatusList = statusList;

            return View();
        }

        public ActionResult _MinimumWageRptAdvanceSearch()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var statusList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            ViewBag.StatusList = statusList;

            var workCategoryList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "designation" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            ViewBag.WorkCategoryList = workCategoryList;

            return View();
        }

        public ViewResult _MinimumWageRptSearch(int? RadioGroup, int? MonthList, int? YearList,
            string SelectionList, string StatusList, string WorkCategoryList, string print)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Viewing dbview2 = new MVC_SYSTEM_Viewing();

            List<CustMod_MinimumWage> GajiMinimaList = new List<CustMod_MinimumWage>();

            ViewBag.MonthList = MonthList;
            ViewBag.YearList = YearList;
            ViewBag.WorkerList = SelectionList;
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;

            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();
            ViewBag.Print = print;

            var minimumWageValue = dbview2.tblOptionConfigsWeb
                .Where(x => x.fldOptConfFlag1 == "gajiMinima" && x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                .Select(s => s.fldOptConfValue)
                .Single();

            var minimumWageInt = Convert.ToInt32(minimumWageValue);


            if (MonthList == null && YearList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChooseWork;
                return View(GajiMinimaList);
            }

            else
            {
                if (RadioGroup == 0)

                {
                    if (SelectionList == "0")
                    {
                        IOrderedQueryable<ViewingModels.vw_GajiMinima> workerData;

                        if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                        {
                            workerData = dbview.vw_GajiMinima
                                .Where(x => x.fld_ByrKerja < minimumWageInt && x.fld_Month == MonthList && x.fld_Year == YearList &&
                                            x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList &&
                                            x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        else
                        {
                            workerData = dbview.vw_GajiMinima
                                .Where(x => x.fld_ByrKerja < minimumWageInt && x.fld_Month == MonthList && x.fld_Year == YearList &&
                                            x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        foreach (var worker in workerData)
                        {
                            var getOfferedWorkingDay = dbview.tbl_Produktiviti
                                .Where(x => x.fld_Nopkj == worker.fld_Nopkj && x.fld_Month == MonthList &&
                                            x.fld_Year == YearList)
                                .Select(s => s.fld_HadirKerja)
                                .SingleOrDefault();

                            var getActualWorkingDay = dbview.tbl_Kerjahdr
                                .Where(x => x.fld_Nopkj == worker.fld_Nopkj &&
                                            x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList &&
                                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                            .Select(s => s.fld_Kdhdct == "H01" || s.fld_Kdhdct == "H02" || s.fld_Kdhdct == "H03").Count();

                            CustMod_MinimumWage GajiMinima = new CustMod_MinimumWage();

                            GajiMinima.NoPkj = worker.fld_Nopkj;
                            GajiMinima.Nama = worker.fld_Nama;
                            GajiMinima.Warganegara = worker.fld_Kdrkyt;
                            GajiMinima.TarikhSahJawatan = worker.fld_Trshjw;
                            GajiMinima.Nokp = worker.fld_Nokp;
                            GajiMinima.KategoriKerja = worker.fld_Ktgpkj;
                            GajiMinima.JumlahHariBekerja = getActualWorkingDay;
                            GajiMinima.JumlahHariTawaranKerja = getOfferedWorkingDay;
                            GajiMinima.GajiBulanan = worker.fld_ByrKerja;
                            GajiMinima.Sebab = worker.fld_Sebab;
                            GajiMinima.PelanTindakan = worker.fld_Tindakan;
                            GajiMinimaList.Add(GajiMinima);
                        }

                        if (GajiMinimaList.Count == 0)
                        {
                            ViewBag.Message = GlobalResEstate.msgNoRecord;
                        }

                        return View(GajiMinimaList);
                    }

                    else
                    {
                        var workerDataSingle = new ViewingModels.vw_GajiMinima();

                        if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                        {
                            workerDataSingle = dbview.vw_GajiMinima
                                .Where(x => x.fld_ByrKerja < minimumWageInt && x.fld_Month == MonthList && x.fld_Year == YearList &&
                                            x.fld_Nopkj == SelectionList && x.fld_Kdaktf == StatusList &&
                                            x.fld_Ktgpkj == WorkCategoryList && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama)
                                .SingleOrDefault();
                        }

                        else
                        {
                            workerDataSingle = dbview.vw_GajiMinima
                                .Where(x => x.fld_ByrKerja < minimumWageInt && x.fld_Month == MonthList && x.fld_Year == YearList &&
                                            x.fld_Nopkj == SelectionList && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama)
                                .SingleOrDefault();
                        }

                        if (workerDataSingle != null)
                        {
                            var getOfferedWorkingDay = dbview.tbl_Produktiviti
                                .Where(x => x.fld_Nopkj == SelectionList && x.fld_Month == MonthList &&
                                            x.fld_Year == YearList)
                                .Select(s => s.fld_HadirKerja)
                                .SingleOrDefault();

                            var getActualWorkingDay = dbview.tbl_Kerjahdr
                                .Where(x => x.fld_Nopkj == SelectionList &&
                                            x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList &&
                                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .Select(s => s.fld_Kdhdct == "H01" || s.fld_Kdhdct == "H02" || s.fld_Kdhdct == "H03").Count();

                            CustMod_MinimumWage GajiMinima = new CustMod_MinimumWage();

                            GajiMinima.NoPkj = workerDataSingle.fld_Nopkj;
                            GajiMinima.Nama = workerDataSingle.fld_Nama;
                            GajiMinima.Warganegara = workerDataSingle.fld_Kdrkyt;
                            GajiMinima.TarikhSahJawatan = workerDataSingle.fld_Trshjw;
                            GajiMinima.Nokp = workerDataSingle.fld_Nokp;
                            GajiMinima.KategoriKerja = workerDataSingle.fld_Ktgpkj;
                            GajiMinima.JumlahHariBekerja = getActualWorkingDay;
                            GajiMinima.JumlahHariTawaranKerja = getOfferedWorkingDay;
                            GajiMinima.GajiBulanan = workerDataSingle.fld_ByrKerja;
                            GajiMinima.Sebab = workerDataSingle.fld_Sebab;
                            GajiMinima.PelanTindakan = workerDataSingle.fld_Tindakan;
                            GajiMinimaList.Add(GajiMinima);
                        }
                    }

                    if (GajiMinimaList.Count == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }

                    return View(GajiMinimaList);
                }

                else
                {
                    if (SelectionList == "0")
                    {
                        IOrderedQueryable<ViewingModels.vw_GajiMinima> workerData;

                        if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                        {
                            workerData = dbview.vw_GajiMinima
                                .Where(x => x.fld_ByrKerja < minimumWageInt && x.fld_Month == MonthList && x.fld_Year == YearList &&
                                            x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList &&
                                            x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        else
                        {
                            workerData = dbview.vw_GajiMinima
                                .Where(x => x.fld_ByrKerja < minimumWageInt && x.fld_Month == MonthList && x.fld_Year == YearList &&
                                            x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        foreach (var worker in workerData)
                        {
                            var getOfferedWorkingDay = dbview.tbl_Produktiviti
                                .Where(x => x.fld_Nopkj == worker.fld_Nopkj && x.fld_Month == MonthList &&
                                            x.fld_Year == YearList)
                                .Select(s => s.fld_HadirKerja)
                                .SingleOrDefault();

                            var getActualWorkingDay = dbview.tbl_Kerjahdr
                                .Where(x => x.fld_Nopkj == worker.fld_Nopkj &&
                                            x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList &&
                                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .Select(s => s.fld_Kdhdct == "H01" || s.fld_Kdhdct == "H02" || s.fld_Kdhdct == "H03").Count();

                            CustMod_MinimumWage GajiMinima = new CustMod_MinimumWage();

                            GajiMinima.NoPkj = worker.fld_Nopkj;
                            GajiMinima.Nama = worker.fld_Nama;
                            GajiMinima.Warganegara = worker.fld_Kdrkyt;
                            GajiMinima.TarikhSahJawatan = worker.fld_Trshjw;
                            GajiMinima.Nokp = worker.fld_Nokp;
                            GajiMinima.KategoriKerja = worker.fld_Ktgpkj;
                            GajiMinima.JumlahHariBekerja = getActualWorkingDay;
                            GajiMinima.JumlahHariTawaranKerja = getOfferedWorkingDay;
                            GajiMinima.GajiBulanan = worker.fld_ByrKerja;
                            GajiMinima.Sebab = worker.fld_Sebab;
                            GajiMinima.PelanTindakan = worker.fld_Tindakan;
                            GajiMinimaList.Add(GajiMinima);
                        }

                        if (GajiMinimaList.Count == 0)
                        {
                            ViewBag.Message = GlobalResEstate.msgNoRecord;
                        }

                        return View(GajiMinimaList);
                    }

                    else
                    {
                        var groupData = dbview.tbl_KumpulanKerja
                            .Where(x => x.fld_KodKumpulan == SelectionList && x.fld_NegaraID == NegaraID &&
                                        x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                        x.fld_LadangID == LadangID)
                            .Select(s => s.fld_KumpulanID)
                            .SingleOrDefault();

                        IOrderedQueryable<ViewingModels.vw_GajiMinima> workerData;

                        if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                        {
                            workerData = dbview.vw_GajiMinima
                                .Where(x => x.fld_ByrKerja < minimumWageInt && x.fld_Month == MonthList && x.fld_Year == YearList &&
                                            x.fld_KumpulanID == groupData && x.fld_Kdaktf == StatusList &&
                                            x.fld_Ktgpkj == WorkCategoryList &&
                                            x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        else
                        {
                            workerData = dbview.vw_GajiMinima
                                .Where(x => x.fld_ByrKerja < minimumWageInt && x.fld_Month == MonthList && x.fld_Year == YearList &&
                                            x.fld_KumpulanID == groupData && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        foreach (var worker in workerData)
                        {
                            var getOfferedWorkingDay = dbview.tbl_Produktiviti
                                .Where(x => x.fld_Nopkj == worker.fld_Nopkj && x.fld_Month == MonthList &&
                                            x.fld_Year == YearList)
                                .Select(s => s.fld_HadirKerja)
                                .SingleOrDefault();

                            var getActualWorkingDay = dbview.tbl_Kerjahdr
                                .Where(x => x.fld_Nopkj == worker.fld_Nopkj &&
                                            x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList &&
                                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .Select(s => s.fld_Kdhdct == "H01" || s.fld_Kdhdct == "H02" || s.fld_Kdhdct == "H03").Count();

                            CustMod_MinimumWage GajiMinima = new CustMod_MinimumWage();

                            GajiMinima.NoPkj = worker.fld_Nopkj;
                            GajiMinima.Nama = worker.fld_Nama;
                            GajiMinima.Warganegara = worker.fld_Kdrkyt;
                            GajiMinima.TarikhSahJawatan = worker.fld_Trshjw;
                            GajiMinima.Nokp = worker.fld_Nokp;
                            GajiMinima.KategoriKerja = worker.fld_Ktgpkj;
                            GajiMinima.JumlahHariBekerja = getActualWorkingDay;
                            GajiMinima.JumlahHariTawaranKerja = getOfferedWorkingDay;
                            GajiMinima.GajiBulanan = worker.fld_ByrKerja;
                            GajiMinima.Sebab = worker.fld_Sebab;
                            GajiMinima.PelanTindakan = worker.fld_Tindakan;
                            GajiMinimaList.Add(GajiMinima);
                        }
                    }

                    if (GajiMinimaList.Count == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }

                    return View(GajiMinimaList);
                }
            }
        }

        public ActionResult ProductivityReport()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int drpyear = 0;
            int drprangeyear = 0;
            int month = timezone.gettimezone().Month;

            List<SelectListItem> SelectionList = new List<SelectListItem>();
            SelectionList = new SelectList(
                dbr.tbl_Pkjmast
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1")
                    .OrderBy(o => o.fld_Nopkj)
                    .Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }),
                "Value", "Text").ToList();
            SelectionList.Insert(0, (new SelectListItem { Text = "Semua", Value = "" }));

            ViewBag.SelectionList = SelectionList;

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
                db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fld_NegaraID == NegaraID &&
                                                   x.fld_SyarikatID == SyarikatID && x.fldDeleted == false),
                "fldOptConfValue", "fldOptConfDesc", month);

            var statusList = new List<SelectListItem>();
            statusList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem {Value = s.fldOptConfValue, Text = s.fldOptConfDesc}),
                "Value", "Text").ToList();
            statusList.Insert(0, (new SelectListItem { Text = "Semua", Value = "" }));


            ViewBag.StatusList = statusList;

            var unitList = new List<SelectListItem>();
            unitList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "unit" && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();
            unitList.Insert(0, (new SelectListItem { Text = "Semua", Value = "" }));

            var allPeringkatList = new List<SelectListItem>();

            var peringkatList = new SelectList(
                dbr.tbl_PktUtama
                    .Where(x => x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                x.fld_LadangID == LadangID && x.fld_Deleted == false)
                    .OrderBy(o => o.fld_PktUtama)
                    .Select(s => new SelectListItem {Value = s.fld_PktUtama, Text = s.fld_PktUtama}),
                "Value", "Text").ToList();

            allPeringkatList.AddRange(peringkatList);

            var subPktList = new SelectList(
                dbr.tbl_SubPkt
                    .Where(x => x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                x.fld_LadangID == LadangID && x.fld_Deleted == false)
                    .OrderBy(o => o.fld_Pkt)
                    .Select(s => new SelectListItem {Value = s.fld_Pkt, Text = s.fld_Pkt}),
                "Value", "Text").ToList();

            allPeringkatList.AddRange(subPktList);

            var blokList = new SelectList(
                dbr.tbl_Blok
                    .Where(x => x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                x.fld_LadangID == LadangID && x.fld_Deleted == false)
                    .OrderBy(o => o.fld_Blok)
                    .Select(s => new SelectListItem {Value = s.fld_Blok, Text = s.fld_Blok}),
                "Value", "Text").ToList();

            allPeringkatList.AddRange(blokList);

            allPeringkatList.Insert(0, (new SelectListItem { Text = "Semua", Value = "" }));
            ViewBag.AllPeringkatList = allPeringkatList;
            //ViewBag.UnitList = unitList;

            allPeringkatList.Insert(0, (new SelectListItem { Text = "Semua", Value = "" }));
            ViewBag.AllPeringkatList = allPeringkatList;
            ViewBag.UnitList = unitList;

            // add by fitri 17-08-2020
            List<SelectListItem> PilihanAktvt = new List<SelectListItem>();
            var SelectAktvt = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).ToList();
            PilihanAktvt = new SelectList(SelectAktvt.Select(s => new SelectListItem { Value = s.fld_KodAktvt, Text = s.fld_KodAktvt + " - " + s.fld_Desc }).Distinct().OrderBy(o => o.Value), "Value", "Text").ToList();
            PilihanAktvt.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));
            ViewBag.PilihanAktvt = PilihanAktvt;
            // add by fitri 17-08-2020
            List<SelectListItem> PilihanGL = new List<SelectListItem>();
            var SelectGL = db.tbl_MapGL.Join(db.tbl_SAPGLPUP, j => new { GL = j.fld_KodGL }, k => new { GL = k.fld_GLCode }, (j, k) => new { j.fld_ID, j.fld_KodGL, k.fld_GLDesc }).Select(s => new { s.fld_KodGL, s.fld_GLDesc }).Distinct().ToList();
            PilihanGL = new SelectList(SelectGL.Select(s => new SelectListItem { Value = s.fld_KodGL, Text = s.fld_KodGL + " - " + s.fld_GLDesc.ToUpper() }), "Value", "Text").ToList();
            PilihanGL.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));
            ViewBag.PilihanGL = PilihanGL;

            return View();
        }

        // edited by fitri 17-08-2020 => PilihanGL, PilihanAktvt
        public ViewResult _ProductivityRptSearch(string PilihanGL, string PilihanAktvt, int? MonthList, int? YearList, string SelectionList, string UnitList, string AllPeringkatList, string StatusList, string print) // Code Asal
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            //MVC_SYSTEM_SP_Models dbsp = MVC_SYSTEM_SP_Models.ConnectToSqlServer(host, catalog, user, pass); // comment by fitri 30-10-2020
            //List<sp_RptProduktiviti_Result> RptProduktiviti = new List<sp_RptProduktiviti_Result>(); // comment by fitri 30-10-2020
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass); // add by fitri 28-08-2020


            ViewBag.MonthList = MonthList;
            ViewBag.YearList = YearList;
            ViewBag.WorkerList = SelectionList;
            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.Print = print;

            // add by fitri 17-08-2020
            // comment by fitri 30-10-2020
            /*var ActvtList = new List<CustMod_ActvtList2>();
            int id = 1;
            RptProduktiviti = dbsp.sp_RptProduktiviti(NegaraID, SyarikatID, WilayahID, LadangID, YearList, MonthList, SelectionList, UnitList, AllPeringkatList, StatusList).ToList();
            foreach (var prdct in RptProduktiviti)
            {
                //add by fitri 5-10-2020
                var getActvtCode = dbr.tbl_Kerja.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID
                        && a.fld_Nopkj == prdct.fld_Nopkj && a.fld_Tarikh.Value.Year == YearList && a.fld_Tarikh.Value.Month == MonthList).Select(s => s.fld_KodAktvt).ToList();

                //edited by fitri 5-10-2020
                var getActvt = db.tbl_UpahAktiviti.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && getActvtCode.Contains(a.fld_KodAktvt) && a.fld_Deleted == false).ToList();
                foreach (var xtvty in getActvt)
                {
                    var KodXtvt = xtvty.fld_KodAktvt;
                    // edited by fitri 28-08-2020
                    var NoPkj = prdct.fld_Nopkj;
                    var GetWorkers = dbr.tbl_Pkjmast.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID && a.fld_Kdaktf == "1" && a.fld_Nopkj == NoPkj).ToList();
                    foreach (var gwrk in GetWorkers)
                    {
                        var Kryt = gwrk.fld_Kdrkyt;
                        if (Kryt == "MA")
                        {
                            var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && a.fld_KodAktvt == KodXtvt && a.fld_Paysheet == "PT").ToList();
                            foreach (var gl in GetGLKod)
                            {
                                ActvtList.Add(new CustMod_ActvtList2
                                {
                                    id = id,
                                    actvtcode = xtvty.fld_KodAktvt,
                                    actvtdesc = xtvty.fld_Desc,
                                    kodgl = gl.fld_KodGL,
                                    nopkj = prdct.fld_Nopkj,
                                    namapkj = prdct.fld_Nama
                                });
                                id++;
                            }
                        }
                        else
                        {
                            var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && a.fld_KodAktvt == KodXtvt && a.fld_Paysheet == "PA").ToList();
                            foreach (var gl in GetGLKod)
                            {
                                ActvtList.Add(new CustMod_ActvtList2
                                {
                                    id = id,
                                    actvtcode = xtvty.fld_KodAktvt,
                                    actvtdesc = xtvty.fld_Desc,
                                    kodgl = gl.fld_KodGL,
                                    nopkj = prdct.fld_Nopkj,
                                    namapkj = prdct.fld_Nama
                                });
                                id++;
                            }
                        }
                    }
                }
            }


            if (ActvtList.Count == 0)
            {
                ViewBag.Message = "Tiada Rekod";
            }
            if (PilihanAktvt == "0" && PilihanGL == "0")
            {
                ActvtList = ActvtList.ToList();
            }
            else if (PilihanAktvt != "0" && PilihanGL != "0")
            {
                ActvtList = ActvtList.Where(x => x.actvtcode == PilihanAktvt && x.kodgl == PilihanGL).ToList();
            }
            else if (PilihanAktvt != "0" && PilihanGL == "0")
            {
                ActvtList = ActvtList.Where(x => x.actvtcode == PilihanAktvt).ToList();
            }
            else if (PilihanAktvt == "0" && PilihanGL != "0")
            {
                ActvtList = ActvtList.Where(x => x.kodgl == PilihanGL).ToList();
            }
            else
            {
                ActvtList = ActvtList.Where(x => x.actvtcode == PilihanAktvt || x.kodgl == PilihanGL).ToList();
            }
            var CustMod_ActvtAndProductcy = new CustMod_ActvtAndProductvy
            {
                CustMod_ActvtList2 = ActvtList,
                sp_RptProduktiviti_Result = RptProduktiviti
            };
            if (MonthList == null && YearList == null && SelectionList == null && PilihanAktvt == null && PilihanGL == null)
            {
                ViewBag.Message = "Sila Pilih Bulan, Tahun, Pekerja, Aktiviti, Dan GL Akaun";
                return View(CustMod_ActvtAndProductcy);
            }
            return View(CustMod_ActvtAndProductcy);*/

            // comment by fitri
            //if (MonthList == null && YearList == null)
            //{
            //    ViewBag.Message = "Sila Pilih Bulan, Tahun Dan Pekerja";
            //    return View(RptProduktiviti);
            //}
            //else
            //{
            //    RptProduktiviti = dbsp.sp_RptProduktiviti(NegaraID, SyarikatID, WilayahID, LadangID, YearList, MonthList, SelectionList, UnitList, AllPeringkatList, StatusList).ToList();
            //    if (RptProduktiviti.Count == 0)
            //    {
            //        ViewBag.Message = "Tiada Rekod";
            //    }
            //    return View(RptProduktiviti);
            //}

            // Add by fitri 30-10-2020
            var GetAtvt = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_KodAktvt).ToList();
            if (MonthList == null && YearList == null)
            {
                ViewBag.Message = "Sila Pilih Bulan, Tahun, Pekerja, Aktiviti, Dan GL Akaun";
                return View();
            }
            //00000
            if ((SelectionList == "" || SelectionList == null) && (PilihanAktvt == "0" || PilihanAktvt == null) && (UnitList == "" || UnitList == null) && (AllPeringkatList == "" || AllPeringkatList == null) && (PilihanGL == "0" || PilihanGL == null))
            {
                var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && GetAtvt.Contains(x.fld_KodAktvt) &&
                                                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                                            .OrderBy(o => o.fld_KodAktvt);
                string[] GetNoPkj = result.Select(s => s.fld_Nopkj).ToArray();
                string[] GetKodActvt = result.Select(s => s.fld_KodAktvt).ToArray();
                var GetKerakyatan = dbr.tbl_Pkjmast.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID && a.fld_Kdaktf == "1" && GetNoPkj.Contains(a.fld_Nopkj)).ToList();
                foreach (var gk in GetKerakyatan)
                {
                    var CodeRakyat = gk.fld_Kdrkyt;
                    if (CodeRakyat == "MA") // Malaysia
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PT").ToList();
                        ViewBag.GLKod = GetGLKod.Distinct();
                    }
                    else // Asing
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PA").ToList();
                        ViewBag.GLKod = GetGLKod.Distinct();
                    }
                }

                ViewBag.DataCount = result.Count();
                if (result.Count() == 0)
                {
                    ViewBag.Message = "Tiada Rekod";
                }
                return View(result);
            }
            // 10000 
            else if ((SelectionList != "" || SelectionList != null) && (PilihanAktvt == "0" || PilihanAktvt == null) && (UnitList == "" || UnitList == null) && (AllPeringkatList == "" || AllPeringkatList == null) && (PilihanGL == "0" || PilihanGL == null))
            {
                var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_NegaraID == NegaraID &&
                                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                                            x.fld_Nopkj == SelectionList).OrderBy(o => o.fld_KodAktvt);

                string[] GetNoPkj = result.Select(s => s.fld_Nopkj).ToArray();
                string[] GetKodActvt = result.Select(s => s.fld_KodAktvt).ToArray();
                var GetKerakyatan = dbr.tbl_Pkjmast.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID && a.fld_Kdaktf == "1" && GetNoPkj.Contains(a.fld_Nopkj)).ToList();
                foreach (var gk in GetKerakyatan)
                {
                    var CodeRakyat = gk.fld_Kdrkyt;
                    if (CodeRakyat == "MA") // Malaysia
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PT").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                    else // Asing
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PA").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                }
                ViewBag.DataCount = result.Count();
                if (result.Count() == 0)
                {
                    ViewBag.Message = "Tiada Rekod";
                }
                return View(result);
            }
            //01000
            else if ((SelectionList == "" || SelectionList == null) && PilihanAktvt != "0" && (UnitList == "" || UnitList == null) && (AllPeringkatList == "" || AllPeringkatList == null) && (PilihanGL == "0" || PilihanGL == null))
            {
                var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_NegaraID == NegaraID &&
                                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                                            x.fld_KodAktvt == PilihanAktvt).OrderBy(o => o.fld_KodAktvt);

                string[] GetNoPkj = result.Select(s => s.fld_Nopkj).ToArray();
                string[] GetKodActvt = result.Select(s => s.fld_KodAktvt).ToArray();
                var GetKerakyatan = dbr.tbl_Pkjmast.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID && a.fld_Kdaktf == "1" && GetNoPkj.Contains(a.fld_Nopkj)).ToList();
                foreach (var gk in GetKerakyatan)
                {
                    var CodeRakyat = gk.fld_Kdrkyt;
                    if (CodeRakyat == "MA") // Malaysia
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PT").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                    else // Asing
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PA").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                }
                ViewBag.DataCount = result.Count();
                if (result.Count() == 0)
                {
                    ViewBag.Message = "Tiada Rekod";
                }
                return View(result);
            }
            //00100
            else if ((SelectionList == "" || SelectionList == null) && (PilihanAktvt == "0" || PilihanAktvt == null) && UnitList != "" && (AllPeringkatList == "" || AllPeringkatList == null) && (PilihanGL == "0" || PilihanGL == null))
            {
                var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_NegaraID == NegaraID &&
                                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                                            x.fld_Unit == UnitList).OrderBy(o => o.fld_KodAktvt);

                string[] GetNoPkj = result.Select(s => s.fld_Nopkj).ToArray();
                string[] GetKodActvt = result.Select(s => s.fld_KodAktvt).ToArray();
                var GetKerakyatan = dbr.tbl_Pkjmast.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID && a.fld_Kdaktf == "1" && GetNoPkj.Contains(a.fld_Nopkj)).ToList();
                foreach (var gk in GetKerakyatan)
                {
                    var CodeRakyat = gk.fld_Kdrkyt;
                    if (CodeRakyat == "MA") // Malaysia
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PT").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                    else // Asing
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PA").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                }
                ViewBag.DataCount = result.Count();
                if (result.Count() == 0)
                {
                    ViewBag.Message = "Tiada Rekod";
                }
                return View(result);
            }
            //00010
            else if ((SelectionList == "" || SelectionList == null) && (PilihanAktvt == "0" || PilihanAktvt == null) && (UnitList == "" || UnitList == null) && AllPeringkatList != "" && (PilihanGL == "0" || PilihanGL == null))
            {
                var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_NegaraID == NegaraID &&
                                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                                            x.fld_KodPkt == AllPeringkatList).OrderBy(o => o.fld_KodAktvt);

                string[] GetNoPkj = result.Select(s => s.fld_Nopkj).ToArray();
                string[] GetKodActvt = result.Select(s => s.fld_KodAktvt).ToArray();
                var GetKerakyatan = dbr.tbl_Pkjmast.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID && a.fld_Kdaktf == "1" && GetNoPkj.Contains(a.fld_Nopkj)).ToList();
                foreach (var gk in GetKerakyatan)
                {
                    var CodeRakyat = gk.fld_Kdrkyt;
                    if (CodeRakyat == "MA") // Malaysia
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PT").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                    else // Asing
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PA").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                }
                ViewBag.DataCount = result.Count();
                if (result.Count() == 0)
                {
                    ViewBag.Message = "Tiada Rekod";
                }
                return View(result);
            }
            //00001
            else if ((SelectionList == "" || SelectionList == null) && (PilihanAktvt == "0" || PilihanAktvt == null) && (UnitList == "" || UnitList == null) && (AllPeringkatList == "" || AllPeringkatList == null) && PilihanGL != "0")
            {
                var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_NegaraID == NegaraID &&
                                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                                            GetAtvt.Contains(x.fld_KodAktvt)).OrderBy(o => o.fld_KodAktvt);

                string[] GetNoPkj = result.Select(s => s.fld_Nopkj).ToArray();
                string[] GetKodActvt = result.Select(s => s.fld_KodAktvt).ToArray();
                var GetKerakyatan = dbr.tbl_Pkjmast.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID && a.fld_Kdaktf == "1" && GetNoPkj.Contains(a.fld_Nopkj)).ToList();
                foreach (var gk in GetKerakyatan)
                {
                    var CodeRakyat = gk.fld_Kdrkyt;
                    if (CodeRakyat == "MA") // Malaysia
                    {   //edit by fitri 5-11-2020
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_KodGL == PilihanGL && a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PT").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                    else // Asing
                    {   //edit by fitri 5-11-2020
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_KodGL == PilihanGL && a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PA").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                }
                ViewBag.DataCount = result.Count();
                if (result.Count() == 0)
                {
                    ViewBag.Message = "Tiada Rekod";
                }
                return View(result);
            }
            //11000
            else if (SelectionList != "" && PilihanAktvt != "0" && (UnitList == "" || UnitList == null) && (AllPeringkatList == "" || AllPeringkatList == null) && (PilihanGL == "0" || PilihanGL == null))
            {
                var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_NegaraID == NegaraID &&
                                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                                            x.fld_Nopkj == SelectionList && x.fld_KodAktvt == PilihanAktvt).OrderBy(o => o.fld_KodAktvt);

                string[] GetNoPkj = result.Select(s => s.fld_Nopkj).ToArray();
                string[] GetKodActvt = result.Select(s => s.fld_KodAktvt).ToArray();
                var GetKerakyatan = dbr.tbl_Pkjmast.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID && a.fld_Kdaktf == "1" && GetNoPkj.Contains(a.fld_Nopkj)).ToList();
                foreach (var gk in GetKerakyatan)
                {
                    var CodeRakyat = gk.fld_Kdrkyt;
                    if (CodeRakyat == "MA") // Malaysia
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PT").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                    else // Asing
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PA").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                }
                ViewBag.DataCount = result.Count();
                if (result.Count() == 0)
                {
                    ViewBag.Message = "Tiada Rekod";
                }
                return View(result);
            }
            //10100
            else if (SelectionList != "" && (PilihanAktvt == "0" || PilihanAktvt == null) && UnitList != "" && (AllPeringkatList == "" || AllPeringkatList == null) && (PilihanGL == "0" || PilihanGL == null))
            {
                var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_NegaraID == NegaraID &&
                                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                                            x.fld_Nopkj == SelectionList && x.fld_Unit == UnitList).OrderBy(o => o.fld_KodAktvt);

                string[] GetNoPkj = result.Select(s => s.fld_Nopkj).ToArray();
                string[] GetKodActvt = result.Select(s => s.fld_KodAktvt).ToArray();
                var GetKerakyatan = dbr.tbl_Pkjmast.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID && a.fld_Kdaktf == "1" && GetNoPkj.Contains(a.fld_Nopkj)).ToList();
                foreach (var gk in GetKerakyatan)
                {
                    var CodeRakyat = gk.fld_Kdrkyt;
                    if (CodeRakyat == "MA") // Malaysia
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PT").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                    else // Asing
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PA").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                }
                ViewBag.DataCount = result.Count();
                if (result.Count() == 0)
                {
                    ViewBag.Message = "Tiada Rekod";
                }
                return View(result);
            }
            //10010
            else if (SelectionList != "" && (PilihanAktvt == "0" || PilihanAktvt == null) && (UnitList == "" || UnitList == null) && AllPeringkatList != "" && (PilihanGL == "0" || PilihanGL == null))
            {
                var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_NegaraID == NegaraID &&
                                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                                            x.fld_Nopkj == SelectionList && x.fld_KodPkt == AllPeringkatList).OrderBy(o => o.fld_KodAktvt);

                string[] GetNoPkj = result.Select(s => s.fld_Nopkj).ToArray();
                string[] GetKodActvt = result.Select(s => s.fld_KodAktvt).ToArray();
                var GetKerakyatan = dbr.tbl_Pkjmast.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID && a.fld_Kdaktf == "1" && GetNoPkj.Contains(a.fld_Nopkj)).ToList();
                foreach (var gk in GetKerakyatan)
                {
                    var CodeRakyat = gk.fld_Kdrkyt;
                    if (CodeRakyat == "MA") // Malaysia
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PT").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                    else // Asing
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PA").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                }
                ViewBag.DataCount = result.Count();
                if (result.Count() == 0)
                {
                    ViewBag.Message = "Tiada Rekod";
                }
                return View(result);
            }
            //10001
            else if (SelectionList != "" && (PilihanAktvt == "0" || PilihanAktvt == null) && (UnitList == "" || UnitList == null) && (AllPeringkatList == "" || AllPeringkatList == null) && PilihanGL != "0")
            {
                var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_NegaraID == NegaraID &&
                                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                                            x.fld_Nopkj == SelectionList).OrderBy(o => o.fld_KodAktvt);

                string[] GetNoPkj = result.Select(s => s.fld_Nopkj).ToArray();
                string[] GetKodActvt = result.Select(s => s.fld_KodAktvt).ToArray();
                var GetKerakyatan = dbr.tbl_Pkjmast.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID && a.fld_Kdaktf == "1" && GetNoPkj.Contains(a.fld_Nopkj)).ToList();
                foreach (var gk in GetKerakyatan)
                {
                    var CodeRakyat = gk.fld_Kdrkyt;
                    if (CodeRakyat == "MA") // Malaysia
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_KodGL == PilihanGL && a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PT").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                    else // Asing
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_KodGL == PilihanGL && a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PA").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                }
                ViewBag.DataCount = result.Count();
                if (result.Count() == 0)
                {
                    ViewBag.Message = "Tiada Rekod";
                }
                return View(result);
            }
            //01100
            else if ((SelectionList == "" || SelectionList == null) && PilihanAktvt != "0" && UnitList != "" && (AllPeringkatList == "" || AllPeringkatList == null) && (PilihanGL == "0" || PilihanGL == null))
            {
                var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_NegaraID == NegaraID &&
                                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                                            x.fld_KodAktvt == PilihanAktvt && x.fld_Unit == UnitList).OrderBy(o => o.fld_KodAktvt);

                string[] GetNoPkj = result.Select(s => s.fld_Nopkj).ToArray();
                string[] GetKodActvt = result.Select(s => s.fld_KodAktvt).ToArray();
                var GetKerakyatan = dbr.tbl_Pkjmast.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID && a.fld_Kdaktf == "1" && GetNoPkj.Contains(a.fld_Nopkj)).ToList();
                foreach (var gk in GetKerakyatan)
                {
                    var CodeRakyat = gk.fld_Kdrkyt;
                    if (CodeRakyat == "MA") // Malaysia
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PT").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                    else // Asing
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PA").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                }
                ViewBag.DataCount = result.Count();
                if (result.Count() == 0)
                {
                    ViewBag.Message = "Tiada Rekod";
                }
                return View(result);
            }
            //01010
            else if ((SelectionList == "" || SelectionList == null) && PilihanAktvt != "0" && (UnitList == "" || UnitList == null) && AllPeringkatList != "" && (PilihanGL == "0" || PilihanGL == null))
            {
                var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_NegaraID == NegaraID &&
                                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                                            x.fld_KodAktvt == PilihanAktvt && x.fld_KodPkt == AllPeringkatList).OrderBy(o => o.fld_KodAktvt);

                string[] GetNoPkj = result.Select(s => s.fld_Nopkj).ToArray();
                string[] GetKodActvt = result.Select(s => s.fld_KodAktvt).ToArray();
                var GetKerakyatan = dbr.tbl_Pkjmast.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID && a.fld_Kdaktf == "1" && GetNoPkj.Contains(a.fld_Nopkj)).ToList();
                foreach (var gk in GetKerakyatan)
                {
                    var CodeRakyat = gk.fld_Kdrkyt;
                    if (CodeRakyat == "MA") // Malaysia
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PT").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                    else // Asing
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PA").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                }
                ViewBag.DataCount = result.Count();
                if (result.Count() == 0)
                {
                    ViewBag.Message = "Tiada Rekod";
                }
                return View(result);
            }
            //01001
            else if ((SelectionList == "" || SelectionList == null) && PilihanAktvt != "0" && (UnitList == "" || UnitList == null) && (AllPeringkatList == "" || AllPeringkatList == null) && PilihanGL != "0")
            {
                var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_NegaraID == NegaraID &&
                                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                                            x.fld_KodAktvt == PilihanAktvt).OrderBy(o => o.fld_KodAktvt);

                string[] GetNoPkj = result.Select(s => s.fld_Nopkj).ToArray();
                string[] GetKodActvt = result.Select(s => s.fld_KodAktvt).ToArray();
                var GetKerakyatan = dbr.tbl_Pkjmast.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID && a.fld_Kdaktf == "1" && GetNoPkj.Contains(a.fld_Nopkj)).ToList();
                foreach (var gk in GetKerakyatan)
                {
                    var CodeRakyat = gk.fld_Kdrkyt;
                    if (CodeRakyat == "MA") // Malaysia
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PT").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                    else // Asing
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PA").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                }
                ViewBag.DataCount = result.Count();
                if (result.Count() == 0)
                {
                    ViewBag.Message = "Tiada Rekod";
                }
                return View(result);
            }
            //00110
            else if ((SelectionList == "" || SelectionList == null) && (PilihanAktvt == "0" || PilihanAktvt == null) && UnitList != "" && AllPeringkatList != "" && (PilihanGL == "0" || PilihanGL == null))
            {
                var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_NegaraID == NegaraID &&
                                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                                            x.fld_Unit == UnitList && x.fld_KodPkt == AllPeringkatList).OrderBy(o => o.fld_KodAktvt);

                string[] GetNoPkj = result.Select(s => s.fld_Nopkj).ToArray();
                string[] GetKodActvt = result.Select(s => s.fld_KodAktvt).ToArray();
                var GetKerakyatan = dbr.tbl_Pkjmast.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID && a.fld_Kdaktf == "1" && GetNoPkj.Contains(a.fld_Nopkj)).ToList();
                foreach (var gk in GetKerakyatan)
                {
                    var CodeRakyat = gk.fld_Kdrkyt;
                    if (CodeRakyat == "MA") // Malaysia
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PT").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                    else // Asing
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PA").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                }
                ViewBag.DataCount = result.Count();
                if (result.Count() == 0)
                {
                    ViewBag.Message = "Tiada Rekod";
                }
                return View(result);
            }
            //00101
            else if ((SelectionList == "" || SelectionList == null) && (PilihanAktvt == "0" || PilihanAktvt == null) && UnitList != "" && (AllPeringkatList == "" || AllPeringkatList == null) && PilihanGL != "0")
            {
                var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_NegaraID == NegaraID &&
                                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                                            x.fld_Unit == UnitList).OrderBy(o => o.fld_KodAktvt);

                string[] GetNoPkj = result.Select(s => s.fld_Nopkj).ToArray();
                string[] GetKodActvt = result.Select(s => s.fld_KodAktvt).ToArray();
                var GetKerakyatan = dbr.tbl_Pkjmast.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID && a.fld_Kdaktf == "1" && GetNoPkj.Contains(a.fld_Nopkj)).ToList();
                foreach (var gk in GetKerakyatan)
                {
                    var CodeRakyat = gk.fld_Kdrkyt;
                    if (CodeRakyat == "MA") // Malaysia
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PT").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                    else // Asing
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PA").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                }
                ViewBag.DataCount = result.Count();
                if (result.Count() == 0)
                {
                    ViewBag.Message = "Tiada Rekod";
                }
                return View(result);
            }
            //00011
            else if ((SelectionList == "" || SelectionList == null) && (PilihanAktvt == "0" || PilihanAktvt == null) && (UnitList == "" || UnitList == null) && AllPeringkatList != "" && PilihanGL != "0")
            {
                var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_NegaraID == NegaraID &&
                                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                                            x.fld_KodPkt == AllPeringkatList).OrderBy(o => o.fld_KodAktvt);

                string[] GetNoPkj = result.Select(s => s.fld_Nopkj).ToArray();
                string[] GetKodActvt = result.Select(s => s.fld_KodAktvt).ToArray();
                var GetKerakyatan = dbr.tbl_Pkjmast.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID && a.fld_Kdaktf == "1" && GetNoPkj.Contains(a.fld_Nopkj)).ToList();
                foreach (var gk in GetKerakyatan)
                {
                    var CodeRakyat = gk.fld_Kdrkyt;
                    if (CodeRakyat == "MA") // Malaysia
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PT").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                    else // Asing
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PA").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                }
                ViewBag.DataCount = result.Count();
                if (result.Count() == 0)
                {
                    ViewBag.Message = "Tiada Rekod";
                }
                return View(result);
            }

            //00111
            else if ((SelectionList == "" || SelectionList == null) && (PilihanAktvt == "0" || PilihanAktvt == null) && UnitList != "" && AllPeringkatList != "" && PilihanGL != "0")
            {
                var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_NegaraID == NegaraID &&
                                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                                            x.fld_Unit == UnitList && x.fld_KodPkt == AllPeringkatList).OrderBy(o => o.fld_KodAktvt);

                string[] GetNoPkj = result.Select(s => s.fld_Nopkj).ToArray();
                string[] GetKodActvt = result.Select(s => s.fld_KodAktvt).ToArray();
                var GetKerakyatan = dbr.tbl_Pkjmast.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID && a.fld_Kdaktf == "1" && GetNoPkj.Contains(a.fld_Nopkj)).ToList();
                foreach (var gk in GetKerakyatan)
                {
                    var CodeRakyat = gk.fld_Kdrkyt;
                    if (CodeRakyat == "MA") // Malaysia
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PT").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                    else // Asing
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PA").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                }
                ViewBag.DataCount = result.Count();
                if (result.Count() == 0)
                {
                    ViewBag.Message = "Tiada Rekod";
                }
                return View(result);
            }
            //01101
            else if ((SelectionList == "" || SelectionList == null) && PilihanAktvt != "0" && UnitList != "" && (AllPeringkatList == "" || AllPeringkatList == null) && PilihanGL != "0")
            {
                var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_NegaraID == NegaraID &&
                                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                                            x.fld_KodAktvt == PilihanAktvt && x.fld_Unit == UnitList).OrderBy(o => o.fld_KodAktvt);

                string[] GetNoPkj = result.Select(s => s.fld_Nopkj).ToArray();
                string[] GetKodActvt = result.Select(s => s.fld_KodAktvt).ToArray();
                var GetKerakyatan = dbr.tbl_Pkjmast.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID && a.fld_Kdaktf == "1" && GetNoPkj.Contains(a.fld_Nopkj)).ToList();
                foreach (var gk in GetKerakyatan)
                {
                    var CodeRakyat = gk.fld_Kdrkyt;
                    if (CodeRakyat == "MA") // Malaysia
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PT").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                    else // Asing
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PA").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                }
                ViewBag.DataCount = result.Count();
                if (result.Count() == 0)
                {
                    ViewBag.Message = "Tiada Rekod";
                }
                return View(result);
            }
            //01011
            else if ((SelectionList == "" || SelectionList == null) && PilihanAktvt != "0" && (UnitList == "" || UnitList == null) && AllPeringkatList != "" && PilihanGL != "0")
            {
                var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_NegaraID == NegaraID &&
                                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                                            x.fld_KodAktvt == PilihanAktvt && x.fld_KodPkt == AllPeringkatList).OrderBy(o => o.fld_KodAktvt);

                string[] GetNoPkj = result.Select(s => s.fld_Nopkj).ToArray();
                string[] GetKodActvt = result.Select(s => s.fld_KodAktvt).ToArray();
                var GetKerakyatan = dbr.tbl_Pkjmast.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID && a.fld_Kdaktf == "1" && GetNoPkj.Contains(a.fld_Nopkj)).ToList();
                foreach (var gk in GetKerakyatan)
                {
                    var CodeRakyat = gk.fld_Kdrkyt;
                    if (CodeRakyat == "MA") // Malaysia
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PT").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                    else // Asing
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PA").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                }
                ViewBag.DataCount = result.Count();
                if (result.Count() == 0)
                {
                    ViewBag.Message = "Tiada Rekod";
                }
                return View(result);
            }
            //01110
            else if ((SelectionList == "" || SelectionList == null) && PilihanAktvt != "0" && UnitList != "" && AllPeringkatList != "" && (PilihanGL == "0" || PilihanGL == null))
            {
                var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_NegaraID == NegaraID &&
                                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                                            x.fld_KodAktvt == PilihanAktvt && x.fld_Unit == UnitList && x.fld_KodPkt == AllPeringkatList).OrderBy(o => o.fld_KodAktvt);

                string[] GetNoPkj = result.Select(s => s.fld_Nopkj).ToArray();
                string[] GetKodActvt = result.Select(s => s.fld_KodAktvt).ToArray();
                var GetKerakyatan = dbr.tbl_Pkjmast.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID && a.fld_Kdaktf == "1" && GetNoPkj.Contains(a.fld_Nopkj)).ToList();
                foreach (var gk in GetKerakyatan)
                {
                    var CodeRakyat = gk.fld_Kdrkyt;
                    if (CodeRakyat == "MA") // Malaysia
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PT").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                    else // Asing
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PA").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                }
                ViewBag.DataCount = result.Count();
                if (result.Count() == 0)
                {
                    ViewBag.Message = "Tiada Rekod";
                }
                return View(result);
            }
            //10011
            else if (SelectionList != "" && (PilihanAktvt == "0" || PilihanAktvt == null) && (UnitList == "" || UnitList == null) && AllPeringkatList != "" && PilihanGL != "0")
            {
                var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_NegaraID == NegaraID &&
                                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                                            x.fld_Nopkj == SelectionList && x.fld_KodPkt == AllPeringkatList).OrderBy(o => o.fld_KodAktvt);

                string[] GetNoPkj = result.Select(s => s.fld_Nopkj).ToArray();
                string[] GetKodActvt = result.Select(s => s.fld_KodAktvt).ToArray();
                var GetKerakyatan = dbr.tbl_Pkjmast.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID && a.fld_Kdaktf == "1" && GetNoPkj.Contains(a.fld_Nopkj)).ToList();
                foreach (var gk in GetKerakyatan)
                {
                    var CodeRakyat = gk.fld_Kdrkyt;
                    if (CodeRakyat == "MA") // Malaysia
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PT").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                    else // Asing
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PA").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                }
                ViewBag.DataCount = result.Count();
                if (result.Count() == 0)
                {
                    ViewBag.Message = "Tiada Rekod";
                }
                return View(result);
            }
            //10101
            else if (SelectionList != "" && (PilihanAktvt == "0" || PilihanAktvt == null) && UnitList != "" && (AllPeringkatList == "" || AllPeringkatList == null) && PilihanGL != "0")
            {
                var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_NegaraID == NegaraID &&
                                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                                            x.fld_Nopkj == SelectionList && x.fld_Unit == UnitList).OrderBy(o => o.fld_KodAktvt);

                string[] GetNoPkj = result.Select(s => s.fld_Nopkj).ToArray();
                string[] GetKodActvt = result.Select(s => s.fld_KodAktvt).ToArray();
                var GetKerakyatan = dbr.tbl_Pkjmast.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID && a.fld_Kdaktf == "1" && GetNoPkj.Contains(a.fld_Nopkj)).ToList();
                foreach (var gk in GetKerakyatan)
                {
                    var CodeRakyat = gk.fld_Kdrkyt;
                    if (CodeRakyat == "MA") // Malaysia
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PT").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                    else // Asing
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PA").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                }
                ViewBag.DataCount = result.Count();
                if (result.Count() == 0)
                {
                    ViewBag.Message = "Tiada Rekod";
                }
                return View(result);
            }
            //10110
            else if (SelectionList != "" && (PilihanAktvt == "0" || PilihanAktvt == null) && UnitList != "" && AllPeringkatList != "" && (PilihanGL == "0" || PilihanGL == null))
            {
                var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_NegaraID == NegaraID &&
                                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                                            x.fld_Nopkj == SelectionList && x.fld_Unit == UnitList && x.fld_KodPkt == AllPeringkatList).OrderBy(o => o.fld_KodAktvt);

                string[] GetNoPkj = result.Select(s => s.fld_Nopkj).ToArray();
                string[] GetKodActvt = result.Select(s => s.fld_KodAktvt).ToArray();
                var GetKerakyatan = dbr.tbl_Pkjmast.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID && a.fld_Kdaktf == "1" && GetNoPkj.Contains(a.fld_Nopkj)).ToList();
                foreach (var gk in GetKerakyatan)
                {
                    var CodeRakyat = gk.fld_Kdrkyt;
                    if (CodeRakyat == "MA") // Malaysia
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PT").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                    else // Asing
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PA").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                }
                ViewBag.DataCount = result.Count();
                if (result.Count() == 0)
                {
                    ViewBag.Message = "Tiada Rekod";
                }
                return View(result);
            }
            //11001
            else if ((SelectionList != "" && PilihanAktvt != "0") && ((UnitList == "" || UnitList == null) && (AllPeringkatList == "" || AllPeringkatList == null)) && PilihanGL != "0")
            {
                var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_NegaraID == NegaraID &&
                                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                                            x.fld_Nopkj == SelectionList && x.fld_KodAktvt == PilihanAktvt).OrderBy(o => o.fld_KodAktvt);

                string[] GetNoPkj = result.Select(s => s.fld_Nopkj).ToArray();
                string[] GetKodActvt = result.Select(s => s.fld_KodAktvt).ToArray();
                var GetKerakyatan = dbr.tbl_Pkjmast.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID && a.fld_Kdaktf == "1" && GetNoPkj.Contains(a.fld_Nopkj)).ToList();
                foreach (var gk in GetKerakyatan)
                {
                    var CodeRakyat = gk.fld_Kdrkyt;
                    if (CodeRakyat == "MA") // Malaysia
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PT").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                    else // Asing
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PA").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                }
                ViewBag.DataCount = result.Count();
                if (result.Count() == 0)
                {
                    ViewBag.Message = "Tiada Rekod";
                }
                return View(result);
            }
            //11010
            else if (SelectionList != "" && PilihanAktvt != "0" && (UnitList == "" || UnitList == null) && AllPeringkatList != "" && (PilihanGL == "0" || PilihanGL == null))
            {
                var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_NegaraID == NegaraID &&
                                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                                            x.fld_Nopkj == SelectionList && x.fld_KodAktvt == PilihanAktvt && x.fld_KodPkt == AllPeringkatList).OrderBy(o => o.fld_KodAktvt);

                string[] GetNoPkj = result.Select(s => s.fld_Nopkj).ToArray();
                string[] GetKodActvt = result.Select(s => s.fld_KodAktvt).ToArray();
                var GetKerakyatan = dbr.tbl_Pkjmast.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID && a.fld_Kdaktf == "1" && GetNoPkj.Contains(a.fld_Nopkj)).ToList();
                foreach (var gk in GetKerakyatan)
                {
                    var CodeRakyat = gk.fld_Kdrkyt;
                    if (CodeRakyat == "MA") // Malaysia
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PT").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                    else // Asing
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PA").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                }
                ViewBag.DataCount = result.Count();
                if (result.Count() == 0)
                {
                    ViewBag.Message = "Tiada Rekod";
                }
                return View(result);
            }
            //11100
            else if (SelectionList != "" && PilihanAktvt != "0" && UnitList != "" && (AllPeringkatList == "" || AllPeringkatList == null) && (PilihanGL == "0" || PilihanGL == null))
            {
                var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_NegaraID == NegaraID &&
                                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                                            x.fld_Nopkj == SelectionList && x.fld_KodAktvt == PilihanAktvt && x.fld_Unit == UnitList).OrderBy(o => o.fld_KodAktvt);

                string[] GetNoPkj = result.Select(s => s.fld_Nopkj).ToArray();
                string[] GetKodActvt = result.Select(s => s.fld_KodAktvt).ToArray();
                var GetKerakyatan = dbr.tbl_Pkjmast.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID && a.fld_Kdaktf == "1" && GetNoPkj.Contains(a.fld_Nopkj)).ToList();
                foreach (var gk in GetKerakyatan)
                {
                    var CodeRakyat = gk.fld_Kdrkyt;
                    if (CodeRakyat == "MA") // Malaysia
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PT").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                    else // Asing
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PA").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                }
                ViewBag.DataCount = result.Count();
                if (result.Count() == 0)
                {
                    ViewBag.Message = "Tiada Rekod";
                }
                return View(result);
            }
            //11110
            else if (SelectionList != "" && PilihanAktvt != "0" && UnitList != "" && AllPeringkatList != "" && (PilihanGL == "0" || PilihanGL == null))
            {
                var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_NegaraID == NegaraID &&
                                                         x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                                         x.fld_Nopkj == SelectionList && x.fld_KodAktvt == PilihanAktvt && x.fld_Unit == UnitList && x.fld_KodPkt == AllPeringkatList)
                                                        .OrderBy(o => o.fld_KodAktvt);

                string[] GetNoPkj = result.Select(s => s.fld_Nopkj).ToArray();
                string[] GetKodActvt = result.Select(s => s.fld_KodAktvt).ToArray();
                var GetKerakyatan = dbr.tbl_Pkjmast.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID && a.fld_Kdaktf == "1" && GetNoPkj.Contains(a.fld_Nopkj)).ToList();
                foreach (var gk in GetKerakyatan)
                {
                    var CodeRakyat = gk.fld_Kdrkyt;
                    if (CodeRakyat == "MA") // Malaysia
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PT").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                    else // Asing
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PA").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                }
                ViewBag.DataCount = result.Count();
                if (result.Count() == 0)
                {
                    ViewBag.Message = "Tiada Rekod";
                }
                return View(result);
            }
            //11101
            else if (SelectionList != "" && PilihanAktvt != "0" && UnitList != "" && (AllPeringkatList == "" || AllPeringkatList == null) && PilihanGL != "0")
            {
                var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_NegaraID == NegaraID &&
                                                         x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                                         x.fld_Nopkj == SelectionList && x.fld_KodAktvt == PilihanAktvt && x.fld_Unit == UnitList).OrderBy(o => o.fld_KodAktvt);

                string[] GetNoPkj = result.Select(s => s.fld_Nopkj).ToArray();
                string[] GetKodActvt = result.Select(s => s.fld_KodAktvt).ToArray();
                var GetKerakyatan = dbr.tbl_Pkjmast.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID && a.fld_Kdaktf == "1" && GetNoPkj.Contains(a.fld_Nopkj)).ToList();
                foreach (var gk in GetKerakyatan)
                {
                    var CodeRakyat = gk.fld_Kdrkyt;
                    if (CodeRakyat == "MA") // Malaysia
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PT").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                    else // Asing
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PA").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                }
                ViewBag.DataCount = result.Count();
                if (result.Count() == 0)
                {
                    ViewBag.Message = "Tiada Rekod";
                }
                return View(result);
            }
            //11011
            else if (SelectionList != "" && PilihanAktvt != "0" && (UnitList == "" || UnitList == null) && AllPeringkatList != "" && PilihanGL != "0")
            {
                var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_NegaraID == NegaraID &&
                                                         x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                                         x.fld_Nopkj == SelectionList && x.fld_KodAktvt == PilihanAktvt && x.fld_KodPkt == AllPeringkatList)
                                                        .OrderBy(o => o.fld_KodAktvt);

                string[] GetNoPkj = result.Select(s => s.fld_Nopkj).ToArray();
                string[] GetKodActvt = result.Select(s => s.fld_KodAktvt).ToArray();
                var GetKerakyatan = dbr.tbl_Pkjmast.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID && a.fld_Kdaktf == "1" && GetNoPkj.Contains(a.fld_Nopkj)).ToList();
                foreach (var gk in GetKerakyatan)
                {
                    var CodeRakyat = gk.fld_Kdrkyt;
                    if (CodeRakyat == "MA") // Malaysia
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PT").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                    else // Asing
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PA").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                }
                ViewBag.DataCount = result.Count();
                if (result.Count() == 0)
                {
                    ViewBag.Message = "Tiada Rekod";
                }
                return View(result);
            }
            //10111
            else if (SelectionList != "" && (PilihanAktvt == "0" || PilihanAktvt == null) && UnitList != "" && AllPeringkatList != "" && PilihanGL != "0")
            {
                var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_NegaraID == NegaraID &&
                                                         x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                                         x.fld_Nopkj == SelectionList && x.fld_Unit == UnitList && x.fld_KodPkt == AllPeringkatList).OrderBy(o => o.fld_KodAktvt);

                string[] GetNoPkj = result.Select(s => s.fld_Nopkj).ToArray();
                string[] GetKodActvt = result.Select(s => s.fld_KodAktvt).ToArray();
                var GetKerakyatan = dbr.tbl_Pkjmast.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID && a.fld_Kdaktf == "1" && GetNoPkj.Contains(a.fld_Nopkj)).ToList();
                foreach (var gk in GetKerakyatan)
                {
                    var CodeRakyat = gk.fld_Kdrkyt;
                    if (CodeRakyat == "MA") // Malaysia
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PT").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                    else // Asing
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PA").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                }
                ViewBag.DataCount = result.Count();
                if (result.Count() == 0)
                {
                    ViewBag.Message = "Tiada Rekod";
                }
                return View(result);
            }
            //01111
            else if ((SelectionList == "" || SelectionList == null) && PilihanAktvt != "0" && UnitList != "" && AllPeringkatList != "" && PilihanGL != "0")
            {
                var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_NegaraID == NegaraID &&
                                                         x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                                         x.fld_KodAktvt == PilihanAktvt && x.fld_Unit == UnitList && x.fld_KodPkt == AllPeringkatList).OrderBy(o => o.fld_KodAktvt);

                string[] GetNoPkj = result.Select(s => s.fld_Nopkj).ToArray();
                string[] GetKodActvt = result.Select(s => s.fld_KodAktvt).ToArray();
                var GetKerakyatan = dbr.tbl_Pkjmast.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID && a.fld_Kdaktf == "1" && GetNoPkj.Contains(a.fld_Nopkj)).ToList();
                foreach (var gk in GetKerakyatan)
                {
                    var CodeRakyat = gk.fld_Kdrkyt;
                    if (CodeRakyat == "MA") // Malaysia
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PT").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                    else // Asing
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PA").ToList();
                        ViewBag.GLKod = GetGLKod;
                    }
                }
                ViewBag.DataCount = result.Count();
                if (result.Count() == 0)
                {
                    ViewBag.Message = "Tiada Rekod";
                }
                return View(result);
            }
            //11111
            else
            {
                var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_NegaraID == NegaraID &&
                                                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                                                x.fld_Nopkj == SelectionList && x.fld_KodAktvt == PilihanAktvt && x.fld_Unit == UnitList &&
                                                                x.fld_KodPkt == AllPeringkatList).OrderBy(o => o.fld_KodAktvt);

                string[] GetNoPkj = result.Select(s => s.fld_Nopkj).ToArray();
                string[] GetKodActvt = result.Select(s => s.fld_KodAktvt).ToArray();
                var GetKerakyatan = dbr.tbl_Pkjmast.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID && a.fld_Kdaktf == "1" && GetNoPkj.Contains(a.fld_Nopkj)).ToList();
                foreach (var gk in GetKerakyatan)
                {
                    var CodeRakyat = gk.fld_Kdrkyt;
                    if (CodeRakyat == "MA") // Malaysia
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PT").ToList();
                        ViewBag.GLKod = GetGLKod.DistinctBy(d => d.fld_KodGL);
                    }
                    else // Asing
                    {
                        var GetGLKod = db.tbl_MapGL.Where(a => a.fld_NegaraID == NegaraID && a.fld_SyarikatID == SyarikatID && a.fld_Deleted == false && GetKodActvt.Contains(a.fld_KodAktvt) && a.fld_Paysheet == "PA").ToList();
                        ViewBag.GLKod = GetGLKod.DistinctBy(d => d.fld_KodGL);
                    }
                }
                ViewBag.DataCount = result.Count();
                if (result.Count() == 0)
                {
                    ViewBag.Message = "Tiada Rekod";
                }
                return View(result);
            }

        }

        public ActionResult _ProductivityRptAdvanceSearch()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var statusList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();
            statusList.Insert(0, (new SelectListItem { Text = "Semua", Value = "" }));

            ViewBag.StatusList = statusList;

            return View();
        }

        public ActionResult KwspSocsoMonthlyReport()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int range = int.Parse(GetConfig.GetData("yeardisplay"));
            int startyear = DateTime.Now.AddYears(-range).Year;
            int currentyear = DateTime.Now.Year;
            DateTime selectdate = DateTime.Now;

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

            List<SelectListItem> GroupList = new List<SelectListItem>();
            GroupList = new SelectList(dbr.tbl_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).OrderBy(o => o.fld_KodKumpulan).Select(s => new SelectListItem { Value = s.fld_KumpulanID.ToString(), Text = s.fld_KodKumpulan }).Distinct(), "Value", "Text").ToList();
            GroupList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            List<SelectListItem> WorkerList = new List<SelectListItem>();
            WorkerList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nama).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + " - " + s.fld_Nama }).Distinct(), "Value", "Text").ToList();
            WorkerList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.YearList = yearlist;
            ViewBag.GroupList = GroupList;
            ViewBag.WorkerList = WorkerList;
            return View();
        }

        public ActionResult KwspSocsoMonthlyReportDetail(int? YearList, string GroupList, string WorkerList, string print)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.YearSelection = YearList;
            ViewBag.Print = print;

            int groupID = Convert.ToInt32(GroupList);
            //int YearID = Convert.ToInt32(YearList);

            if (YearList == null && GroupList == null && WorkerList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChooseAips;
                return View();
            }

            if (GroupList == "0")
            {
                var result = dbr.vw_rptKwspSocso.Where(x => x.fld_Year == YearList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);
                if (result.ToList().Count() == 0)
                {
                    ViewBag.Message = "Tiada Data";
                }
                return View(result);
            }
            else
            {
                //int groupID = int.Parse(GroupList);
                
                if (WorkerList == "0")
                {
                    var result = dbr.vw_rptKwspSocso.Where(x => x.fld_KumpulanID == groupID && x.fld_Year == YearList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);
                    if (result.ToList().Count() == 0)
                    {
                        ViewBag.Message = "Tiada Data";
                    }
                    return View(result);
                }
                else
                {
                    var result = dbr.vw_rptKwspSocso.Where(x => x.fld_KumpulanID == groupID && x.fld_Nopkj == WorkerList && x.fld_Year == YearList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);
                    if (result.ToList().Count() == 0)
                    {
                        ViewBag.Message = "Tiada Data";
                    }
                    return View(result);
                }
            }
        }

        public ActionResult SkbReport()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            //string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            ViewBag.Report = "class = active";
            int month = timezone.gettimezone().AddMonths(-1).Month;
            int year = timezone.gettimezone().Year;
            int rangeyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
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

            ViewBag.MonthList = MonthList;
            ViewBag.YearList = yearlist;
            return View();
        }

        public ActionResult SkbReportDetail(int? MonthList, int? YearList, string print)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_SP_Models dbsp = MVC_SYSTEM_SP_Models.ConnectToSqlServer(host, catalog, user, pass);
            var result = dbsp.sp_RptSkb(NegaraID, SyarikatID, WilayahID, LadangID, MonthList, YearList).ToList();
            ViewBag.DataCount = result.Count();
            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.Print = print;

            if (MonthList == null && YearList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChooseMonthYear;
                return View();
            }

            if (result.ToList().Count() == 0)
            {
                ViewBag.Message = GlobalResEstate.lblNoSkb;
                return View();
            }

            return View(result);
        }

        public ActionResult AccStatusReport()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.Report = "class = active";
            int month = timezone.gettimezone().AddMonths(-1).Month;
            int year = timezone.gettimezone().Year;
            int rangeyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
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

            var MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID==NegaraID && x.fld_SyarikatID==SyarikatID), "fldOptConfValue", "fldOptConfDesc", month).ToList();
            MonthList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.MonthList = MonthList;
            ViewBag.YearList = yearlist;
            return View();
        }

        public ActionResult AccStatusReportDetail(int? YearList, int? MonthList, string print)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.Print = print;

            if(YearList == null && MonthList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChooseMonthYear;
                return View();
            }

            if (MonthList==0)
            {
                var result = dbr.tbl_TutupUrusNiaga.Where(x => x.fld_Year == YearList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o=>o.fld_Month).ToList();
                ViewBag.DataCount = result.Count();
                return View(result);
            }
            else
            {
                var result = dbr.tbl_TutupUrusNiaga.Where(x => x.fld_Month == MonthList && x.fld_Year == YearList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
                ViewBag.DataCount = result.Count();
                return View(result);
            }
        }

        public ActionResult GenSalaryStatusReport()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.Report = "class = active";
            int month = timezone.gettimezone().AddMonths(-1).Month;
            int year = timezone.gettimezone().Year;
            int rangeyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
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

            var MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false & x.fld_NegaraID==NegaraID && x.fld_SyarikatID==SyarikatID), "fldOptConfValue", "fldOptConfDesc", month).ToList();
            MonthList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.MonthList = MonthList;
            ViewBag.YearList = yearlist;
            return View();
        }

        public ActionResult GenSalaryStatusReportDetail(int YearList, int MonthList)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.YearSelection = YearList;
            ViewBag.MonthSelection = MonthList;
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;

            if (MonthList==0)
            {
                var result = db.tbl_SevicesProcess.Where(x => x.fld_Year == YearList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o=>o.fld_Month).ToList();
                ViewBag.DataCount = result.Count();
                return View(result);
            }
            else
            {
                var result = db.tbl_SevicesProcess.Where(x => x.fld_Month == MonthList && x.fld_Year == YearList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
                ViewBag.DataCount = result.Count();
                return View(result);
            }
        }

        public ActionResult PaySlipRpt()
        {
            ViewBag.Report = "class = active";
            int month = timezone.gettimezone().AddMonths(-1).Month;
            int year = timezone.gettimezone().Year;
            int rangeyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;

            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

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
            var monthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", month);

            List<SelectListItem> StatusList = new List<SelectListItem>();
            StatusList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            StatusList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            List<SelectListItem> SelectionList = new List<SelectListItem>();
            //edit by fitri 5-11-2020 (orderby = fld_Nama)
            SelectionList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_DivisionID == DivisionID).OrderBy(o => o.fld_Nama).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }), "Value", "Text").ToList();
            SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            List<SelectListItem> JnsPkjList = new List<SelectListItem>();
            JnsPkjList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            JnsPkjList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.SelectionList = SelectionList;
            ViewBag.MonthList = monthList;
            ViewBag.YearList = yearlist;
            ViewBag.StatusList = StatusList;
            ViewBag.JnsPkjList = JnsPkjList;
            return View();
        }

        //added by faeza 26.02.2023
        public ActionResult PaySlipRpt2()
        {
            ViewBag.Report = "class = active";
            int month = timezone.gettimezone().AddMonths(-1).Month;
            int year = timezone.gettimezone().Year;
            int rangeyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;

            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

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
            var monthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", month);

            List<SelectListItem> StatusList = new List<SelectListItem>();
            StatusList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            StatusList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            List<SelectListItem> SelectionList = new List<SelectListItem>();
            //edit by fitri 5-11-2020 (orderby = fld_Nama)
            SelectionList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_DivisionID == DivisionID).OrderBy(o => o.fld_Nama).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }), "Value", "Text").ToList();
            SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            List<SelectListItem> JnsPkjList = new List<SelectListItem>();
            JnsPkjList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            JnsPkjList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            //add by faeza 09.04.2024
            List<SelectListItem> IncentiveList = new List<SelectListItem>();
            IncentiveList = new SelectList(db.tbl_JenisInsentif.Where(x => x.fld_InclSecondPayslip == true && x.fld_JenisInsentif == "P" && x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_KodInsentif).Select(s => new SelectListItem { Value = s.fld_KodInsentif, Text = s.fld_Keterangan }), "Value", "Text").ToList();

            ViewBag.SelectionList = SelectionList;
            ViewBag.MonthList = monthList;
            ViewBag.YearList = yearlist;
            ViewBag.StatusList = StatusList;
            ViewBag.JnsPkjList = JnsPkjList;
            ViewBag.IncentiveList = IncentiveList;
            return View();
        }

        public ActionResult _PaySlipRptAdvance()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            //string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> CategoryList = new List<SelectListItem>();
            CategoryList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "designation" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            CategoryList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.WorkCategoryList = CategoryList;
            return View();
        }

        public ActionResult _PaySlipRptSearch(int? RadioGroup, int? MonthList, int? YearList, string SelectionList, string StatusList, string WorkCategoryList, string JnsPkjList, string print)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);

            ViewBag.SelectedMonth = MonthList;
            ViewBag.SelectedYear = YearList;
            ViewBag.Print = print;
            //find pekerja
            if(WorkCategoryList=="0" || WorkCategoryList == null)
            {
                if (RadioGroup == 0)
                {
                    //individu
                    if (StatusList == "0")
                    {
                        // aktif & xaktif
                        if (SelectionList == "0")
                        {
                            if (JnsPkjList == "0")
                            {
                                //semua individu
                                var pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_DivisionID == DivisionID).OrderBy(o => o.fld_Nama);
                                return View(pkjList);
                            }
                            else
                            {
                                //semua individu
                                var pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Jenispekerja == JnsPkjList && x.fld_DivisionID == DivisionID).OrderBy(o => o.fld_Nama);
                                return View(pkjList);
                            }
                            
                        }
                        else
                        {
                            if (JnsPkjList == "0")
                            {
                                //selected individu
                                var pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == SelectionList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_DivisionID == DivisionID).OrderBy(o => o.fld_Nama);
                                return View(pkjList);
                            }
                            else
                            {
                                //selected individu
                                var pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == SelectionList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Jenispekerja == JnsPkjList && x.fld_DivisionID == DivisionID).OrderBy(o => o.fld_Nama);
                                return View(pkjList);
                            }
                            
                        }

                    }
                    else
                    {
                        // aktif/xaktif
                        if (SelectionList == "0")
                        {
                            if (JnsPkjList == "0")
                            {
                                //semua individu
                                var pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_Kdaktf == StatusList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_DivisionID == DivisionID).OrderBy(o => o.fld_Nama);
                                return View(pkjList);
                            }
                            else
                            {
                                //semua individu
                                var pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_Kdaktf == StatusList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Jenispekerja == JnsPkjList && x.fld_DivisionID == DivisionID).OrderBy(o => o.fld_Nama);
                                return View(pkjList);
                            }
                            
                        }
                        else
                        {
                            if (JnsPkjList == "0")
                            {
                                //selected individu
                                var pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_Kdaktf == StatusList && x.fld_Nopkj == SelectionList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_DivisionID == DivisionID).OrderBy(o => o.fld_Nama);
                                return View(pkjList);
                            }
                            else
                            {
                                //selected individu
                                var pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_Kdaktf == StatusList && x.fld_Nopkj == SelectionList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Jenispekerja == JnsPkjList && x.fld_DivisionID == DivisionID).OrderBy(o => o.fld_Nama);
                                return View(pkjList);
                            }
                            
                        }
                    }
                }
                else
                {
                    //group
                    if (SelectionList == "0")
                    {
                        if (JnsPkjList == "0")
                        {
                            //semua group
                            var pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_DivisionID == DivisionID).OrderBy(o => o.fld_Nama);
                            return View(pkjList);
                        }
                        else
                        {
                            //semua group
                            var pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Jenispekerja == JnsPkjList && x.fld_DivisionID == DivisionID).OrderBy(o => o.fld_Nama);
                            return View(pkjList);
                        }
                        
                    }
                    else
                    {
                        if (JnsPkjList == "0")
                        {
                            //selected group
                            var kumpID = dbr.tbl_KumpulanKerja.Where(x => x.fld_KodKumpulan == SelectionList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false && x.fld_DivisionID == DivisionID).Select(s => s.fld_KumpulanID).FirstOrDefault();
                            var pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_KumpulanID == kumpID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1);
                            return View(pkjList);
                        }
                        else
                        {
                            //selected group
                            var kumpID = dbr.tbl_KumpulanKerja.Where(x => x.fld_KodKumpulan == SelectionList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false && x.fld_DivisionID == DivisionID).Select(s => s.fld_KumpulanID).FirstOrDefault();
                            var pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_KumpulanID == kumpID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Jenispekerja == JnsPkjList);
                            return View(pkjList);
                        }
                        
                    }
                }
            }
            else
            {
                if (JnsPkjList == "0")
                {
                    //kategori pkj
                    var pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_Ktgpkj == WorkCategoryList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_DivisionID == DivisionID).OrderBy(o => o.fld_Nama);
                    return View(pkjList);
                }
                else
                {
                    //kategori pkj
                    var pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_Ktgpkj == WorkCategoryList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Jenispekerja == JnsPkjList && x.fld_DivisionID == DivisionID).OrderBy(o => o.fld_Nama);
                    return View(pkjList);
                }
                
            }
        }

        //added by faeza 26.02.2023
        public ActionResult _PaySlipRptSearch2(int? RadioGroup, int? MonthList, int? YearList, string SelectionList, string StatusList, string WorkCategoryList, string JnsPkjList, string IncentiveList, string print)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);

            ViewBag.SelectedMonth = MonthList;
            ViewBag.SelectedYear = YearList;
            ViewBag.IncentiveList = IncentiveList;
            ViewBag.Print = print;
            //find pekerja
            if (WorkCategoryList == "0" || WorkCategoryList == null)
            {
                if (RadioGroup == 0)
                {
                    //individu
                    if (StatusList == "0")
                    {
                        // aktif & xaktif
                        if (SelectionList == "0")
                        {
                            if (JnsPkjList == "0")
                            {
                                //semua individu
                                var pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_DivisionID == DivisionID).OrderBy(o => o.fld_Nama);
                                return View(pkjList);
                            }
                            else
                            {
                                //semua individu
                                var pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Jenispekerja == JnsPkjList && x.fld_DivisionID == DivisionID).OrderBy(o => o.fld_Nama);
                                return View(pkjList);
                            }

                        }
                        else
                        {
                            if (JnsPkjList == "0")
                            {
                                //selected individu
                                var pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == SelectionList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_DivisionID == DivisionID).OrderBy(o => o.fld_Nama);
                                return View(pkjList);
                            }
                            else
                            {
                                //selected individu
                                var pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == SelectionList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Jenispekerja == JnsPkjList && x.fld_DivisionID == DivisionID).OrderBy(o => o.fld_Nama);
                                return View(pkjList);
                            }

                        }

                    }
                    else
                    {
                        // aktif/xaktif
                        if (SelectionList == "0")
                        {
                            if (JnsPkjList == "0")
                            {
                                //semua individu
                                var pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_Kdaktf == StatusList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_DivisionID == DivisionID).OrderBy(o => o.fld_Nama);
                                return View(pkjList);
                            }
                            else
                            {
                                //semua individu
                                var pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_Kdaktf == StatusList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Jenispekerja == JnsPkjList && x.fld_DivisionID == DivisionID).OrderBy(o => o.fld_Nama);
                                return View(pkjList);
                            }

                        }
                        else
                        {
                            if (JnsPkjList == "0")
                            {
                                //selected individu
                                var pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_Kdaktf == StatusList && x.fld_Nopkj == SelectionList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_DivisionID == DivisionID).OrderBy(o => o.fld_Nama);
                                return View(pkjList);
                            }
                            else
                            {
                                //selected individu
                                var pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_Kdaktf == StatusList && x.fld_Nopkj == SelectionList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Jenispekerja == JnsPkjList && x.fld_DivisionID == DivisionID).OrderBy(o => o.fld_Nama);
                                return View(pkjList);
                            }

                        }
                    }
                }
                else
                {
                    //group
                    if (SelectionList == "0")
                    {
                        if (JnsPkjList == "0")
                        {
                            //semua group
                            var pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_DivisionID == DivisionID).OrderBy(o => o.fld_Nama);
                            return View(pkjList);
                        }
                        else
                        {
                            //semua group
                            var pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Jenispekerja == JnsPkjList && x.fld_DivisionID == DivisionID).OrderBy(o => o.fld_Nama);
                            return View(pkjList);
                        }

                    }
                    else
                    {
                        if (JnsPkjList == "0")
                        {
                            //selected group
                            var kumpID = dbr.tbl_KumpulanKerja.Where(x => x.fld_KodKumpulan == SelectionList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false && x.fld_DivisionID == DivisionID).Select(s => s.fld_KumpulanID).FirstOrDefault();
                            var pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_KumpulanID == kumpID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1);
                            return View(pkjList);
                        }
                        else
                        {
                            //selected group
                            var kumpID = dbr.tbl_KumpulanKerja.Where(x => x.fld_KodKumpulan == SelectionList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false && x.fld_DivisionID == DivisionID).Select(s => s.fld_KumpulanID).FirstOrDefault();
                            var pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_KumpulanID == kumpID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Jenispekerja == JnsPkjList);
                            return View(pkjList);
                        }

                    }
                }
            }
            else
            {
                if (JnsPkjList == "0")
                {
                    //kategori pkj
                    var pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_Ktgpkj == WorkCategoryList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_DivisionID == DivisionID).OrderBy(o => o.fld_Nama);
                    return View(pkjList);
                }
                else
                {
                    //kategori pkj
                    var pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_Ktgpkj == WorkCategoryList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Jenispekerja == JnsPkjList && x.fld_DivisionID == DivisionID).OrderBy(o => o.fld_Nama);
                    return View(pkjList);
                }

            }
        }

        public ActionResult _PaySlipRptDetail(string pkj, int month, int year)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_SP_Models dbsp = MVC_SYSTEM_SP_Models.ConnectToSqlServer(host, catalog, user, pass);

            var result = dbsp.sp_Payslip(NegaraID, SyarikatID, WilayahID, LadangID, month, year, pkj).ToList();
            var getpkjInfo = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == pkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1);

            ViewBag.NamaPkj = getpkjInfo.Select(s => s.fld_Nama).FirstOrDefault();
            ViewBag.NoKwsp = getpkjInfo.Select(s => s.fld_Nokwsp).FirstOrDefault();
            ViewBag.NoSocso = getpkjInfo.Select(s => s.fld_Noperkeso).FirstOrDefault();
            ViewBag.NoKp = getpkjInfo.Select(s => s.fld_Nokp).FirstOrDefault();
            
            int? kumpID = getpkjInfo.Select(s => s.fld_KumpulanID).FirstOrDefault();//desc
            string ktgrPkj = getpkjInfo.Select(s => s.fld_Ktgpkj).FirstOrDefault();//desc
            string jntnaPkj = getpkjInfo.Select(s => s.fld_Kdjnt).FirstOrDefault();//desc

            ViewBag.Kump = dbr.tbl_KumpulanKerja.Where(x => x.fld_KumpulanID == kumpID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted==false).Select(s => s.fld_Keterangan).FirstOrDefault();
            ViewBag.Kategori = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "designation" && x.fldOptConfValue == ktgrPkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted==false).Select(s => s.fldOptConfDesc).FirstOrDefault();
            ViewBag.Jantina = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jantina" && x.fldOptConfValue == jntnaPkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted==false).Select(s => s.fldOptConfDesc).FirstOrDefault();

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            //added by faeza 26.02.2023
            ViewBag.Ladang = db.tbl_Ladang.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_ID == LadangID).Select(s => s.fld_LdgName).FirstOrDefault();
            ViewBag.Month = month;
            ViewBag.Year = year;
            ViewBag.Date = System.DateTime.Now.ToShortDateString();
            return View(result);
        }

        //added by faeza 26.02.2023
        public ActionResult _PaySlipRptDetail2(string pkj, int month, int year, string incentive)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_SP_Models dbsp = MVC_SYSTEM_SP_Models.ConnectToSqlServer(host, catalog, user, pass);

            var result = dbsp.sp_Payslip2(NegaraID, SyarikatID, WilayahID, LadangID, month, year, pkj, incentive).ToList();
            var getpkjInfo = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == pkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1);

            ViewBag.NamaPkj = getpkjInfo.Select(s => s.fld_Nama).FirstOrDefault();
            ViewBag.NoKwsp = getpkjInfo.Select(s => s.fld_Nokwsp).FirstOrDefault();
            ViewBag.NoSocso = getpkjInfo.Select(s => s.fld_Noperkeso).FirstOrDefault();
            ViewBag.NoKp = getpkjInfo.Select(s => s.fld_Nokp).FirstOrDefault();

            int? kumpID = getpkjInfo.Select(s => s.fld_KumpulanID).FirstOrDefault();//desc
            string ktgrPkj = getpkjInfo.Select(s => s.fld_Ktgpkj).FirstOrDefault();//desc
            string jntnaPkj = getpkjInfo.Select(s => s.fld_Kdjnt).FirstOrDefault();//desc

            ViewBag.Kump = dbr.tbl_KumpulanKerja.Where(x => x.fld_KumpulanID == kumpID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).Select(s => s.fld_Keterangan).FirstOrDefault();
            ViewBag.Kategori = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "designation" && x.fldOptConfValue == ktgrPkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfDesc).FirstOrDefault();
            ViewBag.Jantina = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jantina" && x.fldOptConfValue == jntnaPkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfDesc).FirstOrDefault();

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            //added by faeza 26.02.2023
            ViewBag.Ladang = db.tbl_Ladang.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_ID == LadangID).Select(s => s.fld_LdgName).FirstOrDefault();
            ViewBag.Month = month;
            ViewBag.Year = year;
            ViewBag.Date = System.DateTime.Now.ToShortDateString();
            return View(result);
        }

        public ActionResult _PaySlipRptDaycount(string nopkj, int month, int year)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            //shah
            var FooterPayslipDetails = new List<FooterPayslipDetails>();
            int id = 1;
            //get Hadir and cuti Count
            var hdr = dbr.tbl_Kerjahdr.Where(x => x.fld_Nopkj == nopkj && x.fld_Tarikh.Value.Month == month && x.fld_Tarikh.Value.Year == year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
            var hdrhrbs = hdr.Where(x => x.fld_Kdhdct == "H01").Count();
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrhrbs", count = hdrhrbs });
            id += 1;
            var hdrhrmg = hdr.Where(x => x.fld_Kdhdct == "H02").Count();
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrhrmg", count = hdrhrmg });
            id += 1;
            var hdrhrcu = hdr.Where(x => x.fld_Kdhdct == "H03").Count();
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrhrcu", count = hdrhrcu });
            id += 1;
            var hdrhrpg = hdr.Where(x => x.fld_Kdhdct == "P01").Count();
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrhrpg", count = hdrhrpg });
            id += 1;
            var hdrhrct = hdr.Where(x => x.fld_Kdhdct == "C02").Count();
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrhrct", count = hdrhrct });
            id += 1;
            var hdrhrtg = hdr.Where(x => x.fld_Kdhdct == "C05").Count();
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrhrtg", count = hdrhrtg });
            id += 1;
            var hdrhrcs = hdr.Where(x => x.fld_Kdhdct == "C03").Count();
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrhrcs", count = hdrhrcs });
            id += 1;
            var hdrhrca = hdr.Where(x => x.fld_Kdhdct == "C01").Count();
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrhrca", count = hdrhrca });
            id += 1;
            var hdrhrcm = hdr.Where(x => x.fld_Kdhdct == "C07").Count();
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrhrcm", count = hdrhrcm });
            id += 1;
            var hdrhrcb = hdr.Where(x => x.fld_Kdhdct == "C04").Count();
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrhrcb", count = hdrhrcb });
            id += 1;
            var hdrhrch = hdr.Where(x => x.fld_Kdhdct == "C10").Count();
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrhrch", count = hdrhrch });
            id += 1;
            var hdrhrce = hdr.Where(x => x.fld_Kdhdct == "C09").Count();
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrhrce", count = hdrhrce });

            //get hdr OT
            var hdrot = dbr.vw_KerjaHdrOT.Where(x => x.fld_Nopkj == nopkj && x.fld_Tarikh.Value.Month == month && x.fld_Tarikh.Value.Year == year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
            id += 1;
            var hdrothrbs = hdrot.Where(x => x.fld_Kdhdct == "H01").Sum(s => s.fld_JamOT);
            hdrothrbs = hdrothrbs == null ? 0m : hdrothrbs;
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrothrbs", value = hdrothrbs.Value });
            id += 1;
            var hdrothrcm = hdrot.Where(x => x.fld_Kdhdct == "H02").Sum(s => s.fld_JamOT);
            hdrothrcm = hdrothrcm == null ? 0m : hdrothrcm;
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrothrcm", value = hdrothrcm.Value });
            id += 1;
            var hdrothrcu = hdrot.Where(x => x.fld_Kdhdct == "H03").Sum(s => s.fld_JamOT);
            hdrothrcu = hdrothrcu == null ? 0m : hdrothrcu;
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrothrcu", value = hdrothrcu.Value });

            //get Jumlah Hari Kerja
            int? hrkrja = 0;//db.tbl_HariBekerjaLadang.Where(x => x.fld_Month == month && x.fld_Year == year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_BilHariBekerja).FirstOrDefault();
            //var GetLadangDetail = db.tbl_Ladang.Where(x => x.fld_ID == LadangID && x.fld_WlyhID == WilayahID).FirstOrDefault();
            //int? hrkrja = db.vw_HariBekerja.Where(x => GetLadangDetail.fld_KodNegeri == x.fld_NegeriID.ToString() && x.fld_NegaraID == NegaraID && x.fld_Year == year && x.fld_Month == month && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false && GetLadangDetail.fld_ID == LadangID).Select(s => s.fld_BilanganHariBekerja).FirstOrDefault();
            id += 1;
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hrkrja", count = hrkrja.Value });

            //get jmlh hari hadir
            var cdct = new string[] { "H01", "H02", "H03" };
            var jmlhhdr = hdr.Where(x => cdct.Contains(x.fld_Kdhdct)).Count();
            id += 1;
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "jmlhhdr", count = jmlhhdr });

            //get avg slry
            //modified by Shah 24.12.2023 - untuk purata setahun
            DateTime cdate = new DateTime(year, month, 15);
            DateTime ldate = cdate.AddMonths(-1);
            var cravgslry = dbr.tbl_GajiBulanan.Where(x => x.fld_Month == cdate.Month && x.fld_Year == cdate.Year && x.fld_Nopkj == nopkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => new { s.fld_PurataGaji, s.fld_PurataGaji12Bln }).FirstOrDefault();
            if (cravgslry != null)
            {
                var crmnthavgslry = cravgslry.fld_PurataGaji == null ? 0m : cravgslry.fld_PurataGaji;
                id += 1;
                FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "crmnthavgslry", value = crmnthavgslry.Value });
            }
            var lsmnthavgslry = dbr.tbl_GajiBulanan.Where(x => x.fld_Month == ldate.Month && x.fld_Year == ldate.Year && x.fld_Nopkj == nopkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_PurataGaji).FirstOrDefault();
            lsmnthavgslry = lsmnthavgslry == null ? 0m : lsmnthavgslry;
            id += 1;
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "lsmnthavgslry", value = lsmnthavgslry.Value });
            if (cravgslry != null)
            {
                var yearavgslry = cravgslry.fld_PurataGaji12Bln == null || cravgslry.fld_PurataGaji12Bln > 200 ? 0m : cravgslry.fld_PurataGaji12Bln;
                id += 1;
                FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "yearavgslry", value = yearavgslry.Value });
            }
            //modified by Shah 24.12.2023 - untuk purata setahun
            return View(FooterPayslipDetails);
        }

        //added by faeza 26.02.2023 - untuk 2nd payslip
        public ActionResult _PaySlipRptDaycount2(string nopkj, int month, int year)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            //shah
            var FooterPayslipDetails = new List<FooterPayslipDetails>();
            int id = 1;
            //get Hadir and cuti Count
            //var hdr = dbr.tbl_Kerjahdr.Where(x => x.fld_Nopkj == nopkj && x.fld_Tarikh.Value.Month == month && x.fld_Tarikh.Value.Year == year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
            int hdrhrbs = 0;// hdr.Where(x => x.fld_Kdhdct == "H01").Count();
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrhrbs", count = hdrhrbs });
            id += 1;
            int hdrhrmg = 0;// hdr.Where(x => x.fld_Kdhdct == "H02").Count();
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrhrmg", count = hdrhrmg });
            id += 1;
            int hdrhrcu = 0;// hdr.Where(x => x.fld_Kdhdct == "H03").Count();
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrhrcu", count = hdrhrcu });
            id += 1;
            int hdrhrpg = 0;// hdr.Where(x => x.fld_Kdhdct == "P01").Count();
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrhrpg", count = hdrhrpg });
            id += 1;
            int hdrhrct = 0;// hdr.Where(x => x.fld_Kdhdct == "C02").Count();
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrhrct", count = hdrhrct });
            id += 1;
            int hdrhrtg = 0;// hdr.Where(x => x.fld_Kdhdct == "C05").Count();
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrhrtg", count = hdrhrtg });
            id += 1;
            int hdrhrcs = 0;// hdr.Where(x => x.fld_Kdhdct == "C03").Count();
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrhrcs", count = hdrhrcs });
            id += 1;
            int hdrhrca = 0;// hdr.Where(x => x.fld_Kdhdct == "C01").Count();
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrhrca", count = hdrhrca });
            id += 1;
            int hdrhrcm = 0;// hdr.Where(x => x.fld_Kdhdct == "C07").Count();
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrhrcm", count = hdrhrcm });
            id += 1;
            int hdrhrcb = 0;// hdr.Where(x => x.fld_Kdhdct == "C04").Count();
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrhrcb", count = hdrhrcb });
            id += 1;
            var hdrhrch = 0;// hdr.Where(x => x.fld_Kdhdct == "C10").Count();
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrhrch", count = hdrhrch });
            id += 1;
            int hdrhrce = 0;// hdr.Where(x => x.fld_Kdhdct == "C09").Count();
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrhrce", count = hdrhrce });

            //get hdr OT
            //var hdrot = dbr.vw_KerjaHdrOT.Where(x => x.fld_Nopkj == nopkj && x.fld_Tarikh.Value.Month == month && x.fld_Tarikh.Value.Year == year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
            id += 1;
            int hdrothrbs = 0;// hdrot.Where(x => x.fld_Kdhdct == "H01").Sum(s => s.fld_JamOT);
            //hdrothrbs = hdrothrbs == null ? 0m : hdrothrbs;
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrothrbs", value = hdrothrbs });
            id += 1;
            int hdrothrcm = 0;// hdrot.Where(x => x.fld_Kdhdct == "H02").Sum(s => s.fld_JamOT);
            //hdrothrcm = hdrothrcm == null ? 0m : hdrothrcm;
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrothrcm", value = hdrothrcm });
            id += 1;
            int hdrothrcu = 0;// hdrot.Where(x => x.fld_Kdhdct == "H03").Sum(s => s.fld_JamOT);
            //hdrothrcu = hdrothrcu == null ? 0m : hdrothrcu;
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrothrcu", value = hdrothrcu });

            //get Jumlah Hari Kerja
            int? hrkrja = 0;//db.tbl_HariBekerjaLadang.Where(x => x.fld_Month == month && x.fld_Year == year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_BilHariBekerja).FirstOrDefault();
            id += 1;
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hrkrja", count = hrkrja.Value });

            //get jmlh hari hadir
            //var cdct = new string[] { "H01", "H02", "H03" };
            int jmlhhdr = 0;// hdr.Where(x => cdct.Contains(x.fld_Kdhdct)).Count();
            id += 1;
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "jmlhhdr", count = jmlhhdr });

            //get avg slry
            //DateTime cdate = new DateTime(year, month, 15);
            //DateTime ldate = cdate.AddMonths(-1);
            int crmnthavgslry = 0;// dbr.tbl_GajiBulanan.Where(x => x.fld_Month == cdate.Month && x.fld_Year == cdate.Year && x.fld_Nopkj == nopkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_PurataGaji).FirstOrDefault();
            //crmnthavgslry = crmnthavgslry == null ? 0m : crmnthavgslry;
            id += 1;
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "crmnthavgslry", value = crmnthavgslry });
            int lsmnthavgslry = 0;// dbr.tbl_GajiBulanan.Where(x => x.fld_Month == ldate.Month && x.fld_Year == ldate.Year && x.fld_Nopkj == nopkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_PurataGaji).FirstOrDefault();
            //lsmnthavgslry = lsmnthavgslry == null ? 0m : lsmnthavgslry;
            id += 1;
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "lsmnthavgslry", value = lsmnthavgslry });
            //shah
            return View(FooterPayslipDetails);
        }

        //public ActionResult htmltopdf(string month)
        //{
        //    return new Rotativa.MVC.RouteAsPdf("ExpiredPassport", new { month = month });
        //}

        [HttpPost]
        public ActionResult ConvertPDF2(string myHtml, string filename, string reportname)
        {
            bool success = false;
            string msg = "";
            string status = "";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            Models.tblHtmlReport tblHtmlReport = new Models.tblHtmlReport();

            tblHtmlReport.fldHtlmCode = myHtml;
            tblHtmlReport.fldFileName = filename;
            tblHtmlReport.fldReportName = reportname;

            dbr.tblHtmlReports.Add(tblHtmlReport);
            dbr.SaveChanges();

            success = true;
            status = "success";

            return Json(new { success = success, id = tblHtmlReport.fldID, msg = msg, status = status, link = Url.Action("GetPDF", "Report", null, "http") + "/" + tblHtmlReport.fldID });
        }

        public ActionResult GetPDF(int id)
        {
            //int? NegaraID = 0;
            //int? SyarikatID = 0;
            //int? WilayahID = 0;
            //int? LadangID = 0;
            //int? getuserid = GetIdentity.ID(User.Identity.Name);
            //string width = "", height = "";
            //string imagepath = Server.MapPath("~/Asset/Images/");

            //string host, catalog, user, pass = "";
            //GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            //Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            //MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            //var gethtml = dbr.tblHtmlReports.Find(id);
            //var getsize = db.tblMenuLists.Where(x => x.fld_Val == gethtml.fldReportName.ToString() && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).FirstOrDefault();
            //if (getsize != null)
            //{
            //    width = getsize.fld_WidthReport.ToString();
            //    height = getsize.fld_HeightReport.ToString();
            //}
            //else
            //{
            //    var getsizesubreport = db.tblSubReportLists.Where(x => x.fldSubReportListAction == gethtml.fldReportName.ToString()).FirstOrDefault();
            //    width = getsizesubreport.fldSubWidthReport.ToString();
            //    height = getsizesubreport.fldSubHeightReport.ToString();
            //}
            //GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            //var logosyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_LogoName).FirstOrDefault();

            ////Export HTML String as PDF.
            ////Image logo = Image.GetInstance(imagepath + logosyarikat);
            ////Image alignment
            ////logo.ScaleToFit(50f, 50f);
            ////logo.Alignment = Image.TEXTWRAP | Image.ALIGN_CENTER;
            ////StringReader sr = new StringReader(gethtml.fldHtlmCode);
            //Document pdfDoc = new Document(new Rectangle(int.Parse(width), int.Parse(height)), 50f, 50f, 50f, 50f);
            ////HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
            //PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            //pdfDoc.Open();
            ////pdfDoc.Add(logo);
            //using (TextReader sr = new StringReader(gethtml.fldHtlmCode))
            //{
            //    using (var htmlWorker = new HTMLWorkerExtended(pdfDoc, imagepath + logosyarikat))
            //    {
            //        htmlWorker.Open();
            //        htmlWorker.Parse(sr);
            //    }
            //}
            //pdfDoc.Close();
            //Response.ContentType = "application/pdf";
            //Response.AddHeader("content-disposition", "attachment;filename=" + gethtml.fldFileName + ".pdf");
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //Response.Write(pdfDoc);
            //Response.End();

            //dbr.Entry(gethtml).State = EntityState.Deleted;
            //dbr.SaveChanges();
            //dbr.Dispose();
            return View();
        }



        public ActionResult AsasPeringkat()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;

            List<SelectListItem> StatusList2 = new List<SelectListItem>();
            StatusList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnspkt" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            StatusList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.pktlist = StatusList2;
            ViewBag.getflag = 1;
            return View();

            
        }

        [HttpPost]
        public ActionResult AsasPeringkat(string pktlist)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;

            List<SelectListItem> StatusList2 = new List<SelectListItem>();
            StatusList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnspkt" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            StatusList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.pktlist = StatusList2;
            ViewBag.getflag = 2;
            ViewBag.getlevel = pktlist;
            return View();

        }

        public ActionResult AsasPeringkatSemua()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            ViewBag.TableInfo = "class = active";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            var result = dbr.vw_JnsPkt.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

            return PartialView(result);
        }

        public ActionResult AsasPeringkatUtama()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            
                var result = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(x => x.fld_PktUtama).ToList();
                ViewBag.DataCount = result.Count();
                ViewBag.getflag = 2;
                return View(result);            
        }

        public ActionResult AsasPeringkatSubPkt()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            
            var result = dbr.tbl_SubPkt.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(x => x.fld_Pkt).ToList();
                ViewBag.DataCount = result.Count();
                ViewBag.getflag = 2;
                return View(result);

            
        }

        public ActionResult AsasPeringkatBlok()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();


                var result = dbr.tbl_Blok.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(x => x.fld_Blok).ToList();
                ViewBag.DataCount = result.Count();
                ViewBag.getflag = 2;
                return View(result);

        }

        
        public ActionResult KodMappingAktiviti()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            List<SelectListItem> GLlist = new List<SelectListItem>();
            GLlist = new SelectList(db.tbl_MapGL.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID & x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_ID).Select(s => new SelectListItem { Value = s.fld_KodGL, Text = s.fld_KodGL }).Distinct(), "Value", "Text").ToList();
            GLlist.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            
            ViewBag.GLlist = GLlist;
            ViewBag.getflag = 1;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KodMappingAktiviti(string GLlist)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            //MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            

            List<SelectListItem> GLlist2 = new List<SelectListItem>();
            GLlist2 = new SelectList(db.tbl_MapGL.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID & x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_ID)
                .Select(s => new SelectListItem { Value = s.fld_KodGL, Text = s.fld_KodGL }).Distinct(), "Value", "Text").ToList();
            GLlist2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            ViewBag.GLlist = GLlist2;
            ViewBag.getflag = 2;

            if (GLlist == "0")
            {
                var result = db.tbl_MapGL.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID & x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_ID);
                return View(result);
            }
            else
            {
                var result = db.tbl_MapGL.Where(x => x.fld_KodGL == GLlist && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID & x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_ID);
                return View(result);
            }
        }


        public ActionResult PrintWorkerPdf(int? RadioGroup, string SelectionList, string StatusList, string JnsPkjList, int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("_WorkerRptSearch", new { RadioGroup, StatusList, SelectionList, JnsPkjList, print })
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

        public string PDFInvalid()
        {
            return GlobalResEstate.msgInvalidPDFConvert;
        }

        public ViewResult _WorkerRptSearch(int? RadioGroup, string StatusList, string SelectionList, string JnsPkjList, string print, /*string Sorting_Order,*/ int? Page_No) // edit by fitri 17-09-2020
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> StatusList2 = new List<SelectListItem>();
            List<SelectListItem> SelectionList2 = new List<SelectListItem>();
            List<SelectListItem> JnsPkjList2 = new List<SelectListItem>();

            List<Models.tbl_Pkjmast> InfoPekerja = new List<Models.tbl_Pkjmast>();

            StatusList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text", StatusList).ToList();
            StatusList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            JnsPkjList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text", JnsPkjList).ToList();
            JnsPkjList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.StatusList = StatusList2;
            ViewBag.JnsPkjList = JnsPkjList2;
            ViewBag.Print = print;
            ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy"); // add by fitri 17-09-2020
            DateTime time = DateTime.Now; // add by fitri 17-09-2020
            ViewBag.Time = time.ToString("t");
            // add by fitri 17-09-2020
            ViewBag.NamaLadang = db.tbl_Ladang.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Select(s => s.fld_LdgName).FirstOrDefault();
            int Size_Of_Page = 1; // add by fitri 17-09-2020
            int No_Of_Page = (Page_No ?? 1); // add by fitri 17-09-2020
            //ViewBag.CurrentSortOrder = Sorting_Order; // add by fitri 17-09-2020

            if (StatusList == null && SelectionList == null && JnsPkjList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChooseWorker; // edit by fitri 17-09-2020
                return View(InfoPekerja.ToPagedList(No_Of_Page, Size_Of_Page)); // edit by fitri 17-09-2020
            }
            else
            {
                Page_No = 1; // add by fitri 17-09-2020
                ViewBag.Rad = RadioGroup; // add by fitri 17-09-2020
                ViewBag.Sta = StatusList; // add by fitri 17-09-2020
                ViewBag.Sel = SelectionList; // add by fitri 17-09-2020
                ViewBag.Jns = JnsPkjList; // add by fitri 17-09-2020
                if (RadioGroup == 0)
                {
                    //Individu Semua
                    if (StatusList == "0")
                    {
                        SelectionList2 = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                        x.fld_LadangID == LadangID && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nopkj)
                        .Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }), "Value", "Text").ToList();
                        SelectionList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

                        if (SelectionList == "0")
                        {
                            if (JnsPkjList == "0")
                            {
                                //individu semua pekerja
                                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                x.fld_LadangID == LadangID).OrderBy(o => o.fld_Nopkj); // edit by fitri 17-09-2020
                                if (result.Count() == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }
                                ViewBag.SelectionList = SelectionList2;
                                ViewBag.getflag = 2;

                                return View(result.ToPagedList(No_Of_Page, Size_Of_Page)); // edit by fitri 17-09-2020
                            }
                            else
                            {
                                //pilih jenis pekerja
                                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                x.fld_Jenispekerja == JnsPkjList).OrderBy(o => o.fld_Nopkj);// edit by fitri 17-09-2020
                                if (result.Count() == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }
                                ViewBag.SelectionList = SelectionList2;
                                ViewBag.getflag = 2;

                                return View(result.ToPagedList(No_Of_Page, Size_Of_Page));// edit by fitri 17-09-2020
                            }
                        }
                        else
                        {
                            if (JnsPkjList == "0")
                            {
                                //individu semua pekerja
                                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                x.fld_Nopkj == SelectionList).OrderBy(o => o.fld_Nopkj);// edit by fitri 17-09-2020
                                if (result.Count() == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }
                                ViewBag.SelectionList = SelectionList2;
                                ViewBag.getflag = 2;

                                return View(result.ToPagedList(No_Of_Page, Size_Of_Page));// edit by fitri 17-09-2020
                            }
                            else
                            {
                                //pilih jenis pekerja
                                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                x.fld_Nopkj == SelectionList && x.fld_Jenispekerja == JnsPkjList).OrderBy(o => o.fld_Nopkj);// edit by fitri 17-09-2020
                                if (result.Count() == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }
                                ViewBag.SelectionList = SelectionList2;
                                ViewBag.getflag = 2;

                                return View(result.ToPagedList(No_Of_Page, Size_Of_Page));// edit by fitri 17-09-2020
                            }
                        }
                    }

                    else
                    {
                        SelectionList2 = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == StatusList).OrderBy(o => o.fld_Nopkj).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }), "Value", "Text").ToList();
                        SelectionList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
                        if (SelectionList == "0")
                        {
                            if (JnsPkjList == "0")
                            {
                                //individu aktif/xaktif pekerja
                                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                x.fld_Kdaktf == StatusList).OrderBy(o => o.fld_Nopkj);// edit by fitri 17-09-2020
                                if (result.Count() == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }
                                ViewBag.SelectionList = SelectionList2;
                                ViewBag.getflag = 2;

                                return View(result.ToPagedList(No_Of_Page, Size_Of_Page));// edit by fitri 17-09-2020
                            }
                            else
                            {
                                //pilih jenis pekerja
                                //individu aktif/xaktif pekerja
                                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                x.fld_Kdaktf == StatusList && x.fld_Jenispekerja == JnsPkjList).OrderBy(o => o.fld_Nopkj); // edit by fitri 17-09-2020
                                if (result.Count() == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }
                                ViewBag.SelectionList = SelectionList2;
                                ViewBag.getflag = 2;

                                return View(result.ToPagedList(No_Of_Page, Size_Of_Page));// edit by fitri 17-09-2020
                            }
                        }
                        else
                        {
                            if (JnsPkjList == "0")
                            {
                                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == StatusList &&
                                x.fld_Nopkj == SelectionList).OrderBy(o => o.fld_Nopkj);// edit by fitri 17-09-2020
                                if (result.Count() == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }
                                ViewBag.SelectionList = SelectionList2;
                                ViewBag.getflag = 2;

                                return View(result.ToPagedList(No_Of_Page, Size_Of_Page));// edit by fitri 17-09-2020
                            }
                            else
                            {
                                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == StatusList && x.fld_Nopkj == SelectionList &&
                                x.fld_Jenispekerja == JnsPkjList).OrderBy(o => o.fld_Nopkj);// edit by fitri 17-09-2020
                                if (result.Count() == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }
                                ViewBag.SelectionList = SelectionList2;
                                ViewBag.getflag = 2;

                                return View(result.ToPagedList(No_Of_Page, Size_Of_Page));// edit by fitri 17-09-2020
                            }

                        }
                    }
                }
                else //Group
                {
                    if (StatusList == "0")
                    {
                        SelectionList2 = new SelectList(dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                        x.fld_LadangID == LadangID && x.fld_deleted == false).OrderBy(o => o.fld_KodKumpulan)
                        .Select(s => new SelectListItem { Value = s.fld_KodKumpulan, Text = s.fld_KodKumpulan + "-" + s.fld_Keterangan }), "Value", "Text").ToList();
                        SelectionList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
                        if (SelectionList == "0")
                        {
                            if (JnsPkjList == "0")
                            {
                                //semua kump
                                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                x.fld_KumpulanID != null).OrderBy(o => o.fld_Nopkj);// edit by fitri 17-09-2020
                                ViewBag.SelectionList = SelectionList2;
                                if (result.Count() == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }
                                ViewBag.getflag = 2;

                                return View(result.ToPagedList(No_Of_Page, Size_Of_Page));// edit by fitri 17-09-2020
                            }
                            else
                            {
                                //pilih jenis pekerja
                                //semua kump
                                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                x.fld_KumpulanID != null && x.fld_Jenispekerja == JnsPkjList).OrderBy(o => o.fld_Nopkj);// edit by fitri 17-09-2020
                                ViewBag.SelectionList = SelectionList2;
                                if (result.Count() == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }
                                ViewBag.getflag = 2;

                                return View(result.ToPagedList(No_Of_Page, Size_Of_Page));// edit by fitri 17-09-2020
                            }
                        }
                        else
                        {
                            if (JnsPkjList == "0")
                            {
                                //by kump
                                int getkump = dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false && x.fld_KodKumpulan == SelectionList).Select(s => s.fld_KumpulanID).FirstOrDefault();
                                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                x.fld_KumpulanID == getkump).OrderBy(o => o.fld_Nopkj);// edit by fitri 17-09-2020
                                if (result.Count() == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }
                                ViewBag.SelectionList = SelectionList2;
                                ViewBag.getflag = 2;

                                return View(result.ToPagedList(No_Of_Page, Size_Of_Page));// edit by fitri 17-09-2020
                            }
                            else
                            {
                                //pilih jenis pekerja
                                //by kump
                                int getkump = dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false && x.fld_KodKumpulan == SelectionList).Select(s => s.fld_KumpulanID).FirstOrDefault();
                                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Jenispekerja == JnsPkjList &&
                                x.fld_KumpulanID == getkump).OrderBy(o => o.fld_Nopkj);// edit by fitri 17-09-2020
                                if (result.Count() == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }
                                ViewBag.SelectionList = SelectionList2;
                                ViewBag.getflag = 2;

                                return View(result.ToPagedList(No_Of_Page, Size_Of_Page));// edit by fitri 17-09-2020
                            }
                        }
                    }
                    else
                    {
                        SelectionList2 = new SelectList(dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).OrderBy(o => o.fld_KodKumpulan).Select(s => new SelectListItem { Value = s.fld_KodKumpulan, Text = s.fld_KodKumpulan + "-" + s.fld_Keterangan }), "Value", "Text").ToList();
                        SelectionList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
                        if (SelectionList == "0")
                        {
                            if (JnsPkjList == "0")
                            {
                                //individu aktif/xaktif pekerja
                                //int getkump = dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false && x.fld_KodKumpulan == SelectionList).Select(s => s.fld_KumpulanID).FirstOrDefault();
                                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == StatusList &&
                                x.fld_KumpulanID != null).OrderBy(o => o.fld_Nopkj); // edit by fitri 17-09-2020
                                if (result.Count() == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }
                                ViewBag.SelectionList = SelectionList2;
                                ViewBag.getflag = 2;

                                return View(result.ToPagedList(No_Of_Page, Size_Of_Page));// edit by fitri 17-09-2020
                            }
                            else
                            {
                                //pilih jenis pekerja
                                //individu aktif/xaktif pekerja
                                //int getkump = dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false && x.fld_KodKumpulan == SelectionList).Select(s => s.fld_KumpulanID).FirstOrDefault();
                                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                x.fld_Kdaktf == StatusList && x.fld_Jenispekerja == JnsPkjList && x.fld_KumpulanID != null).OrderBy(o => o.fld_Nopkj); // edit by fitri 17-09-2020
                                if (result.Count() == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }
                                ViewBag.SelectionList = SelectionList2;
                                ViewBag.getflag = 2;

                                return View(result.ToPagedList(No_Of_Page, Size_Of_Page));// edit by fitri 17-09-2020
                            }
                        }
                        else
                        {
                            if (JnsPkjList == "0")
                            {
                                int getkump = dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false && x.fld_KodKumpulan == SelectionList).Select(s => s.fld_KumpulanID).FirstOrDefault();
                                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == StatusList &&
                                x.fld_KumpulanID == getkump).OrderBy(o => o.fld_Nopkj); // edit by fitri 17-09-2020
                                if (result.Count() == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }
                                ViewBag.SelectionList = SelectionList2;
                                ViewBag.getflag = 2;

                                return View(result.ToPagedList(No_Of_Page, Size_Of_Page));// edit by fitri 17-09-2020
                            }
                            else
                            {
                                int getkump = dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false && x.fld_KodKumpulan == SelectionList).Select(s => s.fld_KumpulanID).FirstOrDefault();
                                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == StatusList && x.fld_Jenispekerja == JnsPkjList &&
                                x.fld_KumpulanID == getkump).OrderBy(o => o.fld_Nopkj); // edit by fitri 17-09-2020
                                if (result.Count() == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }
                                ViewBag.SelectionList = SelectionList2;
                                ViewBag.getflag = 2;

                                return View(result.ToPagedList(No_Of_Page, Size_Of_Page));// edit by fitri 17-09-2020
                            }
                        }
                    }
                }

            }
        }


        public ActionResult PrintGrpWorkerPdf(string GroupList, string JnsPkjList, int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("_GroupReport", new { GroupList, JnsPkjList, print })
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

        public ActionResult PrintWorkerInsentifPdf(int? RadioGroup, int? MonthList, int? YearList,
               string SelectionList, string StatusList, string WorkCategoryList, int id, string genid, string JnsPkjList)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("_WorkerIncentiveRptSearch", new { RadioGroup, MonthList, YearList, SelectionList, StatusList, WorkCategoryList, print, JnsPkjList })
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

        public ActionResult PrintLeavePdf(int? RadioGroup, int? YearList,
               string SelectionList, string StatusList, string WorkCategoryList, int id, string genid, string JnsPkjList)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("_WorkerLeaveRptSearch", new { RadioGroup, YearList, SelectionList, StatusList, WorkCategoryList, print, JnsPkjList })
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

        public ActionResult PrintAttRptPdf(int? RadioGroup, int? YearList,
            string SelectionList, string StatusList, string WorkCategoryList, int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("_WorkerAttendanceRptSearch", new { RadioGroup, YearList, SelectionList, print })
                {
                    FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName,
                    Cookies = cookies
                };
            }
            else
            {
                report = new ActionAsPdf("PDFInvalidGen");
            }

            return report;
        }

        public ActionResult PrintAccBankPdf(int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("_BankAccReport", new { print })
                {
                    FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName,
                    Cookies = cookies
                };
            }
            else
            {
                report = new ActionAsPdf("PDFInvalidGen");
            }

            return report;
        }

        public ActionResult PrintPaySheetPdf(int? RadioGroup, int? MonthList, int? YearList,
               string SelectionList, string StatusList, string WorkCategoryList, int id, string JnsPkjList, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("_WorkerPaySheetRptSearch", new { RadioGroup, MonthList, YearList, SelectionList, StatusList, WorkCategoryList, JnsPkjList, print })
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

        public ActionResult PrintMiniWagePdf(int? RadioGroup, int? MonthList, int? YearList,
               string SelectionList, string StatusList, string WorkCategoryList, int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("_MinimumWageRptSearch", new { RadioGroup, MonthList, YearList, SelectionList, StatusList, WorkCategoryList, print })
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

        public ActionResult PrintAccPdf(string StatusList, int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("_AccReport", new { StatusList, print })
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

        public ActionResult PrintKwspSocsoPdf(string StatusList, int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("_KwspSocsoReport", new { StatusList, print })
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

        public ActionResult PrintWorkPdf(string MonthList, string YearList, string WorkerList, int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("_WorkReport", new { MonthList, YearList, WorkerList, print })
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

        public ActionResult PrintTransactionPdf(string MonthList, string YearList, int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("_TransactionListingRptSearch", new { MonthList, YearList, print })
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

        public ActionResult PrintAccStatusPdf(string MonthList, string YearList, int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("AccStatusReportDetail", new { MonthList, YearList, print })
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

        public ActionResult PrintAIPSPdf(string MonthList, string YearList, string WorkerList, int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("AccStatusReportDetail", new { MonthList, YearList, WorkerList, print })
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

        public ActionResult PrintProductPdf(int? MonthList, int? YearList, string SelectionList, string PilihanAktvt, string PilihanGL, string UnitList, string AllPeringkatList, string StatusList, int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("_ProductivityRptSearch", new { MonthList, YearList, SelectionList, PilihanAktvt, PilihanGL, UnitList, AllPeringkatList, StatusList, print })
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

        public ActionResult PrintKwspSocsoMonthPdf(int? YearList, string GroupList, string WorkerList, int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("KwspSocsoMonthlyReportDetail", new { YearList, GroupList, WorkerList, print })
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

        public ActionResult PrintHasilPdf( int? RadioGroup, int? MonthList, int? YearList, string SelectionList, int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("HasilReportDetail", new { RadioGroup, MonthList, YearList, SelectionList, print })
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

        public ActionResult PrintNotiPermitPdf(int? MonthList, int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("ExpiredPermit", new { MonthList, print })
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

        public ActionResult PrintNotiPassportPdf(string MonthList, int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("ExpiredPassport", new { MonthList, print })
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

        public ActionResult PrintWorkerPaySlipPdf(int? RadioGroup, int? MonthList, int? YearList,
            string SelectionList, string StatusList, string WorkCategoryList, int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("_WorkerPaySlipRptSearch", new { RadioGroup, MonthList, YearList, SelectionList, StatusList, WorkCategoryList, print })
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
        public ActionResult KodMappingAktivitiPaysheet()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            //string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            //Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            List<SelectListItem> Paysheetlist = new List<SelectListItem>();
            Paysheetlist = new SelectList(db.tbl_MapGL.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID & x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_ID)
                .Select(s => new SelectListItem { Value = s.fld_Paysheet, Text = s.fld_Paysheet }).Distinct(), "Value", "Text").ToList();
            Paysheetlist.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            ViewBag.Paysheetlist = Paysheetlist;
            ViewBag.getflag = 1;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KodMappingAktivitiPaysheet(string Paysheetlist)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            //string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> Paysheetlist2 = new List<SelectListItem>();
            Paysheetlist2 = new SelectList(db.tbl_MapGL.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID & x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_ID)
                .Select(s => new SelectListItem { Value = s.fld_Paysheet, Text = s.fld_Paysheet }).Distinct(), "Value", "Text").ToList();
            Paysheetlist2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            ViewBag.Paysheetlist = Paysheetlist2;
            ViewBag.getflag = 2;

            if (Paysheetlist == "0")
            {
                var result = db.tbl_MapGL.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID & x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_ID);
                return View(result);
            }
            else
            {
                var result = db.tbl_MapGL.Where(x => x.fld_Paysheet == Paysheetlist && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID & x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_ID);
                return View(result);
            }
        }


        public ActionResult KodMappingAktivitiGMN()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            //string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            //Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);


            var kodKategorilist = db.tbl_KategoriAktiviti
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).ToList();

            List<SelectListItem> KategoriAktiviti = new List<SelectListItem>();
            KategoriAktiviti = new SelectList(db.vw_GmnMapping
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
                .Select(s => new SelectListItem { Value = s.fld_KodKategori.ToString(), Text = s.fld_Kategori }).Distinct(), "Value", "Text").ToList();
            KategoriAktiviti.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            
            List<SelectListItem> Costcnt = new List<SelectListItem>();
            Costcnt = new SelectList(db.vw_GmnMapping
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID  && x.fld_Deleted == false)
                .Select(s => new SelectListItem { Value = s.fld_CostCentre, Text = s.fld_CostCentre }).Distinct(), "Value", "Text").ToList();
            Costcnt.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

           
            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();


            ViewBag.KategoriAktiviti = KategoriAktiviti;
            ViewBag.Costcnt = Costcnt;
            ViewBag.getflag = 1;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KodMappingAktivitiGMN(string KategoriAktiviti, string Costcnt)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            //string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            var kodKategorilist = db.tbl_KategoriAktiviti
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).ToList();

            List<SelectListItem> KategoriAktiviti2 = new List<SelectListItem>();
            KategoriAktiviti2 = new SelectList(db.vw_GmnMapping
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
                .Select(s => new SelectListItem { Value = s.fld_KodKategori.ToString(), Text = s.fld_Kategori }).Distinct(), "Value", "Text").ToList();
            KategoriAktiviti2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));


            List<SelectListItem> Costcnt2 = new List<SelectListItem>();
            Costcnt2 = new SelectList(db.vw_GmnMapping
                .Where(x => x.fld_KodKategori == KategoriAktiviti && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
                .Select(s => new SelectListItem { Value = s.fld_CostCentre, Text = s.fld_CostCentre }).Distinct(), "Value", "Text").ToList();
            Costcnt2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            ViewBag.KategoriAktiviti = KategoriAktiviti2;
            ViewBag.Costcnt = Costcnt2;
            ViewBag.getflag = 2;

            if (KategoriAktiviti == "0" && Costcnt == "0")
            {
                var result = db.vw_GmnMapping
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o => o.bil);
                return View(result);
            }            
            else
            {
                var result = db.vw_GmnMapping
                    .Where(x => x.fld_CostCentre == Costcnt && x.fld_KodKategori == KategoriAktiviti && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o => o.bil);
                return View(result);
            }
        }

       
        public JsonResult GetReportGMN(string KategoriAktiviti)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            //string host, catalog, user, pass = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            
            List<SelectListItem> PilihAktiviti = new List<SelectListItem>();
            PilihAktiviti = new SelectList(db.vw_GmnMapping.Where(x=>x.fld_KodKategori==KategoriAktiviti && x.fld_NegaraID==NegaraID && x.fld_SyarikatID==SyarikatID )
                .Select(s=> new SelectListItem { Value = s.fld_CostCentre, Text = s.fld_CostCentre }).Distinct(), "Value", "Text").ToList();
           
            PilihAktiviti.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "" }));

            //dbr.Dispose();
            return Json(new { PilihAktiviti });
        }


        public ActionResult KodMappingPupYm()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            //string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            //Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            List<SelectListItem> kodKategorilist = new List<SelectListItem>();
            kodKategorilist = new SelectList(db.tbl_KategoriAktiviti
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
                .Select(s => new SelectListItem { Value = s.fld_KodKategori.ToString(), Text = s.fld_Kategori }), "Value", "Text").ToList();
            kodKategorilist.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            
            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            ViewBag.kodKategorilist = kodKategorilist;            
            ViewBag.getflag = 1;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KodMappingPupYm(string kodKategorilist)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            //string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);


            List<SelectListItem> kodKategorilist2 = new List<SelectListItem>();
            kodKategorilist2 = new SelectList(db.tbl_KategoriAktiviti
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
                .Select(s => new SelectListItem { Value = s.fld_KodKategori.ToString(), Text = s.fld_Kategori }), "Value", "Text").ToList();
            kodKategorilist2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            ViewBag.kodKategorilist = kodKategorilist2;
            ViewBag.getflag = 2;

            if (kodKategorilist == "0")
            {
                var result = db.tbl_UpahAktiviti
                     .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_KodAktvt);
                return View(result);
            }
            else
            {
                var result = db.tbl_UpahAktiviti
                    .Where(x => x.fld_KategoriAktvt == kodKategorilist && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_KodAktvt);
                return View(result);
            }
        }


        public ActionResult CustomerReport()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> EstateList = new List<SelectListItem>();

            EstateList = new SelectList(db.tbl_Ladang.OrderBy(o => o.fld_ID).Select(s => new SelectListItem { Value = (s.fld_ID).ToString(), Text = s.fld_ID + "-" + s.fld_LdgName }), "Value", "Text").ToList();
            EstateList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
            ViewBag.EstateList = EstateList;
            //ViewBag.getflag = 1;
            return View();
        }

        public ActionResult _CustomerReport(string EstateList, string print)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<CustMod_CustSatisfaction> InfoCustSatisList = new List<CustMod_CustSatisfaction>();

            
            ViewBag.Print = print;

            if (EstateList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChooseEstate;
                return View();
            }

            if (EstateList == "0")
            {
                var result = dbr.tbl_Kepuasan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_UserID);

                foreach (var info in result)
                {
                    var getLdgInfo = db.tbl_Ladang
                        .Where(x => x.fld_ID == info.fld_LadangID)
                        .Select(s => s.fld_LdgName).FirstOrDefault();

                    var getUserInfo = db.tblUsers
                        .Where(x => x.fldUserID == info.fld_UserID)
                                    .Select(s => s.fldUserFullName).FirstOrDefault();

                    CustMod_CustSatisfaction CustSatis = new CustMod_CustSatisfaction();

                    CustSatis.UID = info.fld_UserID;
                    CustSatis.UIDNama = getUserInfo;
                    CustSatis.LdgID = info.fld_LadangID;
                    CustSatis.LdgNama = getLdgInfo;
                    CustSatis.Satis = info.fld_Kepuasan;
                    CustSatis.Note = info.fld_Catatan;
                    InfoCustSatisList.Add(CustSatis);
                }

                if (InfoCustSatisList.Count == 0)
                {
                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                }

                return View(InfoCustSatisList);
            
            }
            else
            {
                var result = dbr.tbl_Kepuasan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_LadangID == Convert.ToInt32(EstateList));

                foreach (var info in result)
                {
                    var getLdgInfo = db.tbl_Ladang
                        .Where(x => x.fld_ID == info.fld_ID)
                        .Select(s => s.fld_LdgName).FirstOrDefault();

                    var getUserInfo = db.tblUsers
                        .Where(x => x.fldUserID == info.fld_UserID)
                                    .Select(s => s.fldUserFullName).FirstOrDefault();

                    CustMod_CustSatisfaction CustSatis = new CustMod_CustSatisfaction();

                    CustSatis.UID = info.fld_UserID;
                    CustSatis.UIDNama = getUserInfo;
                    CustSatis.LdgID = info.fld_LadangID;
                    CustSatis.LdgNama = getLdgInfo;
                    CustSatis.Satis = info.fld_Kepuasan;
                    CustSatis.Note = info.fld_Catatan;
                    InfoCustSatisList.Add(CustSatis);
                }

                if (InfoCustSatisList.Count == 0)
                {
                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                }

                return View(InfoCustSatisList);
            }


        }

        public ActionResult PrintPaySlipPdf(int? RadioGroup, int? MonthList, int? YearList,
                string StatusList, string SelectionList, int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("_PaySlipRptSearch", new { RadioGroup, MonthList, YearList, StatusList, SelectionList, print })
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
 

        public ActionResult workerPbklReport()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            ViewBag.Report = "class = active";
            List<SelectListItem> PbklList = new List<SelectListItem>();
            PbklList = new SelectList(db.tbl_Pembekal.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                .OrderBy(o => o.fld_KodPbkl)
                .Select(s => new SelectListItem { Value = s.fld_KodPbkl, Text = s.fld_NamaPbkl }), "Value", "Text").ToList();
            PbklList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.PbklList = PbklList;
            ViewBag.getflag = 1;
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult _workerPbklReport(string PbklList, string print)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<Models.tbl_Pkjmast> pbklPekerja = new List<Models.tbl_Pkjmast>();

            List<SelectListItem> StatusList2 = new List<SelectListItem>();
            StatusList2 = new SelectList(db.tbl_Pembekal.Where(x => x.fld_KodPbkl == PbklList && x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                .OrderBy(o => o.fld_KodPbkl)
                .Select(s => new SelectListItem { Value = s.fld_KodPbkl, Text = s.fld_NamaPbkl }), "Value", "Text").ToList();
            StatusList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.StatusList = StatusList2;
            ViewBag.getflag = 2;
            ViewBag.Print = print;

            if (PbklList == null)
            {
                ViewBag.Message = GlobalResEstate.lblChooseSupp;
                return View(pbklPekerja);
            }

            if (PbklList == "0")
            {
                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);
                ViewBag.UserID = getuserid;
                return View(result);
            }
            else
            {
                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kodbkl == PbklList);
                ViewBag.UserID = getuserid;
                return View(result);
            }
        }

        public JsonResult GetWorkerPbklList(string pbklkod)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            
            List<SelectListItem> PbklList = new List<SelectListItem>();
            PbklList = new SelectList(db.tbl_Pembekal.Where(x => x.fld_KodPbkl == pbklkod && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                .Select(s => new SelectListItem { Value = s.fld_KodPbkl, Text = s.fld_NamaPbkl }).Distinct(), "Value", "Text").ToList();

            PbklList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "" }));

            //dbr.Dispose();
            return Json(new { PbklList });
        }

        public ActionResult workerKomisyenPbklReport()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            ViewBag.Report = "class = active";
            List<SelectListItem> StatusList = new List<SelectListItem>();
            StatusList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            StatusList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            List<SelectListItem> JnsPkjList = new List<SelectListItem>();
            JnsPkjList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            JnsPkjList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.StatusList = StatusList;
            ViewBag.JnsPkjList = JnsPkjList;
            ViewBag.getflag = 1;
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult _workerKomisyenPbklReport(string StatusList, string JnsPkjList, string print)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            DateTime lastDay = DateTime.Now;

            List<Models.tbl_Pkjmast> pbklPekerja = new List<Models.tbl_Pkjmast>();

            List<SelectListItem> StatusList2 = new List<SelectListItem>();
            StatusList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            StatusList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            List<SelectListItem> JnsPkjList2 = new List<SelectListItem>();
            JnsPkjList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            JnsPkjList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.StatusList = StatusList2;
            ViewBag.JnsPkjList = JnsPkjList2;
            ViewBag.getflag = 2;
            ViewBag.Print = print;

            if (StatusList == null && JnsPkjList == null)
            {
                ViewBag.Message = GlobalResEstate.lblChooseKomisyen;
                return View(pbklPekerja);
            }

           
            if (StatusList == "0")
            {
                if (JnsPkjList == "0")
                {
                    var app2 = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);                  

                    foreach (var ListPekerja in app2)
                    {
                        DateDiff dateDiff = new DateDiff(Convert.ToDateTime(ListPekerja.fld_Trshjw).AddDays(-1), lastDay);

                        if (dateDiff.Months >= 6)
                        {
                            pbklPekerja.Add(ListPekerja);
                        }
                    }
                       
                    if (pbklPekerja.Count() == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }

                    ViewBag.UserID = getuserid;
                    return View(pbklPekerja);
                }
                else
                {
                    var app2 = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Jenispekerja == JnsPkjList);

                    foreach (var ListPekerja in app2)
                    {
                        DateDiff dateDiff = new DateDiff(Convert.ToDateTime(ListPekerja.fld_Trshjw).AddDays(-1), lastDay);

                        if (dateDiff.Months >= 6)
                        {
                            pbklPekerja.Add(ListPekerja);
                        }
                    }

                    if (pbklPekerja.Count() == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }

                    ViewBag.UserID = getuserid;
                    return View(pbklPekerja);
                }

            }
            else
            {
                if (JnsPkjList == "0")
                {
                    var app2 = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == StatusList);

                    foreach (var ListPekerja in app2)
                    {
                        DateDiff dateDiff = new DateDiff(Convert.ToDateTime(ListPekerja.fld_Trshjw).AddDays(-1), lastDay);

                        if (dateDiff.Months >= 6)
                        {
                            pbklPekerja.Add(ListPekerja);
                        }
                    }

                    if (pbklPekerja.Count() == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }

                    ViewBag.UserID = getuserid;
                    return View(pbklPekerja);
                }
                else
                {
                    var app2 = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == StatusList && x.fld_Jenispekerja == JnsPkjList);

                    foreach (var ListPekerja in app2)
                    {
                        DateDiff dateDiff = new DateDiff(Convert.ToDateTime(ListPekerja.fld_Trshjw).AddDays(-1), lastDay);

                        if (dateDiff.Months >= 6)
                        {
                            pbklPekerja.Add(ListPekerja);
                        }
                    }

                    if (pbklPekerja.Count() == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }

                    ViewBag.UserID = getuserid;
                    return View(pbklPekerja);
                }

            }
        }

        public JsonResult GetWorkerKomisyenPbklList(string pbklkod)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
           
            List<SelectListItem> PbklList = new List<SelectListItem>();
            PbklList = new SelectList(db.tbl_Pembekal.Where(x => x.fld_KodPbkl == pbklkod && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                .Select(s => new SelectListItem { Value = s.fld_KodPbkl, Text = s.fld_NamaPbkl }).Distinct(), "Value", "Text").ToList();

            PbklList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "" }));

            //dbr.Dispose();
            return Json(new { PbklList });
        }


        //YM
        public ActionResult workerGajiKasarPbklReport()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int drpyear = 0;
            int drprangeyear = 0;
            int month = timezone.gettimezone().Month;

            List<SelectListItem> SelectionList = new List<SelectListItem>();
            SelectionList = new SelectList(
                dbr.tbl_Pkjmast
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1")
                    .OrderBy(o => o.fld_Nopkj)
                    .Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }),
                "Value", "Text").ToList();
            SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.SelectionList = SelectionList;

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

            List<SelectListItem> JnsPkjList = new List<SelectListItem>();
            JnsPkjList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            JnsPkjList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
            ViewBag.JnsPkjList = JnsPkjList;

            List<SelectListItem> PbklList = new List<SelectListItem>();
            PbklList = new SelectList(db.tbl_Pembekal.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                .OrderBy(o => o.fld_KodPbkl)
                .Select(s => new SelectListItem { Value = s.fld_KodPbkl, Text = s.fld_NamaPbkl }), "Value", "Text").ToList();
            PbklList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
            ViewBag.PbklList = PbklList;

            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ViewResult _workerGajiKasarPbklReport(int? RadioGroup, int? YearList, string SelectionList, string JnsPkjList, string PbklList, string print)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
           MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<CustMod_GajiKasar> MaklumatGajiPekerja = new List<CustMod_GajiKasar>();

            ViewBag.YearList = YearList;
            ViewBag.WorkerList = SelectionList;
            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();
            
            ViewBag.Print = print;

            if (YearList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChooseWorkGajiKasar;
                return View(MaklumatGajiPekerja);
            }
            else
            {
                if (RadioGroup == 0)
                {
                    if (SelectionList == "0")
                    {
                        if (PbklList == "0")
                        {
                            if (JnsPkjList == "0")
                            {
                                IOrderedQueryable<Models.tbl_Pkjmast> workerData;

                                workerData = dbr.tbl_Pkjmast
                                         .Where(x => x.fld_NegaraID == NegaraID &&
                                                     x.fld_SyarikatID == SyarikatID &&
                                                     x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                         .OrderBy(x => x.fld_Nama);

                                foreach (var i in workerData)
                                {
                                    List<Models.tbl_GajiBulanan> GajiBulananList = new List<Models.tbl_GajiBulanan>();

                                    for (var month = 1; month <= 12; month++)
                                    {
                                        var GajiByBulan = dbr.tbl_GajiBulanan
                                            .SingleOrDefault(x => x.fld_Nopkj == i.fld_Nopkj &&
                                                            x.fld_Month == month &&
                                                            x.fld_Year == YearList &&
                                                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                        GajiBulananList.Add(GajiByBulan);
                                    }
                                    MaklumatGajiPekerja.Add(
                                       new CustMod_GajiKasar
                                       {
                                           Pkjmast = i,
                                           GajiBulanan = GajiBulananList
                                       });
                                }
                                if (MaklumatGajiPekerja.Count == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }
                                return View(MaklumatGajiPekerja);
                            }
                            else
                            {
                                IOrderedQueryable<Models.tbl_Pkjmast> workerData;

                                workerData = dbr.tbl_Pkjmast
                                         .Where(x => x.fld_NegaraID == NegaraID && x.fld_Jenispekerja == JnsPkjList &&
                                                     x.fld_SyarikatID == SyarikatID &&
                                                     x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                         .OrderBy(x => x.fld_Nama);

                                foreach (var i in workerData)
                                {
                                    List<Models.tbl_GajiBulanan> GajiBulananList = new List<Models.tbl_GajiBulanan>();


                                    for (var month = 1; month <= 12; month++)
                                    {
                                        var GajiByBulan = dbr.tbl_GajiBulanan
                                           .SingleOrDefault(x => x.fld_Nopkj == i.fld_Nopkj &&
                                                           x.fld_Month == month &&
                                                           x.fld_Year == YearList &&
                                                           x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                           x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                        GajiBulananList.Add(GajiByBulan);
                                    }

                                    MaklumatGajiPekerja.Add(
                                       new CustMod_GajiKasar
                                       {
                                           Pkjmast = i,
                                           GajiBulanan = GajiBulananList
                                       });
                                }

                                if (MaklumatGajiPekerja.Count == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }

                                return View(MaklumatGajiPekerja);
                            }
                        }
                        else
                        {
                            if (JnsPkjList == "0")
                            {
                                IOrderedQueryable<Models.tbl_Pkjmast> workerData;

                                workerData = dbr.tbl_Pkjmast
                                         .Where(x => x.fld_Kodbkl == PbklList && x.fld_NegaraID == NegaraID && 
                                                     x.fld_SyarikatID == SyarikatID &&
                                                     x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                         .OrderBy(x => x.fld_Nama);

                                foreach (var i in workerData)
                                {
                                    List<Models.tbl_GajiBulanan> GajiBulananList = new List<Models.tbl_GajiBulanan>();

                                    for (var month = 1; month <= 12; month++)
                                    {
                                        var GajiByBulan = dbr.tbl_GajiBulanan
                                            .SingleOrDefault(x => x.fld_Nopkj == i.fld_Nopkj &&
                                                            x.fld_Month == month &&
                                                            x.fld_Year == YearList &&
                                                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                        GajiBulananList.Add(GajiByBulan);
                                    }
                                    MaklumatGajiPekerja.Add(
                                       new CustMod_GajiKasar
                                       {
                                           Pkjmast = i,
                                           GajiBulanan = GajiBulananList
                                       });
                                }
                                if (MaklumatGajiPekerja.Count == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }
                                return View(MaklumatGajiPekerja);
                            }
                            else
                            {
                                IOrderedQueryable<Models.tbl_Pkjmast> workerData;

                                workerData = dbr.tbl_Pkjmast
                                         .Where(x => x.fld_NegaraID == NegaraID && x.fld_Jenispekerja == JnsPkjList && x.fld_Kodbkl == PbklList &&
                                                     x.fld_SyarikatID == SyarikatID &&
                                                     x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                         .OrderBy(x => x.fld_Nama);

                                foreach (var i in workerData)
                                {
                                    List<Models.tbl_GajiBulanan> GajiBulananList = new List<Models.tbl_GajiBulanan>();


                                    for (var month = 1; month <= 12; month++)
                                    {
                                        var GajiByBulan = dbr.tbl_GajiBulanan
                                           .SingleOrDefault(x => x.fld_Nopkj == i.fld_Nopkj &&
                                                           x.fld_Month == month &&
                                                           x.fld_Year == YearList &&
                                                           x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                           x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                        GajiBulananList.Add(GajiByBulan);

                                    }

                                    MaklumatGajiPekerja.Add(
                                       new CustMod_GajiKasar
                                       {
                                           Pkjmast = i,
                                           GajiBulanan = GajiBulananList

                                       });
                                }

                                if (MaklumatGajiPekerja.Count == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }

                                return View(MaklumatGajiPekerja);
                            }
                        }
                        
                    }
                    else
                    {
                        IOrderedQueryable<Models.tbl_Pkjmast> workerData;

                        workerData = dbr.tbl_Pkjmast
                                 .Where(x => x.fld_NegaraID == NegaraID && x.fld_Nopkj == SelectionList &&
                                             x.fld_SyarikatID == SyarikatID &&
                                             x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                 .OrderBy(x => x.fld_Nama);

                        foreach (var i in workerData)
                        {
                            List<Models.tbl_GajiBulanan> GajiBulananList = new List<Models.tbl_GajiBulanan>();


                            for (var month = 1; month <= 12; month++)
                            {
                                var GajiByBulan = dbr.tbl_GajiBulanan
                                       .SingleOrDefault(x => x.fld_Nopkj == i.fld_Nopkj &&
                                                       x.fld_Month == month &&
                                                       x.fld_Year == YearList &&
                                                       x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                       x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                GajiBulananList.Add(GajiByBulan);

                            }

                            MaklumatGajiPekerja.Add(
                               new CustMod_GajiKasar
                               {
                                   Pkjmast = i,
                                   GajiBulanan = GajiBulananList

                               });
                        }

                        if (MaklumatGajiPekerja.Count == 0)
                        {
                            ViewBag.Message = GlobalResEstate.msgNoRecord;
                        }

                        return View(MaklumatGajiPekerja);
                    }
                }
                else //group
                {
                    if (SelectionList == "0")
                    {
                        if (PbklList == "0")
                        {
                            if (JnsPkjList == "0")
                            {
                                IOrderedQueryable<Models.tbl_Pkjmast> workerData;

                                workerData = dbr.tbl_Pkjmast
                                         .Where(x => x.fld_NegaraID == NegaraID &&
                                                     x.fld_SyarikatID == SyarikatID &&
                                                     x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                         .OrderBy(x => x.fld_Nama);

                                foreach (var i in workerData)
                                {
                                    List<Models.tbl_GajiBulanan> GajiBulananList = new List<Models.tbl_GajiBulanan>();

                                    for (var month = 1; month <= 12; month++)
                                    {
                                        var GajiByBulan = dbr.tbl_GajiBulanan
                                            .SingleOrDefault(x => x.fld_Nopkj == i.fld_Nopkj &&
                                                            x.fld_Month == month &&
                                                            x.fld_Year == YearList &&
                                                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                        GajiBulananList.Add(GajiByBulan);

                                    }


                                    MaklumatGajiPekerja.Add(
                                       new CustMod_GajiKasar
                                       {
                                           Pkjmast = i,
                                           GajiBulanan = GajiBulananList

                                       });
                                }

                                if (MaklumatGajiPekerja.Count == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }

                                return View(MaklumatGajiPekerja);
                            }
                            else
                            {
                                IOrderedQueryable<Models.tbl_Pkjmast> workerData;

                                workerData = dbr.tbl_Pkjmast
                                         .Where(x => x.fld_NegaraID == NegaraID && x.fld_Jenispekerja == JnsPkjList &&
                                                     x.fld_SyarikatID == SyarikatID &&
                                                     x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                         .OrderBy(x => x.fld_Nama);

                                foreach (var i in workerData)
                                {
                                    List<Models.tbl_GajiBulanan> GajiBulananList = new List<Models.tbl_GajiBulanan>();


                                    for (var month = 1; month <= 12; month++)
                                    {
                                        var GajiByBulan = dbr.tbl_GajiBulanan
                                           .SingleOrDefault(x => x.fld_Nopkj == i.fld_Nopkj &&
                                                           x.fld_Month == month &&
                                                           x.fld_Year == YearList &&
                                                           x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                           x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                        GajiBulananList.Add(GajiByBulan);

                                    }

                                    MaklumatGajiPekerja.Add(
                                       new CustMod_GajiKasar
                                       {
                                           Pkjmast = i,
                                           GajiBulanan = GajiBulananList

                                       });
                                }

                                if (MaklumatGajiPekerja.Count == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }

                                return View(MaklumatGajiPekerja);
                            }
                        }
                        else
                        {
                            if (JnsPkjList == "0")
                            {
                                IOrderedQueryable<Models.tbl_Pkjmast> workerData;

                                workerData = dbr.tbl_Pkjmast
                                         .Where(x => x.fld_Kodbkl == PbklList && x.fld_NegaraID == NegaraID &&
                                                     x.fld_SyarikatID == SyarikatID &&
                                                     x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                         .OrderBy(x => x.fld_Nama);

                                foreach (var i in workerData)
                                {
                                    List<Models.tbl_GajiBulanan> GajiBulananList = new List<Models.tbl_GajiBulanan>();

                                    for (var month = 1; month <= 12; month++)
                                    {
                                        var GajiByBulan = dbr.tbl_GajiBulanan
                                            .SingleOrDefault(x => x.fld_Nopkj == i.fld_Nopkj &&
                                                            x.fld_Month == month &&
                                                            x.fld_Year == YearList &&
                                                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                        GajiBulananList.Add(GajiByBulan);

                                    }


                                    MaklumatGajiPekerja.Add(
                                       new CustMod_GajiKasar
                                       {
                                           Pkjmast = i,
                                           GajiBulanan = GajiBulananList

                                       });
                                }

                                if (MaklumatGajiPekerja.Count == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }

                                return View(MaklumatGajiPekerja);
                            }
                            else
                            {
                                IOrderedQueryable<Models.tbl_Pkjmast> workerData;

                                workerData = dbr.tbl_Pkjmast
                                         .Where(x => x.fld_NegaraID == NegaraID && x.fld_Jenispekerja == JnsPkjList && x.fld_Kodbkl == PbklList &&
                                                     x.fld_SyarikatID == SyarikatID &&
                                                     x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                         .OrderBy(x => x.fld_Nama);

                                foreach (var i in workerData)
                                {
                                    List<Models.tbl_GajiBulanan> GajiBulananList = new List<Models.tbl_GajiBulanan>();


                                    for (var month = 1; month <= 12; month++)
                                    {
                                        var GajiByBulan = dbr.tbl_GajiBulanan
                                           .SingleOrDefault(x => x.fld_Nopkj == i.fld_Nopkj &&
                                                           x.fld_Month == month &&
                                                           x.fld_Year == YearList &&
                                                           x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                           x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                        GajiBulananList.Add(GajiByBulan);

                                    }

                                    MaklumatGajiPekerja.Add(
                                       new CustMod_GajiKasar
                                       {
                                           Pkjmast = i,
                                           GajiBulanan = GajiBulananList

                                       });
                                }

                                if (MaklumatGajiPekerja.Count == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }

                                return View(MaklumatGajiPekerja);
                            }
                        }

                    }
                    else
                    {
                        IOrderedQueryable<Models.tbl_Pkjmast> workerData;

                        workerData = dbr.tbl_Pkjmast
                                 .Where(x => x.fld_NegaraID == NegaraID && x.fld_Nopkj == SelectionList &&
                                             x.fld_SyarikatID == SyarikatID &&
                                             x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                 .OrderBy(x => x.fld_Nama);

                        foreach (var i in workerData)
                        {
                            List<Models.tbl_GajiBulanan> GajiBulananList = new List<Models.tbl_GajiBulanan>();


                            for (var month = 1; month <= 12; month++)
                            {
                                var GajiByBulan = dbr.tbl_GajiBulanan
                                       .SingleOrDefault(x => x.fld_Nopkj == i.fld_Nopkj &&
                                                       x.fld_Month == month &&
                                                       x.fld_Year == YearList &&
                                                       x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                       x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                GajiBulananList.Add(GajiByBulan);

                            }

                            MaklumatGajiPekerja.Add(
                               new CustMod_GajiKasar
                               {
                                   Pkjmast = i,
                                   GajiBulanan = GajiBulananList

                               });
                        }

                        if (MaklumatGajiPekerja.Count == 0)
                        {
                            ViewBag.Message = GlobalResEstate.msgNoRecord;
                        }

                        return View(MaklumatGajiPekerja);
                    }
                }
            }
        }

        public ActionResult WorkerDebtReport()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int drpyear = 0;
            int drprangeyear = 0;
            List<SelectListItem> StatusList = new List<SelectListItem>();
            List<SelectListItem> SelectionList = new List<SelectListItem>();
            List<SelectListItem> JnsPkjList = new List<SelectListItem>();

            StatusList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            StatusList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            SelectionList = new SelectList(dbr.vw_hutangPekerjaLadang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_Deleted == false).OrderBy(o => o.fld_NoPkj).Select(s => new SelectListItem { Value = s.fld_NoPkj, Text = s.fld_NoPkj + "-" + s.fld_Nama }).Distinct(), "Value", "Text").ToList();
            SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            JnsPkjList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            JnsPkjList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

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
            ViewBag.StatusList = StatusList;
            ViewBag.SelectionList = SelectionList;
            ViewBag.JnsPkjList = JnsPkjList;
            ViewBag.getflag = 1;
            return View();
        }

        public ViewResult _WorkerDebtRptSearch(int? RadioGroup, int? YearList, string StatusList, string SelectionList, string JnsPkjList, string print)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> StatusList2 = new List<SelectListItem>();
            List<SelectListItem> SelectionList2 = new List<SelectListItem>();
            List<SelectListItem> JnsPkjList2 = new List<SelectListItem>();

            List<CustMod_DebtWorker> MaklumatHutangPekerja = new List<CustMod_DebtWorker>();

            StatusList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text", StatusList).ToList();
            StatusList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            JnsPkjList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text", JnsPkjList).ToList();
            JnsPkjList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.StatusList = StatusList2;
            ViewBag.JnsPkjList = JnsPkjList2;
            ViewBag.YearList = YearList;
            ViewBag.Print = print;


            if (StatusList == null && SelectionList == null && JnsPkjList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChooseWorkDept;
                return View(MaklumatHutangPekerja);
            }
            else
            {
                if (RadioGroup == 0)
                {

                    if (StatusList == "0")
                    {
                        SelectionList2 = new SelectList(dbr.vw_hutangPekerjaLadang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                        x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_Deleted == false).OrderBy(o => o.fld_NoPkj)
                        .Select(s => new SelectListItem { Value = s.fld_NoPkj, Text = s.fld_NoPkj + "-" + s.fld_Nama }).Distinct(), "Value", "Text").ToList();
                        SelectionList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

                        if (SelectionList == "0")
                        {
                            if (JnsPkjList == "0")
                            {
                                
                                List<Models.vw_hutangPekerjaLadang> workerData;

                                workerData = dbr.vw_hutangPekerjaLadang
                                   .Where(x => x.fld_NegaraID == NegaraID && 
                                               x.fld_SyarikatID == SyarikatID &&
                                               x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).DistinctBy(d => d.fld_NoPkj)
                                   .OrderBy(x => x.fld_Nama).ToList();

                                foreach (var i in workerData)
                                {
                                    var MLoan = dbr.tbl_HutangPekerja
                                    .Where(a => a.fld_NoPkj == (i.fld_NoPkj) &&
                                                a.fld_KodHutang == "HP01" &&
                                                a.fld_NegaraID == NegaraID &&
                                                a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                                a.fld_LadangID == LadangID && a.fld_Deleted == false)
                                    .OrderBy(x => x.fld_NoPkj)
                                    .ToList();

                                    var HLoan = dbr.tbl_HutangPekerja
                                        .Where(a => a.fld_NoPkj == (i.fld_NoPkj) && a.fld_KodHutang == "HP02" &&
                                                    a.fld_NegaraID == NegaraID &&
                                                    a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                                    a.fld_LadangID == LadangID && a.fld_Deleted == false)
                                        .OrderBy(x => x.fld_NoPkj)
                                        .ToList();

                                    var JumLoan = dbr.tbl_HutangPekerjaJumlah
                                    .Where(x => x.fld_NoPkj == i.fld_NoPkj &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false)
                                                .Select(s => s.fld_JumlahHutang).SingleOrDefault();

                                    var JumBayar = dbr.tbl_HutangPekerjaJumlah
                                    .Where(x => x.fld_NoPkj == i.fld_NoPkj &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false)
                                                .Select(s => s.fld_JumlahBayar).SingleOrDefault();


                                    List<Models.tbl_Insentif> LoanDeducList = new List<Models.tbl_Insentif>();
                                    for (var month = 1; month <= 12; month++)
                                    {
                                        var DeducByBulan = dbr.tbl_Insentif
                                               .SingleOrDefault(x => x.fld_Nopkj == i.fld_NoPkj &&
                                                               x.fld_Month == month &&
                                                               x.fld_Year == YearList && x.fld_KodInsentif == "T07" &&
                                                               x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                               x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                        LoanDeducList.Add(DeducByBulan);

                                    }

                                    MaklumatHutangPekerja.Add(
                                       new CustMod_DebtWorker
                                       {
                                           Pkjmast = i,
                                           MLoan = MLoan,
                                           HLoan = HLoan,
                                           JumLoan = JumLoan,
                                           JumBayar = JumBayar,
                                           LoanDeducList = LoanDeducList
                                       });
                                }

                                if (MaklumatHutangPekerja.Count == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }

                                return View(MaklumatHutangPekerja);
                            }
                            else
                            {
                                List<Models.vw_hutangPekerjaLadang> workerData;

                                workerData = dbr.vw_hutangPekerjaLadang
                                   .Where(x => x.fld_NegaraID == NegaraID &&
                                               x.fld_SyarikatID == SyarikatID &&
                                               x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).DistinctBy(d => d.fld_NoPkj)
                                   .OrderBy(x => x.fld_Nama).ToList();

                                foreach (var i in workerData)
                                {
                                    var MLoan = dbr.tbl_HutangPekerja
                                    .Where(a => a.fld_NoPkj == (i.fld_NoPkj) &&
                                                a.fld_KodHutang == "HP01" &&
                                                a.fld_NegaraID == NegaraID &&
                                                a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                                a.fld_LadangID == LadangID && a.fld_Deleted == false)
                                    .OrderBy(x => x.fld_NoPkj)
                                    .ToList();

                                    var HLoan = dbr.tbl_HutangPekerja
                                        .Where(a => a.fld_NoPkj == (i.fld_NoPkj) && a.fld_KodHutang == "HP02" &&
                                                    a.fld_NegaraID == NegaraID &&
                                                    a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                                    a.fld_LadangID == LadangID && a.fld_Deleted == false)
                                        .OrderBy(x => x.fld_NoPkj)
                                        .ToList();

                                    var JumLoan = dbr.tbl_HutangPekerjaJumlah
                                     .Where(x => x.fld_NoPkj == i.fld_NoPkj &&
                                                 x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                 x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false)
                                                 .Select(s => s.fld_JumlahHutang).SingleOrDefault();

                                    var JumBayar = dbr.tbl_HutangPekerjaJumlah
                                    .Where(x => x.fld_NoPkj == i.fld_NoPkj &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false)
                                                .Select(s => s.fld_JumlahBayar).SingleOrDefault();


                                    List<Models.tbl_Insentif> LoanDeducList = new List<Models.tbl_Insentif>();
                                    for (var month = 1; month <= 12; month++)
                                    {
                                        var DeducByBulan = dbr.tbl_Insentif
                                               .SingleOrDefault(x => x.fld_Nopkj == i.fld_NoPkj &&
                                                               x.fld_Month == month &&
                                                               x.fld_Year == YearList && x.fld_KodInsentif == "T07" &&
                                                               x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                               x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                        LoanDeducList.Add(DeducByBulan);

                                    }

                                    MaklumatHutangPekerja.Add(
                                       new CustMod_DebtWorker
                                       {
                                           Pkjmast = i,
                                           MLoan = MLoan,
                                           HLoan = HLoan,
                                           JumLoan = JumLoan,
                                           JumBayar = JumBayar,
                                           LoanDeducList = LoanDeducList
                                       });
                                }

                                if (MaklumatHutangPekerja.Count == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }

                                return View(MaklumatHutangPekerja);
                            }

                        }
                        else
                        {
                            if (JnsPkjList == "0")
                            {
                                List<Models.vw_hutangPekerjaLadang> workerData;

                                workerData = dbr.vw_hutangPekerjaLadang
                                   .Where(x => x.fld_NegaraID == NegaraID &&
                                               x.fld_SyarikatID == SyarikatID &&
                                               x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).DistinctBy(d => d.fld_NoPkj)
                                   .OrderBy(x => x.fld_Nama).ToList();

                                foreach (var i in workerData)
                                {
                                    var MLoan = dbr.tbl_HutangPekerja
                                    .Where(a => a.fld_NoPkj == (i.fld_NoPkj) &&
                                                a.fld_KodHutang == "HP01" &&
                                                a.fld_NegaraID == NegaraID &&
                                                a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                                a.fld_LadangID == LadangID && a.fld_Deleted == false)
                                    .OrderBy(x => x.fld_NoPkj)
                                    .ToList();

                                    var HLoan = dbr.tbl_HutangPekerja
                                        .Where(a => a.fld_NoPkj == (i.fld_NoPkj) && a.fld_KodHutang == "HP02" &&
                                                    a.fld_NegaraID == NegaraID &&
                                                    a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                                    a.fld_LadangID == LadangID && a.fld_Deleted == false)
                                        .OrderBy(x => x.fld_NoPkj)
                                        .ToList();

                                    var JumLoan = dbr.tbl_HutangPekerjaJumlah
                                    .Where(x => x.fld_NoPkj == i.fld_NoPkj &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false)
                                                .Select(s => s.fld_JumlahHutang).SingleOrDefault();

                                    var JumBayar = dbr.tbl_HutangPekerjaJumlah
                                    .Where(x => x.fld_NoPkj == i.fld_NoPkj &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false)
                                                .Select(s => s.fld_JumlahBayar).SingleOrDefault();


                                    List<Models.tbl_Insentif> LoanDeducList = new List<Models.tbl_Insentif>();
                                    for (var month = 1; month <= 12; month++)
                                    {
                                        var DeducByBulan = dbr.tbl_Insentif
                                               .SingleOrDefault(x => x.fld_Nopkj == i.fld_NoPkj &&
                                                               x.fld_Month == month &&
                                                               x.fld_Year == YearList && x.fld_KodInsentif == "T07" &&
                                                               x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                               x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                        LoanDeducList.Add(DeducByBulan);

                                    }

                                    MaklumatHutangPekerja.Add(
                                       new CustMod_DebtWorker
                                       {
                                           Pkjmast = i,
                                           MLoan = MLoan,
                                           HLoan = HLoan,
                                           JumLoan = JumLoan,
                                           JumBayar = JumBayar,
                                           LoanDeducList = LoanDeducList
                                       });
                                }

                                if (MaklumatHutangPekerja.Count == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }

                                return View(MaklumatHutangPekerja);
                            }
                            else
                            {
                                List<Models.vw_hutangPekerjaLadang> workerData;

                                workerData = dbr.vw_hutangPekerjaLadang
                                   .Where(x => x.fld_NegaraID == NegaraID &&
                                               x.fld_SyarikatID == SyarikatID &&
                                               x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).DistinctBy(d => d.fld_NoPkj)
                                   .OrderBy(x => x.fld_Nama).ToList();

                                foreach (var i in workerData)
                                {
                                    var MLoan = dbr.tbl_HutangPekerja
                                    .Where(a => a.fld_NoPkj == (i.fld_NoPkj) &&
                                                a.fld_KodHutang == "HP01" &&
                                                a.fld_NegaraID == NegaraID &&
                                                a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                                a.fld_LadangID == LadangID && a.fld_Deleted == false)
                                    .OrderBy(x => x.fld_NoPkj)
                                    .ToList();

                                    var HLoan = dbr.tbl_HutangPekerja
                                        .Where(a => a.fld_NoPkj == (i.fld_NoPkj) && a.fld_KodHutang == "HP02" &&
                                                    a.fld_NegaraID == NegaraID &&
                                                    a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                                    a.fld_LadangID == LadangID && a.fld_Deleted == false)
                                        .OrderBy(x => x.fld_NoPkj)
                                        .ToList();

                                    var JumLoan = dbr.tbl_HutangPekerjaJumlah
                                    .Where(x => x.fld_NoPkj == i.fld_NoPkj &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false)
                                                .Select(s => s.fld_JumlahHutang).SingleOrDefault();

                                    var JumBayar = dbr.tbl_HutangPekerjaJumlah
                                    .Where(x => x.fld_NoPkj == i.fld_NoPkj &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false)
                                                .Select(s => s.fld_JumlahBayar).SingleOrDefault();


                                    List<Models.tbl_Insentif> LoanDeducList = new List<Models.tbl_Insentif>();
                                    for (var month = 1; month <= 12; month++)
                                    {
                                        var DeducByBulan = dbr.tbl_Insentif
                                               .SingleOrDefault(x => x.fld_Nopkj == i.fld_NoPkj &&
                                                               x.fld_Month == month &&
                                                               x.fld_Year == YearList && x.fld_KodInsentif == "T07" &&
                                                               x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                               x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                        LoanDeducList.Add(DeducByBulan);

                                    }

                                    MaklumatHutangPekerja.Add(
                                       new CustMod_DebtWorker
                                       {
                                           Pkjmast = i,
                                           MLoan = MLoan,
                                           HLoan = HLoan,
                                           JumLoan = JumLoan,
                                           JumBayar = JumBayar,
                                           LoanDeducList = LoanDeducList
                                       });
                                }

                                if (MaklumatHutangPekerja.Count == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }

                                return View(MaklumatHutangPekerja);
                            }
                        }
                    }
                    //status
                    else
                    {
                        if (JnsPkjList == "0")
                        {
                            List<Models.vw_hutangPekerjaLadang> workerData;

                            workerData = dbr.vw_hutangPekerjaLadang
                               .Where(x => x.fld_NegaraID == NegaraID &&
                                           x.fld_SyarikatID == SyarikatID &&
                                           x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).DistinctBy(d => d.fld_NoPkj)
                               .OrderBy(x => x.fld_Nama).ToList();

                            foreach (var i in workerData)
                            {
                                var MLoan = dbr.tbl_HutangPekerja
                                .Where(a => a.fld_NoPkj == (i.fld_NoPkj) &&
                                            a.fld_KodHutang == "HP01" &&
                                            a.fld_NegaraID == NegaraID &&
                                            a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                            a.fld_LadangID == LadangID && a.fld_Deleted == false)
                                .OrderBy(x => x.fld_NoPkj)
                                .ToList();

                                var HLoan = dbr.tbl_HutangPekerja
                                    .Where(a => a.fld_NoPkj == (i.fld_NoPkj) && a.fld_KodHutang == "HP02" &&
                                                a.fld_NegaraID == NegaraID &&
                                                a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                                a.fld_LadangID == LadangID && a.fld_Deleted == false)
                                    .OrderBy(x => x.fld_NoPkj)
                                    .ToList();

                                var JumLoan = dbr.tbl_HutangPekerjaJumlah
                                    .Where(x => x.fld_NoPkj == i.fld_NoPkj &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false)
                                                .Select(s => s.fld_JumlahHutang).SingleOrDefault();

                                var JumBayar = dbr.tbl_HutangPekerjaJumlah
                                .Where(x => x.fld_NoPkj == i.fld_NoPkj &&
                                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false)
                                            .Select(s => s.fld_JumlahBayar).SingleOrDefault();


                                List<Models.tbl_Insentif> LoanDeducList = new List<Models.tbl_Insentif>();
                                for (var month = 1; month <= 12; month++)
                                {
                                    var DeducByBulan = dbr.tbl_Insentif
                                           .SingleOrDefault(x => x.fld_Nopkj == i.fld_NoPkj &&
                                                           x.fld_Month == month &&
                                                           x.fld_Year == YearList && x.fld_KodInsentif == "T07" &&
                                                           x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                           x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    LoanDeducList.Add(DeducByBulan);

                                }

                                MaklumatHutangPekerja.Add(
                                   new CustMod_DebtWorker
                                   {
                                       Pkjmast = i,
                                       MLoan = MLoan,
                                       HLoan = HLoan,
                                       JumLoan = JumLoan,
                                       JumBayar = JumBayar,
                                       LoanDeducList = LoanDeducList
                                   });
                            }

                            if (MaklumatHutangPekerja.Count == 0)
                            {
                                ViewBag.Message = GlobalResEstate.msgNoRecord;
                            }

                            return View(MaklumatHutangPekerja);
                        }
                        else
                        {
                            List<Models.vw_hutangPekerjaLadang> workerData;

                            workerData = dbr.vw_hutangPekerjaLadang
                               .Where(x => x.fld_NegaraID == NegaraID &&
                                           x.fld_SyarikatID == SyarikatID &&
                                           x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).DistinctBy(d => d.fld_NoPkj)
                               .OrderBy(x => x.fld_Nama).ToList();

                            foreach (var i in workerData)
                            {
                                var MLoan = dbr.tbl_HutangPekerja
                                .Where(a => a.fld_NoPkj == (i.fld_NoPkj) &&
                                            a.fld_KodHutang == "HP01" &&
                                            a.fld_NegaraID == NegaraID &&
                                            a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                            a.fld_LadangID == LadangID && a.fld_Deleted == false)
                                .OrderBy(x => x.fld_NoPkj)
                                .ToList();

                                var HLoan = dbr.tbl_HutangPekerja
                                    .Where(a => a.fld_NoPkj == (i.fld_NoPkj) && a.fld_KodHutang == "HP02" &&
                                                a.fld_NegaraID == NegaraID &&
                                                a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                                a.fld_LadangID == LadangID && a.fld_Deleted == false)
                                    .OrderBy(x => x.fld_NoPkj)
                                    .ToList();

                                var JumLoan = dbr.tbl_HutangPekerjaJumlah
                                    .Where(x => x.fld_NoPkj == i.fld_NoPkj &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false)
                                                .Select(s => s.fld_JumlahHutang).SingleOrDefault();

                                var JumBayar = dbr.tbl_HutangPekerjaJumlah
                                .Where(x => x.fld_NoPkj == i.fld_NoPkj &&
                                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false)
                                            .Select(s => s.fld_JumlahBayar).SingleOrDefault();


                                List<Models.tbl_Insentif> LoanDeducList = new List<Models.tbl_Insentif>();
                                for (var month = 1; month <= 12; month++)
                                {
                                    var DeducByBulan = dbr.tbl_Insentif
                                           .SingleOrDefault(x => x.fld_Nopkj == i.fld_NoPkj &&
                                                           x.fld_Month == month &&
                                                           x.fld_Year == YearList && x.fld_KodInsentif == "T07" &&
                                                           x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                           x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    LoanDeducList.Add(DeducByBulan);

                                }

                                MaklumatHutangPekerja.Add(
                                   new CustMod_DebtWorker
                                   {
                                       Pkjmast = i,
                                       MLoan = MLoan,
                                       HLoan = HLoan,
                                       JumLoan = JumLoan,
                                       JumBayar = JumBayar,
                                       LoanDeducList = LoanDeducList
                                   });
                            }

                            if (MaklumatHutangPekerja.Count == 0)
                            {
                                ViewBag.Message = GlobalResEstate.msgNoRecord;
                            }

                            return View(MaklumatHutangPekerja);
                        }
                    }
                }
                else //Group
                {
                    if (StatusList == "0")
                    {
                        SelectionList2 = new SelectList(dbr.vw_hutangPekerjaLadang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                        x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_Deleted == false).OrderBy(o => o.fld_NoPkj)
                        .Select(s => new SelectListItem { Value = s.fld_NoPkj, Text = s.fld_NoPkj + "-" + s.fld_Nama }), "Value", "Text").ToList();
                        SelectionList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

                        if (SelectionList == "0")
                        {
                            if (JnsPkjList == "0")
                            {
                                List<Models.vw_hutangPekerjaLadang> workerData;

                                workerData = dbr.vw_hutangPekerjaLadang
                                   .Where(x => x.fld_NegaraID == NegaraID &&
                                               x.fld_SyarikatID == SyarikatID &&
                                               x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).DistinctBy(d => d.fld_NoPkj)
                                   .OrderBy(x => x.fld_Nama).ToList();

                                foreach (var i in workerData)
                                {
                                    var MLoan = dbr.tbl_HutangPekerja
                                    .Where(a => a.fld_NoPkj == (i.fld_NoPkj) &&
                                                a.fld_KodHutang == "HP01" &&
                                                a.fld_NegaraID == NegaraID &&
                                                a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                                a.fld_LadangID == LadangID && a.fld_Deleted == false)
                                    .OrderBy(x => x.fld_NoPkj)
                                    .ToList();

                                    var HLoan = dbr.tbl_HutangPekerja
                                        .Where(a => a.fld_NoPkj == (i.fld_NoPkj) && a.fld_KodHutang == "HP02" &&
                                                    a.fld_NegaraID == NegaraID &&
                                                    a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                                    a.fld_LadangID == LadangID && a.fld_Deleted == false)
                                        .OrderBy(x => x.fld_NoPkj)
                                        .ToList();

                                    var JumLoan = dbr.tbl_HutangPekerjaJumlah
                                    .Where(x => x.fld_NoPkj == i.fld_NoPkj &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false)
                                                .Select(s => s.fld_JumlahHutang).SingleOrDefault();

                                    var JumBayar = dbr.tbl_HutangPekerjaJumlah
                                    .Where(x => x.fld_NoPkj == i.fld_NoPkj &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false)
                                                .Select(s => s.fld_JumlahBayar).SingleOrDefault();


                                    List<Models.tbl_Insentif> LoanDeducList = new List<Models.tbl_Insentif>();
                                    for (var month = 1; month <= 12; month++)
                                    {
                                        var DeducByBulan = dbr.tbl_Insentif
                                               .SingleOrDefault(x => x.fld_Nopkj == i.fld_NoPkj &&
                                                               x.fld_Month == month &&
                                                               x.fld_Year == YearList && x.fld_KodInsentif == "T07" &&
                                                               x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                               x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                        LoanDeducList.Add(DeducByBulan);

                                    }

                                    MaklumatHutangPekerja.Add(
                                       new CustMod_DebtWorker
                                       {
                                           Pkjmast = i,
                                           MLoan = MLoan,
                                           HLoan = HLoan,
                                           JumLoan = JumLoan,
                                           JumBayar = JumBayar,
                                           LoanDeducList = LoanDeducList
                                       });
                                }

                                if (MaklumatHutangPekerja.Count == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }

                                return View(MaklumatHutangPekerja);
                            }
                            else
                            {
                                List<Models.vw_hutangPekerjaLadang> workerData;

                                workerData = dbr.vw_hutangPekerjaLadang
                                   .Where(x => x.fld_NegaraID == NegaraID &&
                                               x.fld_SyarikatID == SyarikatID &&
                                               x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).DistinctBy(d => d.fld_NoPkj)
                                   .OrderBy(x => x.fld_Nama).ToList();

                                foreach (var i in workerData)
                                {
                                    var MLoan = dbr.tbl_HutangPekerja
                                    .Where(a => a.fld_NoPkj == (i.fld_NoPkj) &&
                                                a.fld_KodHutang == "HP01" &&
                                                a.fld_NegaraID == NegaraID &&
                                                a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                                a.fld_LadangID == LadangID && a.fld_Deleted == false)
                                    .OrderBy(x => x.fld_NoPkj)
                                    .ToList();

                                    var HLoan = dbr.tbl_HutangPekerja
                                        .Where(a => a.fld_NoPkj == (i.fld_NoPkj) && a.fld_KodHutang == "HP02" &&
                                                    a.fld_NegaraID == NegaraID &&
                                                    a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                                    a.fld_LadangID == LadangID && a.fld_Deleted == false)
                                        .OrderBy(x => x.fld_NoPkj)
                                        .ToList();

                                    var JumLoan = dbr.tbl_HutangPekerjaJumlah
                                    .Where(x => x.fld_NoPkj == i.fld_NoPkj &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false)
                                                .Select(s => s.fld_JumlahHutang).SingleOrDefault();

                                    var JumBayar = dbr.tbl_HutangPekerjaJumlah
                                    .Where(x => x.fld_NoPkj == i.fld_NoPkj &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false)
                                                .Select(s => s.fld_JumlahBayar).SingleOrDefault();


                                    List<Models.tbl_Insentif> LoanDeducList = new List<Models.tbl_Insentif>();
                                    for (var month = 1; month <= 12; month++)
                                    {
                                        var DeducByBulan = dbr.tbl_Insentif
                                               .SingleOrDefault(x => x.fld_Nopkj == i.fld_NoPkj &&
                                                               x.fld_Month == month &&
                                                               x.fld_Year == YearList && x.fld_KodInsentif == "T07" &&
                                                               x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                               x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                        LoanDeducList.Add(DeducByBulan);

                                    }

                                    MaklumatHutangPekerja.Add(
                                       new CustMod_DebtWorker
                                       {
                                           Pkjmast = i,
                                           MLoan = MLoan,
                                           HLoan = HLoan,
                                           JumLoan = JumLoan,
                                           JumBayar = JumBayar,
                                           LoanDeducList = LoanDeducList
                                       });
                                }

                                if (MaklumatHutangPekerja.Count == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }

                                return View(MaklumatHutangPekerja);
                            }

                        }
                        else
                        {
                            if (JnsPkjList == "0")
                            {
                                List<Models.vw_hutangPekerjaLadang> workerData;

                                workerData = dbr.vw_hutangPekerjaLadang
                                   .Where(x => x.fld_NegaraID == NegaraID &&
                                               x.fld_SyarikatID == SyarikatID &&
                                               x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).DistinctBy(d => d.fld_NoPkj)
                                   .OrderBy(x => x.fld_Nama).ToList();

                                foreach (var i in workerData)
                                {
                                    var MLoan = dbr.tbl_HutangPekerja
                                    .Where(a => a.fld_NoPkj == (i.fld_NoPkj) &&
                                                a.fld_KodHutang == "HP01" &&
                                                a.fld_NegaraID == NegaraID &&
                                                a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                                a.fld_LadangID == LadangID && a.fld_Deleted == false)
                                    .OrderBy(x => x.fld_NoPkj)
                                    .ToList();

                                    var HLoan = dbr.tbl_HutangPekerja
                                        .Where(a => a.fld_NoPkj == (i.fld_NoPkj) && a.fld_KodHutang == "HP02" &&
                                                    a.fld_NegaraID == NegaraID &&
                                                    a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                                    a.fld_LadangID == LadangID && a.fld_Deleted == false)
                                        .OrderBy(x => x.fld_NoPkj)
                                        .ToList();

                                    var JumLoan = dbr.tbl_HutangPekerjaJumlah
                                     .Where(x => x.fld_NoPkj == i.fld_NoPkj &&
                                                 x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                 x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false)
                                                 .Select(s => s.fld_JumlahHutang).SingleOrDefault();

                                    var JumBayar = dbr.tbl_HutangPekerjaJumlah
                                    .Where(x => x.fld_NoPkj == i.fld_NoPkj &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false)
                                                .Select(s => s.fld_JumlahBayar).SingleOrDefault();


                                    List<Models.tbl_Insentif> LoanDeducList = new List<Models.tbl_Insentif>();
                                    for (var month = 1; month <= 12; month++)
                                    {
                                        var DeducByBulan = dbr.tbl_Insentif
                                               .SingleOrDefault(x => x.fld_Nopkj == i.fld_NoPkj &&
                                                               x.fld_Month == month &&
                                                               x.fld_Year == YearList && x.fld_KodInsentif == "T07" &&
                                                               x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                               x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                        LoanDeducList.Add(DeducByBulan);

                                    }

                                    MaklumatHutangPekerja.Add(
                                       new CustMod_DebtWorker
                                       {
                                           Pkjmast = i,
                                           MLoan = MLoan,
                                           HLoan = HLoan,
                                           JumLoan = JumLoan,
                                           JumBayar = JumBayar,
                                           LoanDeducList = LoanDeducList
                                       });
                                }

                                if (MaklumatHutangPekerja.Count == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }

                                return View(MaklumatHutangPekerja);
                            }
                            else
                            {
                                List<Models.vw_hutangPekerjaLadang> workerData;

                                workerData = dbr.vw_hutangPekerjaLadang
                                   .Where(x => x.fld_NegaraID == NegaraID &&
                                               x.fld_SyarikatID == SyarikatID &&
                                               x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).DistinctBy(d => d.fld_NoPkj)
                                   .OrderBy(x => x.fld_Nama).ToList();

                                foreach (var i in workerData)
                                {
                                    var MLoan = dbr.tbl_HutangPekerja
                                    .Where(a => a.fld_NoPkj == (i.fld_NoPkj) &&
                                                a.fld_KodHutang == "HP01" &&
                                                a.fld_NegaraID == NegaraID &&
                                                a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                                a.fld_LadangID == LadangID && a.fld_Deleted == false)
                                    .OrderBy(x => x.fld_NoPkj)
                                    .ToList();

                                    var HLoan = dbr.tbl_HutangPekerja
                                        .Where(a => a.fld_NoPkj == (i.fld_NoPkj) && a.fld_KodHutang == "HP02" &&
                                                    a.fld_NegaraID == NegaraID &&
                                                    a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                                    a.fld_LadangID == LadangID && a.fld_Deleted == false)
                                        .OrderBy(x => x.fld_NoPkj)
                                        .ToList();

                                    var JumLoan = dbr.tbl_HutangPekerjaJumlah
                                    .Where(x => x.fld_NoPkj == i.fld_NoPkj &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false)
                                                .Select(s => s.fld_JumlahHutang).SingleOrDefault();

                                    var JumBayar = dbr.tbl_HutangPekerjaJumlah
                                    .Where(x => x.fld_NoPkj == i.fld_NoPkj &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false)
                                                .Select(s => s.fld_JumlahBayar).SingleOrDefault();


                                    List<Models.tbl_Insentif> LoanDeducList = new List<Models.tbl_Insentif>();
                                    for (var month = 1; month <= 12; month++)
                                    {
                                        var DeducByBulan = dbr.tbl_Insentif
                                               .SingleOrDefault(x => x.fld_Nopkj == i.fld_NoPkj &&
                                                               x.fld_Month == month &&
                                                               x.fld_Year == YearList && x.fld_KodInsentif == "T07" &&
                                                               x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                               x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                        LoanDeducList.Add(DeducByBulan);

                                    }

                                    MaklumatHutangPekerja.Add(
                                       new CustMod_DebtWorker
                                       {
                                           Pkjmast = i,
                                           MLoan = MLoan,
                                           HLoan = HLoan,
                                           JumLoan = JumLoan,
                                           JumBayar = JumBayar,
                                           LoanDeducList = LoanDeducList
                                       });
                                }

                                if (MaklumatHutangPekerja.Count == 0)
                                {
                                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                                }

                                return View(MaklumatHutangPekerja);
                            }
                        }
                    }
                    //status
                    else
                    {
                        if (JnsPkjList == "0")
                        {
                            List<Models.vw_hutangPekerjaLadang> workerData;

                            workerData = dbr.vw_hutangPekerjaLadang
                               .Where(x => x.fld_NegaraID == NegaraID &&
                                           x.fld_SyarikatID == SyarikatID &&
                                           x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).DistinctBy(d => d.fld_NoPkj)
                               .OrderBy(x => x.fld_Nama).ToList();

                            foreach (var i in workerData)
                            {
                                var MLoan = dbr.tbl_HutangPekerja
                                .Where(a => a.fld_NoPkj == (i.fld_NoPkj) && a.fld_KodHutang == "HP01" && a.fld_NegaraID == NegaraID &&
                                            a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID && a.fld_Deleted == false)
                                .OrderBy(x => x.fld_NoPkj)
                                .ToList();

                                var HLoan = dbr.tbl_HutangPekerja
                                    .Where(a => a.fld_NoPkj == (i.fld_NoPkj) && a.fld_KodHutang == "HP02" && a.fld_NegaraID == NegaraID &&
                                                a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID && a.fld_LadangID == LadangID && a.fld_Deleted == false)
                                    .OrderBy(x => x.fld_NoPkj)
                                    .ToList();

                                var JumLoan = dbr.tbl_HutangPekerjaJumlah
                                    .Where(x => x.fld_NoPkj == i.fld_NoPkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false)
                                                .Select(s => s.fld_JumlahHutang).SingleOrDefault();

                                var JumBayar = dbr.tbl_HutangPekerjaJumlah
                                .Where(x => x.fld_NoPkj == i.fld_NoPkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false)
                                            .Select(s => s.fld_JumlahBayar).SingleOrDefault();

                                List<Models.tbl_Insentif> LoanDeducList = new List<Models.tbl_Insentif>();
                                for (var month = 1; month <= 12; month++)
                                {
                                    var DeducByBulan = dbr.tbl_Insentif
                                           .SingleOrDefault(x => x.fld_Nopkj == i.fld_NoPkj &&
                                                           x.fld_Month == month &&
                                                           x.fld_Year == YearList && x.fld_KodInsentif == "T07" &&
                                                           x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                           x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    LoanDeducList.Add(DeducByBulan);

                                }

                                MaklumatHutangPekerja.Add(
                                   new CustMod_DebtWorker
                                   {
                                       Pkjmast = i,
                                       MLoan = MLoan,
                                       HLoan = HLoan,
                                       JumLoan = JumLoan,
                                       JumBayar = JumBayar,
                                       LoanDeducList = LoanDeducList
                                   });
                            }

                            if (MaklumatHutangPekerja.Count == 0)
                            {
                                ViewBag.Message = GlobalResEstate.msgNoRecord;
                            }

                            return View(MaklumatHutangPekerja);
                        }
                        else
                        {
                            List<Models.vw_hutangPekerjaLadang> workerData;

                            workerData = dbr.vw_hutangPekerjaLadang
                               .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                           x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).DistinctBy(d => d.fld_NoPkj)
                               .OrderBy(x => x.fld_Nama).ToList();

                            foreach (var i in workerData)
                            {
                                var MLoan = dbr.tbl_HutangPekerja
                                .Where(a => a.fld_NoPkj == (i.fld_NoPkj) && a.fld_KodHutang == "HP01" && a.fld_NegaraID == NegaraID &&
                                            a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                            a.fld_LadangID == LadangID && a.fld_Deleted == false)
                                .OrderBy(x => x.fld_NoPkj)
                                .ToList();

                                var HLoan = dbr.tbl_HutangPekerja
                                    .Where(a => a.fld_NoPkj == (i.fld_NoPkj) && a.fld_KodHutang == "HP02" && a.fld_NegaraID == NegaraID &&
                                                a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                                a.fld_LadangID == LadangID && a.fld_Deleted == false)
                                    .OrderBy(x => x.fld_NoPkj)
                                    .ToList();

                                var JumLoan = dbr.tbl_HutangPekerjaJumlah
                                     .Where(x => x.fld_NoPkj == i.fld_NoPkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false)
                                                 .Select(s => s.fld_JumlahHutang).SingleOrDefault();

                                var JumBayar = dbr.tbl_HutangPekerjaJumlah
                                .Where(x => x.fld_NoPkj == i.fld_NoPkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false)
                                            .Select(s => s.fld_JumlahBayar).SingleOrDefault();


                                List<Models.tbl_Insentif> LoanDeducList = new List<Models.tbl_Insentif>();
                                for (var month = 1; month <= 12; month++)
                                {
                                    var DeducByBulan = dbr.tbl_Insentif
                                           .SingleOrDefault(x => x.fld_Nopkj == i.fld_NoPkj &&
                                                           x.fld_Month == month &&
                                                           x.fld_Year == YearList && x.fld_KodInsentif == "T07" &&
                                                           x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                           x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                    LoanDeducList.Add(DeducByBulan);

                                }

                                MaklumatHutangPekerja.Add(
                                   new CustMod_DebtWorker
                                   {
                                       Pkjmast = i,
                                       MLoan = MLoan,
                                       HLoan = HLoan,
                                       JumLoan = JumLoan,
                                       JumBayar = JumBayar,
                                       LoanDeducList = LoanDeducList
                                   });
                            }

                            if (MaklumatHutangPekerja.Count == 0)
                            {
                                ViewBag.Message = GlobalResEstate.msgNoRecord;
                            }

                            return View(MaklumatHutangPekerja);
                        }
                    }
                }

            }
        }


        public ActionResult WorkerInsuransReport()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> StatusList = new List<SelectListItem>();
            List<SelectListItem> SelectionList = new List<SelectListItem>();
            List<SelectListItem> JnsPkjList = new List<SelectListItem>();
            List<SelectListItem> PbklList = new List<SelectListItem>();

            StatusList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            StatusList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            SelectionList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nopkj).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }), "Value", "Text").ToList();
            SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            JnsPkjList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text", "AT").ToList();
            JnsPkjList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            PbklList = new SelectList(db.tbl_Pembekal.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                .OrderBy(o => o.fld_KodPbkl)
                .Select(s => new SelectListItem { Value = s.fld_KodPbkl, Text = s.fld_NamaPbkl }), "Value", "Text").ToList();
            PbklList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.StatusList = StatusList;
            ViewBag.SelectionList = SelectionList;
            ViewBag.JnsPkjList = JnsPkjList;
            ViewBag.PbklList = PbklList;
            ViewBag.getflag = 1;
            return View();
        }

        public ViewResult _WorkerInsuransRptSearch(int? RadioGroup, string StatusList, string SelectionList, string JnsPkjList, string PbklList, string print)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> StatusList2 = new List<SelectListItem>();
            List<SelectListItem> SelectionList2 = new List<SelectListItem>();
            List<SelectListItem> JnsPkjList2 = new List<SelectListItem>();
            List<SelectListItem> PbklList2 = new List<SelectListItem>();

            List<Models.tbl_Pkjmast> pbklPekerja = new List<Models.tbl_Pkjmast>();

            StatusList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text", StatusList).ToList();
            StatusList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            JnsPkjList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj"  && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text", JnsPkjList).ToList();
            JnsPkjList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            PbklList2 = new SelectList(db.tbl_Pembekal.Where(x => x.fld_KodPbkl == PbklList && x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                .OrderBy(o => o.fld_KodPbkl)
                .Select(s => new SelectListItem { Value = s.fld_KodPbkl, Text = s.fld_NamaPbkl }), "Value", "Text").ToList();
            PbklList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.StatusList = StatusList2;
            ViewBag.JnsPkjList = JnsPkjList2;
            ViewBag.PbklList = PbklList2;
            ViewBag.Print = print;

            if (StatusList == null && SelectionList == null && JnsPkjList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChooseWorkInsurans;
                return View(pbklPekerja);
            }
            else
            {
                if (RadioGroup == 0)
                {
                    if (StatusList == "0")
                    {
                        SelectionList2 = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                        x.fld_LadangID == LadangID && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nopkj)
                        .Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }), "Value", "Text").ToList();
                        SelectionList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

                        if (SelectionList == "0")
                        {
                            if (PbklList == "0")
                            {
                                if (JnsPkjList == "0")
                                {
                                    var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID );
                                    ViewBag.UserID = getuserid;
                                    return View(result);
                                }
                                else
                                {
                                    var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Jenispekerja == JnsPkjList);
                                    ViewBag.UserID = getuserid;
                                    return View(result);
                                }
                            }
                            else
                            {
                                if (JnsPkjList == "0")
                                {
                                    var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kodbkl == PbklList);
                                    ViewBag.UserID = getuserid;
                                    return View(result);
                                }
                                else
                                {
                                    var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kodbkl == PbklList && x.fld_Jenispekerja == JnsPkjList);
                                    ViewBag.UserID = getuserid;
                                    return View(result);
                                }
                            }                         
                         }
                        else
                        {
                            if (JnsPkjList == "0")
                            {
                                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == SelectionList);
                                ViewBag.UserID = getuserid;
                                return View(result);
                            }
                            else
                            {
                                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == SelectionList && x.fld_Jenispekerja == JnsPkjList);
                                ViewBag.UserID = getuserid;
                                return View(result);
                            }
                        }
                    }
                    //status
                    else
                    {
                        if (JnsPkjList == "0")
                        {
                            var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == StatusList);
                            ViewBag.UserID = getuserid;
                            return View(result);
                        }
                        else
                        {
                            var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == StatusList && x.fld_Jenispekerja == JnsPkjList);
                            ViewBag.UserID = getuserid;
                            return View(result);
                        }
                    }
                }
                else //Group
                {
                    if (StatusList == "0")
                    {
                        SelectionList2 = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                        x.fld_LadangID == LadangID && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nopkj)
                        .Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }), "Value", "Text").ToList();
                        SelectionList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

                        if (SelectionList == "0")
                        {
                            if (PbklList == "0")
                            {
                                if (JnsPkjList == "0")
                                {
                                    var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);
                                    ViewBag.UserID = getuserid;
                                    return View(result);
                                }
                                else
                                {
                                    var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Jenispekerja == JnsPkjList);
                                    ViewBag.UserID = getuserid;
                                    return View(result);
                                }
                            }
                            else
                            {
                                if (JnsPkjList == "0")
                                {
                                    var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kodbkl == PbklList);
                                    ViewBag.UserID = getuserid;
                                    return View(result);
                                }
                                else
                                {
                                    var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kodbkl == PbklList && x.fld_Jenispekerja == JnsPkjList);
                                    ViewBag.UserID = getuserid;
                                    return View(result);
                                }
                            }
                        }
                        else
                        {
                            if (JnsPkjList == "0")
                            {
                                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == SelectionList);
                                ViewBag.UserID = getuserid;
                                return View(result);
                            }
                            else
                            {
                                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == SelectionList && x.fld_Jenispekerja == JnsPkjList);
                                ViewBag.UserID = getuserid;
                                return View(result);
                            }
                        }
                    }
                    //status
                    else
                    {
                        if (JnsPkjList == "0")
                        {
                            var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == StatusList);
                            ViewBag.UserID = getuserid;
                            return View(result);
                        }
                        else
                        {
                            var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == StatusList && x.fld_Jenispekerja == JnsPkjList);
                            ViewBag.UserID = getuserid;
                            return View(result);
                        }
                    }
                }
            }
        }

        public ActionResult WorkerContribution()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);

            int drpyear = 0;
            int drprangeyear = 0;
            int month = timezone.gettimezone().Month;

            List<SelectListItem> SelectionList = new List<SelectListItem>();
            SelectionList = new SelectList(
                dbr.tbl_Pkjmast
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_DivisionID == DivisionID)
                    .OrderBy(o => o.fld_Nama)
                    .Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }),
                "Value", "Text").ToList();
            SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.SelectionList = SelectionList;

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

            var monthList = new SelectList(
                db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false &&
                                                   x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID),
                "fldOptConfValue", "fldOptConfDesc", month);


            List<SelectListItem> JnsPkjList = new List<SelectListItem>();

            JnsPkjList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" &&
            x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue)
            .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            JnsPkjList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.MonthList = monthList;
            ViewBag.JnsPkjList = JnsPkjList;

            return View();
        }

        public ViewResult _WorkerContribution(int? MonthList, int? YearList, string SelectionList, string JnsPkjList, string print)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            List<Models.tbl_GajiBulanan> tbl_GajiBulananList = new List<Models.tbl_GajiBulanan>();
            List<Models.tbl_Insentif> tbl_InsentifList = new List<Models.tbl_Insentif>();
            List<Models.tbl_Pkjmast> tbl_PkjmastList = new List<Models.tbl_Pkjmast>();
            List<tbl_ByrCarumanTambahan> tbl_ByrCarumanTambahanList = new List<tbl_ByrCarumanTambahan>();
            List<ContributionReport> ContributionReportList = new List<ContributionReport>();
            decimal? TotalInsentifEfected = 0;
            decimal? TotalSalaryForKWSP = 0;
            decimal? TotalSalaryForPerkeso = 0;
            decimal? BakiCutiTahunan = 0;
            decimal? KWSPEmplyee = 0;
            decimal? KWSPEmplyer = 0;
            decimal? SocsoEmplyee = 0;
            decimal? SocsoEmplyer = 0;
            decimal? SIPEmplyee = 0;
            decimal? SIPEmplyer = 0;
            decimal? SBKPEmplyee = 0;
            decimal? SBKPEmplyer = 0;
            decimal? PCBEmplyee = 0;
            decimal? PCBEmplyer = 0;
            int ID = 1;
            string WorkerName = "";
            string WorkerIDNo = "";
            string WorkerSocsoNo = "";

            decimal? NUPWEmplyee = 0;

            var GetInsetifEffectCode = db.tbl_JenisInsentif.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_JenisInsentif == "P" && x.fld_AdaCaruman == true && x.fld_Deleted == false).Select(s => s.fld_KodInsentif).ToList();
            //var GetAddContributionDetails = db.tbl_SubCarumanTambahan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).ToList();
            if (SelectionList == "0")
            {
                tbl_PkjmastList = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_DivisionID == DivisionID).ToList();

                if (JnsPkjList == "0")
                {
                    var GetNoPkjas = tbl_PkjmastList.Select(s => s.fld_Nopkj).ToList();
                    tbl_GajiBulananList = dbr.tbl_GajiBulanan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && GetNoPkjas.Contains(x.fld_Nopkj) && x.fld_Month == MonthList && x.fld_Year == YearList).ToList();
                    tbl_InsentifList = dbr.tbl_Insentif.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && GetNoPkjas.Contains(x.fld_Nopkj) && GetInsetifEffectCode.Contains(x.fld_KodInsentif) && x.fld_Deleted == false && x.fld_Month == MonthList && x.fld_Year == YearList).ToList();
                }
                else
                {
                    var GetNoPkjas = tbl_PkjmastList.Where(x => x.fld_Jenispekerja == JnsPkjList).Select(s => s.fld_Nopkj).ToList();
                    tbl_GajiBulananList = dbr.tbl_GajiBulanan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && GetNoPkjas.Contains(x.fld_Nopkj) && x.fld_Month == MonthList && x.fld_Year == YearList).ToList();
                    tbl_InsentifList = dbr.tbl_Insentif.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && GetNoPkjas.Contains(x.fld_Nopkj) && GetInsetifEffectCode.Contains(x.fld_KodInsentif) && x.fld_Deleted == false && x.fld_Month == MonthList && x.fld_Year == YearList).ToList();
                }
            }
            else
            {
                tbl_PkjmastList = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_DivisionID == DivisionID && x.fld_Nopkj == SelectionList).ToList();
                tbl_GajiBulananList = dbr.tbl_GajiBulanan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == SelectionList && x.fld_Month == MonthList && x.fld_Year == YearList).ToList();
                tbl_InsentifList = dbr.tbl_Insentif.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == SelectionList && GetInsetifEffectCode.Contains(x.fld_KodInsentif) && x.fld_Deleted == false && x.fld_Month == MonthList && x.fld_Year == YearList).ToList();
            }

            var GetGajiBulananID = tbl_GajiBulananList.Select(s => s.fld_ID).ToList();
            tbl_ByrCarumanTambahanList = dbr.tbl_ByrCarumanTambahan.Where(x => GetGajiBulananID.Contains(x.fld_GajiID.Value)).ToList();

            foreach (var GajiBulananDetail in tbl_GajiBulananList)
            {
                KWSPEmplyee = 0;
                KWSPEmplyer = 0;
                SocsoEmplyee = 0;
                SocsoEmplyer = 0;
                SIPEmplyee = 0;
                SIPEmplyer = 0;
                SBKPEmplyee = 0;
                SBKPEmplyer = 0;
                PCBEmplyee = 0;
                PCBEmplyer = 0;
                NUPWEmplyee = 0;

                TotalInsentifEfected = tbl_InsentifList.Where(x => x.fld_Nopkj == GajiBulananDetail.fld_Nopkj).Sum(s => s.fld_NilaiInsentif);
                TotalInsentifEfected = TotalInsentifEfected == null ? 0 : TotalInsentifEfected;
                BakiCutiTahunan = GajiBulananDetail.fld_BakiCutiTahunan == null ? 0 : GajiBulananDetail.fld_BakiCutiTahunan;
                //modified by faeza 04.01.2023
                //TotalSalaryForKWSP = GajiBulananDetail.fld_ByrKerja + GajiBulananDetail.fld_ByrCuti + GajiBulananDetail.fld_BonusHarian + TotalInsentifEfected + GajiBulananDetail.fld_AIPS + GajiBulananDetail.fld_ByrKwsnSkr;
                //TotalSalaryForPerkeso = GajiBulananDetail.fld_ByrKerja + GajiBulananDetail.fld_ByrCuti + GajiBulananDetail.fld_OT + TotalInsentifEfected + GajiBulananDetail.fld_AIPS + GajiBulananDetail.fld_ByrKwsnSkr;
                TotalSalaryForKWSP = GajiBulananDetail.fld_ByrKerja + GajiBulananDetail.fld_ByrCuti + GajiBulananDetail.fld_BonusHarian + TotalInsentifEfected + GajiBulananDetail.fld_AIPS + GajiBulananDetail.fld_ByrKwsnSkr + BakiCutiTahunan;
                TotalSalaryForPerkeso = GajiBulananDetail.fld_ByrKerja + GajiBulananDetail.fld_ByrCuti + GajiBulananDetail.fld_BonusHarian + GajiBulananDetail.fld_OT + TotalInsentifEfected + GajiBulananDetail.fld_AIPS + GajiBulananDetail.fld_ByrKwsnSkr + BakiCutiTahunan;

                KWSPEmplyee = GajiBulananDetail.fld_KWSPPkj;
                KWSPEmplyer = GajiBulananDetail.fld_KWSPMjk;

                SocsoEmplyee = GajiBulananDetail.fld_SocsoPkj;
                SocsoEmplyer = GajiBulananDetail.fld_SocsoMjk;

                //fatin added - 27/12/2023
                tbl_ByrCarumanTambahanList = dbr.tbl_ByrCarumanTambahan.Where(x => x.fld_GajiID == GajiBulananDetail.fld_ID).ToList();

                foreach (var CarumanTambahan in tbl_ByrCarumanTambahanList)
                {
                    //original code
                    //var GetAddContribution = tbl_ByrCarumanTambahanList.Where(x => x.fld_GajiID == GajiBulananDetail.fld_ID).FirstOrDefault();

                    //if (GetAddContribution != null)
                    //{
                    //    if (GetAddContribution.fld_KodCaruman == "SIP")
                    //    {
                    //        SIPEmplyee = GetAddContribution.fld_CarumanPekerja;
                    //        SIPEmplyer = GetAddContribution.fld_CarumanMajikan;
                    //    }
                    //    else
                    //    {
                    //        SBKPEmplyee = GetAddContribution.fld_CarumanPekerja;
                    //        SBKPEmplyer = GetAddContribution.fld_CarumanMajikan;
                    //    }

                    //}

                    if (CarumanTambahan != null)
                    {
                        if (CarumanTambahan.fld_KodCaruman == "SIP")
                        {
                            SIPEmplyee = CarumanTambahan.fld_CarumanPekerja;
                            SIPEmplyer = CarumanTambahan.fld_CarumanMajikan;
                        }
                        if (CarumanTambahan.fld_KodCaruman == "SBKP")
                        {
                            SBKPEmplyee = CarumanTambahan.fld_CarumanPekerja;
                            SBKPEmplyer = CarumanTambahan.fld_CarumanMajikan;
                        }
                        if (CarumanTambahan.fld_KodCaruman == "PCB")
                        {
                            PCBEmplyee = CarumanTambahan.fld_CarumanPekerja;
                            PCBEmplyer = CarumanTambahan.fld_CarumanMajikan;
                        }
                        if (CarumanTambahan.fld_KodCaruman == "NUPW")
                        {
                            NUPWEmplyee = CarumanTambahan.fld_CarumanPekerja;
                        }

                    }
                }

                WorkerName = tbl_PkjmastList.Where(x => x.fld_Nopkj == GajiBulananDetail.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                WorkerIDNo = tbl_PkjmastList.Where(x => x.fld_Nopkj == GajiBulananDetail.fld_Nopkj).Select(s => s.fld_Nokp).FirstOrDefault();
                WorkerSocsoNo = tbl_PkjmastList.Where(x => x.fld_Nopkj == GajiBulananDetail.fld_Nopkj).Select(s => s.fld_Noperkeso).FirstOrDefault();

                //original code
                //if (SBKPEmplyer != 0 || SocsoEmplyer != 0 || KWSPEmplyer != 0 )
                if (SIPEmplyer != 0 || SBKPEmplyer != 0 || SocsoEmplyer != 0 || KWSPEmplyer != 0 || PCBEmplyer != 0 || NUPWEmplyee !=0)
                {
                    ContributionReportList.Add(new ContributionReport() { ID = ID, WorkerName = WorkerName, TotalSalaryForKwsp = TotalSalaryForKWSP.Value, TotalSalaryForPerkeso = TotalSalaryForPerkeso.Value, KwspContributionEmplyee = KWSPEmplyee.Value, KwspContributionEmplyer = KWSPEmplyer.Value, SipContributionEmplyee = SIPEmplyee.Value, SipContributionEmplyer = SIPEmplyer.Value, SocsoContributionEmplyee = SocsoEmplyee.Value, SocsoContributionEmplyer = SocsoEmplyer.Value, SbkpContributionEmplyee = SBKPEmplyee.Value, SbkpContributionEmplyer = SBKPEmplyer.Value, PcbContributionEmplyee = PCBEmplyee.Value, PcbContributionEmplyer = PCBEmplyer.Value, WorkerNo = GajiBulananDetail.fld_Nopkj, WorkerIDNo = WorkerIDNo , WorkerSocsoNo = WorkerSocsoNo, NUPWContributionEmplyee = NUPWEmplyee.Value });
                    ID++;
                }
            }

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.YearSelection = YearList;
            ViewBag.MonthSelection = MonthList;

            return View(ContributionReportList);
        }

        public ActionResult PCBReport()
        {
            return RedirectToAction("Index", "PCBReport");
        }

        public ActionResult PCB2FormReport()
        {
            return RedirectToAction("Index", "PCB2FormReport");
        }

        public ActionResult EAFormReport()
        {
            return RedirectToAction("Index", "EAFormReport");
        }

        public ActionResult PenggajianDataReport()
        {
            int[] wlyhid = new int[] { };
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int year = timezone.gettimezone().Year;
            int month = timezone.gettimezone().AddMonths(-1).Month;
            int drpyear = 0;
            int drprangeyear = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? wilayahselection = 0;
            int? ladangselection = 0;

            ViewBag.Report = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

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

            List<SelectListItem> companyList = new List<SelectListItem>();
            List<SelectListItem> wilayahList = new List<SelectListItem>();
            List<SelectListItem> ladangList = new List<SelectListItem>();
            companyList = new SelectList(
               db.tbl_Syarikat
                   .Where(x => x.fld_NegaraID == NegaraID && x.fld_Deleted == false).OrderBy(o => o.fld_NamaSyarikat)
                   .Select(
                       s => new SelectListItem { Value = s.fld_NamaPndkSyarikat.ToString(), Text = s.fld_NamaSyarikat }), "Value", "Text").ToList();
            companyList.Insert(0, new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" });
            wilayahList.Insert(0, new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" });
            ladangList.Insert(0, new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" });

            ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);
            ViewBag.YearList = yearlist;
            ViewBag.CompCodeList = companyList;
            ViewBag.UserID = getuserid;
            ViewBag.WilayahList = wilayahList;
            ViewBag.LadangList = ladangList;

            return View();
        }

        public ViewResult _PenggajianDataReport(string CompCodeList,int? YearList)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            DateTime? CurrentDT = timezone.gettimezone();
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            string costcenter = db.tbl_Ladang.Where(x => x.fld_ID == LadangID).Select(x => x.fld_CostCentre).FirstOrDefault();
            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == costcenter).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NomborSyarikat = db.tbl_Syarikat.Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == costcenter).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.Tahun = YearList;

            var namapenuhsyarikat = GetConfig.GetSyarikatFullName(costcenter);
            if (namapenuhsyarikat == "") { namapenuhsyarikat = "-"; }
            var namasyarikat = GetConfig.GetSyarikatName(costcenter);
            var nosyarikat = GetConfig.GetSyarikatName(costcenter);
            ViewBag.costcentre = costcenter;
            ViewBag.namapenuhsyarikat = namapenuhsyarikat.ToUpper();

            List<ModelsDapper.sp_PenggajianData_Est_Result> PenggajianDataResult = new List<ModelsDapper.sp_PenggajianData_Est_Result>();

            var fullconenction = db.tblConnections.Where(x => x.wilayahID == WilayahID && x.fld_Purpose == "checkroll" && x.deleted == false).FirstOrDefault();
            SqlConnection conn = new SqlConnection(
             new SqlConnectionStringBuilder()
             {
                 DataSource = fullconenction.DataSourceInternal,
                 InitialCatalog = fullconenction.InitialCatalog,
                 UserID = fullconenction.userID,
                 Password = fullconenction.Password
             }.ConnectionString
                );

            string ConnEst = conn.ConnectionString;
            var con = new SqlConnection(ConnEst);

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("NegaraID", NegaraID);
            parameters.Add("SyarikatID", SyarikatID);
            parameters.Add("WilayahID", WilayahID);
            parameters.Add("LadangID", LadangID);
            parameters.Add("LevelAccess", 1);
            parameters.Add("Month", 0);
            parameters.Add("Year", YearList);
            parameters.Add("UserID", getuserid);
            parameters.Add("SelectionCategory", 1);

            con.Open();
            Dapper.SqlMapper.Settings.CommandTimeout = 3600;
            PenggajianDataResult = SqlMapper.Query<ModelsDapper.sp_PenggajianData_Est_Result>(con, "sp_PenggajianData_Est", parameters).ToList();
            con.Close();
            return View(PenggajianDataResult);
        }
        public JsonResult GetWilayah(string SyarikatID)
        {
            List<SelectListItem> ladanglist = new List<SelectListItem>();
            List<SelectListItem> wilayahlist = new List<SelectListItem>();

            int? NegaraID = 0;
            int? SyarikatID1 = 0;
            int? WilayahID2 = 0;
            int? LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int[] wlyhid = new int[] { };

            SyarikatID1 = db.tbl_Syarikat.Where(x => x.fld_NamaPndkSyarikat == SyarikatID && x.fld_Deleted == false).Select(x => x.fld_SyarikatID).FirstOrDefault();
            GetNSWL.GetData(out NegaraID, out SyarikatID1, out WilayahID2, out LadangID, getuserid, User.Identity.Name);
            {
                if (SyarikatID == "0")
                {
                }
                else
                {
                    wlyhid = getwilyah.GetWilayahID(SyarikatID1);
                    wilayahlist = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                }
            }

            return Json(wilayahlist);
        }
        public JsonResult GetLadangWil(string SyarikatID, int WilayahID)
        {
            List<SelectListItem> ladanglist = new List<SelectListItem>();
            List<SelectListItem> wilayahlist = new List<SelectListItem>();

            int? NegaraID = 0;
            int? SyarikatID1 = 0;
            int? WilayahID2 = 0;
            int? LadangID1 = 0;
            //int? getuserid = getidentity.ID(User.Identity.Name);

            //GetNSWL.GetData(out NegaraID, out SyarikatID1, out WilayahID2, out LadangID1, getuserid, User.Identity.Name);

            if (SyarikatID == "0")
            {
                //wilayahlist = new SelectList(db5.vw_NSWL.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted_L == false).Distinct().OrderBy(o => o.fld_NamaWilayah).Select(s => new SelectListItem { Value = s.fld_WilayahID.ToString(), Text = s.fld_NamaWilayah }), "Value", "Text").ToList();
            }
            else
            {
                ladanglist = new SelectList(db.vw_NSWL.Where(x => x.fld_CostCentre == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted_L == false).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList();
            }
            return Json(ladanglist);
        }

    }
}