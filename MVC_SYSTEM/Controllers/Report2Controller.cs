using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.CustomModels;
using MVC_SYSTEM.log;
using MVC_SYSTEM.Attributes;
using System.Threading.Tasks;
using MVC_SYSTEM.CustomModels2;
using System.Globalization;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public class Report2Controller : Controller
    {
        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
        GetIdentity GetIdentity = new GetIdentity();
        GetNSWL GetNSWL = new GetNSWL();
        Connection Connection = new Connection();
        ChangeTimeZone timezone = new ChangeTimeZone();
        GetConfig GetConfig = new GetConfig();
        errorlog geterror = new errorlog();
        ReportLogic ReportLogic = new ReportLogic();
        // GET: Report2

        public ActionResult LprnBlnnByrnAktvt()
        {
            ViewBag.Report = "class = active";
            ViewBag.TitleReport = "LAPORAN TAHUNAN MENGIKUT AKTIVITI KERJA";
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

            List<SelectListItem> SelectionList = new List<SelectListItem>();
            SelectionList = new SelectList(
                dbr.tbl_Pkjmast
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1")
                    .OrderBy(o => o.fld_Nopkj)
                    .Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }),
                "Value", "Text").ToList();
            SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            List<SelectListItem> GroupList = new List<SelectListItem>();
            GroupList = new SelectList(dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false && x.bilangan_ahli >= 0).OrderBy(o => o.fld_KodKumpulan).Select(s => new SelectListItem { Value = s.fld_KodKumpulan, Text = s.fld_KodKumpulan + "-" + s.fld_Keterangan }), "Value", "Text").ToList();
            GroupList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));



            List<SelectListItem> JnisAktvt = new List<SelectListItem>();
            var LejarList = db.tbl_Lejar.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_KodLejar).ToList();

            string LejarSelect = LejarList.Select(s => s.fld_KodLejar).Take(1).FirstOrDefault();

            var tbl_JenisAktiviti = db.tbl_JenisAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false && x.fld_Lejar == LejarSelect).ToList();
            JnisAktvt = new SelectList(tbl_JenisAktiviti.OrderBy(o => o.fld_KodJnsAktvt).Select(s => new SelectListItem { Value = s.fld_KodJnsAktvt, Text = s.fld_Desc }), "Value", "Text").ToList();
            var GetJenisAktvtyDesc = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "activityLevel" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => new { s.fldOptConfValue, s.fldOptConfDesc }).ToList();
            JnisAktvt = new SelectList(GetJenisAktvtyDesc.Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            JnisAktvt.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            var blokList = new SelectList(
    dbr.vw_KerjaInfoDetails2
        .Where(x => x.fld_NegaraID == NegaraID &&
                    x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                    x.fld_LadangID == LadangID)
        .OrderBy(o => o.fld_KodAktvt)
        .Select(s => new SelectListItem { Value = s.fld_KodAktvt, Text = s.fld_KodAktvt }),
    "Value", "Text").ToList();

            var allPeringkatList = new List<SelectListItem>();
            allPeringkatList.AddRange(blokList);

            allPeringkatList.Insert(0, (new SelectListItem { Text = "Semua", Value = "" }));
            ViewBag.AllPeringkatList = allPeringkatList;

            ViewBag.GroupList = GroupList;
            ViewBag.JnisAktvt = JnisAktvt;
            ViewBag.SelectionList = SelectionList;
            ViewBag.MonthList = MonthList;
            ViewBag.MonthList2 = MonthList;
            ViewBag.YearList = yearlist;
            return View();
        }

        public ViewResult _LprnBlnnByrnAktvt(int? MonthList, int? MonthList2, int? YearList, string GroupList, string SelectionList)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            long ID = 1;
            var LprnBlnnByrnAktvt = new List<CustMod_LprnBlnnByrnAktvt>();
            var GetPktUtamas = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_IOcode).ToList();
            var GetPktSubs = dbr.tbl_SubPkt.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
            var GetWorkers = dbr.vw_KerjaInfoDetails2.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month >= MonthList && x.fld_Tarikh.Value.Month <= MonthList2 && x.fld_Tarikh.Value.Year == YearList).OrderBy(o => new { o.fld_Tarikh, o.fld_Nopkj }).ToList();

            Parallel.ForEach(GetPktUtamas, GetPktUtama =>
            {
                var GetPktSubByUtmas = GetPktSubs.Where(x => x.fld_KodPktUtama == GetPktUtama.fld_PktUtama).ToList();
                if (GetPktSubByUtmas.Count > 0)
                {
                    Parallel.ForEach(GetPktSubByUtmas, GetPktSubByUtma =>
                    {
                        var GetWorkerByCCs = GetWorkers.Where(x => x.fld_KodPkt == GetPktSubByUtma.fld_Pkt).GroupBy(g => new { g.fld_Nopkj, g.fld_Nama, g.fld_KodPkt, g.fld_NegaraID, g.fld_SyarikatID, g.fld_WilayahID, g.fld_LadangID, g.fld_Tarikh.Value.Month, g.fld_Tarikh.Value.Year })
                        .Select(s => new { s.Key.fld_Nopkj, s.Key.fld_Nama, s.Key.fld_KodPkt, s.Key.fld_NegaraID, s.Key.fld_SyarikatID, s.Key.fld_WilayahID, s.Key.fld_LadangID, fld_Month = s.Key.Month, fld_Year = s.Key.Year, fld_OverallAmount = s.Sum(t => t.fld_OverallAmount) })
                        .ToList();

                        Parallel.ForEach(GetWorkerByCCs, GetWorkerByCC =>
                        {
                            LprnBlnnByrnAktvt.Add(new CustMod_LprnBlnnByrnAktvt() { fld_ID = ID, fld_WorkerNo = GetWorkerByCC.fld_Nopkj, fld_WorkerName = GetWorkerByCC.fld_Nama.ToUpper(), fld_LevelType = 2, fld_CC = GetPktUtama.fld_IOcode, fld_LevelCode = GetWorkerByCC.fld_KodPkt, fld_LevelName = GetPktSubByUtma.fld_NamaPkt, fld_Amount = GetWorkerByCC.fld_OverallAmount.Value, fld_Month = GetWorkerByCC.fld_Month, fld_Year = GetWorkerByCC.fld_Year, fld_NegaraID = GetWorkerByCC.fld_NegaraID.Value, fld_SyarikatID = GetWorkerByCC.fld_SyarikatID.Value, fld_WilayahID = GetWorkerByCC.fld_WilayahID.Value, fld_LadangID = GetWorkerByCC.fld_LadangID.Value });
                            ID++;
                        });
                    });
                }
                else
                {
                    var GetWorkerByCCs = GetWorkers.Where(x => x.fld_KodPkt == GetPktUtama.fld_PktUtama).GroupBy(g => new { g.fld_Nopkj, g.fld_Nama, g.fld_KodPkt, g.fld_NegaraID, g.fld_SyarikatID, g.fld_WilayahID, g.fld_LadangID, g.fld_Tarikh.Value.Month, g.fld_Tarikh.Value.Year })
                        .Select(s => new { s.Key.fld_Nopkj, s.Key.fld_Nama, s.Key.fld_KodPkt, s.Key.fld_NegaraID, s.Key.fld_SyarikatID, s.Key.fld_WilayahID, s.Key.fld_LadangID, fld_Month = s.Key.Month, fld_Year = s.Key.Year, fld_OverallAmount = s.Sum(t => t.fld_OverallAmount) })
                        .ToList();
                    Parallel.ForEach(GetWorkerByCCs, GetWorkerByCC =>
                    {
                        LprnBlnnByrnAktvt.Add(new CustMod_LprnBlnnByrnAktvt() { fld_ID = ID, fld_WorkerNo = GetWorkerByCC.fld_Nopkj, fld_WorkerName = GetWorkerByCC.fld_Nama.ToUpper(), fld_LevelType = 1, fld_CC = GetPktUtama.fld_IOcode, fld_LevelCode = GetWorkerByCC.fld_KodPkt, fld_LevelName = GetPktUtama.fld_NamaPktUtama, fld_Amount = GetWorkerByCC.fld_OverallAmount.Value, fld_Month = GetWorkerByCC.fld_Month, fld_Year = GetWorkerByCC.fld_Year, fld_NegaraID = GetWorkerByCC.fld_NegaraID.Value, fld_SyarikatID = GetWorkerByCC.fld_SyarikatID.Value, fld_WilayahID = GetWorkerByCC.fld_WilayahID.Value, fld_LadangID = GetWorkerByCC.fld_LadangID.Value });
                        ID++;
                    });
                }

            });

            var Syarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).FirstOrDefault();

            ViewBag.NamaSyarikat = Syarikat.fld_NamaSyarikat;
            ViewBag.NoSyarikat = Syarikat.fld_NoSyarikat;
            ViewBag.TitleReport = "Laporan Tahunan Perbelanjaan Aktivi Kerja";
            LprnBlnnByrnAktvt.RemoveAll(x => x == null);
            return View(LprnBlnnByrnAktvt);
        }

        public ActionResult LprnKosPrblnjaanAktvtKrja()
        {
            ViewBag.Report = "class = active";
            ViewBag.TitleReport = "LAPORAN AGIHAN KOS PERBELANJAAN AKTIVIT KERJA";
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
            List<SelectListItem> PilihanPkt = new List<SelectListItem>();
            var SelectPkt = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
            PilihanPkt = new SelectList(SelectPkt.Select(s => new SelectListItem { Value = s.fld_PktUtama, Text = s.fld_PktUtama + " - " + s.fld_NamaPktUtama + " - (" + s.fld_IOcode + ")" }), "Value", "Text").ToList();
            PilihanPkt.Insert(0, (new SelectListItem { Text = "ALL", Value = "0" }));
            ViewBag.PilihanPkt = PilihanPkt;
            ViewBag.MonthList = MonthList;
            ViewBag.MonthList2 = MonthList;
            ViewBag.YearList = yearlist;
            return View();
        }

        public ViewResult _LprnKosPrblnjaanAktvtKrja(int? MonthList, int? MonthList2, int? YearList, string PilihanPkt)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            long ID = 1;
            var RptActvByLvlWorker = new List<CustMod_RptActvByLvlWorker>();
            var GetPktUtamas = new List<Models.tbl_PktUtama>();
            var GetPktSubs = new List<Models.tbl_SubPkt>();
            if (PilihanPkt == "0")
            {
                GetPktUtamas = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_IOcode).ToList();
            }
            else
            {
                GetPktUtamas = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_PktUtama == PilihanPkt).OrderBy(o => o.fld_IOcode).ToList();
            }
            GetPktSubs = dbr.tbl_SubPkt.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
            var GetWorkers = dbr.vw_KerjaInfoDetails2.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month >= MonthList && x.fld_Tarikh.Value.Month <= MonthList2 && x.fld_Tarikh.Value.Year == YearList).OrderBy(o => new { o.fld_Tarikh, o.fld_Nopkj }).ToList();
            var GetWorkerot = dbr.vw_Kerja_OT.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month >= MonthList && x.fld_Tarikh.Value.Month <= MonthList2 && x.fld_Tarikh.Value.Year == YearList).OrderBy(o => new { o.fld_Tarikh, o.fld_Nopkj }).ToList();
            var GetWorkerbonus = dbr.vw_Kerja_Bonus.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month >= MonthList && x.fld_Tarikh.Value.Month <= MonthList2 && x.fld_Tarikh.Value.Year == YearList).OrderBy(o => new { o.fld_Tarikh, o.fld_Nopkj }).ToList();
            var GetWorkercuti = dbr.vw_Kerja_Hdr_Cuti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month >= MonthList && x.fld_Tarikh.Value.Month <= MonthList2 && x.fld_Tarikh.Value.Year == YearList).OrderBy(o => new { o.fld_Tarikh, o.fld_Nopkj }).ToList();
            var GetWorkerIncntveDeduction = dbr.vw_InsentifPekerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month >= MonthList && x.fld_Month <= MonthList2 && x.fld_Year == YearList && x.fld_Deleted == false).OrderBy(o => new { o.fld_Month, o.fld_Nopkj }).ToList();
            var GetIncetiveDeductionCode = db.tbl_JenisInsentif.ToList();
            var GetWrkActvt = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).ToList();
            var GetKong = GetWrkActvt.Where(x => x.fld_DisabledFlag == 3).Select(s => s.fld_KodAktvt).ToList();
            var GetPieceRate = GetWrkActvt.Where(x => x.fld_DisabledFlag != 3).Select(s => s.fld_KodAktvt).ToList();
            var GetIncetiveCode = GetIncetiveDeductionCode.Where(x => x.fld_JenisInsentif == "P").Select(s => s.fld_KodInsentif).ToList();
            var GetDeductionCode = GetIncetiveDeductionCode.Where(x => x.fld_JenisInsentif == "T").Select(s => s.fld_KodInsentif).ToList();
            var GetWorkerIncntve = GetWorkerIncntveDeduction.Where(x => GetIncetiveCode.Contains(x.fld_KodInsentif)).ToList();
            var GetWorkerDeduction = GetWorkerIncntveDeduction.Where(x => GetDeductionCode.Contains(x.fld_KodInsentif)).ToList();
            Parallel.ForEach(GetPktUtamas, GetPktUtama =>
            {
                var GetPktSubByUtmas = GetPktSubs.Where(x => x.fld_KodPktUtama == GetPktUtama.fld_PktUtama).ToList();
                if (GetPktSubByUtmas.Count > 0)
                {
                    Parallel.ForEach(GetPktSubByUtmas, GetPktSubByUtma =>
                    {
                        var GetWorkerByCCs = GetWorkers.Where(x => x.fld_KodPkt == GetPktSubByUtma.fld_Pkt).GroupBy(g => new { g.fld_Nopkj, g.fld_Nama, g.fld_KodPkt, g.fld_NegaraID, g.fld_SyarikatID, g.fld_WilayahID, g.fld_LadangID, g.fld_Tarikh.Value.Month, g.fld_Tarikh.Value.Year })
                        .Select(s => new { s.Key.fld_Nopkj, s.Key.fld_Nama, s.Key.fld_KodPkt, s.Key.fld_NegaraID, s.Key.fld_SyarikatID, s.Key.fld_WilayahID, s.Key.fld_LadangID, fld_Month = s.Key.Month, fld_Year = s.Key.Year, fld_OverallAmount = s.Sum(t => t.fld_OverallAmount) })
                        .ToList();

                        Parallel.ForEach(GetWorkerByCCs, GetWorkerByCC =>
                        {
                            var GetTotalKong = GetWorkers.Where(x => GetKong.Contains(x.fld_KodAktvt) && x.fld_Nopkj == GetWorkerByCC.fld_Nopkj).Sum(s => s.fld_OverallAmount);
                            var GetTotalPieceRate = GetWorkers.Where(x => GetPieceRate.Contains(x.fld_KodAktvt) && x.fld_Nopkj == GetWorkerByCC.fld_Nopkj).Sum(s => s.fld_OverallAmount);
                            var GetTotalOTTime = GetWorkerot.Where(x => x.fld_Nopkj == GetWorkerByCC.fld_Nopkj).Sum(s => s.fld_JamOT);
                            var GetTotalOTPay = GetWorkerot.Where(x => x.fld_Nopkj == GetWorkerByCC.fld_Nopkj).Sum(s => s.fld_Jumlah);
                            var GetTotalIncentive = GetWorkerIncntve.Where(x => x.fld_Nopkj == GetWorkerByCC.fld_Nopkj).Sum(s => s.fld_NilaiInsentif);
                            var GetTotalBonus = GetWorkerbonus.Where(x => x.fld_Nopkj == GetWorkerByCC.fld_Nopkj).Sum(s => s.fld_Jumlah_B);
                            var GetTotalCuti = GetWorkercuti.Where(x => x.fld_Nopkj == GetWorkerByCC.fld_Nopkj).Sum(s => s.fld_Jumlah);
                            var GetTotalDeduction = GetWorkerDeduction.Where(x => x.fld_Nopkj == GetWorkerByCC.fld_Nopkj).Sum(s => s.fld_NilaiInsentif);
                            var AddtionalPay = GetTotalBonus + GetTotalCuti;
                            var GetTotalIncome = GetTotalKong + GetTotalPieceRate + GetTotalOTPay + GetTotalIncentive + GetTotalBonus + GetTotalCuti;
                            var GetTotal = GetTotalIncome - GetTotalDeduction;
                            RptActvByLvlWorker.Add(new CustMod_RptActvByLvlWorker() { id = ID, workerno = GetWorkerByCC.fld_Nopkj, workername = GetWorkerByCC.fld_Nama, kong = GetTotalKong.Value, ottime = GetTotalOTTime.Value, otpayment = GetTotalOTPay.Value, piecereatepay = GetTotalPieceRate.Value, dailyincentivepay = GetTotalIncentive.Value, otherincome = AddtionalPay.Value, lvlcode = GetPktSubByUtma.fld_KodPktUtama, lvldesc = GetPktSubByUtma.fld_NamaPkt, leveltype = 2, costcenter = GetPktUtama.fld_IOcode, totalincome = GetTotalIncome.Value, overallincome = GetTotal.Value, totaldeduction = GetTotalDeduction.Value });
                            ID++;
                        });
                    });
                }
                else
                {
                    var GetWorkerByCCs = GetWorkers.Where(x => x.fld_KodPkt == GetPktUtama.fld_PktUtama).GroupBy(g => new { g.fld_Nopkj, g.fld_Nama, g.fld_KodPkt, g.fld_NegaraID, g.fld_SyarikatID, g.fld_WilayahID, g.fld_LadangID, g.fld_Tarikh.Value.Month, g.fld_Tarikh.Value.Year })
                        .Select(s => new { s.Key.fld_Nopkj, s.Key.fld_Nama, s.Key.fld_KodPkt, s.Key.fld_NegaraID, s.Key.fld_SyarikatID, s.Key.fld_WilayahID, s.Key.fld_LadangID, fld_Month = s.Key.Month, fld_Year = s.Key.Year, fld_OverallAmount = s.Sum(t => t.fld_OverallAmount) })
                        .ToList();
                    Parallel.ForEach(GetWorkerByCCs, GetWorkerByCC =>
                    {
                        var GetTotalKong = GetWorkers.Where(x => GetKong.Contains(x.fld_KodAktvt) && x.fld_Nopkj == GetWorkerByCC.fld_Nopkj).Sum(s => s.fld_OverallAmount);
                        var GetTotalPieceRate = GetWorkers.Where(x => GetPieceRate.Contains(x.fld_KodAktvt) && x.fld_Nopkj == GetWorkerByCC.fld_Nopkj).Sum(s => s.fld_OverallAmount);
                        var GetTotalOTTime = GetWorkerot.Where(x => x.fld_Nopkj == GetWorkerByCC.fld_Nopkj).Sum(s => s.fld_JamOT);
                        var GetTotalOTPay = GetWorkerot.Where(x => x.fld_Nopkj == GetWorkerByCC.fld_Nopkj).Sum(s => s.fld_Jumlah);
                        var GetTotalIncentive = GetWorkerIncntve.Where(x => x.fld_Nopkj == GetWorkerByCC.fld_Nopkj).Sum(s => s.fld_NilaiInsentif);
                        var GetTotalBonus = GetWorkerbonus.Where(x => x.fld_Nopkj == GetWorkerByCC.fld_Nopkj).Sum(s => s.fld_Jumlah_B);
                        var GetTotalCuti = GetWorkercuti.Where(x => x.fld_Nopkj == GetWorkerByCC.fld_Nopkj).Sum(s => s.fld_Jumlah);
                        var GetTotalDeduction = GetWorkerDeduction.Where(x => x.fld_Nopkj == GetWorkerByCC.fld_Nopkj).Sum(s => s.fld_NilaiInsentif);
                        var AddtionalPay = GetTotalBonus + GetTotalCuti;
                        var GetTotalIncome = GetTotalKong + GetTotalPieceRate + GetTotalOTPay + GetTotalIncentive + GetTotalBonus + GetTotalCuti;
                        var GetTotal = GetTotalIncome - GetTotalDeduction;
                        RptActvByLvlWorker.Add(new CustMod_RptActvByLvlWorker() { id = ID, workerno = GetWorkerByCC.fld_Nopkj, workername = GetWorkerByCC.fld_Nama, kong = GetTotalKong.Value, ottime = GetTotalOTTime.Value, otpayment = GetTotalOTPay.Value, piecereatepay = GetTotalPieceRate.Value, dailyincentivepay = GetTotalIncentive.Value, otherincome = AddtionalPay.Value, lvlcode = GetPktUtama.fld_PktUtama, lvldesc = GetPktUtama.fld_NamaPktUtama, leveltype = 1, costcenter = GetPktUtama.fld_IOcode, totalincome = GetTotalIncome.Value, overallincome = GetTotal.Value, totaldeduction = GetTotalDeduction.Value });
                        ID++;
                    });
                }
            });
            var Syarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).FirstOrDefault();

            ViewBag.NamaSyarikat = Syarikat.fld_NamaSyarikat;
            ViewBag.NoSyarikat = Syarikat.fld_NoSyarikat;
            ViewBag.TitleReport = "LAPORAN AGIHAN KOS PERBELANJAAN AKTIVIT KERJA";
            RptActvByLvlWorker.RemoveAll(x => x == null);
            return View(RptActvByLvlWorker);
        }
        public ActionResult LprnThnPrblnjaanAktvtKrja()
        {
            ViewBag.Report = "class = active";
            ViewBag.TitleReport = "LAPORAN TAHUNAN PERBELANJAAN AKTIVITI KERJA";
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
            List<SelectListItem> PilihanAktvt = new List<SelectListItem>();
            var getactvt = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).ToList();
            var GetActCodeLeave = db.tbl_CutiKategori.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).ToList();
            var GetIncetive = db.tbl_JenisInsentif.Where(x => x.fld_JenisInsentif == "P").ToList();
            int ID = 1;
            var CustMod_ActvtList = new List<CustMod_ActvtList>();
            foreach (var actvt in getactvt)
            {
                CustMod_ActvtList.Add(new CustMod_ActvtList { id = ID, actvtcode = actvt.fld_KodAktvt, actvtdesc = actvt.fld_Desc.ToUpper() });
                ID++;
            }
            foreach (var actvt in GetActCodeLeave)
            {
                CustMod_ActvtList.Add(new CustMod_ActvtList { id = ID, actvtcode = actvt.fld_KodAktvt, actvtdesc = actvt.fld_KeteranganCuti.ToUpper() });
                ID++;
            }
            foreach (var actvt in GetIncetive)
            {
                CustMod_ActvtList.Add(new CustMod_ActvtList { id = ID, actvtcode = actvt.fld_KodAktvt, actvtdesc = actvt.fld_Keterangan.ToUpper() });
                ID++;
            }
            PilihanAktvt = new SelectList(CustMod_ActvtList.Select(s => new SelectListItem { Value = s.actvtcode, Text = s.actvtcode + " - " + s.actvtdesc }).Distinct().OrderBy(o => o.Value), "Value", "Text").ToList();
            PilihanAktvt.Insert(0, (new SelectListItem { Text = "ALL", Value = "0" }));
            ViewBag.PilihanAktvt = PilihanAktvt;
            ViewBag.MonthList = MonthList;
            ViewBag.MonthList2 = MonthList;
            ViewBag.YearList = yearlist;
            return View();
        }

        public ViewResult _LprnThnPrblnjaanAktvtKrja(int? MonthList, int? MonthList2, int? YearList, string PilihanAktvt)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            long ID = 1;
            var RptMnthSpendByActvt = new List<CustMod_RptMnthSpendByActvt>();
            var GetSctran = new List<Models.tbl_Sctran>();
            var getactvt = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).ToList();
            var GetActCodeLeave = db.tbl_CutiKategori.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).ToList();
            var GetIncetive = db.tbl_JenisInsentif.Where(x => x.fld_JenisInsentif == "P").ToList();
            int ID2 = 1;
            var CustMod_ActvtList = new List<CustMod_ActvtList>();
            foreach (var actvt in getactvt)
            {
                CustMod_ActvtList.Add(new CustMod_ActvtList { id = ID2, actvtcode = actvt.fld_KodAktvt, actvtdesc = actvt.fld_Desc.ToUpper() });
                ID2++;
            }
            foreach (var actvt in GetActCodeLeave)
            {
                CustMod_ActvtList.Add(new CustMod_ActvtList { id = ID2, actvtcode = actvt.fld_KodAktvt, actvtdesc = actvt.fld_KeteranganCuti.ToUpper() });
                ID2++;
            }
            foreach (var actvt in GetIncetive)
            {
                CustMod_ActvtList.Add(new CustMod_ActvtList { id = ID2, actvtcode = actvt.fld_KodAktvt, actvtdesc = actvt.fld_Keterangan.ToUpper() });
                ID2++;
            }
            var getactvtworkcode = getactvt.Select(s => s.fld_KodAktvt).ToList();
            GetSctran = dbr.tbl_Sctran.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month >= MonthList && x.fld_Month <= MonthList2 && x.fld_Year == YearList && getactvtworkcode.Contains(x.fld_KodAktvt)).OrderBy(o => o.fld_KodAktvt).ToList();

            var GetPktUtamas = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_IOcode).ToList();
            var GetPktSubs = dbr.tbl_SubPkt.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
            foreach (var Sctran in GetSctran)
            {
                var CC = "";
                if (Sctran.fld_JnsPkt == 1)
                {
                    CC = GetPktUtamas.Where(x => x.fld_PktUtama == Sctran.fld_KodPkt).Select(s => s.fld_IOcode).FirstOrDefault();
                }
                else
                {
                    var getpktutm = GetPktSubs.Where(x => x.fld_Pkt == Sctran.fld_KodPkt).FirstOrDefault();
                    CC = GetPktUtamas.Where(x => x.fld_PktUtama == getpktutm.fld_KodPktUtama).Select(s => s.fld_IOcode).FirstOrDefault();
                }
                RptMnthSpendByActvt.Add(new CustMod_RptMnthSpendByActvt { actvtcode = Sctran.fld_KodAktvt, actvtdesc = Sctran.fld_Keterangan, costcenter = CC, month = Sctran.fld_Month.Value, year = Sctran.fld_Year.Value, totalspend = Sctran.fld_Amt.Value, id = ID });
                ID++;
            }

            var GetWorkercuti = dbr.vw_Kerja_Hdr_Cuti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month >= MonthList && x.fld_Tarikh.Value.Month <= MonthList2 && x.fld_Tarikh.Value.Year == YearList).OrderBy(o => new { o.fld_Tarikh, o.fld_Nopkj }).ToList();
            var GetWorkerDetails = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();

            foreach (var Workercuti in GetWorkercuti)
            {
                var CC = GetWorkerDetails.Where(x => x.fld_Nopkj == Workercuti.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                var ActvtCodeDesc = GetActCodeLeave.Where(x => x.fld_KodCuti == Workercuti.fld_Kdhdct).Select(s => new { s.fld_KodAktvt, s.fld_KeteranganCuti }).FirstOrDefault();
                RptMnthSpendByActvt.Add(new CustMod_RptMnthSpendByActvt { actvtcode = ActvtCodeDesc.fld_KodAktvt, actvtdesc = ActvtCodeDesc.fld_KeteranganCuti, costcenter = CC, month = Workercuti.fld_Tarikh.Value.Month, year = Workercuti.fld_Tarikh.Value.Year, totalspend = Workercuti.fld_Jumlah.Value, id = ID });
                ID++;
            }

            var GetIncetiveCode = GetIncetive.Select(s => s.fld_KodInsentif).ToList();
            var GetWorkerIncntve = dbr.tbl_Insentif.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month >= MonthList && x.fld_Month <= MonthList2 && x.fld_Year == YearList && x.fld_Deleted == false && GetIncetiveCode.Contains(x.fld_KodInsentif)).OrderBy(o => new { o.fld_Month, o.fld_Nopkj }).ToList();
            var RptMnthSpendByActvt2 = new List<CustMod_RptMnthSpendByActvt>();
            foreach (var WorkerIncntve in GetWorkerIncntve)
            {
                var CC = GetWorkerDetails.Where(x => x.fld_Nopkj == WorkerIncntve.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                var ActvtCodeDesc = GetIncetive.Where(x => x.fld_KodInsentif == WorkerIncntve.fld_KodInsentif).Select(s => new { s.fld_KodAktvt, s.fld_Keterangan }).FirstOrDefault();
                RptMnthSpendByActvt.Add(new CustMod_RptMnthSpendByActvt { actvtcode = ActvtCodeDesc.fld_KodAktvt, actvtdesc = ActvtCodeDesc.fld_Keterangan, costcenter = CC, month = WorkerIncntve.fld_Month.Value, year = WorkerIncntve.fld_Year.Value, totalspend = WorkerIncntve.fld_NilaiInsentif.Value, id = ID });
                ID++;
            }
            if (PilihanAktvt == "0")
            {
                RptMnthSpendByActvt = RptMnthSpendByActvt.OrderBy(o => o.actvtcode).ToList();
            }
            else
            {
                RptMnthSpendByActvt = RptMnthSpendByActvt.Where(x => x.actvtcode == PilihanAktvt).OrderBy(o => o.actvtcode).ToList();
            }

            var CustMod_RptActvByLvlWorkerNActvitList = new CustMod_RptActvByLvlWorkerNActvitList
            {
                CustMod_ActvtList = CustMod_ActvtList,
                CustMod_RptMnthSpendByActvt = RptMnthSpendByActvt
            };

            var Syarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).FirstOrDefault();

            ViewBag.NamaSyarikat = Syarikat.fld_NamaSyarikat;
            ViewBag.NoSyarikat = Syarikat.fld_NoSyarikat;
            ViewBag.TitleReport = "LAPORAN TAHUNAN PERBELANJAAN AKTIVITI KERJA";
            GetSctran.RemoveAll(x => x == null);
            return View(CustMod_RptActvByLvlWorkerNActvitList);
        }
        public ActionResult LprnKrjaTrprnc()
        {
            ViewBag.Report = "class = active";
            ViewBag.TitleReport = "LAPORAN AGIHAN PERBELANJAAN AKTIVITI KERJA";
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

            List<SelectListItem> PilihanCC = new List<SelectListItem>();
            var getCC = db.tbl_SAPCCPUP.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
            PilihanCC = new SelectList(getCC.Select(s => new SelectListItem { Value = s.fld_CostCenter, Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc.ToUpper() }), "Value", "Text").ToList();
            var CC = getCC.Select(s => s.fld_CostCenter).Distinct().ToList();
            var getWBS = db.tbl_SAPPDPUP.Where(x => CC.Contains(x.fld_CostCenter) && x.fld_Deleted == false).Select(s => s.fld_WBSCode).Distinct().ToList();
            var z = PilihanCC.Count();
            foreach (var WBS in getWBS)
            {
                PilihanCC.Insert(z, (new SelectListItem { Text = WBS, Value = WBS }));
                z++;
            }
            PilihanCC.Insert(0, (new SelectListItem { Text = "ALL", Value = "0" }));
            List<SelectListItem> PilihanAktvt = new List<SelectListItem>();
            //comment by fitri 7.12.2021
            //var SelectAktvt = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).ToList();
            //PilihanAktvt = new SelectList(SelectAktvt.Select(s => new SelectListItem { Value = s.fld_KodAktvt, Text = s.fld_KodAktvt + " - " + s.fld_Desc }).Distinct().OrderBy(o => o.Value), "Value", "Text").ToList();
            //add by fitri 7.12.2021
            var SelectAktvt = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_KodAktvt).GroupBy(g => new { g.fld_KodAktvt, g.fld_Desc }).ToList();
            PilihanAktvt = new SelectList(SelectAktvt.Select(s => new SelectListItem { Value = s.Key.fld_KodAktvt, Text = s.Key.fld_KodAktvt + " - " + s.Key.fld_Desc }), "Value", "Text").ToList();
            PilihanAktvt.Insert(0, (new SelectListItem { Text = "ALL", Value = "0" }));
            List<SelectListItem> PilihanPkt = new List<SelectListItem>();
            var SelectPkt = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
            PilihanPkt = new SelectList(SelectPkt.Select(s => new SelectListItem { Value = s.fld_PktUtama, Text = s.fld_PktUtama + " - " + s.fld_NamaPktUtama }), "Value", "Text").ToList();
            PilihanPkt.Insert(0, (new SelectListItem { Text = "ALL", Value = "0" }));
            List<SelectListItem> PilihanGL = new List<SelectListItem>();
            var SelectGL = db.tbl_MapGL.Join(db.tbl_SAPGLPUP, j => new { GL = j.fld_KodGL }, k => new { GL = k.fld_GLCode }, (j, k) => new { j.fld_ID, j.fld_KodGL, k.fld_GLDesc }).Select(s => new { s.fld_KodGL, s.fld_GLDesc }).Distinct().ToList();
            PilihanGL = new SelectList(SelectGL.Select(s => new SelectListItem { Value = s.fld_KodGL, Text = s.fld_KodGL + " - " + s.fld_GLDesc.ToUpper() }), "Value", "Text").ToList();
            PilihanGL.Insert(0, (new SelectListItem { Text = "ALL", Value = "0" }));
            List<SelectListItem> PilihanGrp = new List<SelectListItem>();
            var SelectGrp = dbr.tbl_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).ToList();
            PilihanGrp = new SelectList(SelectGrp.Select(s => new SelectListItem { Value = s.fld_KodKumpulan, Text = s.fld_KodKumpulan + " - " + s.fld_Keterangan.ToUpper() }), "Value", "Text").ToList();
            PilihanGrp.Insert(0, (new SelectListItem { Text = "ALL", Value = "0" }));
            List<SelectListItem> PilihanWrk = new List<SelectListItem>();
            //edit by fitri 7.12.2021 (orderby - name)
            var SelectWrk = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o => o.fld_Nama).ToList();
            PilihanWrk = new SelectList(SelectWrk.Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + " - " + s.fld_Nama.ToUpper() }), "Value", "Text").ToList();
            PilihanWrk.Insert(0, (new SelectListItem { Text = "ALL", Value = "0" }));
            List<SelectListItem> PilihanSts = new List<SelectListItem>();
            PilihanSts = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            PilihanSts.Insert(0, (new SelectListItem { Text = "ALL", Value = "0" }));
            ViewBag.PilihanCC = PilihanCC;
            ViewBag.PilihanSts = PilihanSts;
            ViewBag.PilihanWrk = PilihanWrk;
            ViewBag.PilihanGrp = PilihanGrp;
            ViewBag.PilihanGL = PilihanGL;
            ViewBag.PilihanPkt = PilihanPkt;
            ViewBag.PilihanAktvt = PilihanAktvt;
            ViewBag.MonthList = MonthList;
            ViewBag.MonthList2 = MonthList;
            ViewBag.YearList = yearlist;
            return View();
        }
        public ViewResult _LprnKrjaTrprnc(string startdate, string enddate, string PilihanSts, string PilihanWrk, string PilihanGrp, string PilihanGL, string PilihanPkt, string PilihanCC, string PilihanAktvt, int Selection1, bool Selection21, bool Selection22, bool Selection23, bool Selection24, bool Selection25, bool Selection26, bool EnbleCC, bool EnblePkt, bool EnbleAktvt, bool EnbleGL, bool EnbleGrp, bool EnbleWrk)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            bool proceed = true;
            var DetailWork = new List<CustMod_DetailWork2>();
            var coloumgroup = new List<coloumgroup>();
            var detailsreport = new detailsreport();
            if (!EnbleCC && !EnblePkt && !EnbleAktvt && !EnbleGL && !EnbleGrp && !EnbleWrk)
            {
                proceed = false;
            }
            if (!Selection21 && !Selection22 && !Selection23 && !Selection24 && !Selection25 && !Selection26)
            {
                proceed = false;
            }
            if (proceed)
            {
                var tbl_Pkjmast = new List<Models.tbl_Pkjmast>();
                if (PilihanSts == "0")
                {
                    tbl_Pkjmast = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Distinct().ToList();
                }
                else if (PilihanSts == "1")
                {
                    tbl_Pkjmast = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1").Distinct().ToList();
                }
                else
                {
                    tbl_Pkjmast = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "2").Distinct().ToList();
                }
                List<string> cc = new List<string>();
                List<string> pkt = new List<string>();
                List<string> actvt = new List<string>();
                List<string> gl = new List<string>();
                List<string> grp = new List<string>();
                List<string> wrk = new List<string>();
                var Pilihan = "";
                int columngrp = 0;
                var getallactvt = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).ToList();
                if (EnbleCC)
                {
                    if (PilihanCC == "0")
                    {
                        var getcc = db.tbl_SAPCCPUP.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).Select(s => s.fld_CostCenter).Distinct().ToList();
                        cc.AddRange(getcc);
                        var getWBS = db.tbl_SAPPDPUP.Where(x => getcc.Contains(x.fld_CostCenter) && x.fld_Deleted == false).Select(s => s.fld_WBSCode).Distinct().ToList();
                        cc.AddRange(getWBS);
                    }
                    else
                    {
                        cc.Add(PilihanCC);
                    }
                    columngrp++;
                    coloumgroup.Add(new coloumgroup { id = columngrp, name = "Kos Pusat (Cost Centre)", type = 1 });
                    Pilihan += "Cost Center, ";
                }
                if (EnblePkt)
                {
                    if (PilihanPkt == "0")
                    {
                        var getpkt = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).Select(s => s.fld_PktUtama).Distinct().ToList();
                        pkt.AddRange(getpkt);
                    }
                    else
                    {
                        pkt.Add(PilihanPkt);
                    }
                    columngrp++;
                    coloumgroup.Add(new coloumgroup { id = columngrp, name = "Peringkat", type = 2 });
                    Pilihan += "Peringkat, ";
                }
                if (EnbleAktvt)
                {
                    if (PilihanAktvt == "0")
                    {
                        var getactvt2 = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_KodAktvt).Distinct().ToList();
                        actvt.AddRange(getactvt2);
                    }
                    else
                    {
                        actvt.Add(PilihanAktvt);
                    }
                    columngrp++;
                    coloumgroup.Add(new coloumgroup { id = columngrp, name = "Aktiviti", type = 3 });
                    Pilihan += "Aktiviti, ";
                }
                if (EnbleGL)
                {
                    if (PilihanGL == "0")
                    {
                        var getgl = db.tbl_MapGL.Join(db.tbl_SAPGLPUP, j => new { GL = j.fld_KodGL }, k => new { GL = k.fld_GLCode }, (j, k) => new { j.fld_ID, j.fld_KodGL, k.fld_GLDesc }).Select(s => s.fld_KodGL).Distinct().ToList();
                        gl.AddRange(getgl);
                    }
                    else
                    {
                        gl.Add(PilihanGL);
                    }
                    columngrp++;
                    coloumgroup.Add(new coloumgroup { id = columngrp, name = "Kod Akaun (GL SAP)", type = 4 });
                    Pilihan += "GL, ";
                }
                if (EnbleGrp)
                {
                    if (PilihanGrp == "0")
                    {
                        var getgrp = dbr.tbl_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).Select(s => s.fld_KodKumpulan).Distinct().ToList();
                        grp.AddRange(getgrp);
                    }
                    else
                    {
                        grp.Add(PilihanGrp);
                    }
                    columngrp++;
                    coloumgroup.Add(new coloumgroup { id = columngrp, name = "Kump", type = 5 });
                    Pilihan += "Kumpulan, ";
                }
                if (EnbleWrk)
                {
                    if (PilihanWrk == "0")
                    {
                        var getwrk = tbl_Pkjmast.Select(s => s.fld_Nopkj).Distinct().ToList();
                        wrk.AddRange(getwrk);
                    }
                    else
                    {
                        wrk.Add(PilihanWrk);
                    }
                    columngrp++;
                    coloumgroup.Add(new coloumgroup { id = columngrp, name = "Pekerja", type = 6 });
                    Pilihan += "Pekerja, ";
                }
                else
                {
                    var getwrk = tbl_Pkjmast.Select(s => s.fld_Nopkj).Distinct().ToList();
                    wrk.AddRange(getwrk);
                }
                //bool Selection21 = KONG , bool Selection22 = OT, bool Selection23 = OTHERS, bool Selection24 = PIECE RATE, bool Selection25 = INCENTIVE, bool Selection26 = DEDUCTION
                //amounttype KONG = 1, OT = 2, OTHERS = 3, PIECE RATE = 4, INCENTIVE = 5, DEDUCTION = 6
                //get kerja

                var proceedfordatakerja = false;
                var listactvtk = new List<string>();
                var listactvtp = new List<string>();
                if (Selection21 || Selection22 || Selection24 || Selection25)
                {
                    if (Selection21)
                    {
                        var getkong = getallactvt.Where(x => x.fld_DisabledFlag == 3).Select(s => s.fld_KodAktvt).Distinct().ToList();
                        listactvtk.AddRange(getkong);
                    }
                    if (Selection24)
                    {
                        var getpiece = getallactvt.Where(x => x.fld_DisabledFlag != 3).Select(s => s.fld_KodAktvt).Distinct().ToList();
                        listactvtp.AddRange(getpiece);
                    }
                    proceedfordatakerja = true;
                }
                int id = 1;
                var GetPktUtamas = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_IOcode).ToList();
                var GetPktSubs = dbr.tbl_SubPkt.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
                var startdt = new DateTime();
                var enddt = new DateTime();
                try
                {
                    startdt = DateTime.ParseExact(startdate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    enddt = DateTime.ParseExact(enddate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    if (Selection1 == 0)
                    {
                        if (proceedfordatakerja)
                        {
                            var getworkdetails = dbr.vw_KerjaInfoDetails2.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value >= startdt.Date && x.fld_Tarikh.Value <= enddt.Date && wrk.Contains(x.fld_Nopkj)).ToList();
                            if ((listactvtk.Count() > 0 || listactvtp.Count() > 0) && !EnbleAktvt)
                            {
                                if (listactvtk.Count() > 0)
                                {
                                    var getkrjak = getworkdetails.Where(x => listactvtk.Contains(x.fld_KodAktvt)).ToList();
                                    foreach (var krjak in getkrjak)
                                    {
                                        var CC = "";
                                        var leveldesc = "";
                                        var mainlevelcode = "";
                                        if (krjak.fld_JnsPkt == 1)
                                        {
                                            //comment by fitri 7-12-2021
                                            //CC = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_IOcode).FirstOrDefault();
                                            //add by fitri 7-12-2021
                                            CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                            leveldesc = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_NamaPktUtama.ToUpper()).FirstOrDefault();
                                            mainlevelcode = krjak.fld_KodPkt;
                                        }
                                        else
                                        {
                                            var getpktutm = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).FirstOrDefault();
                                            leveldesc = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).Select(s => s.fld_NamaPkt.ToUpper()).FirstOrDefault();
                                            //comment by fitri 7-12-2021
                                            //CC = GetPktUtamas.Where(x => x.fld_PktUtama == getpktutm.fld_KodPktUtama).Select(s => s.fld_IOcode).FirstOrDefault();
                                            //add by fitri 7-12-2021
                                            CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                            mainlevelcode = getpktutm.fld_KodPktUtama;
                                        }
                                        var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                                        var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                                        var actvtname = getallactvt.Where(x => x.fld_KodAktvt == krjak.fld_KodAktvt).Select(s => s.fld_Desc).FirstOrDefault();
                                        DetailWork.Add(new CustMod_DetailWork2
                                        {
                                            id = id,
                                            amounttype = 1,
                                            amount = krjak.fld_Amount.Value,
                                            quantity = krjak.fld_JumlahHasil.Value,
                                            wrk = krjak.fld_Nopkj,
                                            wrkname = krjak.fld_Nama,
                                            actvt = krjak.fld_KodAktvt,
                                            actvtdesc = actvtname,
                                            cc = CC,
                                            gl = krjak.fld_GLKod,
                                            grp = krjak.fld_Kum,
                                            pkt = mainlevelcode,
                                            fld_LadangID = krjak.fld_LadangID.Value,
                                            fld_WilayahID = krjak.fld_WilayahID.Value,
                                            fld_SyarikatID = krjak.fld_SyarikatID.Value,
                                            fld_NegaraID = krjak.fld_NegaraID.Value,
                                            active = actstatus
                                        });
                                        id++;
                                    }
                                }
                                if (listactvtp.Count() > 0)
                                {
                                    var getkrjak = getworkdetails.Where(x => listactvtp.Contains(x.fld_KodAktvt)).ToList();
                                    foreach (var krjak in getkrjak)
                                    {
                                        var CC = "";
                                        var leveldesc = "";
                                        var mainlevelcode = "";
                                        if (krjak.fld_JnsPkt == 1)
                                        {
                                            //comment by fitri 7-12-2021
                                            //CC = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_IOcode).FirstOrDefault();
                                            //add by fitri 7-12-2021
                                            CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                            leveldesc = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_NamaPktUtama.ToUpper()).FirstOrDefault();
                                            mainlevelcode = krjak.fld_KodPkt;
                                        }
                                        else
                                        {
                                            var getpktutm = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).FirstOrDefault();
                                            leveldesc = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).Select(s => s.fld_NamaPkt.ToUpper()).FirstOrDefault();
                                            //comment by fitri 7-12-2021
                                            //CC = GetPktUtamas.Where(x => x.fld_PktUtama == getpktutm.fld_KodPktUtama).Select(s => s.fld_IOcode).FirstOrDefault();
                                            //add by fitri 7-12-2021
                                            CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                            mainlevelcode = getpktutm.fld_KodPktUtama;
                                        }
                                        var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                                        var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                                        var actvtname = getallactvt.Where(x => x.fld_KodAktvt == krjak.fld_KodAktvt).Select(s => s.fld_Desc).FirstOrDefault();
                                        DetailWork.Add(new CustMod_DetailWork2
                                        {
                                            id = id,
                                            amounttype = 4,
                                            amount = krjak.fld_Amount.Value,
                                            quantity = krjak.fld_JumlahHasil.Value,
                                            wrk = krjak.fld_Nopkj,
                                            wrkname = pkjname,
                                            actvt = krjak.fld_KodAktvt,
                                            actvtdesc = actvtname,
                                            cc = CC,
                                            gl = krjak.fld_GLKod,
                                            grp = krjak.fld_Kum,
                                            pkt = mainlevelcode,
                                            fld_LadangID = krjak.fld_LadangID.Value,
                                            fld_WilayahID = krjak.fld_WilayahID.Value,
                                            fld_SyarikatID = krjak.fld_SyarikatID.Value,
                                            fld_NegaraID = krjak.fld_NegaraID.Value,
                                            active = actstatus
                                        });
                                        id++;
                                    }
                                }
                            }
                            else if ((listactvtk.Count() > 0 || listactvtp.Count() > 0) && EnbleAktvt && PilihanAktvt == "0")
                            {
                                if (listactvtk.Count() > 0)
                                {
                                    var getkrjak = getworkdetails.Where(x => listactvtk.Contains(x.fld_KodAktvt)).ToList();
                                    foreach (var krjak in getkrjak)
                                    {
                                        var CC = "";
                                        var leveldesc = "";
                                        var mainlevelcode = "";
                                        if (krjak.fld_JnsPkt == 1)
                                        {
                                            //comment by fitri 7-12-2021
                                            //CC = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_IOcode).FirstOrDefault();
                                            //add by fitri 7-12-2021
                                            CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                            leveldesc = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_NamaPktUtama.ToUpper()).FirstOrDefault();
                                            mainlevelcode = krjak.fld_KodPkt;
                                        }
                                        else
                                        {
                                            var getpktutm = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).FirstOrDefault();
                                            leveldesc = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).Select(s => s.fld_NamaPkt.ToUpper()).FirstOrDefault();
                                            //comment by fitri 7-12-2021
                                            //CC = GetPktUtamas.Where(x => x.fld_PktUtama == getpktutm.fld_KodPktUtama).Select(s => s.fld_IOcode).FirstOrDefault();
                                            //add by fitri 7-12-2021
                                            CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                            mainlevelcode = getpktutm.fld_KodPktUtama;
                                        }
                                        var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                                        var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                                        var actvtname = getallactvt.Where(x => x.fld_KodAktvt == krjak.fld_KodAktvt).Select(s => s.fld_Desc).FirstOrDefault();
                                        DetailWork.Add(new CustMod_DetailWork2
                                        {
                                            id = id,
                                            amounttype = 1,
                                            amount = krjak.fld_Amount.Value,
                                            quantity = krjak.fld_JumlahHasil.Value,
                                            wrk = krjak.fld_Nopkj,
                                            wrkname = pkjname,
                                            actvt = krjak.fld_KodAktvt,
                                            actvtdesc = actvtname,
                                            cc = CC,
                                            gl = krjak.fld_GLKod,
                                            grp = krjak.fld_Kum,
                                            pkt = mainlevelcode,
                                            fld_LadangID = krjak.fld_LadangID.Value,
                                            fld_WilayahID = krjak.fld_WilayahID.Value,
                                            fld_SyarikatID = krjak.fld_SyarikatID.Value,
                                            fld_NegaraID = krjak.fld_NegaraID.Value,
                                            active = actstatus
                                        });
                                        id++;
                                    }
                                }
                                if (listactvtp.Count() > 0)
                                {
                                    var getkrjak = getworkdetails.Where(x => listactvtp.Contains(x.fld_KodAktvt)).ToList();
                                    foreach (var krjak in getkrjak)
                                    {
                                        var CC = "";
                                        var leveldesc = "";
                                        var mainlevelcode = "";
                                        if (krjak.fld_JnsPkt == 1)
                                        {
                                            //comment by fitri 7-12-2021
                                            //CC = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_IOcode).FirstOrDefault();
                                            //add by fitri 7-12-2021
                                            CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                            leveldesc = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_NamaPktUtama.ToUpper()).FirstOrDefault();
                                            mainlevelcode = krjak.fld_KodPkt;
                                        }
                                        else
                                        {
                                            var getpktutm = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).FirstOrDefault();
                                            leveldesc = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).Select(s => s.fld_NamaPkt.ToUpper()).FirstOrDefault();
                                            //comment by fitri 7-12-2021
                                            //CC = GetPktUtamas.Where(x => x.fld_PktUtama == getpktutm.fld_KodPktUtama).Select(s => s.fld_IOcode).FirstOrDefault();
                                            //add by fitri 7-12-2021
                                            CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                            mainlevelcode = getpktutm.fld_KodPktUtama;
                                        }
                                        var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                                        var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                                        var actvtname = getallactvt.Where(x => x.fld_KodAktvt == krjak.fld_KodAktvt).Select(s => s.fld_Desc).FirstOrDefault();
                                        DetailWork.Add(new CustMod_DetailWork2
                                        {
                                            id = id,
                                            amounttype = 4,
                                            amount = krjak.fld_Amount.Value,
                                            quantity = krjak.fld_JumlahHasil.Value,
                                            wrk = krjak.fld_Nopkj,
                                            wrkname = pkjname,
                                            actvt = krjak.fld_KodAktvt,
                                            actvtdesc = actvtname,
                                            cc = CC,
                                            gl = krjak.fld_GLKod,
                                            grp = krjak.fld_Kum,
                                            pkt = mainlevelcode,
                                            fld_LadangID = krjak.fld_LadangID.Value,
                                            fld_WilayahID = krjak.fld_WilayahID.Value,
                                            fld_SyarikatID = krjak.fld_SyarikatID.Value,
                                            fld_NegaraID = krjak.fld_NegaraID.Value,
                                            active = actstatus
                                        });
                                        id++;
                                    }
                                }
                            }
                            else if (EnbleAktvt && actvt.Count() == 1)
                            {
                                var getactiviti = getallactvt.Where(x => actvt.Contains(x.fld_KodAktvt)).FirstOrDefault();
                                short amouttype = 0;
                                if (getactiviti.fld_DisabledFlag == 3)
                                {
                                    amouttype = 1;
                                }
                                else
                                {
                                    amouttype = 4;
                                }
                                var getkrjak = getworkdetails.Where(x => actvt.Contains(x.fld_KodAktvt)).ToList();
                                foreach (var krjak in getkrjak)
                                {
                                    var CC = "";
                                    var leveldesc = "";
                                    var mainlevelcode = "";
                                    if (krjak.fld_JnsPkt == 1)
                                    {
                                        //comment by fitri 7-12-2021
                                        //CC = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_IOcode).FirstOrDefault();
                                        //add by fitri 7-12-2021
                                        CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                        leveldesc = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_NamaPktUtama.ToUpper()).FirstOrDefault();
                                        mainlevelcode = krjak.fld_KodPkt;
                                    }
                                    else
                                    {
                                        var getpktutm = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).FirstOrDefault();
                                        leveldesc = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).Select(s => s.fld_NamaPkt.ToUpper()).FirstOrDefault();
                                        //comment by fitri 7-12-2021
                                        //CC = GetPktUtamas.Where(x => x.fld_PktUtama == getpktutm.fld_KodPktUtama).Select(s => s.fld_IOcode).FirstOrDefault();
                                        //add by fitri 7-12-2021
                                        CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                        mainlevelcode = getpktutm.fld_KodPktUtama;
                                    }
                                    var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                                    var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                                    var actvtname = getallactvt.Where(x => x.fld_KodAktvt == krjak.fld_KodAktvt).Select(s => s.fld_Desc).FirstOrDefault();
                                    DetailWork.Add(new CustMod_DetailWork2
                                    {
                                        id = id,
                                        amounttype = amouttype,
                                        amount = krjak.fld_Amount.Value,
                                        quantity = krjak.fld_JumlahHasil.Value,
                                        wrk = krjak.fld_Nopkj,
                                        wrkname = pkjname,
                                        actvt = krjak.fld_KodAktvt,
                                        actvtdesc = actvtname,
                                        cc = CC,
                                        gl = krjak.fld_GLKod,
                                        grp = krjak.fld_Kum,
                                        pkt = mainlevelcode,
                                        fld_LadangID = krjak.fld_LadangID.Value,
                                        fld_WilayahID = krjak.fld_WilayahID.Value,
                                        fld_SyarikatID = krjak.fld_SyarikatID.Value,
                                        fld_NegaraID = krjak.fld_NegaraID.Value,
                                        active = actstatus
                                    });
                                    id++;
                                }
                            }
                            //bool Selection21 = KONG , bool Selection22 = OT, bool Selection23 = OTHERS, bool Selection24 = PIECE RATE, bool Selection25 = INCENTIVE, bool Selection26 = DEDUCTION
                            //amounttype KONG = 1, OT = 2, OTHERS = 3, PIECE RATE = 4, INCENTIVE = 5, DEDUCTION = 6
                            //OT
                            if (Selection22)
                            {
                                var getot = getworkdetails.Where(x => x.fld_JumlahOT > 0).ToList();
                                foreach (var ot in getot)
                                {
                                    var CC = "";
                                    var leveldesc = "";
                                    var mainlevelcode = "";
                                    if (ot.fld_JnsPkt == 1)
                                    {
                                        //comment by fitri 7-12-2021
                                        //CC = GetPktUtamas.Where(x => x.fld_PktUtama == ot.fld_KodPkt).Select(s => s.fld_IOcode).FirstOrDefault();
                                        //add by fitri 7-12-2021
                                        CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == ot.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                        leveldesc = GetPktUtamas.Where(x => x.fld_PktUtama == ot.fld_KodPkt).Select(s => s.fld_NamaPktUtama.ToUpper()).FirstOrDefault();
                                        mainlevelcode = ot.fld_KodPkt;
                                    }
                                    else
                                    {
                                        var getpktutm = GetPktSubs.Where(x => x.fld_Pkt == ot.fld_KodPkt).FirstOrDefault();
                                        leveldesc = GetPktSubs.Where(x => x.fld_Pkt == ot.fld_KodPkt).Select(s => s.fld_NamaPkt.ToUpper()).FirstOrDefault();
                                        //comment by fitri 7-12-2021
                                        //CC = GetPktUtamas.Where(x => x.fld_PktUtama == getpktutm.fld_KodPktUtama).Select(s => s.fld_IOcode).FirstOrDefault();
                                        //add by fitri 7-12-2021
                                        CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == ot.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                        mainlevelcode = getpktutm.fld_KodPktUtama;
                                    }
                                    var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == ot.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                                    var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == ot.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                                    var actvtname = getallactvt.Where(x => x.fld_KodAktvt == ot.fld_KodAktvt).Select(s => s.fld_Desc).FirstOrDefault();
                                    DetailWork.Add(new CustMod_DetailWork2
                                    {
                                        id = id,
                                        amounttype = 2,
                                        amount = ot.fld_JumlahOT.Value,
                                        quantity = ot.fld_JamOT.Value,
                                        wrk = ot.fld_Nopkj,
                                        wrkname = pkjname,
                                        actvt = ot.fld_KodAktvt,
                                        actvtdesc = actvtname,
                                        cc = CC,
                                        gl = ot.fld_GLKod,
                                        grp = ot.fld_Kum,
                                        pkt = mainlevelcode,
                                        fld_LadangID = ot.fld_LadangID.Value,
                                        fld_WilayahID = ot.fld_WilayahID.Value,
                                        fld_SyarikatID = ot.fld_SyarikatID.Value,
                                        fld_NegaraID = ot.fld_NegaraID.Value,
                                        active = actstatus
                                    });
                                    id++;
                                }
                            }
                            //OT
                            //bool Selection21 = KONG , bool Selection22 = OT, bool Selection23 = OTHERS, bool Selection24 = PIECE RATE, bool Selection25 = INCENTIVE, bool Selection26 = DEDUCTION
                            //amounttype KONG = 1, OT = 2, OTHERS = 3, PIECE RATE = 4, INCENTIVE = 5, DEDUCTION = 6
                            //BONUS
                            if (Selection25)
                            {
                                var getinctive = getworkdetails.Where(x => x.fld_Bonus > 0).ToList();
                                foreach (var incentive in getinctive)
                                {
                                    var CC = "";
                                    var leveldesc = "";
                                    var mainlevelcode = "";
                                    if (incentive.fld_JnsPkt == 1)
                                    {
                                        //comment by fitri 7-12-2021
                                        //CC = GetPktUtamas.Where(x => x.fld_PktUtama == incentive.fld_KodPkt).Select(s => s.fld_IOcode).FirstOrDefault();
                                        //add by fitri 7-12-2021
                                        CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == incentive.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                        leveldesc = GetPktUtamas.Where(x => x.fld_PktUtama == incentive.fld_KodPkt).Select(s => s.fld_NamaPktUtama.ToUpper()).FirstOrDefault();
                                        mainlevelcode = incentive.fld_KodPkt;
                                    }
                                    else
                                    {
                                        var getpktutm = GetPktSubs.Where(x => x.fld_Pkt == incentive.fld_KodPkt).FirstOrDefault();
                                        leveldesc = GetPktSubs.Where(x => x.fld_Pkt == incentive.fld_KodPkt).Select(s => s.fld_NamaPkt.ToUpper()).FirstOrDefault();
                                        //comment by fitri 7-12-2021
                                        //CC = GetPktUtamas.Where(x => x.fld_PktUtama == getpktutm.fld_KodPktUtama).Select(s => s.fld_IOcode).FirstOrDefault();
                                        //add by fitri 7-12-2021
                                        CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == incentive.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                        mainlevelcode = getpktutm.fld_KodPktUtama;
                                    }
                                    var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == incentive.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                                    var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == incentive.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                                    var actvtname = getallactvt.Where(x => x.fld_KodAktvt == incentive.fld_KodAktvt).Select(s => s.fld_Desc).FirstOrDefault();
                                    DetailWork.Add(new CustMod_DetailWork2
                                    {
                                        id = id,
                                        amounttype = 5,
                                        amount = incentive.fld_JumlahBonus.Value,
                                        quantity = 1,
                                        wrk = incentive.fld_Nopkj,
                                        wrkname = pkjname,
                                        actvt = incentive.fld_KodAktvt,
                                        actvtdesc = actvtname,
                                        cc = CC,
                                        gl = incentive.fld_GLKod,
                                        grp = incentive.fld_Kum,
                                        pkt = mainlevelcode,
                                        fld_LadangID = incentive.fld_LadangID.Value,
                                        fld_WilayahID = incentive.fld_WilayahID.Value,
                                        fld_SyarikatID = incentive.fld_SyarikatID.Value,
                                        fld_NegaraID = incentive.fld_NegaraID.Value,
                                        active = actstatus
                                    });
                                    id++;
                                }
                            }
                            //BONUS
                        }
                        //get kerja
                        //bool Selection21 = KONG , bool Selection22 = OT, bool Selection23 = OTHERS, bool Selection24 = PIECE RATE, bool Selection25 = INCENTIVE, bool Selection26 = DEDUCTION
                        //amounttype KONG = 1, OT = 2, OTHERS = 3, PIECE RATE = 4, INCENTIVE = 5, DEDUCTION = 6

                        //Leave
                        var GetLeaveGL = db.tbl_CustomerVendorGLMap.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).ToList();
                        if (Selection23)
                        {
                            var GetActCodeLeave = db.tbl_CutiKategori.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).ToList();
                            var GetWorkercuti = dbr.vw_Kerja_Hdr_Cuti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value >= startdt.Date && x.fld_Tarikh.Value <= enddt.Date).OrderBy(o => new { o.fld_Tarikh, o.fld_Nopkj }).ToList();
                            foreach (var Workercuti in GetWorkercuti)
                            {
                                var CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == Workercuti.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                var Krytn = tbl_Pkjmast.Where(x => x.fld_Nopkj == Workercuti.fld_Nopkj).Select(s => s.fld_Kdrkyt).FirstOrDefault();
                                var getworkdetail = tbl_Pkjmast.Where(x => x.fld_Nopkj == Workercuti.fld_Nopkj).FirstOrDefault();
                                var ActvtCodeDesc = GetActCodeLeave.Where(x => x.fld_KodCuti == Workercuti.fld_Kdhdct).Select(s => new { s.fld_KodAktvt, s.fld_KeteranganCuti }).FirstOrDefault();
                                string GL = "";
                                if (Krytn == "MA")
                                {
                                    GL = GetLeaveGL.Where(x => x.fld_KodAktiviti == ActvtCodeDesc.fld_KodAktvt && x.fld_TypeCode == "GLTKT" && x.fld_Flag == "2").Select(s => s.fld_SAPCode).FirstOrDefault();
                                }
                                else
                                {
                                    GL = GetLeaveGL.Where(x => x.fld_KodAktiviti == ActvtCodeDesc.fld_KodAktvt && x.fld_TypeCode == "GLTKA" && x.fld_Flag == "2").Select(s => s.fld_SAPCode).FirstOrDefault();
                                }
                                var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == Workercuti.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                                var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == Workercuti.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                                DetailWork.Add(new CustMod_DetailWork2
                                {
                                    id = id,
                                    amounttype = 3,
                                    amount = Workercuti.fld_Jumlah.Value,
                                    quantity = 1,
                                    wrk = Workercuti.fld_Nopkj,
                                    wrkname = pkjname,
                                    actvt = ActvtCodeDesc.fld_KodAktvt,
                                    actvtdesc = ActvtCodeDesc.fld_KeteranganCuti,
                                    cc = CC,
                                    gl = GL,
                                    grp = Workercuti.fld_Kum,
                                    pkt = null,
                                    fld_LadangID = Workercuti.fld_LadangID.Value,
                                    fld_WilayahID = Workercuti.fld_WilayahID.Value,
                                    fld_SyarikatID = Workercuti.fld_SyarikatID.Value,
                                    fld_NegaraID = Workercuti.fld_NegaraID.Value,
                                    active = actstatus
                                });
                                id++;
                            }
                        }
                        //Leave

                        //bool Selection21 = KONG , bool Selection22 = OT, bool Selection23 = OTHERS, bool Selection24 = PIECE RATE, bool Selection25 = INCENTIVE, bool Selection26 = DEDUCTION
                        //amounttype KONG = 1, OT = 2, OTHERS = 3, PIECE RATE = 4, INCENTIVE = 5, DEDUCTION = 6

                        //Incentive
                        if (Selection25)
                        {
                            var GetIncetive = db.tbl_JenisInsentif.Where(x => x.fld_JenisInsentif == "P").ToList();
                            var GetIncetiveCode = GetIncetive.Select(s => s.fld_KodInsentif).ToList();
                            var GetWorkerIncntve = dbr.tbl_Insentif.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month >= startdt.Month && x.fld_Month <= enddt.Month && x.fld_Year == enddt.Year && x.fld_Deleted == false && GetIncetiveCode.Contains(x.fld_KodInsentif) && x.fld_NilaiInsentif > 0).OrderBy(o => new { o.fld_Month, o.fld_Nopkj }).ToList();
                            var GetGroups = dbr.tbl_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
                            foreach (var WorkerIncntve in GetWorkerIncntve)
                            {
                                var CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == WorkerIncntve.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                var Krytn = tbl_Pkjmast.Where(x => x.fld_Nopkj == WorkerIncntve.fld_Nopkj).Select(s => s.fld_Kdrkyt).FirstOrDefault();
                                var getworkdetail = tbl_Pkjmast.Where(x => x.fld_Nopkj == WorkerIncntve.fld_Nopkj).FirstOrDefault();
                                var ActvtCodeDesc = GetIncetive.Where(x => x.fld_KodInsentif == WorkerIncntve.fld_KodInsentif).Select(s => new { s.fld_KodAktvt, s.fld_Keterangan }).FirstOrDefault();
                                var Grp = GetGroups.Where(x => x.fld_KumpulanID == getworkdetail.fld_KumpulanID).Select(s => s.fld_KodKumpulan).FirstOrDefault() == null ? "" : GetGroups.Where(x => x.fld_KumpulanID == getworkdetail.fld_KumpulanID).Select(s => s.fld_KodKumpulan).FirstOrDefault();
                                string GL = "";
                                if (Krytn == "MA")
                                {
                                    GL = GetLeaveGL.Where(x => x.fld_KodAktiviti == ActvtCodeDesc.fld_KodAktvt && x.fld_TypeCode == "GLTKT" && x.fld_Flag == "5").Select(s => s.fld_SAPCode).FirstOrDefault();
                                }
                                else
                                {
                                    GL = GetLeaveGL.Where(x => x.fld_KodAktiviti == ActvtCodeDesc.fld_KodAktvt && x.fld_TypeCode == "GLTKA" && x.fld_Flag == "5").Select(s => s.fld_SAPCode).FirstOrDefault();
                                }
                                var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == WorkerIncntve.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                                var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == WorkerIncntve.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                                DetailWork.Add(new CustMod_DetailWork2
                                {
                                    id = id,
                                    amounttype = 5,
                                    amount = WorkerIncntve.fld_NilaiInsentif.Value,
                                    quantity = 1,
                                    wrk = WorkerIncntve.fld_Nopkj,
                                    wrkname = pkjname,
                                    actvt = ActvtCodeDesc.fld_KodAktvt,
                                    actvtdesc = ActvtCodeDesc.fld_Keterangan,
                                    cc = CC,
                                    gl = GL,
                                    grp = Grp,
                                    pkt = null,
                                    fld_LadangID = WorkerIncntve.fld_LadangID.Value,
                                    fld_WilayahID = WorkerIncntve.fld_WilayahID.Value,
                                    fld_SyarikatID = WorkerIncntve.fld_SyarikatID.Value,
                                    fld_NegaraID = WorkerIncntve.fld_NegaraID.Value,
                                    active = actstatus
                                });
                                id++;
                            }
                        }
                        //Incentive

                        //bool Selection21 = KONG , bool Selection22 = OT, bool Selection23 = OTHERS, bool Selection24 = PIECE RATE, bool Selection25 = INCENTIVE, bool Selection26 = DEDUCTION
                        //amounttype KONG = 1, OT = 2, OTHERS = 3, PIECE RATE = 4, INCENTIVE = 5, DEDUCTION = 6

                        //Deduction
                        if (Selection26)
                        {
                            var GetIncetive = db.tbl_JenisInsentif.Where(x => x.fld_JenisInsentif == "T").ToList();
                            var GetIncetiveCode = GetIncetive.Select(s => s.fld_KodInsentif).ToList();
                            var GetWorkerIncntve = dbr.tbl_Insentif.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month >= startdt.Month && x.fld_Month <= enddt.Month && x.fld_Year == enddt.Year && x.fld_Deleted == false && GetIncetiveCode.Contains(x.fld_KodInsentif) && x.fld_NilaiInsentif > 0).OrderBy(o => new { o.fld_Month, o.fld_Nopkj }).ToList();
                            var GetGroups = dbr.tbl_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
                            foreach (var WorkerIncntve in GetWorkerIncntve)
                            {
                                var CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == WorkerIncntve.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                var Krytn = tbl_Pkjmast.Where(x => x.fld_Nopkj == WorkerIncntve.fld_Nopkj).Select(s => s.fld_Kdrkyt).FirstOrDefault();
                                var getworkdetail = tbl_Pkjmast.Where(x => x.fld_Nopkj == WorkerIncntve.fld_Nopkj).FirstOrDefault();
                                var ActvtCodeDesc = GetIncetive.Where(x => x.fld_KodInsentif == WorkerIncntve.fld_KodInsentif).Select(s => new { s.fld_KodAktvt, s.fld_Keterangan }).FirstOrDefault();
                                var Grp = GetGroups.Where(x => x.fld_KumpulanID == getworkdetail.fld_KumpulanID).Select(s => s.fld_KodKumpulan).FirstOrDefault() == null ? "" : GetGroups.Where(x => x.fld_KumpulanID == getworkdetail.fld_KumpulanID).Select(s => s.fld_KodKumpulan).FirstOrDefault();
                                string GL = "";
                                if (Krytn == "MA")
                                {
                                    GL = GetLeaveGL.Where(x => x.fld_KodAktiviti == ActvtCodeDesc.fld_KodAktvt && x.fld_TypeCode == "GLTKT" && x.fld_Flag == "5").Select(s => s.fld_SAPCode).FirstOrDefault();
                                }
                                else
                                {
                                    GL = GetLeaveGL.Where(x => x.fld_KodAktiviti == ActvtCodeDesc.fld_KodAktvt && x.fld_TypeCode == "GLTKA" && x.fld_Flag == "5").Select(s => s.fld_SAPCode).FirstOrDefault();
                                }
                                var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == WorkerIncntve.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                                var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == WorkerIncntve.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                                DetailWork.Add(new CustMod_DetailWork2
                                {
                                    id = id,
                                    amounttype = 6,
                                    amount = -WorkerIncntve.fld_NilaiInsentif.Value,
                                    quantity = 1,
                                    wrk = WorkerIncntve.fld_Nopkj,
                                    wrkname = pkjname,
                                    actvt = ActvtCodeDesc.fld_KodAktvt,
                                    actvtdesc = ActvtCodeDesc.fld_Keterangan,
                                    cc = CC,
                                    gl = GL,
                                    grp = Grp,
                                    pkt = null,
                                    fld_LadangID = WorkerIncntve.fld_LadangID.Value,
                                    fld_WilayahID = WorkerIncntve.fld_WilayahID.Value,
                                    fld_SyarikatID = WorkerIncntve.fld_SyarikatID.Value,
                                    fld_NegaraID = WorkerIncntve.fld_NegaraID.Value,
                                    active = actstatus
                                });
                                id++;
                            }
                        }
                        //Deduction
                    }
                    else
                    {
                        var getpiece = getallactvt.Where(x => x.fld_DisabledFlag != 3).Select(s => s.fld_KodAktvt).Distinct().ToList();
                        var getworkdetails = dbr.vw_KerjaInfoDetails2.Where(x => getpiece.Contains(x.fld_KodAktvt) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value >= startdt.Date && x.fld_Tarikh.Value <= enddt.Date && wrk.Contains(x.fld_Nopkj)).ToList();
                        if ((listactvtk.Count() > 0 || listactvtp.Count() > 0) && !EnbleAktvt)
                        {
                            if (listactvtk.Count() > 0)
                            {
                                var getkrjak = getworkdetails.Where(x => listactvtk.Contains(x.fld_KodAktvt)).ToList();
                                foreach (var krjak in getkrjak)
                                {
                                    var CC = "";
                                    var leveldesc = "";
                                    var mainlevelcode = "";
                                    if (krjak.fld_JnsPkt == 1)
                                    {
                                        //comment by fitri 7-12-2021
                                        //CC = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_IOcode).FirstOrDefault();
                                        //add by fitri 7-12-2021
                                        CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                        leveldesc = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_NamaPktUtama.ToUpper()).FirstOrDefault();
                                        mainlevelcode = krjak.fld_KodPkt;
                                    }
                                    else
                                    {
                                        var getpktutm = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).FirstOrDefault();
                                        leveldesc = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).Select(s => s.fld_NamaPkt.ToUpper()).FirstOrDefault();
                                        //comment by fitri 7-12-2021
                                        //CC = GetPktUtamas.Where(x => x.fld_PktUtama == getpktutm.fld_KodPktUtama).Select(s => s.fld_IOcode).FirstOrDefault();
                                        //add by fitri 7-12-2021
                                        CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                        mainlevelcode = getpktutm.fld_KodPktUtama;
                                    }
                                    var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                                    var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                                    var actvtname = getallactvt.Where(x => x.fld_KodAktvt == krjak.fld_KodAktvt).Select(s => s.fld_Desc).FirstOrDefault();
                                    DetailWork.Add(new CustMod_DetailWork2
                                    {
                                        id = id,
                                        amounttype = 1,
                                        amount = 0,
                                        quantity = krjak.fld_JumlahHasil.Value,
                                        wrk = krjak.fld_Nopkj,
                                        wrkname = krjak.fld_Nama,
                                        actvt = krjak.fld_KodAktvt,
                                        actvtdesc = actvtname,
                                        cc = CC,
                                        gl = krjak.fld_GLKod,
                                        grp = krjak.fld_Kum,
                                        pkt = mainlevelcode,
                                        fld_LadangID = krjak.fld_LadangID.Value,
                                        fld_WilayahID = krjak.fld_WilayahID.Value,
                                        fld_SyarikatID = krjak.fld_SyarikatID.Value,
                                        fld_NegaraID = krjak.fld_NegaraID.Value,
                                        active = actstatus
                                    });
                                    id++;
                                }
                            }
                            if (listactvtp.Count() > 0)
                            {
                                var getkrjak = getworkdetails.Where(x => listactvtp.Contains(x.fld_KodAktvt)).ToList();
                                foreach (var krjak in getkrjak)
                                {
                                    var CC = "";
                                    var leveldesc = "";
                                    var mainlevelcode = "";
                                    if (krjak.fld_JnsPkt == 1)
                                    {
                                        //comment by fitri 7-12-2021
                                        //CC = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_IOcode).FirstOrDefault();
                                        //add by fitri 7-12-2021
                                        CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                        leveldesc = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_NamaPktUtama.ToUpper()).FirstOrDefault();
                                        mainlevelcode = krjak.fld_KodPkt;
                                    }
                                    else
                                    {
                                        var getpktutm = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).FirstOrDefault();
                                        leveldesc = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).Select(s => s.fld_NamaPkt.ToUpper()).FirstOrDefault();
                                        //comment by fitri 7-12-2021
                                        //CC = GetPktUtamas.Where(x => x.fld_PktUtama == getpktutm.fld_KodPktUtama).Select(s => s.fld_IOcode).FirstOrDefault();
                                        //add by fitri 7-12-2021
                                        CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                        mainlevelcode = getpktutm.fld_KodPktUtama;
                                    }
                                    var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                                    var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                                    var actvtname = getallactvt.Where(x => x.fld_KodAktvt == krjak.fld_KodAktvt).Select(s => s.fld_Desc).FirstOrDefault();
                                    DetailWork.Add(new CustMod_DetailWork2
                                    {
                                        id = id,
                                        amounttype = 4,
                                        amount = 0,
                                        quantity = krjak.fld_JumlahHasil.Value,
                                        wrk = krjak.fld_Nopkj,
                                        wrkname = pkjname,
                                        actvt = krjak.fld_KodAktvt,
                                        actvtdesc = actvtname,
                                        cc = CC,
                                        gl = krjak.fld_GLKod,
                                        grp = krjak.fld_Kum,
                                        pkt = mainlevelcode,
                                        fld_LadangID = krjak.fld_LadangID.Value,
                                        fld_WilayahID = krjak.fld_WilayahID.Value,
                                        fld_SyarikatID = krjak.fld_SyarikatID.Value,
                                        fld_NegaraID = krjak.fld_NegaraID.Value,
                                        active = actstatus
                                    });
                                    id++;
                                }
                            }
                        }
                        else if ((listactvtk.Count() > 0 || listactvtp.Count() > 0) && EnbleAktvt && PilihanAktvt == "0")
                        {
                            if (listactvtk.Count() > 0)
                            {
                                var getkrjak = getworkdetails.Where(x => listactvtk.Contains(x.fld_KodAktvt)).ToList();
                                foreach (var krjak in getkrjak)
                                {
                                    var CC = "";
                                    var leveldesc = "";
                                    var mainlevelcode = "";
                                    if (krjak.fld_JnsPkt == 1)
                                    {
                                        //comment by fitri 7-12-2021
                                        //CC = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_IOcode).FirstOrDefault();
                                        //add by fitri 7-12-2021
                                        CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                        leveldesc = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_NamaPktUtama.ToUpper()).FirstOrDefault();
                                        mainlevelcode = krjak.fld_KodPkt;
                                    }
                                    else
                                    {
                                        var getpktutm = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).FirstOrDefault();
                                        leveldesc = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).Select(s => s.fld_NamaPkt.ToUpper()).FirstOrDefault();
                                        //comment by fitri 7-12-2021
                                        //CC = GetPktUtamas.Where(x => x.fld_PktUtama == getpktutm.fld_KodPktUtama).Select(s => s.fld_IOcode).FirstOrDefault();
                                        //add by fitri 7-12-2021
                                        CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                        mainlevelcode = getpktutm.fld_KodPktUtama;
                                    }
                                    var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                                    var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                                    var actvtname = getallactvt.Where(x => x.fld_KodAktvt == krjak.fld_KodAktvt).Select(s => s.fld_Desc).FirstOrDefault();
                                    DetailWork.Add(new CustMod_DetailWork2
                                    {
                                        id = id,
                                        amounttype = 1,
                                        amount = 0,
                                        quantity = krjak.fld_JumlahHasil.Value,
                                        wrk = krjak.fld_Nopkj,
                                        wrkname = pkjname,
                                        actvt = krjak.fld_KodAktvt,
                                        actvtdesc = actvtname,
                                        cc = CC,
                                        gl = krjak.fld_GLKod,
                                        grp = krjak.fld_Kum,
                                        pkt = mainlevelcode,
                                        fld_LadangID = krjak.fld_LadangID.Value,
                                        fld_WilayahID = krjak.fld_WilayahID.Value,
                                        fld_SyarikatID = krjak.fld_SyarikatID.Value,
                                        fld_NegaraID = krjak.fld_NegaraID.Value,
                                        active = actstatus
                                    });
                                    id++;
                                }
                            }
                            if (listactvtp.Count() > 0)
                            {
                                var getkrjak = getworkdetails.Where(x => listactvtp.Contains(x.fld_KodAktvt)).ToList();
                                foreach (var krjak in getkrjak)
                                {
                                    var CC = "";
                                    var leveldesc = "";
                                    var mainlevelcode = "";
                                    if (krjak.fld_JnsPkt == 1)
                                    {
                                        //comment by fitri 7-12-2021
                                        //CC = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_IOcode).FirstOrDefault();
                                        //add by fitri 7-12-2021
                                        CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                        leveldesc = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_NamaPktUtama.ToUpper()).FirstOrDefault();
                                        mainlevelcode = krjak.fld_KodPkt;
                                    }
                                    else
                                    {
                                        var getpktutm = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).FirstOrDefault();
                                        leveldesc = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).Select(s => s.fld_NamaPkt.ToUpper()).FirstOrDefault();
                                        //comment by fitri 7-12-2021
                                        //CC = GetPktUtamas.Where(x => x.fld_PktUtama == getpktutm.fld_KodPktUtama).Select(s => s.fld_IOcode).FirstOrDefault();
                                        //add by fitri 7-12-2021
                                        CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                        mainlevelcode = getpktutm.fld_KodPktUtama;
                                    }
                                    var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                                    var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                                    var actvtname = getallactvt.Where(x => x.fld_KodAktvt == krjak.fld_KodAktvt).Select(s => s.fld_Desc).FirstOrDefault();
                                    DetailWork.Add(new CustMod_DetailWork2
                                    {
                                        id = id,
                                        amounttype = 4,
                                        amount = 0,
                                        quantity = krjak.fld_JumlahHasil.Value,
                                        wrk = krjak.fld_Nopkj,
                                        wrkname = pkjname,
                                        actvt = krjak.fld_KodAktvt,
                                        actvtdesc = actvtname,
                                        cc = CC,
                                        gl = krjak.fld_GLKod,
                                        grp = krjak.fld_Kum,
                                        pkt = mainlevelcode,
                                        fld_LadangID = krjak.fld_LadangID.Value,
                                        fld_WilayahID = krjak.fld_WilayahID.Value,
                                        fld_SyarikatID = krjak.fld_SyarikatID.Value,
                                        fld_NegaraID = krjak.fld_NegaraID.Value,
                                        active = actstatus
                                    });
                                    id++;
                                }
                            }
                        }
                        else if (EnbleAktvt && actvt.Count() == 1)
                        {
                            var getkrjak = getworkdetails.Where(x => actvt.Contains(x.fld_KodAktvt)).ToList();
                            foreach (var krjak in getkrjak)
                            {
                                var CC = "";
                                var leveldesc = "";
                                var mainlevelcode = "";
                                if (krjak.fld_JnsPkt == 1)
                                {
                                    //comment by fitri 7-12-2021
                                    //CC = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_IOcode).FirstOrDefault();
                                    //add by fitri 7-12-2021
                                    CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                    leveldesc = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_NamaPktUtama.ToUpper()).FirstOrDefault();
                                    mainlevelcode = krjak.fld_KodPkt;
                                }
                                else
                                {
                                    var getpktutm = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).FirstOrDefault();
                                    leveldesc = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).Select(s => s.fld_NamaPkt.ToUpper()).FirstOrDefault();
                                    //comment by fitri 7-12-2021
                                    //CC = GetPktUtamas.Where(x => x.fld_PktUtama == getpktutm.fld_KodPktUtama).Select(s => s.fld_IOcode).FirstOrDefault();
                                    //add by fitri 7-12-2021
                                    CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                    mainlevelcode = getpktutm.fld_KodPktUtama;
                                }
                                var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                                var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                                var actvtname = getallactvt.Where(x => x.fld_KodAktvt == krjak.fld_KodAktvt).Select(s => s.fld_Desc).FirstOrDefault();
                                DetailWork.Add(new CustMod_DetailWork2
                                {
                                    id = id,
                                    //amounttype = 1, //fitri comment 24.09.2021
                                    amounttype = 4, //fitri add 24.09.2021
                                    amount = 0,
                                    quantity = krjak.fld_JumlahHasil.Value,
                                    wrk = krjak.fld_Nopkj,
                                    wrkname = pkjname,
                                    actvt = krjak.fld_KodAktvt,
                                    actvtdesc = actvtname,
                                    cc = CC,
                                    gl = krjak.fld_GLKod,
                                    grp = krjak.fld_Kum,
                                    pkt = mainlevelcode,
                                    fld_LadangID = krjak.fld_LadangID.Value,
                                    fld_WilayahID = krjak.fld_WilayahID.Value,
                                    fld_SyarikatID = krjak.fld_SyarikatID.Value,
                                    fld_NegaraID = krjak.fld_NegaraID.Value,
                                    active = actstatus
                                });
                                id++;
                            }
                        }
                    }
                }
                catch
                {

                }
                var Syarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).FirstOrDefault();
                var Ladang = db.tbl_Ladang.Where(x => x.fld_ID == LadangID).FirstOrDefault();

                ViewBag.NamaSyarikat = Syarikat.fld_NamaSyarikat;
                ViewBag.NoSyarikat = Syarikat.fld_NoSyarikat;
                ViewBag.NamaLadang = Ladang.fld_LdgName;
                ViewBag.Pilihan = Pilihan;
                ViewBag.Tempoh = startdt.ToString("dd/MM/yyyy") + " - " + enddt.ToString("dd/MM/yyyy");
                ViewBag.TitleReport = "LAPORAN AGIHAN PERBELANJAAN AKTIVITI KERJA";
                DetailWork = ReportLogic.CustMod_DetailWork2(DetailWork, cc, pkt, actvt, gl, grp, wrk); //DetailWork.Where(x => cc.Contains(x.cc) && pkt.Contains(x.pkt) && actvt.Contains(x.actvt) && gl.Contains(x.gl) && grp.Contains(x.grp) && wrk.Contains(x.wrk)).ToList();
                detailsreport = new detailsreport
                {
                    coloumgroup = coloumgroup,
                    CustMod_DetailWork2 = DetailWork
                };
            }
            return View(detailsreport);
        }
        public ActionResult LprnKrjaBlnan()
        {
            ViewBag.Report = "class = active";
            ViewBag.TitleReport = "LAPORAN KERJA BULANAN";
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

            List<SelectListItem> PilihanCC = new List<SelectListItem>();
            var getCC = db.tbl_SAPCCPUP.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
            PilihanCC = new SelectList(getCC.Select(s => new SelectListItem { Value = s.fld_CostCenter, Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc.ToUpper() }), "Value", "Text").ToList();
            var CC = getCC.Select(s => s.fld_CostCenter).Distinct().ToList();
            //fitri comment 13-01-2021
            //var getWBS = db.tbl_SAPPDPUP.Where(x => CC.Contains(x.fld_CostCenter) && x.fld_Deleted == false).Select(s => s.fld_WBSCode).Distinct().ToList();
            //var z = PilihanCC.Count();
            //foreach (var WBS in getWBS)
            //{
            //    PilihanCC.Insert(z, (new SelectListItem { Text = WBS, Value = WBS }));
            //    z++;
            //}
            PilihanCC.Insert(0, (new SelectListItem { Text = "ALL", Value = "0" }));
            List<SelectListItem> PilihanAktvt = new List<SelectListItem>();
            //comment by fitri 7.12.2021
            //var SelectAktvt = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).ToList();
            //PilihanAktvt = new SelectList(SelectAktvt.Select(s => new SelectListItem { Value = s.fld_KodAktvt, Text = s.fld_KodAktvt + " - " + s.fld_Desc }).Distinct().OrderBy(o => o.Value), "Value", "Text").ToList();
            //PilihanAktvt.Insert(0, (new SelectListItem { Text = "ALL", Value = "0" }));
            //add by fitri 7.12.2021
            var SelectAktvt = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_KodAktvt).GroupBy(g => new { g.fld_KodAktvt, g.fld_Desc }).ToList();
            PilihanAktvt = new SelectList(SelectAktvt.Select(s => new SelectListItem { Value = s.Key.fld_KodAktvt, Text = s.Key.fld_KodAktvt + " - " + s.Key.fld_Desc }), "Value", "Text").ToList();
            PilihanAktvt.Insert(0, (new SelectListItem { Text = "ALL", Value = "0" }));
            List<SelectListItem> PilihanPkt = new List<SelectListItem>();
            var SelectPkt = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
            PilihanPkt = new SelectList(SelectPkt.Select(s => new SelectListItem { Value = s.fld_PktUtama, Text = s.fld_PktUtama + " - " + s.fld_NamaPktUtama }), "Value", "Text").ToList();
            PilihanPkt.Insert(0, (new SelectListItem { Text = "ALL", Value = "0" }));
            List<SelectListItem> PilihanGL = new List<SelectListItem>();
            var SelectGL = db.tbl_MapGL.Join(db.tbl_SAPGLPUP, j => new { GL = j.fld_KodGL }, k => new { GL = k.fld_GLCode }, (j, k) => new { j.fld_ID, j.fld_KodGL, k.fld_GLDesc }).Select(s => new { s.fld_KodGL, s.fld_GLDesc }).Distinct().ToList();
            PilihanGL = new SelectList(SelectGL.Select(s => new SelectListItem { Value = s.fld_KodGL, Text = s.fld_KodGL + " - " + s.fld_GLDesc.ToUpper() }), "Value", "Text").ToList();
            PilihanGL.Insert(0, (new SelectListItem { Text = "ALL", Value = "0" }));
            List<SelectListItem> PilihanGrp = new List<SelectListItem>();
            var SelectGrp = dbr.tbl_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).ToList();
            PilihanGrp = new SelectList(SelectGrp.Select(s => new SelectListItem { Value = s.fld_KodKumpulan, Text = s.fld_KodKumpulan + " - " + s.fld_Keterangan.ToUpper() }), "Value", "Text").ToList();
            PilihanGrp.Insert(0, (new SelectListItem { Text = "ALL", Value = "0" }));
            List<SelectListItem> PilihanWrk = new List<SelectListItem>();
            //comment by fitri 13-01-2021
            //var SelectWrk = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o => o.fld_Nopkj).ToList();
            //add by fitri 13-01-2021
            var SelectWrk = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o => o.fld_Nama).ToList();
            PilihanWrk = new SelectList(SelectWrk.Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + " - " + s.fld_Nama.ToUpper() }), "Value", "Text").ToList();
            PilihanWrk.Insert(0, (new SelectListItem { Text = "ALL", Value = "0" }));
            List<SelectListItem> PilihanSts = new List<SelectListItem>();
            PilihanSts = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            PilihanSts.Insert(0, (new SelectListItem { Text = "ALL", Value = "0" }));
            ViewBag.PilihanCC = PilihanCC;
            ViewBag.PilihanSts = PilihanSts;
            ViewBag.PilihanWrk = PilihanWrk;
            ViewBag.PilihanGrp = PilihanGrp;
            ViewBag.PilihanGL = PilihanGL;
            ViewBag.PilihanPkt = PilihanPkt;
            ViewBag.PilihanAktvt = PilihanAktvt;
            ViewBag.MonthList = MonthList;
            ViewBag.MonthList2 = MonthList;
            ViewBag.YearList = yearlist;
            return View();
        }
        public ViewResult _LprnKrjaBlnan(int? MonthList, int? MonthList2, int? YearList, string PilihanSts, string PilihanWrk, string PilihanGrp, string PilihanGL, string PilihanPkt, string PilihanCC, string PilihanAktvt, int Selection1, bool Selection21, bool Selection22, bool Selection23, bool Selection24, bool Selection25, bool Selection26, bool EnbleCC, bool EnblePkt, bool EnbleAktvt, bool EnbleGL, bool EnbleGrp, bool EnbleWrk)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            bool proceed = true;
            var DetailWork = new List<CustMod_DetailWork2>();
            var coloumgroup = new List<coloumgroup>();
            var detailsreport = new detailsreport();
            if (!EnbleCC && !EnblePkt && !EnbleAktvt && !EnbleGL && !EnbleGrp && !EnbleWrk)
            {
                proceed = false;
            }
            if (!Selection21 && !Selection22 && !Selection23 && !Selection24 && !Selection25 && !Selection26)
            {
                proceed = false;
            }
            if (proceed)
            {
                var tbl_Pkjmast = new List<Models.tbl_Pkjmast>();
                if (PilihanSts == "0")
                {
                    tbl_Pkjmast = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Distinct().ToList();
                }
                else if (PilihanSts == "1")
                {
                    tbl_Pkjmast = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1").Distinct().ToList();
                }
                else
                {
                    tbl_Pkjmast = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "2").Distinct().ToList();
                }
                List<string> cc = new List<string>();
                List<string> pkt = new List<string>();
                List<string> actvt = new List<string>();
                List<string> gl = new List<string>();
                List<string> grp = new List<string>();
                List<string> wrk = new List<string>();
                int columngrp = 0;
                var Pilihan = "";
                var getallactvt = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).ToList();
                if (EnbleCC)
                {
                    if (PilihanCC == "0")
                    {
                        var getcc = db.tbl_SAPCCPUP.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).Select(s => s.fld_CostCenter).Distinct().ToList();
                        cc.AddRange(getcc);
                        //comment by fitri 13-01-2021
                        //var getWBS = db.tbl_SAPPDPUP.Where(x => getcc.Contains(x.fld_CostCenter) && x.fld_Deleted == false).Select(s => s.fld_WBSCode).Distinct().ToList();
                        //cc.AddRange(getWBS);
                    }
                    else
                    {
                        cc.Add(PilihanCC);
                    }
                    columngrp++;
                    coloumgroup.Add(new coloumgroup { id = columngrp, name = "Kos Pusat (Cost Centre)", type = 1 });
                    Pilihan += "Cost Center, ";
                }
                if (EnblePkt)
                {
                    if (PilihanPkt == "0")
                    {
                        var getpkt = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).Select(s => s.fld_PktUtama).Distinct().ToList();
                        pkt.AddRange(getpkt);
                    }
                    else
                    {
                        pkt.Add(PilihanPkt);
                    }
                    columngrp++;
                    coloumgroup.Add(new coloumgroup { id = columngrp, name = "Peringkat", type = 2 });
                    Pilihan += "Peringkat, ";
                }
                if (EnbleAktvt)
                {
                    if (PilihanAktvt == "0")
                    {
                        var getactvt2 = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_KodAktvt).Distinct().ToList();
                        actvt.AddRange(getactvt2);
                    }
                    else
                    {
                        actvt.Add(PilihanAktvt);
                    }
                    columngrp++;
                    coloumgroup.Add(new coloumgroup { id = columngrp, name = "Aktiviti", type = 3 });
                    Pilihan += "Aktiviti, ";
                }
                if (EnbleGL)
                {
                    if (PilihanGL == "0")
                    {
                        var getgl = db.tbl_MapGL.Join(db.tbl_SAPGLPUP, j => new { GL = j.fld_KodGL }, k => new { GL = k.fld_GLCode }, (j, k) => new { j.fld_ID, j.fld_KodGL, k.fld_GLDesc }).Select(s => s.fld_KodGL).Distinct().ToList();
                        gl.AddRange(getgl);
                    }
                    else
                    {
                        gl.Add(PilihanGL);
                    }
                    columngrp++;
                    coloumgroup.Add(new coloumgroup { id = columngrp, name = "Kod Akaun (GL SAP)", type = 4 });
                    Pilihan += "GL, ";
                }
                if (EnbleGrp)
                {
                    if (PilihanGrp == "0")
                    {
                        var getgrp = dbr.tbl_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).Select(s => s.fld_KodKumpulan).Distinct().ToList();
                        grp.AddRange(getgrp);
                    }
                    else
                    {
                        grp.Add(PilihanGrp);
                    }
                    columngrp++;
                    coloumgroup.Add(new coloumgroup { id = columngrp, name = "Kump", type = 5 });
                    Pilihan += "Kumpulan, ";
                }
                if (EnbleWrk)
                {
                    if (PilihanWrk == "0")
                    {
                        var getwrk = tbl_Pkjmast.Select(s => s.fld_Nopkj).Distinct().ToList();
                        wrk.AddRange(getwrk);
                    }
                    else
                    {
                        wrk.Add(PilihanWrk);
                    }
                    columngrp++;
                    coloumgroup.Add(new coloumgroup { id = columngrp, name = "Pekerja", type = 6 });
                    Pilihan += "Pekerja, ";
                }
                else
                {
                    var getwrk = tbl_Pkjmast.Select(s => s.fld_Nopkj).Distinct().ToList();
                    wrk.AddRange(getwrk);
                }
                //bool Selection21 = KONG , bool Selection22 = OT, bool Selection23 = OTHERS, bool Selection24 = PIECE RATE, bool Selection25 = INCENTIVE, bool Selection26 = DEDUCTION
                //amounttype KONG = 1, OT = 2, OTHERS = 3, PIECE RATE = 4, INCENTIVE = 5, DEDUCTION = 6
                //get kerja

                var proceedfordatakerja = false;
                var listactvtk = new List<string>();
                var listactvtp = new List<string>();
                if (Selection21 || Selection22 || Selection24 || Selection25)
                {
                    if (Selection21)
                    {
                        var getkong = getallactvt.Where(x => x.fld_DisabledFlag == 3).Select(s => s.fld_KodAktvt).Distinct().ToList();
                        listactvtk.AddRange(getkong);
                    }
                    if (Selection24)
                    {
                        var getpiece = getallactvt.Where(x => x.fld_DisabledFlag != 3).Select(s => s.fld_KodAktvt).Distinct().ToList();
                        listactvtp.AddRange(getpiece);
                    }
                    proceedfordatakerja = true;
                }
                int id = 1;
                var GetPktUtamas = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_IOcode).ToList();
                var GetPktSubs = dbr.tbl_SubPkt.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
                if (Selection1 == 0)
                {
                    if (proceedfordatakerja)
                    {
                        var getworkdetails = dbr.vw_KerjaInfoDetails2.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month >= MonthList && x.fld_Tarikh.Value.Month <= MonthList2 && x.fld_Tarikh.Value.Year == YearList && wrk.Contains(x.fld_Nopkj)).ToList();
                        if ((listactvtk.Count() > 0 || listactvtp.Count() > 0) && !EnbleAktvt)
                        {
                            if (listactvtk.Count() > 0)
                            {
                                var getkrjak = getworkdetails.Where(x => listactvtk.Contains(x.fld_KodAktvt)).ToList();
                                foreach (var krjak in getkrjak)
                                {
                                    var CC = "";
                                    var leveldesc = "";
                                    var mainlevelcode = "";
                                    if (krjak.fld_JnsPkt == 1)
                                    {
                                        //fitri comment 14-01-2021
                                        //CC = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_IOcode).FirstOrDefault();
                                        //fitri add 14-01-2021
                                        CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                        leveldesc = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_NamaPktUtama.ToUpper()).FirstOrDefault();
                                        mainlevelcode = krjak.fld_KodPkt;
                                    }
                                    else
                                    {
                                        var getpktutm = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).FirstOrDefault();
                                        leveldesc = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).Select(s => s.fld_NamaPkt.ToUpper()).FirstOrDefault();
                                        //fitri comment 14-01-2021
                                        //CC = GetPktUtamas.Where(x => x.fld_PktUtama == getpktutm.fld_KodPktUtama).Select(s => s.fld_IOcode).FirstOrDefault();
                                        //fitri add 14-01-2021
                                        CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                        mainlevelcode = getpktutm.fld_KodPktUtama;
                                    }
                                    var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                                    var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                                    var actvtname = getallactvt.Where(x => x.fld_KodAktvt == krjak.fld_KodAktvt).Select(s => s.fld_Desc).FirstOrDefault();
                                    DetailWork.Add(new CustMod_DetailWork2
                                    {
                                        id = id,
                                        amounttype = 1,
                                        amount = krjak.fld_Amount.Value,
                                        quantity = krjak.fld_JumlahHasil.Value,
                                        wrk = krjak.fld_Nopkj,
                                        wrkname = krjak.fld_Nama,
                                        actvt = krjak.fld_KodAktvt,
                                        actvtdesc = actvtname,
                                        cc = CC,
                                        gl = krjak.fld_GLKod,
                                        grp = krjak.fld_Kum,
                                        pkt = mainlevelcode,
                                        fld_LadangID = krjak.fld_LadangID.Value,
                                        fld_WilayahID = krjak.fld_WilayahID.Value,
                                        fld_SyarikatID = krjak.fld_SyarikatID.Value,
                                        fld_NegaraID = krjak.fld_NegaraID.Value,
                                        active = actstatus,
                                        month = krjak.fld_Tarikh.Value.Month,
                                        year = krjak.fld_Tarikh.Value.Year
                                    });
                                    id++;
                                }
                            }
                            if (listactvtp.Count() > 0)
                            {
                                var getkrjak = getworkdetails.Where(x => listactvtp.Contains(x.fld_KodAktvt)).ToList();
                                foreach (var krjak in getkrjak)
                                {
                                    var CC = "";
                                    var leveldesc = "";
                                    var mainlevelcode = "";
                                    if (krjak.fld_JnsPkt == 1)
                                    {
                                        //fitri comment 14-01-2021
                                        //CC = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_IOcode).FirstOrDefault();
                                        //fitri add 14-01-2021
                                        CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                        leveldesc = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_NamaPktUtama.ToUpper()).FirstOrDefault();
                                        mainlevelcode = krjak.fld_KodPkt;
                                    }
                                    else
                                    {
                                        var getpktutm = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).FirstOrDefault();
                                        leveldesc = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).Select(s => s.fld_NamaPkt.ToUpper()).FirstOrDefault();
                                        //fitri comment 14-01-2021
                                        //CC = GetPktUtamas.Where(x => x.fld_PktUtama == getpktutm.fld_KodPktUtama).Select(s => s.fld_IOcode).FirstOrDefault();
                                        //fitri add 14-01-2021
                                        CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                        mainlevelcode = getpktutm.fld_KodPktUtama;
                                    }
                                    var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                                    var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                                    var actvtname = getallactvt.Where(x => x.fld_KodAktvt == krjak.fld_KodAktvt).Select(s => s.fld_Desc).FirstOrDefault();
                                    DetailWork.Add(new CustMod_DetailWork2
                                    {
                                        id = id,
                                        amounttype = 4,
                                        amount = krjak.fld_Amount.Value,
                                        quantity = krjak.fld_JumlahHasil.Value,
                                        wrk = krjak.fld_Nopkj,
                                        wrkname = pkjname,
                                        actvt = krjak.fld_KodAktvt,
                                        actvtdesc = actvtname,
                                        cc = CC,
                                        gl = krjak.fld_GLKod,
                                        grp = krjak.fld_Kum,
                                        pkt = mainlevelcode,
                                        fld_LadangID = krjak.fld_LadangID.Value,
                                        fld_WilayahID = krjak.fld_WilayahID.Value,
                                        fld_SyarikatID = krjak.fld_SyarikatID.Value,
                                        fld_NegaraID = krjak.fld_NegaraID.Value,
                                        active = actstatus,
                                        month = krjak.fld_Tarikh.Value.Month,
                                        year = krjak.fld_Tarikh.Value.Year
                                    });
                                    id++;
                                }
                            }
                        }
                        else if ((listactvtk.Count() > 0 || listactvtp.Count() > 0) && EnbleAktvt && PilihanAktvt == "0")
                        {
                            if (listactvtk.Count() > 0)
                            {
                                var getkrjak = getworkdetails.Where(x => listactvtk.Contains(x.fld_KodAktvt)).ToList();
                                foreach (var krjak in getkrjak)
                                {
                                    var CC = "";
                                    var leveldesc = "";
                                    var mainlevelcode = "";
                                    if (krjak.fld_JnsPkt == 1)
                                    {
                                        //fitri comment 14-01-2021
                                        //CC = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_IOcode).FirstOrDefault();
                                        //fitri add 14-01-2021
                                        CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                        leveldesc = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_NamaPktUtama.ToUpper()).FirstOrDefault();
                                        mainlevelcode = krjak.fld_KodPkt;
                                    }
                                    else
                                    {
                                        var getpktutm = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).FirstOrDefault();
                                        leveldesc = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).Select(s => s.fld_NamaPkt.ToUpper()).FirstOrDefault();
                                        //fitri comment 14-01-2021
                                        //CC = GetPktUtamas.Where(x => x.fld_PktUtama == getpktutm.fld_KodPktUtama).Select(s => s.fld_IOcode).FirstOrDefault();
                                        //fitri add 14-01-2021
                                        CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                        mainlevelcode = getpktutm.fld_KodPktUtama;
                                    }
                                    var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                                    var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                                    var actvtname = getallactvt.Where(x => x.fld_KodAktvt == krjak.fld_KodAktvt).Select(s => s.fld_Desc).FirstOrDefault();
                                    DetailWork.Add(new CustMod_DetailWork2
                                    {
                                        id = id,
                                        amounttype = 1,
                                        amount = krjak.fld_Amount.Value,
                                        quantity = krjak.fld_JumlahHasil.Value,
                                        wrk = krjak.fld_Nopkj,
                                        wrkname = pkjname,
                                        actvt = krjak.fld_KodAktvt,
                                        actvtdesc = actvtname,
                                        cc = CC,
                                        gl = krjak.fld_GLKod,
                                        grp = krjak.fld_Kum,
                                        pkt = mainlevelcode,
                                        fld_LadangID = krjak.fld_LadangID.Value,
                                        fld_WilayahID = krjak.fld_WilayahID.Value,
                                        fld_SyarikatID = krjak.fld_SyarikatID.Value,
                                        fld_NegaraID = krjak.fld_NegaraID.Value,
                                        active = actstatus,
                                        month = krjak.fld_Tarikh.Value.Month,
                                        year = krjak.fld_Tarikh.Value.Year
                                    });
                                    id++;
                                }
                            }
                            if (listactvtp.Count() > 0)
                            {
                                var getkrjak = getworkdetails.Where(x => listactvtp.Contains(x.fld_KodAktvt)).ToList();
                                foreach (var krjak in getkrjak)
                                {
                                    var CC = "";
                                    var leveldesc = "";
                                    var mainlevelcode = "";
                                    if (krjak.fld_JnsPkt == 1)
                                    {
                                        //fitri comment 14-01-2021
                                        //CC = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_IOcode).FirstOrDefault();
                                        //fitri add 14-01-2021
                                        CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                        leveldesc = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_NamaPktUtama.ToUpper()).FirstOrDefault();
                                        mainlevelcode = krjak.fld_KodPkt;
                                    }
                                    else
                                    {
                                        var getpktutm = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).FirstOrDefault();
                                        leveldesc = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).Select(s => s.fld_NamaPkt.ToUpper()).FirstOrDefault();
                                        //fitri comment 14-01-2021
                                        //CC = GetPktUtamas.Where(x => x.fld_PktUtama == getpktutm.fld_KodPktUtama).Select(s => s.fld_IOcode).FirstOrDefault();
                                        //fitri add 14-01-2021
                                        CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                        mainlevelcode = getpktutm.fld_KodPktUtama;
                                    }
                                    var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                                    var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                                    var actvtname = getallactvt.Where(x => x.fld_KodAktvt == krjak.fld_KodAktvt).Select(s => s.fld_Desc).FirstOrDefault();
                                    DetailWork.Add(new CustMod_DetailWork2
                                    {
                                        id = id,
                                        amounttype = 4,
                                        amount = krjak.fld_Amount.Value,
                                        quantity = krjak.fld_JumlahHasil.Value,
                                        wrk = krjak.fld_Nopkj,
                                        wrkname = pkjname,
                                        actvt = krjak.fld_KodAktvt,
                                        actvtdesc = actvtname,
                                        cc = CC,
                                        gl = krjak.fld_GLKod,
                                        grp = krjak.fld_Kum,
                                        pkt = mainlevelcode,
                                        fld_LadangID = krjak.fld_LadangID.Value,
                                        fld_WilayahID = krjak.fld_WilayahID.Value,
                                        fld_SyarikatID = krjak.fld_SyarikatID.Value,
                                        fld_NegaraID = krjak.fld_NegaraID.Value,
                                        active = actstatus,
                                        month = krjak.fld_Tarikh.Value.Month,
                                        year = krjak.fld_Tarikh.Value.Year
                                    });
                                    id++;
                                }
                            }
                        }
                        else if (EnbleAktvt && actvt.Count() == 1)
                        {
                            var getkrjak = getworkdetails.Where(x => actvt.Contains(x.fld_KodAktvt)).ToList();
                            foreach (var krjak in getkrjak)
                            {
                                var CC = "";
                                var leveldesc = "";
                                var mainlevelcode = "";
                                if (krjak.fld_JnsPkt == 1)
                                {
                                    //fitri comment 14-01-2021
                                    //CC = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_IOcode).FirstOrDefault();
                                    //fitri add 14-01-2021
                                    CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                    leveldesc = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_NamaPktUtama.ToUpper()).FirstOrDefault();
                                    mainlevelcode = krjak.fld_KodPkt;
                                }
                                else
                                {
                                    var getpktutm = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).FirstOrDefault();
                                    leveldesc = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).Select(s => s.fld_NamaPkt.ToUpper()).FirstOrDefault();
                                    //fitri comment 14-01-2021
                                    //CC = GetPktUtamas.Where(x => x.fld_PktUtama == getpktutm.fld_KodPktUtama).Select(s => s.fld_IOcode).FirstOrDefault();
                                    //fitri add 14-01-2021
                                    CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                    mainlevelcode = getpktutm.fld_KodPktUtama;
                                }
                                var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                                var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                                var actvtname = getallactvt.Where(x => x.fld_KodAktvt == krjak.fld_KodAktvt).Select(s => s.fld_Desc).FirstOrDefault();
                                DetailWork.Add(new CustMod_DetailWork2
                                {
                                    id = id,
                                    amounttype = 1,
                                    amount = krjak.fld_Amount.Value,
                                    quantity = krjak.fld_JumlahHasil.Value,
                                    wrk = krjak.fld_Nopkj,
                                    wrkname = pkjname,
                                    actvt = krjak.fld_KodAktvt,
                                    actvtdesc = actvtname,
                                    cc = CC,
                                    gl = krjak.fld_GLKod,
                                    grp = krjak.fld_Kum,
                                    pkt = mainlevelcode,
                                    fld_LadangID = krjak.fld_LadangID.Value,
                                    fld_WilayahID = krjak.fld_WilayahID.Value,
                                    fld_SyarikatID = krjak.fld_SyarikatID.Value,
                                    fld_NegaraID = krjak.fld_NegaraID.Value,
                                    active = actstatus,
                                    month = krjak.fld_Tarikh.Value.Month,
                                    year = krjak.fld_Tarikh.Value.Year
                                });
                                id++;
                            }
                        }
                        //bool Selection21 = KONG , bool Selection22 = OT, bool Selection23 = OTHERS, bool Selection24 = PIECE RATE, bool Selection25 = INCENTIVE, bool Selection26 = DEDUCTION
                        //amounttype KONG = 1, OT = 2, OTHERS = 3, PIECE RATE = 4, INCENTIVE = 5, DEDUCTION = 6
                        //OT
                        if (Selection22)
                        {
                            var getot = getworkdetails.Where(x => x.fld_JumlahOT > 0).ToList();
                            foreach (var ot in getot)
                            {
                                var CC = "";
                                var leveldesc = "";
                                var mainlevelcode = "";
                                if (ot.fld_JnsPkt == 1)
                                {
                                    //fitri comment 14-01-2021
                                    //CC = GetPktUtamas.Where(x => x.fld_PktUtama == ot.fld_KodPkt).Select(s => s.fld_IOcode).FirstOrDefault();
                                    //fitri add 14-01-2021
                                    CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == ot.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                    leveldesc = GetPktUtamas.Where(x => x.fld_PktUtama == ot.fld_KodPkt).Select(s => s.fld_NamaPktUtama.ToUpper()).FirstOrDefault();
                                    mainlevelcode = ot.fld_KodPkt;
                                }
                                else
                                {
                                    var getpktutm = GetPktSubs.Where(x => x.fld_Pkt == ot.fld_KodPkt).FirstOrDefault();
                                    leveldesc = GetPktSubs.Where(x => x.fld_Pkt == ot.fld_KodPkt).Select(s => s.fld_NamaPkt.ToUpper()).FirstOrDefault();
                                    //fitri comment 14-01-2021
                                    //CC = GetPktUtamas.Where(x => x.fld_PktUtama == getpktutm.fld_KodPktUtama).Select(s => s.fld_IOcode).FirstOrDefault();
                                    //fitri add 14-01-2021
                                    CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == ot.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                    mainlevelcode = getpktutm.fld_KodPktUtama;
                                }
                                var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == ot.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                                var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == ot.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                                var actvtname = getallactvt.Where(x => x.fld_KodAktvt == ot.fld_KodAktvt).Select(s => s.fld_Desc).FirstOrDefault();
                                DetailWork.Add(new CustMod_DetailWork2
                                {
                                    id = id,
                                    amounttype = 2,
                                    amount = ot.fld_JumlahOT.Value,
                                    quantity = ot.fld_JamOT.Value,
                                    wrk = ot.fld_Nopkj,
                                    wrkname = pkjname,
                                    actvt = ot.fld_KodAktvt,
                                    actvtdesc = actvtname,
                                    cc = CC,
                                    gl = ot.fld_GLKod,
                                    grp = ot.fld_Kum,
                                    pkt = mainlevelcode,
                                    fld_LadangID = ot.fld_LadangID.Value,
                                    fld_WilayahID = ot.fld_WilayahID.Value,
                                    fld_SyarikatID = ot.fld_SyarikatID.Value,
                                    fld_NegaraID = ot.fld_NegaraID.Value,
                                    active = actstatus,
                                    month = ot.fld_Tarikh.Value.Month,
                                    year = ot.fld_Tarikh.Value.Year
                                });
                                id++;
                            }
                        }
                        //OT
                        //bool Selection21 = KONG , bool Selection22 = OT, bool Selection23 = OTHERS, bool Selection24 = PIECE RATE, bool Selection25 = INCENTIVE, bool Selection26 = DEDUCTION
                        //amounttype KONG = 1, OT = 2, OTHERS = 3, PIECE RATE = 4, INCENTIVE = 5, DEDUCTION = 6
                        //OT

                        //Comment by fitri 14-01-2021
                        //if (Selection25)
                        //{
                        //    var getinctive = getworkdetails.Where(x => x.fld_Bonus > 0).ToList();
                        //    foreach (var incentive in getinctive)
                        //    {
                        //        var CC = "";
                        //        var leveldesc = "";
                        //        var mainlevelcode = "";
                        //        if (incentive.fld_JnsPkt == 1)
                        //        {
                        //            //fitri comment 14-01-2021
                        //            //CC = GetPktUtamas.Where(x => x.fld_PktUtama == incentive.fld_KodPkt).Select(s => s.fld_IOcode).FirstOrDefault();
                        //            //fitri add 14-01-2021
                        //            CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == incentive.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                        //            leveldesc = GetPktUtamas.Where(x => x.fld_PktUtama == incentive.fld_KodPkt).Select(s => s.fld_NamaPktUtama.ToUpper()).FirstOrDefault();
                        //            mainlevelcode = incentive.fld_KodPkt;
                        //        }
                        //        else
                        //        {
                        //            var getpktutm = GetPktSubs.Where(x => x.fld_Pkt == incentive.fld_KodPkt).FirstOrDefault();
                        //            leveldesc = GetPktSubs.Where(x => x.fld_Pkt == incentive.fld_KodPkt).Select(s => s.fld_NamaPkt.ToUpper()).FirstOrDefault();
                        //            //fitri comment 14-01-2021
                        //            //CC = GetPktUtamas.Where(x => x.fld_PktUtama == getpktutm.fld_KodPktUtama).Select(s => s.fld_IOcode).FirstOrDefault();
                        //            //fitri add 14-01-2021
                        //            CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == incentive.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                        //            mainlevelcode = getpktutm.fld_KodPktUtama;
                        //        }
                        //        var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == incentive.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                        //        var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == incentive.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                        //        var actvtname = getallactvt.Where(x => x.fld_KodAktvt == incentive.fld_KodAktvt).Select(s => s.fld_Desc).FirstOrDefault();
                        //        DetailWork.Add(new CustMod_DetailWork2
                        //        {
                        //            id = id,
                        //            amounttype = 5,
                        //            amount = incentive.fld_JumlahBonus.Value,
                        //            quantity = 1,
                        //            wrk = incentive.fld_Nopkj,
                        //            wrkname = pkjname,
                        //            actvt = incentive.fld_KodAktvt,
                        //            actvtdesc = actvtname,
                        //            cc = CC,
                        //            gl = incentive.fld_GLKod,
                        //            grp = incentive.fld_Kum,
                        //            pkt = mainlevelcode,
                        //            fld_LadangID = incentive.fld_LadangID.Value,
                        //            fld_WilayahID = incentive.fld_WilayahID.Value,
                        //            fld_SyarikatID = incentive.fld_SyarikatID.Value,
                        //            fld_NegaraID = incentive.fld_NegaraID.Value,
                        //            active = actstatus,
                        //            month = incentive.fld_Tarikh.Value.Month,
                        //            year = incentive.fld_Tarikh.Value.Year
                        //        });
                        //        id++;
                        //    }
                        //}
                        //OT
                    }
                    //get kerja
                    //bool Selection21 = KONG , bool Selection22 = OT, bool Selection23 = OTHERS, bool Selection24 = PIECE RATE, bool Selection25 = INCENTIVE, bool Selection26 = DEDUCTION
                    //amounttype KONG = 1, OT = 2, OTHERS = 3, PIECE RATE = 4, INCENTIVE = 5, DEDUCTION = 6

                    //Leave
                    var GetLeaveGL = db.tbl_CustomerVendorGLMap.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).ToList();
                    if (Selection23)
                    {
                        var GetActCodeLeave = db.tbl_CutiKategori.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).ToList();
                        var GetWorkercuti = dbr.vw_Kerja_Hdr_Cuti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month >= MonthList && x.fld_Tarikh.Value.Month <= MonthList2 && x.fld_Tarikh.Value.Year == YearList).OrderBy(o => new { o.fld_Tarikh, o.fld_Nopkj }).ToList();
                        foreach (var Workercuti in GetWorkercuti)
                        {
                            var CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == Workercuti.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                            var Krytn = tbl_Pkjmast.Where(x => x.fld_Nopkj == Workercuti.fld_Nopkj).Select(s => s.fld_Kdrkyt).FirstOrDefault();
                            var getworkdetail = tbl_Pkjmast.Where(x => x.fld_Nopkj == Workercuti.fld_Nopkj).FirstOrDefault();
                            var ActvtCodeDesc = GetActCodeLeave.Where(x => x.fld_KodCuti == Workercuti.fld_Kdhdct).Select(s => new { s.fld_KodAktvt, s.fld_KeteranganCuti }).FirstOrDefault();
                            string GL = "";
                            if (Krytn == "MA")
                            {
                                GL = GetLeaveGL.Where(x => x.fld_KodAktiviti == ActvtCodeDesc.fld_KodAktvt && x.fld_TypeCode == "GLTKT" && x.fld_Flag == "2").Select(s => s.fld_SAPCode).FirstOrDefault();
                            }
                            else
                            {
                                GL = GetLeaveGL.Where(x => x.fld_KodAktiviti == ActvtCodeDesc.fld_KodAktvt && x.fld_TypeCode == "GLTKA" && x.fld_Flag == "2").Select(s => s.fld_SAPCode).FirstOrDefault();
                            }
                            var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == Workercuti.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                            var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == Workercuti.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                            DetailWork.Add(new CustMod_DetailWork2
                            {
                                id = id,
                                amounttype = 3,
                                amount = Workercuti.fld_Jumlah.Value,
                                quantity = 1,
                                wrk = Workercuti.fld_Nopkj,
                                wrkname = pkjname,
                                actvt = ActvtCodeDesc.fld_KodAktvt,
                                actvtdesc = ActvtCodeDesc.fld_KeteranganCuti,
                                cc = CC,
                                gl = GL,
                                grp = Workercuti.fld_Kum,
                                pkt = null,
                                fld_LadangID = Workercuti.fld_LadangID.Value,
                                fld_WilayahID = Workercuti.fld_WilayahID.Value,
                                fld_SyarikatID = Workercuti.fld_SyarikatID.Value,
                                fld_NegaraID = Workercuti.fld_NegaraID.Value,
                                active = actstatus,
                                month = Workercuti.fld_Tarikh.Value.Month,
                                year = Workercuti.fld_Tarikh.Value.Year
                            });
                            id++;
                        }
                    }
                    //Leave

                    //bool Selection21 = KONG , bool Selection22 = OT, bool Selection23 = OTHERS, bool Selection24 = PIECE RATE, bool Selection25 = INCENTIVE, bool Selection26 = DEDUCTION
                    //amounttype KONG = 1, OT = 2, OTHERS = 3, PIECE RATE = 4, INCENTIVE = 5, DEDUCTION = 6

                    //Incentive
                    if (Selection25)
                    {
                        var GetIncetive = db.tbl_JenisInsentif.Where(x => x.fld_JenisInsentif == "P").ToList();
                        var GetIncetiveCode = GetIncetive.Select(s => s.fld_KodInsentif).ToList();
                        var GetWorkerIncntve = dbr.tbl_Insentif.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month >= MonthList && x.fld_Month <= MonthList2 && x.fld_Year == YearList && x.fld_Deleted == false && GetIncetiveCode.Contains(x.fld_KodInsentif) && x.fld_NilaiInsentif > 0).OrderBy(o => new { o.fld_Month, o.fld_Nopkj }).ToList();
                        var GetGroups = dbr.tbl_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
                        foreach (var WorkerIncntve in GetWorkerIncntve)
                        {
                            var CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == WorkerIncntve.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                            var Krytn = tbl_Pkjmast.Where(x => x.fld_Nopkj == WorkerIncntve.fld_Nopkj).Select(s => s.fld_Kdrkyt).FirstOrDefault();
                            var getworkdetail = tbl_Pkjmast.Where(x => x.fld_Nopkj == WorkerIncntve.fld_Nopkj).FirstOrDefault();
                            var ActvtCodeDesc = GetIncetive.Where(x => x.fld_KodInsentif == WorkerIncntve.fld_KodInsentif).Select(s => new { s.fld_KodAktvt, s.fld_Keterangan }).FirstOrDefault();
                            var Grp = GetGroups.Where(x => x.fld_KumpulanID == getworkdetail.fld_KumpulanID).Select(s => s.fld_KodKumpulan).FirstOrDefault() == null ? "" : GetGroups.Where(x => x.fld_KumpulanID == getworkdetail.fld_KumpulanID).Select(s => s.fld_KodKumpulan).FirstOrDefault();
                            string GL = "";
                            if (Krytn == "MA")
                            {
                                GL = GetLeaveGL.Where(x => x.fld_KodAktiviti == ActvtCodeDesc.fld_KodAktvt && x.fld_TypeCode == "GLTKT" && x.fld_Flag == "5").Select(s => s.fld_SAPCode).FirstOrDefault();
                            }
                            else
                            {
                                GL = GetLeaveGL.Where(x => x.fld_KodAktiviti == ActvtCodeDesc.fld_KodAktvt && x.fld_TypeCode == "GLTKA" && x.fld_Flag == "5").Select(s => s.fld_SAPCode).FirstOrDefault();
                            }
                            var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == WorkerIncntve.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                            var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == WorkerIncntve.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                            DetailWork.Add(new CustMod_DetailWork2
                            {
                                id = id,
                                amounttype = 5,
                                amount = WorkerIncntve.fld_NilaiInsentif.Value,
                                quantity = 1,
                                wrk = WorkerIncntve.fld_Nopkj,
                                wrkname = pkjname,
                                actvt = ActvtCodeDesc.fld_KodAktvt,
                                actvtdesc = ActvtCodeDesc.fld_Keterangan,
                                cc = CC,
                                gl = GL,
                                grp = Grp,
                                pkt = null,
                                fld_LadangID = WorkerIncntve.fld_LadangID.Value,
                                fld_WilayahID = WorkerIncntve.fld_WilayahID.Value,
                                fld_SyarikatID = WorkerIncntve.fld_SyarikatID.Value,
                                fld_NegaraID = WorkerIncntve.fld_NegaraID.Value,
                                active = actstatus,
                                month = WorkerIncntve.fld_Month.Value,
                                year = WorkerIncntve.fld_Year.Value
                            });
                            id++;
                        }
                    }
                    //Incentive

                    //bool Selection21 = KONG , bool Selection22 = OT, bool Selection23 = OTHERS, bool Selection24 = PIECE RATE, bool Selection25 = INCENTIVE, bool Selection26 = DEDUCTION
                    //amounttype KONG = 1, OT = 2, OTHERS = 3, PIECE RATE = 4, INCENTIVE = 5, DEDUCTION = 6

                    //Deduction
                    if (Selection26)
                    {
                        var GetIncetive = db.tbl_JenisInsentif.Where(x => x.fld_JenisInsentif == "T").ToList();
                        var GetIncetiveCode = GetIncetive.Select(s => s.fld_KodInsentif).ToList();
                        var GetWorkerIncntve = dbr.tbl_Insentif.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month >= MonthList && x.fld_Month <= MonthList2 && x.fld_Year == YearList && x.fld_Deleted == false && GetIncetiveCode.Contains(x.fld_KodInsentif) && x.fld_NilaiInsentif > 0).OrderBy(o => new { o.fld_Month, o.fld_Nopkj }).ToList();
                        var GetGroups = dbr.tbl_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
                        foreach (var WorkerIncntve in GetWorkerIncntve)
                        {
                            var CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == WorkerIncntve.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                            var Krytn = tbl_Pkjmast.Where(x => x.fld_Nopkj == WorkerIncntve.fld_Nopkj).Select(s => s.fld_Kdrkyt).FirstOrDefault();
                            var getworkdetail = tbl_Pkjmast.Where(x => x.fld_Nopkj == WorkerIncntve.fld_Nopkj).FirstOrDefault();
                            var ActvtCodeDesc = GetIncetive.Where(x => x.fld_KodInsentif == WorkerIncntve.fld_KodInsentif).Select(s => new { s.fld_KodAktvt, s.fld_Keterangan }).FirstOrDefault();
                            var Grp = GetGroups.Where(x => x.fld_KumpulanID == getworkdetail.fld_KumpulanID).Select(s => s.fld_KodKumpulan).FirstOrDefault() == null ? "" : GetGroups.Where(x => x.fld_KumpulanID == getworkdetail.fld_KumpulanID).Select(s => s.fld_KodKumpulan).FirstOrDefault();
                            string GL = "";
                            if (Krytn == "MA")
                            {
                                GL = GetLeaveGL.Where(x => x.fld_KodAktiviti == ActvtCodeDesc.fld_KodAktvt && x.fld_TypeCode == "GLTKT" && x.fld_Flag == "5").Select(s => s.fld_SAPCode).FirstOrDefault();
                            }
                            else
                            {
                                GL = GetLeaveGL.Where(x => x.fld_KodAktiviti == ActvtCodeDesc.fld_KodAktvt && x.fld_TypeCode == "GLTKA" && x.fld_Flag == "5").Select(s => s.fld_SAPCode).FirstOrDefault();
                            }
                            var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == WorkerIncntve.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                            var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == WorkerIncntve.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                            DetailWork.Add(new CustMod_DetailWork2
                            {
                                id = id,
                                amounttype = 6,
                                amount = -WorkerIncntve.fld_NilaiInsentif.Value,
                                quantity = 1,
                                wrk = WorkerIncntve.fld_Nopkj,
                                wrkname = pkjname,
                                actvt = ActvtCodeDesc.fld_KodAktvt,
                                actvtdesc = ActvtCodeDesc.fld_Keterangan,
                                cc = CC,
                                gl = GL,
                                grp = Grp,
                                pkt = null,
                                fld_LadangID = WorkerIncntve.fld_LadangID.Value,
                                fld_WilayahID = WorkerIncntve.fld_WilayahID.Value,
                                fld_SyarikatID = WorkerIncntve.fld_SyarikatID.Value,
                                fld_NegaraID = WorkerIncntve.fld_NegaraID.Value,
                                active = actstatus,
                                month = WorkerIncntve.fld_Month.Value,
                                year = WorkerIncntve.fld_Year.Value
                            });
                            id++;
                        }
                    }
                    //Deduction

                }
                else
                {
                    var getpiece = getallactvt.Where(x => x.fld_DisabledFlag != 3).Select(s => s.fld_KodAktvt).Distinct().ToList();
                    var getworkdetails = dbr.vw_KerjaInfoDetails2.Where(x => getpiece.Contains(x.fld_KodAktvt) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month >= MonthList && x.fld_Tarikh.Value.Month <= MonthList2 && x.fld_Tarikh.Value.Year == YearList && wrk.Contains(x.fld_Nopkj)).ToList();
                    if ((listactvtk.Count() > 0 || listactvtp.Count() > 0) && !EnbleAktvt)
                    {
                        if (listactvtk.Count() > 0)
                        {
                            var getkrjak = getworkdetails.Where(x => listactvtk.Contains(x.fld_KodAktvt)).ToList();
                            foreach (var krjak in getkrjak)
                            {
                                var CC = "";
                                var leveldesc = "";
                                var mainlevelcode = "";
                                if (krjak.fld_JnsPkt == 1)
                                {
                                    //fitri comment 14-01-2021
                                    //CC = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_IOcode).FirstOrDefault();
                                    //fitri add 14-01-2021
                                    CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                    leveldesc = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_NamaPktUtama.ToUpper()).FirstOrDefault();
                                    mainlevelcode = krjak.fld_KodPkt;
                                }
                                else
                                {
                                    var getpktutm = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).FirstOrDefault();
                                    leveldesc = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).Select(s => s.fld_NamaPkt.ToUpper()).FirstOrDefault();
                                    //fitri comment 14-01-2021
                                    //CC = GetPktUtamas.Where(x => x.fld_PktUtama == getpktutm.fld_KodPktUtama).Select(s => s.fld_IOcode).FirstOrDefault();
                                    //fitri add 14-01-2021
                                    CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                    mainlevelcode = getpktutm.fld_KodPktUtama;
                                }
                                var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                                var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                                var actvtname = getallactvt.Where(x => x.fld_KodAktvt == krjak.fld_KodAktvt).Select(s => s.fld_Desc).FirstOrDefault();
                                DetailWork.Add(new CustMod_DetailWork2
                                {
                                    id = id,
                                    amounttype = 1,
                                    amount = 0,
                                    quantity = krjak.fld_JumlahHasil.Value,
                                    wrk = krjak.fld_Nopkj,
                                    wrkname = krjak.fld_Nama,
                                    actvt = krjak.fld_KodAktvt,
                                    actvtdesc = actvtname,
                                    cc = CC,
                                    gl = krjak.fld_GLKod,
                                    grp = krjak.fld_Kum,
                                    pkt = mainlevelcode,
                                    fld_LadangID = krjak.fld_LadangID.Value,
                                    fld_WilayahID = krjak.fld_WilayahID.Value,
                                    fld_SyarikatID = krjak.fld_SyarikatID.Value,
                                    fld_NegaraID = krjak.fld_NegaraID.Value,
                                    active = actstatus,
                                    month = krjak.fld_Tarikh.Value.Month,
                                    year = krjak.fld_Tarikh.Value.Year
                                });
                                id++;
                            }
                        }
                        if (listactvtp.Count() > 0)
                        {
                            var getkrjak = getworkdetails.Where(x => listactvtp.Contains(x.fld_KodAktvt)).ToList();
                            foreach (var krjak in getkrjak)
                            {
                                var CC = "";
                                var leveldesc = "";
                                var mainlevelcode = "";
                                if (krjak.fld_JnsPkt == 1)
                                {
                                    //fitri comment 14-01-2021
                                    //CC = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_IOcode).FirstOrDefault();
                                    //fitri add 14-01-2021
                                    CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                    leveldesc = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_NamaPktUtama.ToUpper()).FirstOrDefault();
                                    mainlevelcode = krjak.fld_KodPkt;
                                }
                                else
                                {
                                    var getpktutm = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).FirstOrDefault();
                                    leveldesc = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).Select(s => s.fld_NamaPkt.ToUpper()).FirstOrDefault();
                                    //fitri comment 14-01-2021
                                    //CC = GetPktUtamas.Where(x => x.fld_PktUtama == getpktutm.fld_KodPktUtama).Select(s => s.fld_IOcode).FirstOrDefault();
                                    //fitri add 14-01-2021
                                    CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                    mainlevelcode = getpktutm.fld_KodPktUtama;
                                }
                                var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                                var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                                var actvtname = getallactvt.Where(x => x.fld_KodAktvt == krjak.fld_KodAktvt).Select(s => s.fld_Desc).FirstOrDefault();
                                DetailWork.Add(new CustMod_DetailWork2
                                {
                                    id = id,
                                    amounttype = 4,
                                    amount = 0,
                                    quantity = krjak.fld_JumlahHasil.Value,
                                    wrk = krjak.fld_Nopkj,
                                    wrkname = pkjname,
                                    actvt = krjak.fld_KodAktvt,
                                    actvtdesc = actvtname,
                                    cc = CC,
                                    gl = krjak.fld_GLKod,
                                    grp = krjak.fld_Kum,
                                    pkt = mainlevelcode,
                                    fld_LadangID = krjak.fld_LadangID.Value,
                                    fld_WilayahID = krjak.fld_WilayahID.Value,
                                    fld_SyarikatID = krjak.fld_SyarikatID.Value,
                                    fld_NegaraID = krjak.fld_NegaraID.Value,
                                    active = actstatus,
                                    month = krjak.fld_Tarikh.Value.Month,
                                    year = krjak.fld_Tarikh.Value.Year
                                });
                                id++;
                            }
                        }
                    }
                    else if ((listactvtk.Count() > 0 || listactvtp.Count() > 0) && EnbleAktvt && PilihanAktvt == "0")
                    {
                        if (listactvtk.Count() > 0)
                        {
                            var getkrjak = getworkdetails.Where(x => listactvtk.Contains(x.fld_KodAktvt)).ToList();
                            foreach (var krjak in getkrjak)
                            {
                                var CC = "";
                                var leveldesc = "";
                                var mainlevelcode = "";
                                if (krjak.fld_JnsPkt == 1)
                                {
                                    //fitri comment 14-01-2021
                                    //CC = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_IOcode).FirstOrDefault();
                                    //fitri add 14-01-2021
                                    CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                    leveldesc = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_NamaPktUtama.ToUpper()).FirstOrDefault();
                                    mainlevelcode = krjak.fld_KodPkt;
                                }
                                else
                                {
                                    var getpktutm = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).FirstOrDefault();
                                    leveldesc = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).Select(s => s.fld_NamaPkt.ToUpper()).FirstOrDefault();
                                    //fitri comment 14-01-2021
                                    //CC = GetPktUtamas.Where(x => x.fld_PktUtama == getpktutm.fld_KodPktUtama).Select(s => s.fld_IOcode).FirstOrDefault();
                                    //fitri add 14-01-2021
                                    CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                    mainlevelcode = getpktutm.fld_KodPktUtama;
                                }
                                var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                                var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                                var actvtname = getallactvt.Where(x => x.fld_KodAktvt == krjak.fld_KodAktvt).Select(s => s.fld_Desc).FirstOrDefault();
                                DetailWork.Add(new CustMod_DetailWork2
                                {
                                    id = id,
                                    amounttype = 1,
                                    amount = 0,
                                    quantity = krjak.fld_JumlahHasil.Value,
                                    wrk = krjak.fld_Nopkj,
                                    wrkname = pkjname,
                                    actvt = krjak.fld_KodAktvt,
                                    actvtdesc = actvtname,
                                    cc = CC,
                                    gl = krjak.fld_GLKod,
                                    grp = krjak.fld_Kum,
                                    pkt = mainlevelcode,
                                    fld_LadangID = krjak.fld_LadangID.Value,
                                    fld_WilayahID = krjak.fld_WilayahID.Value,
                                    fld_SyarikatID = krjak.fld_SyarikatID.Value,
                                    fld_NegaraID = krjak.fld_NegaraID.Value,
                                    active = actstatus,
                                    month = krjak.fld_Tarikh.Value.Month,
                                    year = krjak.fld_Tarikh.Value.Year
                                });
                                id++;
                            }
                        }
                        if (listactvtp.Count() > 0)
                        {
                            var getkrjak = getworkdetails.Where(x => listactvtp.Contains(x.fld_KodAktvt)).ToList();
                            foreach (var krjak in getkrjak)
                            {
                                var CC = "";
                                var leveldesc = "";
                                var mainlevelcode = "";
                                if (krjak.fld_JnsPkt == 1)
                                {
                                    //fitri comment 14-01-2021
                                    //CC = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_IOcode).FirstOrDefault();
                                    //fitri add 14-01-2021
                                    CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                    leveldesc = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_NamaPktUtama.ToUpper()).FirstOrDefault();
                                    mainlevelcode = krjak.fld_KodPkt;
                                }
                                else
                                {
                                    var getpktutm = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).FirstOrDefault();
                                    leveldesc = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).Select(s => s.fld_NamaPkt.ToUpper()).FirstOrDefault();
                                    //fitri comment 14-01-2021
                                    //CC = GetPktUtamas.Where(x => x.fld_PktUtama == getpktutm.fld_KodPktUtama).Select(s => s.fld_IOcode).FirstOrDefault();
                                    //fitri add 14-01-2021
                                    CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                    mainlevelcode = getpktutm.fld_KodPktUtama;
                                }
                                var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                                var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                                var actvtname = getallactvt.Where(x => x.fld_KodAktvt == krjak.fld_KodAktvt).Select(s => s.fld_Desc).FirstOrDefault();
                                DetailWork.Add(new CustMod_DetailWork2
                                {
                                    id = id,
                                    amounttype = 4,
                                    amount = 0,
                                    quantity = krjak.fld_JumlahHasil.Value,
                                    wrk = krjak.fld_Nopkj,
                                    wrkname = pkjname,
                                    actvt = krjak.fld_KodAktvt,
                                    actvtdesc = actvtname,
                                    cc = CC,
                                    gl = krjak.fld_GLKod,
                                    grp = krjak.fld_Kum,
                                    pkt = mainlevelcode,
                                    fld_LadangID = krjak.fld_LadangID.Value,
                                    fld_WilayahID = krjak.fld_WilayahID.Value,
                                    fld_SyarikatID = krjak.fld_SyarikatID.Value,
                                    fld_NegaraID = krjak.fld_NegaraID.Value,
                                    active = actstatus,
                                    month = krjak.fld_Tarikh.Value.Month,
                                    year = krjak.fld_Tarikh.Value.Year
                                });
                                id++;
                            }
                        }
                    }
                    else if (EnbleAktvt && actvt.Count() == 1)
                    {
                        var getkrjak = getworkdetails.Where(x => actvt.Contains(x.fld_KodAktvt)).ToList();
                        foreach (var krjak in getkrjak)
                        {
                            var CC = "";
                            var leveldesc = "";
                            var mainlevelcode = "";
                            if (krjak.fld_JnsPkt == 1)
                            {
                                //fitri comment 14-01-2021
                                //CC = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_IOcode).FirstOrDefault();
                                //fitri add 14-01-2021
                                CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                leveldesc = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_NamaPktUtama.ToUpper()).FirstOrDefault();
                                mainlevelcode = krjak.fld_KodPkt;
                            }
                            else
                            {
                                var getpktutm = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).FirstOrDefault();
                                leveldesc = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).Select(s => s.fld_NamaPkt.ToUpper()).FirstOrDefault();
                                //fitri comment 14-01-2021
                                //CC = GetPktUtamas.Where(x => x.fld_PktUtama == getpktutm.fld_KodPktUtama).Select(s => s.fld_IOcode).FirstOrDefault();
                                //fitri add 14-01-2021
                                CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                                mainlevelcode = getpktutm.fld_KodPktUtama;
                            }
                            var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                            var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                            var actvtname = getallactvt.Where(x => x.fld_KodAktvt == krjak.fld_KodAktvt).Select(s => s.fld_Desc).FirstOrDefault();
                            DetailWork.Add(new CustMod_DetailWork2
                            {
                                id = id,
                                amounttype = 1,
                                amount = 0,
                                quantity = krjak.fld_JumlahHasil.Value,
                                wrk = krjak.fld_Nopkj,
                                wrkname = pkjname,
                                actvt = krjak.fld_KodAktvt,
                                actvtdesc = actvtname,
                                cc = CC,
                                gl = krjak.fld_GLKod,
                                grp = krjak.fld_Kum,
                                pkt = mainlevelcode,
                                fld_LadangID = krjak.fld_LadangID.Value,
                                fld_WilayahID = krjak.fld_WilayahID.Value,
                                fld_SyarikatID = krjak.fld_SyarikatID.Value,
                                fld_NegaraID = krjak.fld_NegaraID.Value,
                                active = actstatus,
                                month = krjak.fld_Tarikh.Value.Month,
                                year = krjak.fld_Tarikh.Value.Year
                            });
                            id++;
                        }
                    }
                }

                var Syarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).FirstOrDefault();
                var Ladang = db.tbl_Ladang.Where(x => x.fld_ID == LadangID).FirstOrDefault();

                ViewBag.NamaSyarikat = Syarikat.fld_NamaSyarikat;
                ViewBag.NoSyarikat = Syarikat.fld_NoSyarikat;
                ViewBag.NamaLadang = Ladang.fld_LdgName;
                ViewBag.Pilihan = Pilihan;
                ViewBag.Tempoh = MonthList + "/" + YearList + " - " + MonthList2 + "/" + YearList;
                ViewBag.TitleReport = "LAPORAN KERJA BULANAN";
                DetailWork = ReportLogic.CustMod_DetailWork2(DetailWork, cc, pkt, actvt, gl, grp, wrk); //DetailWork.Where(x => cc.Contains(x.cc) || pkt.Contains(x.pkt) || actvt.Contains(x.actvt) || gl.Contains(x.gl) || grp.Contains(x.grp) || wrk.Contains(x.wrk)).ToList();
                detailsreport = new detailsreport
                {
                    coloumgroup = coloumgroup,
                    CustMod_DetailWork2 = DetailWork
                };
            }
            ViewBag.Selection1 = Selection1;
            return View(detailsreport);
        }
        public ActionResult LprnKrjaHarian()
        {
            ViewBag.Report = "class = active";
            ViewBag.TitleReport = "LAPORAN KERJA HARIAN";
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

            List<SelectListItem> PilihanCC = new List<SelectListItem>();
            var getCC = db.tbl_SAPCCPUP.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
            PilihanCC = new SelectList(getCC.Select(s => new SelectListItem { Value = s.fld_CostCenter, Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc.ToUpper() }), "Value", "Text").ToList();
            var CC = getCC.Select(s => s.fld_CostCenter).Distinct().ToList();
            var getWBS = db.tbl_SAPPDPUP.Where(x => CC.Contains(x.fld_CostCenter) && x.fld_Deleted == false).Select(s => s.fld_WBSCode).Distinct().ToList();
            var z = PilihanCC.Count();
            foreach (var WBS in getWBS)
            {
                PilihanCC.Insert(z, (new SelectListItem { Text = WBS, Value = WBS }));
                z++;
            }
            PilihanCC.Insert(0, (new SelectListItem { Text = "ALL", Value = "0" }));
            List<SelectListItem> PilihanAktvt = new List<SelectListItem>();
            //comment by fitri 7-12-2021
            //var SelectAktvt = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).ToList();
            //PilihanAktvt = new SelectList(SelectAktvt.Select(s => new SelectListItem { Value = s.fld_KodAktvt, Text = s.fld_KodAktvt + " - " + s.fld_Desc }).Distinct().OrderBy(o => o.Value), "Value", "Text").ToList();
            //PilihanAktvt.Insert(0, (new SelectListItem { Text = "ALL", Value = "0" }));
            //add by fitri 7.12.2021
            var SelectAktvt = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_KodAktvt).GroupBy(g => new { g.fld_KodAktvt, g.fld_Desc }).ToList();
            PilihanAktvt = new SelectList(SelectAktvt.Select(s => new SelectListItem { Value = s.Key.fld_KodAktvt, Text = s.Key.fld_KodAktvt + " - " + s.Key.fld_Desc }), "Value", "Text").ToList();
            PilihanAktvt.Insert(0, (new SelectListItem { Text = "ALL", Value = "0" }));
            List<SelectListItem> PilihanPkt = new List<SelectListItem>();
            var SelectPkt = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
            PilihanPkt = new SelectList(SelectPkt.Select(s => new SelectListItem { Value = s.fld_PktUtama, Text = s.fld_PktUtama + " - " + s.fld_NamaPktUtama }), "Value", "Text").ToList();
            PilihanPkt.Insert(0, (new SelectListItem { Text = "ALL", Value = "0" }));
            List<SelectListItem> PilihanGL = new List<SelectListItem>();
            var SelectGL = db.tbl_MapGL.Join(db.tbl_SAPGLPUP, j => new { GL = j.fld_KodGL }, k => new { GL = k.fld_GLCode }, (j, k) => new { j.fld_ID, j.fld_KodGL, k.fld_GLDesc }).Select(s => new { s.fld_KodGL, s.fld_GLDesc }).Distinct().ToList();
            PilihanGL = new SelectList(SelectGL.Select(s => new SelectListItem { Value = s.fld_KodGL, Text = s.fld_KodGL + " - " + s.fld_GLDesc.ToUpper() }), "Value", "Text").ToList();
            PilihanGL.Insert(0, (new SelectListItem { Text = "ALL", Value = "0" }));
            List<SelectListItem> PilihanGrp = new List<SelectListItem>();
            var SelectGrp = dbr.tbl_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).ToList();
            PilihanGrp = new SelectList(SelectGrp.Select(s => new SelectListItem { Value = s.fld_KodKumpulan, Text = s.fld_KodKumpulan + " - " + s.fld_Keterangan.ToUpper() }), "Value", "Text").ToList();
            PilihanGrp.Insert(0, (new SelectListItem { Text = "ALL", Value = "0" }));
            List<SelectListItem> PilihanWrk = new List<SelectListItem>();
            //edit by fitri 11-11-2020 orderby fld_Nama
            var SelectWrk = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o => o.fld_Nama).ToList();
            PilihanWrk = new SelectList(SelectWrk.Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + " - " + s.fld_Nama.ToUpper() }), "Value", "Text").ToList();
            PilihanWrk.Insert(0, (new SelectListItem { Text = "ALL", Value = "0" }));
            List<SelectListItem> PilihanSts = new List<SelectListItem>();
            PilihanSts = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            PilihanSts.Insert(0, (new SelectListItem { Text = "ALL", Value = "0" }));
            ViewBag.PilihanCC = PilihanCC;
            ViewBag.PilihanSts = PilihanSts;
            ViewBag.PilihanWrk = PilihanWrk;
            ViewBag.PilihanGrp = PilihanGrp;
            ViewBag.PilihanGL = PilihanGL;
            ViewBag.PilihanPkt = PilihanPkt;
            ViewBag.PilihanAktvt = PilihanAktvt;
            ViewBag.MonthList = MonthList;
            ViewBag.MonthList2 = MonthList;
            ViewBag.YearList = yearlist;
            return View();
        }
        public ViewResult _LprnKrjaHarian(int? MonthList, int? MonthList2, int? YearList, string PilihanSts, string PilihanWrk, string PilihanGrp, string PilihanGL, string PilihanPkt, string PilihanCC, string PilihanAktvt, int Selection1, bool Selection21, bool Selection22, bool Selection23, bool Selection24, bool Selection25, bool Selection26, bool EnbleCC, bool EnblePkt, bool EnbleAktvt, bool EnbleGL, bool EnbleGrp, bool EnbleWrk)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            bool proceed = true;
            var DetailWork = new List<CustMod_DetailWork2>();
            var coloumgroup = new List<coloumgroup>();
            var detailsreport = new detailsreport();
            if (!EnbleCC && !EnblePkt && !EnbleAktvt && !EnbleGL && !EnbleGrp && !EnbleWrk)
            {
                proceed = false;
            }
            if (!Selection21 && !Selection22 && !Selection23 && !Selection24 && !Selection25 && !Selection26)
            {
                proceed = false;
            }
            if (proceed)
            {
                var tbl_Pkjmast = new List<Models.tbl_Pkjmast>();
                if (PilihanSts == "0")
                {
                    tbl_Pkjmast = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Distinct().ToList();
                }
                else if (PilihanSts == "1")
                {
                    tbl_Pkjmast = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1").Distinct().ToList();
                }
                else
                {
                    tbl_Pkjmast = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "2").Distinct().ToList();
                }
                List<string> cc = new List<string>();
                List<string> pkt = new List<string>();
                List<string> actvt = new List<string>();
                List<string> gl = new List<string>();
                List<string> grp = new List<string>();
                List<string> wrk = new List<string>();
                int columngrp = 0;
                var Pilihan = "";
                var getallactvt = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).ToList();
                if (EnbleCC)
                {
                    if (PilihanCC == "0")
                    {
                        var getcc = db.tbl_SAPCCPUP.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).Select(s => s.fld_CostCenter).Distinct().ToList();
                        cc.AddRange(getcc);
                        var getWBS = db.tbl_SAPPDPUP.Where(x => getcc.Contains(x.fld_CostCenter) && x.fld_Deleted == false).Select(s => s.fld_WBSCode).Distinct().ToList();
                        cc.AddRange(getWBS);
                    }
                    else
                    {
                        cc.Add(PilihanCC);
                    }
                    columngrp++;
                    coloumgroup.Add(new coloumgroup { id = columngrp, name = "Kos Pusat (Cost Centre)", type = 1 });
                    Pilihan += "Cost Center, ";
                }
                if (EnblePkt)
                {
                    if (PilihanPkt == "0")
                    {
                        var getpkt = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).Select(s => s.fld_PktUtama).Distinct().ToList();
                        pkt.AddRange(getpkt);
                    }
                    else
                    {
                        pkt.Add(PilihanPkt);
                    }
                    columngrp++;
                    coloumgroup.Add(new coloumgroup { id = columngrp, name = "Peringkat", type = 2 });
                    Pilihan += "Peringkat, ";
                }
                if (EnbleAktvt)
                {
                    if (PilihanAktvt == "0")
                    {
                        var getactvt2 = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_KodAktvt).Distinct().ToList();
                        actvt.AddRange(getactvt2);
                    }
                    else
                    {
                        actvt.Add(PilihanAktvt);
                    }
                    columngrp++;
                    coloumgroup.Add(new coloumgroup { id = columngrp, name = "Aktiviti", type = 3 });
                    Pilihan += "Aktiviti, ";
                }
                if (EnbleGL)
                {
                    if (PilihanGL == "0")
                    {
                        var getgl = db.tbl_MapGL.Join(db.tbl_SAPGLPUP, j => new { GL = j.fld_KodGL }, k => new { GL = k.fld_GLCode }, (j, k) => new { j.fld_ID, j.fld_KodGL, k.fld_GLDesc }).Select(s => s.fld_KodGL).Distinct().ToList();
                        gl.AddRange(getgl);
                    }
                    else
                    {
                        gl.Add(PilihanGL);
                    }
                    columngrp++;
                    coloumgroup.Add(new coloumgroup { id = columngrp, name = "Kod Akaun (GL SAP)", type = 4 });
                    Pilihan += "GL, ";
                }
                if (EnbleGrp)
                {
                    if (PilihanGrp == "0")
                    {
                        var getgrp = dbr.tbl_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).Select(s => s.fld_KodKumpulan).Distinct().ToList();
                        grp.AddRange(getgrp);
                    }
                    else
                    {
                        grp.Add(PilihanGrp);
                    }
                    columngrp++;
                    coloumgroup.Add(new coloumgroup { id = columngrp, name = "Kump", type = 5 });
                    Pilihan += "Kumpulan, ";
                }
                if (EnbleWrk)
                {
                    if (PilihanWrk == "0")
                    {
                        var getwrk = tbl_Pkjmast.Select(s => s.fld_Nopkj).Distinct().ToList();
                        wrk.AddRange(getwrk);
                    }
                    else
                    {
                        wrk.Add(PilihanWrk);
                    }
                    columngrp++;
                    coloumgroup.Add(new coloumgroup { id = columngrp, name = "Pekerja", type = 6 });
                    Pilihan += "Pekerja, ";
                }
                else
                {
                    var getwrk = tbl_Pkjmast.Select(s => s.fld_Nopkj).Distinct().ToList();
                    wrk.AddRange(getwrk);
                }
                //bool Selection21 = KONG , bool Selection22 = OT, bool Selection23 = OTHERS, bool Selection24 = PIECE RATE, bool Selection25 = INCENTIVE, bool Selection26 = DEDUCTION
                //amounttype KONG = 1, OT = 2, OTHERS = 3, PIECE RATE = 4, INCENTIVE = 5, DEDUCTION = 6
                //get kerja

                var proceedfordatakerja = false;
                var listactvtk = new List<string>();
                var listactvtp = new List<string>();
                if (Selection21 || Selection22 || Selection24 || Selection25)
                {
                    if (Selection21)
                    {
                        var getkong = getallactvt.Where(x => x.fld_DisabledFlag == 3).Select(s => s.fld_KodAktvt).Distinct().ToList();
                        listactvtk.AddRange(getkong);
                    }
                    if (Selection24)
                    {
                        var getpiece = getallactvt.Where(x => x.fld_DisabledFlag != 3).Select(s => s.fld_KodAktvt).Distinct().ToList();
                        listactvtp.AddRange(getpiece);
                    }
                    proceedfordatakerja = true;
                }
                int id = 1;
                var GetPktUtamas = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_IOcode).ToList();
                var GetPktSubs = dbr.tbl_SubPkt.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
                if (Selection1 == 0)
                {
                    if (proceedfordatakerja)
                    {
                        var getworkdetails = dbr.vw_KerjaInfoDetails2.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && wrk.Contains(x.fld_Nopkj)).ToList();
                        if ((listactvtk.Count() > 0 || listactvtp.Count() > 0) && !EnbleAktvt)
                        {
                            if (listactvtk.Count() > 0)
                            {
                                var getkrjak = getworkdetails.Where(x => listactvtk.Contains(x.fld_KodAktvt)).ToList();
                                foreach (var krjak in getkrjak)
                                {
                                    var CC = "";
                                    var leveldesc = "";
                                    var mainlevelcode = "";
                                    if (krjak.fld_JnsPkt == 1)
                                    {
                                        CC = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_IOcode).FirstOrDefault();
                                        leveldesc = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_NamaPktUtama.ToUpper()).FirstOrDefault();
                                        mainlevelcode = krjak.fld_KodPkt;
                                    }
                                    else
                                    {
                                        var getpktutm = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).FirstOrDefault();
                                        leveldesc = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).Select(s => s.fld_NamaPkt.ToUpper()).FirstOrDefault();
                                        CC = GetPktUtamas.Where(x => x.fld_PktUtama == getpktutm.fld_KodPktUtama).Select(s => s.fld_IOcode).FirstOrDefault();
                                        mainlevelcode = getpktutm.fld_KodPktUtama;
                                    }
                                    var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                                    var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                                    var actvtname = getallactvt.Where(x => x.fld_KodAktvt == krjak.fld_KodAktvt).Select(s => s.fld_Desc).FirstOrDefault();
                                    DetailWork.Add(new CustMod_DetailWork2
                                    {
                                        id = id,
                                        amounttype = 1,
                                        amount = krjak.fld_Amount.Value,
                                        quantity = krjak.fld_JumlahHasil.Value,
                                        wrk = krjak.fld_Nopkj,
                                        wrkname = krjak.fld_Nama,
                                        actvt = krjak.fld_KodAktvt,
                                        actvtdesc = actvtname,
                                        cc = CC,
                                        gl = krjak.fld_GLKod,
                                        grp = krjak.fld_Kum,
                                        pkt = mainlevelcode,
                                        fld_LadangID = krjak.fld_LadangID.Value,
                                        fld_WilayahID = krjak.fld_WilayahID.Value,
                                        fld_SyarikatID = krjak.fld_SyarikatID.Value,
                                        fld_NegaraID = krjak.fld_NegaraID.Value,
                                        active = actstatus,
                                        month = krjak.fld_Tarikh.Value.Month,
                                        year = krjak.fld_Tarikh.Value.Year,
                                        day = krjak.fld_Tarikh.Value.Day
                                    });
                                    id++;
                                }
                            }
                            if (listactvtp.Count() > 0)
                            {
                                var getkrjak = getworkdetails.Where(x => listactvtp.Contains(x.fld_KodAktvt)).ToList();
                                foreach (var krjak in getkrjak)
                                {
                                    var CC = "";
                                    var leveldesc = "";
                                    var mainlevelcode = "";
                                    if (krjak.fld_JnsPkt == 1)
                                    {
                                        CC = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_IOcode).FirstOrDefault();
                                        leveldesc = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_NamaPktUtama.ToUpper()).FirstOrDefault();
                                        mainlevelcode = krjak.fld_KodPkt;
                                    }
                                    else
                                    {
                                        var getpktutm = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).FirstOrDefault();
                                        leveldesc = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).Select(s => s.fld_NamaPkt.ToUpper()).FirstOrDefault();
                                        CC = GetPktUtamas.Where(x => x.fld_PktUtama == getpktutm.fld_KodPktUtama).Select(s => s.fld_IOcode).FirstOrDefault();
                                        mainlevelcode = getpktutm.fld_KodPktUtama;
                                    }
                                    var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                                    var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                                    var actvtname = getallactvt.Where(x => x.fld_KodAktvt == krjak.fld_KodAktvt).Select(s => s.fld_Desc).FirstOrDefault();
                                    DetailWork.Add(new CustMod_DetailWork2
                                    {
                                        id = id,
                                        amounttype = 4,
                                        amount = krjak.fld_Amount.Value,
                                        quantity = krjak.fld_JumlahHasil.Value,
                                        wrk = krjak.fld_Nopkj,
                                        wrkname = pkjname,
                                        actvt = krjak.fld_KodAktvt,
                                        actvtdesc = actvtname,
                                        cc = CC,
                                        gl = krjak.fld_GLKod,
                                        grp = krjak.fld_Kum,
                                        pkt = mainlevelcode,
                                        fld_LadangID = krjak.fld_LadangID.Value,
                                        fld_WilayahID = krjak.fld_WilayahID.Value,
                                        fld_SyarikatID = krjak.fld_SyarikatID.Value,
                                        fld_NegaraID = krjak.fld_NegaraID.Value,
                                        active = actstatus,
                                        month = krjak.fld_Tarikh.Value.Month,
                                        year = krjak.fld_Tarikh.Value.Year,
                                        day = krjak.fld_Tarikh.Value.Day
                                    });
                                    id++;
                                }
                            }
                        }
                        else if ((listactvtk.Count() > 0 || listactvtp.Count() > 0) && EnbleAktvt && PilihanAktvt == "0")
                        {
                            if (listactvtk.Count() > 0)
                            {
                                var getkrjak = getworkdetails.Where(x => listactvtk.Contains(x.fld_KodAktvt)).ToList();
                                foreach (var krjak in getkrjak)
                                {
                                    var CC = "";
                                    var leveldesc = "";
                                    var mainlevelcode = "";
                                    if (krjak.fld_JnsPkt == 1)
                                    {
                                        CC = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_IOcode).FirstOrDefault();
                                        leveldesc = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_NamaPktUtama.ToUpper()).FirstOrDefault();
                                        mainlevelcode = krjak.fld_KodPkt;
                                    }
                                    else
                                    {
                                        var getpktutm = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).FirstOrDefault();
                                        leveldesc = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).Select(s => s.fld_NamaPkt.ToUpper()).FirstOrDefault();
                                        CC = GetPktUtamas.Where(x => x.fld_PktUtama == getpktutm.fld_KodPktUtama).Select(s => s.fld_IOcode).FirstOrDefault();
                                        mainlevelcode = getpktutm.fld_KodPktUtama;
                                    }
                                    var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                                    var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                                    var actvtname = getallactvt.Where(x => x.fld_KodAktvt == krjak.fld_KodAktvt).Select(s => s.fld_Desc).FirstOrDefault();
                                    DetailWork.Add(new CustMod_DetailWork2
                                    {
                                        id = id,
                                        amounttype = 1,
                                        amount = krjak.fld_Amount.Value,
                                        quantity = krjak.fld_JumlahHasil.Value,
                                        wrk = krjak.fld_Nopkj,
                                        wrkname = pkjname,
                                        actvt = krjak.fld_KodAktvt,
                                        actvtdesc = actvtname,
                                        cc = CC,
                                        gl = krjak.fld_GLKod,
                                        grp = krjak.fld_Kum,
                                        pkt = mainlevelcode,
                                        fld_LadangID = krjak.fld_LadangID.Value,
                                        fld_WilayahID = krjak.fld_WilayahID.Value,
                                        fld_SyarikatID = krjak.fld_SyarikatID.Value,
                                        fld_NegaraID = krjak.fld_NegaraID.Value,
                                        active = actstatus,
                                        month = krjak.fld_Tarikh.Value.Month,
                                        year = krjak.fld_Tarikh.Value.Year,
                                        day = krjak.fld_Tarikh.Value.Day
                                    });
                                    id++;
                                }
                            }
                            if (listactvtp.Count() > 0)
                            {
                                var getkrjak = getworkdetails.Where(x => listactvtp.Contains(x.fld_KodAktvt)).ToList();
                                foreach (var krjak in getkrjak)
                                {
                                    var CC = "";
                                    var leveldesc = "";
                                    var mainlevelcode = "";
                                    if (krjak.fld_JnsPkt == 1)
                                    {
                                        CC = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_IOcode).FirstOrDefault();
                                        leveldesc = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_NamaPktUtama.ToUpper()).FirstOrDefault();
                                        mainlevelcode = krjak.fld_KodPkt;
                                    }
                                    else
                                    {
                                        var getpktutm = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).FirstOrDefault();
                                        leveldesc = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).Select(s => s.fld_NamaPkt.ToUpper()).FirstOrDefault();
                                        CC = GetPktUtamas.Where(x => x.fld_PktUtama == getpktutm.fld_KodPktUtama).Select(s => s.fld_IOcode).FirstOrDefault();
                                        mainlevelcode = getpktutm.fld_KodPktUtama;
                                    }
                                    var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                                    var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                                    var actvtname = getallactvt.Where(x => x.fld_KodAktvt == krjak.fld_KodAktvt).Select(s => s.fld_Desc).FirstOrDefault();
                                    DetailWork.Add(new CustMod_DetailWork2
                                    {
                                        id = id,
                                        amounttype = 4,
                                        amount = krjak.fld_Amount.Value,
                                        quantity = krjak.fld_JumlahHasil.Value,
                                        wrk = krjak.fld_Nopkj,
                                        wrkname = pkjname,
                                        actvt = krjak.fld_KodAktvt,
                                        actvtdesc = actvtname,
                                        cc = CC,
                                        gl = krjak.fld_GLKod,
                                        grp = krjak.fld_Kum,
                                        pkt = mainlevelcode,
                                        fld_LadangID = krjak.fld_LadangID.Value,
                                        fld_WilayahID = krjak.fld_WilayahID.Value,
                                        fld_SyarikatID = krjak.fld_SyarikatID.Value,
                                        fld_NegaraID = krjak.fld_NegaraID.Value,
                                        active = actstatus,
                                        month = krjak.fld_Tarikh.Value.Month,
                                        year = krjak.fld_Tarikh.Value.Year,
                                        day = krjak.fld_Tarikh.Value.Day
                                    });
                                    id++;
                                }
                            }
                        }
                        else if (EnbleAktvt && actvt.Count() == 1)
                        {
                            var getkrjak = getworkdetails.Where(x => actvt.Contains(x.fld_KodAktvt)).ToList();
                            foreach (var krjak in getkrjak)
                            {
                                var CC = "";
                                var leveldesc = "";
                                var mainlevelcode = "";
                                if (krjak.fld_JnsPkt == 1)
                                {
                                    CC = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_IOcode).FirstOrDefault();
                                    leveldesc = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_NamaPktUtama.ToUpper()).FirstOrDefault();
                                    mainlevelcode = krjak.fld_KodPkt;
                                }
                                else
                                {
                                    var getpktutm = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).FirstOrDefault();
                                    leveldesc = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).Select(s => s.fld_NamaPkt.ToUpper()).FirstOrDefault();
                                    CC = GetPktUtamas.Where(x => x.fld_PktUtama == getpktutm.fld_KodPktUtama).Select(s => s.fld_IOcode).FirstOrDefault();
                                    mainlevelcode = getpktutm.fld_KodPktUtama;
                                }
                                var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                                var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                                var actvtname = getallactvt.Where(x => x.fld_KodAktvt == krjak.fld_KodAktvt).Select(s => s.fld_Desc).FirstOrDefault();
                                DetailWork.Add(new CustMod_DetailWork2
                                {
                                    id = id,
                                    amounttype = 1,
                                    amount = krjak.fld_Amount.Value,
                                    quantity = krjak.fld_JumlahHasil.Value,
                                    wrk = krjak.fld_Nopkj,
                                    wrkname = pkjname,
                                    actvt = krjak.fld_KodAktvt,
                                    actvtdesc = actvtname,
                                    cc = CC,
                                    gl = krjak.fld_GLKod,
                                    grp = krjak.fld_Kum,
                                    pkt = mainlevelcode,
                                    fld_LadangID = krjak.fld_LadangID.Value,
                                    fld_WilayahID = krjak.fld_WilayahID.Value,
                                    fld_SyarikatID = krjak.fld_SyarikatID.Value,
                                    fld_NegaraID = krjak.fld_NegaraID.Value,
                                    active = actstatus,
                                    month = krjak.fld_Tarikh.Value.Month,
                                    year = krjak.fld_Tarikh.Value.Year,
                                    day = krjak.fld_Tarikh.Value.Day
                                });
                                id++;
                            }
                        }
                        //bool Selection21 = KONG , bool Selection22 = OT, bool Selection23 = OTHERS, bool Selection24 = PIECE RATE, bool Selection25 = INCENTIVE, bool Selection26 = DEDUCTION
                        //amounttype KONG = 1, OT = 2, OTHERS = 3, PIECE RATE = 4, INCENTIVE = 5, DEDUCTION = 6
                        //OT
                        if (Selection22)
                        {
                            var getot = getworkdetails.Where(x => x.fld_JumlahOT > 0).ToList();
                            foreach (var ot in getot)
                            {
                                var CC = "";
                                var leveldesc = "";
                                var mainlevelcode = "";
                                if (ot.fld_JnsPkt == 1)
                                {
                                    CC = GetPktUtamas.Where(x => x.fld_PktUtama == ot.fld_KodPkt).Select(s => s.fld_IOcode).FirstOrDefault();
                                    leveldesc = GetPktUtamas.Where(x => x.fld_PktUtama == ot.fld_KodPkt).Select(s => s.fld_NamaPktUtama.ToUpper()).FirstOrDefault();
                                    mainlevelcode = ot.fld_KodPkt;
                                }
                                else
                                {
                                    var getpktutm = GetPktSubs.Where(x => x.fld_Pkt == ot.fld_KodPkt).FirstOrDefault();
                                    leveldesc = GetPktSubs.Where(x => x.fld_Pkt == ot.fld_KodPkt).Select(s => s.fld_NamaPkt.ToUpper()).FirstOrDefault();
                                    CC = GetPktUtamas.Where(x => x.fld_PktUtama == getpktutm.fld_KodPktUtama).Select(s => s.fld_IOcode).FirstOrDefault();
                                    mainlevelcode = getpktutm.fld_KodPktUtama;
                                }
                                var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == ot.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                                var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == ot.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                                var actvtname = getallactvt.Where(x => x.fld_KodAktvt == ot.fld_KodAktvt).Select(s => s.fld_Desc).FirstOrDefault();
                                DetailWork.Add(new CustMod_DetailWork2
                                {
                                    id = id,
                                    amounttype = 2,
                                    amount = ot.fld_JumlahOT.Value,
                                    quantity = ot.fld_JamOT.Value,
                                    wrk = ot.fld_Nopkj,
                                    wrkname = pkjname,
                                    actvt = ot.fld_KodAktvt,
                                    actvtdesc = actvtname,
                                    cc = CC,
                                    gl = ot.fld_GLKod,
                                    grp = ot.fld_Kum,
                                    pkt = mainlevelcode,
                                    fld_LadangID = ot.fld_LadangID.Value,
                                    fld_WilayahID = ot.fld_WilayahID.Value,
                                    fld_SyarikatID = ot.fld_SyarikatID.Value,
                                    fld_NegaraID = ot.fld_NegaraID.Value,
                                    active = actstatus,
                                    month = ot.fld_Tarikh.Value.Month,
                                    year = ot.fld_Tarikh.Value.Year,
                                    day = ot.fld_Tarikh.Value.Day
                                });
                                id++;
                            }
                        }
                        //OT
                        //bool Selection21 = KONG , bool Selection22 = OT, bool Selection23 = OTHERS, bool Selection24 = PIECE RATE, bool Selection25 = INCENTIVE, bool Selection26 = DEDUCTION
                        //amounttype KONG = 1, OT = 2, OTHERS = 3, PIECE RATE = 4, INCENTIVE = 5, DEDUCTION = 6
                        //OT
                        if (Selection25)
                        {
                            var getinctive = getworkdetails.Where(x => x.fld_Bonus > 0).ToList();
                            foreach (var incentive in getinctive)
                            {
                                var CC = "";
                                var leveldesc = "";
                                var mainlevelcode = "";
                                if (incentive.fld_JnsPkt == 1)
                                {
                                    CC = GetPktUtamas.Where(x => x.fld_PktUtama == incentive.fld_KodPkt).Select(s => s.fld_IOcode).FirstOrDefault();
                                    leveldesc = GetPktUtamas.Where(x => x.fld_PktUtama == incentive.fld_KodPkt).Select(s => s.fld_NamaPktUtama.ToUpper()).FirstOrDefault();
                                    mainlevelcode = incentive.fld_KodPkt;
                                }
                                else
                                {
                                    var getpktutm = GetPktSubs.Where(x => x.fld_Pkt == incentive.fld_KodPkt).FirstOrDefault();
                                    leveldesc = GetPktSubs.Where(x => x.fld_Pkt == incentive.fld_KodPkt).Select(s => s.fld_NamaPkt.ToUpper()).FirstOrDefault();
                                    CC = GetPktUtamas.Where(x => x.fld_PktUtama == getpktutm.fld_KodPktUtama).Select(s => s.fld_IOcode).FirstOrDefault();
                                    mainlevelcode = getpktutm.fld_KodPktUtama;
                                }
                                var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == incentive.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                                var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == incentive.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                                var actvtname = getallactvt.Where(x => x.fld_KodAktvt == incentive.fld_KodAktvt).Select(s => s.fld_Desc).FirstOrDefault();
                                DetailWork.Add(new CustMod_DetailWork2
                                {
                                    id = id,
                                    amounttype = 5,
                                    amount = incentive.fld_JumlahBonus.Value,
                                    quantity = 1,
                                    wrk = incentive.fld_Nopkj,
                                    wrkname = pkjname,
                                    actvt = incentive.fld_KodAktvt,
                                    actvtdesc = actvtname,
                                    cc = CC,
                                    gl = incentive.fld_GLKod,
                                    grp = incentive.fld_Kum,
                                    pkt = mainlevelcode,
                                    fld_LadangID = incentive.fld_LadangID.Value,
                                    fld_WilayahID = incentive.fld_WilayahID.Value,
                                    fld_SyarikatID = incentive.fld_SyarikatID.Value,
                                    fld_NegaraID = incentive.fld_NegaraID.Value,
                                    active = actstatus,
                                    month = incentive.fld_Tarikh.Value.Month,
                                    year = incentive.fld_Tarikh.Value.Year,
                                    day = incentive.fld_Tarikh.Value.Day
                                });
                                id++;
                            }
                        }
                        //OT
                    }
                    //get kerja
                    //bool Selection21 = KONG , bool Selection22 = OT, bool Selection23 = OTHERS, bool Selection24 = PIECE RATE, bool Selection25 = INCENTIVE, bool Selection26 = DEDUCTION
                    //amounttype KONG = 1, OT = 2, OTHERS = 3, PIECE RATE = 4, INCENTIVE = 5, DEDUCTION = 6

                    //Leave
                    var GetLeaveGL = db.tbl_CustomerVendorGLMap.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).ToList();
                    if (Selection23)
                    {
                        var GetActCodeLeave = db.tbl_CutiKategori.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).ToList();
                        var GetWorkercuti = dbr.vw_Kerja_Hdr_Cuti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList).OrderBy(o => new { o.fld_Tarikh, o.fld_Nopkj }).ToList();
                        foreach (var Workercuti in GetWorkercuti)
                        {
                            var CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == Workercuti.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                            var Krytn = tbl_Pkjmast.Where(x => x.fld_Nopkj == Workercuti.fld_Nopkj).Select(s => s.fld_Kdrkyt).FirstOrDefault();
                            var getworkdetail = tbl_Pkjmast.Where(x => x.fld_Nopkj == Workercuti.fld_Nopkj).FirstOrDefault();
                            var ActvtCodeDesc = GetActCodeLeave.Where(x => x.fld_KodCuti == Workercuti.fld_Kdhdct).Select(s => new { s.fld_KodAktvt, s.fld_KeteranganCuti }).FirstOrDefault();
                            string GL = "";
                            if (Krytn == "MA")
                            {
                                GL = GetLeaveGL.Where(x => x.fld_KodAktiviti == ActvtCodeDesc.fld_KodAktvt && x.fld_TypeCode == "GLTKT" && x.fld_Flag == "2").Select(s => s.fld_SAPCode).FirstOrDefault();
                            }
                            else
                            {
                                GL = GetLeaveGL.Where(x => x.fld_KodAktiviti == ActvtCodeDesc.fld_KodAktvt && x.fld_TypeCode == "GLTKA" && x.fld_Flag == "2").Select(s => s.fld_SAPCode).FirstOrDefault();
                            }
                            var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == Workercuti.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                            var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == Workercuti.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                            DetailWork.Add(new CustMod_DetailWork2
                            {
                                id = id,
                                amounttype = 3,
                                amount = Workercuti.fld_Jumlah.Value,
                                quantity = 1,
                                wrk = Workercuti.fld_Nopkj,
                                wrkname = pkjname,
                                actvt = ActvtCodeDesc.fld_KodAktvt,
                                actvtdesc = ActvtCodeDesc.fld_KeteranganCuti,
                                cc = CC,
                                gl = GL,
                                grp = Workercuti.fld_Kum,
                                pkt = null,
                                fld_LadangID = Workercuti.fld_LadangID.Value,
                                fld_WilayahID = Workercuti.fld_WilayahID.Value,
                                fld_SyarikatID = Workercuti.fld_SyarikatID.Value,
                                fld_NegaraID = Workercuti.fld_NegaraID.Value,
                                active = actstatus,
                                month = Workercuti.fld_Tarikh.Value.Month,
                                year = Workercuti.fld_Tarikh.Value.Year,
                                day = Workercuti.fld_Tarikh.Value.Day
                            });
                            id++;
                        }
                    }
                    //Leave

                    //bool Selection21 = KONG , bool Selection22 = OT, bool Selection23 = OTHERS, bool Selection24 = PIECE RATE, bool Selection25 = INCENTIVE, bool Selection26 = DEDUCTION
                    //amounttype KONG = 1, OT = 2, OTHERS = 3, PIECE RATE = 4, INCENTIVE = 5, DEDUCTION = 6

                    //Incentive
                    if (Selection25)
                    {
                        //var GetIncetive = db.tbl_JenisInsentif.Where(x => x.fld_JenisInsentif == "P").ToList();
                        //var GetIncetiveCode = GetIncetive.Select(s => s.fld_KodInsentif).ToList();
                        //var GetWorkerIncntve = dbr.tbl_Insentif.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month >= MonthList && x.fld_Month <= MonthList2 && x.fld_Year == YearList && x.fld_Deleted == false && GetIncetiveCode.Contains(x.fld_KodInsentif) && x.fld_NilaiInsentif > 0).OrderBy(o => new { o.fld_Month, o.fld_Nopkj }).ToList();
                        //var GetGroups = dbr.tbl_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
                        //foreach (var WorkerIncntve in GetWorkerIncntve)
                        //{
                        //    var CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == WorkerIncntve.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                        //    var Krytn = tbl_Pkjmast.Where(x => x.fld_Nopkj == WorkerIncntve.fld_Nopkj).Select(s => s.fld_Kdrkyt).FirstOrDefault();
                        //    var getworkdetail = tbl_Pkjmast.Where(x => x.fld_Nopkj == WorkerIncntve.fld_Nopkj).FirstOrDefault();
                        //    var ActvtCodeDesc = GetIncetive.Where(x => x.fld_KodInsentif == WorkerIncntve.fld_KodInsentif).Select(s => new { s.fld_KodAktvt, s.fld_Keterangan }).FirstOrDefault();
                        //    var Grp = GetGroups.Where(x => x.fld_KumpulanID == getworkdetail.fld_KumpulanID).Select(s => s.fld_KodKumpulan).FirstOrDefault() == null ? "" : GetGroups.Where(x => x.fld_KumpulanID == getworkdetail.fld_KumpulanID).Select(s => s.fld_KodKumpulan).FirstOrDefault();
                        //    string GL = "";
                        //    if (Krytn == "MA")
                        //    {
                        //        GL = GetLeaveGL.Where(x => x.fld_KodAktiviti == ActvtCodeDesc.fld_KodAktvt && x.fld_TypeCode == "GLTKT" && x.fld_Flag == "5").Select(s => s.fld_SAPCode).FirstOrDefault();
                        //    }
                        //    else
                        //    {
                        //        GL = GetLeaveGL.Where(x => x.fld_KodAktiviti == ActvtCodeDesc.fld_KodAktvt && x.fld_TypeCode == "GLTKA" && x.fld_Flag == "5").Select(s => s.fld_SAPCode).FirstOrDefault();
                        //    }
                        //    var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == WorkerIncntve.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                        //    var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == WorkerIncntve.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                        //    DetailWork.Add(new CustMod_DetailWork2
                        //    {
                        //        id = id,
                        //        amounttype = 5,
                        //        amount = WorkerIncntve.fld_NilaiInsentif.Value,
                        //        quantity = 1,
                        //        wrk = WorkerIncntve.fld_Nopkj,
                        //        wrkname = pkjname,
                        //        actvt = ActvtCodeDesc.fld_KodAktvt,
                        //        actvtdesc = ActvtCodeDesc.fld_Keterangan,
                        //        cc = CC,
                        //        gl = GL,
                        //        grp = Grp,
                        //        pkt = null,
                        //        fld_LadangID = WorkerIncntve.fld_LadangID.Value,
                        //        fld_WilayahID = WorkerIncntve.fld_WilayahID.Value,
                        //        fld_SyarikatID = WorkerIncntve.fld_SyarikatID.Value,
                        //        fld_NegaraID = WorkerIncntve.fld_NegaraID.Value,
                        //        active = actstatus,
                        //        month = WorkerIncntve.fld_Month.Value,
                        //        year = WorkerIncntve.fld_Year.Value,
                        //        day = krjak.fld_Tarikh.Value.Day
                        //    });
                        //    id++;
                        //}
                    }
                    //Incentive

                    //bool Selection21 = KONG , bool Selection22 = OT, bool Selection23 = OTHERS, bool Selection24 = PIECE RATE, bool Selection25 = INCENTIVE, bool Selection26 = DEDUCTION
                    //amounttype KONG = 1, OT = 2, OTHERS = 3, PIECE RATE = 4, INCENTIVE = 5, DEDUCTION = 6

                    //Deduction
                    if (Selection26)
                    {
                        //var GetIncetive = db.tbl_JenisInsentif.Where(x => x.fld_JenisInsentif == "T").ToList();
                        //var GetIncetiveCode = GetIncetive.Select(s => s.fld_KodInsentif).ToList();
                        //var GetWorkerIncntve = dbr.tbl_Insentif.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month >= MonthList && x.fld_Month <= MonthList2 && x.fld_Year == YearList && x.fld_Deleted == false && GetIncetiveCode.Contains(x.fld_KodInsentif) && x.fld_NilaiInsentif > 0).OrderBy(o => new { o.fld_Month, o.fld_Nopkj }).ToList();
                        //var GetGroups = dbr.tbl_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
                        //foreach (var WorkerIncntve in GetWorkerIncntve)
                        //{
                        //    var CC = tbl_Pkjmast.Where(x => x.fld_Nopkj == WorkerIncntve.fld_Nopkj).Select(s => s.fld_KodSAPPekerja).FirstOrDefault();
                        //    var Krytn = tbl_Pkjmast.Where(x => x.fld_Nopkj == WorkerIncntve.fld_Nopkj).Select(s => s.fld_Kdrkyt).FirstOrDefault();
                        //    var getworkdetail = tbl_Pkjmast.Where(x => x.fld_Nopkj == WorkerIncntve.fld_Nopkj).FirstOrDefault();
                        //    var ActvtCodeDesc = GetIncetive.Where(x => x.fld_KodInsentif == WorkerIncntve.fld_KodInsentif).Select(s => new { s.fld_KodAktvt, s.fld_Keterangan }).FirstOrDefault();
                        //    var Grp = GetGroups.Where(x => x.fld_KumpulanID == getworkdetail.fld_KumpulanID).Select(s => s.fld_KodKumpulan).FirstOrDefault() == null ? "" : GetGroups.Where(x => x.fld_KumpulanID == getworkdetail.fld_KumpulanID).Select(s => s.fld_KodKumpulan).FirstOrDefault();
                        //    string GL = "";
                        //    if (Krytn == "MA")
                        //    {
                        //        GL = GetLeaveGL.Where(x => x.fld_KodAktiviti == ActvtCodeDesc.fld_KodAktvt && x.fld_TypeCode == "GLTKT" && x.fld_Flag == "5").Select(s => s.fld_SAPCode).FirstOrDefault();
                        //    }
                        //    else
                        //    {
                        //        GL = GetLeaveGL.Where(x => x.fld_KodAktiviti == ActvtCodeDesc.fld_KodAktvt && x.fld_TypeCode == "GLTKA" && x.fld_Flag == "5").Select(s => s.fld_SAPCode).FirstOrDefault();
                        //    }
                        //    var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == WorkerIncntve.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                        //    var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == WorkerIncntve.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                        //    DetailWork.Add(new CustMod_DetailWork2
                        //    {
                        //        id = id,
                        //        amounttype = 6,
                        //        amount = -WorkerIncntve.fld_NilaiInsentif.Value,
                        //        quantity = 1,
                        //        wrk = WorkerIncntve.fld_Nopkj,
                        //        wrkname = pkjname,
                        //        actvt = ActvtCodeDesc.fld_KodAktvt,
                        //        actvtdesc = ActvtCodeDesc.fld_Keterangan,
                        //        cc = CC,
                        //        gl = GL,
                        //        grp = Grp,
                        //        pkt = null,
                        //        fld_LadangID = WorkerIncntve.fld_LadangID.Value,
                        //        fld_WilayahID = WorkerIncntve.fld_WilayahID.Value,
                        //        fld_SyarikatID = WorkerIncntve.fld_SyarikatID.Value,
                        //        fld_NegaraID = WorkerIncntve.fld_NegaraID.Value,
                        //        active = actstatus,
                        //        month = WorkerIncntve.fld_Month.Value,
                        //        year = WorkerIncntve.fld_Year.Value,
                        //        day = krjak.fld_Tarikh.Value.Day
                        //    });
                        //    id++;
                        //}
                    }
                    //Deduction
                }
                else
                {
                    var getpiece = getallactvt.Where(x => x.fld_DisabledFlag != 3).Select(s => s.fld_KodAktvt).Distinct().ToList();
                    var getworkdetails = dbr.vw_KerjaInfoDetails2.Where(x => getpiece.Contains(x.fld_KodAktvt) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && wrk.Contains(x.fld_Nopkj)).ToList();
                    if ((listactvtk.Count() > 0 || listactvtp.Count() > 0) && !EnbleAktvt)
                    {
                        if (listactvtk.Count() > 0)
                        {
                            var getkrjak = getworkdetails.Where(x => listactvtk.Contains(x.fld_KodAktvt)).ToList();
                            foreach (var krjak in getkrjak)
                            {
                                var CC = "";
                                var leveldesc = "";
                                var mainlevelcode = "";
                                if (krjak.fld_JnsPkt == 1)
                                {
                                    CC = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_IOcode).FirstOrDefault();
                                    leveldesc = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_NamaPktUtama.ToUpper()).FirstOrDefault();
                                    mainlevelcode = krjak.fld_KodPkt;
                                }
                                else
                                {
                                    var getpktutm = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).FirstOrDefault();
                                    leveldesc = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).Select(s => s.fld_NamaPkt.ToUpper()).FirstOrDefault();
                                    CC = GetPktUtamas.Where(x => x.fld_PktUtama == getpktutm.fld_KodPktUtama).Select(s => s.fld_IOcode).FirstOrDefault();
                                    mainlevelcode = getpktutm.fld_KodPktUtama;
                                }
                                var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                                var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                                var actvtname = getallactvt.Where(x => x.fld_KodAktvt == krjak.fld_KodAktvt).Select(s => s.fld_Desc).FirstOrDefault();
                                DetailWork.Add(new CustMod_DetailWork2
                                {
                                    id = id,
                                    amounttype = 1,
                                    amount = 0,
                                    quantity = krjak.fld_JumlahHasil.Value,
                                    wrk = krjak.fld_Nopkj,
                                    wrkname = krjak.fld_Nama,
                                    actvt = krjak.fld_KodAktvt,
                                    actvtdesc = actvtname,
                                    cc = CC,
                                    gl = krjak.fld_GLKod,
                                    grp = krjak.fld_Kum,
                                    pkt = mainlevelcode,
                                    fld_LadangID = krjak.fld_LadangID.Value,
                                    fld_WilayahID = krjak.fld_WilayahID.Value,
                                    fld_SyarikatID = krjak.fld_SyarikatID.Value,
                                    fld_NegaraID = krjak.fld_NegaraID.Value,
                                    active = actstatus,
                                    month = krjak.fld_Tarikh.Value.Month,
                                    year = krjak.fld_Tarikh.Value.Year,
                                    day = krjak.fld_Tarikh.Value.Day
                                });
                                id++;
                            }
                        }
                        if (listactvtp.Count() > 0)
                        {
                            var getkrjak = getworkdetails.Where(x => listactvtp.Contains(x.fld_KodAktvt)).ToList();
                            foreach (var krjak in getkrjak)
                            {
                                var CC = "";
                                var leveldesc = "";
                                var mainlevelcode = "";
                                if (krjak.fld_JnsPkt == 1)
                                {
                                    CC = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_IOcode).FirstOrDefault();
                                    leveldesc = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_NamaPktUtama.ToUpper()).FirstOrDefault();
                                    mainlevelcode = krjak.fld_KodPkt;
                                }
                                else
                                {
                                    var getpktutm = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).FirstOrDefault();
                                    leveldesc = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).Select(s => s.fld_NamaPkt.ToUpper()).FirstOrDefault();
                                    CC = GetPktUtamas.Where(x => x.fld_PktUtama == getpktutm.fld_KodPktUtama).Select(s => s.fld_IOcode).FirstOrDefault();
                                    mainlevelcode = getpktutm.fld_KodPktUtama;
                                }
                                var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                                var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                                var actvtname = getallactvt.Where(x => x.fld_KodAktvt == krjak.fld_KodAktvt).Select(s => s.fld_Desc).FirstOrDefault();
                                DetailWork.Add(new CustMod_DetailWork2
                                {
                                    id = id,
                                    amounttype = 4,
                                    amount = 0,
                                    quantity = krjak.fld_JumlahHasil.Value,
                                    wrk = krjak.fld_Nopkj,
                                    wrkname = pkjname,
                                    actvt = krjak.fld_KodAktvt,
                                    actvtdesc = actvtname,
                                    cc = CC,
                                    gl = krjak.fld_GLKod,
                                    grp = krjak.fld_Kum,
                                    pkt = mainlevelcode,
                                    fld_LadangID = krjak.fld_LadangID.Value,
                                    fld_WilayahID = krjak.fld_WilayahID.Value,
                                    fld_SyarikatID = krjak.fld_SyarikatID.Value,
                                    fld_NegaraID = krjak.fld_NegaraID.Value,
                                    active = actstatus,
                                    month = krjak.fld_Tarikh.Value.Month,
                                    year = krjak.fld_Tarikh.Value.Year,
                                    day = krjak.fld_Tarikh.Value.Day
                                });
                                id++;
                            }
                        }
                    }
                    else if ((listactvtk.Count() > 0 || listactvtp.Count() > 0) && EnbleAktvt && PilihanAktvt == "0")
                    {
                        if (listactvtk.Count() > 0)
                        {
                            var getkrjak = getworkdetails.Where(x => listactvtk.Contains(x.fld_KodAktvt)).ToList();
                            foreach (var krjak in getkrjak)
                            {
                                var CC = "";
                                var leveldesc = "";
                                var mainlevelcode = "";
                                if (krjak.fld_JnsPkt == 1)
                                {
                                    CC = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_IOcode).FirstOrDefault();
                                    leveldesc = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_NamaPktUtama.ToUpper()).FirstOrDefault();
                                    mainlevelcode = krjak.fld_KodPkt;
                                }
                                else
                                {
                                    var getpktutm = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).FirstOrDefault();
                                    leveldesc = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).Select(s => s.fld_NamaPkt.ToUpper()).FirstOrDefault();
                                    CC = GetPktUtamas.Where(x => x.fld_PktUtama == getpktutm.fld_KodPktUtama).Select(s => s.fld_IOcode).FirstOrDefault();
                                    mainlevelcode = getpktutm.fld_KodPktUtama;
                                }
                                var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                                var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                                var actvtname = getallactvt.Where(x => x.fld_KodAktvt == krjak.fld_KodAktvt).Select(s => s.fld_Desc).FirstOrDefault();
                                DetailWork.Add(new CustMod_DetailWork2
                                {
                                    id = id,
                                    amounttype = 1,
                                    amount = 0,
                                    quantity = krjak.fld_JumlahHasil.Value,
                                    wrk = krjak.fld_Nopkj,
                                    wrkname = pkjname,
                                    actvt = krjak.fld_KodAktvt,
                                    actvtdesc = actvtname,
                                    cc = CC,
                                    gl = krjak.fld_GLKod,
                                    grp = krjak.fld_Kum,
                                    pkt = mainlevelcode,
                                    fld_LadangID = krjak.fld_LadangID.Value,
                                    fld_WilayahID = krjak.fld_WilayahID.Value,
                                    fld_SyarikatID = krjak.fld_SyarikatID.Value,
                                    fld_NegaraID = krjak.fld_NegaraID.Value,
                                    active = actstatus,
                                    month = krjak.fld_Tarikh.Value.Month,
                                    year = krjak.fld_Tarikh.Value.Year,
                                    day = krjak.fld_Tarikh.Value.Day
                                });
                                id++;
                            }
                        }
                        if (listactvtp.Count() > 0)
                        {
                            var getkrjak = getworkdetails.Where(x => listactvtp.Contains(x.fld_KodAktvt)).ToList();
                            foreach (var krjak in getkrjak)
                            {
                                var CC = "";
                                var leveldesc = "";
                                var mainlevelcode = "";
                                if (krjak.fld_JnsPkt == 1)
                                {
                                    CC = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_IOcode).FirstOrDefault();
                                    leveldesc = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_NamaPktUtama.ToUpper()).FirstOrDefault();
                                    mainlevelcode = krjak.fld_KodPkt;
                                }
                                else
                                {
                                    var getpktutm = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).FirstOrDefault();
                                    leveldesc = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).Select(s => s.fld_NamaPkt.ToUpper()).FirstOrDefault();
                                    CC = GetPktUtamas.Where(x => x.fld_PktUtama == getpktutm.fld_KodPktUtama).Select(s => s.fld_IOcode).FirstOrDefault();
                                    mainlevelcode = getpktutm.fld_KodPktUtama;
                                }
                                var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                                var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                                var actvtname = getallactvt.Where(x => x.fld_KodAktvt == krjak.fld_KodAktvt).Select(s => s.fld_Desc).FirstOrDefault();
                                DetailWork.Add(new CustMod_DetailWork2
                                {
                                    id = id,
                                    amounttype = 4,
                                    amount = 0,
                                    quantity = krjak.fld_JumlahHasil.Value,
                                    wrk = krjak.fld_Nopkj,
                                    wrkname = pkjname,
                                    actvt = krjak.fld_KodAktvt,
                                    actvtdesc = actvtname,
                                    cc = CC,
                                    gl = krjak.fld_GLKod,
                                    grp = krjak.fld_Kum,
                                    pkt = mainlevelcode,
                                    fld_LadangID = krjak.fld_LadangID.Value,
                                    fld_WilayahID = krjak.fld_WilayahID.Value,
                                    fld_SyarikatID = krjak.fld_SyarikatID.Value,
                                    fld_NegaraID = krjak.fld_NegaraID.Value,
                                    active = actstatus,
                                    month = krjak.fld_Tarikh.Value.Month,
                                    year = krjak.fld_Tarikh.Value.Year,
                                    day = krjak.fld_Tarikh.Value.Day
                                });
                                id++;
                            }
                        }
                    }
                    else if (EnbleAktvt && actvt.Count() == 1)
                    {
                        var getkrjak = getworkdetails.Where(x => actvt.Contains(x.fld_KodAktvt)).ToList();
                        foreach (var krjak in getkrjak)
                        {
                            var CC = "";
                            var leveldesc = "";
                            var mainlevelcode = "";
                            if (krjak.fld_JnsPkt == 1)
                            {
                                CC = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_IOcode).FirstOrDefault();
                                leveldesc = GetPktUtamas.Where(x => x.fld_PktUtama == krjak.fld_KodPkt).Select(s => s.fld_NamaPktUtama.ToUpper()).FirstOrDefault();
                                mainlevelcode = krjak.fld_KodPkt;
                            }
                            else
                            {
                                var getpktutm = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).FirstOrDefault();
                                leveldesc = GetPktSubs.Where(x => x.fld_Pkt == krjak.fld_KodPkt).Select(s => s.fld_NamaPkt.ToUpper()).FirstOrDefault();
                                CC = GetPktUtamas.Where(x => x.fld_PktUtama == getpktutm.fld_KodPktUtama).Select(s => s.fld_IOcode).FirstOrDefault();
                                mainlevelcode = getpktutm.fld_KodPktUtama;
                            }
                            var actstatus = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Kdaktf).FirstOrDefault();
                            var pkjname = tbl_Pkjmast.Where(x => x.fld_Nopkj == krjak.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                            var actvtname = getallactvt.Where(x => x.fld_KodAktvt == krjak.fld_KodAktvt).Select(s => s.fld_Desc).FirstOrDefault();
                            DetailWork.Add(new CustMod_DetailWork2
                            {
                                id = id,
                                amounttype = 1,
                                amount = 0,
                                quantity = krjak.fld_JumlahHasil.Value,
                                wrk = krjak.fld_Nopkj,
                                wrkname = pkjname,
                                actvt = krjak.fld_KodAktvt,
                                actvtdesc = actvtname,
                                cc = CC,
                                gl = krjak.fld_GLKod,
                                grp = krjak.fld_Kum,
                                pkt = mainlevelcode,
                                fld_LadangID = krjak.fld_LadangID.Value,
                                fld_WilayahID = krjak.fld_WilayahID.Value,
                                fld_SyarikatID = krjak.fld_SyarikatID.Value,
                                fld_NegaraID = krjak.fld_NegaraID.Value,
                                active = actstatus,
                                month = krjak.fld_Tarikh.Value.Month,
                                year = krjak.fld_Tarikh.Value.Year,
                                day = krjak.fld_Tarikh.Value.Day
                            });
                            id++;
                        }
                    }
                }

                var Syarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).FirstOrDefault();
                var Ladang = db.tbl_Ladang.Where(x => x.fld_ID == LadangID).FirstOrDefault();

                ViewBag.NamaSyarikat = Syarikat.fld_NamaSyarikat;
                ViewBag.NoSyarikat = Syarikat.fld_NoSyarikat;
                ViewBag.NamaLadang = Ladang.fld_LdgName;
                ViewBag.Pilihan = Pilihan;
                ViewBag.Tempoh = MonthList + "/" + YearList;
                ViewBag.TitleReport = "LAPORAN KERJA HARIAN";
                DetailWork = ReportLogic.CustMod_DetailWork2(DetailWork, cc, pkt, actvt, gl, grp, wrk);
                detailsreport = new detailsreport
                {
                    coloumgroup = coloumgroup,
                    CustMod_DetailWork2 = DetailWork
                };
                ViewBag.Selection1 = Selection1;
            }
            return View(detailsreport);
        }
        public JsonResult GetPkt(byte JnsPkt)
        {
            string host, catalog, user, pass = "";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            List<SelectListItem> PilihPeringkat = new List<SelectListItem>();
            switch (JnsPkt)
            {
                case 1:
                    var SelectPkt = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_DivisionID == DivisionID).ToList();
                    PilihPeringkat = new SelectList(SelectPkt.Select(s => new SelectListItem { Value = s.fld_PktUtama, Text = s.fld_PktUtama + " - " + s.fld_NamaPktUtama + " - (" + s.fld_IOcode + ")" }), "Value", "Text").ToList();
                    break;
                case 2:
                    var SelectPkt2 = dbr.tbl_SubPkt.Join(dbr.tbl_PktUtama, j => new { j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, fld_PktUtama = j.fld_KodPktUtama }, k => new { k.fld_NegaraID, k.fld_SyarikatID, k.fld_WilayahID, k.fld_LadangID, fld_PktUtama = k.fld_PktUtama }, (j, k) => new { j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, k.fld_JnsLot, j.fld_Pkt, j.fld_NamaPkt, j.fld_Deleted, k.fld_IOcode, k.fld_DivisionID }).Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_DivisionID == DivisionID).ToList();
                    PilihPeringkat = new SelectList(SelectPkt2.Select(s => new SelectListItem { Value = s.fld_Pkt, Text = s.fld_Pkt + " - " + s.fld_NamaPkt + " - (" + s.fld_IOcode + ")" }), "Value", "Text").ToList();
                    break;
                case 3:
                    var SelectPkt3 = dbr.tbl_Blok.Join(dbr.tbl_PktUtama, j => new { j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, fld_PktUtama = j.fld_KodPktutama }, k => new { k.fld_NegaraID, k.fld_SyarikatID, k.fld_WilayahID, k.fld_LadangID, fld_PktUtama = k.fld_PktUtama }, (j, k) => new { j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, k.fld_JnsLot, j.fld_Blok, j.fld_NamaBlok, j.fld_Deleted, k.fld_IOcode, k.fld_DivisionID }).Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_DivisionID == DivisionID).ToList();
                    PilihPeringkat = new SelectList(SelectPkt3.Select(s => new SelectListItem { Value = s.fld_Blok, Text = s.fld_Blok + " - " + s.fld_NamaBlok + " - (" + s.fld_IOcode + ")" }), "Value", "Text").ToList();
                    break;
            }
            if (PilihPeringkat.Count > 0)
            {
                PilihPeringkat.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            }
            else
            {
                PilihPeringkat.Insert(0, (new SelectListItem { Text = "No Data", Value = "-" }));
            }

            dbr.Dispose();

            return Json(new { PilihPeringkat });
        }
    }
}