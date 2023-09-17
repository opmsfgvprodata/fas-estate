using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.Models;
//using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.log;
using Itenso.TimePeriod;
using System.Data.Entity;
using MVC_SYSTEM.ViewingModels;
using tbl_CutiPeruntukan = MVC_SYSTEM.Models.tbl_CutiPeruntukan;
using tbl_Produktiviti = MVC_SYSTEM.Models.tbl_Produktiviti;

namespace MVC_SYSTEM.Controllers
{
    public class ApprovalController : Controller
    {
        //Check_Balik
        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
        private GetIdentity GetIdentity = new GetIdentity();
        private GetNSWL GetNSWL = new GetNSWL();
        private GetWilayah GetWilayah = new GetWilayah();
        private Connection Connection = new Connection();
        private errorlog geterror = new errorlog();

        // GET: Approval
        public ActionResult Index()
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? getroleid = GetIdentity.getRoleID(getuserid);
            int?[] reportid = new int?[] { };

            ViewBag.Approval = "class = active";
            //ViewBag.TableInfoList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jadualUpah" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc");
            ViewBag.ApprovalList = new SelectList(db.tblMenuLists.Where(x => x.fld_Flag == "approval" && x.fldDeleted == false), "fld_Val", "fld_Desc");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string ApprovalList)
        {
            return RedirectToAction(ApprovalList, "Approval");
        }

        public ActionResult NewWorker(int WilayahIDList=0, int LadangIDList=0)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
            List<SelectListItem> LadangIDList2 = new List<SelectListItem>();
            if (WilayahID == 0 && LadangID == 0)
            {
                WilayahIDList2 = new SelectList(db.tbl_Wilayah.Where(x =>x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                WilayahIDList2.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));
                LadangIDList2 = new SelectList(db.tbl_Ladang.Where(x =>x.fld_Deleted == false).OrderBy(o=>o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode+" - "+s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList2.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));
                ViewBag.WilayahIDList = WilayahIDList2;
                ViewBag.LadangIDList = LadangIDList2;
                var resultreport = db.tblASCApprovalFileDetails.Where(x => x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID).OrderBy(o => o.fldWilayahID);
                return View("NewWorker", resultreport);
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                WilayahIDList2 = new SelectList(db.tbl_Wilayah.Where(x =>x.fld_ID== WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                LadangIDList2 = new SelectList(db.tbl_Ladang.Where(x =>x.fld_WlyhID== WilayahID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList2.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));
                ViewBag.WilayahIDList = WilayahIDList2;
                ViewBag.LadangIDList = LadangIDList2;
                var resultreport = db.tblASCApprovalFileDetails.Where(x =>x.fldWilayahID==WilayahID && x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID).OrderBy(o => o.fldWilayahID);
                return View("NewWorker", resultreport);
            }
            else 
            {
                WilayahIDList2 = new SelectList(db.tbl_Wilayah.Where(x => x.fld_ID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                LadangIDList2 = new SelectList(db.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahID && x.fld_ID==LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                ViewBag.WilayahIDList = WilayahIDList2;
                ViewBag.LadangIDList = LadangIDList2;
                var resultreport = db.tblASCApprovalFileDetails.Where(x =>x.fldWilayahID==WilayahID && x.fldLadangID==LadangID && x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID).OrderBy(o => o.fldWilayahID);
                ViewBag.resultcount = resultreport.Count();
                return View("NewWorker", resultreport);
            }
            
        }

        public PartialViewResult NewWorkerDetail(int fileID)
        {
            var result = db.tblPkjmastApps.Where(x => x.fldFileID == fileID && x.fldStatus == 2);
            //var result = db.tblPkjmastApps.Where(x => x.fldFileID == fileID);
            ViewBag.Datacount = result.Count();
            return PartialView("NewWorkerDetail", result);
        }

        public JsonResult GetLadang(int WilayahID)
        {
            List<SelectListItem> ladanglist = new List<SelectListItem>();

            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID2 = 0;
            int? LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID2, out LadangID, getuserid, User.Identity.Name);

            if (GetWilayah.GetAvailableWilayah(SyarikatID))
            {
                if (WilayahID == 0)
                {
                    ladanglist = new SelectList(db.tbl_Ladang.Where(x => x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                }
                else
                {
                    ladanglist = new SelectList(db.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                }
            }

            return Json(ladanglist);
        }

        //public JsonResult ActionApprove (int act, int id, string sbbTolak)
        //{
            //Boolean status = false;
            //if (act==1)
            //{
            //    //approve(1)
            //    var app1 = db.tblPkjmastApps.Where(x => x.fldID == id).FirstOrDefault();
            //    app1.fldStatus = 1;
            //    app1.fldDateTimeApprove = DateTime.Now;
            //    app1.fldActionBy= GetIdentity.ID(User.Identity.Name);
            //    db.SaveChanges();

            //    string nopkj = app1.fldNoPkj;
            //    int ldgID = app1.fldLadangID.Value;
            //    int wlyhID = app1.fldWilayahID.Value;
            //    int syrktID = app1.fldSyarikatID.Value;
            //    int ngraID = app1.fldNegaraID.Value;
            //    string host, catalog, user, pass = "";
            //    Connection.GetConnection(out host, out catalog, out user, out pass, wlyhID, syrktID, ngraID);
            //    MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            //    MVC_SYSTEM_Viewing dbview2 = new MVC_SYSTEM_Viewing();

            //    if (app1.fldSbbMsk.Trim()== "PL")
            //    {
            //        var workerData = dbr.tbl_Pkjmast
            //            .Single(x => x.fld_Nopkj == nopkj && x.fld_LadangID == ldgID &&
            //                         x.fld_WilayahID == wlyhID && x.fld_NegaraID == ngraID);

            //        string host2, catalog2, user2, pass2= "";
            //        Connection.GetConnection(out host2, out catalog2, out user2, out pass2, app1.fldWilayahAsal, syrktID, ngraID);
            //        MVC_SYSTEM_Models dbr2 = MVC_SYSTEM_Models.ConnectToSqlServer(host2, catalog2, user2, pass2);

            //        List<tbl_CutiPeruntukan> copyleaves = dbr2.tbl_CutiPeruntukan
            //            .Where(x => x.fld_NoPkj == workerData.fld_IDpkj && x.fld_NegaraID == ngraID && x.fld_SyarikatID == syrktID && x.fld_WilayahID == app1.fldWilayahAsal && x.fld_LadangID == app1.fldLadangAsal && x.fld_Deleted == false).ToList();

            //        var saveLeave = dbr.tbl_CutiPeruntukan.Where(x => x.fld_NoPkj == nopkj);
            //        if(saveLeave == null)
            //        {
            //            foreach(var item in copyleaves)
            //            {
            //                Models.tbl_CutiPeruntukan tblLeaves = new tbl_CutiPeruntukan();
            //                tblLeaves.fld_KodCuti = item.fld_KodCuti;
            //                tblLeaves.fld_NoPkj = nopkj;
            //                tblLeaves.fld_Tahun = item.fld_Tahun;
            //                tblLeaves.fld_JumlahCuti = item.fld_JumlahCuti;
            //                tblLeaves.fld_JumlahCutiDiambil = item.fld_JumlahCutiDiambil;
            //                tblLeaves.fld_NegaraID = ngraID;
            //                tblLeaves.fld_SyarikatID = syrktID;
            //                tblLeaves.fld_WilayahID = wlyhID;
            //                tblLeaves.fld_LadangID = ldgID;
            //                tblLeaves.fld_Deleted = false;
            //                dbr.tbl_CutiPeruntukan.Add(tblLeaves);
            //                dbr.SaveChanges();
            //                status = true;
            //            }                        
            //        }
            //    }
            //   else
            //    {
            //        var workerData = dbr.tbl_Pkjmast
            //            .Single(x => x.fld_Nopkj == nopkj && x.fld_LadangID == ldgID &&
            //                         x.fld_WilayahID == wlyhID && x.fld_NegaraID == ngraID);
            //        try
            //        {
            //            int year = DateTime.Now.Year;
            //            DateTime lastDay = new DateTime(year, 12, 31);

            //            var app2 = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == nopkj).FirstOrDefault();
            //            app2.fld_Kdaktf = "1";
            //            app2.fld_StatusAkaun = "1";
            //            app2.fld_StatusApproved = 1;
            //            app2.fld_ActionBy = User.Identity.Name;
            //            app2.fld_ActionDate = DateTime.Now;

            //            DateDiff dateDiff = new DateDiff(Convert.ToDateTime(workerData.fld_Trmlkj).AddDays(-1), lastDay);

            //            var kodCutiUmum = db.tblOptionConfigsWebs
            //                .SingleOrDefault(x => x.fldOptConfFlag1 == "cutiUmumFlag" && x.fldDeleted == false
            //                                                                          && x.fld_NegaraID == ngraID &&
            //                                                                          x.fld_SyarikatID == syrktID)
            //                .fldOptConfValue;

            //            var leaveCategoryData = db.tbl_CutiKategori.Where(x =>
            //                x.fld_KodCuti != kodCutiUmum && x.fld_NegaraID == ngraID && x.fld_SyarikatID == syrktID &&
            //                x.fld_Deleted == false);

            //            foreach (var leaveCategory in leaveCategoryData)
            //            {
            //                var leaveAllocationData = db.tbl_CutiMaintenance.SingleOrDefault(x =>
            //                        x.fld_JenisCuti == leaveCategory.fld_KodCuti && x.fld_LowerLimit <= dateDiff.Months &&
            //                        x.fld_LowerLimit <= dateDiff.Months && x.fld_UpperLimit >= dateDiff.Months &&
            //                        x.fld_NegaraID == ngraID && x.fld_SyarikatID == syrktID && x.fld_Deleted == false)
            //                    .fld_PeruntukkanCuti;

            //                Models.tbl_CutiPeruntukan CutiPeruntukanTahunan = new Models.tbl_CutiPeruntukan();

            //                CutiPeruntukanTahunan.fld_NoPkj = workerData.fld_Nopkj;
            //                CutiPeruntukanTahunan.fld_JumlahCuti = leaveAllocationData;
            //                CutiPeruntukanTahunan.fld_KodCuti = leaveCategory.fld_KodCuti;
            //                CutiPeruntukanTahunan.fld_JumlahCutiDiambil = 0;
            //                CutiPeruntukanTahunan.fld_Tahun = Convert.ToInt16(year);
            //                CutiPeruntukanTahunan.fld_NegaraID = ngraID;
            //                CutiPeruntukanTahunan.fld_SyarikatID = syrktID;
            //                CutiPeruntukanTahunan.fld_WilayahID = wlyhID;
            //                CutiPeruntukanTahunan.fld_LadangID = ldgID;
            //                CutiPeruntukanTahunan.fld_Deleted = false;

            //                dbr.tbl_CutiPeruntukan.Add(CutiPeruntukanTahunan);
            //            }

            //            var kodNegeriLadang = db.tbl_Ladang
            //                .Where(x => x.fld_ID == ldgID)
            //                .Select(s => s.fld_KodNegeri)
            //                .Single();

            //            var cutiUmumCount = db.tbl_CutiUmum
            //                .Count(x => x.fld_Negeri == kodNegeriLadang && x.fld_Tahun == year && x.fld_Deleted == false
            //                            && x.fld_NegaraID == ngraID && x.fld_SyarikatID == syrktID && x.fld_IsSelected == true &&
            //                            x.fld_TarikhCuti >= workerData.fld_Trmlkj);
                        

            //            Models.tbl_CutiPeruntukan CutiPeruntukanUmum = new Models.tbl_CutiPeruntukan();

            //            CutiPeruntukanUmum.fld_NoPkj = workerData.fld_Nopkj;
            //            CutiPeruntukanUmum.fld_JumlahCuti = cutiUmumCount;
            //            CutiPeruntukanUmum.fld_KodCuti = kodCutiUmum;
            //            CutiPeruntukanUmum.fld_JumlahCutiDiambil = 0;
            //            CutiPeruntukanUmum.fld_Tahun = Convert.ToInt16(year);
            //            CutiPeruntukanUmum.fld_NegaraID = ngraID;
            //            CutiPeruntukanUmum.fld_SyarikatID = syrktID;
            //            CutiPeruntukanUmum.fld_WilayahID = wlyhID;
            //            CutiPeruntukanUmum.fld_LadangID = ldgID;
            //            CutiPeruntukanUmum.fld_Deleted = false;
            //            dbr.tbl_CutiPeruntukan.Add(CutiPeruntukanUmum);

            //            var jenisMingguNegeri = db.tbl_MingguNegeri.SingleOrDefault(x =>
            //                x.fld_NegaraID == ngraID && x.fld_SyarikatID == syrktID && x.fld_NegeriID == kodNegeriLadang &&
            //                x.fld_Deleted == false).fld_JenisMinggu;
                        

            //            var dayOfWeek = Enum.Parse(typeof(DayOfWeek), jenisMingguNegeri.ToString());

            //            var cutiUmum = db.tbl_CutiUmum
            //                .Where(x => x.fld_NegaraID == ngraID && x.fld_SyarikatID == syrktID &&
            //                            x.fld_Deleted == false && x.fld_Negeri == kodNegeriLadang &&
            //                            x.fld_Tahun == year &&
            //                            x.fld_TarikhCuti.Value.Month == workerData.fld_Trmlkj.Value.Month)
            //                .ToList();
          
            //            // First We find out last date of month
            //            //DateTime today = DateTime.Today;
            //            DateTime endOfMonth = new DateTime(year, Convert.ToInt32(workerData.fld_Trmlkj.Value.Month),
            //                DateTime.DaysInMonth(year, Convert.ToInt32(workerData.fld_Trmlkj.Value.Month)));

            //            //get only last day of month
            //            int daysInMonth = endOfMonth.Day;

            //            int totalLeaveInAMonth = 0;

            //            for (int i = 0; i < daysInMonth; ++i)
            //            {
            //                DateTime d = new DateTime(year, Convert.ToInt32(workerData.fld_Trmlkj.Value.Month), i + 1);

            //                //Compare date with sunday
            //                if (d.DayOfWeek.ToString() == dayOfWeek.ToString())
            //                {
            //                    totalLeaveInAMonth = totalLeaveInAMonth + 1;
            //                }
            //            }

            //            var workingDaysInMonth = (DateTime.DaysInMonth(year, Convert.ToInt32(workerData.fld_Trmlkj.Value.Month))) - totalLeaveInAMonth -
            //                                     cutiUmum.Count;

            //            Models.tbl_Produktiviti Produktiviti = new Models.tbl_Produktiviti();

            //            Produktiviti.fld_Nopkj = workerData.fld_Nopkj;
            //            Produktiviti.fld_HadirKerja = workingDaysInMonth;
            //            Produktiviti.fld_Year = year;
            //            Produktiviti.fld_Month = workerData.fld_Trmlkj.Value.Month;
            //            Produktiviti.fld_NegaraID = ngraID;
            //            Produktiviti.fld_SyarikatID = syrktID;
            //            Produktiviti.fld_WilayahID = wlyhID;
            //            Produktiviti.fld_LadangID = ldgID;
            //            Produktiviti.fld_Deleted = false;
            //            dbr.tbl_Produktiviti.Add(Produktiviti);

            //            //kwsp & socso
            //            if (workerData.fld_Kdrkyt=="MA")
            //            {
            //                DateDiff umurPekerja = new DateDiff(Convert.ToDateTime(workerData.fld_Trlhr).AddDays(-1), lastDay);

            //                var kodSocso = db.tbl_JenisCaruman
            //                    .Where(x => x.fld_UmurLower <= umurPekerja.Years && x.fld_UmurUpper >= umurPekerja.Years && x.fld_JenisCaruman == "SOCSO" && x.fld_Default == true)
            //                    .Select(s => s.fld_KodCaruman).First();

            //                var kodKwsp = db.tbl_JenisCaruman
            //                    .Where(x => x.fld_UmurLower <= umurPekerja.Years && x.fld_UmurUpper >= umurPekerja.Years && x.fld_JenisCaruman == "KWSP" && x.fld_Default == true)
            //                    .Select(s => s.fld_KodCaruman).First();

            //                workerData.fld_KodSocso = kodSocso;
            //                workerData.fld_KodKWSP = kodKwsp;
            //                workerData.fld_StatusKwspSocso = "1";
            //            }
            //            else
            //            {
            //                workerData.fld_StatusKwspSocso = "2";
            //            }
            //            dbr.Entry(workerData).State = EntityState.Modified;
            //            dbr.SaveChanges();
            //            status = true;
            //        }
            //        catch (Exception ex)
            //        {
            //            geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
            //        }
            //    }
            //}
            //return Json(new { msg = status }, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult Rejectreason(int id)
        {
            //Check_Balik
            var viewresult = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "sbbTolak" && x.fldDeleted == false);
            var result = db.tblPkjmastApps.Where(x => x.fldID==id).FirstOrDefault();
            ViewBag.Nopkj = result.fldNoPkj;
            ViewBag.Nama = result.fldNama1;
            ViewBag.ID = result.fldID;
            //ViewBag.fldSbbTolak = sbbtolak;
            return PartialView(viewresult);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Rejectreason()
        {
            string chksbbtolak = "";
            chksbbtolak = Request.Form["ChkVal"];
            int id =int.Parse(Request.Form["idval"]);
            var pkjreject = db.tblPkjmastApps.Where(x => x.fldID.Equals(id)).FirstOrDefault();
            pkjreject.fldSbbTolak = chksbbtolak;
            pkjreject.fldStatus = 0;
            db.SaveChanges();

            return Json(new { success = true, msg = "Data successfully rejected.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "", data3 = "" });
        }

        public ActionResult TransferWorker(int WilayahIDList = 0, int LadangIDList = 0)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
            List<SelectListItem> LadangIDList2 = new List<SelectListItem>();
            if (WilayahID == 0 && LadangID == 0)
            {
                WilayahIDList2 = new SelectList(db.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                WilayahIDList2.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));
                LadangIDList2 = new SelectList(db.tbl_Ladang.Where(x => x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList2.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));
                ViewBag.WilayahIDList = WilayahIDList2;
                ViewBag.LadangIDList = LadangIDList2;
                var resultreport = db.tblASCApprovalFileDetails.Where(x => x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID ).OrderBy(o => o.fldWilayahID);
                return View("TransferWorker", resultreport);
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                WilayahIDList2 = new SelectList(db.tbl_Wilayah.Where(x => x.fld_ID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                LadangIDList2 = new SelectList(db.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList2.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));
                ViewBag.WilayahIDList = WilayahIDList2;
                ViewBag.LadangIDList = LadangIDList2;
                var resultreport = db.tblASCApprovalFileDetails.Where(x => x.fldWilayahID == WilayahID && x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID).OrderBy(o => o.fldWilayahID);
                return View("TransferWorker", resultreport);
            }
            else
            {
                WilayahIDList2 = new SelectList(db.tbl_Wilayah.Where(x => x.fld_ID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                LadangIDList2 = new SelectList(db.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahID && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                ViewBag.WilayahIDList = WilayahIDList2;
                ViewBag.LadangIDList = LadangIDList2;
                var resultreport = db.tblASCApprovalFileDetails.Where(x => x.fldWilayahID == WilayahID && x.fldLadangID == LadangID && x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID).OrderBy(o => o.fldWilayahID);
                ViewBag.resultcount = resultreport.Count();
                return View("TransferWorker", resultreport);
            }
        }

        public PartialViewResult TransferWorkerDetail(int fileID)
        {
            var result = db.tblPkjmastApps.Where(x => x.fldFileID == fileID && x.fldStatus == 2 && x.fldSbbMsk=="PL");
            //var result = db.tblPkjmastApps.Where(x => x.fldFileID == fileID);
            ViewBag.Datacount = result.Count();
            return PartialView("NewWorkerDetail", result);
        }

        public ActionResult TransferApprove()
        {
            //if(act == 1)
            //{
            //    //approve(1)
            //    var app1 = db.tblPkjmastApps.Where(x => x.fldID == id).FirstOrDefault();
            //    app1.fldStatus = 1;
            //    app1.fldDateTimeApprove = DateTime.Now;
            //    app1.fldActionBy = GetIdentity.ID(User.Identity.Name);
            //    db.SaveChanges();

            //    string nopkj = app1.fldNoPkj;
            //    int ldgID = app1.fldLadangID.Value;
            //    int wlyhID = app1.fldWilayahID.Value;
            //    int syrktID = app1.fldSyarikatID.Value;
            //    int ngraID = app1.fldNegaraID.Value;
            //    string host, catalog, user, pass = "";
            //    Connection.GetConnection(out host, out catalog, out user, out pass, wlyhID, syrktID, ngraID);
            //    MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            //    if (app1.fldSbbMsk.Trim() == "PL")
            //    {
            //        var workerData = dbr.tbl_Pkjmast
            //            .Single(x => x.fld_Nopkj == nopkj && x.fld_LadangID == ldgID &&
            //                         x.fld_WilayahID == wlyhID && x.fld_NegaraID == ngraID);

            //        string host2, catalog2, user2, pass2 = "";
            //        Connection.GetConnection(out host2, out catalog2, out user2, out pass2, app1.fldWilayahAsal, syrktID, ngraID);
            //        MVC_SYSTEM_Models dbr2 = MVC_SYSTEM_Models.ConnectToSqlServer(host2, catalog2, user2, pass2);

            //        //Models.tbl_CutiPeruntukan test = new tbl_CutiPeruntukan();

            //        List<tbl_CutiPeruntukan> copyleaves = dbr2.tbl_CutiPeruntukan
            //            .Where(x => x.fld_NoPkj == workerData.fld_IDpkj && x.fld_NegaraID == ngraID && x.fld_SyarikatID == syrktID && x.fld_WilayahID == app1.fldWilayahAsal && x.fld_LadangID == app1.fldLadangAsal && x.fld_Deleted == false).ToList();




            //        var saveLeave = dbr.tbl_CutiPeruntukan.Where(x => x.fld_NoPkj == nopkj);
            //        if (saveLeave == null)
            //        {
            //            Models.tbl_CutiPeruntukan tblLeaves = new tbl_CutiPeruntukan();
            //            tblLeaves.fld_CutiKategoriID = copyleaves.Select(s => s.fld_CutiKategoriID);
            //        }



            //    }
            //    else
            //    {
            //        var workerData = dbr.tbl_Pkjmast
            //            .Single(x => x.fld_Nopkj == nopkj && x.fld_LadangID == ldgID &&
            //                         x.fld_WilayahID == wlyhID && x.fld_NegaraID == ngraID);
            //        try
            //        {
            //            int year = DateTime.Now.Year;
            //            DateTime lastDay = new DateTime(year, 12, 31);

            //            var app2 = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == nopkj).FirstOrDefault();
            //            app2.fld_Kdaktf = "1";
            //            app2.fld_StatusApproved = 1;
            //            app2.fld_ActionBy = User.Identity.Name;
            //            app2.fld_ActionDate = DateTime.Now;

            //            DateDiff dateDiff = new DateDiff(Convert.ToDateTime(workerData.fld_Trmlkj).AddDays(-1), lastDay);

            //            //calculate annual leave

            //            if (dateDiff.Months < 12)
            //            {
            //                var cutiTahunanPkjBaru = db.tbl_CutiMaintenance
            //                .Where(x => x.fld_JenisCuti == "cutiTahunanPkjBaru" && x.fld_Deleted == false
            //                            && x.fld_NegaraID == ngraID && x.fld_SyarikatID == syrktID
            //                            && x.fld_LowerLimit <= dateDiff.Months && x.fld_UpperLimit >= dateDiff.Months)
            //                .Select(s => s.fld_PeruntukkanCuti)
            //                .Single();

            //                var kodCutiTahunan = db.tbl_CutiKategori
            //                    .Where(x => x.fld_KeteranganCuti == "CUTI TAHUNAN" && x.fld_Deleted == false
            //                            && x.fld_NegaraID == ngraID && x.fld_SyarikatID == syrktID)
            //                    .Select(s => s.fld_KodCuti)
            //                    .Single();

            //                Models.tbl_CutiPeruntukan CutiPeruntukanTahunan = new tbl_CutiPeruntukan();

            //                CutiPeruntukanTahunan.fld_NoPkj = workerData.fld_Nopkj;
            //                CutiPeruntukanTahunan.fld_JumlahCuti = cutiTahunanPkjBaru;
            //                CutiPeruntukanTahunan.fld_CutiKategoriID = kodCutiTahunan;
            //                CutiPeruntukanTahunan.fld_JumlahCutiDiambil = 0;
            //                CutiPeruntukanTahunan.fld_Tahun = Convert.ToInt16(year);
            //                CutiPeruntukanTahunan.fld_NegaraID = ngraID;
            //                CutiPeruntukanTahunan.fld_SyarikatID = syrktID;
            //                CutiPeruntukanTahunan.fld_WilayahID = wlyhID;
            //                CutiPeruntukanTahunan.fld_LadangID = ldgID;
            //                CutiPeruntukanTahunan.fld_Deleted = false;

            //                dbr.tbl_CutiPeruntukan.Add(CutiPeruntukanTahunan);
            //            }

            //            else
            //            {
            //                var cutiTahunanPkjLama = db.tbl_CutiMaintenance
            //            .Where(x => x.fld_JenisCuti == "cutiTahunanPkjLama" && x.fld_Deleted == false
            //                        && x.fld_NegaraID == ngraID && x.fld_SyarikatID == syrktID
            //                        && x.fld_LowerLimit <= dateDiff.Years && x.fld_UpperLimit >= dateDiff.Years)
            //            .Select(s => s.fld_PeruntukkanCuti)
            //            .Single();

            //                var kodCutiTahunan = db.tbl_CutiKategori
            //                    .Where(x => x.fld_KeteranganCuti == "CUTI TAHUNAN" && x.fld_Deleted == false
            //                            && x.fld_NegaraID == ngraID && x.fld_SyarikatID == syrktID)
            //                    .Select(s => s.fld_KodCuti)
            //                    .Single();

            //                Models.tbl_CutiPeruntukan CutiPeruntukanTahunan = new tbl_CutiPeruntukan();

            //                CutiPeruntukanTahunan.fld_NoPkj = workerData.fld_Nopkj;
            //                CutiPeruntukanTahunan.fld_JumlahCuti = cutiTahunanPkjLama;
            //                CutiPeruntukanTahunan.fld_CutiKategoriID = kodCutiTahunan;
            //                CutiPeruntukanTahunan.fld_JumlahCutiDiambil = 0;
            //                CutiPeruntukanTahunan.fld_Tahun = Convert.ToInt16(year);
            //                CutiPeruntukanTahunan.fld_NegaraID = ngraID;
            //                CutiPeruntukanTahunan.fld_SyarikatID = syrktID;
            //                CutiPeruntukanTahunan.fld_WilayahID = wlyhID;
            //                CutiPeruntukanTahunan.fld_LadangID = ldgID;
            //                CutiPeruntukanTahunan.fld_Deleted = false;

            //                dbr.tbl_CutiPeruntukan.Add(CutiPeruntukanTahunan);
            //            }

            //            //calculate sick leave
            //            var cutiSakitPkjBaru = db.tbl_CutiMaintenance
            //                .Where(x => x.fld_JenisCuti == "cutiSakitPkj" && x.fld_Deleted == false
            //                            && x.fld_NegaraID == ngraID && x.fld_SyarikatID == syrktID
            //                            && x.fld_LowerLimit <= dateDiff.Years && x.fld_UpperLimit >= dateDiff.Years)
            //                .Select(s => s.fld_PeruntukkanCuti)
            //                .Single();

            //            var kodCutiSakit = db.tbl_CutiKategori
            //                .Where(x => x.fld_KeteranganCuti == "CUTI SAKIT" && x.fld_Deleted == false
            //                            && x.fld_NegaraID == ngraID && x.fld_SyarikatID == syrktID)
            //                .Select(s => s.fld_KodCuti)
            //                .Single();

            //            Models.tbl_CutiPeruntukan CutiPeruntukanSakit = new tbl_CutiPeruntukan();

            //            CutiPeruntukanSakit.fld_NoPkj = workerData.fld_Nopkj;
            //            CutiPeruntukanSakit.fld_JumlahCuti = cutiSakitPkjBaru;
            //            CutiPeruntukanSakit.fld_CutiKategoriID = kodCutiSakit;
            //            CutiPeruntukanSakit.fld_JumlahCutiDiambil = 0;
            //            CutiPeruntukanSakit.fld_Tahun = Convert.ToInt16(year);
            //            CutiPeruntukanSakit.fld_NegaraID = ngraID;
            //            CutiPeruntukanSakit.fld_SyarikatID = syrktID;
            //            CutiPeruntukanSakit.fld_WilayahID = wlyhID;
            //            CutiPeruntukanSakit.fld_LadangID = ldgID;
            //            CutiPeruntukanSakit.fld_Deleted = false;

            //            dbr.tbl_CutiPeruntukan.Add(CutiPeruntukanSakit);

            //            //calculate public holiday
            //            var kodCutiUmum = db.tbl_CutiKategori
            //                .Where(x => x.fld_KeteranganCuti == "CUTI AM" && x.fld_Deleted == false
            //                            && x.fld_NegaraID == ngraID && x.fld_SyarikatID == syrktID)
            //                .Select(s => s.fld_KodCuti)
            //                .Single();

            //            var kodNegeriLadang = dbr.tbl_Ladang
            //                .Where(x => x.fld_ID == workerData.fld_LadangID)
            //                .Select(s => s.fld_KodNegeri)
            //                .Single();

            //            var CutiUmum = db.tbl_CutiUmum
            //                .Count(x => x.fld_Negeri == kodNegeriLadang && x.fld_Tahun == year && x.fld_Deleted == false
            //                            && x.fld_NegaraID == ngraID && x.fld_SyarikatID == syrktID &&
            //                            x.fld_TarikhCuti > workerData.fld_Trmlkj);

            //            Models.tbl_CutiPeruntukan CutiPeruntukanUmum = new tbl_CutiPeruntukan();

            //            CutiPeruntukanUmum.fld_NoPkj = workerData.fld_Nopkj;
            //            CutiPeruntukanUmum.fld_JumlahCuti = CutiUmum;
            //            CutiPeruntukanUmum.fld_CutiKategoriID = kodCutiUmum;
            //            CutiPeruntukanUmum.fld_JumlahCutiDiambil = 0;
            //            CutiPeruntukanUmum.fld_Tahun = Convert.ToInt16(year);
            //            CutiPeruntukanUmum.fld_NegaraID = ngraID;
            //            CutiPeruntukanUmum.fld_SyarikatID = syrktID;
            //            CutiPeruntukanUmum.fld_WilayahID = wlyhID;
            //            CutiPeruntukanUmum.fld_LadangID = ldgID;
            //            CutiPeruntukanUmum.fld_Deleted = false;
            //            dbr.tbl_CutiPeruntukan.Add(CutiPeruntukanUmum);

            //            //kwsp & socso
            //            DateDiff umurPekerja = new DateDiff(Convert.ToDateTime(workerData.fld_Trlhr).AddDays(-1), lastDay);

            //            var kodSocso = db.tbl_JenisCaruman
            //                .Where(x => x.fld_UmurLower <= umurPekerja.Years && x.fld_UmurUpper >= umurPekerja.Years && x.fld_JenisCaruman == "SOCSO" && x.fld_Default == true)
            //                .Select(s => s.fld_KodCaruman).First();

            //            var kodKwsp = db.tbl_JenisCaruman
            //                .Where(x => x.fld_UmurLower <= umurPekerja.Years && x.fld_UmurUpper >= umurPekerja.Years && x.fld_JenisCaruman == "KWSP" && x.fld_Default == true)
            //                .Select(s => s.fld_KodCaruman).First();

            //            workerData.fld_KodSocso = kodSocso;
            //            workerData.fld_KodKWSP = kodKwsp;
            //            dbr.Entry(workerData).State = EntityState.Modified;
            //            dbr.SaveChanges();
            //            status = true;
            //        }
            //        catch (Exception ex)
            //        {
            //            geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
            //        }
            //    }


            //}
            return View();
        }

        public ActionResult testmodal()
        {
            return View();
        }
    }
}