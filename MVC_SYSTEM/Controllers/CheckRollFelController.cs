using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.App_LocalResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_SYSTEM.CustomModels;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Security;
using System.Data.Entity;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public class CheckRollFelController : Controller
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
        private CheckrollFunction EstateFunction = new CheckrollFunction();

        //public ActionResult Index()
        //{
        //    ViewBag.CheckRoll = "class = active";
        //    int? getuserid = GetIdentity.ID(User.Identity.Name);
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    ViewBag.CheckRollMenu = new SelectList(db.tblMenuLists.Where(x => x.fld_Flag == "dataEntry" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fld_Val", "fld_Desc");
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Index(string CheckRollMenu)
        //{
        //    return RedirectToAction(CheckRollMenu, "CheckRoll");
        //}

        public ActionResult WorkingDetails()
        {
            DateTime? date = timezone.gettimezone();
            DateTime Today = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day);
            string host, catalog, user, pass = "";
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            double ValidDayApp = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> SelectionData = new List<SelectListItem>();
            SelectionData.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblSelection, Value = "0" }));
            ViewBag.SelectionData = SelectionData;
            ViewBag.CheckRoll = "class = active";

            var CheckBlockKeyInDay = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "blokdatakerja" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfValue).FirstOrDefault();
            var CheckBlockValidDayApp = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "blokdatakerjavlddt" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfValue).FirstOrDefault();
            var CheckLastDataKeyIn = dbr.tbl_Kerjahdr.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Year == date.Value.Year && x.fld_Tarikh.Value.Month == date.Value.Month).Select(s => s.fld_Tarikh).FirstOrDefault();
            ValidDayApp = 1 - double.Parse(CheckBlockValidDayApp);

            if (CheckLastDataKeyIn == null)
            {
                DateTime? LastDate = new DateTime(date.Value.Year, date.Value.Month, 1);
                CheckLastDataKeyIn = LastDate;
            }

            double TotalDayLastKeyInDbl = (date - CheckLastDataKeyIn).Value.TotalDays;
            short TotalDayLastKeyIn = Convert.ToInt16(TotalDayLastKeyInDbl);
            short TotalDayLastNeedKeyIn = short.Parse(CheckBlockKeyInDay);
            double ApprovalDayCount = 0;
            var CheckBlockStatus = db.tbl_BlckKmskknDataKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Year == date.Value.Year && x.fld_Month == date.Value.Month).FirstOrDefault();

            if (TotalDayLastKeyIn >= TotalDayLastNeedKeyIn && CheckBlockStatus != null)
            {
                if (CheckBlockStatus.fld_BlokStatus == true)
                {
                    dbr.Dispose();
                    return RedirectToAction("CheckRollBlock", "CheckRollFel", new { msg = 1 });
                }
                else
                {
                    ApprovalDayCount = (CheckBlockStatus.fld_ValidDT - Today).Value.TotalDays;

                    if (ApprovalDayCount < ValidDayApp)
                    {
                        CheckBlockStatus.fld_BlokStatus = true;
                        db.Entry(CheckBlockStatus).State = EntityState.Modified;
                        db.SaveChanges();
                        dbr.Dispose();
                        return RedirectToAction("CheckRollBlock", "CheckRollFel", new { msg = 1 });
                    }
                    else
                    {
                        dbr.Dispose();
                        return View();
                    }
                }
            }
            else if (TotalDayLastKeyIn >= TotalDayLastNeedKeyIn && CheckBlockStatus == null)
            {
                tbl_BlckKmskknDataKerja tbl_BlckKmskknDataKerja = new tbl_BlckKmskknDataKerja();
                tbl_BlckKmskknDataKerja.fld_BlokStatus = true;
                tbl_BlckKmskknDataKerja.fld_BilHariXKyIn = TotalDayLastKeyIn;
                tbl_BlckKmskknDataKerja.fld_Month = date.Value.Month;
                tbl_BlckKmskknDataKerja.fld_Year = date.Value.Year;
                tbl_BlckKmskknDataKerja.fld_LadangID = LadangID;
                tbl_BlckKmskknDataKerja.fld_WilayahID = WilayahID;
                tbl_BlckKmskknDataKerja.fld_SyarikatID = SyarikatID;
                tbl_BlckKmskknDataKerja.fld_NegaraID = NegaraID;
                tbl_BlckKmskknDataKerja.fld_Reason = "";
                db.tbl_BlckKmskknDataKerja.Add(tbl_BlckKmskknDataKerja);
                db.SaveChanges();
                dbr.Dispose();
                return RedirectToAction("CheckRollBlock", "CheckRollFel", new { msg = 1 });
            }
            else if (TotalDayLastKeyIn <= TotalDayLastNeedKeyIn && CheckBlockStatus != null)
            {
                if (CheckBlockStatus.fld_BlokStatus == true)
                {
                    ApprovalDayCount = (CheckBlockStatus.fld_ValidDT - Today).Value.TotalDays;
                    if (ApprovalDayCount < ValidDayApp)
                    {
                        CheckBlockStatus.fld_BlokStatus = true;
                        db.Entry(CheckBlockStatus).State = EntityState.Modified;
                        db.SaveChanges();
                        dbr.Dispose();
                        return RedirectToAction("CheckRollBlock", "CheckRollFel", new { msg = 1 });
                    }
                    else
                    {
                        dbr.Dispose();
                        return View();
                    }
                }
                else
                {
                    dbr.Dispose();
                    return View();
                }
            }
            else
            {
                dbr.Dispose();
                return View();
            }
        }

        public ActionResult CheckRollBlock(int msg)
        {
            string Message = "";
            ViewBag.CheckRoll = "class = active";
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            var CheckBlockKeyInDay = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "blokdatakerja" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfValue).FirstOrDefault();
            switch (msg)
            {
                case 1:
                    Message = "Kemasukkan maklumat kerja telah disekat kerana pihak wilayah atau HQ belum membuka sekatan ketika ini.";
                    break;
                case 2:
                    Message = "Kemasukkan maklumat kerja telah disekat kerana tiada kemasukkan kerja dilakukan selama " + CheckBlockKeyInDay + " hari berturut-turut.";
                    break;
            }

            ViewBag.Message = Message;

            return View();
        }

        public ActionResult Attendance()
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WorkCode = new List<SelectListItem>();
            List<SelectListItem> Rainning = new List<SelectListItem>();

            WorkCode = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "cuti" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }), "Value", "Text", "H01").ToList();
            Rainning.Insert(0, (new SelectListItem { Text = "Tidak", Value = "0", Selected = true }));
            Rainning.Insert(0, (new SelectListItem { Text = "Ya", Value = "1" }));

            ViewBag.Rainning = Rainning;
            ViewBag.WorkCode = WorkCode;
            return View();
        }

        [HttpPost]
        public ActionResult Attendance(CustMod_Attandance CustMod_Attandance)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string msg = "";
            string statusmsg = "";
            string msg2 = "";
            string statusmsg2 = "";
            bool disablesavebtn = true;
            int KumpulanID = 0;
            string KumpulanKod = "";
            bool ZeroLeaveBal = false;
            bool InvalidCutiAm = false;
            bool AlertPopup = false;
            bool LeaveSelection = false;
            DateTime date = timezone.gettimezone();
            string bodyview2 = "";
            bool CutOfDateStatus = false;
            string Msg = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            List<tbl_Kerjahdr> tbl_Kerjahdrs = new List<tbl_Kerjahdr>();
            List<tbl_Kerjahdr> returntbl_Kerjahdr = new List<tbl_Kerjahdr>();
            List<CustMod_WorkIdList> CustMod_WorkIdLists = new List<CustMod_WorkIdList>();
            int? LadangNegeriCode = 0;

            try
            {
                LadangNegeriCode = int.Parse(GetLadang.GetLadangNegeriCode(LadangID));
                if (EstateFunction.GetCutiAmMgguMatchDate(NegaraID, SyarikatID, WilayahID, LadangID, CustMod_Attandance.dateseleted, CustMod_Attandance.WorkCode, out Msg))
                {
                    CutOfDateStatus = EstateFunction.GetStatusCutProcess(dbr, CustMod_Attandance.dateseleted, NegaraID, SyarikatID, WilayahID, LadangID);
                    if (!CutOfDateStatus)
                    {
                        LeaveSelection = EstateFunction.CheckLeaveType(CustMod_Attandance.WorkCode, NegaraID, SyarikatID) ? true : false;
                        if (CustMod_Attandance.SelectionCategory == 1)
                        {
                            KumpulanID = dbr.tbl_KumpulanKerja.Where(x => x.fld_KodKumpulan.Trim() == CustMod_Attandance.SelectionData.Trim() && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).Select(s => s.fld_KumpulanID).FirstOrDefault();
                            var pkjids = dbr.tbl_Pkjmast.Where(x => x.fld_KumpulanID == KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").Select(s => s.fld_Nopkj.Trim()).ToList();
                            var datainkrjhdrs = dbr.tbl_Kerjahdr.Where(x => x.fld_Kum.Trim() == CustMod_Attandance.SelectionData && x.fld_Tarikh == CustMod_Attandance.dateseleted && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
                            if (datainkrjhdrs.Count() == 0)
                            {
                                foreach (var pkjid in pkjids)
                                {
                                    if (LeaveSelection)
                                    {
                                        if (EstateFunction.LeaveCalBal(dbr, CustMod_Attandance.dateseleted.Year, pkjid, CustMod_Attandance.WorkCode, NegaraID, SyarikatID, WilayahID, LadangID))
                                        {
                                            tbl_Kerjahdrs.Add(new tbl_Kerjahdr() { fld_Nopkj = pkjid, fld_Kum = CustMod_Attandance.SelectionData, fld_Tarikh = CustMod_Attandance.dateseleted, fld_Kdhdct = CustMod_Attandance.WorkCode, fld_Hujan = CustMod_Attandance.Rainning, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_CreatedBy = getuserid, fld_CreatedDT = date, fld_DataSource = "B" });
                                            EstateFunction.LeaveDeduct(dbr, CustMod_Attandance.dateseleted.Year, pkjid, CustMod_Attandance.WorkCode, NegaraID, SyarikatID, WilayahID, LadangID);
                                        }
                                        else
                                        {
                                            msg2 = msg2 + " ," + pkjid;
                                            statusmsg2 = "Perhatian";
                                            ZeroLeaveBal = true;
                                        }
                                    }
                                    else
                                    {
                                        tbl_Kerjahdrs.Add(new tbl_Kerjahdr() { fld_Nopkj = pkjid, fld_Kum = CustMod_Attandance.SelectionData, fld_Tarikh = CustMod_Attandance.dateseleted, fld_Kdhdct = CustMod_Attandance.WorkCode, fld_Hujan = CustMod_Attandance.Rainning, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_CreatedBy = getuserid, fld_CreatedDT = date, fld_DataSource = "B" });
                                    }
                                }

                                disablesavebtn = true;
                            }
                            else
                            {
                                if (CustMod_Attandance.atteditstatus == 1)
                                {
                                    List<string> needtoadds = pkjids.Except(datainkrjhdrs.Select(s => s.fld_Nopkj.Trim())).ToList();
                                    if (LeaveSelection)
                                    {
                                        foreach (var pkjid in needtoadds)
                                        {
                                            if (EstateFunction.LeaveCalBal(dbr, CustMod_Attandance.dateseleted.Year, pkjid, CustMod_Attandance.WorkCode, NegaraID, SyarikatID, WilayahID, LadangID))
                                            {
                                                tbl_Kerjahdrs.Add(new tbl_Kerjahdr() { fld_Nopkj = pkjid, fld_Kum = CustMod_Attandance.SelectionData, fld_Tarikh = CustMod_Attandance.dateseleted, fld_Kdhdct = CustMod_Attandance.WorkCode, fld_Hujan = CustMod_Attandance.Rainning, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_CreatedBy = getuserid, fld_CreatedDT = date, fld_DataSource = "B" });
                                                EstateFunction.LeaveDeduct(dbr, CustMod_Attandance.dateseleted.Year, pkjid, CustMod_Attandance.WorkCode, NegaraID, SyarikatID, WilayahID, LadangID);
                                            }
                                            else
                                            {
                                                msg2 = msg2 + " ," + pkjid;
                                                statusmsg2 = "Perhatian";
                                                ZeroLeaveBal = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (var pkjid in needtoadds)
                                        {
                                            tbl_Kerjahdrs.Add(new tbl_Kerjahdr() { fld_Nopkj = pkjid, fld_Kum = CustMod_Attandance.SelectionData, fld_Tarikh = CustMod_Attandance.dateseleted, fld_Kdhdct = CustMod_Attandance.WorkCode, fld_Hujan = CustMod_Attandance.Rainning, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_CreatedBy = getuserid, fld_CreatedDT = date, fld_DataSource = "B" });
                                        }
                                    }
                                    msg = GlobalResEstate.msgUpdate;
                                    statusmsg = "success";
                                    disablesavebtn = true;
                                    if (needtoadds.Count() == 0)
                                    {
                                        msg2 = GlobalResEstate.msgAddAttendance;
                                        statusmsg2 = "Perhatian";
                                        AlertPopup = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            var datainkrjhdr = dbr.tbl_Kerjahdr.Where(x => x.fld_Nopkj.Trim() == CustMod_Attandance.SelectionData && x.fld_Tarikh == CustMod_Attandance.dateseleted && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).FirstOrDefault();

                            if (datainkrjhdr == null)
                            {
                                if (LeaveSelection)
                                {
                                    if (EstateFunction.LeaveCalBal(dbr, CustMod_Attandance.dateseleted.Year, CustMod_Attandance.SelectionData, CustMod_Attandance.WorkCode, NegaraID, SyarikatID, WilayahID, LadangID))
                                    {
                                        var pkjdata = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == CustMod_Attandance.SelectionData && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1").Select(s => new { s.fld_Nopkj, s.fld_KumpulanID }).FirstOrDefault();
                                        KumpulanKod = dbr.tbl_KumpulanKerja.Where(x => x.fld_KumpulanID == pkjdata.fld_KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).Select(s => s.fld_KodKumpulan).FirstOrDefault();
                                        tbl_Kerjahdrs.Add(new tbl_Kerjahdr() { fld_Nopkj = pkjdata.fld_Nopkj, fld_Kum = KumpulanKod.Trim(), fld_Tarikh = CustMod_Attandance.dateseleted, fld_Kdhdct = CustMod_Attandance.WorkCode, fld_Hujan = CustMod_Attandance.Rainning, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_CreatedBy = getuserid, fld_CreatedDT = date, fld_DataSource = "B" });
                                        disablesavebtn = true;
                                        EstateFunction.LeaveDeduct(dbr, CustMod_Attandance.dateseleted.Year, CustMod_Attandance.SelectionData, CustMod_Attandance.WorkCode, NegaraID, SyarikatID, WilayahID, LadangID);
                                    }
                                    else
                                    {
                                        msg2 = msg2 + " ," + CustMod_Attandance.SelectionData;
                                        statusmsg2 = "Perhatian";
                                        ZeroLeaveBal = true;
                                    }
                                }
                                else
                                {
                                    var pkjdata = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == CustMod_Attandance.SelectionData && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1").Select(s => new { s.fld_Nopkj, s.fld_KumpulanID }).FirstOrDefault();
                                    KumpulanKod = dbr.tbl_KumpulanKerja.Where(x => x.fld_KumpulanID == pkjdata.fld_KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).Select(s => s.fld_KodKumpulan).FirstOrDefault();
                                    tbl_Kerjahdrs.Add(new tbl_Kerjahdr() { fld_Nopkj = pkjdata.fld_Nopkj, fld_Kum = KumpulanKod.Trim(), fld_Tarikh = CustMod_Attandance.dateseleted, fld_Kdhdct = CustMod_Attandance.WorkCode, fld_Hujan = CustMod_Attandance.Rainning, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_CreatedBy = getuserid, fld_CreatedDT = date, fld_DataSource = "B" });
                                }
                            }
                            else
                            {
                                if (CustMod_Attandance.atteditstatus == 1)
                                {
                                    if (LeaveSelection)
                                    {
                                        if (EstateFunction.IndividuCheckLeaveTake(datainkrjhdr.fld_Kdhdct, NegaraID, SyarikatID))
                                        {
                                            EstateFunction.LeaveAdd(dbr, CustMod_Attandance.dateseleted.Year, datainkrjhdr.fld_Nopkj, datainkrjhdr.fld_Kdhdct, NegaraID, SyarikatID, WilayahID, LadangID);

                                            if (EstateFunction.LeaveCalBal(dbr, CustMod_Attandance.dateseleted.Year, datainkrjhdr.fld_Nopkj, CustMod_Attandance.WorkCode, NegaraID, SyarikatID, WilayahID, LadangID))
                                            {
                                                EstateFunction.LeaveDeduct(dbr, CustMod_Attandance.dateseleted.Year, datainkrjhdr.fld_Nopkj, CustMod_Attandance.WorkCode, NegaraID, SyarikatID, WilayahID, LadangID);
                                            }
                                            else
                                            {
                                                msg2 = msg2 + " ," + datainkrjhdr.fld_Nopkj;
                                                statusmsg2 = "Perhatian";
                                                ZeroLeaveBal = true;
                                            }
                                        }
                                        else
                                        {
                                            if (EstateFunction.LeaveCalBal(dbr, CustMod_Attandance.dateseleted.Year, datainkrjhdr.fld_Nopkj, CustMod_Attandance.WorkCode, NegaraID, SyarikatID, WilayahID, LadangID))
                                            {
                                                EstateFunction.LeaveDeduct(dbr, CustMod_Attandance.dateseleted.Year, datainkrjhdr.fld_Nopkj, CustMod_Attandance.WorkCode, NegaraID, SyarikatID, WilayahID, LadangID);
                                            }
                                            else
                                            {
                                                msg2 = msg2 + " ," + datainkrjhdr.fld_Nopkj;
                                                statusmsg2 = "Perhatian";
                                                ZeroLeaveBal = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (EstateFunction.IndividuCheckLeaveTake(datainkrjhdr.fld_Kdhdct, NegaraID, SyarikatID))
                                        {
                                            EstateFunction.LeaveAdd(dbr, CustMod_Attandance.dateseleted.Year, datainkrjhdr.fld_Nopkj, datainkrjhdr.fld_Kdhdct, NegaraID, SyarikatID, WilayahID, LadangID);
                                        }
                                    }

                                    if (!ZeroLeaveBal)
                                    {
                                        bool DeleteDataKerja = false;
                                        if (datainkrjhdr.fld_Kdhdct != CustMod_Attandance.WorkCode && datainkrjhdr.fld_Hujan != CustMod_Attandance.Rainning)
                                        {
                                            DeleteDataKerja = true;
                                        }
                                        else if (datainkrjhdr.fld_Kdhdct != CustMod_Attandance.WorkCode && datainkrjhdr.fld_Hujan == CustMod_Attandance.Rainning)
                                        {
                                            DeleteDataKerja = true;
                                        }
                                        else if (datainkrjhdr.fld_Kdhdct == CustMod_Attandance.WorkCode && datainkrjhdr.fld_Hujan != CustMod_Attandance.Rainning)
                                        {
                                            DeleteDataKerja = true;
                                        }

                                        if (DeleteDataKerja)
                                        {
                                            var GetKerja = dbr.tbl_Kerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh == datainkrjhdr.fld_Tarikh && x.fld_Nopkj == datainkrjhdr.fld_Nopkj).ToList();
                                            if (GetKerja.Count > 0)
                                            {
                                                dbr.tbl_Kerja.RemoveRange(GetKerja);
                                            }
                                        }

                                        datainkrjhdr.fld_Hujan = CustMod_Attandance.Rainning;
                                        datainkrjhdr.fld_Kdhdct = CustMod_Attandance.WorkCode;
                                        datainkrjhdr.fld_CreatedBy = getuserid;
                                        datainkrjhdr.fld_CreatedDT = date;
                                        dbr.SaveChanges();

                                        msg = GlobalResEstate.msgUpdate;
                                        statusmsg = "success";
                                        disablesavebtn = true;
                                    }
                                }
                            }
                        }

                        if (tbl_Kerjahdrs.Count() != 0)
                        {
                            msg = GlobalResEstate.msgAdd;
                            statusmsg = "success";
                            dbr.tbl_Kerjahdr.AddRange(tbl_Kerjahdrs);
                            dbr.SaveChanges();
                        }

                        if (ZeroLeaveBal)
                        {
                            msg2 = GlobalResEstate.msgAttendanceLeave;
                            msg = msg2;
                            statusmsg2 = "Perhatian";
                            statusmsg = "warning";
                        }
                    }
                    else
                    {
                        msg = GlobalResEstate.msgError;
                        statusmsg = "warning";
                        disablesavebtn = true;
                    }
                }
                else
                {
                    msg = Msg;
                    statusmsg2 = "Perhatian";
                    statusmsg = "warning";
                }

                List<CustMod_WorkerWork> CustMod_WorkerWorks = new List<CustMod_WorkerWork>();
                List<tbl_Kerja> tbl_KerjaList = new List<tbl_Kerja>();
                if (CustMod_Attandance.SelectionCategory == 1)
                {
                    tbl_KerjaList = dbr.tbl_Kerja.Where(x => x.fld_Kum == CustMod_Attandance.SelectionData && x.fld_Tarikh == CustMod_Attandance.dateseleted && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Distinct().ToList();
                }
                else
                {
                    tbl_KerjaList = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == CustMod_Attandance.SelectionData && x.fld_Tarikh == CustMod_Attandance.dateseleted && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Distinct().ToList();
                }

                foreach (var tbl_KerjaData in tbl_KerjaList)
                {
                    var namepkj = EstateFunction.PkjName(dbr, NegaraID, SyarikatID, WilayahID, LadangID, tbl_KerjaData.fld_Nopkj);
                    CustMod_WorkerWorks.Add(new CustMod_WorkerWork() { fld_ID = tbl_KerjaData.fld_ID, fld_Nopkj = tbl_KerjaData.fld_Nopkj, fld_NamaPkj = namepkj, fld_Amount = tbl_KerjaData.fld_Amount, fld_JumlahHasil = tbl_KerjaData.fld_JumlahHasil, fld_KodAktvt = tbl_KerjaData.fld_KodAktvt, fld_KodGL = tbl_KerjaData.fld_KodGL, fld_KodPkt = tbl_KerjaData.fld_KodPkt, fld_Kum = tbl_KerjaData.fld_Kum, fld_Tarikh = tbl_KerjaData.fld_Tarikh, fld_JamOT = tbl_KerjaData.fld_JamOT, fld_Unit = tbl_KerjaData.fld_Unit, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_AmountOA = tbl_KerjaData.fld_OverallAmount });
                }

                bodyview2 = RenderRazorViewToString("WorkRecordList", CustMod_WorkerWorks, CutOfDateStatus);
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                msg = GlobalResEstate.msgError;
                statusmsg = "warning";
                disablesavebtn = true;
            }
            dbr.Dispose();
            return Json(new { proceedstatus = disablesavebtn, statusmsg, msg, ZeroLeaveBal, statusmsg2, msg2, AlertPopup, tablelisting2 = bodyview2 });
        }

        public ActionResult _NeglectedDay()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> JenisChargeHT = new List<SelectListItem>();
            List<SelectListItem> JnisPktHT = new List<SelectListItem>();
            List<SelectListItem> PilihanPktHT = new List<SelectListItem>();
            List<SelectListItem> PilihanAktvtHT = new List<SelectListItem>();
            List<SelectListItem> PilihanMasaHT = new List<SelectListItem>();

            var getJenisActvtDetails = db.tbl_JenisAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_DisabledFlag == 3 && x.fld_Deleted == false).FirstOrDefault();

            JnisPktHT = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnspkt" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            PilihanPktHT = new SelectList(dbr.tbl_PktUtama.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).Select(s => new SelectListItem { Value = s.fld_PktUtama, Text = s.fld_PktUtama + " - " + s.fld_NamaPktUtama }), "Value", "Text").ToList();
            PilihanAktvtHT = new SelectList(db.tbl_UpahAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJenisAktvt == getJenisActvtDetails.fld_KodJnsAktvt && x.fld_Deleted == false).OrderBy(o => o.fld_KodAktvt).Select(s => new SelectListItem { Value = s.fld_KodAktvt, Text = s.fld_KodAktvt }), "Value", "Text").ToList();
            PilihanAktvtHT.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            JenisChargeHT.Add(new SelectListItem { Text = "Kong", Value = "kong", Selected = true });
            JenisChargeHT.Add(new SelectListItem { Text = "Kadaran", Value = "kadaran", Selected = false });

            PilihanMasaHT.Add(new SelectListItem { Text = "Sepenuh Hari", Value = "penuh", Selected = true });
            PilihanMasaHT.Add(new SelectListItem { Text = "Separuh Hari", Value = "separuh", Selected = false });

            ViewBag.JnisPktHT = JnisPktHT;
            ViewBag.PilihanPktHT = PilihanPktHT;
            ViewBag.PilihanAktvtHT = PilihanAktvtHT;
            ViewBag.JenisChargeHT = JenisChargeHT;
            ViewBag.PilihanMasaHT = PilihanMasaHT;
            dbr.Dispose();

            return View();
        }

        [HttpPost]
        public ActionResult _NeglectedDay(CustMod_HariTerabai CustMod_HariTerabai)
        {
            EncryptDecrypt Encrypt = new EncryptDecrypt();
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string msg = "";
            string statusmsg = "";
            string msg2 = "";
            string statusmsg2 = "";
            bool disablesavebtn = true;
            int KumpulanID = 0;
            string KumpulanKod = "";
            bool ZeroLeaveBal = false;
            bool AlertPopup = false;
            bool AuthStatus = false;
            DateTime date = timezone.gettimezone();
            string bodyview2 = "";
            bool CutOfDateStatus = false;
            string GLCode = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            List<tbl_Kerjahdr> tbl_Kerjahdrs = new List<tbl_Kerjahdr>();
            List<tbl_Kerja> tbl_Kerjas = new List<tbl_Kerja>();
            List<CustMod_WorkIdList> CustMod_WorkIdLists = new List<CustMod_WorkIdList>();
            List<tbl_KerjaHariTerabai> tbl_KerjaHariTerabais = new List<tbl_KerjaHariTerabai>();
            tbl_JenisAktiviti GetJenisActvtDetails = new tbl_JenisAktiviti();
            tbl_UpahAktiviti GetActvty = new tbl_UpahAktiviti();

            string idpengurus = CustMod_HariTerabai.ManagerID;
            string passpengurus = CustMod_HariTerabai.ManagerPassword;
            decimal? Amount = 0;
            decimal? Hasil = 0;
            decimal? GetKadarUpah = 0;
            string JenisCharge = "";
            string MasaKerja = "";

            if (string.IsNullOrEmpty(idpengurus) == false && string.IsNullOrEmpty(passpengurus) == false)
            {
                if (CustMod_HariTerabai.JenisChargeHT == "kong")
                {
                    GetActvty = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodAktvt == CustMod_HariTerabai.PilihanAktvtHT).FirstOrDefault();
                    GetKadarUpah = GetActvty.fld_Harga;
                    GetJenisActvtDetails = db.tbl_JenisAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_DisabledFlag == 3 && x.fld_Deleted == false).FirstOrDefault();
                    JenisCharge = CustMod_HariTerabai.JenisChargeHT;
                    MasaKerja = CustMod_HariTerabai.PilihanMasaHT;
                }
                else
                {
                    JenisCharge = CustMod_HariTerabai.JenisChargeHT;
                    MasaKerja = "-";
                }

                passpengurus = Encrypt.Encrypt(passpengurus);
                var pengurus = db.tblUsers.Where(x => x.fldUserName == idpengurus && x.fldUserPassword == passpengurus && x.fldDeleted == false && x.fldRoleID <= 6 && x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID && x.fldWilayahID == WilayahID && x.fldLadangID == LadangID).SingleOrDefault();
                if (pengurus != null)
                {
                    try
                    {
                        if (EstateFunction.CheckSAPGLMap(dbr, CustMod_HariTerabai.JnisPktHT, CustMod_HariTerabai.PilihanPktHT, CustMod_HariTerabai.PilihanAktvtHT, NegaraID, SyarikatID, WilayahID, LadangID, true, CustMod_HariTerabai.JenisChargeHT, out GLCode))
                        {
                            CutOfDateStatus = EstateFunction.GetStatusCutProcess(dbr, CustMod_HariTerabai.dateseleted, NegaraID, SyarikatID, WilayahID, LadangID);
                            if (!CutOfDateStatus)
                            {
                                if (CustMod_HariTerabai.SelectionCategory == 1)
                                {
                                    KumpulanID = dbr.tbl_KumpulanKerja.Where(x => x.fld_KodKumpulan.Trim() == CustMod_HariTerabai.SelectionData.Trim() && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).Select(s => s.fld_KumpulanID).FirstOrDefault();
                                    var pkjids = dbr.tbl_Pkjmast.Where(x => x.fld_KumpulanID == KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").Select(s => s.fld_Nopkj).ToList();
                                    var datainkrjhdrs = dbr.tbl_Kerjahdr.Where(x => x.fld_Kum.Trim() == CustMod_HariTerabai.SelectionData && x.fld_Tarikh == CustMod_HariTerabai.dateseleted && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
                                    if (datainkrjhdrs.Count() == 0)
                                    {
                                        foreach (var pkjid in pkjids)
                                        {
                                            tbl_Kerjahdrs.Add(new tbl_Kerjahdr() { fld_Nopkj = pkjid, fld_Kum = CustMod_HariTerabai.SelectionData, fld_Tarikh = CustMod_HariTerabai.dateseleted, fld_Kdhdct = CustMod_HariTerabai.WorkCode, fld_Hujan = CustMod_HariTerabai.Rainning, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_CreatedBy = getuserid, fld_CreatedDT = date, fld_DataSource = "B" });
                                            if (CustMod_HariTerabai.JenisChargeHT == "kong")
                                            {
                                                if (CustMod_HariTerabai.PilihanMasaHT == "separuh")
                                                {
                                                    Amount = GetKadarUpah / 2;
                                                    Hasil = 0.5m;
                                                }
                                                else
                                                {
                                                    Amount = GetKadarUpah;
                                                    Hasil = 1;
                                                }

                                                tbl_Kerjas.Add(new tbl_Kerja() { fld_Amount = Amount, fld_Bonus = 0, fld_BrtGth = 0, fld_CreatedBy = getuserid, fld_CreatedDT = timezone.gettimezone(), fld_DataSource = "B", fld_JamOT = 0, fld_JnisAktvt = GetJenisActvtDetails.fld_KodJnsAktvt, fld_JnsPkt = CustMod_HariTerabai.JnisPktHT, fld_JumlahHasil = Hasil, fld_KadarByr = GetKadarUpah, fld_KdhMenuai = "-", fld_KodAktvt = CustMod_HariTerabai.PilihanAktvtHT, fld_KodGL = "602", fld_KodPkt = CustMod_HariTerabai.PilihanPktHT, fld_Kong = 0, fld_Kum = CustMod_HariTerabai.SelectionData, fld_LadangID = LadangID, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_Nopkj = pkjid, fld_PerBrshGth = 0, fld_Quality = 0, fld_Tarikh = CustMod_HariTerabai.dateseleted, fld_Unit = GetActvty.fld_Unit, fld_HrgaKwsnSkar = 0, fld_KodKwsnSkar = "-", fld_OverallAmount = Amount });
                                            }
                                            tbl_KerjaHariTerabais.Add(new tbl_KerjaHariTerabai() { fld_Nopkj = pkjid, fld_Tarikh = CustMod_HariTerabai.dateseleted, fld_JenisCharge = JenisCharge, fld_MasaKerja = MasaKerja, fld_ApprovedBy = pengurus.fldUserID, fld_ApprovedDT = timezone.gettimezone(), fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID });
                                        }
                                        disablesavebtn = true;
                                    }
                                    else
                                    {
                                        msg = "Sila hapuskan data kehadiran terdahulu sebelum meneruskan tindakkan.";
                                        statusmsg = "warning";
                                        disablesavebtn = true;
                                    }
                                }
                                else
                                {
                                    var datainkrjhdr = dbr.tbl_Kerjahdr.Where(x => x.fld_Nopkj.Trim() == CustMod_HariTerabai.SelectionData && x.fld_Tarikh == CustMod_HariTerabai.dateseleted && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).FirstOrDefault();

                                    if (datainkrjhdr == null)
                                    {
                                        var pkjdata = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == CustMod_HariTerabai.SelectionData && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1").Select(s => new { s.fld_Nopkj, s.fld_KumpulanID }).FirstOrDefault();
                                        KumpulanKod = dbr.tbl_KumpulanKerja.Where(x => x.fld_KumpulanID == pkjdata.fld_KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).Select(s => s.fld_KodKumpulan).FirstOrDefault();
                                        tbl_Kerjahdrs.Add(new tbl_Kerjahdr() { fld_Nopkj = pkjdata.fld_Nopkj, fld_Kum = KumpulanKod.Trim(), fld_Tarikh = CustMod_HariTerabai.dateseleted, fld_Kdhdct = CustMod_HariTerabai.WorkCode, fld_Hujan = CustMod_HariTerabai.Rainning, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_CreatedBy = getuserid, fld_CreatedDT = date, fld_DataSource = "B" });
                                        if (CustMod_HariTerabai.JenisChargeHT == "kong")
                                        {
                                            if (CustMod_HariTerabai.PilihanMasaHT == "separuh")
                                            {
                                                Amount = GetKadarUpah / 2;
                                                Hasil = 0.5m;
                                            }
                                            else
                                            {
                                                Amount = GetKadarUpah;
                                                Hasil = 1;
                                            }

                                            tbl_Kerjas.Add(new tbl_Kerja() { fld_Amount = Amount, fld_Bonus = 0, fld_BrtGth = 0, fld_CreatedBy = getuserid, fld_CreatedDT = timezone.gettimezone(), fld_DataSource = "B", fld_JamOT = 0, fld_JnisAktvt = GetJenisActvtDetails.fld_KodJnsAktvt, fld_JnsPkt = CustMod_HariTerabai.JnisPktHT, fld_JumlahHasil = Hasil, fld_KadarByr = GetKadarUpah, fld_KdhMenuai = "-", fld_KodAktvt = CustMod_HariTerabai.PilihanAktvtHT, fld_KodGL = "602", fld_KodPkt = CustMod_HariTerabai.PilihanPktHT, fld_Kong = 0, fld_Kum = KumpulanKod, fld_LadangID = LadangID, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_Nopkj = pkjdata.fld_Nopkj, fld_PerBrshGth = 0, fld_Quality = 0, fld_Tarikh = CustMod_HariTerabai.dateseleted, fld_Unit = GetActvty.fld_Unit, fld_HrgaKwsnSkar = 0, fld_KodKwsnSkar = "-", fld_OverallAmount = Amount });
                                        }

                                        tbl_KerjaHariTerabais.Add(new tbl_KerjaHariTerabai() { fld_Nopkj = pkjdata.fld_Nopkj, fld_Tarikh = CustMod_HariTerabai.dateseleted, fld_JenisCharge = JenisCharge, fld_MasaKerja = MasaKerja, fld_ApprovedBy = pengurus.fldUserID, fld_ApprovedDT = timezone.gettimezone(), fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID });
                                    }
                                    else
                                    {
                                        msg = "Sila hapuskan data kehadiran terdahulu sebelum meneruskan tindakkan.";
                                        statusmsg = "warning";
                                        disablesavebtn = true;
                                    }
                                }

                                if (tbl_Kerjahdrs.Count() != 0)
                                {
                                    msg = GlobalResEstate.msgAdd;
                                    statusmsg = "success";
                                    dbr.tbl_Kerjahdr.AddRange(tbl_Kerjahdrs);

                                    if (tbl_Kerjas.Count() > 0)
                                    {
                                        dbr.tbl_Kerja.AddRange(tbl_Kerjas);
                                    }
                                    dbr.tbl_KerjaHariTerabai.AddRange(tbl_KerjaHariTerabais);
                                    dbr.SaveChanges();
                                    EstateFunction.SaveDataKerjaSAP(dbr, tbl_Kerjas, NegaraID, SyarikatID, WilayahID, LadangID, GLCode, "", "");
                                }
                            }
                            else
                            {
                                msg = GlobalResEstate.msgError;
                                statusmsg = "warning";
                                disablesavebtn = true;
                            }
                        }
                        else
                        {
                            msg = "Kod GL tidak dijumpai untuk aktiviti ini.";
                            statusmsg = "warning";
                        }

                        List<CustMod_WorkerWork> CustMod_WorkerWorks = new List<CustMod_WorkerWork>();
                        List<tbl_Kerja> tbl_KerjaList = new List<tbl_Kerja>();
                        if (CustMod_HariTerabai.SelectionCategory == 1)
                        {
                            tbl_KerjaList = dbr.tbl_Kerja.Where(x => x.fld_Kum == CustMod_HariTerabai.SelectionData && x.fld_Tarikh == CustMod_HariTerabai.dateseleted && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Distinct().ToList();
                        }
                        else
                        {
                            tbl_KerjaList = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == CustMod_HariTerabai.SelectionData && x.fld_Tarikh == CustMod_HariTerabai.dateseleted && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Distinct().ToList();
                        }

                        foreach (var tbl_KerjaData in tbl_KerjaList)
                        {
                            var namepkj = EstateFunction.PkjName(dbr, NegaraID, SyarikatID, WilayahID, LadangID, tbl_KerjaData.fld_Nopkj);
                            CustMod_WorkerWorks.Add(new CustMod_WorkerWork() { fld_ID = tbl_KerjaData.fld_ID, fld_Nopkj = tbl_KerjaData.fld_Nopkj, fld_NamaPkj = namepkj, fld_Amount = tbl_KerjaData.fld_Amount, fld_JumlahHasil = tbl_KerjaData.fld_JumlahHasil, fld_KodAktvt = tbl_KerjaData.fld_KodAktvt, fld_KodGL = tbl_KerjaData.fld_KodGL, fld_KodPkt = tbl_KerjaData.fld_KodPkt, fld_Kum = tbl_KerjaData.fld_Kum, fld_Tarikh = tbl_KerjaData.fld_Tarikh, fld_JamOT = tbl_KerjaData.fld_JamOT, fld_Unit = tbl_KerjaData.fld_Unit, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_AmountOA = tbl_KerjaData.fld_OverallAmount });
                        }

                        bodyview2 = RenderRazorViewToString("WorkRecordList", CustMod_WorkerWorks, CutOfDateStatus);
                    }
                    catch (Exception ex)
                    {
                        geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                        msg = GlobalResEstate.msgError;
                        statusmsg = "warning";
                        disablesavebtn = true;
                    }
                    dbr.Dispose();
                    AuthStatus = true;
                }
                else
                {
                    AuthStatus = false;
                    msg = "ID Pengurus tidak sah.";
                    statusmsg = "warning";
                    disablesavebtn = true;
                }
            }
            else
            {
                AuthStatus = false;
                msg = "Sila masukkan ID Pengurus untuk pengesahan.";
                statusmsg = "warning";
                disablesavebtn = true;
            }

            return Json(new { proceedstatus = disablesavebtn, statusmsg, msg, ZeroLeaveBal, statusmsg2, msg2, AlertPopup, tablelisting2 = bodyview2, AuthStatus });
        }

        public ActionResult _OtherDifficulty()
        {
            return View();
        }

        [HttpPost]
        public ActionResult _OtherDifficulty(decimal? OtherDifValue, string ManagerID2, string ManagerPassword2)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            EncryptDecrypt Encrypt = new EncryptDecrypt();
            DateTime? AppDT = timezone.gettimezone();
            string msg = "";
            string statusmsg = "";
            bool AuthStatus = false;
            int AppUsrID = 0;
            string GetValidation = "3";
            if (string.IsNullOrEmpty(ManagerID2) == false && string.IsNullOrEmpty(ManagerPassword2) == false)
            {
                ManagerPassword2 = Encrypt.Encrypt(ManagerPassword2);
                var pengurus = db.tblUsers.Where(x => x.fldUserName == ManagerID2 && x.fldUserPassword == ManagerPassword2 && x.fldDeleted == false && x.fldRoleID <= 6 && x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID && x.fldWilayahID == WilayahID && x.fldLadangID == LadangID).SingleOrDefault();

                if (pengurus != null)
                {
                    AppUsrID = pengurus.fldUserID;
                    AuthStatus = true;
                    msg = "Nilai berjaya dimasukkan.";
                    statusmsg = "success";
                }
                else
                {
                    AppDT = null;
                    AuthStatus = false;
                    OtherDifValue = 0;
                    msg = "ID Pengurus tidak sah.";
                    statusmsg = "warning";
                }
            }
            else
            {
                AppDT = null;
                AuthStatus = false;
                OtherDifValue = 0;
                msg = "Sila masukkan ID Pengurus untuk pengesahan.";
                statusmsg = "warning";
            }
            return Json(new { OtherDifValue, msg, statusmsg, AuthStatus, AppUsrID, AppDT, GetValidation });
        }

        public ActionResult WorkDetail()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            List<SelectListItem> Lejar = new List<SelectListItem>();
            List<SelectListItem> JnisPkt = new List<SelectListItem>();
            List<SelectListItem> PilihanPkt = new List<SelectListItem>();
            List<SelectListItem> JnisAktvt = new List<SelectListItem>();
            List<SelectListItem> PilihanAktvt = new List<SelectListItem>();
            List<SelectListItem> Bonus = new List<SelectListItem>();

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var getJenisActvtDetails = db.tbl_JenisAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Take(1).FirstOrDefault();
            var Pkt = dbr.tbl_PktUtama.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();

            var LejarList = db.tbl_Lejar.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_KodLejar).ToList();
            string LejarSelect = LejarList.Select(s => s.fld_KodLejar).Take(1).FirstOrDefault();
            Lejar = new SelectList(LejarList.Select(s => new SelectListItem { Value = s.fld_KodLejar, Text = s.fld_KodLejar + " - " + s.fld_Desc }), "Value", "Text").ToList();
            JnisPkt = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnspkt" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            PilihanPkt = new SelectList(Pkt.Select(s => new SelectListItem { Value = s.fld_PktUtama, Text = s.fld_PktUtama + " - " + s.fld_NamaPktUtama }), "Value", "Text").ToList();
            var tbl_JenisAktiviti = db.tbl_JenisAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false && x.fld_Lejar == LejarSelect).ToList();
            JnisAktvt = new SelectList(tbl_JenisAktiviti.OrderBy(o => o.fld_KodJnsAktvt).Select(s => new SelectListItem { Value = s.fld_KodJnsAktvt, Text = s.fld_Desc }), "Value", "Text").ToList();
            PilihanAktvt = new SelectList(db.tbl_UpahAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJenisAktvt == getJenisActvtDetails.fld_KodJnsAktvt && x.fld_Deleted == false).OrderBy(o => o.fld_KodAktvt).Select(s => new SelectListItem { Value = s.fld_KodAktvt, Text = s.fld_KodAktvt }), "Value", "Text").ToList();
            PilihanAktvt.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            Bonus.Add(new SelectListItem { Text = "0", Value = "0", Selected = true });
            Bonus.Add(new SelectListItem { Text = "50", Value = "50", Selected = false });
            Bonus.Add(new SelectListItem { Text = "100", Value = "100", Selected = false });

            string CdKesukaranMenuai = Pkt.Select(s => s.fld_KesukaranMenuaiPktUtama).Take(1).FirstOrDefault();
            string CdKesukaranMembaja = Pkt.Select(s => s.fld_KesukaranMembajaPktUtama).Take(1).FirstOrDefault();
            string GetKesukaranMenuai = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "KesukaranMenuai" && x.fldOptConfValue == CdKesukaranMenuai && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fldOptConfFlag2).FirstOrDefault();
            string GetKesukaranMembaja = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "KesukaranMembaja" && x.fldOptConfValue == CdKesukaranMembaja && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fldOptConfFlag2).FirstOrDefault();

            ViewBag.JenisAktvt = tbl_JenisAktiviti.Select(s => s.fld_Desc).Take(1).FirstOrDefault();
            ViewBag.GetKesukaranMenuai = GetKesukaranMenuai;
            ViewBag.GetKesukaranMembaja = GetKesukaranMembaja;
            ViewBag.CdKesukaranMenuai = CdKesukaranMenuai;
            ViewBag.CdKesukaranMembaja = CdKesukaranMembaja;
            ViewBag.Lejar = Lejar;
            ViewBag.JnisPkt = JnisPkt;
            ViewBag.PilihanPkt = PilihanPkt;
            ViewBag.JnisAktvt = JnisAktvt;
            ViewBag.PilihanAktvt = PilihanAktvt;
            ViewBag.Bonus = Bonus;
            dbr.Dispose();
            return View();
        }

        [HttpPost]
        public ActionResult Working(DateTime SelectDate, string SelectionData, int SelectionCategory, string Lejar, byte JnisPkt, string PilihanPkt, string JnisAktvt, string PilihanAktvt, decimal? HrgaKwsnSkr, string KdKwsnSkr, int AppKwnsSkrLainID, DateTime? AppKwnsSkrLainDT, List<CustMod_Work> HadirData)
        {
            string msg = "";
            string statusmsg = "";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            int checksameactvt = 0;
            string kodkumpulan = "";
            string unitcode = "";
            decimal? kong = 0;
            string bodyview = "";
            bool CutOfDateStatus = false;
            int checkkongactvt = 0;
            string GLCode = "";
            decimal? HrgaKwsnSkr2 = HrgaKwsnSkr;
            DateTime DTCreated = timezone.gettimezone();
            List<tbl_Kerja> tbl_KerjaList = new List<tbl_Kerja>();

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            if (EstateFunction.CheckSAPGLMap(dbr, JnisPkt, PilihanPkt, PilihanAktvt, NegaraID, SyarikatID, WilayahID, LadangID, false, "-", out GLCode))
            {
                if (HadirData != null)
                {
                    CutOfDateStatus = EstateFunction.GetStatusCutProcess(dbr, SelectDate, NegaraID, SyarikatID, WilayahID, LadangID);
                    if (!CutOfDateStatus)
                    {
                        checksameactvt = SelectionCategory == 1 ? dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_Kum == SelectionData && x.fld_KodPkt == PilihanPkt && x.fld_KodAktvt == PilihanAktvt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Count()
                        :
                        dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_Nopkj == SelectionData && x.fld_KodPkt == PilihanPkt && x.fld_KodAktvt == PilihanAktvt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Count();

                        var getJenisActvtDetails = db.tbl_JenisAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_DisabledFlag == 3 && x.fld_Deleted == false).FirstOrDefault();

                        checkkongactvt = SelectionCategory == 1 ? dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_Kum == SelectionData && x.fld_JnisAktvt == getJenisActvtDetails.fld_KodJnsAktvt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Count()
                        :
                        dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_Nopkj == SelectionData && x.fld_JnisAktvt == getJenisActvtDetails.fld_KodJnsAktvt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Count();

                        if (getJenisActvtDetails.fld_KodJnsAktvt == JnisAktvt)
                        {
                            checksameactvt = SelectionCategory == 1 ? dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_Kum == SelectionData && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Count()
                            :
                            dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_Nopkj == SelectionData && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Count();
                        }

                        if (checksameactvt == 0 && HadirData.Count() != 0 && checkkongactvt == 0)
                        {
                            foreach (var datakerja in HadirData)
                            {
                                switch (datakerja.checkpurpose)
                                {
                                    case 1:

                                        break;
                                    case 2:
                                        datakerja.kdhmnuai = "-";
                                        break;
                                    case 3:
                                        datakerja.kdhmnuai = "-";
                                        datakerja.kualiti = 0;
                                        datakerja.hasil = 1;
                                        datakerja.bonus = 0;
                                        break;
                                }

                                datakerja.jumlah = datakerja.hasil == null ? datakerja.kadar : datakerja.jumlah;
                                datakerja.kdhmnuai = datakerja.kdhmnuai == null ? "-" : datakerja.kdhmnuai;
                                datakerja.kualiti = datakerja.kualiti == null ? 0 : datakerja.kualiti;
                                datakerja.hasil = datakerja.hasil == null ? 0 : datakerja.hasil;
                                datakerja.bonus = datakerja.bonus == null ? 0 : datakerja.bonus;
                                //masukkan looping checking esk
                                kodkumpulan = dbr.tbl_Kerjahdr.Where(x => x.fld_Nopkj == datakerja.nopkj && x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_Kum).FirstOrDefault();
                                unitcode = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJenisAktvt == JnisAktvt && x.fld_KodAktvt == PilihanAktvt && x.fld_Deleted == false).Select(s => s.fld_Unit).FirstOrDefault();
                                HrgaKwsnSkr = HrgaKwsnSkr2 * datakerja.hasil * datakerja.gandaankadar;
                                tbl_KerjaList.Add(new tbl_Kerja() { fld_Nopkj = datakerja.nopkj, fld_Kum = kodkumpulan, fld_Tarikh = SelectDate, fld_KodPkt = PilihanPkt, fld_Amount = datakerja.jumlah, fld_JnsPkt = JnisPkt, fld_JumlahHasil = datakerja.hasil, fld_KadarByr = datakerja.kadar, fld_KodGL = Lejar, fld_KodAktvt = PilihanAktvt, fld_JamOT = datakerja.ot, fld_DataSource = "B", fld_BrtGth = 0, fld_PerBrshGth = 0, fld_Kong = kong, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_CreatedBy = getuserid, fld_CreatedDT = DTCreated, fld_JnisAktvt = JnisAktvt, fld_KdhMenuai = datakerja.kdhmnuai, fld_Bonus = datakerja.bonus, fld_Unit = unitcode, fld_Quality = datakerja.kualiti, fld_HrgaKwsnSkar = HrgaKwsnSkr, fld_KodKwsnSkar = KdKwsnSkr, fld_ApprovalKwsnSkarDT = AppKwnsSkrLainDT, fld_ApprovalKwsnSkarLainBy = AppKwnsSkrLainID, fld_OverallAmount = datakerja.jumlahOA });
                            }
                            dbr.tbl_Kerja.AddRange(tbl_KerjaList);
                            dbr.SaveChanges();
                            EstateFunction.SaveDataKerjaSAP(dbr, tbl_KerjaList, NegaraID, SyarikatID, WilayahID, LadangID, GLCode, "", "");
                            msg = GlobalResEstate.msgAdd;
                            statusmsg = "success";
                        }
                        else
                        {
                            if (checkkongactvt >= 1)
                            {
                                msg = GlobalResEstate.msgExistKong;
                                statusmsg = "warning";
                            }
                            else
                            {
                                msg = GlobalResEstate.msgDataExist;
                                statusmsg = "warning";
                            }
                        }
                    }
                    else
                    {
                        msg = GlobalResEstate.msgError;
                        statusmsg = "warning";
                    }
                }
                else
                {
                    msg = GlobalResEstate.msgNoRecord;
                    statusmsg = "warning";
                }
            }
            else
            {
                msg = "Kod GL tidak dijumpai untuk aktiviti ini.";
                statusmsg = "warning";
            }

            bodyview = RenderRazorViewToString("WorkRecordList", EstateFunction.RecordWorkingList(dbr, SelectionCategory, SelectionData, SelectDate, NegaraID, SyarikatID, WilayahID, LadangID), false);
            dbr.Dispose();
            return Json(new { msg, statusmsg, tablelisting = bodyview });
        }

        public JsonResult DeleteAttInfo(Guid Data, int SelectionCategory, string SelectionData, DateTime SelectDate)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string msg = "";
            string statusmsg = "";
            string bodyview = "";
            string bodyview2 = "";
            bool disablesavebtn = true;
            int KumpulanID = 0;
            string namelabel = "";
            string dateformat = GetConfig.GetData("dateformat2");
            string SelectDatePassback = string.Format("{0:" + dateformat + "}", SelectDate);
            bool datakrjaproceed = false;
            bool checkhadir = false;
            bool checkxhadir = false;
            string kodhdr = "";
            string kodhjn = "";
            string namepkj = "";
            decimal? GajiTerkumpul = 0;
            List<tbl_Pkjmast> tbl_Pkjmast = new List<tbl_Pkjmast>();
            List<CustMod_Kerjahdr> CustMod_Kerjahdrs = new List<CustMod_Kerjahdr>();
            List<tbl_Kerja> tbl_KerjaList = new List<tbl_Kerja>();
            tbl_KumpulanKerja tbl_KumpulanKerja = new tbl_KumpulanKerja();
            tbl_Kerjahdr tbl_Kerjahdr = new tbl_Kerjahdr();
            List<CustMod_WorkerWork> CustMod_WorkerWorks = new List<CustMod_WorkerWork>();

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var GetKerjaHdr = dbr.tbl_Kerjahdr.Find(Data);

            if (EstateFunction.IndividuCheckLeaveTake(GetKerjaHdr.fld_Kdhdct, NegaraID, SyarikatID))
            {
                EstateFunction.LeaveAdd(dbr, GetKerjaHdr.fld_Tarikh.Value.Year, GetKerjaHdr.fld_Nopkj, GetKerjaHdr.fld_Kdhdct, NegaraID, SyarikatID, WilayahID, LadangID);
            }

            if (GetKerjaHdr != null)
            {
                var GetKerja = dbr.tbl_Kerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh == GetKerjaHdr.fld_Tarikh && x.fld_Nopkj == GetKerjaHdr.fld_Nopkj).ToList();
                if (GetKerja.Count > 0)
                {
                    dbr.tbl_Kerja.RemoveRange(GetKerja);
                }
                dbr.tbl_Kerjahdr.Remove(GetKerjaHdr);
                dbr.SaveChanges();
                msg = GlobalResEstate.msgDelete2;
                statusmsg = "success";
            }

            if (SelectionCategory == 1)
            {
                //check kehadiran
                KumpulanID = dbr.tbl_KumpulanKerja.Where(x => x.fld_KodKumpulan.Trim() == SelectionData.Trim() && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_KumpulanID).FirstOrDefault();
                var datainpkjmast = dbr.tbl_Pkjmast.Where(x => x.fld_KumpulanID == KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").Select(s => s.fld_Nopkj).ToList();
                var datainkrjhdr = dbr.tbl_Kerjahdr.Join(dbr.tbl_Pkjmast, j => j.fld_Nopkj, k => k.fld_Nopkj, (j, k) => new { j.fld_Kum, j.fld_Tarikh, j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, k.fld_StatusApproved, j.fld_Nopkj }).Where(x => x.fld_Kum.Trim() == SelectionData && x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1).Select(s => s.fld_Nopkj).ToList();
                if (datainkrjhdr.Count() > 0)
                {
                    List<string> datainpkjmastexldatainkrjhdrs = datainpkjmast.Except(datainkrjhdr).ToList();

                    var pkjmasts1 = dbr.tbl_Pkjmast.Where(x => datainpkjmastexldatainkrjhdrs.Contains(x.fld_Nopkj) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nopkj).ToList();

                    var pkjmasts2 = dbr.tbl_Pkjmast.Where(x => datainkrjhdr.Contains(x.fld_Nopkj) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nopkj).ToList();

                    foreach (var pkjmast1 in pkjmasts1)
                    {
                        tbl_KumpulanKerja = dbr.tbl_KumpulanKerja.Where(x => x.fld_KumpulanID == pkjmast1.fld_KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).FirstOrDefault();
                        CustMod_Kerjahdrs.Add(new CustMod_Kerjahdr() { fld_Nopkj = pkjmast1.fld_Nopkj, fld_Nama = pkjmast1.fld_Nama, fld_Kum = tbl_KumpulanKerja.fld_KodKumpulan, fld_Status = "Tiada rekod", fld_HdrCt = "-", fld_Hujan = "-", fld_GajiTerkumpul = "-", fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID });
                        namelabel = tbl_KumpulanKerja.fld_Keterangan;
                        checkxhadir = true;
                    }

                    foreach (var pkjmast2 in pkjmasts2)
                    {
                        tbl_Kerjahdr = dbr.tbl_Kerjahdr.Where(x => x.fld_Kum.Trim() == SelectionData && x.fld_Tarikh == SelectDate && x.fld_Nopkj == pkjmast2.fld_Nopkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).FirstOrDefault();
                        tbl_KumpulanKerja = dbr.tbl_KumpulanKerja.Where(x => x.fld_KumpulanID == pkjmast2.fld_KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).FirstOrDefault();
                        GajiTerkumpul = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == pkjmast2.fld_Nopkj && x.fld_Kum == tbl_KumpulanKerja.fld_KodKumpulan && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == SelectDate.Month && x.fld_Tarikh.Value.Year == SelectDate.Year).Sum(s => s.fld_OverallAmount);
                        GajiTerkumpul = GajiTerkumpul == null ? 0 : GajiTerkumpul;
                        CustMod_Kerjahdrs.Add(new CustMod_Kerjahdr() { fld_Nopkj = pkjmast2.fld_Nopkj, fld_Nama = pkjmast2.fld_Nama, fld_Kum = tbl_KumpulanKerja.fld_KodKumpulan, fld_Status = "Ada rekod", fld_HdrCt = GetConfig.GetWebConfigDesc(tbl_Kerjahdr.fld_Kdhdct, "cuti", (int)NegaraID, (int)SyarikatID), fld_Hujan = tbl_Kerjahdr.fld_Hujan == 0 ? "Tidak" : "Ya", fld_CreatedBy = getidentity.Username2(tbl_Kerjahdr.fld_CreatedBy), fld_CreatedDT = tbl_Kerjahdr.fld_CreatedDT, fld_UniqueID = tbl_Kerjahdr.fld_UniqueID, fld_GajiTerkumpul = GajiTerkumpul.ToString(), fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID });
                        namelabel = tbl_KumpulanKerja.fld_Keterangan;
                        kodhdr = tbl_Kerjahdr.fld_Kdhdct;
                        kodhjn = tbl_Kerjahdr.fld_Hujan.ToString();
                        checkhadir = true;
                    }

                    if (checkxhadir && checkhadir)
                    {
                        datakrjaproceed = false;
                    }
                    else if (!checkxhadir && checkhadir)
                    {
                        datakrjaproceed = true;
                    }
                    else if (checkxhadir && !checkhadir)
                    {
                        datakrjaproceed = false;
                    }
                    else
                    {
                        datakrjaproceed = false;
                    }

                    disablesavebtn = true;
                }
                else
                {
                    var pkjmasts1 = dbr.tbl_Pkjmast.Where(x => datainpkjmast.Contains(x.fld_Nopkj) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nopkj).ToList();
                    foreach (var pkjmast1 in pkjmasts1)
                    {
                        tbl_KumpulanKerja = dbr.tbl_KumpulanKerja.Where(x => x.fld_KumpulanID == pkjmast1.fld_KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).FirstOrDefault();
                        CustMod_Kerjahdrs.Add(new CustMod_Kerjahdr() { fld_Nopkj = pkjmast1.fld_Nopkj, fld_Nama = pkjmast1.fld_Nama, fld_Kum = tbl_KumpulanKerja.fld_KodKumpulan, fld_Status = "Tiada rekod", fld_HdrCt = "-", fld_Hujan = "-", fld_GajiTerkumpul = "-", fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID });
                        namelabel = tbl_KumpulanKerja.fld_Keterangan;
                    }
                    disablesavebtn = false;
                    datakrjaproceed = false;
                    checkxhadir = true;
                }

                //check kerja
                tbl_KerjaList = dbr.tbl_Kerja.Where(x => x.fld_Kum == SelectionData && x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Distinct().ToList();

            }
            else
            {
                var datainpkjmast = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == SelectionData && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").FirstOrDefault();
                var datainkrjhdr = dbr.tbl_Kerjahdr.Join(dbr.tbl_Pkjmast, j => j.fld_Nopkj, k => k.fld_Nopkj, (j, k) => new { j.fld_Kum, j.fld_Tarikh, j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, k.fld_StatusApproved, j.fld_Nopkj, j.fld_Kdhdct, j.fld_Hujan, j.fld_CreatedBy, j.fld_CreatedDT }).Where(x => x.fld_Nopkj.Trim() == SelectionData && x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1).FirstOrDefault();
                //var datainkrjhdr = dbr.tbl_Kerjahdr.Where(x => x.fld_Nopkj.Trim() == SelectionData && x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).FirstOrDefault();
                tbl_KumpulanKerja = dbr.tbl_KumpulanKerja.Where(x => x.fld_KumpulanID == datainpkjmast.fld_KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).FirstOrDefault();
                if (datainkrjhdr != null)
                {
                    GajiTerkumpul = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == datainkrjhdr.fld_Nopkj && x.fld_Kum == tbl_KumpulanKerja.fld_KodKumpulan && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == SelectDate.Month && x.fld_Tarikh.Value.Year == SelectDate.Year).Sum(s => s.fld_OverallAmount);
                    GajiTerkumpul = GajiTerkumpul == null ? 0 : GajiTerkumpul;
                    CustMod_Kerjahdrs.Add(new CustMod_Kerjahdr() { fld_Nopkj = datainpkjmast.fld_Nopkj, fld_Nama = datainpkjmast.fld_Nama, fld_Kum = tbl_KumpulanKerja.fld_KodKumpulan, fld_Status = "Ada rekod", fld_HdrCt = GetConfig.GetWebConfigDesc(datainkrjhdr.fld_Kdhdct, "cuti", (int)NegaraID, (int)SyarikatID), fld_Hujan = datainkrjhdr.fld_Hujan == 0 ? "Tidak" : "Ya", fld_CreatedBy = getidentity.Username2(datainkrjhdr.fld_CreatedBy), fld_CreatedDT = datainkrjhdr.fld_CreatedDT, fld_GajiTerkumpul = GajiTerkumpul.ToString(), fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID });
                    kodhdr = datainkrjhdr.fld_Kdhdct;
                    kodhjn = datainkrjhdr.fld_Hujan.ToString();
                    disablesavebtn = true;
                    datakrjaproceed = true;
                    checkhadir = true;
                }
                else
                {
                    CustMod_Kerjahdrs.Add(new CustMod_Kerjahdr() { fld_Nopkj = datainpkjmast.fld_Nopkj, fld_Nama = datainpkjmast.fld_Nama, fld_Kum = tbl_KumpulanKerja.fld_KodKumpulan, fld_Tarikh = SelectDate, fld_Status = "Tiada rekod", fld_HdrCt = "-", fld_Hujan = "-", fld_GajiTerkumpul = "-", fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID });
                    namelabel = datainpkjmast.fld_Nama;
                    disablesavebtn = false;
                    datakrjaproceed = false;
                    checkxhadir = true;
                }
                namelabel = datainpkjmast.fld_Nama;

                tbl_KerjaList = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == SelectionData && x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Distinct().ToList();
            }

            foreach (var tbl_KerjaData in tbl_KerjaList)
            {
                namepkj = EstateFunction.PkjName(dbr, NegaraID, SyarikatID, WilayahID, LadangID, tbl_KerjaData.fld_Nopkj);
                CustMod_WorkerWorks.Add(new CustMod_WorkerWork() { fld_ID = tbl_KerjaData.fld_ID, fld_Nopkj = tbl_KerjaData.fld_Nopkj, fld_NamaPkj = namepkj, fld_Amount = tbl_KerjaData.fld_Amount, fld_JumlahHasil = tbl_KerjaData.fld_JumlahHasil, fld_KodAktvt = tbl_KerjaData.fld_KodAktvt, fld_KodGL = tbl_KerjaData.fld_KodGL, fld_KodPkt = tbl_KerjaData.fld_KodPkt, fld_Kum = tbl_KerjaData.fld_Kum, fld_Tarikh = tbl_KerjaData.fld_Tarikh, fld_JamOT = tbl_KerjaData.fld_JamOT, fld_Unit = tbl_KerjaData.fld_Unit, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_AmountOA = tbl_KerjaData.fld_OverallAmount });
            }

            bodyview = RenderRazorViewToString("WorkerListDetailsCheck", CustMod_Kerjahdrs, NegaraID, SyarikatID, false);
            bodyview2 = RenderRazorViewToString("WorkRecordList", CustMod_WorkerWorks, false);

            string dayname = "";
            int getday = (int)SelectDate.DayOfWeek;
            dayname = GetTriager.getDayName(getday);
            dbr.Dispose();



            return Json(new { statusmsg, msg, tablelisting = bodyview, tablelisting2 = bodyview2, dayname, proceedstatus = disablesavebtn, namelabel = namelabel + " - " + SelectDatePassback, datakrjaproceed, kodhdr, kodhjn, checkhadir, checkxhadir });
        }

        public ActionResult DeleteWorkInfo(DateTime SelectDate, string SelectionData, int SelectionCategory, string pkt, string kodatvt)
        {
            string msg = "";
            string statusmsg = "";
            string bodyview = "";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            if (SelectionCategory == 1)
            {
                var deleteworkerinfo = dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_Kum == SelectionData && x.fld_KodPkt == pkt && x.fld_KodAktvt == kodatvt).ToList();
                dbr.tbl_Kerja.RemoveRange(deleteworkerinfo);
                dbr.SaveChanges();
            }
            else
            {
                var deleteworkerinfo = dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_Nopkj == SelectionData && x.fld_KodPkt == pkt && x.fld_KodAktvt == kodatvt).ToList();
                dbr.tbl_Kerja.RemoveRange(deleteworkerinfo);
                dbr.SaveChanges();
            }
            msg = GlobalResEstate.msgDelete2;
            statusmsg = "success";
            bodyview = RenderRazorViewToString("WorkRecordList", EstateFunction.RecordWorkingList(dbr, SelectionCategory, SelectionData, SelectDate, NegaraID, SyarikatID, WilayahID, LadangID), false);
            dbr.Dispose();
            return Json(new { msg, statusmsg, tablelisting = bodyview });
        }

        public ActionResult ActivityCodeDetails(string ActivityType)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            var CodeActivt = db.tbl_UpahAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJenisAktvt == ActivityType).OrderBy(o => o.fld_KodAktvt).ToList();

            ViewBag.ActvtType = db.tbl_JenisAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJnsAktvt == ActivityType).Select(s => s.fld_Desc).FirstOrDefault();

            return View(CodeActivt);
        }

        public JsonResult WorkerData(int SelectionCategory)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> SelectionData = new List<SelectListItem>();
            SelectionData = SelectionCategory == 1 ?
            new SelectList(dbr.tbl_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).Select(s => new SelectListItem { Value = s.fld_KodKumpulan, Text = s.fld_KodKumpulan + " - " + s.fld_Keterangan }), "Value", "Text").ToList()
            :
            new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1" && x.fld_KumpulanID != null).Select(s => new SelectListItem { Value = s.fld_Nopkj.ToString(), Text = s.fld_Nopkj + " - " + s.fld_Nama }), "Value", "Text").ToList();
            SelectionData.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            dbr.Dispose();
            return Json(SelectionData);
        }

        public JsonResult WorkerAvlbleChecking(int SelectionCategory, string SelectionData)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string msg = "";
            string statusmsg = "";
            string bodyview = "";
            bool disablesavebtn = true;
            bool datedisable = true;
            string namelabel = "";
            List<tbl_Pkjmast> tbl_Pkjmast = new List<tbl_Pkjmast>();
            List<CustMod_Kerjahdr> CustMod_Kerjahdrs = new List<CustMod_Kerjahdr>();
            tbl_KumpulanKerja tbl_KumpulanKerja = new tbl_KumpulanKerja();

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            if (SelectionCategory == 1)
            {
                tbl_KumpulanKerja = dbr.tbl_KumpulanKerja.Where(x => x.fld_KodKumpulan.Trim() == SelectionData.Trim() && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).FirstOrDefault();
                tbl_Pkjmast = dbr.tbl_Pkjmast.Where(x => x.fld_KumpulanID == tbl_KumpulanKerja.fld_KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nopkj).ToList();
                if (tbl_Pkjmast.Count() == 0)
                {
                    msg = GlobalResEstate.msgNoRecord; //tiada pekerja dalam kumpulan ini
                    statusmsg = "warning";
                    disablesavebtn = true;
                    datedisable = true;
                }
                else
                {
                    msg = GlobalResEstate.msgWorkerFound; //ada pekerja dalam kumpulan ini
                    statusmsg = "success";
                    disablesavebtn = false;
                    datedisable = false;
                }
                namelabel = tbl_KumpulanKerja.fld_Keterangan;
            }
            else
            {
                tbl_Pkjmast = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == SelectionData && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").ToList();
                msg = GlobalResEstate.msgWorkerFound; //ada pekerja dalam kumpulan ini
                statusmsg = "success";
                disablesavebtn = false;
                datedisable = false;
            }

            foreach (var pkjmast1 in tbl_Pkjmast)
            {
                tbl_KumpulanKerja = dbr.tbl_KumpulanKerja.Where(x => x.fld_KumpulanID == pkjmast1.fld_KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).FirstOrDefault();
                if (tbl_KumpulanKerja != null)
                {
                    datedisable = false;
                    CustMod_Kerjahdrs.Add(new CustMod_Kerjahdr() { fld_Nopkj = pkjmast1.fld_Nopkj, fld_Nama = pkjmast1.fld_Nama, fld_Kum = tbl_KumpulanKerja.fld_KodKumpulan, fld_Status = "-", fld_HdrCt = "-", fld_Hujan = "-", fld_GajiTerkumpul = "-", fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID });
                }
                else
                {
                    msg = GlobalResEstate.msgWorkerGroup; //ada pekerja dalam kumpulan ini
                    statusmsg = "warning";
                    datedisable = true;
                    CustMod_Kerjahdrs.Add(new CustMod_Kerjahdr() { fld_Nopkj = pkjmast1.fld_Nopkj, fld_Nama = pkjmast1.fld_Nama, fld_Kum = "Tiada Kumpulan", fld_Status = "-", fld_HdrCt = "-", fld_Hujan = "-", fld_GajiTerkumpul = "-", fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID });
                }
                if (SelectionCategory != 1)
                {
                    namelabel = pkjmast1.fld_Nama;
                }
            }

            bodyview = RenderRazorViewToString("WorkerListDetailsCheck", CustMod_Kerjahdrs, NegaraID, SyarikatID, false);
            dbr.Dispose();
            return Json(new { statusmsg, msg, tablelisting = bodyview, proceedstatus = disablesavebtn, namelabel, datedisable });
        }

        public JsonResult WorkerDateChecking(int SelectionCategory, string SelectionData, DateTime SelectDate)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string msg = "";
            string statusmsg = "";
            string bodyview = "";
            string bodyview2 = "";
            bool disablesavebtn = true;
            int KumpulanID = 0;
            string namelabel = "";
            string dateformat = GetConfig.GetData("dateformat2");
            string SelectDatePassback = string.Format("{0:" + dateformat + "}", SelectDate);
            bool datakrjaproceed = false;
            bool checkhadir = false;
            bool checkxhadir = false;
            string kodhdr = "";
            string kodhjn = "";
            string namepkj = "";
            decimal? GajiTerkumpul = 0;
            bool CutOfDateStatus = false;
            string HariTerabaiStatus = "";
            List<tbl_Pkjmast> tbl_Pkjmast = new List<tbl_Pkjmast>();
            List<CustMod_Kerjahdr> CustMod_Kerjahdrs = new List<CustMod_Kerjahdr>();
            List<tbl_Kerja> tbl_KerjaList = new List<tbl_Kerja>();
            tbl_KumpulanKerja tbl_KumpulanKerja = new tbl_KumpulanKerja();
            tbl_Kerjahdr tbl_Kerjahdr = new tbl_Kerjahdr();
            List<CustMod_WorkerWork> CustMod_WorkerWorks = new List<CustMod_WorkerWork>();

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            if (SelectionCategory == 1)
            {
                //check kehadiran
                KumpulanID = dbr.tbl_KumpulanKerja.Where(x => x.fld_KodKumpulan.Trim() == SelectionData.Trim() && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).Select(s => s.fld_KumpulanID).FirstOrDefault();
                var datainpkjmast = dbr.tbl_Pkjmast.Where(x => x.fld_KumpulanID == KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").Select(s => s.fld_Nopkj.Trim()).ToList();
                var datainkrjhdr = dbr.tbl_Kerjahdr.Join(dbr.tbl_Pkjmast, j => new { j.fld_Nopkj, j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID }, k => new { k.fld_Nopkj, k.fld_NegaraID, k.fld_SyarikatID, k.fld_WilayahID, k.fld_LadangID }, (j, k) => new { j.fld_Kum, j.fld_Tarikh, j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, k.fld_StatusApproved, j.fld_Nopkj }).Where(x => x.fld_Kum.Trim() == SelectionData && x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1).Select(s => s.fld_Nopkj.Trim()).ToList();
                if (datainkrjhdr.Count() > 0)
                {
                    List<string> datainpkjmastexldatainkrjhdrs = datainpkjmast.Except(datainkrjhdr).ToList();

                    var pkjmasts1 = dbr.tbl_Pkjmast.Where(x => datainpkjmastexldatainkrjhdrs.Contains(x.fld_Nopkj) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nopkj).ToList();

                    var pkjmasts2 = dbr.tbl_Pkjmast.Where(x => datainkrjhdr.Contains(x.fld_Nopkj) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nopkj).ToList();

                    foreach (var pkjmast1 in pkjmasts1)
                    {
                        tbl_KumpulanKerja = dbr.tbl_KumpulanKerja.Where(x => x.fld_KumpulanID == pkjmast1.fld_KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).FirstOrDefault();
                        GajiTerkumpul = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == pkjmast1.fld_Nopkj && x.fld_Kum == tbl_KumpulanKerja.fld_KodKumpulan && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == SelectDate.Month && x.fld_Tarikh.Value.Year == SelectDate.Year).Sum(s => s.fld_OverallAmount);
                        GajiTerkumpul = GajiTerkumpul == null ? 0 : GajiTerkumpul;
                        CustMod_Kerjahdrs.Add(new CustMod_Kerjahdr() { fld_Nopkj = pkjmast1.fld_Nopkj, fld_Nama = pkjmast1.fld_Nama, fld_Kum = tbl_KumpulanKerja.fld_KodKumpulan, fld_Status = "Tiada rekod", fld_HdrCt = "-", fld_Hujan = "-", fld_GajiTerkumpul = GajiTerkumpul.ToString(), fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_Tarikh = SelectDate });
                        namelabel = tbl_KumpulanKerja.fld_Keterangan;
                        checkxhadir = true;
                    }

                    foreach (var pkjmast2 in pkjmasts2)
                    {
                        tbl_Kerjahdr = dbr.tbl_Kerjahdr.Where(x => x.fld_Kum.Trim() == SelectionData && x.fld_Tarikh == SelectDate && x.fld_Nopkj == pkjmast2.fld_Nopkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).FirstOrDefault();
                        tbl_KumpulanKerja = dbr.tbl_KumpulanKerja.Where(x => x.fld_KumpulanID == pkjmast2.fld_KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).FirstOrDefault();
                        GajiTerkumpul = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == pkjmast2.fld_Nopkj && x.fld_Kum == tbl_KumpulanKerja.fld_KodKumpulan && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == SelectDate.Month && x.fld_Tarikh.Value.Year == SelectDate.Year).Sum(s => s.fld_OverallAmount);
                        GajiTerkumpul = GajiTerkumpul == null ? 0 : GajiTerkumpul;
                        HariTerabaiStatus = tbl_Kerjahdr.fld_Hujan == 0 ? "Tidak" : "Ya - " + EstateFunction.GetHariTerabaiJnsCharge(dbr, pkjmast2.fld_Nopkj, SelectDate, NegaraID, SyarikatID, WilayahID, LadangID).ToUpper();
                        CustMod_Kerjahdrs.Add(new CustMod_Kerjahdr() { fld_Nopkj = pkjmast2.fld_Nopkj, fld_Nama = pkjmast2.fld_Nama, fld_Kum = tbl_KumpulanKerja.fld_KodKumpulan, fld_Status = "Ada rekod", fld_HdrCt = GetConfig.GetWebConfigDesc(tbl_Kerjahdr.fld_Kdhdct, "cuti", (int)NegaraID, (int)SyarikatID), fld_Hujan = HariTerabaiStatus, fld_CreatedBy = getidentity.Username2(tbl_Kerjahdr.fld_CreatedBy), fld_CreatedDT = tbl_Kerjahdr.fld_CreatedDT, fld_UniqueID = tbl_Kerjahdr.fld_UniqueID, fld_GajiTerkumpul = GajiTerkumpul.ToString(), fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_Tarikh = SelectDate });
                        namelabel = tbl_KumpulanKerja.fld_Keterangan;
                        kodhdr = tbl_Kerjahdr.fld_Kdhdct;
                        kodhjn = tbl_Kerjahdr.fld_Hujan.ToString();
                        checkhadir = true;
                    }

                    if (checkxhadir && checkhadir)
                    {
                        datakrjaproceed = false;
                    }
                    else if (!checkxhadir && checkhadir)
                    {
                        datakrjaproceed = true;
                    }
                    else if (checkxhadir && !checkhadir)
                    {
                        datakrjaproceed = false;
                    }
                    else
                    {
                        datakrjaproceed = false;
                    }

                    msg = GlobalResEstate.msgDataExist;
                    statusmsg = "warning";
                    disablesavebtn = true;
                }
                else
                {
                    var pkjmasts1 = dbr.tbl_Pkjmast.Where(x => datainpkjmast.Contains(x.fld_Nopkj) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nopkj).ToList();
                    foreach (var pkjmast1 in pkjmasts1)
                    {
                        tbl_KumpulanKerja = dbr.tbl_KumpulanKerja.Where(x => x.fld_KumpulanID == pkjmast1.fld_KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).FirstOrDefault();
                        GajiTerkumpul = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == pkjmast1.fld_Nopkj && x.fld_Kum == tbl_KumpulanKerja.fld_KodKumpulan && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == SelectDate.Month && x.fld_Tarikh.Value.Year == SelectDate.Year).Sum(s => s.fld_OverallAmount);
                        GajiTerkumpul = GajiTerkumpul == null ? 0 : GajiTerkumpul;
                        CustMod_Kerjahdrs.Add(new CustMod_Kerjahdr() { fld_Nopkj = pkjmast1.fld_Nopkj, fld_Nama = pkjmast1.fld_Nama, fld_Kum = tbl_KumpulanKerja.fld_KodKumpulan, fld_Status = "Tiada rekod", fld_HdrCt = "-", fld_Hujan = "-", fld_GajiTerkumpul = GajiTerkumpul.ToString(), fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_Tarikh = SelectDate });
                        namelabel = tbl_KumpulanKerja.fld_Keterangan;
                    }
                    msg = GlobalResEstate.msgNoRecord; //ada pekerja dalam kumpulan ini
                    statusmsg = "success";
                    disablesavebtn = false;
                    datakrjaproceed = false;
                    checkxhadir = true;
                }

                //check kerja
                tbl_KerjaList = dbr.tbl_Kerja.Where(x => x.fld_Kum == SelectionData && x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Distinct().ToList();

            }
            else
            {
                var datainpkjmast = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == SelectionData && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").FirstOrDefault();
                var datainkrjhdr = dbr.tbl_Kerjahdr.Join(dbr.tbl_Pkjmast, j => j.fld_Nopkj, k => k.fld_Nopkj, (j, k) => new { j.fld_Kum, j.fld_Tarikh, j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, k.fld_StatusApproved, j.fld_Nopkj, j.fld_Kdhdct, j.fld_Hujan, j.fld_CreatedBy, j.fld_CreatedDT, j.fld_UniqueID }).Where(x => x.fld_Nopkj.Trim() == SelectionData && x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1).FirstOrDefault();
                //var datainkrjhdr = dbr.tbl_Kerjahdr.Where(x => x.fld_Nopkj.Trim() == SelectionData && x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).FirstOrDefault();
                tbl_KumpulanKerja = dbr.tbl_KumpulanKerja.Where(x => x.fld_KumpulanID == datainpkjmast.fld_KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).FirstOrDefault();
                if (datainkrjhdr != null)
                {
                    GajiTerkumpul = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == datainpkjmast.fld_Nopkj && x.fld_Kum == tbl_KumpulanKerja.fld_KodKumpulan && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == SelectDate.Month && x.fld_Tarikh.Value.Year == SelectDate.Year).Sum(s => s.fld_OverallAmount);
                    GajiTerkumpul = GajiTerkumpul == null ? 0 : GajiTerkumpul;
                    HariTerabaiStatus = datainkrjhdr.fld_Hujan == 0 ? "Tidak" : "Ya - " + EstateFunction.GetHariTerabaiJnsCharge(dbr, datainkrjhdr.fld_Nopkj, SelectDate, NegaraID, SyarikatID, WilayahID, LadangID).ToUpper();
                    CustMod_Kerjahdrs.Add(new CustMod_Kerjahdr() { fld_Nopkj = datainpkjmast.fld_Nopkj, fld_Nama = datainpkjmast.fld_Nama, fld_Kum = tbl_KumpulanKerja.fld_KodKumpulan, fld_Status = "Ada rekod", fld_HdrCt = GetConfig.GetWebConfigDesc(datainkrjhdr.fld_Kdhdct, "cuti", (int)NegaraID, (int)SyarikatID), fld_Hujan = HariTerabaiStatus, fld_CreatedBy = getidentity.Username2(datainkrjhdr.fld_CreatedBy), fld_CreatedDT = datainkrjhdr.fld_CreatedDT, fld_GajiTerkumpul = GajiTerkumpul.ToString(), fld_UniqueID = datainkrjhdr.fld_UniqueID, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_Tarikh = SelectDate });
                    kodhdr = datainkrjhdr.fld_Kdhdct;
                    kodhjn = datainkrjhdr.fld_Hujan.ToString();
                    msg = GlobalResEstate.msgDataExist;
                    statusmsg = "warning";
                    disablesavebtn = true;
                    datakrjaproceed = true;
                    checkhadir = true;
                }
                else
                {
                    GajiTerkumpul = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == datainpkjmast.fld_Nopkj && x.fld_Kum == tbl_KumpulanKerja.fld_KodKumpulan && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == SelectDate.Month && x.fld_Tarikh.Value.Year == SelectDate.Year).Sum(s => s.fld_OverallAmount);
                    GajiTerkumpul = GajiTerkumpul == null ? 0 : GajiTerkumpul;
                    CustMod_Kerjahdrs.Add(new CustMod_Kerjahdr() { fld_Nopkj = datainpkjmast.fld_Nopkj, fld_Nama = datainpkjmast.fld_Nama, fld_Kum = tbl_KumpulanKerja.fld_KodKumpulan, fld_Tarikh = SelectDate, fld_Status = "Tiada rekod", fld_HdrCt = "-", fld_Hujan = "-", fld_GajiTerkumpul = GajiTerkumpul.ToString(), fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID });
                    namelabel = datainpkjmast.fld_Nama;
                    msg = GlobalResEstate.msgNoRecord;
                    statusmsg = "success";
                    disablesavebtn = false;
                    datakrjaproceed = false;
                    checkxhadir = true;
                }
                namelabel = datainpkjmast.fld_Nama;

                tbl_KerjaList = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == SelectionData && x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Distinct().ToList();
            }

            foreach (var tbl_KerjaData in tbl_KerjaList)
            {
                namepkj = EstateFunction.PkjName(dbr, NegaraID, SyarikatID, WilayahID, LadangID, tbl_KerjaData.fld_Nopkj);
                CustMod_WorkerWorks.Add(new CustMod_WorkerWork() { fld_ID = tbl_KerjaData.fld_ID, fld_Nopkj = tbl_KerjaData.fld_Nopkj, fld_NamaPkj = namepkj, fld_Amount = tbl_KerjaData.fld_Amount, fld_JumlahHasil = tbl_KerjaData.fld_JumlahHasil, fld_KodAktvt = tbl_KerjaData.fld_KodAktvt, fld_KodGL = tbl_KerjaData.fld_KodGL, fld_KodPkt = tbl_KerjaData.fld_KodPkt, fld_Kum = tbl_KerjaData.fld_Kum, fld_Tarikh = tbl_KerjaData.fld_Tarikh, fld_JamOT = tbl_KerjaData.fld_JamOT, fld_Unit = tbl_KerjaData.fld_Unit, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_AmountOA = tbl_KerjaData.fld_OverallAmount });
            }

            CutOfDateStatus = EstateFunction.GetStatusCutProcess(dbr, SelectDate, NegaraID, SyarikatID, WilayahID, LadangID);

            bodyview = RenderRazorViewToString("WorkerListDetailsCheck", CustMod_Kerjahdrs, NegaraID, SyarikatID, CutOfDateStatus);
            bodyview2 = RenderRazorViewToString("WorkRecordList", CustMod_WorkerWorks, CutOfDateStatus);

            string dayname = "";
            int getday = (int)SelectDate.DayOfWeek;
            dayname = GetTriager.getDayName(getday);
            dbr.Dispose();

            int? LadangNegeriCode = int.Parse(GetLadang.GetLadangNegeriCode(LadangID));
            string AttCodeType = EstateFunction.GetAttType(NegaraID, SyarikatID, WilayahID, LadangID, SelectDate);

            return Json(new { statusmsg, msg, tablelisting = bodyview, tablelisting2 = bodyview2, dayname, proceedstatus = disablesavebtn, namelabel = namelabel + " - " + SelectDatePassback, datakrjaproceed, kodhdr, kodhjn, checkhadir, checkxhadir, CutOfDateStatus, AttCodeType });
        }

        public JsonResult GetAttandanceDetails(int SelectionCategory, string SelectionData)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            int year = timezone.gettimezone().Year;
            int month = timezone.gettimezone().Month;
            string bodyview = "";
            List<vw_Kerjahdr> vw_Kerjahdr = new List<vw_Kerjahdr>();
            List<CustMod_Kerjahdrgroup> CustMod_Kerjahdrgroups = new List<CustMod_Kerjahdrgroup>();

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            if (SelectionCategory == 1)
            {
                var trkhkjrhdrs = dbr.vw_Kerjahdr.Where(x => x.fld_Kum == SelectionData && x.fld_Tarikh.Value.Month == month && x.fld_Tarikh.Value.Year == year && x.fld_StatusApproved == 1).OrderBy(o => o.fld_Tarikh).Select(s => s.fld_Tarikh).Distinct().ToList();

                foreach (var trkhkjrhdr in trkhkjrhdrs)
                {
                    CustMod_Kerjahdrgroups.Add(new CustMod_Kerjahdrgroup() { fld_KodKumpulan = SelectionData, fld_Tarikh = trkhkjrhdr });
                }

                bodyview = RenderRazorViewToString("GroupAttandanceDetails", CustMod_Kerjahdrgroups, false);
            }
            else
            {
                vw_Kerjahdr = dbr.vw_Kerjahdr.Where(x => x.fld_Nopkj == SelectionData && x.fld_Tarikh.Value.Month == month && x.fld_Tarikh.Value.Year == year && x.fld_StatusApproved == 1).OrderBy(o => o.fld_Tarikh).ToList();
                bodyview = RenderRazorViewToString("IndividuAttandanceDetails", vw_Kerjahdr, false);
            }
            dbr.Dispose();
            return Json(new { tablelisting2 = bodyview });
        }

        public JsonResult GetWorkCodeStatus(string WorkCode)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            var GetWorkCodeStatus = db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "cuti" && x.fldOptConfValue == WorkCode && x.fldDeleted == false).FirstOrDefault();

            db.Dispose();

            return Json(new { workstatus = GetWorkCodeStatus.fldOptConfFlag2 });
        }

        public JsonResult GetPkt(int JnisPkt)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> PilihPeringkat = new List<SelectListItem>();
            string hargaKesukaranMembaja = "";
            string hargaKesukaranMenuai = "";
            string CdKesukaranMembaja = "";
            string CdKesukaranMenuai = "";
            switch (JnisPkt)
            {
                case 1:
                    var SelectPkt = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
                    PilihPeringkat = new SelectList(SelectPkt.Select(s => new SelectListItem { Value = s.fld_PktUtama, Text = s.fld_PktUtama + " - " + s.fld_NamaPktUtama }), "Value", "Text").ToList();
                    CdKesukaranMembaja = SelectPkt.Select(s => s.fld_KesukaranMembajaPktUtama).Take(1).FirstOrDefault();
                    CdKesukaranMenuai = SelectPkt.Select(s => s.fld_KesukaranMenuaiPktUtama).Take(1).FirstOrDefault();
                    hargaKesukaranMembaja = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "KesukaranMembaja" && x.fldOptConfValue == CdKesukaranMembaja && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fldOptConfFlag2).FirstOrDefault(); ;
                    hargaKesukaranMenuai = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "KesukaranMenuai" && x.fldOptConfValue == CdKesukaranMenuai && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fldOptConfFlag2).FirstOrDefault();
                    break;
                case 2:
                    var SelectPkt2 = dbr.tbl_SubPkt.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
                    PilihPeringkat = new SelectList(SelectPkt2.Select(s => new SelectListItem { Value = s.fld_Pkt, Text = s.fld_Pkt + " - " + s.fld_NamaPkt }), "Value", "Text").ToList();
                    CdKesukaranMembaja = SelectPkt2.Select(s => s.fld_KesukaranMembajaPkt).Take(1).FirstOrDefault();
                    CdKesukaranMenuai = SelectPkt2.Select(s => s.fld_KesukaranMenuaiPkt).Take(1).FirstOrDefault();
                    hargaKesukaranMembaja = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "KesukaranMembaja" && x.fldOptConfValue == CdKesukaranMembaja && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fldOptConfFlag2).FirstOrDefault(); ;
                    hargaKesukaranMenuai = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "KesukaranMenuai" && x.fldOptConfValue == CdKesukaranMenuai && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fldOptConfFlag2).FirstOrDefault();
                    break;
                case 3:
                    var SelectPkt3 = dbr.tbl_Blok.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
                    PilihPeringkat = new SelectList(SelectPkt3.Select(s => new SelectListItem { Value = s.fld_Blok, Text = s.fld_Blok + " - " + s.fld_NamaBlok }), "Value", "Text").ToList();
                    CdKesukaranMembaja = SelectPkt3.Select(s => s.fld_KesukaranMembajaBlok).Take(1).FirstOrDefault();
                    CdKesukaranMenuai = SelectPkt3.Select(s => s.fld_KesukaranMenuaiBlok).Take(1).FirstOrDefault();
                    hargaKesukaranMembaja = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "KesukaranMembaja" && x.fldOptConfValue == CdKesukaranMembaja && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fldOptConfFlag2).FirstOrDefault(); ;
                    hargaKesukaranMenuai = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "KesukaranMenuai" && x.fldOptConfValue == CdKesukaranMenuai && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fldOptConfFlag2).FirstOrDefault();
                    break;
            }
            dbr.Dispose();
            return Json(new { PilihPeringkat, hargaKesukaranMembaja, hargaKesukaranMenuai, CdKesukaranMembaja, CdKesukaranMenuai });
        }

        public JsonResult GetPlhnPkt(string PilihanPkt, int JnisPkt)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            string hargaKesukaranMembaja = "";
            string hargaKesukaranMenuai = "";
            string CdKesukaranMembaja = "";
            string CdKesukaranMenuai = "";
            switch (JnisPkt)
            {
                case 1:
                    var SelectPkt = dbr.tbl_PktUtama.Where(x => x.fld_PktUtama == PilihanPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
                    CdKesukaranMembaja = SelectPkt.Select(s => s.fld_KesukaranMembajaPktUtama).Take(1).FirstOrDefault();
                    CdKesukaranMenuai = SelectPkt.Select(s => s.fld_KesukaranMenuaiPktUtama).Take(1).FirstOrDefault();
                    hargaKesukaranMembaja = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "KesukaranMembaja" && x.fldOptConfValue == CdKesukaranMembaja && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fldOptConfFlag2).FirstOrDefault(); ;
                    hargaKesukaranMenuai = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "KesukaranMenuai" && x.fldOptConfValue == CdKesukaranMenuai && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fldOptConfFlag2).FirstOrDefault();
                    break;
                case 2:
                    var SelectPkt2 = dbr.tbl_SubPkt.Where(x => x.fld_Pkt == PilihanPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
                    CdKesukaranMembaja = SelectPkt2.Select(s => s.fld_KesukaranMembajaPkt).Take(1).FirstOrDefault();
                    CdKesukaranMenuai = SelectPkt2.Select(s => s.fld_KesukaranMenuaiPkt).Take(1).FirstOrDefault();
                    hargaKesukaranMembaja = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "KesukaranMembaja" && x.fldOptConfValue == CdKesukaranMembaja && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fldOptConfFlag2).FirstOrDefault(); ;
                    hargaKesukaranMenuai = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "KesukaranMenuai" && x.fldOptConfValue == CdKesukaranMenuai && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fldOptConfFlag2).FirstOrDefault();
                    break;
                case 3:
                    var SelectPkt3 = dbr.tbl_Blok.Where(x => x.fld_Blok == PilihanPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
                    CdKesukaranMembaja = SelectPkt3.Select(s => s.fld_KesukaranMembajaBlok).Take(1).FirstOrDefault();
                    CdKesukaranMenuai = SelectPkt3.Select(s => s.fld_KesukaranMenuaiBlok).Take(1).FirstOrDefault();
                    hargaKesukaranMembaja = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "KesukaranMembaja" && x.fldOptConfValue == CdKesukaranMembaja && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fldOptConfFlag2).FirstOrDefault(); ;
                    hargaKesukaranMenuai = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "KesukaranMenuai" && x.fldOptConfValue == CdKesukaranMenuai && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fldOptConfFlag2).FirstOrDefault();
                    break;
            }
            dbr.Dispose();
            return Json(new { hargaKesukaranMembaja, hargaKesukaranMenuai, CdKesukaranMembaja, CdKesukaranMenuai });
        }

        public JsonResult GetActvtType(string Lejar)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<SelectListItem> JnisAktvt = new List<SelectListItem>();
            List<SelectListItem> PilihanAktvt = new List<SelectListItem>();
            var JnisAktvtList = db.tbl_JenisAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false && x.fld_Lejar == Lejar).OrderBy(o => o.fld_KodJnsAktvt).ToList();
            string SelectJnisActvt = JnisAktvtList.Select(s => s.fld_KodJnsAktvt).Take(1).FirstOrDefault();
            JnisAktvt = new SelectList(JnisAktvtList.Select(s => new SelectListItem { Value = s.fld_KodJnsAktvt, Text = s.fld_Desc }), "Value", "Text").ToList();
            var tbl_UpahAktiviti = db.tbl_UpahAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJenisAktvt == SelectJnisActvt && x.fld_Deleted == false).ToList();
            PilihanAktvt = new SelectList(tbl_UpahAktiviti.OrderBy(o => o.fld_KodAktvt).Select(s => new SelectListItem { Value = s.fld_KodAktvt, Text = s.fld_KodAktvt }), "Value", "Text").ToList();
            PilihanAktvt.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            db.Dispose();
            return Json(new { JnisAktvt, PilihanAktvt });
        }

        public JsonResult GetAktvt(string JnisAktvt)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<SelectListItem> PilihAktiviti = new List<SelectListItem>();
            var tbl_UpahAktiviti = db.tbl_UpahAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJenisAktvt == JnisAktvt && x.fld_Deleted == false).ToList();
            PilihAktiviti = new SelectList(tbl_UpahAktiviti.OrderBy(o => o.fld_KodAktvt).Select(s => new SelectListItem { Value = s.fld_KodAktvt, Text = s.fld_KodAktvt }), "Value", "Text").ToList();
            PilihAktiviti.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            var AktivitiToolTip = tbl_UpahAktiviti.OrderBy(o => o.fld_KodAktvt).Select(s => new { Label = s.fld_KodAktvt + " - " + s.fld_Desc + " - RM" + s.fld_Harga }).ToList();
            var JenisAktiviti = db.tbl_JenisAktiviti.Where(x => x.fld_DisabledFlag != 5 && x.fld_KodJnsAktvt == JnisAktvt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).FirstOrDefault();
            string Lain2JnsAtvt = "";
            if (JenisAktiviti != null)
            {
                Lain2JnsAtvt = JenisAktiviti.fld_Desc;
            }
            else
            {
                Lain2JnsAtvt = "-";
            }
            db.Dispose();
            return Json(new { PilihAktiviti, Lain2JnsAtvt, AktivitiToolTip });
        }

        public JsonResult GetAktvt2()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<SelectListItem> PilihanAktvtHT = new List<SelectListItem>();
            var getJenisActvtDetails = db.tbl_JenisAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_DisabledFlag == 3 && x.fld_Deleted == false).FirstOrDefault();
            PilihanAktvtHT = new SelectList(db.tbl_UpahAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJenisAktvt == getJenisActvtDetails.fld_KodJnsAktvt && x.fld_Deleted == false).OrderBy(o => o.fld_KodAktvt).Select(s => new SelectListItem { Value = s.fld_KodAktvt, Text = s.fld_KodAktvt }), "Value", "Text").ToList();
            PilihanAktvtHT.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            var AktivitiToolTip = db.tbl_UpahAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJenisAktvt == getJenisActvtDetails.fld_KodJnsAktvt && x.fld_Deleted == false).OrderBy(o => o.fld_KodAktvt).Select(s => new { Label = s.fld_KodAktvt + " - " + s.fld_Desc + " - RM" + s.fld_Harga }).ToList();

            db.Dispose();
            return Json(new { PilihanAktvtHT, AktivitiToolTip });
        }

        public JsonResult GetGLSAP(byte JnisPkt, string PilihanPkt, string PilihanAktvt)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            int? getuserid = getidentity.ID(User.Identity.Name);
            string msg = "";
            string statusmsg = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            string GLCode = "";

            if (EstateFunction.CheckSAPGLMap(dbr, JnisPkt, PilihanPkt, PilihanAktvt, NegaraID, SyarikatID, WilayahID, LadangID, false, "-", out GLCode))
            {

            }
            else
            {
                msg = "Kod GL tidak dijumpai untuk aktiviti ini.";
                statusmsg = "warning";
            }

            dbr.Dispose();
            return Json(new { msg, statusmsg, GLCode });
        }

        public JsonResult GetMenuaiRate(DateTime SelectDate, int JnisPkt, string PilihanPkt, string kdhmnuai)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            decimal? kadarharga = 0;
            string msg = "";
            string statusmsg = "";
            bool closeform = true;
            bool YieldBracketFullMonth = true;

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            kadarharga = EstateFunction.SpecialRateTable(SelectDate, JnisPkt, PilihanPkt, kdhmnuai, dbr, NegaraID, SyarikatID, WilayahID, LadangID, out YieldBracketFullMonth, 1);
            if (kadarharga != 0)
            {
                msg = GlobalResEstate.msgWorkInfo;
                statusmsg = "success";
                closeform = false;
            }
            else
            {
                if (!YieldBracketFullMonth)
                {
                    msg = GlobalResEstate.msgYieldBracket;
                }
                else
                {
                    msg = GlobalResEstate.msgErrorData;
                }
                statusmsg = "warning";
                closeform = true;
            }
            return Json(new { kadarharga = string.Format("{0:0.00}", kadarharga), msg = msg, statusmsg = statusmsg, closeform = closeform });

        }

        public JsonResult CheckValidKwsnSkr(string JnisAktvt)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            string Result = "";

            var GetValidation = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kwsnskr" && x.fldOptConfValue == JnisAktvt && x.fldDeleted == false).FirstOrDefault();

            Result = GetValidation != null ? GetValidation.fldOptConfFlag2 : "3";

            return Json(Result);
        }

        public JsonResult GetForm(int SelectionCategory, string SelectionData, byte JnisPkt, string PilihanPkt, string JnisAktvt, string KodAktvt, DateTime SelectDate)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string keteranganhdr, statushdr;
            string bodyview = "";
            short KadarByrn = 0;
            decimal? kadarharga = 0;
            string msg = "";
            string statusmsg = "";
            bool closeform = true;
            bool YieldBracketFullMonth = true;
            string GLCode = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            List<CustMod_AttWork> CustMod_AttWorkList = new List<CustMod_AttWork>();

            if (EstateFunction.CheckSAPGLMap(dbr, JnisPkt, PilihanPkt, KodAktvt, NegaraID, SyarikatID, WilayahID, LadangID, false, "-", out GLCode))
            {
                var tbl_JenisAktiviti = db.tbl_JenisAktiviti.Join(db.tbl_UpahAktiviti,
                    j => new { j.fld_NegaraID, j.fld_SyarikatID, KodJnsAktvt = j.fld_KodJnsAktvt },
                    k => new { k.fld_NegaraID, k.fld_SyarikatID, KodJnsAktvt = k.fld_KodJenisAktvt },
                    (j, k) => new {
                        k.fld_NegaraID,
                        k.fld_SyarikatID,
                        j.fld_KodJnsAktvt,
                        k.fld_DisabledFlag,
                        k.fld_Harga,
                        k.fld_KodAktvt,
                        k.fld_KdhByr,
                        k.fld_Unit,
                        j.fld_Deleted,
                        k.fld_MaxProduktiviti
                    })
                        .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJnsAktvt == JnisAktvt && x.fld_KodAktvt == KodAktvt && x.fld_Deleted == false).FirstOrDefault();
                //tbl_JenisAktiviti.fld_DisabledFlag - 1 = kong box & kualiti box xde, 2 = kong je xde, 3 = kong je kluar
                kadarharga = tbl_JenisAktiviti.fld_KdhByr == "B" ? tbl_JenisAktiviti.fld_Harga : EstateFunction.SpecialRateTable(SelectDate, JnisPkt, PilihanPkt, "A", dbr, NegaraID, SyarikatID, WilayahID, LadangID, out YieldBracketFullMonth, 1);
                if (kadarharga != 0)
                {
                    if (SelectionCategory == 1)
                    {
                        var checkatts = dbr.tbl_Kerjahdr.Join(dbr.tbl_Pkjmast, j => new { j.fld_Nopkj, j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID }, k => new { k.fld_Nopkj, k.fld_NegaraID, k.fld_SyarikatID, k.fld_WilayahID, k.fld_LadangID }, (j, k) => new { j.fld_Kum, j.fld_Tarikh, j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, k.fld_Nama, j.fld_Nopkj, j.fld_Kdhdct }).Where(x => x.fld_Kum == SelectionData && x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
                        foreach (var checkatt in checkatts)
                        {
                            GetConfig.GetCutiDesc(checkatt.fld_Kdhdct, "cuti", out keteranganhdr, out statushdr, out KadarByrn, NegaraID, SyarikatID);
                            CustMod_AttWorkList.Add(new CustMod_AttWork() { Nopkj = checkatt.fld_Nopkj, Namapkj = checkatt.fld_Nama, Keteranganhdr = keteranganhdr, statushdr = statushdr, disabletextbox = tbl_JenisAktiviti.fld_DisabledFlag, Kadar = kadarharga, KadarByrn = KadarByrn, KdhByr = tbl_JenisAktiviti.fld_KdhByr, Unit = tbl_JenisAktiviti.fld_Unit, MaximumHsl = tbl_JenisAktiviti.fld_MaxProduktiviti });
                        }
                    }
                    else
                    {
                        var checkatt = dbr.tbl_Kerjahdr.Join(dbr.tbl_Pkjmast, j => new { j.fld_Nopkj, j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID }, k => new { k.fld_Nopkj, k.fld_NegaraID, k.fld_SyarikatID, k.fld_WilayahID, k.fld_LadangID }, (j, k) => new { j.fld_Kum, j.fld_Tarikh, j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, k.fld_Nama, j.fld_Nopkj, j.fld_Kdhdct }).Where(x => x.fld_Nopkj == SelectionData && x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).FirstOrDefault();
                        GetConfig.GetCutiDesc(checkatt.fld_Kdhdct, "cuti", out keteranganhdr, out statushdr, out KadarByrn, NegaraID, SyarikatID);
                        CustMod_AttWorkList.Add(new CustMod_AttWork() { Nopkj = checkatt.fld_Nopkj, Namapkj = checkatt.fld_Nama, Keteranganhdr = keteranganhdr, statushdr = statushdr, disabletextbox = tbl_JenisAktiviti.fld_DisabledFlag, Kadar = kadarharga, KadarByrn = KadarByrn, KdhByr = tbl_JenisAktiviti.fld_KdhByr, Unit = tbl_JenisAktiviti.fld_Unit, MaximumHsl = tbl_JenisAktiviti.fld_MaxProduktiviti });
                    }
                    msg = GlobalResEstate.msgWorkInfo;
                    statusmsg = "success";
                    closeform = false;
                }
                else
                {
                    if (!YieldBracketFullMonth)
                    {
                        msg = GlobalResEstate.msgYieldBracket;
                    }
                    else
                    {
                        msg = GlobalResEstate.msgErrorData;
                    }
                    statusmsg = "warning";
                    closeform = true;
                }

                //var JnisAktvtKod = db.tbl_UpahAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJenisAktvt == JnisAktvt && x.fld_KodAktvt == KodAktvt).FirstOrDefault();
                //JnisAktvtKod.fld_DisabledFlag - 1 = kong box & kualiti box xde, 2 = kong je xde, 3 = kong je kluar
                bodyview = RenderRazorViewToString("WorkingDetailsForm", CustMod_AttWorkList, false);
            }
            else
            {
                msg = "Kod GL tidak dijumpai untuk aktiviti ini.";
                statusmsg = "warning";
                closeform = true;
            }

            dbr.Dispose();
            return Json(new { tablelisting = bodyview, msg, statusmsg, closeform, GLCode });
        }

        public string RenderRazorViewToString(string viewname, object dataview, int? NegaraID, int? SyarikatID, bool CutOfDateStatus)
        {
            ViewData.Model = dataview;
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.CutOfDateStatus = CutOfDateStatus;
            using (var sw = new System.IO.StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewname);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        public string RenderRazorViewToString(string viewname, object dataview, bool CutOfDateStatus)
        {
            ViewData.Model = dataview;
            ViewBag.CutOfDateStatus = CutOfDateStatus;
            using (var sw = new System.IO.StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewname);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
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
        public ActionResult Work()
        {
            return View();
        }
    }
}